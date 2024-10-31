using System.Collections;
using UnityEngine;

public class Slime : Enemy
{
    public enum State
    {
        Idle,
        Jump,
        Attack,
        Teleport,
        Hurt,
        Die
    }
    public State currentState;

    private readonly int SlimeIdleAnim = Animator.StringToHash("SlimeIdle");
    private readonly int SlimeJumpAnim = Animator.StringToHash("SlimeJump");
    private readonly int SlimeLandAnim = Animator.StringToHash("SlimeLand");
    private readonly int SlimeAttackAnim = Animator.StringToHash("SlimeAttack");
    private readonly int SlimeHurt = Animator.StringToHash("SlimeHurt");
    private readonly int SlimeDieAnim = Animator.StringToHash("SlimeDie");

    private readonly int SlimeTeleportStart = Animator.StringToHash("SlimeTeleportStart");
    private readonly int SlimeTeleportEnd = Animator.StringToHash("SlimeTeleportEnd");

    [Header("Slime Stats")]
    [SerializeField] private Vector2 jumpForce;
    [SerializeField] private float jumpAngle;
    [SerializeField] private float patrolJumpInterval;
    [SerializeField] private float chaseJumpInterval;

    private bool isPatroling = true;
    private bool isJumping = false;

    private Coroutine JumpRoutine;

    public override void InitializeEnemy()
    {
        base.InitializeEnemy();
        aiNavigation.SetTarget(waypoints[currentWaypoint]);

        ChangeState(State.Idle);

        onPlayerInChaseRange += () => { isPatroling = false; aiNavigation.SetTarget(player.transform); };
        onHitEvent += (target, damage, crit, pos) => { if (CheckHurt()) ChangeState(State.Hurt); };
        OnBreached += (multiplier) => { animator.Play(SlimeIdleAnim); };
        OnParry += (stat) => { animator.Play(SlimeIdleAnim); };
    }

    public override void OnDie()
    {
        base.OnDie();
        ChangeState(State.Die);
    }

    private void ChangeState(State newState)
    {
        if (isFrozen || !canUpdate || currentState == State.Die)
            return;

        currentState = newState;

        switch (newState)
        {
            case State.Idle:
                animator.CrossFade(SlimeIdleAnim, 0f);

                if (isPatroling)
                    JumpRoutine = StartCoroutine(JumpCooldown(patrolJumpInterval));
                else
                    JumpRoutine = StartCoroutine(JumpCooldown(chaseJumpInterval));

                break;
            case State.Jump:
                UpdateDirectionToPlayer();
                animator.CrossFade(SlimeJumpAnim, 0f);
                break;
            case State.Attack:
                UpdateDirectionToPlayer();
                animator.Play(SlimeAttackAnim, -1, 0f);
                break;
            case State.Teleport:
                UpdateDirectionToPlayer();
                animator.CrossFade(SlimeTeleportStart, 0f);
                break;
            case State.Hurt:
                animator.CrossFade(SlimeHurt, 0f);
                break;
            case State.Die:
                animator.speed = 1;
                animator.CrossFade(SlimeDieAnim, 0f);
                uiController.SetCanvasActive(false);
                break;
        }
    }

    public override void UpdateEnemy()
    {
        base.UpdateEnemy();

        if (isPatroling)
        {
            CheckChasePlayer();
            if (aiNavigation.IsNearTarget(patrolThreshold))
            {
                currentWaypoint++;
                if (currentWaypoint > waypoints.Count - 1)
                    currentWaypoint = 0;

                aiNavigation.SetTarget(waypoints[currentWaypoint]);
            }
        }

        if (currentState == State.Idle)
        {
            UpdateMovementDirection();
            if (isPatroling)
                return;

            if (!AttackDecision())
                return;

            if (JumpRoutine != null)
                StopCoroutine(JumpRoutine);
        }

        if (currentState == State.Jump && isJumping)
        {
            if (!Physics2D.Raycast(transform.position, Vector2.down, 2f, groundLayer) || enemyRB.velocity.y > 0)
                return;

            isJumping = false;
            animator.CrossFade(SlimeLandAnim, 0f);
        }
    }

    private bool AttackDecision()
    {
        if (Physics2D.OverlapCircle(transform.position, attackRange, playerLayer))
        {
            ChangeState(State.Attack);
            return true;
        }

        if (!Physics2D.OverlapCircle(transform.position, chaseRange, playerLayer) && player.IsGrounded())
        {
            ChangeState(State.Teleport);
            return true;
        }

        ChangeState(State.Idle);

        return false;
    }

    private IEnumerator JumpCooldown(float cooldown)
    {
        yield return new WaitForSeconds(cooldown);

        ChangeState(State.Jump);
    }

    public void Teleport()
    {
        transform.position = player.transform.position;
        animator.CrossFade(SlimeTeleportEnd, 0f);
    }

    public void Jump()
    {
        isJumping = true;
        float angleInRadians = jumpAngle * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians)).normalized;

        direction = transform.position.x > player.transform.position.x ? new Vector2(-direction.x, direction.y) : new Vector2(direction.x, direction.y);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up, 100f, groundLayer);

        if (hit)
        {
            float distanceToCeiling = hit.distance;
            float maxSafeJumpHeight = Mathf.Sqrt(2 * Mathf.Abs(Physics2D.gravity.y) * distanceToCeiling);
            float maxYVelocity = Mathf.Min(maxSafeJumpHeight, direction.y * jumpForce.y);
            direction = new Vector2(direction.x, maxYVelocity / jumpForce.y);
        }

        // Apply force to Rigidbody2D
        enemyRB.AddForce(direction * jumpForce, ForceMode2D.Impulse);
    }
}