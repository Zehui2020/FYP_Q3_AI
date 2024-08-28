using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;
    [SerializeField] private Animator fadeTransition;
    private Coroutine changeSceneRoutine;

    void Awake()
    {
        Instance = this;
    }

    public void FadeIn()
    {
        if (fadeTransition == null)
            return;

        fadeTransition.SetBool("fade", false);
    }

    public void FadeOut()
    {
        if (fadeTransition == null)
            return;

        fadeTransition.SetBool("fade", true);
    }

    public void LoadScene(string sceneName)
    {
        if (changeSceneRoutine == null)
            changeSceneRoutine = StartCoroutine(LoadNewScene(sceneName));
    }

    private IEnumerator LoadNewScene(string sceneName)
    {
        FadeOut();

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


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
            LoadScene("PlayerMovement");
    }
}