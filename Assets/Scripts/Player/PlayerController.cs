using System.Collections;
using UnityEngine;

public class PlayerController : PlayerStats
{
    public static PlayerController Instance;

    private MovementController movementController;
    private FadeTransition fadeTransition;

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
        fadeTransition = GetComponent<FadeTransition>();

        movementController.InitializeMovementController();
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

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (checkPlungeRoutine == null)
                checkPlungeRoutine = StartCoroutine(CheckPlungeRoutine());
        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            if (checkPlungeRoutine != null)
                StopCoroutine(checkPlungeRoutine);
            checkPlungeRoutine = null;
        }

        movementController.CheckGroundCollision();
        movementController.HandleMovment(horizontal);
        movementController.HandleGrappling(vertical, ropeX);
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
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Rope"))
        {
            movementController.StopGrappling();
            movementController.canGrapple = false;
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