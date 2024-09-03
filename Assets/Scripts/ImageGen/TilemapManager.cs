using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapManager : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private TileBase newTile;
    [SerializeField] private TileBase targetTile;

    public void SwapTile()
    {
        tilemap.SwapTile(newTile, targetTile);
    }
}