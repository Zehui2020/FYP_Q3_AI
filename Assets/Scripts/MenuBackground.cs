using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuBackground : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    [SerializeField] private ImageSaver imageSaver;

    public void SetupBackground()
    {
        spriteRenderer.sprite = imageSaver.GetSpriteFromLocalDisk("background");
        animator.SetTrigger("FadeIn");
    }

    public void ResetBackground()
    {
        animator.SetTrigger("FadeOut");
    }
}