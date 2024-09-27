using UnityEngine;

public class Skeleton : Enemy
{
    public enum State
    {
        Idle,
        Patrol,
        Deciding,
        Scratch,
        Lunge,
        Land,
        Hurt,
        Die
    }
    public State currentState;

    private readonly int IdleAnim = Animator.StringToHash("SkeletonIdle");
    private readonly int RunAnim = Animator.StringToHash("SkeletonRun");
    private readonly int ScratchAnim = Animator.StringToHash("SkeletonScratch");
    private readonly int LungeAnim = Animator.StringToHash("SkeletonLunge");
    private readonly int LandAnim = Animator.StringToHash("SkeletonLand");
    private readonly int HurtAnim = Animator.StringToHash("SkeletonHurt");
    private readonly int DieAnim = Animator.StringToHash("SkeletonDie");

    [Header("Skeleton Stats")]
    [SerializeField] private Vector2 lungeForce;
    [SerializeField] private float lungeAngle;
    [SerializeField] private float scratchRange;

    public override void InitializeEnemy()
    {
        base.InitializeEnemy();

        ChangeState(State.Patrol);

        onReachWaypoint += () => { ChangeState(State.Idle); };
        onFinishIdle += () => { ChangeState(State.Patrol); };
        onPlayerInChaseRange += () => { ChangeState(State.Deciding); };
        OnDieEvent += (target) => { ChangeState(State.Die); };
        OnBreached += (multiplier) => { ChangeState(State.Hurt); };
        onHitEvent += (target, damage, crit, pos) => { if (CheckHurt()) ChangeState(State.Hurt); };
    }

    private void ChangeState(State newState)
    {
        if (currentState == State.Die)
            return;

        switch (newState)
        {
            case State.Idle:
                animator.CrossFade(IdleAnim, 0f);
                Idle();
                break;
            case State.Patrol:
                animator.CrossFade(RunAnim, 0f);
                break;
            case State.Deciding:
                aiNavigation.StopNavigation();
                AttackDecision();
                break;
            case State.Scratch:
                aiNavigation.StopNavigation();
                animator.Play(ScratchAnim, -1, 0f);
                break;
            case State.Lunge:
                aiNavigation.StopNavigation();
                animator.Play(LungeAnim, -1, 0f);
                break;
            case State.Land:
                enemyRB.velocity = Vector2.zero;
                UpdateMovementDirection();
                animator.Play(LandAnim, -1, 0f);
                break;
            case State.Hurt:
                aiNavigation.StopNavigation();
                animator.Play(HurtAnim, -1, 0f);
                break;
            case State.Die:
                animator.speed = 1;
                animator.CrossFade(DieAnim, 0f);
                aiNavigation.StopNavigation();
                uiController.SetCanvasActive(false);
                break;
        }
    }

    public override void UpdateEnemy()
    {
        base.UpdateEnemy();

        switch (currentState)
        {
            case State.Idle:
                CheckChasePlayer();
                break;
            case State.Patrol:
                if (CheckChasePlayer())
                    return;

                PatrolUpdate();
                break;
        }

        if (currentState != State.Scratch &&
            currentState != State.Lunge)
            UpdateMovementDirection();
    }

    private void AttackDecision()
    {
        if (transform.position.x < player.transform.position.x)
            transform.localScale = new Vector3(1, 1, 1);
        else
            transform.localScale = new Vector3(-1, 1, 1);

        Vector2 dir = transform.localScale.x < 0 ? -transform.right : transform.right;

        if (Physics2D.Raycast(transform.position, dir.normalized, scratchRange, playerLayer))
            ChangeState(State.Scratch);
        else
            ChangeState(State.Lunge);
    }

    public void Lunge()
    {
        float angleInRadians = lungeAngle * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians)).normalized;
        if (transform.localScale.x < 0)
            direction = new Vector2(-direction.x, direction.y);

        enemyRB.AddForce(direction * lungeForce, ForceMode2D.Impulse);
    }

    public override void Knockback(float initialSpeed, float distance)
    {
        enemyRB.velocity = Vector2.zero;
        base.Knockback(initialSpeed, distance);
    }

    public override bool TakeDamage(BaseStats attacker, Damage damage, bool isCrit, Vector3 closestPoint, DamagePopup.DamageType damageType)
    {
        bool tookDamage = base.TakeDamage(attacker, damage, isCrit, closestPoint, damageType);

        if (health <= 0)
            ChangeState(State.Die);

        return tookDamage;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!Utility.Instance.CheckLayer(collision.gameObject, groundLayer) || currentState != State.Lunge)
            return;

        ChangeState(State.Land);
        OnDamageEventEnd(0);
    }
}