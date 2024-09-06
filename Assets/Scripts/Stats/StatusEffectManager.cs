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

    public void RemoveEffectUI(StatusEffectUI.StatusEffectType effectType)
    {
        foreach (StatusEffectUI effectUI in currentEffectUIs)
        {
            if (effectUI.effectType.Equals(effectType))
            {
                effectUI.RemoveStatusEffect();
                currentEffectUIs.Remove(effectUI);
                return;
            }
        }
    }

    private void OnDisable()
    {
        OnApplyStatusEffect = null;
        OnThresholdReached = null;
    }
}