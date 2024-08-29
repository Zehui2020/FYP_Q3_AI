using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapController : MonoBehaviour
{
    public ProceduralMapGenerator map;

    private List<MiniMapGenerator> miniMap = new List<MiniMapGenerator>();

    private void Start()
    {
        map = GameObject.FindGameObjectWithTag("MainMap").GetComponent<ProceduralMapGenerator>();
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("MiniMap"))
        {
            miniMap.Add(obj.GetComponent<MiniMapGenerator>());
        }
        for (int i = 0; i < miniMap.Count; i++)
        {
            miniMap[i].mapContainer.SetActive(miniMap[i].isShowMap);
        }
    }

    private void Update()
    {
        HandleMinimapIndicator();
        HandleMinimap();
    }

    private void HandleMinimapIndicator()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            RoomController rData = miniMap[0].pathObjectsList[map.GetCurrIndicatorNode()].GetComponent<RoomController>();
            if (rData.isSpaceOccupied[0])
            {
                for (int i = 0; i < miniMap[0].takenRooms.Count; i++)
                {
                    if (miniMap[0].takenRooms[i].x == miniMap[0].takenRooms[map.GetCurrIndicatorNode()].x &&
                        miniMap[0].takenRooms[i].y == miniMap[0].takenRooms[map.GetCurrIndicatorNode()].y + miniMap[0].mData.roomSpacing)
                    {
                        map.SetIndicatorNode(i);
                        for (int j = 0; j < miniMap.Count; j++)
                        {
                            miniMap[j].pathObjectsList[map.GetCurrIndicatorNode()].GetComponent<RoomController>().ToggleRoomCover(true);
                        }
                        break;
                    }
                }
            }
            for (int j = 0; j < miniMap.Count; j++)
            {
                miniMap[j].mapIndicator.transform.localPosition = miniMap[j].takenRooms[map.GetCurrIndicatorNode()];
            }
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            RoomController rData = miniMap[0].pathObjectsList[map.GetCurrIndicatorNode()].GetComponent<RoomController>();
            if (rData.isSpaceOccupied[1])
            {
                for (int i = 0; i < miniMap[0].takenRooms.Count; i++)
                {
                    if (miniMap[0].takenRooms[i].x == miniMap[0].takenRooms[map.GetCurrIndicatorNode()].x &&
                        miniMap[0].takenRooms[i].y == miniMap[0].takenRooms[map.GetCurrIndicatorNode()].y - miniMap[0].mData.roomSpacing)
                    {
                        map.SetIndicatorNode(i);
                        for (int j = 0; j < miniMap.Count; j++)
                        {
                            miniMap[j].pathObjectsList[map.GetCurrIndicatorNode()].GetComponent<RoomController>().ToggleRoomCover(true);
                        }
                        break;
                    }
                }
            }
            for (int j = 0; j < miniMap.Count; j++)
            {
                miniMap[j].mapIndicator.transform.localPosition = miniMap[j].takenRooms[map.GetCurrIndicatorNode()];
            }
        }
        else if (Input.GetKeyDown(KeyCode.J))
        {
            RoomController rData = miniMap[0].pathObjectsList[map.GetCurrIndicatorNode()].GetComponent<RoomController>();
            if (rData.isSpaceOccupied[2])
            {
                for (int i = 0; i < miniMap[0].takenRooms.Count; i++)
                {
                    if (miniMap[0].takenRooms[i].x == miniMap[0].takenRooms[map.GetCurrIndicatorNode()].x - miniMap[0].mData.roomSpacing &&
                        miniMap[0].takenRooms[i].y == miniMap[0].takenRooms[map.GetCurrIndicatorNode()].y)
                    {
                        map.SetIndicatorNode(i);
                        for (int j = 0; j < miniMap.Count; j++)
                        {
                            miniMap[j].pathObjectsList[map.GetCurrIndicatorNode()].GetComponent<RoomController>().ToggleRoomCover(true);
                        }
                        break;
                    }
                }
            }
            for (int j = 0; j < miniMap.Count; j++)
            {
                miniMap[j].mapIndicator.transform.localPosition = miniMap[j].takenRooms[map.GetCurrIndicatorNode()];
            }
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            RoomController rData = miniMap[0].pathObjectsList[map.GetCurrIndicatorNode()].GetComponent<RoomController>();
            if (rData.isSpaceOccupied[3])
            {
                for (int i = 0; i < miniMap[0].takenRooms.Count; i++)
                {
                    if (miniMap[0].takenRooms[i].x == miniMap[0].takenRooms[map.GetCurrIndicatorNode()].x + miniMap[0].mData.roomSpacing &&
                        miniMap[0].takenRooms[i].y == miniMap[0].takenRooms[map.GetCurrIndicatorNode()].y)
                    {
                        map.SetIndicatorNode(i);
                        for (int j = 0; j < miniMap.Count; j++)
                        {
                            miniMap[j].pathObjectsList[map.GetCurrIndicatorNode()].GetComponent<RoomController>().ToggleRoomCover(true);
                        }
                        break;
                    }
                }
            }
            for (int j = 0; j < miniMap.Count; j++)
            {
                miniMap[j].mapIndicator.transform.localPosition = miniMap[j].takenRooms[map.GetCurrIndicatorNode()];
            }
        }
    }

    public void MoveNode(int dir)
    {
        if (dir == 1)
        {
            RoomController rData = miniMap[0].pathObjectsList[map.GetCurrIndicatorNode()].GetComponent<RoomController>();
            if (rData.isSpaceOccupied[0])
            {
                for (int i = 0; i < miniMap[0].takenRooms.Count; i++)
                {
                    if (miniMap[0].takenRooms[i].x == miniMap[0].takenRooms[map.GetCurrIndicatorNode()].x &&
                        miniMap[0].takenRooms[i].y == miniMap[0].takenRooms[map.GetCurrIndicatorNode()].y + miniMap[0].mData.roomSpacing)
                    {
                        map.SetIndicatorNode(i);
                        for (int j = 0; j < miniMap.Count; j++)
                        {
                            miniMap[j].pathObjectsList[map.GetCurrIndicatorNode()].GetComponent<RoomController>().ToggleRoomCover(true);
                        }
                        break;
                    }
                }
            }
            for (int j = 0; j < miniMap.Count; j++)
            {
                miniMap[j].mapIndicator.transform.localPosition = miniMap[j].takenRooms[map.GetCurrIndicatorNode()];
            }
        }
        else if (dir == 2)
        {
            RoomController rData = miniMap[0].pathObjectsList[map.GetCurrIndicatorNode()].GetComponent<RoomController>();
            if (rData.isSpaceOccupied[1])
            {
                for (int i = 0; i < miniMap[0].takenRooms.Count; i++)
                {
                    if (miniMap[0].takenRooms[i].x == miniMap[0].takenRooms[map.GetCurrIndicatorNode()].x &&
                        miniMap[0].takenRooms[i].y == miniMap[0].takenRooms[map.GetCurrIndicatorNode()].y - miniMap[0].mData.roomSpacing)
                    {
                        map.SetIndicatorNode(i);
                        for (int j = 0; j < miniMap.Count; j++)
                        {
                            miniMap[j].pathObjectsList[map.GetCurrIndicatorNode()].GetComponent<RoomController>().ToggleRoomCover(true);
                        }
                        break;
                    }
                }
            }
            for (int j = 0; j < miniMap.Count; j++)
            {
                miniMap[j].mapIndicator.transform.localPosition = miniMap[j].takenRooms[map.GetCurrIndicatorNode()];
            }
        }
        else if (dir == 3)
        {
            RoomController rData = miniMap[0].pathObjectsList[map.GetCurrIndicatorNode()].GetComponent<RoomController>();
            if (rData.isSpaceOccupied[2])
            {
                for (int i = 0; i < miniMap[0].takenRooms.Count; i++)
                {
                    if (miniMap[0].takenRooms[i].x == miniMap[0].takenRooms[map.GetCurrIndicatorNode()].x - miniMap[0].mData.roomSpacing &&
                        miniMap[0].takenRooms[i].y == miniMap[0].takenRooms[map.GetCurrIndicatorNode()].y)
                    {
                        map.SetIndicatorNode(i);
                        for (int j = 0; j < miniMap.Count; j++)
                        {
                            miniMap[j].pathObjectsList[map.GetCurrIndicatorNode()].GetComponent<RoomController>().ToggleRoomCover(true);
                        }
                        break;
                    }
                }
            }
            for (int j = 0; j < miniMap.Count; j++)
            {
                miniMap[j].mapIndicator.transform.localPosition = miniMap[j].takenRooms[map.GetCurrIndicatorNode()];
            }
        }
        else if (dir == 4)
        {
            RoomController rData = miniMap[0].pathObjectsList[map.GetCurrIndicatorNode()].GetComponent<RoomController>();
            if (rData.isSpaceOccupied[3])
            {
                for (int i = 0; i < miniMap[0].takenRooms.Count; i++)
                {
                    if (miniMap[0].takenRooms[i].x == miniMap[0].takenRooms[map.GetCurrIndicatorNode()].x + miniMap[0].mData.roomSpacing &&
                        miniMap[0].takenRooms[i].y == miniMap[0].takenRooms[map.GetCurrIndicatorNode()].y)
                    {
                        map.SetIndicatorNode(i);
                        for (int j = 0; j < miniMap.Count; j++)
                        {
                            miniMap[j].pathObjectsList[map.GetCurrIndicatorNode()].GetComponent<RoomController>().ToggleRoomCover(true);
                        }
                        break;
                    }
                }
            }
            for (int j = 0; j < miniMap.Count; j++)
            {
                miniMap[j].mapIndicator.transform.localPosition = miniMap[j].takenRooms[map.GetCurrIndicatorNode()];
            }
        }
    }

    private void HandleMinimap()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            for (int i = 0; i < miniMap.Count; i++)
            {
                miniMap[i].isShowMap = !miniMap[i].isShowMap;
                miniMap[i].mapContainer.SetActive(miniMap[i].isShowMap);
            }
        }
    }
}
