using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemStats")]
public class ItemStats : ScriptableObject
{
    public int critRate;
    public int critDamage;

    public void ResetStats()
    {
        critRate = 0;
        critDamage = 0;
    }
}