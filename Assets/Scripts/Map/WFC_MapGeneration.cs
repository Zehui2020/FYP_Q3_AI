using System.Collections.Generic;
using UnityEngine;

public class WFC_MapGeneration : MonoBehaviour
{
    [SerializeField] private Vector2 mapSize;
    [SerializeField] private float tileSize;
    [SerializeField] private int borderThickness;
    [SerializeField] private List<GameObject> startingTilePrefabs;
    [SerializeField] private List<GameObject> allTilePrefabs;
    [SerializeField] private List<GameObject> topBorderTiles;
    [SerializeField] private List<GameObject> bottomBorderTiles;
    [SerializeField] private List<GameObject> leftBorderTiles;
    [SerializeField] private List<GameObject> rightBorderTiles;
    [SerializeField] private List<GameObject> topLeftCornerTiles;
    [SerializeField] private List<GameObject> topRightCornerTiles;
    [SerializeField] private List<GameObject> bottomLeftCornerTiles;
    [SerializeField] private List<GameObject> bottomRightCornerTiles;
    [SerializeField] private GameObject solidTile;
    [SerializeField] private List<MapTile> mapTiles;
    [SerializeField] private int mapSeed;
    private Vector2 currTile;
    private List<Vector2> collapsableTiles = new List<Vector2>();
    private List<Vector2> collapsedTiles = new List<Vector2>();
    private List<int> collapsableTileNum = new List<int>();

    private void Update()
    {
        // debug inputs
        if (Input.GetKeyDown(KeyCode.R))
        {
            RandomizeSeed();
            GenerateMap();
        }
    }

    public void InitMapGenerator()
    {
        SetSeed(mapSeed);
        GenerateMap();
    }

    private void RandomizeSeed()
    {
        mapSeed = Random.Range(0, 1000000000);
        Random.InitState(mapSeed);
    }

    public void SetSeed(int seed)
    {
        mapSeed = seed;
        Random.InitState(mapSeed);
    }

    public void GenerateMap()
    {
        // randomize starting node
        currTile = new Vector2(Random.Range(0, (int)mapSize.x), Random.Range(0, (int)mapSize.y));
        // set random starting room tile
        int randomIndex = Random.Range(0, startingTilePrefabs.Count);
        mapTiles[(int)currTile.x + (int)(currTile.y * mapSize.x)] = startingTilePrefabs[randomIndex].GetComponent<MapTile>();
        //place room
        GameObject obj = Instantiate(startingTilePrefabs[randomIndex], transform);
        obj.transform.localPosition = currTile * tileSize;
        // add new collapsable tiles
        collapsedTiles.Add(currTile);
        AddCollapsableTiles();
        // set rooms
        SetNextTile();
        // set borders
        SetBorderTiles();
    }

    private void SetNextTile()
    {
        // find tile with least amount of collapse options
        Vector2 tileToCollapse = Vector2.zero;
        int leastTilesToCollapse = -1;
        for (int i = 0; i < collapsableTiles.Count; i++)
        {
            if (CheckTileInBounds(collapsableTiles[i]))
            {
                if (tileToCollapse == Vector2.zero)
                {
                    tileToCollapse = collapsableTiles[i];
                    leastTilesToCollapse = collapsableTileNum[i];
                }
                else if (collapsableTileNum[i] <= leastTilesToCollapse)
                {
                    tileToCollapse = collapsableTiles[i];
                    leastTilesToCollapse = collapsableTileNum[i];
                }
            }
        }
        if (tileToCollapse == null || leastTilesToCollapse < 0)
            return;
        // set curr tile to it
        currTile = tileToCollapse;
        // check neighbouring tiles for placeable tiles in current tile
        List<GameObject> availableTiles = GetAvailableTilesList();
        // choose random tile
        GameObject tileToSet = availableTiles[Random.Range(0, availableTiles.Count)];
        if (tileToSet == null)
            return;
        // set room tile
        mapTiles[(int)currTile.x + (int)(currTile.y * mapSize.x)] = tileToSet.GetComponent<MapTile>();
        //place room
        GameObject obj = Instantiate(tileToSet, transform);
        obj.transform.localPosition = currTile * tileSize;
        // remove tile from collapsable list
        collapsableTiles.Remove(tileToCollapse);
        collapsedTiles.Add(tileToCollapse);
        collapsableTileNum.Remove(leastTilesToCollapse);
        // add new collapsable tiles
        AddCollapsableTiles();
        // set next room
        SetNextTile();
    }

