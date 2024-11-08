using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private SimpleAnimation keycodeUI;
    [SerializeField] private Animator animator;
    [SerializeField] private string nextLevel;
    [SerializeField] private List<AudioSource> audioSources = new();
    private bool isActivated = false;

    public void OnEnterRange()
    {
        keycodeUI.Show();
        animator.SetTrigger("activate");

        if (!isActivated)
            foreach (AudioSource source in audioSources)
                source.Play();

        isActivated = true;
    }

    public bool OnInteract()
    {
        AudioManager.Instance.PlayOneShot(Sound.SoundName.TeleportStart);
        SceneLoader.Instance.LoadScene(nextLevel);

        keycodeUI.Hide();
        GameData.Instance.levelCount++;
        return true;
    }

    public void OnLeaveRange()
    {
        keycodeUI.Hide();
    }
}