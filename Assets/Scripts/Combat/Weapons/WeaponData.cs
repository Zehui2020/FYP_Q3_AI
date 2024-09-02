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
    public int attack;
    public int attackSpeed;
    public int critRate;
    public int critDamage;
}
