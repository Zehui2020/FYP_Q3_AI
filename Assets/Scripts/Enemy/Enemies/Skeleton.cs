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
    [SerializeField] private AudioProxy audioProxy;
    [SerializeField] private Vector2 lungeForce;
    [SerializeField] private float lungeAngle;
    [SerializeField] private float scratchRange;
    private bool isLunging = false;

    public override void InitializeEnemy()
    {
        base.InitializeEnemy();

        ChangeState(State.Patrol);

        onReachWaypoint += () => { ChangeState(State.Idle); };
        onFinishIdle += () => { ChangeState(State.Patrol); };
        onPlayerInChaseRange += () => { ChangeState(State.Deciding); };
        OnBreached += (multiplier) => { animator.Play(IdleAnim); };
        OnParry += (stat) => { animator.Play(IdleAnim); };
        onHitEvent += (target, damage, crit, pos) => { if (CheckHurt()) ChangeState(State.Hurt); };
    }

    public override void OnDie()
    {
        base.OnDie();
        ChangeState(State.Die);
    }

    private void ChangeState(State newState)
    {
        if (currentState == State.Die)
            return;

        if (health <= 0 && newState != State.Die)
            return;

        currentState = newState;

        switch (newState)
        {
            case State.Idle:
                animator.Play(IdleAnim, -1, 0);
                aiNavigation.StopNavigation();
                Idle();
                break;
            case State.Patrol:
                isInCombat = false;
                aiNavigation.ResumeNavigation();
                animator.Play(RunAnim, -1, 0);
                break;
            case State.Deciding:
                animator.Play(IdleAnim, -1, 0);
                AttackDecision();
                aiNavigation.StopNavigation();
                break;
            case State.Scratch:
                UpdateDirectionToPlayer();
                animator.Play(ScratchAnim, -1, 0);
                aiNavigation.StopNavigation();
                break;
            case State.Lunge:
                animator.Play(LungeAnim, -1, 0);
                aiNavigation.StopNavigation();
                break;
            case State.Land:
                animator.Play(LandAnim, -1, 0);
                aiNavigation.StopNavigation();
                break;
            case State.Hurt:
                animator.Play(HurtAnim, -1, 0);
                aiNavigation.StopNavigation();
                break;
            case State.Die:
                animator.Play(DieAnim, -1, 0);
                aiNavigation.StopNavigation();
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
                UpdateMovementDirection();
                break;
            case State.Lunge:
                if (!isLunging || enemyRB.velocity.y > 0 || !Physics2D.Raycast(transform.position, Vector2.down, 2f, groundLayer))
                    return;

                ChangeState(State.Land);
                OnDamageEventEnd(0);
                isLunging = false;

                break;
        }
    }

    private void AttackDecision()
    {
        if (transform.position.x < player.transform.position.x)
            transform.localScale = new Vector3(1, 1, 1);
        else
            transform.localScale = new Vector3(-1, 1, 1);

        Vector2 dir = transform.localScale.x < 0 ? -transform.right : transform.right;

        // Lunge direction
        float angleInRadians = lungeAngle * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians)).normalized;
        direction = transform.position.x > player.transform.position.x ? new Vector2(-direction.x, direction.y) : new Vector2(direction.x, direction.y);

        if (Physics2D.Raycast(transform.position, dir.normalized, scratchRange, playerLayer))
            ChangeState(State.Scratch);
        else
            ChangeState(State.Lunge);
    }

    public void Lunge()
    {
        isLunging = true;
        float angleInRadians = lungeAngle * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians)).normalized;

        direction = transform.localScale.x < 0 ? new Vector2(-direction.x, direction.y) : new Vector2(direction.x, direction.y);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 100f, groundLayer);

        if (hit)
        {
            float distanceToCeiling = hit.distance;
            float maxSafeJumpHeight = Mathf.Sqrt(2 * Mathf.Abs(Physics2D.gravity.y) * distanceToCeiling);

            float maxYVelocity = Mathf.Min(maxSafeJumpHeight, direction.y * lungeForce.y);
            direction = new Vector2(direction.x, maxYVelocity / lungeForce.y);
        }

        enemyRB.AddForce(direction * lungeForce, ForceMode2D.Impulse);
    }

    public override void Knockback(float force)
    {
        enemyRB.velocity = Vector2.zero;
        base.Knockback(force);
    }

    public override bool TakeDamage(BaseStats attacker, Damage damage, bool isCrit, Vector3 closestPoint, DamagePopup.DamageType damageType)
    {
        bool tookDamage = base.TakeDamage(attacker, damage, isCrit, closestPoint, damageType);

        if (health <= 0)
            ChangeState(State.Die);

        return tookDamage;
    }

    public void PlayAudioProxy(Sound.SoundName soundName)
    {
        audioProxy.PlayAudioOneShot(soundName);
    }
}