    private void SetBorderTiles()
    {
        // check through all border edges
        // top
        for (int i = 0; i < mapSize.x; i++)
        {
            Vector2 tilePos = new Vector2(i, mapSize.y);
            // check tile available up with top edge border tiles
            List<GameObject> availableTiles = GetAvailableBorderTilesList(tilePos - Vector2.up, Vector2.up);
            // choose random tile
            GameObject tileToSet = availableTiles[Random.Range(0, availableTiles.Count)];
            if (tileToSet == null)
                return;
            //place room
            GameObject obj = Instantiate(tileToSet, transform);
            obj.transform.localPosition = tilePos * tileSize;
            Debug.Log("Placed Border Up");
        }
        // bottom
        for (int i = 0; i < mapSize.x; i++)
        {
            Vector2 tilePos = new Vector2(i, -1);
            // check tile available up with top edge border tiles
            List<GameObject> availableTiles = GetAvailableBorderTilesList(tilePos - Vector2.down, Vector2.down);
            // choose random tile
            GameObject tileToSet = availableTiles[Random.Range(0, availableTiles.Count)];
            if (tileToSet == null)
                return;
            //place room
            GameObject obj = Instantiate(tileToSet, transform);
            obj.transform.localPosition = tilePos * tileSize;
        }
        // left
        for (int i = 0; i < mapSize.y; i++)
        {
            Vector2 tilePos = new Vector2(-1, i);
            // check tile available up with top edge border tiles
            List<GameObject> availableTiles = GetAvailableBorderTilesList(tilePos - Vector2.left, Vector2.left);
            // choose random tile
            GameObject tileToSet = availableTiles[Random.Range(0, availableTiles.Count)];
            if (tileToSet == null)
                return;
            //place room
            GameObject obj = Instantiate(tileToSet, transform);
            obj.transform.localPosition = tilePos * tileSize;
        }
        // right
        for (int i = 0; i < mapSize.y; i++)
        {
            Vector2 tilePos = new Vector2(mapSize.x, i);
            // check tile available up with top edge border tiles
            List<GameObject> availableTiles = GetAvailableBorderTilesList(tilePos - Vector2.right, Vector2.right);
            // choose random tile
            GameObject tileToSet = availableTiles[Random.Range(0, availableTiles.Count)];
            if (tileToSet == null)
                return;
            //place room
            GameObject obj = Instantiate(tileToSet, transform);
            obj.transform.localPosition = tilePos * tileSize;
        }
        // place corners
        GameObject corner;
        // top left corner
        corner = Instantiate(topLeftCornerTiles[Random.Range(0, topLeftCornerTiles.Count)], transform);
        corner.transform.localPosition = new Vector2(-1, mapSize.y) * tileSize;
        // top right corner
        corner = Instantiate(topRightCornerTiles[Random.Range(0, topRightCornerTiles.Count)], transform);
        corner.transform.localPosition = new Vector2(mapSize.x, mapSize.y) * tileSize;
        // bottom left corner
        corner = Instantiate(bottomLeftCornerTiles[Random.Range(0, bottomLeftCornerTiles.Count)], transform);
        corner.transform.localPosition = new Vector2(-1, -1) * tileSize;
        // bottom right corner
        corner = Instantiate(bottomRightCornerTiles[Random.Range(0, bottomRightCornerTiles.Count)], transform);
        corner.transform.localPosition = new Vector2(mapSize.x, -1) * tileSize;

        PlaceSolidBorderTiles();
    }

    private void PlaceSolidBorderTiles()
    {
        for (int i = -borderThickness - 1; i < mapSize.x + borderThickness + 1; i++)
        {
            for (int j = -borderThickness - 1; j < mapSize.y + borderThickness + 1; j++)
            {
                if (i < -1 || i > mapSize.x || j < -1 || j > mapSize.y)
                {
                    GameObject obj;
                    obj = Instantiate(solidTile, transform);
                    obj.transform.localPosition = new Vector2(i, j) * tileSize;
                }
            }
        }
    }

