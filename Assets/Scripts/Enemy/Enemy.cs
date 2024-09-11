using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : EnemyStats
{
    public enum EnemyClass { Undead, Slime }
    public EnemyClass enemyClass;

    public enum EnemyType { Normal, Elite, Boss }
    public EnemyType enemyType;

    protected PlayerController player;

    public bool canUpdate = true;

    protected AINavigation aiNavigation;
    protected EnemyUIController uiController;
    protected Collider2D enemyCol;
    protected Rigidbody2D enemyRB;
    protected CombatCollisionController collisionController;
    [SerializeField] protected Animator animator;
    [SerializeField] protected LayerMask playerLayer;

    [SerializeField] protected List<Transform> waypoints = new();
    protected int currentWaypoint = 0;

    protected event System.Action onReachWaypoint;
    protected event System.Action onFinishIdle;
    protected event System.Action onPlayerInChaseRange;
    protected event System.Action onReachChaseTarget;

    public event System.Action<BaseStats, Damage, bool, Vector3> onHitEvent;

    private Coroutine idleRoutine;
    private bool isInCombat = false;

    private void Start()
    {
        InitializeEnemy();
    }

    public virtual void InitializeEnemy()
    {
        aiNavigation = GetComponent<AINavigation>();
        enemyCol = GetComponent<Collider2D>();
        enemyRB = GetComponent<Rigidbody2D>();
        collisionController = GetComponent<CombatCollisionController>();
        uiController = GetComponent<EnemyUIController>();
        statusEffectManager = GetComponent<StatusEffectManager>();

        aiNavigation.InitPathfindingAgent();
        uiController.InitUIController(this);
        collisionController.InitCollisionController(this);
        player = PlayerController.Instance;

        statusEffectManager.OnThresholdReached += ApplyStatusState;
        statusEffectManager.OnApplyStatusEffect += ApplyStatusEffect;

        onPlayerInChaseRange += () => { isInCombat = true; uiController.SetCanvasActive(true); };
        OnHealthChanged += (increase, isCrit) => { if (!increase) { isInCombat = true; uiController.SetCanvasActive(true); } uiController.OnHealthChanged(health, maxHealth, increase, isCrit); };
        OnShieldChanged += (increase, isCrit) => { if (!increase) { isInCombat = true; uiController.SetCanvasActive(true); } uiController.OnShieldChanged(shield, maxShield, increase, isCrit); };
        OnBreached += (multiplier) => { statusEffectManager.AddEffectUI(StatusEffectUI.StatusEffectType.Breached, 0); PlayerEffectsController.Instance.HitStop(0.2f); };

        onHitEvent += player.OnHitEnemyEvent;
        OnDieEvent += player.OnEnemyDie;
    }

    private void Update()
    {
        if (!canUpdate)
            return;

        UpdateEnemy();
    }

    public override void TakeTrueDamage(Damage damage)
    {
        base.TakeTrueDamage(damage);
        onHitEvent?.Invoke(this, damage, false, transform.position);
    }
    public override void TakeTrueShieldDamage(Damage damage)
    {
        base.TakeTrueShieldDamage(damage);
        onHitEvent?.Invoke(this, damage, false, transform.position);
    }

    public override bool TakeDamage(BaseStats attacker, Damage damage, bool isCrit, Vector3 closestPoint, DamagePopup.DamageType damageType)
    {
        bool tookDamage = base.TakeDamage(attacker, damage, isCrit, closestPoint, damageType);

        if (tookDamage)
        {
            onHitEvent?.Invoke(this, damage, isCrit, closestPoint);
        }

        return tookDamage;
    }

    public virtual void UpdateEnemy()
    {
        statusEffectManager.UpdateStatusEffects();

        CheckPlayerOverlap();
        if (!isInCombat || health <= 0)
            uiController.SetCanvasActive(false);
    }

    public void OnDamageEventStart(int col)
    {
        collisionController.EnableCollider(col);
    }
    public void OnDamageEventEnd(int col)
    {
        collisionController.DisableCollider(col);
    }

    public void OnDie()
    {
        Destroy(transform.parent.gameObject, 10f);
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
        aiNavigation.SetPathfindingTarget(player.transform, chaseMovementSpeed, true);

        if (Mathf.Abs(Vector2.Distance(transform.position, player.transform.position)) > attackRange)
            return;

        onReachChaseTarget?.Invoke();
    }

    protected bool CheckChasePlayer()
    {
        Vector2 dir;
        if (transform.localScale.x < 0)
            dir = -transform.right;
        else
            dir = transform.right;

        Debug.DrawRay(transform.position, dir * chaseRange, Color.red);

        if (!Physics2D.Raycast(transform.position, dir.normalized, chaseRange, playerLayer))
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

    private void CheckPlayerOverlap()
    {
        Vector2 closestPoint = enemyCol.ClosestPoint(player.transform.position);
        float distanceToPlayer = Vector2.Distance(closestPoint, player.transform.position);

        // Check if the player is within the detection range (collider bounds + extra range)
        if (distanceToPlayer <= 0)
            player.OnPlayerOverlap(true);
        else
            player.OnPlayerOverlap(false);
    }

    public override IEnumerator FrozenRoutine()
    {
        if (aiNavigation != null)
        aiNavigation.StopNavigationUntilResume();

        isFrozen = true;
        canUpdate = false;

        float previousAnimSpeed = animator.speed;
        animator.speed = 0;

        statusEffectManager.AddEffectUI(StatusEffectUI.StatusEffectType.Frozen, 0);

        yield return new WaitForSeconds(statusEffectStats.frozenDuration);

        statusEffectManager.RemoveEffectUI(StatusEffectUI.StatusEffectType.Frozen);
        animator.speed = previousAnimSpeed;
        frozenRoutine = null;

        if (health <= 0)
            yield break;

        if (!isInCombat && aiNavigation != null)
            aiNavigation.ResumeNavigationFromStop();

        isFrozen = false;
        canUpdate = true;
    }

    public override IEnumerator StunnedRoutine()
    {
        int currentShield = shield;
        shield = 0;
        InvokeOnShieldChanged(false, true);

        aiNavigation.StopNavigationUntilResume();
        canUpdate = false;

        float previousAnimSpeed = animator.speed;
        animator.speed = 0;

        statusEffectManager.AddEffectUI(StatusEffectUI.StatusEffectType.Stunned, 0);

        float timer = statusEffectStats.stunDuration;

        while (timer > 0)
        {
            timer -= Time.deltaTime;

            if (shieldRegenRoutine != null)
                StopCoroutine(shieldRegenRoutine);

            yield return null;
        }

        statusEffectManager.RemoveEffectUI(StatusEffectUI.StatusEffectType.Stunned);

        animator.speed = previousAnimSpeed;
        shield = currentShield;
        InvokeOnShieldChanged(true, false);
        stunnedRoutine = null;

        if (health <= 0)
            yield break;

        if (!isInCombat)
            aiNavigation.ResumeNavigationFromStop();
        canUpdate = true;
    }

    public override IEnumerator DazedRoutine()
    {
        aiNavigation.StopNavigationUntilResume();
        float previousAnimSpeed = animator.speed;
        animator.speed = 0;
        canUpdate = false;
        statusEffectManager.AddEffectUI(StatusEffectUI.StatusEffectType.Dazed, 0);

        yield return new WaitForSeconds(statusEffectStats.stunDuration);

        animator.speed = previousAnimSpeed;
        statusEffectManager.RemoveEffectUI(StatusEffectUI.StatusEffectType.Dazed);

        if (health <= 0)
            yield break;

        if (!isInCombat)
            aiNavigation.ResumeNavigationFromStop();
        canUpdate = true;
    }

    private void OnDisable()
    {
        onReachWaypoint = null;
        onFinishIdle = null;
        onPlayerInChaseRange = null;
        onReachChaseTarget = null;
        onHitEvent = null;
    }
}