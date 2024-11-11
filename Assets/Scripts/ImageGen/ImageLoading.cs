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
    private Animator animator;

    public void InitImageLoading()
    {
        animator = GetComponent<Animator>();
        comfyWebsocket.InitWebsocket();

        DontDestroyOnLoad(gameObject);

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
            }
        };
    }

    private void LateUpdate()
    {
        if (GameData.Instance.loadingQueue.Count == 0)
        {
            Debug.Log("NOTHING  IN Q");
            return;
        }

        currentImage.text = "Generating:\n" + GameData.Instance.loadingQueue.Peek();
        SetSliderValue();
    }

    private void SetSliderValue()
    {
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
        }
    }

    private IEnumerator FadeRoutine()
    {
        yield return new WaitForSeconds(8f);

        animator.SetBool("fadeIn", false);
    }
}