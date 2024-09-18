using DesignPatterns.ObjectPool;
using System.Collections;
using UnityEngine;
using static MovementController;

public class PlayerController : PlayerStats
{
    public enum PlayerStates
    {
        Movement,
        Combat,
        Hurt
    }
    public PlayerStates currentState;

    public static PlayerController Instance;

    private AnimationManager animationManager;
    private MovementController movementController;
    private CombatController combatController;
    private AbilityController abilityController;
    private FadeTransition fadeTransition;
    private ItemManager itemManager;
    private PlayerEffectsController playerEffectsController;
    private Rigidbody2D playerRB;

    [SerializeField] private WFC_MapGeneration proceduralMapGenerator;
    [SerializeField] private LayerMask enemyLayer;

    private IInteractable currentInteractable;
    private float ropeX;
    private Damage previousDamage;

    private Coroutine transceiverBuffRoutine;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        movementController = GetComponent<MovementController>();
        combatController = GetComponent<CombatController>();
        abilityController = GetComponent<AbilityController>();
        fadeTransition = GetComponent<FadeTransition>();
        itemManager = GetComponent<ItemManager>();
        statusEffectManager = GetComponent<StatusEffectManager>();
        playerEffectsController = GetComponent<PlayerEffectsController>();
        animationManager = GetComponent<AnimationManager>();
        playerRB = GetComponent<Rigidbody2D>();

        animationManager.InitAnimationController();
        itemManager.InitItemManager();
        movementController.InitializeMovementController(animationManager);
        combatController.InitializeCombatController(this);
        abilityController.InitializeAbilityController();
        playerEffectsController.InitializePlayerEffectsController();
        if (proceduralMapGenerator != null)
        {
            proceduralMapGenerator.InitMapGenerator();
            transform.position = proceduralMapGenerator.GetStartingPos();
        }

        statusEffectManager.OnThresholdReached += TriggerStatusState;
        statusEffectManager.OnApplyStatusEffect += TriggerStatusEffect;
        statusEffectManager.OnCleanse += OnCleanse;

