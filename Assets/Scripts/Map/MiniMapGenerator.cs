using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapGenerator : MonoBehaviour
{
    [SerializeField] public MapData mData;

    public GameObject mapContainer;
    public GameObject mapIndicator;
    public List<GameObject> pathObjectsList = new List<GameObject>();
    public List<Vector2> takenRooms;
    public bool isShowMap = false;

    private int mapSeed = 0;
    private GameObject createdObj;

    public void StartMapGeneration(int seed, List<Vector2> takenRooms, int roomsAdded)
    {
        ResetMap();
        SetSeed(seed);
        this.takenRooms = new List<Vector2>(takenRooms);
        // place rooms
        PlaceRooms(roomsAdded);
        ConfigureRoomDoors();
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
                CreateRoom(j, mData.roomType1[Random.Range(0, mData.roomType1.Count)]);
            }
        }
        pathObjectsList[0].GetComponent<RoomController>().ToggleRoomCover(true);
    }

    private void CreateRoom(int posInList, GameObject roomObject)
    {
        // place room
        createdObj = Instantiate(roomObject);
        createdObj.transform.SetParent(mapContainer.transform);
        createdObj.transform.localPosition = takenRooms[posInList];
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
            if (takenRooms.Contains(new Vector3(takenRooms[i].x, takenRooms[i].y + mData.roomSpacing, 1)) ||
                takenRooms.Contains(new Vector3(takenRooms[i].x, takenRooms[i].y + mData.roomSpacing, 2)) ||
                takenRooms.Contains(new Vector3(takenRooms[i].x, takenRooms[i].y + mData.roomSpacing, 3)))
            {
                rData.isSpaceOccupied[0] = true;
            }
            // down
            if (takenRooms.Contains(new Vector3(takenRooms[i].x, takenRooms[i].y - mData.roomSpacing, 1)) ||
                takenRooms.Contains(new Vector3(takenRooms[i].x, takenRooms[i].y - mData.roomSpacing, 2)) ||
                takenRooms.Contains(new Vector3(takenRooms[i].x, takenRooms[i].y - mData.roomSpacing, 3)))
            {
                rData.isSpaceOccupied[1] = true;
            }
            // left
            if (takenRooms.Contains(new Vector3(takenRooms[i].x - mData.roomSpacing, takenRooms[i].y, 1)) ||
                takenRooms.Contains(new Vector3(takenRooms[i].x - mData.roomSpacing, takenRooms[i].y, 2)) ||
                takenRooms.Contains(new Vector3(takenRooms[i].x - mData.roomSpacing, takenRooms[i].y, 3)))
            {
                rData.isSpaceOccupied[2] = true;
            }
            // right
            if (takenRooms.Contains(new Vector3(takenRooms[i].x + mData.roomSpacing, takenRooms[i].y, 1)) ||
                takenRooms.Contains(new Vector3(takenRooms[i].x + mData.roomSpacing, takenRooms[i].y, 2)) ||
                takenRooms.Contains(new Vector3(takenRooms[i].x + mData.roomSpacing, takenRooms[i].y, 3)))
            {
                rData.isSpaceOccupied[3] = true;
            }
            // update doors
            rData.UpdateDoors();
        }
    }
}