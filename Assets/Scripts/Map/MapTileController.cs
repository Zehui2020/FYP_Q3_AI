using System.Collections.Generic;
using UnityEngine;

public class MapTileController : MonoBehaviour
{
    [HideInInspector] public List<GameObject> startTilePrefabs;
    [HideInInspector] public List<GameObject> autoInitTilePrefabs;
    [HideInInspector] public List<GameObject> deadEndTilePrefabs;
    [HideInInspector] public List<GameObject> uniqueTilePrefabs;
    [HideInInspector] public List<GameObject> solidTilePrefabs;
    [HideInInspector] public List<GameObject> shopTilePrefabs;

    private List<GameObject> tile0U = new();
    private List<GameObject> tile1U = new();
    private List<GameObject> tile0D = new();
    private List<GameObject> tile1D = new();
    private List<GameObject> tile0L = new();
    private List<GameObject> tile1L = new();
    private List<GameObject> tile0R = new();
    private List<GameObject> tile1R = new();

    public void InitMapTiles(MapData mData)
    {
        startTilePrefabs = mData.startTilePrefabs;
        autoInitTilePrefabs = mData.autoInitTilePrefabs;
        deadEndTilePrefabs = mData.deadEndTilePrefabs;
        uniqueTilePrefabs = mData.uniqueTilePrefabs;
        solidTilePrefabs = mData.solidTilePrefabs;
        shopTilePrefabs = mData.shopTilePrefabs;
        SortTiles();
        SetTileContraints();
    }

    private void SortTiles()
    {
        // store auto set tiles
        for (int i = 0; i < autoInitTilePrefabs.Count; i++)
            CheckTileNeighbours(autoInitTilePrefabs[i]);
        // store dead end tiles
        for (int i = 0; i < deadEndTilePrefabs.Count; i++)
            CheckTileNeighbours(deadEndTilePrefabs[i]);
        // store unique tiles
        for (int i = 0; i < uniqueTilePrefabs.Count; i++)
            CheckTileNeighbours(uniqueTilePrefabs[i]);
        // store shop tiles
        for (int i = 0; i < shopTilePrefabs.Count; i++)
            CheckTileNeighbours(shopTilePrefabs[i]);
    }

    private void CheckTileNeighbours(GameObject tile)
    {
        if (tile.name.Contains("0U"))
            tile0U.Add(tile);
        else if (tile.name.Contains("1U"))
            tile1U.Add(tile);

        if (tile.name.Contains("0D"))
            tile0D.Add(tile);
        else if (tile.name.Contains("1D"))
            tile1D.Add(tile);

        if (tile.name.Contains("0L"))
            tile0L.Add(tile);
        else if (tile.name.Contains("1L"))
            tile1L.Add(tile);

        if (tile.name.Contains("0R"))
            tile0R.Add(tile);
        else if (tile.name.Contains("1R"))
            tile1R.Add(tile);
    }

    private void SetTileContraints()
    {
        // set auto set tiles
        for (int i = 0; i < autoInitTilePrefabs.Count; i++)
            SetTileNeighbours(autoInitTilePrefabs[i].GetComponent<MapTile>());
        // set shop tiles
        for (int i = 0; i < shopTilePrefabs.Count; i++)
            SetTileNeighbours(shopTilePrefabs[i].GetComponent<MapTile>());
    }

    private void SetTileNeighbours(MapTile tileToSet)
    {
        if (tileToSet.name.Contains("0U"))
            tileToSet.availableTilesUp = tile0D;
        else if (tileToSet.name.Contains("1U"))
            tileToSet.availableTilesUp = tile1D;

        if (tileToSet.name.Contains("0D"))
            tileToSet.availableTilesDown = tile0U;
        else if (tileToSet.name.Contains("1D"))
            tileToSet.availableTilesDown = tile1U;

        if (tileToSet.name.Contains("0L"))
            tileToSet.availableTilesLeft = tile0R;
        else if (tileToSet.name.Contains("1L"))
            tileToSet.availableTilesLeft = tile1R;

        if (tileToSet.name.Contains("0R"))
            tileToSet.availableTilesRight = tile0L;
        else if (tileToSet.name.Contains("1R"))
            tileToSet.availableTilesRight = tile1L;
    }
}
