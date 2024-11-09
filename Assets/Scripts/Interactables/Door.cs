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
    [SerializeField] private bool portalDoor;
    [SerializeField] private bool shakeOnAppear;

    [SerializeField] private string baseSceneName;

    [SerializeField] private ParticleManager appearPS;

    private IEnumerator Start()
    {
        if (shakeOnAppear)
        {
            float timer = 2f;
            float tragetPosY = transform.position.y;

            transform.position = new Vector3(transform.position.x, transform.position.y - 3f, transform.position.z);
            PlayerController.Instance.playerEffectsController.ShakeCamera(5, 5, 2f);
            appearPS.PlayAll();

            while (timer > 0)
            {
                timer -= Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, tragetPosY, transform.position.z), Time.deltaTime * 2f);
                yield return null;
            }

            appearPS.StopAll();
        }
    }

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
        {
            if (portalDoor)
            {
                if (GameData.Instance.levelCount >= GameData.Instance.maxLevels)
                    SceneLoader.Instance.LoadScene("BossLevel");
                else
                    SceneLoader.Instance.LoadScene(baseSceneName + GameData.Instance.levelCount);
            }

            GameData.Instance.levelCount++;
        }
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