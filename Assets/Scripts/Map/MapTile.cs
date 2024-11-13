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

    public WFC_MapGeneration mapGen;
    public bool resetTiles;

    private void Update()
    {
        if (resetTiles)
        {
            mapGen.ConfigureTile(this);
            resetTiles = false;
        }
    }

    public void InitializeTile() // for debugging
    {
        foreach (Transform obj in GetComponentsInChildren<Transform>())
        {
            if (obj.name.Contains("Chest") && chestTransform == null)
                chestTransform = obj;
            if (obj.name.Contains("Door") && doorTransform == null)
                doorTransform = obj;
        }
    }

    public void InitializeTile(TileBase targetTile, Tile tile)
    {
        Tilemap[] tilemaps = GetComponentsInChildren<Tilemap>();

        foreach (Tilemap tilemap in tilemaps)
            tilemap.SwapTile(targetTile, tile);

        foreach (Transform obj in GetComponentsInChildren<Transform>())
        {
            if (obj.name.Contains("Chest") && chestTransform == null)
                chestTransform = obj;
            if (obj.name.Contains("Door") && doorTransform == null)
                doorTransform = obj;
        }
    }
}