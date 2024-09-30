using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class AbilityUIController : MonoBehaviour
{
    [SerializeField] private Image cooldownSlider;
    [SerializeField] private Image abilityIcon;
    [SerializeField] private TMP_Text chargeText;
    [SerializeField] private TMP_Text keybindText;

    private void Start()
    {
        cooldownSlider.fillAmount = 0;
    }

    public void InitAbilityUI(string keybind)
    {
        abilityIcon.enabled = false;
        SetCooldown(0, 0);
        keybindText.text = keybind;
    }

    public void InitAbilityUI(BaseAbility ability, string keybind)
    {
        abilityIcon.enabled = true;
        abilityIcon.sprite = ability.abilityIcon;
        SetCooldown(0, ability.abilityCharges);
        keybindText.text = keybind;
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
}
