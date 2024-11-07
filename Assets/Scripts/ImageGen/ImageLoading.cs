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

    private void Start()
    {
        animator = GetComponent<Animator>();
        comfyWebsocket.InitWebsocket();
    }

    private void Update()
    {
        currentImage.text = "Generating:\n" + GameData.Instance.currentlyLoadingImage.Peek();
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

        if (loadingSlider.value == loadingSlider.maxValue)
        {
            GameData.Instance.currentlyLoadingImage.Dequeue();
            loadingSlider.value = 0;
            loadingSlider.maxValue = 100;
        }

        if (GameData.Instance.currentlyLoadingImage.Peek() != previousPrompt)
        {
            previousPrompt = GameData.Instance.currentlyLoadingImage.Peek();
            if (fadeOutRoutine != null)
            {
                StopCoroutine(fadeOutRoutine);
                fadeOutRoutine = null;
                animator.SetBool("fadeIn", true);
            }
        }
        else if (currentProgress != previousProgress)
        {
            previousProgress = currentProgress;
            if (fadeOutRoutine != null)
            {
                StopCoroutine(fadeOutRoutine);
                fadeOutRoutine = null;
                animator.SetBool("fadeIn", true);
            }
        }
        else
        {
            if (fadeOutRoutine == null)
                fadeOutRoutine = StartCoroutine(FadeRoutine()); 
        }

        Debug.Log(GameData.Instance.currentlyLoadingImage.Count);
    }

    private IEnumerator FadeRoutine()
    {
        yield return new WaitForSeconds(8f);

        animator.SetBool("fadeIn", false);
    }
}