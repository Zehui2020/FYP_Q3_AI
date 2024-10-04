using System.Collections;
using UnityEngine;

public class EnemyUIController : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private EnemyStatBar healthBar;
    [SerializeField] private EnemyStatBar shieldBar;
    [SerializeField] private Animator alertSignal;
    [SerializeField] private CanvasGroup canvasGroup;

    public void InitUIController(BaseStats baseStats)
    {
        healthBar.InitStatBar(baseStats.health, baseStats.maxHealth);

        if (shieldBar != null)
            shieldBar.InitStatBar(baseStats.shield, baseStats.maxShield);
    }

    public void FadeCanvas(bool fadeIn, float duration)
    {
        StartCoroutine(FadeRoutine(fadeIn, duration));
    }

    private IEnumerator FadeRoutine(bool fadeIn, float duration)
    {
        float timer = 0f;
        float startAlpha = fadeIn ? 0f : 1f;
        float endAlpha = fadeIn ? 1f : 0f;

        canvasGroup.alpha = startAlpha;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(timer / duration); 

            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, normalizedTime);
            yield return null;
        }

        canvasGroup.alpha = endAlpha;
    }

    public void SetCanvasActive(bool active)
    {
        if (canvas != null)
            canvas.enabled = active;
    }

    public void OnHealthChanged(int health, int maxHealth, bool increase, bool critical)
    {
        if (!increase)
            healthBar.OnDecrease(health, maxHealth, critical, true);
        else
            healthBar.OnIncreased(health, maxHealth, critical);
    }

    public void OnShieldChanged(int shield, int maxShield, bool increase, float increaseDuration, bool critical)
    {
        if (shieldBar == null)
            return;

        if (!increase)
            shieldBar.OnDecrease(shield, maxShield, critical, true);
        else
            shieldBar.IncreaseDelayBar(shield, maxShield, increaseDuration);
    }

    public void ShowAlertSignal()
    {
        alertSignal.SetTrigger("alert");
    }

    public void ShowSpottedSignal()
    {
        alertSignal.SetTrigger("spotted");
    }
}