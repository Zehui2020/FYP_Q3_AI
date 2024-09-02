using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : PlayerStats
{
    public static PlayerController Instance;

    private Collider2D playerCol;
    private MovementController movementController;
    private CombatController combatController;
    private FadeTransition fadeTransition;
    private ItemManager itemManager;

    private IInteractable currentInteractable;

    private float ropeX;

    [SerializeField] private float plungeHoldDuration;
    private Coroutine checkPlungeRoutine;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        movementController = GetComponent<MovementController>();
        combatController = GetComponent<CombatController>();
        fadeTransition = GetComponent<FadeTransition>();
        itemManager = GetComponent<ItemManager>();
        playerCol = GetComponent<Collider2D>();

        itemManager.InitItemManager();
        movementController.InitializeMovementController();
        combatController.InitializeCombatController();
    }

    private void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.Space))
            movementController.HandleJump(horizontal);

        if (Input.GetKeyDown(KeyCode.LeftShift))
            movementController.HandleDash(horizontal);

        if (Input.GetKeyDown(KeyCode.LeftControl))
            movementController.HandleRoll();

        if (Input.GetKeyDown(KeyCode.E) && currentInteractable != null)
            currentInteractable.OnInteract();

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (checkPlungeRoutine == null)
                checkPlungeRoutine = StartCoroutine(CheckPlungeRoutine());

            combatController.HandleAttack();
            movementController.StopPlayer();
        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            if (checkPlungeRoutine != null)
                StopCoroutine(checkPlungeRoutine);
            checkPlungeRoutine = null;
        }

        if (!combatController.CheckAttacking() && !movementController.canMove)
            movementController.ResumePlayer();

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

    private IEnumerator CheckPlungeRoutine()
    {
        yield return new WaitForSeconds(plungeHoldDuration);

        while (true)
        {
            if (movementController.HandlePlunge())
            {
                checkPlungeRoutine = null;
                yield break;
            }

            yield return null;
        }
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
}