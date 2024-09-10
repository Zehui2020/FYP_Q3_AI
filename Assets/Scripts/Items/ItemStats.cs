using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemStats")]
public class ItemStats : ScriptableObject
{
    public int ritualBleedChance;
    public int ritualBleedStacks;

    public void ResetStats()
    {
        ritualBleedChance = 0;
        ritualBleedStacks = 0;
    }
}