using DesignPatterns.ObjectPool;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusEffectManager : MonoBehaviour
{
    public enum StatusState
    {
        BloodLoss,
        Frozen,
        Dazed,
        Stunned,
    }

    [SerializeField] private StatusEffectStats statusEffectStats;

    [Header("Status Stacks")]
    public StatusEffect bleedStacks;
    public StatusEffect burnStacks;
    public StatusEffect poisonStacks;
    public StatusEffect freezeStacks;
    public StatusEffect staticStacks;

    public event System.Action<StatusEffect.StatusType> OnApplyStatusEffect;
    public event System.Action<StatusState> OnThresholdReached;

    [Header("Status Effect UI")]
    [SerializeField] private RectTransform effectCanvas;
    [SerializeField] private Transform statusEffectUIParent;
    private List<StatusEffectUI> currentEffectUIs = new();

    public void ApplyStatusEffect(StatusEffect.StatusType statusEffect, int amount)
    {
        OnApplyStatusEffect?.Invoke(statusEffect);

        switch (statusEffect)
        {
            case StatusEffect.StatusType.Bleed:
                if (bleedStacks.AddStack(amount))
                {
                    OnThresholdReached?.Invoke(StatusState.BloodLoss);
                    bleedStacks.SetThreshold(bleedStacks.stackThreshold * statusEffectStats.bleedThresholdMultiplier);
                    RemoveEffectUI(StatusEffectUI.StatusEffectType.Bleed);
                }
                else
                    SpawnEffectUI(statusEffect, bleedStacks.stackCount);
                break;
            case StatusEffect.StatusType.Burn:
                burnStacks.AddStack(amount);
                SpawnEffectUI(statusEffect, burnStacks.stackCount);
                break;
            case StatusEffect.StatusType.Poison:
                poisonStacks.AddStack(amount);
                SpawnEffectUI(statusEffect, poisonStacks.stackCount);
                break;
            case StatusEffect.StatusType.Freeze:
                if (freezeStacks.AddStack(amount))
                {
                    OnThresholdReached?.Invoke(StatusState.Frozen);
                    RemoveEffectUI(StatusEffectUI.StatusEffectType.Freeze);
                }
                else
                    SpawnEffectUI(statusEffect, freezeStacks.stackCount);
                break;
            case StatusEffect.StatusType.Static:
                if (staticStacks.AddStack(amount))
                {
                    OnThresholdReached?.Invoke(StatusState.Stunned);
                    RemoveEffectUI(StatusEffectUI.StatusEffectType.Static);
                }
                else
                    SpawnEffectUI(statusEffect, staticStacks.stackCount);
                break;
        }
    }

    public void UpdateStatusEffects()
    {
        bleedStacks.UpdateStack();
        burnStacks.UpdateStack();
        poisonStacks.UpdateStack();
        freezeStacks.UpdateStack();
        staticStacks.UpdateStack();
    }

    private void SpawnEffectUI(StatusEffect.StatusType statusType, int count)
    {
        StatusEffectUI.StatusEffectType type;

        switch (statusType)
        {
            case StatusEffect.StatusType.Bleed:
                type = StatusEffectUI.StatusEffectType.Bleed;
                break;
            case StatusEffect.StatusType.Burn:
                type = StatusEffectUI.StatusEffectType.Burn;
                break;
            case StatusEffect.StatusType.Poison:
                type = StatusEffectUI.StatusEffectType.Poison;
                break;
            case StatusEffect.StatusType.Freeze:
                type = StatusEffectUI.StatusEffectType.Freeze; 
                break;
            case StatusEffect.StatusType.Static:
                type = StatusEffectUI.StatusEffectType.Static; 
                break;
            default:
                type = StatusEffectUI.StatusEffectType.Bleed;
                break;
        }

        foreach (StatusEffectUI effectUI in currentEffectUIs)
        {
            if (effectUI.effectType.Equals(type))
            {
                effectUI.SetStackCount(count);
                return;
            }
        }

        StatusEffectUI statusEffectUI = ObjectPool.Instance.GetPooledObject("StatusEffectUI", true) as StatusEffectUI;
        statusEffectUI.SetStatusEffectUI(type, count);
        currentEffectUIs.Add(statusEffectUI);

        statusEffectUI.transform.SetParent(statusEffectUIParent);
        statusEffectUI.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        LayoutRebuilder.ForceRebuildLayoutImmediate(effectCanvas);
    }

    public void AddEffectUI(StatusEffectUI.StatusEffectType effectType, int count)
    {
        foreach (StatusEffectUI effectUI in currentEffectUIs)
        {
            if (effectUI.effectType.Equals(effectType))
            {
                effectUI.SetStackCount(count);
                return;
            }
        }

        StatusEffectUI statusEffectUI = ObjectPool.Instance.GetPooledObject("StatusEffectUI", true) as StatusEffectUI;
        statusEffectUI.SetStatusEffectUI(effectType, count);
        currentEffectUIs.Add(statusEffectUI);

        statusEffectUI.transform.SetParent(statusEffectUIParent);
        statusEffectUI.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        LayoutRebuilder.ForceRebuildLayoutImmediate(effectCanvas);
    }

    public void RemoveEffectUI(StatusEffectUI.StatusEffectType effectType)
    {
        StatusEffectUI effectUI = FindStatusEffectUI(effectType);
        if (effectUI == null)
            return;

        effectUI.RemoveStatusEffect();
        currentEffectUIs.Remove(effectUI);
    }

    private StatusEffectUI FindStatusEffectUI(StatusEffectUI.StatusEffectType effectType)
    {
        foreach (StatusEffectUI effectUI in currentEffectUIs)
        {
            if (effectUI.effectType.Equals(effectType))
                return effectUI;
        }

        return null;
    }

    public void ReduceEffectStack(StatusEffect.StatusType statusType, int amount)
    {
        StatusEffectUI effectUI;

        switch (statusType)
        {
            case StatusEffect.StatusType.Bleed:
                effectUI = FindStatusEffectUI(StatusEffectUI.StatusEffectType.Bleed);
                ReduceStackCount(bleedStacks, effectUI, amount);
                break;
            case StatusEffect.StatusType.Burn:
                effectUI = FindStatusEffectUI(StatusEffectUI.StatusEffectType.Burn);
                ReduceStackCount(burnStacks, effectUI, amount);
                break;
            case StatusEffect.StatusType.Poison:
                effectUI = FindStatusEffectUI(StatusEffectUI.StatusEffectType.Poison);
                ReduceStackCount(poisonStacks, effectUI, amount);
                break;
            case StatusEffect.StatusType.Freeze:
                effectUI = FindStatusEffectUI(StatusEffectUI.StatusEffectType.Freeze);
                ReduceStackCount(freezeStacks, effectUI, amount);
                break;
            case StatusEffect.StatusType.Static:
                effectUI = FindStatusEffectUI(StatusEffectUI.StatusEffectType.Static);
                ReduceStackCount(staticStacks, effectUI, amount);
                break;
        }
    }

    private void ReduceStackCount(StatusEffect targetEffect, StatusEffectUI effectUI, int amount)
    {
        targetEffect.RemoveStack(amount);

        if (targetEffect.stackCount <= 0)
        {
            effectUI.RemoveStatusEffect();
            currentEffectUIs.Remove(effectUI);
        }
        else
        {
            effectUI.SetStackCount(targetEffect.stackCount);
        }
    }

    private void OnDisable()
    {
        OnApplyStatusEffect = null;
        OnThresholdReached = null;
    }
}