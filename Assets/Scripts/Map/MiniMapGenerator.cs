using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapGenerator : MonoBehaviour
{
    [SerializeField] public MapData mData;
    [SerializeField] private GameObject mapContainer;
    [SerializeField] private GameObject mapIndicator;

    private List<GameObject> pathObjectsList = new List<GameObject>();
    private List<GameObject> spaceObjectsList = new List<GameObject>();
    private List<Vector2> takenRooms = new List<Vector2>();
    private int mapSeed = 0;
    private GameObject createdObj;
    private int currIndicatorNode;
    private bool isShowMap = true;

    private void Update()
    {
        HandleMinimapIndicator(takenRooms);
        HandleMinimap();
    }

    public void StartMapGeneration(int seed, List<Vector2> availableRooms, List<Vector2> takenRooms)
    {
        ResetMap();
        SetSeed(seed);
        this.takenRooms = new List<Vector2>(takenRooms);
        // place rooms
        PlaceRooms(availableRooms, takenRooms);
        ConfigureRoomDoors();
    }

    private void ResetMap()
    {
        foreach (GameObject obj in pathObjectsList)
        {
            Destroy(obj);
        }
        foreach (GameObject obj in spaceObjectsList)
        {
            Destroy(obj);
        }
        pathObjectsList.Clear();
        spaceObjectsList.Clear();
        takenRooms.Clear();
    }

    public void SetSeed(int seed)
    {
        mapSeed = seed;
        Random.seed = mapSeed;
    }

    private void PlaceRooms(List<Vector2> availableRooms, List<Vector2> takenRooms)
    {
        // place normal rooms
        for (int j = 0; j < takenRooms.Count; j++)
        {
            createdObj = Instantiate(GetRandomRoomFromType(1));
            createdObj.transform.SetParent(mapContainer.transform);
            createdObj.transform.localPosition = takenRooms[j];
            createdObj.transform.localScale = new Vector3(1, 1, 1);
            pathObjectsList.Add(createdObj);
        }
        // place remaining type 0 rooms
        for (int i = 0; i < availableRooms.Count; i++)
        {
            createdObj = Instantiate(GetRandomRoomFromType(0));
            createdObj.transform.SetParent(mapContainer.transform);
            createdObj.transform.localPosition = availableRooms[i];
            createdObj.transform.localScale = new Vector3(1, 1, 1);
            spaceObjectsList.Add(createdObj);
        }
        // position indicator
        mapIndicator.transform.localPosition = takenRooms[0];
        currIndicatorNode = 0;
        pathObjectsList[currIndicatorNode].GetComponent<RoomController>().ToggleRoomCover(false);
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

    private void HandleMinimapIndicator(List<Vector2> takenRooms)
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            RoomController rData = pathObjectsList[currIndicatorNode].GetComponent<RoomController>();
            if (rData.isSpaceOccupied[0])
            {
                for (int i = 0; i < takenRooms.Count; i++)
                {
                    if (takenRooms[i].x == takenRooms[currIndicatorNode].x &&
                        takenRooms[i].y == takenRooms[currIndicatorNode].y + mData.roomSpacing)
                    {
                        currIndicatorNode = i;
                        pathObjectsList[currIndicatorNode].GetComponent<RoomController>().ToggleRoomCover(false);
                        break;
                    }
                }
            }
            mapIndicator.transform.localPosition = takenRooms[currIndicatorNode];
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            RoomController rData = pathObjectsList[currIndicatorNode].GetComponent<RoomController>();
            if (rData.isSpaceOccupied[1])
            {
                for (int i = 0; i < takenRooms.Count; i++)
                {
                    if (takenRooms[i].x == takenRooms[currIndicatorNode].x &&
                        takenRooms[i].y == takenRooms[currIndicatorNode].y - mData.roomSpacing)
                    {
                        currIndicatorNode = i;
                        pathObjectsList[currIndicatorNode].GetComponent<RoomController>().ToggleRoomCover(false);
                        break;
                    }
                }
            }
            mapIndicator.transform.localPosition = takenRooms[currIndicatorNode];
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            RoomController rData = pathObjectsList[currIndicatorNode].GetComponent<RoomController>();
            if (rData.isSpaceOccupied[2])
            {
                for (int i = 0; i < takenRooms.Count; i++)
                {
                    if (takenRooms[i].x == takenRooms[currIndicatorNode].x - mData.roomSpacing &&
                        takenRooms[i].y == takenRooms[currIndicatorNode].y)
                    {
                        currIndicatorNode = i;
                        pathObjectsList[currIndicatorNode].GetComponent<RoomController>().ToggleRoomCover(false);
                        break;
                    }
                }
            }
            mapIndicator.transform.localPosition = takenRooms[currIndicatorNode];
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            RoomController rData = pathObjectsList[currIndicatorNode].GetComponent<RoomController>();
            if (rData.isSpaceOccupied[3])
            {
                for (int i = 0; i < takenRooms.Count; i++)
                {
                    if (takenRooms[i].x == takenRooms[currIndicatorNode].x + mData.roomSpacing &&
                        takenRooms[i].y == takenRooms[currIndicatorNode].y)
                    {
                        currIndicatorNode = i;
                        pathObjectsList[currIndicatorNode].GetComponent<RoomController>().ToggleRoomCover(false);
                        break;
                    }
                }
            }
            mapIndicator.transform.localPosition = takenRooms[currIndicatorNode];
        }
    }

    private void HandleMinimap()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            isShowMap = !isShowMap;
            mapContainer.SetActive(isShowMap);
        }
    }

    private GameObject GetRandomRoomFromType(int type)
    {
        switch (type)
        {
            case 0:
                return mData.roomType0[Random.Range(0, mData.roomType0.Count)];
            case 1:
                return mData.roomType1[Random.Range(0, mData.roomType1.Count)];
            case 2:
                return mData.roomType2[Random.Range(0, mData.roomType2.Count)];
            case 3:
                return mData.roomType3[Random.Range(0, mData.roomType3.Count)];
            default:
                return null;
        }
    }
}