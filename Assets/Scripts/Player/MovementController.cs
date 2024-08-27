using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    [SerializeField] private MovementData movementData;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private Transform groundCheckPosition;

    [SerializeField] private Transform wallCheckPosition;

    private CapsuleCollider2D playerCol;
    private Rigidbody2D playerRB;

    public bool isMoving = false;
    public bool isGrounded = true;
    public bool lockMomentum = false;

    public bool canGrapple = false;
    public bool isGrappling = false;

    private Vector2 direction;
    private float moveSpeed;
    private float moveSpeedModifier = 1;

    private float fallingDuration;

    public int wallJumpCount;
    public int maxWallJumps;

    private Coroutine burstDragRoutine;
    private Coroutine dashRoutine;

    public void InitializeMovementController()
    {
        moveSpeed = movementData.walkSpeed;
        playerCol = GetComponent<CapsuleCollider2D>();
        playerRB = GetComponent<Rigidbody2D>();
    }

    public void HandleMovment(float horizontal)
    {
        isMoving = horizontal != 0;

        // Update player facing dir
        if (!lockMomentum)
        {
            if (horizontal > 0)
                transform.localScale = new Vector3(1, 1, 1);
            else if (horizontal < 0)
                transform.localScale = new Vector3(-1, 1, 1);
        }

        if (isMoving)
        {
            direction = Camera.main.transform.forward.normalized;

            Vector3 sideDirection = Vector3.ProjectOnPlane(Camera.main.transform.right * horizontal, Vector3.up);
            direction = sideDirection.normalized;
        }
        else if (!isMoving && isGrounded)
        {
            if (burstDragRoutine != null)
                StartCoroutine(BurstDrag());
            moveSpeed = movementData.walkSpeed;
        }

        if (!isGrounded)
            fallingDuration += Time.deltaTime;

        SpeedControl();
    }

    public void HandleGrappling(float vertical)
    {
        if (!canGrapple || vertical == 0)
            return;

        isGrappling = true;
        playerRB.gravityScale = 0;
        playerRB.velocity = Vector2.zero;

        transform.position = new Vector3(transform.position.x, transform.position.y + Time.deltaTime * 5 * vertical, transform.position.z);
    }

    public void StopGrappling()
    {
        playerRB.gravityScale = 2;
        isGrappling = false;
    }

    public void HandleJump()
    {
        if (isGrappling)
        {
            StopGrappling();
            return;
        }

        Collider2D col = Physics2D.OverlapCircle(wallCheckPosition.position, 0.2f, ~playerLayer);

        if (col == null)
        {
            if (!isGrounded && fallingDuration > movementData.cyoteTime)
                return;

            // Normal Jump
            playerRB.velocity = new Vector2(playerRB.velocity.x, 0);
            playerRB.AddForce(transform.up * movementData.baseJumpForce, ForceMode2D.Impulse);
        }
        else if (wallJumpCount > 0)
        {
            // Wall Jump
            playerRB.velocity = new Vector2(playerRB.velocity.x, 0);

            Vector2 dir = Vector2.up;
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

            if (col.transform.position.x < transform.position.x)
                dir = new Vector2(0.5f, 1.1f);
            else
                dir = new Vector2(-0.5f, 1.1f);

            playerRB.AddForce(dir * movementData.baseJumpForce, ForceMode2D.Impulse);

            wallJumpCount--;
            lockMomentum = true;
        }
    }

    public void HandleDash(float direction)
    {
        if (dashRoutine == null)
            dashRoutine = StartCoroutine(DashRoutine(direction));
    }

    private IEnumerator DashRoutine(float direction)
    {
        lockMomentum = false;
        float timer = movementData.dashDuration;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            playerRB.velocity = new Vector2(movementData.dashSpeed * direction, 0);
            yield return null;
        }

        yield return new WaitForSeconds(movementData.dashCooldown);

        dashRoutine = null;
    }

    public void MovePlayer()
    {
        if (!isMoving || lockMomentum || isGrappling)
            return;

        Vector3 force;

        // Adjust drag & force
        if (isGrounded)
            force = direction * (moveSpeed + playerRB.velocity.magnitude) * 5f;
        else if (!isGrounded)
            force = direction * moveSpeed * 10f * movementData.airMultiplier * moveSpeedModifier;
        else
            force = Vector3.zero;

        // Move player
        playerRB.AddForce(force * moveSpeedModifier, ForceMode2D.Force);
    }

    public void CheckGroundCollision()
    {
        RaycastHit2D groundHit = Physics2D.Raycast(groundCheckPosition.position, Vector3.down, 100, groundLayer);
        if (!groundHit)
            return;

        float dist = Vector3.Distance(groundCheckPosition.position, groundHit.point);
        if (dist <= movementData.minGroundDist)
        {
            isGrounded = true;
            playerRB.drag = movementData.groundDrag;
            fallingDuration = 0;
            wallJumpCount = maxWallJumps;
            lockMomentum = false;

            if (isGrappling)
                StopGrappling();
        }
        else if (dist > movementData.minGroundDist)
        {
            isGrounded = false;
            playerRB.drag = movementData.airDrag;
        }
    }

    public void StopPlayer()
    {
        isMoving = false;
        playerRB.velocity = Vector3.zero;
    }

    public void SetMoveSpeedModifier(float newModifier)
    {
        moveSpeedModifier = newModifier;
    }

    private void SpeedControl()
    {
        Vector2 currentVel = new Vector2(playerRB.velocity.x, 0);

        if (currentVel.magnitude > moveSpeed)
        {
            Vector3 limitVel = currentVel.normalized * moveSpeed;
            playerRB.velocity = new Vector2(limitVel.x, playerRB.velocity.y);
        }
    }

    private IEnumerator BurstDrag()
    {
        playerRB.drag = 15;
        yield return new WaitForSeconds(0.3f);
        playerRB.drag = movementData.groundDrag;
        burstDragRoutine = null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(wallCheckPosition.position, 0.2f);
    }
}