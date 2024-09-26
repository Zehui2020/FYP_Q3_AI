using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ChestSpawnChance : ScriptableObject
{
    public int maxChests;
    public List<GameObject> chestTypes;
    public List<int> spawnChances;
}
