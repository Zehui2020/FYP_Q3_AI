using System.Collections;
using UnityEngine;

public class Undead : Enemy
{
    public enum State
    {
        Idle,
        Patrol,
        Chase,
        Attack,
        Teleport,
        Hurt,
        Die
    }
    public State currentState;

    private readonly int IdleAnim = Animator.StringToHash("UndeadIdle");
    private readonly int WalkAnim = Animator.StringToHash("UndeadWalk");
    private readonly int AttackAnim = Animator.StringToHash("UndeadAttack");
    private readonly int TeleportAnim = Animator.StringToHash("UndeadTeleport");
    private readonly int HurtAnim = Animator.StringToHash("UndeadHurt");
    private readonly int DieAnim = Animator.StringToHash("UndeadDie");

    [SerializeField] private float teleportThreshold;
    [SerializeField] private float teleportCooldown;
    private bool canTeleport = true;

    public override void InitializeEnemy()
    {
        base.InitializeEnemy();

        ChangeState(State.Patrol);

        onReachWaypoint += () => { ChangeState(State.Idle); };
        onFinishIdle += () => { ChangeState(State.Patrol); };
        onPlayerInChaseRange += () => { ChangeState(State.Chase); };
        OnDieEvent += (target) => { ChangeState(State.Die); };
        OnBreached += (multiplier) => { ChangeState(State.Hurt); };
        onHitEvent += (target, damage, crit, pos) => { if (CheckHurt()) ChangeState(State.Hurt); };
    }

    private void ChangeState(State newState)
    {
        if (currentState == State.Die)
            return;

        currentState = newState;

        switch (currentState)
        {
            case State.Idle:
                animator.Play(IdleAnim, -1, 0);
                aiNavigation.StopNavigation();
                Idle();
                break;
            case State.Patrol:
                animator.Play(WalkAnim, -1, 0);
                break;
            case State.Chase:
                animator.Play(WalkAnim, -1, 0);
                break;
            case State.Attack:
                animator.Play(AttackAnim, -1, 0);
                aiNavigation.StopNavigation();
                break;
            case State.Teleport:
                animator.Play(TeleportAnim, -1, 0);
                aiNavigation.StopNavigation();

                if (Physics2D.Raycast(transform.position, (player.transform.position - transform.position).normalized, 100f, groundLayer))
                    transform.position = player.transform.position;
                else
                    transform.position = new Vector3(player.transform.position.x, transform.position.y, transform.position.z);

                StartCoroutine(TeleportCooldown(0));

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
                UpdateMovementDirection();
                break;
            case State.Patrol:
                if (CheckChasePlayer())
                    return;

                PatrolUpdate();
                UpdateMovementDirection();
                break;
            case State.Chase:
                aiNavigation.SetPathfindingTarget(player.transform, chaseMovementSpeed, true);
                if (Vector2.Distance(player.transform.position, transform.position) <= attackRange)
                    ChangeState(State.Attack);

                if (Vector2.Distance(player.transform.position, transform.position) >= teleportThreshold && canTeleport && player.IsGrounded())
                    ChangeState(State.Teleport);

                UpdateMovementDirection();
                break;
        }            
    }

    private IEnumerator TeleportCooldown(float delay)
    {
        canTeleport = false;

        yield return new WaitForSeconds(teleportCooldown + delay);

        canTeleport = true;
    }
}