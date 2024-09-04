using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : PlayerStats
{
    public static PlayerController Instance;

    private Collider2D playerCol;
    private MovementController movementController;
    private CombatController combatController;
    private AbilityController abilityController;
    private FadeTransition fadeTransition;
    private ItemManager itemManager;
    [SerializeField] private ProceduralMapGenerator proceduralMapGenerator;

    private IInteractable currentInteractable;

    private float ropeX;

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
        playerCol = GetComponent<Collider2D>();

        itemManager.InitItemManager();
        movementController.InitializeMovementController();
        combatController.InitializeCombatController();
        abilityController.InitializeAbilityController();
        proceduralMapGenerator.InitMapGenerator();
    }

    private void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.Space) && !Input.GetKey(KeyCode.S))
            movementController.HandleJump(horizontal);

        if (Input.GetKeyDown(KeyCode.LeftShift))
            movementController.HandleDash(horizontal);

        if (Input.GetKeyDown(KeyCode.LeftControl))
            movementController.HandleRoll();

        if (Input.GetKeyDown(KeyCode.F) && currentInteractable != null)
            currentInteractable.OnInteract();

        if (Input.GetKeyDown(KeyCode.E))
            abilityController.HandleAbility(this, 0);
        if (Input.GetKeyDown(KeyCode.Q))
            abilityController.HandleAbility(this, 1);

        if (Input.GetMouseButton(0))
        {
            combatController.HandleAttack();
            movementController.StopPlayer();
        }

        if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.Space))
        {
            movementController.HandlePlunge();
        }

        if (!combatController.CheckAttacking() && !movementController.canMove)
            movementController.ResumePlayer();

        if (movementController.canPlungeDamage)
        {
            combatController.HandlePlungeAttack();
            movementController.canPlungeDamage = false;
            movementController.StopPlayer();
        }

        movementController.CheckGroundCollision();
        movementController.HandleMovment(horizontal);
        movementController.HandleGrappling(vertical, ropeX);
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

    public override void TakeDamage(float damage, int critRate, float critMultiplier, Vector3 closestPoint)
    {
        base.TakeDamage(damage, critRate, critMultiplier, closestPoint);

        if (health <= 0)
        {
            Debug.Log("YOU DIED!");
        }
    }
}