using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public enum AnimType
    {
        None,
        ResetIfSame,
        CannotOverride
    }

    private Animator animator;
    private int currentState;
    private float transitionDelay;

    // Idles
    public readonly int Idle = Animator.StringToHash("Idle");

    // Movements
    public readonly int Running = Animator.StringToHash("Running");
    public readonly int Jumping = Animator.StringToHash("Jumping");
    public readonly int DoubleJump = Animator.StringToHash("DoubleJump");
    public readonly int Land = Animator.StringToHash("Land");
    public readonly int Roll = Animator.StringToHash("Roll");
    public readonly int LungeRoll = Animator.StringToHash("LungeRoll");
    public readonly int Falling = Animator.StringToHash("FallingLoop");
    public readonly int AirDash = Animator.StringToHash("AirDash");
    public readonly int GroundDash = Animator.StringToHash("GroundDash");
    public readonly int WallJump = Animator.StringToHash("WallJump");
    public readonly int WallClimb = Animator.StringToHash("WallClimb");
    public readonly int Grappling = Animator.StringToHash("Grappling");
    public readonly int GrappleIdle = Animator.StringToHash("GrappleIdle");
    public readonly int Hurt = Animator.StringToHash("Hurt");

    // Combat
    private int Attacking = Animator.StringToHash("Sword1");

    public void InitAnimationController()
    {
        animator = GetComponent<Animator>();
    }

    public void ChangeAnimation(int state, float transitionDuration, float delayDuration, AnimType animType)
    {
        if (state == currentState && animType == AnimType.ResetIfSame)
        {
            animator.Play(state, -1, 0f);
            return;
        }

        if (Time.time < transitionDelay)
            return;

        if (state != Attacking)
            animator.speed = 1;

        // Add transition delay
        transitionDelay = Time.time + delayDuration;

        animator.CrossFade(state, transitionDuration);
        currentState = state;
    }

    public AnimationClip GetAnimation(int animation)
    {
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (Animator.StringToHash(clip.name) == animation)
                return clip;
        }

        return null;
    }

    public int GetAttackAnimation()
    {
        return Attacking;
    }

    public void SetAttackAnimationClip(int animation, float attackSpeed)
    {
        Attacking = animation;
        animator.speed = attackSpeed;
    }
}