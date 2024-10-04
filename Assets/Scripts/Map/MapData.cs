using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class MapData : ScriptableObject
{
    [Header("Map Settings")]
    public ChestSpawnChance chestSpawn;
    public Vector2 mapSize;
    public float mapTileSize;
    public int solidTileBorderThickness;
    public int mapSeed;
    [Header("Map Prefabs")]
    public List<GameObject> startTilePrefabs;
    public List<GameObject> autoInitTilePrefabs;
    public List<GameObject> deadEndTilePrefabs;
    public List<GameObject> uniqueTilePrefabs;
    public List<GameObject> solidTilePrefabs;
}
