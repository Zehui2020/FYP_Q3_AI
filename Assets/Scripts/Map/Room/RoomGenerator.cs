using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    [SerializeField] private Vector2 roomSize;
    [SerializeField] private float tileSize;
    [SerializeField] private GameObject doorPrefab;
    [SerializeField] private List<GameObject> startingTilePrefabs;
    [SerializeField] private List<GameObject> allTilePrefabs;
    [SerializeField] private List<RoomTile> roomTiles;
    private Vector2 currTile;
    private List<Vector2> collapsableTiles = new List<Vector2>();
    private List<Vector2> collapsedTiles = new List<Vector2>();
    private List<int> collapsableTileNum = new List<int>();

    private void Start()
    {
        GenerateRooms();
    }

    public void GenerateRooms()
    {
        // randomize starting node
        currTile = new Vector2(Random.Range(0, (int)roomSize.x), Random.Range(0, (int)roomSize.y));
        // set random starting room tile
        int randomIndex = Random.Range(0, startingTilePrefabs.Count);
        roomTiles[(int)currTile.x + (int)(currTile.y * roomSize.x)] = startingTilePrefabs[randomIndex].GetComponent<RoomTile>();
        //place room
        GameObject obj = Instantiate(startingTilePrefabs[randomIndex], transform);
        obj.transform.localPosition = currTile * tileSize;
        // add new collapsable tiles
        collapsedTiles.Add(currTile);
        AddCollapsableTiles();
        // set rooms
        SetNextRoom();
    }

    private void SetNextRoom()
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
                else if (collapsableTileNum[i] < leastTilesToCollapse)
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
        roomTiles[(int)currTile.x + (int)(currTile.y * roomSize.x)] = tileToSet.GetComponent<RoomTile>();
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
        SetNextRoom();
    }

    private List<GameObject> GetAvailableTilesList()
    {
        // get list of placeable tiles from each direction
        List<GameObject> availableTilesUp = GetAvailableTilesListFromNeighbour(Vector2.up);
        List<GameObject> availableTilesDown = GetAvailableTilesListFromNeighbour(Vector2.down);
        List<GameObject> availableTilesLeft = GetAvailableTilesListFromNeighbour(Vector2.left);
        List<GameObject> availableTilesRight = GetAvailableTilesListFromNeighbour(Vector2.right);
        // get list of tiles that are available in each list
        List<GameObject> availableTiles = new List<GameObject>();
        availableTiles.AddRange(allTilePrefabs);
        for (int i = 0; i < availableTiles.Count; i++)
        {
            if (!availableTilesUp.Contains(availableTiles[i]) || 
                !availableTilesDown.Contains(availableTiles[i]) || 
                !availableTilesLeft.Contains(availableTiles[i]) || 
                !availableTilesRight.Contains(availableTiles[i]))
            {
                availableTiles.Remove(availableTiles[i]);
            }
        }
        return availableTiles;
    }

    private List<GameObject> GetAvailableTilesListFromNeighbour(Vector2 direction)
    {
        Vector2 checkTilePos = currTile + direction;
        // check if not out of the map and if the space has an existing tile
        if (CheckTileInBounds(checkTilePos))
        {
            if (roomTiles[(int)checkTilePos.x + (int)(checkTilePos.y * roomSize.x)] != null)
            {
                if (direction == Vector2.up)
                {
                    return roomTiles[(int)checkTilePos.x + (int)(checkTilePos.y * roomSize.x)].availableTilesDown;
                }
                else if (direction == Vector2.down)
                {
                    return roomTiles[(int)checkTilePos.x + (int)(checkTilePos.y * roomSize.x)].availableTilesUp;
                }
                else if (direction == Vector2.left)
                {
                    return roomTiles[(int)checkTilePos.x + (int)(checkTilePos.y * roomSize.x)].availableTilesRight;
                }
                else if (direction == Vector2.right)
                {
                    return roomTiles[(int)checkTilePos.x + (int)(checkTilePos.y * roomSize.x)].availableTilesLeft;
                }
            }
        }
        return allTilePrefabs;
    }

    private void AddCollapsableTiles()
    {
        // add to list of placeable tiles in each direction and start from least amount of tiles
        if (CheckTileInBounds(currTile + Vector2.up) && roomTiles[(int)currTile.x + (int)((currTile.y + 1) * roomSize.x)] == null)
        {
            if (!collapsedTiles.Contains(currTile + Vector2.up) && !collapsableTiles.Contains(currTile + Vector2.up))
            {
                collapsableTiles.Add(currTile + Vector2.up);
                collapsableTileNum.Add(GetCurrentRoomTile().availableTilesUp.Count);
            }
        }
        if (CheckTileInBounds(currTile + Vector2.down) && roomTiles[(int)currTile.x + (int)((currTile.y - 1) * roomSize.x)] == null)
        {
            if (!collapsedTiles.Contains(currTile + Vector2.down) && !collapsableTiles.Contains(currTile + Vector2.down))
            {
                collapsableTiles.Add(currTile + Vector2.down);
                collapsableTileNum.Add(GetCurrentRoomTile().availableTilesDown.Count);
            }
        }
        if (CheckTileInBounds(currTile + Vector2.left) && roomTiles[(int)(currTile.x - 1) + (int)(currTile.y * roomSize.x)] == null)
        {
            if (!collapsedTiles.Contains(currTile + Vector2.left) && !collapsableTiles.Contains(currTile + Vector2.left))
            {
                collapsableTiles.Add(currTile + Vector2.left);
                collapsableTileNum.Add(GetCurrentRoomTile().availableTilesLeft.Count);
            }
        }
        if (CheckTileInBounds(currTile + Vector2.right) && roomTiles[(int)(currTile.x + 1) + (int)(currTile.y * roomSize.x)] == null)
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
        return pos.x >= 0 && pos.x < roomSize.x && pos.y >= 0 && pos.y < roomSize.y;
    }

    public RoomTile GetCurrentRoomTile()
    {
        if (CheckTileInBounds(currTile))
            return roomTiles[(int)currTile.x + (int)(currTile.y * roomSize.x)];
        return null;
    }
}
