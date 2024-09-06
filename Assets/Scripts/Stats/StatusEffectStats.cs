using UnityEngine;

[CreateAssetMenu(fileName = "StatusEffectStats")]
public class StatusEffectStats : ScriptableObject
{
    public float bloodLossDamage;
    public int bleedThresholdMultiplier;

    public float burnShieldDamage;
    public float burnHealthDamage;
    public float burnsPerStack;
    public float burnInterval;

    public float basePoisonHealthDamage;
    public float stackPoisonHealthDamage;
    public float poisonInterval;
    public float poisonDuration;

    public float frozenCritRate;
    public float frozenCritDmg;
    public float frozenDuration;

    public float stunDuration;
    public float dazeDuration;
}