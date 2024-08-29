using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : EnemyStats
{
    public enum EnemyType { Normal, Elite, Boss }
    public EnemyType enemyType;

    protected AINavigation aiNavigation;
    protected Collider2D enemyCol;
    [SerializeField] protected Animator animator;

    [SerializeField] protected List<Transform> waypoints = new();
    protected int currentWaypoint = 0;

    protected event System.Action onReachWaypoint;
    protected event System.Action onFinishIdle;
    protected event System.Action onPlayerInChaseRange;
    protected event System.Action onReachChaseTarget;

    private Coroutine idleRoutine;

    private void Start()
    {
        InitializeEnemy();
    }

    public virtual void InitializeEnemy()
    {
        aiNavigation = GetComponent<AINavigation>();
        enemyCol = GetComponent<Collider2D>();

        aiNavigation.InitPathfindingAgent();
    }

    public virtual void UpdateEnemy()
    {
        UpdatePlayerDirection();
    }

    private void Update()
    {
        UpdateEnemy();
    }

    protected void PatrolUpdate()
    {
        aiNavigation.SetPathfindingTarget(waypoints[currentWaypoint], patrolMovementSpeed, true);

        if (Mathf.Abs(Vector2.Distance(transform.position, waypoints[currentWaypoint].position)) > patrolThreshold)
            return;

        currentWaypoint++;
        if (currentWaypoint > waypoints.Count - 1)
            currentWaypoint = 0;

        onReachWaypoint?.Invoke();
    }

    protected void ChaseUpdate()
    {
        StopIdleRoutine();
        aiNavigation.SetPathfindingTarget(PlayerController.Instance.transform, chaseMovementSpeed, true);

        if (Mathf.Abs(Vector2.Distance(transform.position, waypoints[currentWaypoint].position)) > attackRange)
            return;

        onReachChaseTarget?.Invoke();
    }

    protected bool CheckChasePlayer()
    {
        if (Mathf.Abs(Vector2.Distance(transform.position, PlayerController.Instance.transform.position)) > chaseRange)
            return false;

        onPlayerInChaseRange?.Invoke();
        return true;
    }

    protected void StopIdleRoutine()
    {
        if (idleRoutine != null)
            StopCoroutine(idleRoutine);
    }

    protected void Idle()
    {
        StopIdleRoutine();
        idleRoutine = StartCoroutine(IdleRoutine());
    }

    protected IEnumerator IdleRoutine()
    {
        aiNavigation.StopNavigation();

        yield return new WaitForSeconds(idleDuration);

        aiNavigation.ResumeNavigation();
        onFinishIdle?.Invoke();
        idleRoutine = null;
    }

    protected void UpdatePlayerDirection()
    {
        if (aiNavigation.GetMovementDirection() == Vector2.right)
            transform.localScale = Vector3.one;
        else
            transform.localScale = new Vector3(-1, 1, 1);
    }

    private void OnDisable()
    {
        onReachWaypoint = null;
        onFinishIdle = null;
        onPlayerInChaseRange = null;
        onReachChaseTarget = null;
    }
}