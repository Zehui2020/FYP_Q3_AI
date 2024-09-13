using UnityEngine;

public class Undead : Enemy
{
    public enum State
    {
        Idle,
        Patrol,
        Deciding,
        Scratch,
        Lunge,
        Die
    }
    public State currentState;

    private readonly int IdleAnim = Animator.StringToHash("UndeadIdle");
    private readonly int RunAnim = Animator.StringToHash("UndeadRun");
    private readonly int ScratchAnim = Animator.StringToHash("UndeadScratch");
    private readonly int LungeAnim = Animator.StringToHash("UndeadLunge");
    private readonly int DieAnim = Animator.StringToHash("UndeadDie");

    [Header("Undead Stats")]
    [SerializeField] private LayerMask groundLayer;
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
    }

    private void ChangeState(State newState)
    {
        currentState = newState;

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
            case State.Die:
                animator.CrossFade(DieAnim, 0f);
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
                break;
        }

        if (currentState != State.Scratch &&
            currentState != State.Lunge &&
            currentState != State.Deciding)
            UpdatePlayerDirection();
    }

    private void AttackDecision()
    {
        if (transform.position.x < player.transform.position.x)
            transform.localScale = new Vector3(1, 1, 1);
        else
            transform.localScale = new Vector3(-1, 1, 1);

        Vector2 dir;
        if (transform.localScale.x < 0)
            dir = -transform.right;
        else
            dir = transform.right;

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

    public override bool TakeDamage(float damage, bool isCrit, Vector3 closestPoint, DamagePopup.DamageType damageType)
    {
        bool tookDamage = base.TakeDamage(damage, isCrit, closestPoint, damageType);

        if (health <= 0)
            ChangeState(State.Die);

        return tookDamage;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!Utility.Instance.CheckLayer(collision.gameObject, groundLayer) || currentState != State.Lunge)
            return;

        enemyRB.velocity = Vector2.zero;
    }
}