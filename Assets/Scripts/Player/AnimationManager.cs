using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    // Singleton
    public static AnimationManager Instance;

    private Animator animator;
    private int currentState;
    private float transitionDelay;


    // Idles
    public readonly int Idle = Animator.StringToHash("Idle");

    // Movements
    public readonly int Walking = Animator.StringToHash("Walking");

    private void Awake()
    {
        Instance = this;
    }

    public void InitAnimationController()
    {
        animator = GetComponent<Animator>();
    }

    public void ChangeAnimation(int state, float transitionDuration, float delayDuration)
    {
        if (state == currentState || Time.time < transitionDelay)
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
}