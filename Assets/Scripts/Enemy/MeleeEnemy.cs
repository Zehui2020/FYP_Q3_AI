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
    [SerializeField] protected LayerMask jumpCheckLayers;
    private Coroutine jumpRoutine;

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
                    break;

                JumpCheck();

                if (jumpRoutine == null)
                    ChaseUpdate();
                break;
            case State.Attack:
                break;
            case State.Die:
                break;
        }
    }

    private bool JumpCheck()
    {
        if (jumpRoutine != null)
            return true;

        Vector2 dir = (PlayerController.Instance.transform.position - transform.position).normalized;
        float dist = Vector2.Distance(transform.position, PlayerController.Instance.transform.position);
        RaycastHit2D[] hits = Physics2D.RaycastAll(jumpCheckPos.position, dir, dist, jumpCheckLayers);

        Debug.DrawLine(jumpCheckPos.position, dir, Color.red);

        if (hits.Length == 0)
            return false;

        Debug.Log(hits[0].collider.name);

        Vector3 targetPoint = hits[hits.Length - 1].point;
        jumpRoutine = StartCoroutine(DoJumpRoutine(targetPoint));
        return true;
    }

    private IEnumerator DoJumpRoutine(Vector3 targetPos)
    {
        aiNavigation.StopNavigation();
        enemyCol.isTrigger = true;

        Vector3 jumpDist = (transform.position - PlayerController.Instance.transform.position) / 3f;
        Vector3 peakPoint = new Vector3(jumpDist.x, transform.position.y + jumpHeight, transform.position.z);

        float dist = 100f;

        while (dist > 1f)
        {
            dist = Vector2.Distance(transform.position, targetPos);
            transform.position = BezierCurve(peakPoint, targetPos) / 2f;

            yield return null;
        }

        jumpRoutine = null;
        aiNavigation.ResumeNavigation();
        enemyCol.isTrigger = false;
    }

    private Vector3 BezierCurve(Vector3 peakPos, Vector3 targetPos)
    {
        return Mathf.Pow((1 - Time.deltaTime), 2) * transform.position + 2 * (1 - Time.deltaTime) * Time.deltaTime * peakPos + Mathf.Pow(Time.deltaTime, 2) * targetPos;
    }
}