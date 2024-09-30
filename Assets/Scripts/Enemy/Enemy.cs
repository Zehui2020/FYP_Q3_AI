using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MovementController;

public class Enemy : EnemyStats
{
    [SerializeField] private bool isEnemy = true;

    public enum EnemyClass { Undead, Slime, Dummy, Skeleton, Scorpion }
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
    [SerializeField] protected LayerMask groundLayer;

    [SerializeField] protected List<Transform> waypoints = new();
    [SerializeField] protected int hurtValue;
    [SerializeField] protected float parryDazeDuration;
    [SerializeField] protected float parryShieldReduction;

    protected int currentWaypoint = 0;

    protected event System.Action onReachWaypoint;
    protected event System.Action onFinishIdle;
    protected event System.Action onPlayerInChaseRange;
    protected event System.Action onReachChaseTarget;
    protected event System.Action onDazeEnd;

    public event System.Action<BaseStats, Damage, bool, Vector3> onHitEvent;

    private Coroutine idleRoutine;
    private Coroutine knockbackRoutine;

    protected bool isInCombat = false;
    private float previousAnimSpeed;
    private int currentShield;

    private void Start()
    {
        InitializeEnemy();
    }

    public virtual void InitializeEnemy()
    {
        if (!isEnemy)
            return;

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

        statusEffectManager.OnThresholdReached += TriggerStatusState;
        statusEffectManager.OnApplyStatusEffect += TriggerStatusEffect;
        statusEffectManager.OnCleanse += OnCleanse;

        onPlayerInChaseRange += () => { isInCombat = true; uiController.SetCanvasActive(true); };
        OnHealthChanged += (increase, isCrit) => { if (!increase) { isInCombat = true; uiController.SetCanvasActive(true); } uiController.OnHealthChanged(health, maxHealth, increase, isCrit); };
        OnShieldChanged += (increase, isCrit, duration) => { if (!increase) { isInCombat = true; uiController.SetCanvasActive(true); } uiController.OnShieldChanged(shield, maxShield, increase, duration, isCrit); };
        OnBreached += (multiplier) => 
        {
            ApplyStatusEffect(new StatusEffect.StatusType(StatusEffect.StatusType.Type.Debuff, StatusEffect.StatusType.Status.Breached), 0);
            TriggerStatusState(StatusEffect.StatusType.Status.Dazed, shieldRegenDelay);
            PlayerEffectsController.Instance.HitStop(0.2f); 
        };

        onHitEvent += player.OnHitEnemyEvent;
        OnDieEvent += (target) => 
        {
            player.OnEnemyDie(target);
            uiController.SetCanvasActive(false);
            animator.speed = 1;
        };

        player.OnParry += (target) => 
        {
            if (target != this)
                return;

            Knockback(15f);
            OnGetParried();
        };
    }

    private void Update()
    {
        if (!isEnemy)
            return;

        if (!canUpdate)
            return;

        UpdateEnemy();
    }

