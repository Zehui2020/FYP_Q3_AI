using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilitySelectUI : MonoBehaviour
{
    [SerializeField] private HorizontalLayoutGroup layout;
    [SerializeField] private GameObject newAbilityUI;
    [SerializeField] private Image abilityIcon;
    [SerializeField] private GameObject arrowObj;
    [SerializeField] private GameObject borderObj;
    [SerializeField] private Image background;
    [SerializeField] public List<Image> fadeIcons = new();
    [HideInInspector] public List<TMP_Text> fadeTexts = new();
    [HideInInspector] public List<AbilitySlotUI> abilitySlots = new();
    private float borderStartPos;
    private float arrowStartPos;
    [SerializeField] private Canvas canvas;

    private void Start()
    {
        borderStartPos = borderObj.transform.localPosition.y;
        arrowStartPos = arrowObj.transform.localPosition.y;
        ShowSelectAbility(false, null);
    }

    public void ShowSelectAbility(bool show, BaseAbility ability)
    {
        StopAllCoroutines();
        newAbilityUI.SetActive(show);
        if (show)
        {
            AudioManager.Instance.PlayOneShot(Sound.SoundName.AbilitySelect);
            layout.padding = new RectOffset(0, 0, 150, 0);
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.spacing = 35;

            abilityIcon.sprite = ability.spriteIcon;
            StartCoroutine(LerpRoutine(-15, 100, -200, -75));
            foreach (AbilitySlotUI slot in abilitySlots)
            {
                slot.chargeText.color = new Color(slot.chargeText.color.r, slot.chargeText.color.g, slot.chargeText.color.b, 0);
                slot.cooldownSlider.color = new Color(slot.cooldownSlider.color.r, slot.cooldownSlider.color.g, slot.cooldownSlider.color.b, 0);
            }

            canvas.sortingOrder = 201;
        }
        else
        {
            layout.padding = new RectOffset(0, 15, 0, 60);
            layout.childAlignment = TextAnchor.LowerRight;
            layout.spacing = 15;

            abilityIcon.sprite = null;

            foreach (Image icon in fadeIcons)
                icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, 1);
            foreach (TMP_Text text in fadeTexts)
                text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
            foreach (AbilitySlotUI slot in abilitySlots)
            {
                slot.chargeText.color = new Color(slot.chargeText.color.r, slot.chargeText.color.g, slot.chargeText.color.b, 1);
                slot.cooldownSlider.color = new Color(slot.cooldownSlider.color.r, slot.cooldownSlider.color.g, slot.cooldownSlider.color.b, 0.8f);
            }
            transform.localPosition = Vector3.zero;

            canvas.sortingOrder = 0;
        }
    }

    private IEnumerator LerpRoutine(float arrowY, float borderY, float abilitiesStartPos, float abilitiesY)
    {
        ResetIcons();
        float abilitiesPos = abilitiesStartPos;
        float arrowPos = arrowStartPos;
        float borderPos = borderStartPos;
        float alpha = 0;
        float alphaBG = 0;
        int count = 3;

        while (true)
        {
            // fade
            alpha = Mathf.Lerp(alpha, 1, 0.05f);
            foreach (Image icon in fadeIcons)
                icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, alpha);
            foreach (TMP_Text text in fadeTexts)
                text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);

            alphaBG = Mathf.Lerp(alphaBG, 0.75f, 0.1f);
            background.color = new Color(background.color.r, background.color.g, background.color.b, alphaBG);

            // abilities
            abilitiesPos = Mathf.Lerp(abilitiesPos, abilitiesY, 0.06f);
            if (abilitiesPos < abilitiesY - 0.1f)
                transform.localPosition = new Vector3(transform.localPosition.x, abilitiesPos, transform.localPosition.z);
            else
            {
                transform.localPosition = new Vector3(transform.localPosition.x, abilitiesY, transform.localPosition.z);
                count--;
            }

            // arrow
            arrowPos = Mathf.Lerp(arrowPos, arrowY, 0.07f);
            // border
            if (arrowPos > arrowY + 0.1f)
                arrowObj.transform.localPosition = new Vector3(arrowObj.transform.localPosition.x, arrowPos, arrowObj.transform.localPosition.z);
            else
            {
                arrowObj.transform.localPosition = new Vector3(arrowObj.transform.localPosition.x, arrowY, arrowObj.transform.localPosition.z);
                count--;
            }

            // border
            borderPos = Mathf.Lerp(borderPos, borderY, 0.05f);
            if (borderPos > borderY + 0.1f)
                borderObj.transform.localPosition = new Vector3(borderObj.transform.localPosition.x, borderPos, borderObj.transform.localPosition.z);
            else
            {
                borderObj.transform.localPosition = new Vector3(borderObj.transform.localPosition.x, borderY, borderObj.transform.localPosition.z);
                count--;
            }

            if (count <= 0)
                break;

            yield return new WaitForFixedUpdate();
        }

        foreach (Image icon in fadeIcons)
            icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, 1);
        foreach (TMP_Text text in fadeTexts)
            text.color = new Color(text.color.r, text.color.g, text.color.b, 1);

        yield return null;
    }

    private void ResetIcons()
    {
        borderObj.transform.localPosition = new Vector3(borderObj.transform.localPosition.x, borderStartPos, borderObj.transform.localPosition.z);
        arrowObj.transform.localPosition = new Vector3(arrowObj.transform.localPosition.x, borderStartPos, arrowObj.transform.localPosition.z);

        foreach (Image icon in fadeIcons)
            icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, 0);

        foreach (TMP_Text text in fadeTexts)
            text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
    }
}
