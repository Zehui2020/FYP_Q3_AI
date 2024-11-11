using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ImageLoading : MonoBehaviour
{
    [SerializeField] private ComfyWebsocket comfyWebsocket;

    [SerializeField] private TextMeshProUGUI currentImage;
    [SerializeField] private Slider loadingSlider;

    private string previousPrompt;
    private int previousProgress;

    private Coroutine fadeOutRoutine;
    private Coroutine loadRoutine;

    private Animator animator;

    private void Awake()
    {
        GameData.Instance.OnLoadingQueueChanged += (enqueue, title) =>
        {
            if (enqueue)
            {
                loadingSlider.value = 0;
                loadingSlider.maxValue = 100;
                Debug.Log("En Q: " + title);
                previousPrompt = title;
                if (fadeOutRoutine != null)
                {
                    StopCoroutine(fadeOutRoutine);
                    fadeOutRoutine = null;
                    animator.SetBool("fadeIn", true);
                }

                if (loadRoutine == null)
                    loadRoutine = StartCoroutine(StartLoading());
            }
        };
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        comfyWebsocket.InitWebsocket();

        DontDestroyOnLoad(gameObject);
    }

    private IEnumerator StartLoading()
    {
        while (true)
        {
            currentImage.text = "Generating:\n" + GameData.Instance.loadingQueue.Peek();
            int currentProgress = Utility.ParseJsonValue(comfyWebsocket.response, "value");
            int maxProgress = Utility.ParseJsonValue(comfyWebsocket.response, "max");

            if (currentProgress <= 0 || maxProgress <= 1)
            {
                loadingSlider.value = 0;
                loadingSlider.maxValue = 100;
            }
            else
            {
                loadingSlider.value = currentProgress;
                loadingSlider.maxValue = maxProgress;
            }

            if (currentProgress != previousProgress)
            {
                previousProgress = currentProgress;
                if (fadeOutRoutine != null)
                {
                    StopCoroutine(fadeOutRoutine);
                    fadeOutRoutine = null;
                    animator.SetBool("fadeIn", true);
                }
            }

            if (GameData.Instance.loadingQueue.Count != 0)
            {
                if (GameData.Instance.loadingQueue.Peek() == previousPrompt)
                {
                    if (fadeOutRoutine == null)
                        fadeOutRoutine = StartCoroutine(FadeRoutine());
                }
            }

            if (loadingSlider.value == loadingSlider.maxValue)
            {
                Debug.Log("De Q: " + GameData.Instance.loadingQueue.Peek());
                GameData.Instance.DequeueLoading();
                loadingSlider.value = 0;
                loadingSlider.maxValue = 100;

                if (GameData.Instance.loadingQueue.Count > 0)
                    loadRoutine = StartCoroutine(StartLoading());
                else
                    loadRoutine = null;

                yield break;
            }

            yield return null;
        }
    }

    private IEnumerator FadeRoutine()
    {
        yield return new WaitForSeconds(8f);

        animator.SetBool("fadeIn", false);
    }
}