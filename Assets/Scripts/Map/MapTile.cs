using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapTile : MonoBehaviour
{
    [SerializeField] public List<GameObject> availableTilesUp;
    [SerializeField] public List<GameObject> availableTilesDown;
    [SerializeField] public List<GameObject> availableTilesLeft;
    [SerializeField] public List<GameObject> availableTilesRight;

    [SerializeField] public Transform chestTransform;
    [SerializeField] public Transform doorTransform;
    
    public void InitializeTile(TileBase targetTile, Tile tile)
    {
        Tilemap[] tilemaps = GetComponentsInChildren<Tilemap>();

        foreach (Tilemap tilemap in tilemaps)
            tilemap.SwapTile(targetTile, tile);

        foreach (Transform obj in GetComponentsInChildren<Transform>())
        {
            if (obj.name.Contains("Chest"))
                chestTransform = obj;
        }
    }
}