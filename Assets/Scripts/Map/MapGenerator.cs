using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private MapData mData;
    [SerializeField] private GameObject mapContainer;

    private List<RoomData> roomData = new List<RoomData>();
    private List<GameObject> roomList = new List<GameObject>();
    private List<Vector2> spaceTaken = new List<Vector2>();
    private List<Vector2> directions = new List<Vector2>
        {
            new Vector2(0, 1),
            new Vector2(0, -1),
            new Vector2(1, 0),
            new Vector2(-1, 0)
        };

    private float temp = 0;

    private void Start()
    {
        SpawnRooms();
    }

    private void Update()
    {
        // debug inputs
        if (Input.GetKeyDown(KeyCode.E))
        {
            SpawnRooms();
        }

        if (temp < 1)
        {
            temp += Time.deltaTime;
        }
        else
        {
            SpawnRooms();
            temp = 0;
        }
    }

    private GameObject GetRandomRoom()
    {
        // add random room to the list of rooms
        GameObject obj = mData.roomTypes[Random.Range(0, mData.roomTypes.Count)];
        roomData.Add(obj.GetComponent<RoomData>());
        return obj;
    }

    private void SpawnRooms()
    {
        // clear map
        foreach (GameObject room in roomList)
        {
            Destroy(room);
        }
        roomList.Clear();
        roomData.Clear();
        spaceTaken.Clear();
        Vector2 roomPos = Vector2.zero;
        // get random room and place it
        GameObject roomToSpawn = GetRandomRoom();
        GameObject obj = Instantiate(mData.roomTypes[0]);
        obj.transform.SetParent(mapContainer.transform);
        obj.transform.position = roomPos;
        spaceTaken.Add(Vector2.zero);
        roomList.Add(obj);
        // spawn other rooms
        for (int i = 1; i < mData.noOfRooms; i++)
        {
            roomToSpawn = GetRandomRoom();
            Vector2 dir = GetRandomDirection(0);
            obj = Instantiate(roomToSpawn);
            obj.transform.SetParent(mapContainer.transform);
            obj.transform.position = roomPos + (dir * (roomData[i].roomSize + roomData[i - 1].roomSize / 2));
            spaceTaken.Add(spaceTaken[spaceTaken.Count - 1] + dir);
            roomList.Add(obj);
            roomPos = obj.transform.position;
        }
    }

    private Vector2 GetRandomDirection(int num)
    {
        int i = Random.Range(0, directions.Count - num);
        Vector2 dir = directions[i];
        directions.RemoveAt(i);
        directions.Add(dir);
        if (spaceTaken.Contains(spaceTaken[spaceTaken.Count - 1] + dir))
        {
            dir = GetRandomDirection(num++);
        }
        return dir;
    }
}
