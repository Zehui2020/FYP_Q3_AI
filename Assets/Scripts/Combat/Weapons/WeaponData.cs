using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

[CreateAssetMenu]
public class WeaponData : ScriptableObject
{
    public enum WeaponType
    {
        Sword,
        Axe
    }

    public WeaponType weaponType;
    public List<AnimationClip> attackAnimations;

    public AnimatorController effectController;
    public List<AnimationClip> attackEffectAnimations;

    public List<float> attackMultipliers;
    public List<float> breachMultipliers;

    public AnimationClip plungeAttackAnimation;
    public AnimationClip plungeSlamAnimation;
    public float plungeAttackMultiplier;
    public AnimationClip parryAnimation;
    public int critRate;
    public float critDamage;
    public float comboCooldown;
}