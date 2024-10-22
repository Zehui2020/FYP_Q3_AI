using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilitySlotUI : MonoBehaviour
{
    [SerializeField] public Image cooldownSlider;
    [SerializeField] private Image abilityIcon;
    [SerializeField] public TMP_Text chargeText;
    [SerializeField] private TMP_Text keybindText;
    [SerializeField] private List<Image> icons;
    [SerializeField] private List<TMP_Text> texts;
    private AbilitySelectUI abilitySelect;

    private void Start()
    {
        cooldownSlider.fillAmount = 0;
    }

    public void InitAbilityUI(string keybind)
    {
        abilitySelect = GetComponentInParent<AbilitySelectUI>();
        abilitySelect.fadeIcons.AddRange(icons);
        abilitySelect.fadeTexts.AddRange(texts);
        abilitySelect.abilitySlots.Add(this);
        abilityIcon.enabled = false;
        SetCooldown(0, 0);
        keybindText.text = keybind;
    }

    public void InitAbilityUI(BaseAbility ability, string keybind)
    {
        abilityIcon.enabled = true;
        abilityIcon.sprite = ability.spriteIcon;
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
