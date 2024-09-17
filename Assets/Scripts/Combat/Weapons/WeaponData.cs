using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class WeaponData : ScriptableObject
{
    public enum WeaponType
    {
        Sword,
        Axe,
        Dagger
    }

    public WeaponType weaponType;
    public List<AnimationClip> attackAnimations;

    public RuntimeAnimatorController effectController;
    public List<AnimationClip> attackEffectAnimations;

    public List<float> attackMultipliers;
    public List<float> breachMultipliers;

    public AnimationClip plungeAttackAnimation;
    public AnimationClip plungeEffectAnimation;

    public AnimationClip plungeSlamAnimation;
    public AnimationClip plungeSlamEffectAnimation;

    public float plungeAttackMultiplier;
    public AnimationClip parryAnimation;
    public int critRate;
    public float critDamage;
    public float comboCooldown;
}