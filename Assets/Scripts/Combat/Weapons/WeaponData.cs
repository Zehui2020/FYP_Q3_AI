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
    public List<AnimationClip> animations;
    public int baseAttack;
    public List<float> attackMultipliers;
    public int attackSpeed;
    public int critRate;
    public int critMultiplier;
}