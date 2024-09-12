using System.Collections;
using UnityEngine;

public class PlayerController : PlayerStats
{
    public static PlayerController Instance;

    private MovementController movementController;
    private CombatController combatController;
    private AbilityController abilityController;
    private FadeTransition fadeTransition;
    private ItemManager itemManager;
    private PlayerEffectsController playerEffectsController;
    [SerializeField] private ProceduralMapGenerator proceduralMapGenerator;
    [SerializeField] private LayerMask enemyLayer;

    private IInteractable currentInteractable;

    private float ropeX;
    private Damage previousDamage;

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

        itemManager.InitItemManager();
        movementController.InitializeMovementController(playerEffectsController);
        combatController.InitializeCombatController(this);
        abilityController.InitializeAbilityController();
        playerEffectsController.InitializePlayerEffectsController();
        if (proceduralMapGenerator != null)
            proceduralMapGenerator.InitMapGenerator();

        statusEffectManager.OnThresholdReached += ApplyStatusState;
        statusEffectManager.OnApplyStatusEffect += ApplyStatusEffect;

        movementController.OnPlungeEnd += HandlePlungeAttack;
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

        if (ConsoleManager.Instance.gameObject.activeInHierarchy)
            return;

        if (Input.GetKeyDown(KeyCode.Space) && !Input.GetKey(KeyCode.S))
            movementController.HandleJump(horizontal);

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            movementController.HandleDash(horizontal);

            if (movementController.isPlunging)
                combatController.CancelPlungeAttack();
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
            movementController.HandleRoll();

        if (Input.GetKeyDown(KeyCode.F) && currentInteractable != null)
            currentInteractable.OnInteract();

        if (Input.GetKeyDown(KeyCode.E))
            abilityController.HandleAbility(0);
        if (Input.GetKeyDown(KeyCode.Q))
            abilityController.HandleAbility(1);

        if (Input.GetMouseButton(0) && !movementController.isClimbingLedge)
        {
            combatController.HandleAttack();
            movementController.StopPlayer();
        }
        else if (Input.GetMouseButton(1) && !movementController.isClimbingLedge)
        {
            if (combatController.HandleParry())
            {
                movementController.StopPlayer();
            }
        }
        else if (Input.GetMouseButtonUp(1))
        {
            combatController.OnReleaseParry();
        }

        if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.Space))
        {
            movementController.HandlePlunge();
        }

        if (!combatController.CheckAttacking() && !movementController.canMove)
            movementController.ResumePlayer();

        movementController.CheckGroundCollision();
        movementController.HandleMovment(horizontal);
        movementController.HandleGrappling(vertical, ropeX);
    }

    private void HandlePlungeAttack()
    {
        if (combatController.HandlePlungeAttack())
            movementController.StopPlayer();
    }

    public void OnPlayerOverlap(bool overlap)
    {
        movementController.OnPlayerOverlap(overlap);
    }

    private void FixedUpdate()
    {
        movementController.MovePlayer();
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
            currentInteractable.OnEnterRange();
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
            currentInteractable.OnLeaveRange();
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

        if (tookDamage)
        {
            playerEffectsController.ShakeCamera(4f, 5f, 0.2f);
            playerEffectsController.Pulse(0.5f, 3f, 0f, 0.3f, true);
        }
        else
        {
            playerEffectsController.HitStop(0.1f);

            if (immuneType == ImmuneType.Dodge)
            {
                // Icy Crampon
                int randNum = Random.Range(0, 100);
                if (randNum < itemStats.cramponChance)
                {
                    Damage proccDamage = CalculateProccDamageDealt(attacker,
                        new Damage((attack + attackIncrease.GetTotalModifier()) * itemStats.cramponDamageModifier),
                        out bool proccCrit,
                        out DamagePopup.DamageType proccDamageType);

                    attacker.TakeTrueDamage(proccDamage);
                    attacker.statusEffectManager.ApplyStatusEffect(StatusEffect.StatusType.Freeze, itemStats.cramponFreezeStacks);
                }
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

        float damage = base.CalculateDamageDealt(target, out isCrit, out damageType);

        // Knuckle Duster
        damageMultipler.RemoveModifier(itemStats.knuckleDusterDamageModifier);
        // CrudeKnife
        damageMultipler.RemoveModifier(itemStats.crudeKnifeDamageModifier);

        return damage;
    }

    public override IEnumerator FrozenRoutine()
    {
        movementController.StopPlayer();
        isFrozen = true;

        yield return new WaitForSeconds(statusEffectStats.frozenDuration);

        movementController.ResumePlayer();
        isFrozen = false;
    }

    public override IEnumerator StunnedRoutine()
    {
        movementController.StopPlayer();

        yield return new WaitForSeconds(statusEffectStats.stunDuration);

        movementController.ResumePlayer();
    }

    public override IEnumerator DazedRoutine()
    {
        movementController.StopPlayer();

        yield return new WaitForSeconds(statusEffectStats.stunDuration);

        movementController.ResumePlayer();
    }

    public void OnHitEnemyEvent(BaseStats target, Damage damage, bool isCrit, Vector3 closestPoint)
    {
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
                target.statusEffectManager.ApplyStatusEffect(StatusEffect.StatusType.Bleed, itemStats.ritualBleedStacks);
        }

        // Jagged Dagger
        randNum = Random.Range(0, 100);
        if (randNum < itemStats.daggerBleedChance)
            target.statusEffectManager.ApplyStatusEffect(StatusEffect.StatusType.Bleed, 1);

        // Frazzled Wire
        randNum = Random.Range(0, 100);
        if (randNum < itemStats.frazzledWireChance && !previousDamage.damageSource.Equals(Damage.DamageSource.FrazzledWire))
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
                targetEnemy.statusEffectManager.ApplyStatusEffect(StatusEffect.StatusType.Static, itemStats.frazzledWireStaticStacks);
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
            targetEnemy.statusEffectManager.ApplyStatusEffect(StatusEffect.StatusType.Burn, itemStats.gasolineBurnStacks);
        }
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