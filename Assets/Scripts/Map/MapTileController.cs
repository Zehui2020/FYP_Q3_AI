using System.Collections.Generic;
using UnityEngine;

public class MapTileController : MonoBehaviour
{
    [SerializeField] public List<GameObject> allTilePrefabs;
    [SerializeField] public List<GameObject> constantTilePrefabs;

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
        SortTiles();
        SetTileContraints();
    }

    private void SortTiles()
    {
        for (int i = 0; i < allTilePrefabs.Count; i++)
        {
            if (allTilePrefabs[i].name.Contains("0U"))
                tile0U.Add(allTilePrefabs[i]);
            else if (allTilePrefabs[i].name.Contains("1U"))
                tile1U.Add(allTilePrefabs[i]);

            if (allTilePrefabs[i].name.Contains("0D"))
                tile0D.Add(allTilePrefabs[i]);
            else if (allTilePrefabs[i].name.Contains("1D"))
                tile1D.Add(allTilePrefabs[i]);

            if (allTilePrefabs[i].name.Contains("0L"))
                tile0L.Add(allTilePrefabs[i]);
            else if (allTilePrefabs[i].name.Contains("1L"))
                tile1L.Add(allTilePrefabs[i]);

            if (allTilePrefabs[i].name.Contains("0R"))
                tile0R.Add(allTilePrefabs[i]);
            else if (allTilePrefabs[i].name.Contains("1R"))
                tile1R.Add(allTilePrefabs[i]);
        }
        for (int i = 0; i < constantTilePrefabs.Count; i++)
        {
            if (constantTilePrefabs[i].name.Contains("0U"))
                tile0U.Add(constantTilePrefabs[i]);
            else if (constantTilePrefabs[i].name.Contains("1U"))
                tile1U.Add(constantTilePrefabs[i]);

            if (constantTilePrefabs[i].name.Contains("0D"))
                tile0D.Add(constantTilePrefabs[i]);
            else if (constantTilePrefabs[i].name.Contains("1D"))
                tile1D.Add(constantTilePrefabs[i]);

            if (constantTilePrefabs[i].name.Contains("0L"))
                tile0L.Add(constantTilePrefabs[i]);
            else if (constantTilePrefabs[i].name.Contains("1L"))
                tile1L.Add(constantTilePrefabs[i]);

            if (constantTilePrefabs[i].name.Contains("0R"))
                tile0R.Add(constantTilePrefabs[i]);
            else if (constantTilePrefabs[i].name.Contains("1R"))
                tile1R.Add(constantTilePrefabs[i]);
        }
    }

    private void SetTileContraints()
    {
        for (int i = 0; i < allTilePrefabs.Count; i++)
        {
            MapTile tileToSet = allTilePrefabs[i].GetComponent<MapTile>();

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