    public virtual void OnGetParried()
    {
        TriggerStatusState(StatusEffect.StatusType.Status.Dazed, parryDazeDuration);
        TakeShieldDamageOnly(player, new Damage(parryShieldReduction * player.breachDamageMultiplier.GetTotalModifier()), false, transform.position, DamagePopup.DamageType.Shield);
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

    protected void UpdateMovementDirection()
    {
        if (aiNavigation.GetMovementDirection() == Vector2.right)
            transform.localScale = Vector3.one;
        else
            transform.localScale = new Vector3(-1, 1, 1);
    }

    protected void UpdateDirectionToPlayer()
    {
        transform.localScale = transform.position.x > player.transform.position.x ? new Vector3(-1, 1, 1) : Vector3.one;
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

    public bool CheckHurt()
    {
        int randNum = Random.Range(0, 100);
        if (randNum < hurtValue)
            return true;

        return false;
    }

    public override void ApplyStatusEffect(StatusEffect.StatusType statusEffect, int amount)
    {
        base.ApplyStatusEffect(statusEffect, amount);
        uiController.SetCanvasActive(true);
        isInCombat = true;
    }

    public override IEnumerator FrozenRoutine(float duration)
    {
        particleVFXManager.OnFrozen();

        if (aiNavigation != null)
            aiNavigation.StopNavigationUntilResume();

        isFrozen = true;
        canUpdate = false;

        previousAnimSpeed = animator.speed;
        animator.speed = 0;

        ApplyStatusEffect(new StatusEffect.StatusType(StatusEffect.StatusType.Type.Debuff, StatusEffect.StatusType.Status.Frozen), 0);

        yield return new WaitForSeconds(duration);

        OnFrozenEnd();
    }
    private void OnFrozenEnd()
    {
        statusEffectManager.RemoveEffectUI(StatusEffect.StatusType.Status.Frozen);
        animator.speed = previousAnimSpeed;
        frozenRoutine = null;

        if (health <= 0)
            return;

        if (!isInCombat && aiNavigation != null)
            aiNavigation.ResumeNavigationFromStop();

        isFrozen = false;
        canUpdate = true;
        particleVFXManager.StopFrozen();
        states.Dequeue();
    }

    public override IEnumerator StunnedRoutine(float duration)
    {
        currentShield = shield;
        shield = 0;
        InvokeOnShieldChanged(false, 0, true);

        aiNavigation.StopNavigationUntilResume();
        canUpdate = false;

        previousAnimSpeed = animator.speed;
        animator.speed = 0;

        ApplyStatusEffect(new StatusEffect.StatusType(StatusEffect.StatusType.Type.Debuff, StatusEffect.StatusType.Status.Stunned), 0);

        float timer = duration;
        InvokeOnShieldChanged(true, duration, false);

        while (timer > 0)
        {
            timer -= Time.deltaTime;

            if (shieldRegenRoutine != null)
                StopCoroutine(shieldRegenRoutine);

            yield return null;
        }

        OnStunEnd();
    }
    private void OnStunEnd()
    {
        statusEffectManager.RemoveEffectUI(StatusEffect.StatusType.Status.Stunned);

        animator.speed = previousAnimSpeed;
        shield = currentShield;
        stunnedRoutine = null;

        if (health <= 0)
            return;

        if (!isInCombat)
            aiNavigation.ResumeNavigationFromStop();
        canUpdate = true;
        states.Dequeue();
    }

    public override IEnumerator DazedRoutine(float duration)
    {
        aiNavigation.StopNavigationUntilResume();
        previousAnimSpeed = animator.speed;
        animator.speed = 0;
        canUpdate = false;
        ApplyStatusEffect(new StatusEffect.StatusType(StatusEffect.StatusType.Type.Debuff, StatusEffect.StatusType.Status.Dazed), 0);
        InvokeOnShieldChanged(true, duration, false);

        yield return new WaitForSeconds(duration);

        OnDazeEnd();
    }
    private void OnDazeEnd()
    {
        statusEffectManager.RemoveEffectUI(StatusEffect.StatusType.Status.Dazed);

        animator.speed = previousAnimSpeed;

        if (shield <= 0)
            shield = maxShield;

        dazedRoutine = null;

        if (health <= 0)
            return;

        if (!isInCombat)
            aiNavigation.ResumeNavigationFromStop();
        canUpdate = true;
        states.Dequeue();
        onDazeEnd?.Invoke();
    }

    public override void OnCleanse(StatusEffect.StatusType.Status status)
    {
        switch (status)
        {
            case StatusEffect.StatusType.Status.Frozen:
                if (frozenRoutine == null)
                    break;

                StopCoroutine(frozenRoutine);
                OnFrozenEnd();
                break;
            case StatusEffect.StatusType.Status.Stunned:
                if (stunnedRoutine == null)
                    break;

                StopCoroutine(stunnedRoutine);
                OnStunEnd();
                break;
            case StatusEffect.StatusType.Status.Dazed:
                if (dazedRoutine == null)
                    return;

                StopCoroutine(dazedRoutine);
                OnDazeEnd();
                break;
        }
    }

    public virtual void Knockback(float force)
    {
        if (knockbackRoutine != null)
            StopCoroutine(knockbackRoutine);
        knockbackRoutine = StartCoroutine(KnockbackRoutine(force));
    }

    private IEnumerator KnockbackRoutine(float force)
    {
        aiNavigation.StopNavigationUntilResume();

        Vector2 dir = transform.localScale.x > 0 ? -transform.right : transform.right;
        enemyRB.AddForce(dir * force, ForceMode2D.Impulse);

        while (enemyRB.velocity.magnitude > 0.1f)
        {
            yield return null;
        }

        knockbackRoutine = null;
        if (!isInCombat)
            aiNavigation.ResumeNavigationFromStop();
        knockbackRoutine = null;
    }

    private void OnDisable()
    {
        onReachWaypoint = null;
        onFinishIdle = null;
        onPlayerInChaseRange = null;
        onReachChaseTarget = null;
        onHitEvent = null;
        onDazeEnd = null;
    }
}