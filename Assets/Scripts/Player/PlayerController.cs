using DesignPatterns.ObjectPool;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static BaseStats.Damage;
using static MovementController;

public class PlayerController : PlayerStats
{
    public enum PlayerStates
    {
        Movement,
        Combat,
        Hurt,
        Dialogue,
        Ability,
        Map,
        Shop,
        ShadowBound,
        Transition
    }
    public PlayerStates currentState;

    public static PlayerController Instance;

    private AnimationManager animationManager;
    public MovementController movementController;
    [HideInInspector] public CombatController combatController;
    [HideInInspector] public AbilityController abilityController;
    private FadeTransition fadeTransition;
    [HideInInspector] public ItemManager itemManager;
    public PlayerEffectsController playerEffectsController;
    private Rigidbody2D playerRB;

    private Coroutine hurtRoutine;

    [SerializeField] public Canvas playerCanvas;
    [SerializeField] public DialogueManager dialogueManager;
    [SerializeField] private WFC_MapGeneration proceduralMapGenerator;
    [SerializeField] private MinimapController minimapController;
    [SerializeField] public PortalController portalController;
    [SerializeField] private LayerMask enemyLayer;

    [SerializeField] private EnemyStatBar healthBar;

    private IInteractable currentInteractable;
    private float ropeX;
    private Queue<Damage> damageQueue = new();

    private Vector2 plungeStartPos;
    private Vector2 plungeEndPos;

    public int chestUnlockCount = 0;
    public int extraLives = 0;
    private bool hitImmune = false;

    [SerializeField] private TextMeshProUGUI goldText;
    public int gold = 0;

    private GameData gameData;
    [SerializeField] private TextMeshProUGUI timerText;

    private float horizontal;
    private float vertical;

    private Coroutine transceiverBuffRoutine;
    private Coroutine gavelCooldown;

    private int shadowBoundClicks;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        gameData = GameData.Instance;
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
        movementController.InitializeMovementController(animationManager, playerRB);
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