    private List<GameObject> GetAvailableBorderTilesList(Vector2 checkTilePos, Vector2 direction)
    {
        List<GameObject> availableTiles = new List<GameObject>();
        List<GameObject> tilesToRemove = new List<GameObject>();
        List<GameObject> placeableTiles;
        if (direction == Vector2.up)
        {
            placeableTiles = mapTiles[(int)checkTilePos.x + (int)(checkTilePos.y * mapSize.x)].availableTilesUp;
            // get list of tiles that are available in each list
            availableTiles.AddRange(allTilePrefabs);
            for (int i = 0; i < availableTiles.Count; i++)
            {
                if (!placeableTiles.Contains(availableTiles[i]) ||
                    !topBorderTiles.Contains(availableTiles[i]))
                {
                    tilesToRemove.Add(availableTiles[i]);
                }
            }
            for (int i = 0; i < tilesToRemove.Count; i++)
            {
                availableTiles.Remove(tilesToRemove[i]);
            }
        }
        else if (direction == Vector2.down)
        {
            placeableTiles = mapTiles[(int)checkTilePos.x + (int)(checkTilePos.y * mapSize.x)].availableTilesDown;
            // get list of tiles that are available in each list
            availableTiles.AddRange(allTilePrefabs);
            for (int i = 0; i < availableTiles.Count; i++)
            {
                if (!placeableTiles.Contains(availableTiles[i]) ||
                    !bottomBorderTiles.Contains(availableTiles[i]))
                {
                    tilesToRemove.Add(availableTiles[i]);
                }
            }
            for (int i = 0; i < tilesToRemove.Count; i++)
            {
                availableTiles.Remove(tilesToRemove[i]);
            }
        }
        else if (direction == Vector2.left)
        {
            placeableTiles = mapTiles[(int)checkTilePos.x + (int)(checkTilePos.y * mapSize.x)].availableTilesLeft;
            // get list of tiles that are available in each list
            availableTiles.AddRange(allTilePrefabs);
            for (int i = 0; i < availableTiles.Count; i++)
            {
                if (!placeableTiles.Contains(availableTiles[i]) ||
                    !leftBorderTiles.Contains(availableTiles[i]))
                {
                    tilesToRemove.Add(availableTiles[i]);
                }
            }
            for (int i = 0; i < tilesToRemove.Count; i++)
            {
                availableTiles.Remove(tilesToRemove[i]);
            }
        }
        else if (direction == Vector2.right)
        {
            placeableTiles = mapTiles[(int)checkTilePos.x + (int)(checkTilePos.y * mapSize.x)].availableTilesRight;
            // get list of tiles that are available in each list
            availableTiles.AddRange(allTilePrefabs);
            for (int i = 0; i < availableTiles.Count; i++)
            {
                if (!placeableTiles.Contains(availableTiles[i]) ||
                    !rightBorderTiles.Contains(availableTiles[i]))
                {
                    tilesToRemove.Add(availableTiles[i]);
                }
            }
            for (int i = 0; i < tilesToRemove.Count; i++)
            {
                availableTiles.Remove(tilesToRemove[i]);
            }
        }
        return availableTiles;
    }

    private List<GameObject> GetAvailableTilesList()
    {
        // get list of placeable tiles from each direction
        List<GameObject> availableTilesFromUp = GetAvailableTilesListFromNeighbour(Vector2.up);
        List<GameObject> availableTilesFromDown = GetAvailableTilesListFromNeighbour(Vector2.down);
        List<GameObject> availableTilesFromLeft = GetAvailableTilesListFromNeighbour(Vector2.left);
        List<GameObject> availableTilesFromRight = GetAvailableTilesListFromNeighbour(Vector2.right);
        // get list of tiles that are available in each list
        List<GameObject> availableTiles = new List<GameObject>();
        List<GameObject> tilesToRemove = new List<GameObject>();
        availableTiles.AddRange(allTilePrefabs);
        for (int i = 0; i < availableTiles.Count; i++)
        {
            if (!availableTilesFromUp.Contains(availableTiles[i]) || 
                !availableTilesFromDown.Contains(availableTiles[i]) || 
                !availableTilesFromLeft.Contains(availableTiles[i]) || 
                !availableTilesFromRight.Contains(availableTiles[i]))
            {
                tilesToRemove.Add(availableTiles[i]);
            }
        }
        for (int i = 0; i < tilesToRemove.Count; i++)
        {
            availableTiles.Remove(tilesToRemove[i]);
        }
        return availableTiles;
    }

