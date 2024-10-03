using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class MapTileController : MonoBehaviour
{
    [SerializeField] private MapData mdata;

    [HideInInspector] public List<GameObject> startTilePrefabs;
    [HideInInspector] public List<GameObject> autoSetTilePrefabs;
    [HideInInspector] public List<GameObject> deadEndTilePrefabs;
    [HideInInspector] public List<GameObject> uniqueTilePrefabs;
    [HideInInspector] public List<GameObject> solidTilePrefabs;

    private List<GameObject> tile0U = new List<GameObject>();
    private List<GameObject> tile1U = new List<GameObject>();
    private List<GameObject> tile0D = new List<GameObject>();
    private List<GameObject> tile1D = new List<GameObject>();
    private List<GameObject> tile0L = new List<GameObject>(); 
    private List<GameObject> tile1L = new List<GameObject>();
    private List<GameObject> tile0R = new List<GameObject>();
    private List<GameObject> tile1R = new List<GameObject>();

    public void InitMapTiles()
    {
        startTilePrefabs = mdata.startTilePrefabs;
        autoSetTilePrefabs = mdata.autoSetTilePrefabs;
        deadEndTilePrefabs = mdata.deadEndTilePrefabs;
        uniqueTilePrefabs = mdata.uniqueTilePrefabs;
        solidTilePrefabs = mdata.solidTilePrefabs;
        SortTiles();
        SetTileContraints();
    }

    private void SortTiles()
    {
        // store auto set tiles
        for (int i = 0; i < autoSetTilePrefabs.Count; i++)
        {
            if (autoSetTilePrefabs[i].name.Contains("0U"))
                tile0U.Add(autoSetTilePrefabs[i]);
            else if (autoSetTilePrefabs[i].name.Contains("1U"))
                tile1U.Add(autoSetTilePrefabs[i]);

            if (autoSetTilePrefabs[i].name.Contains("0D"))
                tile0D.Add(autoSetTilePrefabs[i]);
            else if (autoSetTilePrefabs[i].name.Contains("1D"))
                tile1D.Add(autoSetTilePrefabs[i]);

            if (autoSetTilePrefabs[i].name.Contains("0L"))
                tile0L.Add(autoSetTilePrefabs[i]);
            else if (autoSetTilePrefabs[i].name.Contains("1L"))
                tile1L.Add(autoSetTilePrefabs[i]);

            if (autoSetTilePrefabs[i].name.Contains("0R"))
                tile0R.Add(autoSetTilePrefabs[i]);
            else if (autoSetTilePrefabs[i].name.Contains("1R"))
                tile1R.Add(autoSetTilePrefabs[i]);
        }
        // store dead end tiles
        for (int i = 0; i < deadEndTilePrefabs.Count; i++)
        {
            if (deadEndTilePrefabs[i].name.Contains("0U"))
                tile0U.Add(deadEndTilePrefabs[i]);
            else if (deadEndTilePrefabs[i].name.Contains("1U"))
                tile1U.Add(deadEndTilePrefabs[i]);

            if (deadEndTilePrefabs[i].name.Contains("0D"))
                tile0D.Add(deadEndTilePrefabs[i]);
            else if (deadEndTilePrefabs[i].name.Contains("1D"))
                tile1D.Add(deadEndTilePrefabs[i]);

            if (deadEndTilePrefabs[i].name.Contains("0L"))
                tile0L.Add(deadEndTilePrefabs[i]);
            else if (deadEndTilePrefabs[i].name.Contains("1L"))
                tile1L.Add(deadEndTilePrefabs[i]);

            if (deadEndTilePrefabs[i].name.Contains("0R"))
                tile0R.Add(deadEndTilePrefabs[i]);
            else if (deadEndTilePrefabs[i].name.Contains("1R"))
                tile1R.Add(deadEndTilePrefabs[i]);
        }
    }

    private void SetTileContraints()
    {
        // set auto set tiles
        for (int i = 0; i < autoSetTilePrefabs.Count; i++)
        {
            MapTile tileToSet = autoSetTilePrefabs[i].GetComponent<MapTile>();

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
}
