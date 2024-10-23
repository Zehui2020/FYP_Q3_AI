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
    private Coroutine increaseDelayBarRoutine;

    private Vector3 originalBarPosition;

    public void InitStatBar(int amount, int maxAmount)
    {
        statBar.maxValue = maxAmount;
        statBar.value = amount;

        delayBarWidth = delayBar.sizeDelta.x;
        originalBarPosition = transform.localPosition;

        delayBar.sizeDelta = new Vector2((float)amount / maxAmount * delayBarWidth, delayBar.sizeDelta.y);
    }

    public void OnDecrease(int amount, int maxAmount, bool critical, bool shake)
    {
        if (amount == maxAmount)
            return;

        if (DelayBarRoutine != null)
            StopCoroutine(DelayBarRoutine);
        DelayBarRoutine = StartCoroutine(SetDelayBar(amount, maxAmount, false));

        if (shake)
        {
            StopShakeRoutine();
            BarShakeRoutine = StartCoroutine(ShakeRoutine(critical));
        }
    }

    public void OnIncreased(int amount, int maxAmount, bool critical)
    {
        if (DelayBarRoutine != null)
            StopCoroutine(DelayBarRoutine);
        DelayBarRoutine = StartCoroutine(SetDelayBar(amount, maxAmount, true));
    }

    public void IncreaseDelayBar(int amount, int maxAmount, float duration)
    {
        if (increaseDelayBarRoutine != null)
            StopCoroutine(increaseDelayBarRoutine);
        increaseDelayBarRoutine = StartCoroutine(IncreaseDelayBarRoutine(amount, maxAmount, duration));
    }

    private void StopShakeRoutine()
    {
        if (BarShakeRoutine == null)
            return;

        StopCoroutine(BarShakeRoutine);
    }

    private IEnumerator SetDelayBar(int amount, int maxAmount, bool increase)
    {
        statBar.maxValue = maxAmount;

        if (!increase)
        {
            statBar.value = amount;

            yield return new WaitForSeconds(delayDuration);

            float targetWidth = (float)amount / maxAmount * delayBarWidth;

            while (Mathf.Abs(delayBar.sizeDelta.x - targetWidth) > 0.01f)
            {
                delayBar.sizeDelta = new Vector2(Mathf.Lerp(delayBar.sizeDelta.x, targetWidth, Time.deltaTime * delayBarSpeed), delayBar.sizeDelta.y);
                yield return null;
            }

            delayBar.sizeDelta = new Vector2(targetWidth, delayBar.sizeDelta.y);
            DelayBarRoutine = null;
        }
        else
        {
            float initialValue = statBar.value;
            float progress = 0f;

            while (progress < 1f)
            {
                progress += Time.deltaTime * delayBarSpeed;
                statBar.value = Mathf.Lerp(initialValue, amount, progress);
                delayBar.sizeDelta = new Vector2(statBar.value / statBar.maxValue * delayBarWidth, delayBar.sizeDelta.y);
                yield return null;
            }

            statBar.value = amount;
            DelayBarRoutine = null;
        }
    }
    private IEnumerator IncreaseDelayBarRoutine(int amount, int maxAmount, float duration)
    {
        float elapsedTime = 0f;

        while (DelayBarRoutine != null)
        {
            duration -= Time.deltaTime;
            yield return null;
        }

        float initialDelayBarWidth = (float)amount / maxAmount * delayBarWidth;
        float targetDelayBarWidth = (float)maxAmount / maxAmount * delayBarWidth;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / duration);

            float currentDelayBarWidth = Mathf.Lerp(initialDelayBarWidth, targetDelayBarWidth, progress);
            delayBar.sizeDelta = new Vector2(currentDelayBarWidth, delayBar.sizeDelta.y);

            yield return null;
        }

        delayBar.sizeDelta = new Vector2(targetDelayBarWidth, delayBar.sizeDelta.y);
        statBar.value = maxAmount;
        increaseDelayBarRoutine = null;
    }

    private IEnumerator ShakeRoutine(bool crit)
    {
        float intensity = crit ? critShakeIntensity : shakeIntensity;
        float duration = crit ? critShakeDuration : shakeDuration;

        float timer = 0f;

        while (timer < duration)
        {
            float offsetX = Random.Range(-intensity, intensity);
            float offsetY = Random.Range(-intensity, intensity);

            transform.localPosition = new Vector3(originalBarPosition.x + offsetX, originalBarPosition.y + offsetY, originalBarPosition.z);

            timer += Time.deltaTime;

            yield return null; 
        }

        transform.localPosition = originalBarPosition;
    }
}