        OnHealthChanged += (increase, isCrit) => 
        { 
            if (!increase) 
                healthBar.OnDecrease(health, maxHealth, isCrit, false); 
            else
                healthBar.OnIncreased(health, maxHealth, isCrit);
        };
    }

    private void Update()
    {
        // Timer
        if (gameData.timer <= 3600f)
        {
            var ts = System.TimeSpan.FromSeconds(gameData.timer);
            timerText.text = string.Format("{0:00}:{1:00}", ts.Minutes, ts.Seconds);
        }
        else
        {
            var ts = System.TimeSpan.FromSeconds(gameData.timer);
            timerText.text = string.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds);
        }

        // Console
        if (Input.GetKeyDown(KeyCode.Backslash))
            ConsoleManager.Instance.SetConsole();

        if (Input.GetKeyDown(KeyCode.Return))
            ConsoleManager.Instance.OnInputCommand();

        if (currentState == PlayerStates.ShadowBound)
        {
            if (Input.GetMouseButtonDown(0))
            {
                playerEffectsController.ShakeCamera(1f, 1f, 0.3f);
                shadowBoundClicks++;
            }

            return;
        }
        if (currentState == PlayerStates.Transition)
            return;

        if (abilityController != null && abilityController.swappingAbility)
        {
            movementController.currentState = MovementState.Idle;
            movementController.StopPlayer();
            for (int i = 0; i < abilityController.abilities.Count; i++)
            {
                if (i < 9 && Input.GetKeyDown((i + 1).ToString()))
                    abilityController.SwapAbility(i);
            }
            if (Input.GetKeyDown(KeyCode.Escape))
                abilityController.SwapAbility();
            return;
        }

        if (currentState == PlayerStates.Shop)
            return;

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
                minimapController.ChangeView(false, true);
                ChangeState(PlayerStates.Movement);
            }
            else
            {
                playerRB.velocity = Vector3.zero;
                playerRB.isKinematic = true;
            }
            return;
        }
        else
            playerRB.isKinematic = false;

        if (health <= 0)
            return;

        // abilities
        if (abilityController != null && !abilityController.swappingAbility)
            for (int i = 0; i < abilityController.abilities.Count; i++)
                if (i < 9 && Input.GetKeyDown((i + 1).ToString()))
                    abilityController.HandleAbility(i);

        if (currentState == PlayerStates.Ability)
            return;

        if (Input.GetKeyDown(KeyCode.Tab) && movementController.currentState != MovementState.LedgeGrab)
        {
            minimapController.ChangeView(true, true);
            ChangeState(PlayerStates.Map);
        }

        if (goldText != null)
            goldText.text = gold.ToString();

        statusEffectManager.UpdateStatusEffects();

        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        if (ConsoleManager.Instance.gameObject.activeInHierarchy)
            return;

        if (movementController.currentState == MovementState.Knockback ||
            currentState == PlayerStates.Hurt)
        {
            movementController.CheckGroundCollision();
            return;
        }

        // Combat Inputs
        if (Input.GetMouseButton(0))
        {
            if (movementController.CheckCannotCombat())
                return;

            ChangeState(PlayerStates.Combat);
            combatController.HandleAttack(horizontal);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            combatController.ResetComboAttack();
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (movementController.CheckCannotCombat())
                return;

            if (combatController.HandleParry())
                ChangeState(PlayerStates.Combat);
        }

        // Movment Inputs
        if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.Space))
        {
            if (movementController.HandlePlunge())
            {
                plungeStartPos = transform.position;
                ChangeState(PlayerStates.Movement);
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
    }

    private IEnumerator HurtRoutine()
    {
        movementController.canMove = false;
        playerRB.velocity = Vector2.zero;

        playerEffectsController.HitStop(0.3f, 0, true, 6f);
        playerEffectsController.ShakeCamera(6f, 8f, 0.2f);
        playerEffectsController.Pulse(0.5f, 3f, 0f, 0.3f, true);

        yield return new WaitForSeconds(0.5f);

        combatController.canAttack = true;
        movementController.canMove = true;
        playerRB.isKinematic = false;
        ChangeState(PlayerStates.Movement);

        hurtRoutine = null;
    }

    private void FixedUpdate()
    {
        if (health <= 0 || currentState == PlayerStates.ShadowBound)
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
        if (health <= 0 || hurtRoutine != null || hitImmune)
            return false;

        bool tookDamage = base.TakeDamage(attacker, damage, isCrit, closestPoint, damageType);

        if (health <= 0 && tookDamage)
        {
            Debug.Log("You hit the ground too hard");

            if (hurtRoutine != null)
                StopCoroutine(hurtRoutine);
            movementController.StopPlayer();
            animationManager.ChangeAnimation(animationManager.Die, 0f, 0f, AnimationManager.AnimType.None);

            StartCoroutine(DieRoutine());

            return tookDamage;
        }

        if (tookDamage)
        {
            if (movementController.currentState != MovementState.Plunge &&
                movementController.currentState != MovementState.LedgeGrab &&
                currentState != PlayerStates.Ability &&
                movementController.isGrounded)
                ChangeState(PlayerStates.Hurt);

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
            AddGold(itemStats.rebateGold);

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

                    playerEffectsController.HitStop(0.3f, 0, true, 6f);

                    break;
                case ImmuneType.Parry:
                    break;
            }
        }

        StartCoroutine(HitRoutine());

        return tookDamage;
    }
    private IEnumerator HitRoutine()
    {
        hitImmune = true;
        yield return new WaitForSeconds(0.5f);
        hitImmune = false;
    }
    private IEnumerator DieRoutine()
    {
        itemStats.ResetStats();
        itemManager.ResetItemStacks();

        yield return new WaitForSeconds(2f);

        if (extraLives <= 0)
        {
            SceneLoader.Instance.LoadScene("LobbyLevel");
            yield break;
        }

        extraLives--;
        health = maxHealth;

        Cleanse(StatusEffect.StatusType.Type.Debuff);
        Cleanse(StatusEffect.StatusType.Type.Buff);

        movementController.ResumePlayer();
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
        //target.ApplyStatusEffect(new StatusEffect.StatusType(StatusEffect.StatusType.Type.Debuff, StatusEffect.StatusType.Status.Freeze), 10);
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
            AudioManager.Instance.PlayOneShot(Sound.SoundName.FrazzledWire);

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
            AudioManager.Instance.PlayOneShot(Sound.SoundName.DynaMight);

            totalDamageMultiplier.RemoveModifier(itemStats.dynamightTotalDamageMultiplier);
        }

        // Ancient Gavel
        if (!previousDamage.damageSource.Equals(DamageSource.Gavel) && itemStats.gavelThreshold != 0 && gavelCooldown == null)
        {
            float gavelThreshold = attack * itemStats.gavelThreshold;
            if (damage.damage >= gavelThreshold)
            {
                Damage gavelDamage = CalculateProccDamageDealt(target, new Damage(DamageSource.Gavel, attack * itemStats.gavelDamageMultiplier), out bool gavelCrit, out DamagePopup.DamageType gavelDamageType);
                target.TakeDamage(this, gavelDamage, gavelCrit, target.transform.position, gavelDamageType);

                target.ApplyStatusEffect(new StatusEffect.StatusType(StatusEffect.StatusType.Type.Debuff, StatusEffect.StatusType.Status.Burn), itemStats.gavelStacks);
                target.ApplyStatusEffect(new StatusEffect.StatusType(StatusEffect.StatusType.Type.Debuff, StatusEffect.StatusType.Status.Static), itemStats.gavelStacks);

                AudioManager.Instance.PlayOneShot(Sound.SoundName.Ravage);
                gavelCooldown = StartCoroutine(GavelCooldown());
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
        if (itemStats.gasolineRadius > 0)
        {
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
            AudioManager.Instance.PlayOneShot(Sound.SoundName.DynaMight);

            target.particleVFXManager.GasolineBurst();
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

                if (i == 0)
                    AudioManager.Instance.PlayOneShot(Sound.SoundName.BottleOSurprise);
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
        AudioManager.Instance.PlayOneShot(Sound.SoundName.ParryHit);
        playerEffectsController.HitStop(0.3f, 0, false, 0);
        playerEffectsController.ShakeCamera(5, 20, 0.5f);
        playerEffectsController.SetCameraTrigger("parry");

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

    private IEnumerator GavelCooldown()
    {
        yield return new WaitForSeconds(itemStats.gavelCooldown);

        gavelCooldown = null;
    }

    public void ChangeState(PlayerStates newState)
    {
        if (currentState == newState || health <= 0)
            return;

        currentState = newState;
        switch (currentState)
        {
            case PlayerStates.Movement:
                movementController.ResumePlayer();
                movementController.ChangeState(MovementState.Idle);
                break;
            case PlayerStates.Hurt:
                if (hurtRoutine != null)
                    StopCoroutine(hurtRoutine);
                hurtRoutine = StartCoroutine(HurtRoutine());
                animationManager.ChangeAnimation(animationManager.Hurt, 0f, 0f, AnimationManager.AnimType.CannotOverride);
                break;
            case PlayerStates.Map:
            case PlayerStates.Shop:
            case PlayerStates.Dialogue:
            case PlayerStates.Ability:
                movementController.StopPlayer();
                movementController.ChangeState(MovementState.Idle);
                break;
            case PlayerStates.ShadowBound:
                animationManager.ChangeAnimation(animationManager.Idle, 0f, 0f, AnimationManager.AnimType.CannotOverride);
                StartCoroutine(ShadowBoundRoutine());
                break;
        }
    }

    private IEnumerator ShadowBoundRoutine()
    {
        float timer = 10f;

        while (timer > 0)
        {
            playerRB.velocity = Vector2.zero;
            timer -= Time.deltaTime * shadowBoundClicks;
            yield return null;
        }

        shadowBoundClicks = 0;
        ChangeState(PlayerStates.Movement);
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

        if (coinsToSpawn == 0)
            return;

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

    public void AddGold(int amount)
    {
        gold += amount;
        playerEffectsController.AddMoney(amount);
    }
    public void RemoveGold(int amount)
    {
        gold -= amount;
    }

    public bool IsGrounded()
    {
        return movementController.isGrounded;
    }

    public void GiveItem(Item item)
    {
        itemManager.AddItem(item);
    }
    public void GiveAbility(BaseAbility ability)
    {
        itemManager.AddAbility(ability);
    }
    public void GiveShopItem(ShopItemData shopItemData)
    {
        if (shopItemData is Item item)
            itemManager.AddItem(item);
        else if (shopItemData is BaseAbility ability)
            abilityController.HandleAbilityPickUp(ability, ability.abilityCharges);
        else if (shopItemData is WeaponData weapon)
            combatController.ChangeWeapon(weapon);
    }

    // For console
    public void GiveItem(string itemName, string amount)
    {
        itemManager.GiveItem(itemName, amount);
    }
    public void GiveAllItems()
    {
        itemManager.GiveAllItems();
    }
    public void GiveAbility(string itemName)
    {
        if (itemName == "Test")
        {
            itemManager.GiveAbility("FreezingOrb");
            itemManager.GiveAbility("Shatter");
            return;
        }

        itemManager.GiveAbility(itemName);
    }

    private void OnApplicationQuit()
    {
        itemStats.ResetStats();
        itemManager.ResetItemStacks();
    }
}