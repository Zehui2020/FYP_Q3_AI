using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MeleeEnemy : Enemy
{
    public enum State
    {
        Idle,
        Patrol,
        Chase,
        Attack,
        Die
    }
    public State currentState;

    [SerializeField] private Transform jumpCheckPos;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float jumpThreshold;
    [SerializeField] private LayerMask playerLayer;
    private Coroutine jumpRoutine;
    private Vector3 lastSeenPos = Vector3.zero;

    public GameObject marker;

    private readonly int IdleAnim = Animator.StringToHash("EnemyIdle");
    private readonly int RunAnim = Animator.StringToHash("EnemyRun");
    private readonly int AttackAnim = Animator.StringToHash("EnemyAttack");
    private readonly int DieAnim = Animator.StringToHash("EnemyIdle");

    public override void InitializeEnemy()
    {
        base.InitializeEnemy();

        ChangeState(State.Patrol);

        onReachWaypoint += () => { ChangeState(State.Idle); };
        onFinishIdle += () => { ChangeState(State.Patrol); };
        onPlayerInChaseRange += () => { ChangeState(State.Chase); };
        onReachChaseTarget += () => { ChangeState(State.Attack); };
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
            case State.Chase:
                animator.CrossFade(RunAnim, 0f);
                StopIdleRoutine();
                break;
            case State.Attack:
                animator.CrossFade(AttackAnim, 0f);
                aiNavigation.StopNavigation();
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
                    break;

                PatrolUpdate();
                break;
            case State.Chase:
                if (jumpRoutine != null)
                    return;

                JumpCheck();
                ChaseUpdate();
                break;
            case State.Attack:
                break;
            case State.Die:
                break;
        }
    }

    private void JumpCheck()
    {
        Collider2D hit = Physics2D.OverlapCircle(jumpCheckPos.position, chaseRange, playerLayer);

        if (!hit && lastSeenPos == Vector3.zero)
            lastSeenPos = player.transform.position;

        if (Mathf.Abs(Mathf.Abs(transform.position.x) - Mathf.Abs(lastSeenPos.x)) > jumpThreshold ||
            lastSeenPos == Vector3.zero)
            return;

        Instantiate(marker, lastSeenPos, Quaternion.identity);
        jumpRoutine = StartCoroutine(DoJumpRoutine(lastSeenPos));

        return;
    }

    private IEnumerator DoJumpRoutine(Vector3 targetPos)
    {
        aiNavigation.StopNavigation();
        enemyCol.isTrigger = true;
        enemyRB.isKinematic = true;

        float dist = 100f;

        while (dist > 1f)
        {
            dist = Vector2.Distance(transform.position, targetPos);
            transform.position = Vector3.Slerp(transform.position, targetPos, Time.deltaTime);

            yield return null;
        }

        jumpRoutine = null;
        lastSeenPos = Vector3.zero;
        aiNavigation.ResumeNavigation();
        enemyCol.isTrigger = false;
        enemyRB.isKinematic = false;
    }
}