    private List<GameObject> GetAvailableTilesListFromNeighbour(Vector2 direction)
    {
        Vector2 checkTilePos = currTile + direction;
        // check if not out of the map and if the space has an existing tile
        if (CheckTileInBounds(checkTilePos))
        {
            if (mapTiles[(int)checkTilePos.x + (int)(checkTilePos.y * mapSize.x)] != null)
            {
                if (direction == Vector2.up)
                {
                    return mapTiles[(int)checkTilePos.x + (int)(checkTilePos.y * mapSize.x)].availableTilesDown;
                }
                else if (direction == Vector2.down)
                {
                    return mapTiles[(int)checkTilePos.x + (int)(checkTilePos.y * mapSize.x)].availableTilesUp;
                }
                else if (direction == Vector2.left)
                {
                    return mapTiles[(int)checkTilePos.x + (int)(checkTilePos.y * mapSize.x)].availableTilesRight;
                }
                else if (direction == Vector2.right)
                {
                    return mapTiles[(int)checkTilePos.x + (int)(checkTilePos.y * mapSize.x)].availableTilesLeft;
                }
            }
        }
        return allTilePrefabs;
    }

    private void AddCollapsableTiles()
    {
        // add to list of placeable tiles in each direction and start from least amount of tiles
        if (CheckTileInBounds(currTile + Vector2.up) && mapTiles[(int)currTile.x + (int)((currTile.y + 1) * mapSize.x)] == null)
        {
            if (!collapsedTiles.Contains(currTile + Vector2.up) && !collapsableTiles.Contains(currTile + Vector2.up))
            {
                collapsableTiles.Add(currTile + Vector2.up);
                collapsableTileNum.Add(GetCurrentRoomTile().availableTilesUp.Count);
            }
        }
        if (CheckTileInBounds(currTile + Vector2.down) && mapTiles[(int)currTile.x + (int)((currTile.y - 1) * mapSize.x)] == null)
        {
            if (!collapsedTiles.Contains(currTile + Vector2.down) && !collapsableTiles.Contains(currTile + Vector2.down))
            {
                collapsableTiles.Add(currTile + Vector2.down);
                collapsableTileNum.Add(GetCurrentRoomTile().availableTilesDown.Count);
            }
        }
        if (CheckTileInBounds(currTile + Vector2.left) && mapTiles[(int)(currTile.x - 1) + (int)(currTile.y * mapSize.x)] == null)
        {
            if (!collapsedTiles.Contains(currTile + Vector2.left) && !collapsableTiles.Contains(currTile + Vector2.left))
            {
                collapsableTiles.Add(currTile + Vector2.left);
                collapsableTileNum.Add(GetCurrentRoomTile().availableTilesLeft.Count);
            }
        }
        if (CheckTileInBounds(currTile + Vector2.right) && mapTiles[(int)(currTile.x + 1) + (int)(currTile.y * mapSize.x)] == null)
        {
            if (!collapsedTiles.Contains(currTile + Vector2.right) && !collapsableTiles.Contains(currTile + Vector2.right))
            {
                collapsableTiles.Add(currTile + Vector2.right);
                collapsableTileNum.Add(GetCurrentRoomTile().availableTilesRight.Count);
            }
        }
    }

    private bool CheckTileInBounds(Vector2 pos)
    {
        return pos.x >= 0 && pos.x < mapSize.x && pos.y >= 0 && pos.y < mapSize.y;
    }

    public MapTile GetCurrentRoomTile()
    {
        if (CheckTileInBounds(currTile))
            return mapTiles[(int)currTile.x + (int)(currTile.y * mapSize.x)];
        return null;
    }
}
