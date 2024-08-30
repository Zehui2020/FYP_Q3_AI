using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuBackground : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;

    public void SetupBackground(Sprite image)
    {
        spriteRenderer.sprite = image;
        animator.SetTrigger("FadeIn");
    }

    public void ResetBackground()
    {
        animator.SetTrigger("FadeOut");
    }
}