using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : PlayerStats
{
    private MovementController movementController;

    private void Start()
    {
        movementController = GetComponent<MovementController>();

        movementController.InitializeMovementController();
    }

    private void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.Space))
            movementController.HandleJump();

        if (Input.GetKeyDown(KeyCode.LeftShift))
            movementController.HandleDash(horizontal);

        movementController.CheckGroundCollision();
        movementController.HandleMovment(horizontal);
        movementController.HandleGrappling(vertical);
    }

    private void FixedUpdate()
    {
        movementController.MovePlayer();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Rope"))
            movementController.canGrapple = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Rope"))
            movementController.canGrapple = false;
    }
}