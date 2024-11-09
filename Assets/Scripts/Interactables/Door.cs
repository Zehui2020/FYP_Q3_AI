using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private SimpleAnimation keycodeUI;
    [SerializeField] private Animator animator;
    [SerializeField] private string nextLevel;
    [SerializeField] private List<AudioSource> audioSources = new();
    [HideInInspector] public GameObject icon;
    private bool isActivated = false;

    [SerializeField] private bool endingDoor;

    public void OnEnterRange()
    {
        keycodeUI.Show();
        animator.SetTrigger("activate");

        if (!isActivated)
            foreach (AudioSource source in audioSources)
                source.Play();

        isActivated = true;

        if (icon != null)
            icon.SetActive(true);
    }

    public bool OnInteract()
    {
        StartCoroutine(TeleportRoutine());
        keycodeUI.Hide();

        return true;
    }

    private IEnumerator TeleportRoutine()
    {
        if (!endingDoor)
            GameData.Instance.levelCount++;
        else
            GameData.Instance.ResetData();

        SceneLoader.Instance.FadeOut();
        AudioManager.Instance.PlayOneShot(Sound.SoundName.TeleportStart);
        yield return new WaitForSecondsRealtime(1);
        SceneLoader.Instance.LoadScene(nextLevel);
    }

    public void OnLeaveRange()
    {
        keycodeUI.Hide();
    }
}