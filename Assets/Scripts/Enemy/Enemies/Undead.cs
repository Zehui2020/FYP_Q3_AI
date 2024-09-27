using System.Collections;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.VFX;

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
        OnBreached += (multiplier) => { ChangeState(State.Idle); };

        onHitEvent += (target, damage, crit, pos) => { if (CheckHurt()) ChangeState(State.Hurt); };
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
                animator.CrossFade(WalkAnim, 0f);
                break;
            case State.Chase:
                animator.CrossFade(WalkAnim, 0f);
                break;
            case State.Attack:
                aiNavigation.StopNavigation();
                animator.Play(AttackAnim, -1, 0f);
                UpdateMovementDirection();
                break;
            case State.Teleport:
                UpdateMovementDirection();
                aiNavigation.StopNavigation();

                animator.Play(TeleportAnim, -1, 0f);

                if (Physics2D.Raycast(transform.position, player.transform.position, 100f, groundLayer))
                    transform.position = player.transform.position;
                else
                    transform.position = new Vector3(player.transform.position.x, transform.position.y, transform.position.z);

                StartCoroutine(TeleportCooldown(1f));

                break;
            case State.Hurt:
                aiNavigation.StopNavigation();
                animator.Play(HurtAnim, -1, 0f);
                ChangeState(State.Idle);
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
            case State.Chase:
                aiNavigation.SetPathfindingTarget(player.transform, chaseMovementSpeed, true);
                if (Vector2.Distance(player.transform.position, transform.position) <= attackRange)
                    ChangeState(State.Attack);

                if (Vector2.Distance(player.transform.position, transform.position) >= teleportThreshold && canTeleport)
                    ChangeState(State.Teleport);
                break;
        }

        if (currentState != State.Attack &&
            currentState != State.Teleport)
            UpdateMovementDirection();
    }

    private IEnumerator TeleportCooldown(float delay)
    {
        canTeleport = false;

        yield return new WaitForSeconds(teleportCooldown + delay);

        canTeleport = true;
    }
}