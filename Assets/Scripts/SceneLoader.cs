using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;
    [SerializeField] private Animator fadeTransition;
    private Coroutine changeSceneRoutine;
    [SerializeField] protected PlayerPrefs playerPrefs;

    void Awake()
    {
        Instance = this;
    }

    public void FadeIn()
    {
        if (fadeTransition == null)
            return;

        fadeTransition.SetBool("FadeIn", false);
    }

    public void FadeOut()
    {
        if (fadeTransition == null)
            return;

        fadeTransition.SetBool("FadeOut", true);
    }

    public void LoadScene(string sceneName)
    {
        if (changeSceneRoutine == null)
            changeSceneRoutine = StartCoroutine(LoadNewScene(sceneName));
    }

    public void LoadFirstScene()
    {
        if (playerPrefs.experiencedTutorial)
            LoadScene("LobbyLevel");
        else
            LoadScene("TutorialLevel");
    }

    private IEnumerator LoadNewScene(string sceneName)
    {
        FadeOut();
        if (GameData.Instance != null)
            GameData.Instance.SavePlayerData();

        yield return new WaitForSecondsRealtime(0.5f);

        SceneManager.LoadSceneAsync(sceneName);
        Time.timeScale = 1.0f;

        yield return new WaitForSecondsRealtime(0.5f);

        changeSceneRoutine = null;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}