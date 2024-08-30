using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapGenerator : MonoBehaviour
{
    [SerializeField] public MapData mData;

    public GameObject mapContainer;
    public GameObject mapIndicator;
    public List<GameObject> pathObjectsList = new List<GameObject>();
    public List<Vector3> takenRooms;
    public bool isShowMap = false;

    private int mapSeed = 0;
    private GameObject createdObj;

    public void StartMapGeneration(int seed, List<Vector3> takenRooms, int roomsAdded)
    {
        ResetMap();
        SetSeed(seed);
        this.takenRooms = new List<Vector3>(takenRooms);
        // place rooms
        PlaceRooms(roomsAdded);
        ConfigureRoomDoors();
        pathObjectsList[0].GetComponent<RoomController>().ToggleRoomCover(true);
    }

    private void ResetMap()
    {
        foreach (GameObject obj in pathObjectsList)
        {
            Destroy(obj);
        }
        pathObjectsList.Clear();
        takenRooms.Clear();
    }

    public void SetSeed(int seed)
    {
        mapSeed = seed;
        Random.InitState(mapSeed);
    }

    private void PlaceRooms(int roomsAdded)
    {
        // randomize shop room location (1-2 spaces before end room)
        int shopIndex = Random.Range(2, 4);
        // place starting room
        CreateRoom(0, mData.startRoom);
        // place normal rooms
        for (int j = 1; j < takenRooms.Count; j++)
        {
            // place end room
            if (j == takenRooms.Count - 1 - roomsAdded)
            {
                CreateRoom(j, mData.endRoom);
            }
            // place shop room
            else if (j == takenRooms.Count - shopIndex - roomsAdded)
            {
                CreateRoom(j, mData.shopRoom);
            }
            // place other rooms
            else
            {
                CreateRoom(j, GetRandomRoomFromType((int)takenRooms[j].z));
            }
        }
    }

    private void CreateRoom(int posInList, GameObject roomObject)
    {
        // place room
        createdObj = Instantiate(roomObject);
        createdObj.transform.SetParent(mapContainer.transform);
        createdObj.transform.localPosition = new Vector3(takenRooms[posInList].x, takenRooms[posInList].y, 0);
        createdObj.transform.localScale = new Vector3(1, 1, 1); 
        pathObjectsList.Add(createdObj);
    }

    private void ConfigureRoomDoors()
    {
        for (int i = 0; i < pathObjectsList.Count; i++)
        {
            RoomController rData = pathObjectsList[i].GetComponent<RoomController>();
            // check all directions for spaces or rooms
            // up
            if (CheckAdjacentSpaceTaken(takenRooms[i], 1))
            {
                rData.isSpaceOccupied[0] = true;
            }
            // down
            if (CheckAdjacentSpaceTaken(takenRooms[i], 2))
            {
                rData.isSpaceOccupied[1] = true;
            }
            // left
            if (CheckAdjacentSpaceTaken(takenRooms[i], 3))
            {
                rData.isSpaceOccupied[2] = true;
            }
            // right
            if (CheckAdjacentSpaceTaken(takenRooms[i], 4))
            {
                rData.isSpaceOccupied[3] = true;
            }
            // update doors
            rData.UpdateDoors();
            rData.ToggleRoomCover(false);
        }
    }

    private bool CheckAdjacentSpaceTaken(Vector2 pos, int dir)
    {
        switch (dir)
        {
            case 1:
                // up
                if (takenRooms.Contains(new Vector3(pos.x, pos.y + mData.roomSpacing, 0)))
                {
                    return true;
                }
                break;
            case 2:
                // down
                if (takenRooms.Contains(new Vector3(pos.x, pos.y - mData.roomSpacing, 0)))
                {
                    return true;
                }
                break;
            case 3:
                // left
                if (takenRooms.Contains(new Vector3(pos.x - mData.roomSpacing, pos.y, 0)))
                {
                    return true;
                }
                break;
            case 4:
                // right
                if (takenRooms.Contains(new Vector3(pos.x + mData.roomSpacing, pos.y, 0)))
                {
                    return true;
                }
                break;
            case 5:
                // up
                if (takenRooms.Contains(new Vector3(pos.x, pos.y + mData.roomSpacing, 0)))
                {
                    return true;
                }
                // down
                if (takenRooms.Contains(new Vector3(pos.x, pos.y - mData.roomSpacing, 0)))
                {
                    return true;
                }
                // left
                if (takenRooms.Contains(new Vector3(pos.x - mData.roomSpacing, pos.y, 0)))
                {
                    return true;
                }
                // right
                if (takenRooms.Contains(new Vector3(pos.x + mData.roomSpacing, pos.y, 0)))
                {
                    return true;
                }
                break;
        }
        return false;
    }

    private GameObject GetRandomRoomFromType(int type)
    {
        switch (type)
        {
            case 0:
                return mData.enemyRoom[Random.Range(0, mData.enemyRoom.Count)];
            case 1:
                return mData.eliteRoom[Random.Range(0, mData.eliteRoom.Count)];
            case 2:
                return mData.puzzleRoom[Random.Range(0, mData.puzzleRoom.Count)];
            default:
                return null;
        }
    }
}