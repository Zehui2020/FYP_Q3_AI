using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class MapData : ScriptableObject
{
    [Header("Map Settings")]
    public Vector2 mapSize;
    public float tileSize;
    public int borderThickness;
    public int mapSeed;
    [Header("Map Prefabs")]
    public List<GameObject> startTilePrefabs;
    public List<GameObject> autoSetTilePrefabs;
    public List<GameObject> deadEndTilePrefabs;
    public List<GameObject> uniqueTilePrefabs;
    public List<GameObject> solidTilePrefabs;
}
