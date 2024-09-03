using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class WeaponData : ScriptableObject
{
    public enum WeaponType
    {
        Sword
    }

    public WeaponType weaponType;
    public List<AnimationClip> attackAnimations;
    public AnimationClip plungeAttackAnimation;
    public int baseAttack;
    public List<float> attackMultipliers;
    public float plungeAttackMultiplier;
    public float attackSpeed;
    public int critRate;
    public float critMultiplier;
}