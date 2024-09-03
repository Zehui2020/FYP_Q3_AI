using System.Collections;
using System.Drawing;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    [SerializeField] private MovementData movementData;
    private AnimationManager animationManager;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallJumpCheck;
    [SerializeField] private Transform groundCheckPosition;

    [SerializeField] private Transform wallCheckPosition;
    [SerializeField] private Transform ledgeCheckPosition;

    private CapsuleCollider2D playerCol;
    private Rigidbody2D playerRB;

    public bool canMove = true;

    public bool isMoving = false;
    public bool isGrounded = true;
    public bool isDashing = false;
    public bool isClimbingLedge = false;

    public bool lockMomentum = false;
    public bool lockDirection = false;

    public bool canGrapple = false;
    public bool isGrappling = false;

    private Vector2 ledgePosBot;
    private Vector2 ledgePos1;
    private Vector2 ledgePos2;

    private Vector2 direction;
    private float moveSpeed;
    private float moveSpeedModifier = 1;

    private float fallingDuration;
    private bool isLanding = false;

    public int jumpCount;
    public int maxJumpCount;

    public int wallJumpCount;
    public int maxWallJumps;

    private Coroutine jumpRoutine;
    private Coroutine burstDragRoutine;
    private Coroutine dashRoutine;
    private Coroutine rollRoutine;
    private Coroutine plungeRoutine;

    public void InitializeMovementController()
    {
        moveSpeed = movementData.walkSpeed;
        playerCol = GetComponent<CapsuleCollider2D>();
        playerRB = GetComponent<Rigidbody2D>();
        animationManager = GetComponent<AnimationManager>();

        animationManager.InitAnimationController();

        moveSpeed = movementData.walkSpeed;
    }

    public void HandleMovment(float horizontal)
    {
        if (!canMove)
            return;

        if (!isGrappling)
            HandleLedgeGrab();

        isMoving = horizontal != 0;

        // Update player facing dir
        if (!lockDirection)
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

            if (jumpRoutine == null && rollRoutine == null && !isDashing)
                animationManager.ChangeAnimation(animationManager.Idle, 0f, 0f, false);
        }

        if (!isGrounded)
            fallingDuration += Time.deltaTime;

        SpeedControl();

        if (playerRB.velocity.y < 0 && !isLanding)
        {
            lockMomentum = false;
            lockDirection = false;
            animationManager.ChangeAnimation(animationManager.Falling, 0, 0, false);
        }

        Debug.DrawRay(groundCheckPosition.position, Vector3.down * movementData.plungeThreshold, UnityEngine.Color.red);
    }

    public void HandleGrappling(float vertical, float posX)
    {
        if (plungeRoutine != null)
            return;

        if (!isGrappling)
        {
            if (!canGrapple || vertical == 0)
                return;
        }

        if (vertical == 0)
            animationManager.ChangeAnimation(animationManager.GrappleIdle, 0, 0, false);
        else
            animationManager.ChangeAnimation(animationManager.Grappling, 0, 0, false);

        transform.position = new Vector3(Mathf.Lerp(transform.position.x, posX, Time.deltaTime * 10f),
            transform.position.y,
            transform.position.z);

        isGrappling = true;
        playerRB.gravityScale = 0;
        playerRB.velocity = Vector2.zero;
        playerCol.isTrigger = true;

        transform.position = new Vector3(transform.position.x, transform.position.y + Time.deltaTime * movementData.grappleSpeed * vertical, transform.position.z);
    }

    public void StopGrappling()
    {
        playerRB.gravityScale = movementData.gravityScale;
        isGrappling = false;
        playerCol.isTrigger = false;
    }

    public void HandleJump(float horizontal)
    {
        if (plungeRoutine != null || rollRoutine != null || isClimbingLedge)
            return;

        if (isGrappling)
            StopGrappling();

        bool isTouchingWall;
        Vector2 dir;
        if (transform.localScale.x < 0)
            dir = -transform.right;
        else
            dir = transform.right;

        Debug.DrawRay(wallCheckPosition.position, dir * movementData.wallCheckDist, UnityEngine.Color.red);
        Collider2D col = Physics2D.Raycast(wallCheckPosition.position, dir, 0.2f, wallJumpCheck).collider;
        isTouchingWall = col != null;

        if (!isTouchingWall || wallJumpCount <= 0)
        {
            // Normal Jump
            if (fallingDuration > movementData.cyoteTime && jumpCount == maxJumpCount)
                return;

            if (jumpCount <= 0 && !isGrounded)
                return;

            if (jumpRoutine == null)
                jumpRoutine = StartCoroutine(JumpRoutine(horizontal));
        }
        else if (wallJumpCount > 0)
        {
            // Wall Jump
            animationManager.ChangeAnimation(animationManager.WallJump, 0, 0, true);

            playerRB.velocity = new Vector2(playerRB.velocity.x, 0);
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

            if (col.ClosestPoint(transform.position).x < transform.position.x)
                dir = new Vector2(movementData.wallJumpForceX, movementData.wallJumpForceY);
            else
                dir = new Vector2(-movementData.wallJumpForceX, movementData.wallJumpForceY);

            playerRB.AddForce(dir * movementData.baseJumpForce, ForceMode2D.Impulse);

            wallJumpCount--;
            lockMomentum = true;
            lockDirection = true;
        }
    }

    private void HandleLedgeGrab()
    {
        if (isClimbingLedge)
            return;

        Vector2 dir;
        if (transform.localScale.x < 0)
            dir = -transform.right;
        else
            dir = transform.right;

        Debug.DrawRay(wallCheckPosition.position, dir * movementData.ledgeCheckDist, UnityEngine.Color.red);
        Debug.DrawRay(ledgeCheckPosition.position, dir * movementData.ledgeCheckDist, UnityEngine.Color.red);

        RaycastHit2D wallHit = Physics2D.Raycast(wallCheckPosition.position, dir, movementData.ledgeCheckDist, wallJumpCheck);
        RaycastHit2D ledgeHit = Physics2D.Raycast(ledgeCheckPosition.position, dir, movementData.ledgeCheckDist, wallJumpCheck);

        // Check if player is touching wall but not touching ledge
        if (ledgeHit || !wallHit)
            return;

        ledgePosBot = wallHit.point;
        playerCol.isTrigger = true;
        playerRB.gravityScale = 0;

        // If facing right
        if (transform.localScale.x > 0)
        {
            ledgePos1 = new Vector2(ledgePosBot.x - movementData.ledgeXOffset1, ledgePosBot.y + movementData.ledgeYOffset1);
            ledgePos2 = new Vector2(ledgePosBot.x + movementData.ledgeXOffset2, ledgePosBot.y + movementData.ledgeYOffset2);
        }
        else
        {
            ledgePos1 = new Vector2(ledgePosBot.x + movementData.ledgeXOffset1, ledgePosBot.y + movementData.ledgeYOffset1);
            ledgePos2 = new Vector2(ledgePosBot.x - movementData.ledgeXOffset2, ledgePosBot.y + movementData.ledgeYOffset2);
        }

        lockMomentum = true;
        lockDirection = true;
        playerRB.velocity = Vector2.zero;
        playerRB.gravityScale = 0;

        transform.position = ledgePos1;

        animationManager.ChangeAnimation(animationManager.WallClimb, 0, 0, false);

        isClimbingLedge = true;
    }

    public void FinishLedgeClimb()
    {
        lockMomentum = false;
        lockDirection = false;
        transform.position = ledgePos2;
        playerRB.gravityScale = movementData.gravityScale;
        isClimbingLedge = false;
        playerCol.isTrigger = false;
        playerRB.gravityScale = movementData.gravityScale;
    }

    private IEnumerator JumpRoutine(float horizontal)
    {
        CancelDash();
        jumpCount--;
        float velX = playerRB.velocity.x;
        lockMomentum = false;

        if (horizontal < 0)
        {
            if (velX > 0)
                velX = -velX;
        }
        else if (horizontal > 0)
        {
            velX = Mathf.Abs(velX);
        }

        if (maxJumpCount - jumpCount > 1)
        {
            animationManager.ChangeAnimation(animationManager.DoubleJump, 0, 0, true);
            velX /= 1.25f;
        }
        else
        {
            animationManager.ChangeAnimation(animationManager.Jumping, 0, 0, true);
        }

        playerRB.velocity = new Vector2(velX, 0);
        playerRB.AddForce(transform.up * movementData.baseJumpForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(movementData.jumpInterval);

        jumpRoutine = null;
    }

    public void HandleDash(float direction)
    {
        if (dashRoutine == null && !isClimbingLedge)
            dashRoutine = StartCoroutine(DashRoutine(direction));
    }

    private IEnumerator DashRoutine(float direction)
    {
        lockMomentum = false;
        lockDirection = true;

        float timer;
        PlayerController.Instance.ApplyImmune(movementData.dashIFrames, BaseStats.ImmuneType.Dodge);
        playerRB.gravityScale = 0;
        playerRB.velocity = new Vector2(playerRB.velocity.x, 0);

        float dashSpeed;

        if (!isGrounded)
        {
            animationManager.ChangeAnimation(animationManager.AirDash, 0, 0, true);
            dashSpeed = movementData.dashSpeed;
            timer = movementData.dashDuration;
        }
        else
        {
            animationManager.ChangeAnimation(animationManager.GroundDash, 0, 0, true);
            dashSpeed = movementData.groundDashSpeed;
            timer = movementData.groundDashDuration;
        }

        while (timer > 0)
        {
            timer -= Time.deltaTime;

            if (direction == 0)
            {
                if (transform.localScale.x > 0)
                    direction = 1;
                else
                    direction = -1;
            }

            isDashing = true;
            playerRB.velocity = new Vector2(dashSpeed * direction, playerRB.velocity.y);
            yield return null;
        }

        isDashing = false;
        playerRB.gravityScale = movementData.gravityScale;
        lockDirection = false;

        yield return new WaitForSeconds(movementData.dashCooldown);

        dashRoutine = null;
    }

    private void CancelDash()
    {
        if (dashRoutine != null)
            StopCoroutine(dashRoutine);

        dashRoutine = null;
        isDashing = false;
        playerRB.gravityScale = movementData.gravityScale;
    }

    public void HandleRoll()
    {
        if (!isGrounded)
            return;

        if (rollRoutine == null)
            rollRoutine = StartCoroutine(RollRoutine());
    }

    private IEnumerator RollRoutine()
    {
        CancelDash();

        lockMomentum = true;
        lockDirection = true;

        Vector2 originalSize = playerCol.size;
        Vector2 originalOffset = playerCol.offset;

        float change = movementData.rollColliderSize - playerCol.size.y;
        float rollDuration;
        playerRB.drag = 0;

        playerCol.size = new Vector2(playerCol.size.x, movementData.rollColliderSize);
        playerCol.offset = new Vector2(playerCol.offset.x, -(Mathf.Abs(change) / 2));

        if (playerRB.velocity.magnitude < 5f)
        {
            animationManager.ChangeAnimation(animationManager.Roll, 0, 0, false);
            playerRB.AddForce(new Vector3(transform.localScale.x, 0, 0) * 2, ForceMode2D.Impulse);
            rollDuration = movementData.rollDuration;
        }
        else
        {
            animationManager.ChangeAnimation(animationManager.LungeRoll, 0, 0, false);
            playerRB.AddForce(-playerRB.velocity.normalized * movementData.rollFriction, ForceMode2D.Impulse);
            rollDuration = movementData.lungeRollDuration;
        }

        yield return new WaitForSeconds(rollDuration);

        playerCol.size = originalSize;
        playerCol.offset = originalOffset;

        playerRB.drag = movementData.groundDrag;
        lockMomentum = false;
        lockDirection = false;
        rollRoutine = null;
    }

    public bool HandlePlunge()
    {
        if (plungeRoutine != null)
            return false;

        RaycastHit2D groundHit = Physics2D.Raycast(groundCheckPosition.position, Vector3.down, movementData.plungeThreshold, groundLayer);
        if (groundHit)
            return false;

        StopGrappling();
        plungeRoutine = StartCoroutine(PlungeRoutine());

        return true;
    }

    private IEnumerator PlungeRoutine()
    {
        playerRB.velocity = Vector2.zero;
        playerRB.gravityScale = 0;

        yield return new WaitForSeconds(movementData.plungeDelay);

        playerRB.gravityScale = movementData.gravityScale;
        playerRB.AddForce(Vector2.down * movementData.plungeForce, ForceMode2D.Impulse);
    }

    public void MovePlayer()
    {
        if (!isMoving || lockMomentum || isGrappling || plungeRoutine != null || !canMove)
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

        if (isGrounded && jumpRoutine == null && !isDashing)
            animationManager.ChangeAnimation(animationManager.Running, 0f, 0f, false);
    }

    public void CheckGroundCollision()
    {
        RaycastHit2D groundHit = Physics2D.Raycast(groundCheckPosition.position, Vector3.down, 100, groundLayer);
        if (!groundHit || isClimbingLedge)
            return;

        float dist = Vector3.Distance(groundCheckPosition.position, groundHit.point);

        if (!isGrounded && playerRB.velocity.y < 0 && dist <= 2f)
        {
            isLanding = true;
            animationManager.ChangeAnimation(animationManager.Land, 0, 0, false);
        }
        else if (dist > 2f)
        {
            isLanding = false;
        }

        if (dist <= movementData.minGroundDist)
        {
            isLanding = false;
            isGrounded = true;
            fallingDuration = 0;

            if (rollRoutine == null)
            {
                playerRB.drag = movementData.groundDrag;
                lockMomentum = false;
            }

            wallJumpCount = maxWallJumps;
            if (jumpRoutine == null)
                jumpCount = maxJumpCount;

            plungeRoutine = null;

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
        canMove = false;
        playerRB.velocity = Vector3.zero;
        playerRB.gravityScale = 0;
    }

    public void ResumePlayer()
    {
        canMove = true;
        playerRB.velocity = Vector3.zero;
        playerRB.gravityScale = movementData.gravityScale;
    }

    public void SetMoveSpeedModifier(float newModifier)
    {
        moveSpeedModifier = newModifier;
    }

    public void OnPlayerOverlap(bool inRange)
    {
        if (inRange)
            moveSpeed = movementData.overlapSpeed;
        else
            moveSpeed = movementData.walkSpeed;
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
}