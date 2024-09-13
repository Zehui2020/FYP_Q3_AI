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

    private readonly int IdleAnim = Animator.StringToHash("EnemyIdle");
    private readonly int RunAnim = Animator.StringToHash("EnemyRun");
    private readonly int AttackAnim = Animator.StringToHash("EnemyAttack");
    private readonly int DieAnim = Animator.StringToHash("EnemyDie");

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

        if (currentState != State.Attack)
            UpdatePlayerDirection();
    }

    public override void TakeDamage(float damage, int critRate, float critMultiplier, Vector3 closestPoint)
    {
        base.TakeDamage(damage, critRate, critMultiplier, closestPoint);

        if (health <= 0)
        {
            ChangeState(State.Die);
        }
    }
}