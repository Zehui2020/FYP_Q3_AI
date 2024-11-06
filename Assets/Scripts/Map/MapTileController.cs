using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MapTileController : MonoBehaviour
{
    [HideInInspector] public List<GameObject> startTilePrefabs;
    [HideInInspector] public List<GameObject> autoInitTilePrefabs;
    [HideInInspector] public List<GameObject> deadEndTilePrefabs;
    [HideInInspector] public List<GameObject> uniqueTilePrefabs;
    [HideInInspector] public List<GameObject> borderTilePrefabs;
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
        borderTilePrefabs = mData.borderTilePrefabs;
        solidTilePrefabs = mData.solidTilePrefabs;
        shopTilePrefabs = mData.shopTilePrefabs;
        ConfigureTiles();
    }

    private void ConfigureTiles()
    {
        AddTilesToSort(autoInitTilePrefabs);
        AddTilesToSort(uniqueTilePrefabs);
        AddTilesToSort(shopTilePrefabs);
        AddTilesToSort(solidTilePrefabs);
        SetTileContraints(startTilePrefabs, false);
        AddTilesToSort(deadEndTilePrefabs);
        SetTileContraints(autoInitTilePrefabs, false);
        SetTileContraints(shopTilePrefabs, false);
        SetTileContraints(deadEndTilePrefabs, true);
        SetTileContraints(solidTilePrefabs, true);
        AddTilesToSort(borderTilePrefabs);
        SetTileContraints(borderTilePrefabs, true);
    }

    private void AddTilesToSort(List<GameObject> objList)
    {
        // store tiles
        for (int i = 0; i < objList.Count; i++)
            CheckTileNeighbours(objList[i]);
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

    private void SetTileContraints(List<GameObject> objList, bool setToSelf)
    {
        // set tiles
        for (int i = 0; i < objList.Count; i++)
            SetTileNeighbours(objList[i].GetComponent<MapTile>(), setToSelf);
    }

    private void SetTileNeighbours(MapTile tileToSet, bool setToSelf)
    {
        tileToSet.availableTilesUp.Clear();
        tileToSet.availableTilesDown.Clear();
        tileToSet.availableTilesLeft.Clear();
        tileToSet.availableTilesRight.Clear();

        if (tileToSet.name.Contains("0U"))
            tileToSet.availableTilesUp.AddRange(tile0D);
        else if (tileToSet.name.Contains("1U"))
            tileToSet.availableTilesUp.AddRange(tile1D);

        if (tileToSet.name.Contains("0D"))
            tileToSet.availableTilesDown.AddRange(tile0U);
        else if (tileToSet.name.Contains("1D"))
            tileToSet.availableTilesDown.AddRange(tile1U);

        if (tileToSet.name.Contains("0L"))
            tileToSet.availableTilesLeft.AddRange(tile0R);
        else if (tileToSet.name.Contains("1L"))
            tileToSet.availableTilesLeft.AddRange(tile1R);

        if (tileToSet.name.Contains("0R"))
            tileToSet.availableTilesRight.AddRange(tile0L);
        else if (tileToSet.name.Contains("1R"))
            tileToSet.availableTilesRight.AddRange(tile1L);

        if (!setToSelf)
        {
            if (tileToSet.availableTilesUp.Contains(tileToSet.gameObject))
                tileToSet.availableTilesUp.Remove(tileToSet.gameObject);
            if (tileToSet.availableTilesDown.Contains(tileToSet.gameObject))
                tileToSet.availableTilesDown.Remove(tileToSet.gameObject);
            if (tileToSet.availableTilesLeft.Contains(tileToSet.gameObject))
                tileToSet.availableTilesLeft.Remove(tileToSet.gameObject);
            if (tileToSet.availableTilesRight.Contains(tileToSet.gameObject))
                tileToSet.availableTilesRight.Remove(tileToSet.gameObject);
        }
    }
}
