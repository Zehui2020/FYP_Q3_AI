using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    [SerializeField] private Vector2 roomSize;
    [SerializeField] private GameObject doorPrefab;
    [SerializeField] private List<GameObject> startingTilePrefabs;
    [SerializeField] private List<List<RoomTile>> roomTiles;
    private Vector2 currTile;

    public void GenerateRoom()
    {
        // randomize starting node
        currTile = new Vector2(Random.Range(0, roomSize.x - 1), Random.Range(0, roomSize.y - 1));
        // set random starting room tile
        roomTiles[(int)currTile.x][(int)currTile.y] = startingTilePrefabs[Random.Range(0, startingTilePrefabs.Count)].GetComponent<RoomTile>();
       // set rooms
       SetNextRoom();
    }

    private void SetNextRoom()
    {
        // get random available direction
        Vector2 direction = GetCurrentRoomTile().GetRandomAvailableDirection();
        // update current tile if direction not null 
        if (direction != Vector2.zero)
            currTile += direction;
        else
            Debug.Log("Room Generator Died :D");
        // check neighbouring tiles for placeable tiles in current tile
        List<GameObject> availableTiles = GetAvailableTilesList();
        // choose random tile
        GameObject tileToSet = availableTiles[Random.Range(0, availableTiles.Count)];
        if (tileToSet == null)
            return;
        // set room tile
        roomTiles[(int)currTile.x][(int)currTile.y] = tileToSet.GetComponent<RoomTile>();
        // set next room
        SetNextRoom();
    }

    private List<GameObject> GetAvailableTilesList()
    {
        // get list of placeable tiles from each direction
        List<GameObject> availableTilesUp = GetAvailableTilesListInDirection(Vector2.up);
        List<GameObject> availableTilesDown = GetAvailableTilesListInDirection(Vector2.down);
        List<GameObject> availableTilesLeft = GetAvailableTilesListInDirection(Vector2.left);
        List<GameObject> availableTilesRight = GetAvailableTilesListInDirection(Vector2.right);
        // get list of tiles that are available in each list
        List<GameObject> availableTiles = new List<GameObject>();
        for (int i = 0; i < availableTilesUp.Count; i++)
        {
            for (int j = 0; j < availableTilesDown.Count; j++)
            {
                for (int k = 0; k < availableTilesLeft.Count; k++)
                {
                    for (int l = 0; l < availableTilesRight.Count; l++)
                    {
                        if (availableTilesUp[i] == availableTilesDown[j] == availableTilesLeft[k] == availableTilesRight[l])
                        {
                            availableTiles.Add(availableTilesUp[i]);
                        }
                    }
                }
            }
        }
        return availableTiles;
    }

    private List<GameObject> GetAvailableTilesListInDirection(Vector2 direction)
    {
        Vector2 checkTilePos = currTile + direction;
        // check if not out of the map and if the space has an existing tile
        if (checkTilePos.x >= 0 && checkTilePos.x < roomSize.x && checkTilePos.y >= 0 && checkTilePos.y < roomSize.y)
            if (roomTiles[(int)checkTilePos.x][(int)checkTilePos.y] != null)
                return roomTiles[(int)checkTilePos.x][(int)checkTilePos.y].availableTilesDown;
        return new List<GameObject>{ null };
    }

    public RoomTile GetCurrentRoomTile()
    {
        return roomTiles[(int)currTile.x][(int)currTile.y];
    }
}
