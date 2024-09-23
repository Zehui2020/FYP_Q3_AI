using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class Skeleton : Enemy
{
    public enum State
    {
        Idle,
        Patrol,
        Chase,
        Attack,
        Teleport,
        Die
    }
    public State currentState;

    private readonly int IdleAnim = Animator.StringToHash("SkeletonIdle");
    private readonly int WalkAnim = Animator.StringToHash("SkeletonWalk");
    private readonly int AttackAnim = Animator.StringToHash("SkeletonAttack");
    private readonly int TeleportAnim = Animator.StringToHash("SkeletonTeleport");
    private readonly int DieAnim = Animator.StringToHash("SkeletonDie");

    [SerializeField] private VisualEffect teleportTrail;
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
                UpdatePlayerDirection();
                break;
            case State.Teleport:
                aiNavigation.StopNavigation();
                animator.Play(TeleportAnim, -1, 0f);

                StartCoroutine(TeleportTrail());
                transform.position = player.transform.position;

                StartCoroutine(TeleportCooldown());
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
            UpdatePlayerDirection();
    }

    private IEnumerator TeleportTrail()
    {
        teleportTrail.Reinit();
        teleportTrail.Play();

        yield return new WaitForSeconds(1f);

        teleportTrail.Stop();
    }

    private IEnumerator TeleportCooldown()
    {
        canTeleport = false;

        yield return new WaitForSeconds(teleportCooldown);

        canTeleport = true;
    }
}