        combatController.OnAttackReset += () => { currentState = PlayerStates.Movement; };
        movementController.OnPlungeEnd += HandlePlungeAttack;
        movementController.ChangePlayerState += ChangeState;
        OnParry += OnParryEnemy;
    }

    private void Update()
    {
        statusEffectManager.UpdateStatusEffects();

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.Tab))
            ConsoleManager.Instance.SetConsole();

        if (Input.GetKeyDown(KeyCode.Return))
            ConsoleManager.Instance.OnInputCommand();

        if (ConsoleManager.Instance.gameObject.activeInHierarchy || 
            movementController.currentState == MovementState.Knockback ||
            currentState == PlayerStates.Hurt)
            return;

        // Combat Inputs
        if (Input.GetMouseButton(0))
        {
            if (movementController.currentState == MovementState.GroundDash ||
                movementController.currentState == MovementState.AirDash)
                return;

            currentState = PlayerStates.Combat;
            combatController.HandleAttack();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            combatController.ResetComboAttack();
        }

        if (Input.GetMouseButton(1))
        {
            if (combatController.HandleParry())
                currentState = PlayerStates.Combat;
        }

        // Movment Inputs
        if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.Space))
        {
            if (movementController.HandlePlunge())
            {
                currentState = PlayerStates.Movement;
                combatController.OnPlungeStart();
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            currentState = PlayerStates.Movement;
            movementController.OnJump(horizontal);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (movementController.HandleDash(horizontal))
                currentState = PlayerStates.Movement;
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            currentState = PlayerStates.Movement;
            movementController.HandleRoll();
        }

        if (currentState == PlayerStates.Movement)
        {
            if (!movementController.canMove)
            {
                movementController.ResumePlayer();
                combatController.SetCanAttack(true);
                combatController.OnParryEnd();
            }

            movementController.CheckGroundCollision();
            movementController.HandleGrappling(vertical, ropeX);
            movementController.HandleMovment(horizontal);
        }
        else
        {
            movementController.currentState = MovementState.Idle;
            movementController.StopPlayer();
        }

        // Other Inputs
        if (Input.GetKeyDown(KeyCode.F) && currentInteractable != null)
            currentInteractable.OnInteract();

        if (Input.GetKeyDown(KeyCode.E))
            abilityController.HandleAbility(0);
        if (Input.GetKeyDown(KeyCode.Q))
            abilityController.HandleAbility(1);
    }

    private void FixedUpdate()
    {
        movementController.MovePlayer(movementSpeedMultiplier.GetTotalModifier());
    }

    private void HandlePlungeAttack()
    {
        combatController.HandlePlungeAttack();
        movementController.StopPlayer();
    }

    public void OnPlayerOverlap(bool overlap)
    {
        movementController.OnPlayerOverlap(overlap);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Rope"))
        {
            ropeX = collision.transform.position.x;
            movementController.canGrapple = true;
        }

        if (collision.TryGetComponent<IInteractable>(out IInteractable interactable))
        {
            currentInteractable = interactable;
            interactable.OnEnterRange();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Rope"))
        {
            movementController.StopGrappling();
            movementController.canGrapple = false;
        }

        if (collision.TryGetComponent<IInteractable>(out IInteractable interactable))
        {
            interactable.OnLeaveRange();
            currentInteractable = null;
        }
    }

    public void FadeIn()
    {
        fadeTransition.FadeIn();
    }
    public void FadeOut()
    {
        fadeTransition.FadeOut();
    }

    public override bool TakeDamage(BaseStats attacker, Damage damage, bool isCrit, Vector3 closestPoint, DamagePopup.DamageType damageType)
    {
        bool tookDamage = base.TakeDamage(attacker, damage, isCrit, closestPoint, damageType);

        // Spiked Chestplate
        Damage chestplateDamage = CalculateProccDamageDealt(
            attacker, 
            new Damage((attack + attackIncrease.GetTotalModifier()) * itemStats.chestplateDamageModifier), 
            out bool chestplateCrit, 
            out DamagePopup.DamageType chestplateDamageType);

        attacker.TakeDamage(this, chestplateDamage, chestplateCrit, attacker.transform.position, chestplateDamageType);
        attacker.ApplyStatusEffect(new StatusEffect.StatusType(StatusEffect.StatusType.Type.Debuff, StatusEffect.StatusType.Status.Poison), itemStats.chestplatePoisonStacks);

        if (tookDamage)
        {
            playerEffectsController.ShakeCamera(4f, 5f, 0.2f);
            playerEffectsController.Pulse(0.5f, 3f, 0f, 0.3f, true);

            if (movementController.currentState == MovementState.Plunge ||
                movementController.currentState == MovementState.LedgeGrab)
                return true;

            playerRB.velocity = Vector2.zero;
            ChangeState(PlayerStates.Hurt);
            combatController.ResetComboInstantly();
            animationManager.ChangeAnimation(animationManager.Hurt, 0f, 0f, AnimationManager.AnimType.CannotOverride);
        }
        else
        {
            switch (immuneType)
            {
                case ImmuneType.Dodge:
                    // Icy Crampon
                    int randNum = Random.Range(0, 100);
                    if (randNum < itemStats.cramponChance)
                    {
                        Damage proccDamage = CalculateProccDamageDealt(attacker,
                            new Damage((attack + attackIncrease.GetTotalModifier()) * itemStats.cramponDamageModifier),
                            out bool proccCrit,
                            out DamagePopup.DamageType proccDamageType);

                        attacker.TakeTrueDamage(proccDamage);
                        attacker.ApplyStatusEffect(new StatusEffect.StatusType(StatusEffect.StatusType.Type.Debuff, StatusEffect.StatusType.Status.Freeze), itemStats.cramponFreezeStacks);
                    }

                    playerEffectsController.HitStop(0.1f);

                    break;
                case ImmuneType.Parry:
                    break;
            }
        }

        if (health <= 0)
        {
            Debug.Log("YOU DIED!");
        }

        return tookDamage;
    }

    public override float CalculateDamageDealt(BaseStats target, out bool isCrit, out DamagePopup.DamageType damageType)
    {
        // Knuckle Duster
        if (target.health >= target.maxHealth * itemStats.knucleDusterThreshold && target.shield <= 0)
            damageMultipler.AddModifier(itemStats.knuckleDusterDamageModifier);
        // Crude Knife
        if (Vector2.Distance(transform.position, target.transform.position) <= itemStats.crudeKnifeDistanceCheck)
            damageMultipler.AddModifier(itemStats.crudeKnifeDamageModifier);
        // Overloaded Capcitor
        if (target.shield <= 0)
            damageMultipler.AddModifier(itemStats.capacitorDamageModifier);

        float damage = base.CalculateDamageDealt(target, out isCrit, out damageType);

        // Knuckle Duster
        damageMultipler.RemoveModifier(itemStats.knuckleDusterDamageModifier);
        // CrudeKnife
        damageMultipler.RemoveModifier(itemStats.crudeKnifeDamageModifier);
        // Overloaded Capcitor
        damageMultipler.RemoveModifier(itemStats.capacitorDamageModifier);

        target.ApplyStatusEffect(new StatusEffect.StatusType(StatusEffect.StatusType.Type.Debuff, StatusEffect.StatusType.Status.Poison), 10);

        return damage;
    }

    public override IEnumerator FrozenRoutine()
    {
        particleVFXManager.OnFrozen();
        movementController.StopPlayer();
        isFrozen = true;

        yield return new WaitForSeconds(statusEffectStats.frozenDuration);

        FrozenEnd();
    }
    private void FrozenEnd()
    {
        movementController.ResumePlayer();
        isFrozen = false;
        particleVFXManager.StopFrozen();
        frozenRoutine = null;
    }

    public override IEnumerator StunnedRoutine()
    {
        movementController.StopPlayer();

        yield return new WaitForSeconds(statusEffectStats.stunDuration);

        StunEnd();
    }
    private void StunEnd()
    {
        movementController.ResumePlayer();
        stunnedRoutine = null;
    }

    public override IEnumerator DazedRoutine()
    {
        movementController.StopPlayer();

        yield return new WaitForSeconds(statusEffectStats.stunDuration);

        DazeEnd();
    }
    private void DazeEnd()
    {
        movementController.ResumePlayer();
        dazedRoutine = null;
    }

    public override void OnCleanse(StatusEffect.StatusType.Status status)
    {
        switch (status)
        {
            case StatusEffect.StatusType.Status.Frozen:
                if (frozenRoutine == null)
                    return;

                StopCoroutine(frozenRoutine);
                FrozenEnd();
                break;
            case StatusEffect.StatusType.Status.Stunned:
                if (stunnedRoutine == null)
                    return;

                StopCoroutine(stunnedRoutine);
                StunEnd();
                break;
            case StatusEffect.StatusType.Status.Dazed:
                if (dazedRoutine == null)
                    return;

                StopCoroutine(dazedRoutine);
                DazeEnd();
                break;
        }
    }

    public void OnHitEnemyEvent(BaseStats target, Damage damage, bool isCrit, Vector3 closestPoint)
    {
        if (damage.damageSource.Equals(Damage.DamageSource.StatusEffect))
            return;

        if (previousDamage.damageSource.Equals(damage.damageSource) && previousDamage.damageSource != Damage.DamageSource.Normal)
            previousDamage.counter++;
        else
            previousDamage = damage;

        int randNum;

        if (isCrit)
        {
            // Ritual Sickle
            randNum = Random.Range(0, 100);
            if (randNum < itemStats.ritualBleedChance)
                target.ApplyStatusEffect(new StatusEffect.StatusType(StatusEffect.StatusType.Type.Debuff, StatusEffect.StatusType.Status.Bleed), itemStats.ritualBleedStacks);
        }

        // Jagged Dagger
        randNum = Random.Range(0, 100);
        if (randNum < itemStats.daggerBleedChance)
            target.ApplyStatusEffect(new StatusEffect.StatusType(StatusEffect.StatusType.Type.Debuff, StatusEffect.StatusType.Status.Bleed), 1);

        // Frazzled Wire
        randNum = Random.Range(0, 100);
        if (randNum < itemStats.frazzledWireChance && 
            !previousDamage.damageSource.Equals(Damage.DamageSource.FrazzledWire))
        {
            totalDamageMultiplier.AddModifier(itemStats.frazzledWireTotalDamageModifier);

            Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, itemStats.frazzledWireRange, enemyLayer);
            foreach (Collider2D enemy in enemiesInRange)
            {
                Enemy targetEnemy = enemy.GetComponent<Enemy>();

                if (targetEnemy == null)
                    continue;

                Damage proccDamage = CalculateProccDamageDealt(targetEnemy,
                    new Damage(Damage.DamageSource.FrazzledWire, damage.damage),
                    out bool proccCrit,
                    out DamagePopup.DamageType proccDamageType);

                targetEnemy.TakeDamage(this, proccDamage, proccCrit, targetEnemy.transform.position, proccDamageType);
                targetEnemy.ApplyStatusEffect(new StatusEffect.StatusType(StatusEffect.StatusType.Type.Debuff, StatusEffect.StatusType.Status.Static), itemStats.frazzledWireStaticStacks);
            }

            totalDamageMultiplier.RemoveModifier(itemStats.frazzledWireTotalDamageModifier);
        }
    }
    public void OnEnemyDie(BaseStats target)
    {
        // Gasoline
        Collider2D[] enemies = Physics2D.OverlapCircleAll(target.transform.position, itemStats.gasolineRadius, enemyLayer);

        foreach (Collider2D enemy in enemies)
        {
            Enemy targetEnemy = enemy.GetComponent<Enemy>();

            if (targetEnemy == null || targetEnemy.Equals(target))
                continue;

            Damage damage = CalculateProccDamageDealt(targetEnemy, 
                new Damage((attack + attackIncrease.GetTotalModifier()) * itemStats.gasolineDamageModifier), 
                out bool isCrit, 
                out DamagePopup.DamageType damageType);

            targetEnemy.TakeDamage(this, damage, isCrit, enemy.transform.position, damageType);
            targetEnemy.ApplyStatusEffect(new StatusEffect.StatusType(StatusEffect.StatusType.Type.Debuff, StatusEffect.StatusType.Status.Burn), itemStats.gasolineBurnStacks);
        }
    }

    public void OnParryEnemy(BaseStats target)
    {
        playerEffectsController.HitStop(0.5f);
        playerEffectsController.ShakeCamera(5, 20, 0.5f);
        playerEffectsController.SetCameraTrigger("parry");
        movementController.Knockback(80f, 2f);

        // Metal Bat
        int randNum = Random.Range(0, 100);

        if (randNum < itemStats.metalBatChance)
            target.ApplyStatusEffect(new StatusEffect.StatusType(StatusEffect.StatusType.Type.Debuff, StatusEffect.StatusType.Status.Static), itemStats.metalBatStacks);
    }

    public void ApplyTransceiverBuff()
    {
        if (transceiverBuffRoutine == null)
            transceiverBuffRoutine = StartCoroutine(TransceiverBuff());
    }
    private IEnumerator TransceiverBuff()
    {
        float attackBuff;
        float attackSpeedBuff;

        attackBuff = attack * itemStats.transceiverBuffMultiplier;
        attackSpeedBuff = itemStats.transceiverBuffMultiplier;

        attackIncrease.AddModifier(attackBuff);
        attackSpeedMultiplier.AddModifier(attackSpeedBuff);

        yield return new WaitForSeconds(itemStats.transceiverBuffDuration);

        attackIncrease.RemoveModifier(attackBuff);
        attackSpeedMultiplier.RemoveModifier(attackSpeedBuff);

        transceiverBuffRoutine = null;
    }

    public void ChangeState(PlayerStates newState)
    {
        currentState = newState;
    }

    // For dev console
    public void GiveItem(string itemName, string amount)
    {
        itemManager.GiveItem(itemName, amount);
    }

    public void GiveAllItems()
    {
        itemManager.GiveAllItems();
    }
}