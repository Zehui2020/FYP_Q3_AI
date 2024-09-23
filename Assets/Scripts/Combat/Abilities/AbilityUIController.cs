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
    [SerializeField] private TMP_Text chargeText;

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

    public void SetCooldown(float amount, int charge)
    {
        cooldownSlider.fillAmount = amount;
        chargeText.text = "";
        for (int i = 0; i < charge; i++)
        {
            chargeText.text += ".";
        }
    }

    public void SetIcon(Sprite icon)
    {
        abilityIcon.sprite = icon;
    }
}
