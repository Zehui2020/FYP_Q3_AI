using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    public enum MovementState
    {
        Idle,
        Running,
        Jump,
        DoubleJump,
        WallJump,
        GroundDash,
        AirDash,
        Falling,
        Land,
        Plunge,
        LedgeGrab,
        Grapple,
        GrappleIdle,
        Roll,
        LungeRoll,
        Knockback
    }
    public MovementState currentState;

    [SerializeField] private MovementData movementData;

    [SerializeField] private LayerMask platformLayer;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallJumpCheck;
    [SerializeField] private Transform groundCheckPosition;

    [SerializeField] private Transform wallCheckPosition;
    [SerializeField] private Transform ledgeCheckPosition;

    private CapsuleCollider2D playerCol;
    private Rigidbody2D playerRB;
    private AnimationManager animationManager;
    private PlayerEffectsController playerEffectsController;

    public bool isGrounded = false;
    public bool canMove = true;
    public bool canGrapple = false;

    private Vector2 ledgePosBot;
    private Vector2 ledgePos1;
    private Vector2 ledgePos2;

    private Vector2 direction;

    private float fallingDuration;
    public int jumpCount;
    public int maxJumpCount;

    public int wallJumpCount;
    public int maxWallJumps;

    private float moveSpeed;

    private Coroutine jumpRoutine;
    private Coroutine burstDragRoutine;
    private Coroutine dashRoutine;
    private Coroutine plungeRoutine;
    private Coroutine knockbackRoutine;

    public event System.Action OnPlungeEnd;
    public event System.Action<PlayerController.PlayerStates> ChangePlayerState;

    private Rope rope;

    public void InitializeMovementController(AnimationManager animationManager, Rigidbody2D playerRB)
    {
        playerCol = GetComponent<CapsuleCollider2D>();
        this.playerRB = playerRB;

        this.animationManager = animationManager;
        moveSpeed = movementData.walkSpeed;
        playerEffectsController = PlayerEffectsController.Instance;
    }

    public void ChangeState(MovementState movementState)
    {
        switch (movementState)
        {
            case MovementState.Idle:
                animationManager.ChangeAnimation(animationManager.Idle, 0f, 0f, AnimationManager.AnimType.None);
                CancelDash();
                break;
            case MovementState.Running:
                animationManager.ChangeAnimation(animationManager.Running, 0f, 0f, AnimationManager.AnimType.None);
                break;
            case MovementState.Jump:
                animationManager.ChangeAnimation(animationManager.Jumping, 0f, 0f, AnimationManager.AnimType.None);
                break;
            case MovementState.DoubleJump:
                animationManager.ChangeAnimation(animationManager.DoubleJump, 0f, 0f, AnimationManager.AnimType.None);
                break;
            case MovementState.WallJump:
                animationManager.ChangeAnimation(animationManager.WallJump, 0f, 0f, AnimationManager.AnimType.None);
                break;
            case MovementState.GroundDash:
                animationManager.ChangeAnimation(animationManager.GroundDash, 0f, 0f, AnimationManager.AnimType.None);
                break;
            case MovementState.AirDash:
                animationManager.ChangeAnimation(animationManager.AirDash, 0f, 0f, AnimationManager.AnimType.None);
                break;
            case MovementState.Falling:
                animationManager.ChangeAnimation(animationManager.Falling, 0f, 0f, AnimationManager.AnimType.None);
                CancelKnockback();
                break;
            case MovementState.Land:
                animationManager.ChangeAnimation(animationManager.Land, 0f, 0f, AnimationManager.AnimType.None);
                break;
            case MovementState.LedgeGrab:
                animationManager.ChangeAnimation(animationManager.WallClimb, 0f, 0f, AnimationManager.AnimType.None);
                break;
            case MovementState.Grapple:
                animationManager.ChangeAnimation(animationManager.Grappling, 0, 0, AnimationManager.AnimType.None);
                break;
            case MovementState.GrappleIdle:
                animationManager.ChangeAnimation(animationManager.GrappleIdle, 0, 0, AnimationManager.AnimType.None);
                break;
            case MovementState.Roll:
                animationManager.ChangeAnimation(animationManager.Roll, 0, 0, AnimationManager.AnimType.None);
                break;
            case MovementState.LungeRoll:
                animationManager.ChangeAnimation(animationManager.LungeRoll, 0, 0, AnimationManager.AnimType.None);
                break;
        }

        currentState = movementState;
    }

    public void Knockback(float initialSpeed)
    {
        ChangeState(MovementState.Knockback);

        if (knockbackRoutine != null)
            StopCoroutine(knockbackRoutine);
        knockbackRoutine = StartCoroutine(KnockbackRoutine(initialSpeed));
    }

    private IEnumerator KnockbackRoutine(float force)
    {
        Vector2 dir = transform.localScale.x > 0 ? -transform.right : transform.right;
        playerRB.AddForce(dir * force, ForceMode2D.Impulse);
        playerRB.drag = movementData.groundDrag;

        while (playerRB.velocity.magnitude > 0.1f)
        {
            yield return null;
        }

        knockbackRoutine = null;

        if (PlayerController.Instance.health <= 0)
            yield break;

        ChangeState(MovementState.Idle);
        ChangePlayerState?.Invoke(PlayerController.PlayerStates.Movement);
    }

    public void CancelKnockback()
    {
        if (knockbackRoutine == null)
            return;

        StopCoroutine(knockbackRoutine);
        knockbackRoutine = null;
        ChangeState(MovementState.Idle);
        ChangePlayerState?.Invoke(PlayerController.PlayerStates.Movement);
    }

    public void HandleMovment(float horizontal)
    {
        if (!canMove || currentState == MovementState.Knockback || playerRB == null)
            return;

        // Afterimage
        if (dashRoutine != null)
            playerEffectsController.StartSpawnAfterimage();
        else
            playerEffectsController.StopSpawnAfterimage();

        // Calculate move dir
        direction = Camera.main.transform.forward.normalized;
        Vector3 sideDirection = Vector3.ProjectOnPlane(Camera.main.transform.right * horizontal, Vector3.up);
        direction = sideDirection.normalized;

        // Update player facing dir
        if (currentState == MovementState.Running ||
            currentState == MovementState.Falling)
        {
            if (horizontal > 0)
                transform.localScale = new Vector3(1, 1, 1);
            else if (horizontal < 0)
                transform.localScale = new Vector3(-1, 1, 1);
        }

        if (horizontal == 0 &&
            playerRB.velocity.magnitude <= 3f &&
            currentState != MovementState.Roll &&
            currentState != MovementState.LedgeGrab &&
            currentState != MovementState.GroundDash &&
            currentState != MovementState.Knockback &&
            currentState != MovementState.Plunge &&
            currentState != MovementState.Grapple &&
            currentState != MovementState.GrappleIdle &&
            isGrounded)
            ChangeState(MovementState.Idle);

        if (playerRB.velocity.y < -0.1f && 
            !isGrounded &&
            currentState != MovementState.Land &&
            currentState != MovementState.Plunge &&
            currentState != MovementState.Grapple &&
            currentState != MovementState.GrappleIdle)
            ChangeState(MovementState.Falling);
        else if (currentState == MovementState.Idle && horizontal != 0)
            ChangeState(MovementState.Running);

        switch (currentState)
        {
            case MovementState.Idle:
                if (burstDragRoutine != null)
                    StartCoroutine(BurstDrag());
                break;
            case MovementState.Falling:
                fallingDuration += Time.deltaTime;
                break;
        }

        SpeedControl();
        HandleLedgeGrab();
    }

    public void HandleGrappling(float vertical, float posX)
    {
        if (plungeRoutine != null ||
            !canGrapple ||
            currentState == MovementState.LedgeGrab ||
            currentState == MovementState.Plunge ||
            rope == null)
            return;

        if (vertical == 0)
        {
            if (currentState == MovementState.Grapple)
                ChangeState(MovementState.GrappleIdle);
            return;
        }

        if (rope != null && vertical > 0 && rope.CheckCannotGrappleUp(transform))
        {
            StopGrappling();
            ChangeState(MovementState.Idle);
            return;
        }
        else if (rope != null && vertical < 0 && rope.CheckCannotGrappleDown(transform))
        {
            StopGrappling();
            ChangeState(MovementState.Falling);
            return;
        }

        rope.GrappleStart();
        ChangeState(MovementState.Grapple);

        transform.position = new Vector3(posX, transform.position.y, transform.position.z);

        playerRB.gravityScale = 0;
        playerRB.drag = 0;
        playerRB.velocity = Vector2.zero;

        float grappleMovement = movementData.grappleSpeed * vertical;
        playerRB.MovePosition(new Vector2(transform.position.x, transform.position.y + grappleMovement * Time.fixedDeltaTime));
    }

    public void StopGrappling()
    {
        playerRB.gravityScale = movementData.gravityScale;
        playerRB.drag = movementData.airDrag;

        rope.GrappleEnd();
        rope = null;
    }

    public void OnJump(float horizontal)
    {
        if (currentState == MovementState.Plunge || 
            currentState == MovementState.Roll || 
            currentState == MovementState.LungeRoll || 
            currentState == MovementState.Knockback)
            return;

        bool isTouchingWall;
        Vector2 dir;
        if (transform.localScale.x < 0)
            dir = -transform.right;
        else
            dir = transform.right;

        Debug.DrawRay(wallCheckPosition.position, dir * movementData.wallCheckDist, UnityEngine.Color.red);

        Collider2D col = Physics2D.Raycast(wallCheckPosition.position, dir, 0.2f, wallJumpCheck).collider;
        isTouchingWall = col != null;

        if (jumpRoutine != null)
            return;

        if (isTouchingWall && wallJumpCount > 0 && jumpCount != maxJumpCount)
            HandleWallJump(col, horizontal);
        else
            HandleJump(horizontal);
    }

    public void HandleJump(float horizontal)
    {
        if (currentState == MovementState.Grapple ||
            currentState == MovementState.GrappleIdle)
        {
            StopGrappling();
            ChangeState(MovementState.Falling);
            return;
        }

        if (fallingDuration > movementData.cyoteTime && jumpCount == maxJumpCount)
            return;

        if (jumpCount <= 0 && !isGrounded || currentState == MovementState.Knockback || currentState == MovementState.LedgeGrab)
            return;

        ChangeState(MovementState.Jump);
        jumpRoutine = StartCoroutine(JumpRoutine(horizontal));
    }
    private IEnumerator JumpRoutine(float horizontal)
    {
        CancelDash();
        jumpCount--;
        float velX = playerRB.velocity.x;

        if (horizontal < 0)
        {
            if (velX > 0)
                velX = -velX;
        }
        else if (horizontal > 0)
        {
            velX = Mathf.Abs(velX);
        }

        if (horizontal < 0)
            transform.localScale = new Vector3(-1, 1, 1);
        else if (horizontal > 0)
            transform.localScale = new Vector3(1, 1, 1);

        if (maxJumpCount - jumpCount > 1)
        {
            if (Mathf.Sign(horizontal) != Mathf.Sign(playerRB.velocity.x))
                velX *= movementData.oppositeJumpMultiplier;

            ChangeState(MovementState.DoubleJump);
            playerEffectsController.PlayDoubleJumpPS();
        }

        playerRB.velocity = new Vector2(velX, 0);
        playerRB.AddForce(transform.up * movementData.baseJumpForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(movementData.jumpInterval);

        jumpRoutine = null;
    }

    public void HandleWallJump(Collider2D col, float horizontal)
    {
        ChangeState(MovementState.WallJump);
        jumpRoutine = StartCoroutine(WallJumpRoutine(col, horizontal));
    }
    private IEnumerator WallJumpRoutine(Collider2D col, float horizontal)
    {
        wallJumpCount--;

        Vector2 dir;
        playerRB.velocity = new Vector2(playerRB.velocity.x, 0);
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

        if (col.ClosestPoint(transform.position).x < transform.position.x)
            dir = new Vector2(movementData.wallJumpForceX, movementData.wallJumpForceY);
        else
            dir = new Vector2(-movementData.wallJumpForceX, movementData.wallJumpForceY);

        dir = horizontal == 0 ? new Vector2(0, dir.y) : dir;

        playerRB.AddForce(dir * movementData.wallJumpForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(movementData.jumpInterval);

        jumpRoutine = null;
    }

    private void HandleLedgeGrab()
    {
        if (currentState == MovementState.Plunge ||
            currentState == MovementState.Grapple ||
            currentState == MovementState.GrappleIdle)
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

        //Check if player is touching platform
        if (wallHit.collider.gameObject.GetComponent<PlatformEffector2D>())
            return;

        ChangeState(MovementState.LedgeGrab);
        ledgePosBot = wallHit.point;
        playerCol.isTrigger = true;
        playerRB.gravityScale = 0;
        CancelDash();

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

        playerRB.velocity = Vector2.zero;
        playerRB.gravityScale = 0;

        transform.position = ledgePos1;
    }

    public void FinishLedgeClimb()
    {
        transform.position = ledgePos2;
        playerRB.gravityScale = movementData.gravityScale;
        playerCol.isTrigger = false;
        playerRB.gravityScale = movementData.gravityScale;

        ChangeState(MovementState.Idle);
    }

    public bool HandleDash(float direction)
    {
        if (dashRoutine == null &&
            currentState != MovementState.Knockback &&
            currentState != MovementState.LedgeGrab &&
            currentState != MovementState.Roll &&
            currentState != MovementState.LungeRoll &&
            currentState != MovementState.Plunge &&
            currentState != MovementState.Grapple &&
            currentState != MovementState.GrappleIdle)
        {
            dashRoutine = StartCoroutine(DashRoutine(direction));
            return true;
        }

        return false;
    }
    private IEnumerator DashRoutine(float direction)
    {
        float timer;
        PlayerController.Instance.ApplyImmune(movementData.dashIFrames, BaseStats.ImmuneType.Dodge);
        playerRB.gravityScale = 0;
        playerRB.velocity = new Vector2(playerRB.velocity.x, 0);

        float dashSpeed;

        if (!isGrounded)
        {
            playerEffectsController.PlayDashPS(false);
            ChangeState(MovementState.AirDash);
            dashSpeed = movementData.dashSpeed;
            timer = movementData.dashDuration;
        }
        else
        {
            playerEffectsController.PlayDashPS(true);
            ChangeState(MovementState.GroundDash);
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
            else if (direction < 0)
                transform.localScale = new Vector3(-1, 1, 1);
            else
                transform.localScale = new Vector3(1, 1, 1);

            playerRB.velocity = new Vector2(dashSpeed * direction, playerRB.velocity.y);
            yield return null;
        }

        playerRB.gravityScale = movementData.gravityScale;
        playerEffectsController.StopDashPS();

        if (currentState != MovementState.Plunge)
        {
            if (isGrounded)
                ChangeState(MovementState.Idle);
            else
                ChangeState(MovementState.Falling);
        }

        yield return new WaitForSeconds(movementData.dashCooldown);

        dashRoutine = null;
    }

    public void CancelDash()
    {
        if (dashRoutine != null)
            StopCoroutine(dashRoutine);

        dashRoutine = null;
        playerRB.gravityScale = movementData.gravityScale;
    }

    public void HandleRoll()
    {
        if (!isGrounded || 
            currentState == MovementState.Knockback ||
            currentState == MovementState.Roll ||
            currentState == MovementState.LungeRoll)
            return;

        StartCoroutine(RollRoutine());
    }

    private IEnumerator RollRoutine()
    {
        CancelDash();

        Vector2 originalSize = playerCol.size;
        Vector2 originalOffset = playerCol.offset;

        float change = movementData.rollColliderSize - playerCol.size.y;
        float rollDuration;
        playerRB.drag = 0;

        playerCol.size = new Vector2(playerCol.size.x, movementData.rollColliderSize);
        playerCol.offset = new Vector2(playerCol.offset.x, -(Mathf.Abs(change) / 2));

        if (playerRB.velocity.magnitude < 5f)
        {
            ChangeState(MovementState.Roll);
            playerRB.AddForce(new Vector3(transform.localScale.x, 0, 0) * 2, ForceMode2D.Impulse);
            rollDuration = movementData.rollDuration;
        }
        else
        {
            ChangeState(MovementState.LungeRoll);
            playerRB.AddForce(-playerRB.velocity.normalized * movementData.rollFriction, ForceMode2D.Impulse);
            rollDuration = movementData.lungeRollDuration;
        }

        yield return new WaitForSeconds(rollDuration);

        playerCol.size = originalSize;
        playerCol.offset = originalOffset;
        playerRB.drag = movementData.groundDrag;
        ChangeState(MovementState.Idle);
    }

    public bool HandlePlunge()
    {
        if (currentState == MovementState.Roll ||
            currentState == MovementState.LungeRoll ||
            currentState == MovementState.LedgeGrab ||
            currentState == MovementState.Plunge ||
            currentState == MovementState.Grapple ||
            currentState == MovementState.GrappleIdle)
            return false;

        Collider2D areaHit = Physics2D.OverlapCircle(groundCheckPosition.position, 1f, groundLayer);
        if (areaHit != null)
            return false;

        RaycastHit2D groundHit = Physics2D.Raycast(groundCheckPosition.position, Vector3.down, movementData.plungeThreshold, groundLayer);
        if (groundHit)
            return false;

        ChangeState(MovementState.Plunge);
        CancelDash();
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

    public void StopPlunge()
    {
        if (plungeRoutine == null)
            return;

        playerRB.isKinematic = true;
        StopCoroutine(plungeRoutine);
        playerRB.gravityScale = movementData.gravityScale;
        playerRB.AddForce(Vector2.down * movementData.plungeForce, ForceMode2D.Impulse);
        plungeRoutine = null;
        OnPlungeEnd?.Invoke();
    }

    public void OnPlungeAnimEnd()
    {
        playerRB.isKinematic = false;
    }

    public void MovePlayer(float movementSpeedMultiplier)
    {
        if (currentState != MovementState.Running &&
            currentState != MovementState.Jump &&
            currentState != MovementState.DoubleJump &&
            currentState != MovementState.Falling &&
            currentState != MovementState.Knockback ||
            !canMove)
            return;

        Vector3 force;

        // Adjust drag & force
        if (isGrounded)
            force = direction * (moveSpeed + playerRB.velocity.magnitude) * 5f;
        else if (!isGrounded)
            force = direction * moveSpeed * 10f * movementData.airMultiplier;
        else
            force = Vector3.zero;

        // Move player
        playerRB.AddForce(force * movementSpeedMultiplier, ForceMode2D.Force);
    }

    public void CheckGroundCollision()
    {
        if (!canMove || playerRB == null)
            return;

        Vector2 raycastPos = transform.localScale.x < 0 ? 
            new Vector2(groundCheckPosition.position.x - 0.3f, groundCheckPosition.position.y) : 
            new Vector2(groundCheckPosition.position.x + 0.3f, groundCheckPosition.position.y);

        RaycastHit2D groundHit = Physics2D.Raycast(groundCheckPosition.position, Vector3.down, 100, groundLayer);
        RaycastHit2D groundHit1 = Physics2D.Raycast(raycastPos, Vector2.down, 100, groundLayer);

        Debug.DrawRay(raycastPos, Vector2.down * 100, Color.red);

        float dist = Vector3.Distance(groundCheckPosition.position, groundHit.point);
        float dist1 = Vector3.Distance(raycastPos, groundHit1.point);

        if (!isGrounded && 
            playerRB.velocity.y < 0 && 
            dist <= 1.5f &&
            dist > 0.2f &&
            currentState != MovementState.Plunge &&
            currentState != MovementState.Grapple &&
            currentState != MovementState.GrappleIdle)
        {
            ChangeState(MovementState.Land);
        }
        else if (!isGrounded &&
            playerRB.velocity.y < 0 &&
            dist > 1.5f &&
            currentState != MovementState.Plunge &&
            currentState != MovementState.Grapple &&
            currentState != MovementState.GrappleIdle)
        {
            ChangeState(MovementState.Falling);
        }

        if (dist <= movementData.minGroundDist && dist1 <= movementData.minGroundDist)
        {
            if (!isGrounded &&
                currentState != MovementState.LedgeGrab
                && currentState != MovementState.GroundDash &&
                currentState != MovementState.Plunge &&
                currentState != MovementState.Running &&
                currentState != MovementState.Knockback &&
                currentState != MovementState.Grapple &&
                currentState != MovementState.GrappleIdle)
                ChangeState(MovementState.Idle);

            isGrounded = true;

            if (currentState == MovementState.Knockback)
                return;

            fallingDuration = 0;

            if (currentState != MovementState.Roll &&
                currentState != MovementState.LungeRoll)
                playerRB.drag = movementData.groundDrag;

            wallJumpCount = maxWallJumps;
            if (jumpRoutine == null)
                jumpCount = maxJumpCount;
        }
        else if (dist > movementData.minGroundDist)
        {
            isGrounded = false;
            playerRB.drag = movementData.airDrag;
        }
    }

    public void StopPlayer()
    {
        if (playerRB == null)
            return;

        canMove = false;
        playerRB.velocity = Vector3.zero;
        playerRB.gravityScale = 0;
        CancelDash();
    }

    public void ResumePlayer()
    {
        canMove = true;
        playerRB.velocity = Vector3.zero;
        playerRB.gravityScale = movementData.gravityScale;
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

        if (currentVel.magnitude > movementData.walkSpeed)
        {
            Vector3 limitVel = currentVel.normalized * movementData.walkSpeed;
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

    public bool CheckCannotCombat()
    {
        return currentState == MovementState.GroundDash ||
                currentState == MovementState.AirDash ||
                currentState == MovementState.Roll ||
                currentState == MovementState.LungeRoll ||
                currentState == MovementState.LedgeGrab ||
                currentState == MovementState.Plunge ||
                currentState == MovementState.Knockback;
    }

    public bool RopeTriggerEnter(Collider2D collision)
    {
        if (collision.TryGetComponent<Rope>(out rope))
        {
            canGrapple = true;
            return true;
        }

        return false;
    }

    public bool RopeTriggerExit(Collider2D collision)
    {
        if (collision.TryGetComponent<Rope>(out rope))
        {
            canGrapple = false;
            return true;
        }

        return false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (Utility.CheckLayer(collision.gameObject, groundLayer) && (currentState == MovementState.Falling || currentState == MovementState.Land))
        {
            playerEffectsController.BurstLandPS();
            AudioManager.Instance.PlayOneShot(Sound.SoundName.Land);
        }
    }

    private void OnDisable()
    {
        OnPlungeEnd = null;
        ChangePlayerState = null;
    }
}