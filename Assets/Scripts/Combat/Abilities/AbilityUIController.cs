using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class AbilityUIController : MonoBehaviour
{
    [SerializeField] private Image cooldownSlider;
    [SerializeField] private TMP_Text durationText;
    [SerializeField] private Image abilityIcon;

    private void Start()
    {
        cooldownSlider.fillAmount = 0;
        durationText.gameObject.SetActive(false);
    }

    public void SetDurationText(string text, bool active)
    {
        durationText.text = text;
        durationText.gameObject.SetActive(active);
    }

    public void SetCooldown(float amount)
    {
        cooldownSlider.fillAmount = amount;
    }

    public void SetIcon(Sprite icon)
    {
        abilityIcon.sprite = icon;
    }
}
