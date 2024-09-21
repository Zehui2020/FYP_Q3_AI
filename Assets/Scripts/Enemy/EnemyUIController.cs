using UnityEngine;

public class EnemyUIController : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private EnemyStatBar healthBar;
    [SerializeField] private EnemyStatBar shieldBar;
    [SerializeField] private Animator alertSignal;

    public void InitUIController(BaseStats baseStats)
    {
        healthBar.InitStatBar(baseStats.health, baseStats.maxHealth);

        if (shieldBar != null)
            shieldBar.InitStatBar(baseStats.shield, baseStats.maxShield);
    }

    public void SetCanvasActive(bool active)
    {
        canvas.enabled = active;
    }

    public void OnHealthChanged(int health, int maxHealth, bool increase, bool critical)
    {
        if (!increase)
            healthBar.OnDecrease(health, maxHealth, critical);
        else
            healthBar.OnIncreased(health, maxHealth, critical);
    }

    public void OnShieldChanged(int shield, int maxShield, bool increase, float increaseDuration, bool critical)
    {
        if (shieldBar == null)
            return;

        if (!increase)
            shieldBar.OnDecrease(shield, maxShield, critical);
        else
            shieldBar.IncreaseDelayBar(shield, maxShield, increaseDuration);
    }

    public void ShowAlertSignal()
    {
        alertSignal.SetTrigger("alert");
    }
}