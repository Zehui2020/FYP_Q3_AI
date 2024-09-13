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
        Die
    }
    public State currentState;

    private readonly int SlimeIdleAnim = Animator.StringToHash("SlimeIdle");
    private readonly int SlimeJumpAnim = Animator.StringToHash("SlimeJump");
    private readonly int SlimeLandAnim = Animator.StringToHash("SlimeLand");
    private readonly int SlimeAttackAnim = Animator.StringToHash("SlimeAttack");
    private readonly int SlimeDieAnim = Animator.StringToHash("SlimeDie");

    private readonly int SlimeTeleportStart = Animator.StringToHash("SlimeTeleportStart");
    private readonly int SlimeTeleportEnd = Animator.StringToHash("SlimeTeleportEnd");

    [Header("Slime Stats")]
    [SerializeField] private LayerMask groundLayer;
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
        OnDieEvent += (target) => { ChangeState(State.Die); };
    }

    private void ChangeState(State newState)
    {
        if (isFrozen || !canUpdate)
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
                animator.CrossFade(SlimeJumpAnim, 0f);
                break;
            case State.Attack:
                animator.Play(SlimeAttackAnim, -1, 0f);
                break;
            case State.Teleport:
                animator.CrossFade(SlimeTeleportStart, 0f);
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
            UpdatePlayerDirection();
            if (isPatroling)
                return;

            if (!AttackDecision())
                return;

            if (JumpRoutine != null)
                StopCoroutine(JumpRoutine);
        }
    }

    private bool AttackDecision()
    {
        if (Physics2D.OverlapCircle(transform.position, attackRange, playerLayer))
        {
            ChangeState(State.Attack);
            return true;
        }

        if (!Physics2D.OverlapCircle(transform.position, chaseRange, playerLayer))
        {
            ChangeState(State.Teleport);
            return true;
        }

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
        if (transform.localScale.x < 0)
            direction = new Vector2(-direction.x, direction.y);

        enemyRB.AddForce(direction * jumpForce, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!Utility.Instance.CheckLayer(collision.gameObject, groundLayer) || !isJumping)
            return;

        isJumping = false;
        enemyRB.velocity = Vector2.zero;
        animator.CrossFade(SlimeLandAnim, 0f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}