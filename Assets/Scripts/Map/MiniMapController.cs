using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapController : MonoBehaviour
{
    private List<MiniMapGenerator> miniMap = new List<MiniMapGenerator>();
    [SerializeField] private int currIndicatorNode = 0;

    private void Start()
    {
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
        HandleMinimap();
    }

    public void MoveNode(int dir)
    {
        if (dir == 1)
        {
            for (int i = 0; i < miniMap[0].takenRooms.Count; i++)
            {
                if (CheckAdjacentNode(miniMap[0].takenRooms[i], miniMap[0].takenRooms[currIndicatorNode], new Vector2(0, miniMap[0].mData.roomSpacing)))
                {
                    currIndicatorNode = i;
                    ShowMiniMapTile();
                    break;
                }
            }
        }
        else if (dir == 2)
        {
            for (int i = 0; i < miniMap[0].takenRooms.Count; i++)
            {
                if (CheckAdjacentNode(miniMap[0].takenRooms[i], miniMap[0].takenRooms[currIndicatorNode], new Vector2(0, -miniMap[0].mData.roomSpacing)))
                {
                    currIndicatorNode = i;
                    ShowMiniMapTile();
                    break;
                }
            }
        }
        else if (dir == 3)
        {
            for (int i = 0; i < miniMap[0].takenRooms.Count; i++)
            {
                if (CheckAdjacentNode(miniMap[0].takenRooms[i], miniMap[0].takenRooms[currIndicatorNode], new Vector2(-miniMap[0].mData.roomSpacing, 0)))
                {
                    currIndicatorNode = i;
                    ShowMiniMapTile();
                    break;
                }
            }
        }
        else if (dir == 4)
        {
            for (int i = 0; i < miniMap[0].takenRooms.Count; i++)
            {
                if (CheckAdjacentNode(miniMap[0].takenRooms[i], miniMap[0].takenRooms[currIndicatorNode], new Vector2(miniMap[0].mData.roomSpacing, 0)))
                {
                    currIndicatorNode = i;
                    ShowMiniMapTile();
                    break;
                }
            }
        }
        for (int j = 0; j < miniMap.Count; j++)
        {
            miniMap[j].mapIndicator.transform.localPosition = miniMap[j].takenRooms[currIndicatorNode];
        }
    }

    private bool CheckAdjacentNode(Vector2 pos1, Vector2 pos2, Vector2 offset)
    {
        return (pos1 == pos2 + offset);
    }

    private void ShowMiniMapTile()
    {
        for (int j = 0; j < miniMap.Count; j++)
        {
            miniMap[j].pathObjectsList[currIndicatorNode].GetComponent<RoomController>().ToggleRoomCover(true);
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

    public Vector2 GetStartPos()
    {
        return miniMap[0].takenRooms[currIndicatorNode];
    }

    public int GetCurrIndicatorNode()
    {
        return currIndicatorNode;
    }

    public void SetIndicatorNode(int node)
    {
        currIndicatorNode = node;
    }

    public void ResetIndicatorNode()
    {
        currIndicatorNode = 0;
        ShowMiniMapTile();
        for (int j = 0; j < miniMap.Count; j++)
        {
            miniMap[j].mapIndicator.transform.localPosition = miniMap[j].takenRooms[currIndicatorNode];
        }
    }
}
