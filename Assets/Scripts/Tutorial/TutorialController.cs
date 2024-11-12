using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private void Awake()
    {
        Time.timeScale = 0;
    }

    public void StartFade()
    {
        animator.SetTrigger("fade");
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
    }
}