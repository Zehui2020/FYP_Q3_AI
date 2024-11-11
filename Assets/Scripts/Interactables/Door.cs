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

    [SerializeField] private bool exitDoor;
    [SerializeField] private bool endingDoor;
    [SerializeField] private bool portalDoor;
    [SerializeField] private bool shakeOnAppear;
    [SerializeField] private bool cutsceneOnAppear;

    [SerializeField] private string baseSceneName;

    [SerializeField] private List<ParticleManager> appearPS;
    [SerializeField] private CutsceneGroup entranceCutscene;
    
    private IEnumerator Start()
    {
        if (shakeOnAppear)
        {
            animator.SetTrigger("appear");

            if (cutsceneOnAppear)
            {
                AudioManager.Instance.PlayOneShot(Sound.SoundName.EmergeFromGround);
                entranceCutscene.EnterCutscene();
            }
            
            float timer = 2f;
            float tragetPosY = transform.position.y;

            transform.position = new Vector3(transform.position.x, transform.position.y - 3f, transform.position.z);
            PlayerController.Instance.playerEffectsController.ShakeCamera(5, 10, 2f);

            foreach (ParticleManager particle in appearPS)
                particle.PlayAll();

            while (timer > 0)
            {
                timer -= Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, tragetPosY, transform.position.z), Time.deltaTime * 2f);

                if (timer <= 1f)
                    appearPS[1].StopAll();

                yield return null;
            }

            foreach (ParticleManager particle in appearPS)
                particle.StopAll();
        }
    }

    public void PlaySigilAudio()
    {
        AudioManager.Instance.PlayOneShot(Sound.SoundName.SigilLight);
    }

    public void PlayActivateAudio()
    {
        if (isActivated)
            return;

        AudioManager.Instance.PlayOneShot(Sound.SoundName.DoorAppear);
        foreach (AudioSource source in audioSources)
            source.Play();

        isActivated = true;
    }

    public void OnEnterRange()
    {
        keycodeUI.Show();
        animator.SetTrigger("activate");

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
        SceneLoader.Instance.FadeOut();
        AudioManager.Instance.PlayOneShot(Sound.SoundName.TeleportStart);

        yield return new WaitForSecondsRealtime(1);

        if (!endingDoor)
        {
            if (portalDoor)
            {
                if (GameData.Instance.levelCount >= GameData.Instance.maxLevels)
                    SceneLoader.Instance.LoadScene("BossLevel");
                else
                    SceneLoader.Instance.LoadScene(baseSceneName + GameData.Instance.levelCount);

                GameData.Instance.levelCount++;
            }
            else if (exitDoor)
            {
                SceneLoader.Instance.LoadScene(nextLevel);
            }
        }
        else
            GameData.Instance.ResetData();
    }

    public void OnLeaveRange()
    {
        keycodeUI.Hide();
    }
}