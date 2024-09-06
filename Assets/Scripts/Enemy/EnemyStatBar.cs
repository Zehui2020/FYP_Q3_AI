using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyStatBar : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Slider statBar;
    [SerializeField] private RectTransform delayBar;

    [Header("Values")]
    [SerializeField] private float delayDuration;
    [SerializeField] private float delayBarSpeed;
    private float delayBarWidth;

    [SerializeField] private float shakeIntensity;
    [SerializeField] private float critShakeIntensity;
    [SerializeField] private float shakeDuration;
    [SerializeField] private float critShakeDuration;

    private Coroutine DelayBarRoutine;
    private Coroutine BarShakeRoutine;

    public void InitStatBar(int amount, int maxAmount)
    {
        statBar.maxValue = maxAmount;
        statBar.value = amount;

        delayBarWidth = delayBar.sizeDelta.x;
    }

    public void OnDecrease(int amount, int maxAmount, bool critical)
    {
        if (DelayBarRoutine != null)
            StopCoroutine(DelayBarRoutine);
        DelayBarRoutine = StartCoroutine(SetDelayBar(amount, maxAmount, false));

        if (BarShakeRoutine != null)
            StopCoroutine(BarShakeRoutine);
        BarShakeRoutine = StartCoroutine(ShakeRoutine(critical));
    }

    public void OnIncreased(int amount, int maxAmount, bool critical)
    {
        if (DelayBarRoutine != null)
            StopCoroutine(DelayBarRoutine);
        DelayBarRoutine = StartCoroutine(SetDelayBar(amount, maxAmount, true));
    }

    private IEnumerator SetDelayBar(int amount, int maxAmount, bool increase)
    {
        if (!increase)
        {
            statBar.maxValue = maxAmount;
            statBar.value = amount;

            yield return new WaitForSeconds(delayDuration);

            float targetWidth = (float)amount / maxAmount * delayBarWidth;

            while (Mathf.Abs(delayBar.sizeDelta.x - targetWidth) > 0.01f)
            {
                delayBar.sizeDelta = new Vector2(Mathf.Lerp(delayBar.sizeDelta.x, targetWidth, Time.deltaTime * delayBarSpeed), delayBar.sizeDelta.y);
                yield return null;
            }

            delayBar.sizeDelta = new Vector2(targetWidth, delayBar.sizeDelta.y);
        }

        if (increase)
        {
            while (amount != maxAmount)
            {
                statBar.value = Mathf.Lerp(amount, maxAmount, Time.deltaTime * delayBarSpeed);
                delayBar.sizeDelta = new Vector2(statBar.value / statBar.maxValue * delayBarWidth, delayBar.sizeDelta.y);
                yield return null;
            }
        }
    }

    private IEnumerator ShakeRoutine(bool crit)
    {
        float intensity = crit ? critShakeIntensity : shakeIntensity;
        float duration = crit ? critShakeDuration : shakeDuration;

        Vector3 originalPosition = transform.localPosition;
        float timer = 0f;

        while (timer < duration)
        {
            float offsetX = Random.Range(-intensity, intensity);
            float offsetY = Random.Range(-intensity, intensity);

            transform.localPosition = new Vector3(originalPosition.x + offsetX, originalPosition.y + offsetY, originalPosition.z);

            timer += Time.deltaTime;

            yield return null; 
        }

        transform.localPosition = originalPosition;
    }
}