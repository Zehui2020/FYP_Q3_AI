using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

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
    public List<GameObject> borderTilePrefabs;
    public List<GameObject> solidTilePrefabs;
    public List<GameObject> shopTilePrefabs;

    [System.Serializable]
    public struct PropTileData
    {
        public Tile tile;
        public string tileName;
        public ImageSaver.TileSize tileSize;
    }
    public List<PropTileData> propTiles = new();
}
