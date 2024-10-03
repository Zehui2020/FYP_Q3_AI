using DesignPatterns.ObjectPool;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static BaseStats.Damage;
using static MovementController;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class PlayerController : PlayerStats
{
    public enum PlayerStates
    {
        Movement,
        Combat,
        Hurt,
        Dialogue,
        Ability,
        Map
    }
    public PlayerStates currentState;

    public static PlayerController Instance;

    private AnimationManager animationManager;
    private MovementController movementController;
    private CombatController combatController;
    [HideInInspector] public AbilityController abilityController;
    private FadeTransition fadeTransition;
    private ItemManager itemManager;
    private PlayerEffectsController playerEffectsController;
    private Rigidbody2D playerRB;

    private Coroutine hurtRoutine;

    [SerializeField] public Canvas playerCanvas;
    [SerializeField] public DialogueManager dialogueManager;
    [SerializeField] private WFC_MapGeneration proceduralMapGenerator;
    [SerializeField] private MinimapController minimapController;
    [SerializeField] public PortalController portalController;
    [SerializeField] private LayerMask enemyLayer;

    [SerializeField] private EnemyStatBar healthBar;
    [SerializeField] private EnemyStatBar shieldBar;

    private IInteractable currentInteractable;
    private float ropeX;
    private Queue<Damage> damageQueue = new();

    private Vector2 plungeStartPos;
    private Vector2 plungeEndPos;

    public int chestUnlockCount = 0;
    public int extraLives = 0;

    [SerializeField] private TextMeshProUGUI goldText;
    public int gold = 0;

    private float horizontal;
    private float vertical;

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

        if (abilityController != null)
            abilityController.InitializeAbilityController();

        playerEffectsController.InitializePlayerEffectsController();
        if (proceduralMapGenerator != null)
            proceduralMapGenerator.InitMapGenerator();
        abilityStats.ResetAbilityStats();

        statusEffectManager.OnThresholdReached += TriggerStatusState;
        statusEffectManager.OnApplyStatusEffect += TriggerStatusEffect;
        statusEffectManager.OnCleanse += OnCleanse;

        combatController.OnAttackReset += () => { currentState = PlayerStates.Movement; };
        movementController.OnPlungeEnd += HandlePlungeAttack;
        movementController.ChangePlayerState += ChangeState;
        OnParry += OnParryEnemy;

        healthBar.InitStatBar(health, maxHealth);
        shieldBar.InitStatBar(shield, maxShield);

        OnHealthChanged += (increase, isCrit) => 
        { 
            if (!increase) 
                healthBar.OnDecrease(health, maxHealth, isCrit, false); 
            else
                healthBar.OnIncreased(health, maxHealth, isCrit);
        };

        OnShieldChanged += (increase, isCrit, duration) => 
        {
            if (!increase)
                shieldBar.OnDecrease(shield, maxShield, isCrit, false);
            else
                shieldBar.OnIncreased(shield, maxShield, isCrit);
        };
    }

    private void Update()
    {
        // Console
        if (Input.GetKeyDown(KeyCode.P))
            ConsoleManager.Instance.SetConsole();

        if (Input.GetKeyDown(KeyCode.Return))
            ConsoleManager.Instance.OnInputCommand();

        if (currentState == PlayerStates.Dialogue)
        {
            if (Input.GetMouseButtonDown(0))
            {
                dialogueManager.ShowNextDialogue();
            }

            return;
        }

        if (currentState == PlayerStates.Map)
        {
            if (Input.GetKeyUp(KeyCode.Tab))
            {
                minimapController.ChangeView(false);
                ChangeState(PlayerStates.Movement);
            }

            return;
        }

        if (health <= 0 || 
            currentState == PlayerStates.Ability)
            return;

        if (abilityController != null && abilityController.swappingAbility)
        {
            for (int i = 0; i < abilityController.abilities.Count; i++)
            {
                if (i < 9 && Input.GetKeyDown((i + 1).ToString()))
                    abilityController.SwapAbility(i);
            }
            if (Input.GetKeyDown(KeyCode.Escape))
                abilityController.SwapAbility();
            return;
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            minimapController.ChangeView(true);
            ChangeState(PlayerStates.Map);
        }

        if (goldText != null)
            goldText.text = gold.ToString();

        statusEffectManager.UpdateStatusEffects();

        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        if (ConsoleManager.Instance.gameObject.activeInHierarchy ||
            currentState == PlayerStates.Hurt)
            return;

        if (movementController.currentState == MovementState.Knockback)
        {
            movementController.CheckGroundCollision();
            return;
        }

        // Combat Inputs
        if (Input.GetMouseButton(0))
        {
            if (movementController.CheckCannotCombat())
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
            if (movementController.CheckCannotCombat())
                return;

            if (combatController.HandleParry())
                currentState = PlayerStates.Combat;
        }

        // Movment Inputs
        if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.Space))
        {
            if (movementController.HandlePlunge())
            {
                plungeStartPos = transform.position;
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
            movementController.HandleMovment(horizontal);
        }
        else
        {
            movementController.currentState = MovementState.Idle;
            movementController.StopPlayer();
        }

        // Other Inputs
        if (Input.GetKeyDown(KeyCode.E) && currentInteractable != null)
        {
            if (currentInteractable.OnInteract())
            {
                // Daze Grenade
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, itemStats.dazeGrenadeRadius, enemyLayer);
                foreach (Collider2D col in colliders)
                {
                    if (!col.TryGetComponent<Enemy>(out Enemy enemy))
                        continue;

                    enemy.TriggerStatusState(StatusEffect.StatusType.Status.Dazed, statusEffectStats.dazeDuration);
                }
            }
        }

        // abilities
        if (abilityController != null && !abilityController.swappingAbility)
            for (int i = 0; i < abilityController.abilities.Count; i++)
                if (i < 9 && Input.GetKeyDown((i + 1).ToString()))
                        abilityController.HandleAbility(i);
    }

    private IEnumerator HurtRoutine()
    {
        movementController.canMove = false;
        playerRB.velocity = Vector2.zero;
        playerRB.isKinematic = true;

        if (movementController.currentState == MovementState.GroundDash || 
            movementController.currentState == MovementState.AirDash)
            movementController.CancelDash();

        yield return new WaitForSeconds(0.5f);

        movementController.canMove = true;
        playerRB.isKinematic = false;
        hurtRoutine = null;

        movementController.ChangeState(MovementState.Idle);
        ChangeState(PlayerStates.Movement);
    }

    private void FixedUpdate()
    {
        if (health <= 0)
            return;

        movementController.MovePlayer(movementSpeedMultiplier.GetTotalModifier());

        if (currentState == PlayerStates.Movement)
            movementController.HandleGrappling(vertical, ropeX);
    }

    private void HandlePlungeAttack()
    {
        plungeEndPos = transform.position;
        combatController.HandlePlungeAttack();
        movementController.StopPlayer();
        playerEffectsController.ShakeCamera(8f, 5f, 0.3f);
    }

    public void OnPlayerOverlap(bool overlap)
    {
        movementController.OnPlayerOverlap(overlap);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (movementController.currentState == MovementState.Plunge)
            movementController.StopPlunge();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<IInteractable>(out IInteractable interactable))
        {
            currentInteractable = interactable;
            interactable.OnEnterRange();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (movementController.RopeTriggerEnter(collision))
            ropeX = collision.transform.position.x;

        if (collision.TryGetComponent<IInteractable>(out IInteractable interactable))
        {
            currentInteractable = interactable;
            interactable.OnEnterRange();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        movementController.RopeTriggerExit(collision);

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
        if (health <= 0)
            return false;

        bool tookDamage = base.TakeDamage(attacker, damage, isCrit, closestPoint, damageType);

        if (tookDamage)
        {
            playerEffectsController.ShakeCamera(4f, 5f, 0.2f);
            playerEffectsController.Pulse(0.5f, 3f, 0f, 0.3f, true);

            if (movementController.currentState != MovementState.Plunge &&
                movementController.currentState != MovementState.LedgeGrab &&
                movementController.isGrounded)
            {
                ChangeState(PlayerStates.Hurt);
                animationManager.ChangeAnimation(animationManager.Hurt, 0f, 0f, AnimationManager.AnimType.CannotOverride);

                if (hurtRoutine != null)
                    StopCoroutine(hurtRoutine);
                hurtRoutine = StartCoroutine(HurtRoutine());
            }

            combatController.ResetComboInstantly();

            // Spiked Chestplate
            if (itemStats.chestplateDamageModifier != 0)
            {
                Damage chestplateDamage = CalculateProccDamageDealt(
                    attacker,
                    new Damage((attack + attackIncrease.GetTotalModifier()) * itemStats.chestplateDamageModifier),
                    out bool chestplateCrit,
                    out DamagePopup.DamageType chestplateDamageType);

                attacker.TakeDamage(this, chestplateDamage, chestplateCrit, attacker.transform.position, chestplateDamageType);
                attacker.ApplyStatusEffect(new StatusEffect.StatusType(StatusEffect.StatusType.Type.Debuff, StatusEffect.StatusType.Status.Poison), itemStats.chestplatePoisonStacks);
            }

            // Rebate Token
            gold += itemStats.rebateGold;

            // Charged Defibrillators
            if (itemStats.defibrillatorHealMultiplier != 0)
                StartCoroutine(DefibrillatorRoutine());
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
            Debug.Log("You hit the ground too hard");

            if (extraLives > 0)
                StartCoroutine(DieRoutine());
        }

        return tookDamage;
    }
    private IEnumerator DieRoutine()
    {
        yield return new WaitForSeconds(2f);

        extraLives--;
        health = maxHealth;

        Cleanse(StatusEffect.StatusType.Type.Debuff);
        Cleanse(StatusEffect.StatusType.Type.Buff);
    }

    public override float CalculateDamageDealt(BaseStats target, DamageSource damageSource, out bool isCrit, out DamagePopup.DamageType damageType)
    {
        // Item Multipliers
        AddItemMultipliers(target);

        // Lead Plunger
        float plungerDamageMultiplier = 0;
        if (damageSource == DamageSource.Plunge)
        {
            float dist = Vector2.Distance(plungeStartPos, plungeEndPos);

            if (dist >= itemStats.minPlungeDist)
            {
                float distPercentage = Mathf.Clamp01(dist / itemStats.maxPlungeDist);
                plungerDamageMultiplier = distPercentage * itemStats.maxPlungeMultiplier;
                plungerDamageMultiplier = Mathf.Clamp(plungerDamageMultiplier, itemStats.minPlungeMultiplier, distPercentage * itemStats.maxPlungeMultiplier);

                damageMultipler.AddModifier(plungerDamageMultiplier);
            }
        }

        float damage = base.CalculateDamageDealt(target, damageSource, out isCrit, out damageType);

        // Item Multipliers
        RemoveItemMultipliers();

        // Lead Plunger
        if (damageSource == DamageSource.Plunge)
            damageMultipler.RemoveModifier(plungerDamageMultiplier);

        // Vampiric Stake
        if (isCrit && itemStats.stakeHealAmount > 0)
            Heal(Mathf.CeilToInt(itemStats.stakeHealAmount * maxHealth));

        //target.ApplyStatusEffect(new StatusEffect.StatusType(StatusEffect.StatusType.Type.Debuff, StatusEffect.StatusType.Status.Burn), 1);
        //target.ApplyStatusEffect(new StatusEffect.StatusType(StatusEffect.StatusType.Type.Debuff, StatusEffect.StatusType.Status.Poison), 1);
        //target.ApplyStatusEffect(new StatusEffect.StatusType(StatusEffect.StatusType.Type.Debuff, StatusEffect.StatusType.Status.Static), 1);
        //target.ApplyStatusEffect(new StatusEffect.StatusType(StatusEffect.StatusType.Type.Debuff, StatusEffect.StatusType.Status.Freeze), 1);
        //target.ApplyStatusEffect(new StatusEffect.StatusType(StatusEffect.StatusType.Type.Debuff, StatusEffect.StatusType.Status.Bleed), 1);

        return damage;
    }
    public override Damage CalculateProccDamageDealt(BaseStats target, Damage damage, out bool isCrit, out DamagePopup.DamageType damageType)
    {
        AddItemMultipliers(target);

        Damage finalDamage = base.CalculateProccDamageDealt(target, damage, out isCrit, out damageType);

        RemoveItemMultipliers();

        return finalDamage;
    }

    private void AddItemMultipliers(BaseStats target)
    {
        // Knuckle Duster
        if (target.health >= target.maxHealth * itemStats.knucleDusterThreshold && target.shield <= 0)
            damageMultipler.AddModifier(itemStats.knuckleDusterDamageModifier);
        // Crude Knife
        if (Vector2.Distance(transform.position, target.transform.position) <= itemStats.crudeKnifeDistanceCheck)
            damageMultipler.AddModifier(itemStats.crudeKnifeDamageModifier);
        // Overloaded Capcitor
        if (target.states.Contains(StatusEffect.StatusType.Status.Stunned) || 
            target.states.Contains(StatusEffect.StatusType.Status.Dazed))
            damageMultipler.AddModifier(itemStats.capacitorDamageModifier);
    }
    private void RemoveItemMultipliers()
    {
        // Knuckle Duster
        damageMultipler.RemoveModifier(itemStats.knuckleDusterDamageModifier);
        // CrudeKnife
        damageMultipler.RemoveModifier(itemStats.crudeKnifeDamageModifier);
        // Overloaded Capcitor
        damageMultipler.RemoveModifier(itemStats.capacitorDamageModifier);
    }

    public override IEnumerator FrozenRoutine(float duration)
    {
        particleVFXManager.OnFrozen();
        movementController.StopPlayer();
        isFrozen = true;

        yield return new WaitForSeconds(duration);

        FrozenEnd();
    }
    private void FrozenEnd()
    {
        movementController.ResumePlayer();
        isFrozen = false;
        particleVFXManager.StopFrozen();
        frozenRoutine = null;
        states.Dequeue();
    }

    public override IEnumerator StunnedRoutine(float duration)
    {
        movementController.StopPlayer();

        yield return new WaitForSeconds(duration);

        StunEnd();
    }
    private void StunEnd()
    {
        movementController.ResumePlayer();
        stunnedRoutine = null;
        states.Dequeue();
    }

    public override IEnumerator DazedRoutine(float duration)
    {
        movementController.StopPlayer();

        yield return new WaitForSeconds(duration);

        DazeEnd();
    }
    private void DazeEnd()
    {
        movementController.ResumePlayer();
        dazedRoutine = null;
        states.Dequeue();
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

    private bool CanProccItem(Damage previousDamage)
    {
        return previousDamage.damageSource.Equals(DamageSource.Normal) || previousDamage.damageSource.Equals(DamageSource.Plunge);
    }
    public void OnHitEnemyEvent(BaseStats target, Damage damage, bool isCrit, Vector3 closestPoint)
    {
        if (damage.damageSource.Equals(DamageSource.StatusEffect))
            return;

        Damage previousDamage;
        if (!damageQueue.TryPeek(out previousDamage))
            previousDamage = damage;

        damageQueue.Enqueue(damage);

        if (previousDamage.damageSource.Equals(damage.damageSource) && previousDamage.damageSource != DamageSource.Normal)
            previousDamage.counter++;
        else
            previousDamage = damage;

        int randNum;

        // Blood Arts Bleed
        randNum = Random.Range(0, 100);
        if (randNum < abilityStats.bloodArtsBleedChance)
            target.ApplyStatusEffect(new StatusEffect.StatusType(StatusEffect.StatusType.Type.Debuff, StatusEffect.StatusType.Status.Bleed), 1);

        // Blood Arts Lifesteal
        if (abilityStats.bloodArtsLifestealMultiplier > 0)
            Heal((int)(damage.damage * abilityStats.bloodArtsLifestealMultiplier));

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
        if (randNum < itemStats.frazzledWireChance && CanProccItem(previousDamage))
        {
            totalDamageMultiplier.AddModifier(itemStats.frazzledWireTotalDamageModifier);

            Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, itemStats.frazzledWireRange, enemyLayer);
            foreach (Collider2D enemy in enemiesInRange)
            {
                Enemy targetEnemy = enemy.GetComponent<Enemy>();

                if (targetEnemy == null)
                    continue;

                Damage proccDamage = CalculateProccDamageDealt(targetEnemy,
                    new Damage(DamageSource.Item, damage.damage),
                    out bool proccCrit,
                    out DamagePopup.DamageType proccDamageType);

                targetEnemy.TakeDamage(this, proccDamage, proccCrit, targetEnemy.transform.position, proccDamageType);
                targetEnemy.ApplyStatusEffect(new StatusEffect.StatusType(StatusEffect.StatusType.Type.Debuff, StatusEffect.StatusType.Status.Static), itemStats.frazzledWireStaticStacks);
            }

            totalDamageMultiplier.RemoveModifier(itemStats.frazzledWireTotalDamageModifier);
        }

        // Dynamight
        if (CanProccItem(previousDamage) && itemStats.dynamightTotalDamageMultiplier != 0)
        {
            totalDamageMultiplier.AddModifier(itemStats.dynamightTotalDamageMultiplier);

            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, itemStats.dynamightRadius);

            foreach (Collider2D col in colliders)
            {
                if (!col.TryGetComponent<EnemyStats>(out EnemyStats enemy))
                    continue;

                if (enemy == target)
                    continue;

                float dynamightDamage = CalculateDamageDealt(enemy, DamageSource.Item, out bool dynamightCrit, out DamagePopup.DamageType dynamightDamageType);
                enemy.TakeDamage(this, new Damage(DamageSource.Item, dynamightDamage), dynamightCrit, enemy.transform.position, dynamightDamageType);
            }

            totalDamageMultiplier.RemoveModifier(itemStats.dynamightTotalDamageMultiplier);
        }

        // Ancient Gavel
        if (!previousDamage.damageSource.Equals(DamageSource.Gavel) && itemStats.gavelThreshold != 0)
        {
            float gavelThreshold = attack * itemStats.gavelThreshold;
            if (damage.damage >= gavelThreshold)
            {
                Damage gavelDamage = CalculateProccDamageDealt(target, new Damage(DamageSource.Gavel, attack * itemStats.gavelDamageMultiplier), out bool gavelCrit, out DamagePopup.DamageType gavelDamageType);
                target.TakeDamage(this, gavelDamage, gavelCrit, target.transform.position, gavelDamageType);

                target.ApplyStatusEffect(new StatusEffect.StatusType(StatusEffect.StatusType.Type.Debuff, StatusEffect.StatusType.Status.Burn), itemStats.gavelStacks);
                target.ApplyStatusEffect(new StatusEffect.StatusType(StatusEffect.StatusType.Type.Debuff, StatusEffect.StatusType.Status.Static), itemStats.gavelStacks);
            }
        }

        // Blood Fungi
        if (itemStats.fungiHealAmount > 0)
            Heal(itemStats.fungiHealAmount);

        damageQueue.Dequeue();
    }
    public void OnEnemyDie(BaseStats target)
    {
        int randNum;

        // Gasoline
        Collider2D[] enemies = Physics2D.OverlapCircleAll(target.transform.position, itemStats.gasolineRadius, enemyLayer);
        if (itemStats.gasolineRadius > 0)
            target.particleVFXManager.GasolineBurst();
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

        // Bottle Of Surprises
        Collider2D[] colliders = Physics2D.OverlapCircleAll(target.transform.position, itemStats.bottleRadius, enemyLayer);
        List<BaseStats> enemiesInRange = new();
        foreach (Collider2D col in colliders)
        {
            if (!col.TryGetComponent<BaseStats>(out BaseStats enemy))
                continue;

            if (enemy == target)
                continue;

            enemiesInRange.Add(enemy);
        }

        if (enemiesInRange.Count != 0)
        {
            for (int i = 0; i < itemStats.bottleStacks; i++)
            {
                randNum = Random.Range(0, enemiesInRange.Count);

                StatusEffect.StatusType.Status randStatusEffect = (StatusEffect.StatusType.Status)Random.Range(0, (int)StatusEffect.StatusType.Status.TotalStatusEffect);

                StatusEffect.StatusType statusType = new StatusEffect.StatusType(
                    StatusEffect.StatusType.Type.Debuff,
                    randStatusEffect);

                enemiesInRange[randNum].ApplyStatusEffect(statusType, 1);
            }
        }

        // Interest Contract
        int goldToDrop = target.goldUponDeath;
        randNum = Random.Range(0, 100);
        if (randNum < itemStats.interestChance)
            goldToDrop *= 2;
        SpawnGoldPickup(goldToDrop, target.transform);

        // NRG Bar
        randNum = Random.Range(0, 100);
        if (randNum < itemStats.nrgBarChance)
        {
            NRGBarPickup nRGBarPickup = ObjectPool.Instance.GetPooledObject("NRGBar", false) as NRGBarPickup;
            nRGBarPickup.transform.position = target.transform.position;
            nRGBarPickup.gameObject.SetActive(true);
        }
    }

    public void OnParryEnemy(BaseStats target)
    {
        playerEffectsController.HitStop(0.5f);
        playerEffectsController.ShakeCamera(5, 20, 0.5f);
        playerEffectsController.SetCameraTrigger("parry");
        movementController.Knockback(20f);

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

    private IEnumerator DefibrillatorRoutine()
    {
        yield return new WaitForSeconds(itemStats.defibrillatorHealDelay);

        Heal(Mathf.CeilToInt(itemStats.defibrillatorHealMultiplier * maxHealth));
    }

    public void ChangeState(PlayerStates newState)
    {
        currentState = newState;
        if (currentState == PlayerStates.Movement)
            movementController.ChangeState(MovementState.Idle);
    }

    public void AddJumpCount(int count)
    {
        movementController.jumpCount += count;
        movementController.maxJumpCount += count;
    }
    public void AddWallJumpCount(int count)
    {
        movementController.wallJumpCount += count;
        movementController.maxWallJumps += count;
    }

    public void PickupWeapon(WeaponData weaponData)
    {
        combatController.ChangeWeapon(weaponData);
    }

    public void HideCanvas()
    {
        playerCanvas.enabled = false;
    }

    public void ShowCanvas()
    {
        playerCanvas.enabled = true;
    }

    public void TalkToNPC(BaseNPC npc)
    {
        ChangeState(PlayerStates.Dialogue);
        movementController.ChangeState(MovementState.Idle);
        HideCanvas();
        dialogueManager.SetTalkingNPC(npc);
    }

    public void LeaveNPC()
    {
        ChangeState(PlayerStates.Movement);
        ShowCanvas();
        dialogueManager.HideDialogue();
    }

    public void SpawnGoldPickup(int goldToDrop, Transform target)
    {
        //Spawn Gold pickup
        int coinsToSpawn = Mathf.CeilToInt(goldToDrop / 2f);
        int goldPerCoin = goldToDrop / coinsToSpawn;
        int remainderGold = goldToDrop % coinsToSpawn;
        for (int i = 0; i < coinsToSpawn; i++)
        {
            GoldPickup goldPickup = ObjectPool.Instance.GetPooledObject("Gold", true) as GoldPickup;
            goldPickup.transform.position = target.transform.position;
            int amountToGive = goldPerCoin + (i < remainderGold ? 1 : 0);
            goldPickup.InitGoldPickup(amountToGive);
        }
    }

    public void ShowDialoguePopup(int index)
    {
        dialogueManager.ShowDialoguePopup(index);
    }

    public bool IsGrounded()
    {
        return movementController.isGrounded;
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

    private void OnApplicationQuit()
    {
        itemStats.ResetStats();
    }
}