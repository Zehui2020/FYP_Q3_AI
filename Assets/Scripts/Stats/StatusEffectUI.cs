using DesignPatterns.ObjectPool;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusEffectUI : PooledObject
{
    public StatusEffect.StatusType effectType;

    [System.Serializable]
    public struct StatusEffectHolder
    {
        public StatusEffect.StatusType.Status effectType;
        public Sprite icon;
        public bool removeAfterNoStacks;
        public bool isStackable;
    }

    [SerializeField] private List<StatusEffectHolder> effects = new();
    [SerializeField] private Image effectIcon;
    [SerializeField] private TextMeshProUGUI effectStacks;
    [SerializeField] private RectTransform rectTransform;

    public void SetStatusEffectUI(StatusEffect.StatusType type, int count)
    {
        foreach (StatusEffectHolder holder in effects)
        {
            if (holder.effectType != type.statusEffect)
                continue;

            if (count <= 0 && holder.removeAfterNoStacks)
            {
                Release();
                gameObject.SetActive(false);
                return;
            }

            effectIcon.sprite = holder.icon;
            effectType = type;

            if (holder.isStackable)
                effectStacks.text = count.ToString();
            else
                effectStacks.text = string.Empty;

            break;
        }

        rectTransform.sizeDelta = new Vector3(0.5f, 0.5f, 0.5f);
    }

    public void SetStackCount(int count)
    {
        if (count <= 0)
        {
            effectStacks.text = string.Empty;
            return;
        }

        effectStacks.text = count.ToString();
    }

    public void RemoveStatusEffect()
    {
        Release();
        gameObject.SetActive(false);
    }
}