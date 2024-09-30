using UnityEngine;

[CreateAssetMenu(fileName = "MovementData")]
public class MovementData : ScriptableObject
{
    public float gravityScale;

    public float overlapSpeed;
    public float walkSpeed;
    public float baseJumpForce;
    public float wallJumpForce;
    public float jumpInterval;
    public float oppositeJumpMultiplier;

    public float airMultiplier;
    public float groundDrag;
    public float airDrag;

    public float dashDuration;
    public float dashSpeed;
    public float groundDashDuration;
    public float groundDashSpeed;
    public float dashCooldown;
    public float dashIFrames;

    public float maxSlopeAngle;
    public float minGroundDist;

    public float wallJumpForceX;
    public float wallJumpForceY;
    public float wallCheckDist;

    public float rollColliderSize;
    public float rollDuration;
    public float lungeRollDuration;
    public float rollFriction;

    public float grappleSpeed;

    public float plungeThreshold;
    public float plungeDelay;
    public float plungeForce;

    public float ledgeCheckDist;
    public float ledgeXOffset1;
    public float ledgeYOffset1;
    public float ledgeXOffset2;
    public float ledgeYOffset2;

    public float cyoteTime;
}