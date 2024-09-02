using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
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
    private int Attacking = Animator.StringToHash("Sword_Attack_1");

    public void InitAnimationController()
    {
        animator = GetComponent<Animator>();
    }

    public void ChangeAnimation(int state, float transitionDuration, float delayDuration, bool resetAnimationIfSame)
    {
        if (state == currentState && resetAnimationIfSame)
        {
            animator.Play(state, -1, 0f);
            return;
        }

        if (Time.time < transitionDelay)
            return;

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

    public void SetAttackAnimationClip(int animation)
    {
        Attacking = animation;
    }
}