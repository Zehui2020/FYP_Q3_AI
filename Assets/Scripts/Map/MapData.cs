using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class MapData : ScriptableObject
{
    public List<GameObject> startTilePrefabs;
    public List<GameObject> autoSetTilePrefabs;
    public List<GameObject> deadEndTilePrefabs;
    public List<GameObject> uniqueTilePrefabs;
    public List<GameObject> solidTilePrefabs;
}
