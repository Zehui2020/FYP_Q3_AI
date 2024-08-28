using UnityEngine;

[CreateAssetMenu(fileName = "MovementData")]
public class MovementData : ScriptableObject
{
    public float walkSpeed;
    public float baseJumpForce;

    public float airMultiplier;
    public float groundDrag;
    public float airDrag;

    public float dashDuration;
    public float dashSpeed;
    public float dashCooldown;

    public float maxSlopeAngle;
    public float minGroundDist;

    public float wallJumpForceX;
    public float wallJumpForceY;

    public float rollColliderSize;

    public float dashIFrames;
    public float rollDuration;

    public float grappleSpeed;

    public float plungeThreshold;
    public float plungeDelay;
    public float plungeForce;

    public float cyoteTime;
}