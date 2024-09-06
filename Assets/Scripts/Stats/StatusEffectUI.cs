using DesignPatterns.ObjectPool;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusEffectUI : PooledObject
{
    public enum StatusEffectType
    {
        Bleed,
        Burn,
        Poison,
        Freeze,
        Static
    }
    public StatusEffectType effectType;

    [System.Serializable]
    public struct StatusEffectHolder
    {
        public StatusEffectType effectType;
        public Sprite icon;
    }

    [SerializeField] private List<StatusEffectHolder> effects = new();
    [SerializeField] private Image effectIcon;
    [SerializeField] private TextMeshProUGUI effectStacks;

    public void SetStatusEffectUI(StatusEffectType type, int count)
    {
        if (count <= 0)
        {
            Release();
            gameObject.SetActive(false);
            return;
        }

        foreach (StatusEffectHolder holder in effects)
        {
            if (holder.effectType.Equals(type))
                effectIcon.sprite = holder.icon;
        }

        effectType = type;
        effectStacks.text = count.ToString();
    }

    public void SetStackCount(int count)
    {
        effectStacks.text = count.ToString();
    }

    public void RemoveStatusEffect()
    {
        Release();
        gameObject.SetActive(false);
    }
}