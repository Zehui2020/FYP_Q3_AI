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
    private Queue<Damage> damageQueue = new();

    private Vector2 plungeStartPos;
    private Vector2 plungeEndPos;

    public int chestUnlockCount = 0;
    public int extraLives = 0;

    [SerializeField] private TextMeshProUGUI goldText;
    public int gold = 0;

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
        if (health <= 0)
            return;

        goldText.text = gold.ToString();

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
        if (health <= 0)
            return;

        movementController.MovePlayer(movementSpeedMultiplier.GetTotalModifier());
    }

    private void HandlePlungeAttack()
    {
        plungeEndPos = transform.position;
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
        if (health <= 0)
            return false;

        bool tookDamage = base.TakeDamage(attacker, damage, isCrit, closestPoint, damageType);

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
        if (target.shield <= 0)
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

        damageQueue.Dequeue();
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
        for (int i = 0; i < itemStats.bottleStacks; i++)
        {
            int randNum = Random.Range(0, enemiesInRange.Count);

            StatusEffect.StatusType.Status randStatusEffect = (StatusEffect.StatusType.Status)Random.Range(0, (int)StatusEffect.StatusType.Status.TotalStatusEffect);

            StatusEffect.StatusType statusType = new StatusEffect.StatusType(
                StatusEffect.StatusType.Type.Debuff,
                randStatusEffect);

            enemiesInRange[randNum].ApplyStatusEffect(statusType, 1);
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