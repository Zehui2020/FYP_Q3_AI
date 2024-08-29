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
        Vector2 dir = (PlayerController.Instance.transform.position - transform.position).normalized;

        RaycastHit2D[] hits = Physics2D.RaycastAll(jumpCheckPos.position, dir, ~enemyLayer, ~playerLayer);

        if (hits.Length == 0)
            return;

        Vector3 targetPoint = hits[hits.Length - 1].point;
    }

    private IEnumerator DoJumpRoutine(Vector3 targetPos)
    {

    }
}