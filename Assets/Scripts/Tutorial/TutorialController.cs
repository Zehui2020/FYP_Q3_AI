using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Animator cutsceneBar;

    private void Awake()
    {
        Time.timeScale = 0;
        cutsceneBar.SetBool("isShowing", true);
    }

    public void StartFade()
    {
        animator.SetTrigger("fade");
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        cutsceneBar.SetBool("isShowing", false);
    }
}