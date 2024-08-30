using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ProceduralMapGenerator : MonoBehaviour
{
    [SerializeField] private MapData mData;
    [SerializeField] private GameObject mapContainer;
    [SerializeField] public int mapSeed = 0;

    private List<Vector2> availablePosList = new List<Vector2>();
    private List<Vector3> takenPosList = new List<Vector3>();
    private List<GameObject> takenObjectsList = new List<GameObject>();
    private List<int> stepDirList = new List<int>
    {
        -1, -1, 1, 1, 0
    };
    private int currDir;
    private List<MiniMapGenerator> miniMap = new List<MiniMapGenerator>();
    private GameObject createdObj;
    private Vector2 startPos;
    private Vector2 currPos;
    private int roomsAdded = 0;
    private bool isPathDone = false;

    private void Awake()
    {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("MiniMap"))
        {
            miniMap.Add(obj.GetComponent<MiniMapGenerator>());
        }

        SetSeed(mapSeed);
        StartMapGeneration();
    }

    private void Update()
    {
        // debug inputs
        if (Input.GetKeyDown(KeyCode.R))
        {
            RandomizeSeed();
            StartMapGeneration();
        }
    }

    public void StartMapGeneration()
    {
        ResetMap();
        // set all possible positions into a list
        for (int y = 0; y < mData.mapSize.y; y++)
        {
            for (int x = 0; x < mData.mapSize.x; x++)
            {
                availablePosList.Add(new Vector2(x * mData.roomSpacing, -y * mData.roomSpacing));
            }
        }
        // pick random space in the top row
        int randX = (int)Random.Range(0, mData.mapSize.x) * mData.roomSpacing;
        startPos = new Vector2(randX, 0);
        currPos = startPos;
        takenPosList.Add(new Vector3(currPos.x, currPos.y, 1));
        availablePosList.Remove(currPos);
        // get random step
        currDir = GetRandomStepDir();
        // plot out rooms
        DoStep();
        // place rooms
        RegulateRoomAmount();
        PlaceRooms();
        ConfigureRoomDoors();
        // pass info to mini map
        for (int i = 0; i < miniMap.Count; i++)
        {
            miniMap[i].StartMapGeneration(mapSeed, ConfigureListForMiniMap(takenPosList, i), roomsAdded);
        }
    }

    private void ResetMap()
    {
        foreach (GameObject obj in takenObjectsList)
        {
            Destroy(obj);
        }
        takenObjectsList.Clear();
        takenPosList.Clear();
        availablePosList.Clear();
        isPathDone = false;
        roomsAdded = 0;
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

    private void DoStep()
    {
        // check if path is done
        if (isPathDone)
        {
            return;
        }
        // go down
        if (currDir == 0)
        {
            StepDown();
        }
        else
        {
            // check if next position is out of mapsize (hit a wall)
            int tempPosX = (int)currPos.x + (mData.roomSpacing * currDir);
            if (tempPosX < 0 || tempPosX > (mData.mapSize.x - 1) * mData.roomSpacing)
            {
                currDir = -currDir;
                StepDown();
            }
            else
            {
                currPos = new Vector2(tempPosX, currPos.y);
                takenPosList.Add(new Vector3(currPos.x, currPos.y, 1));
                availablePosList.Remove(currPos);
                // get random step
                if (GetRandomStepDir() == 0)
                {
                    StepDown();
                }
                DoStep();
            }
        }
    }

    private void StepDown()
    {
        // check if path is done
        if (isPathDone)
        {
            return;
        }
        // change type 1 room to type 2 room
        takenPosList[takenPosList.Count - 1] = new Vector3(currPos.x, currPos.y, 2);
        // check if able to go down
        currPos += new Vector2(0, -mData.roomSpacing);
        if (currPos.y < -(mData.mapSize.y - 2) * mData.roomSpacing)
        {
            isPathDone = true;
        }
        takenPosList.Add(new Vector3(currPos.x, currPos.y, 3));
        availablePosList.Remove(currPos);
        if (currDir == 0)
        {
            if (Random.Range(0,2) == 0)
            {
                currDir = -1;
            }
            else
            {
                currDir = 1;
            }
        }
        DoStep();
    }

    private void RegulateRoomAmount()
    {
        // delete rooms if too many
        if (takenPosList.Count > mData.minMaxRoomAmt.y)
        {
            for (int j = takenPosList.Count - 1; j > (int)mData.minMaxRoomAmt.y - 1; j--)
            {
                availablePosList.Add(takenPosList[j]);
                takenPosList.RemoveAt(j);
            }
        }
        // add rooms if too little
        for (int i = 0; i < availablePosList.Count; i++)
        {
            // check if not enough rooms
            if (takenPosList.Count < mData.minMaxRoomAmt.x)
            {
                if (CheckAdjacentSpaceTaken(availablePosList[i], 5))
                {
                    takenPosList.Add(new Vector3(availablePosList[i].x, availablePosList[i].y, 1));
                    availablePosList.RemoveAt(i);
                    roomsAdded++;
                }
            }
        }
    }

    private void PlaceRooms()
    {
        // randomize shop room location (1-2 spaces before end room)
        int shopIndex = Random.Range(2, 4);
        // place starting room
        CreateRoom(0, mData.startRoom);
        // place normal rooms
        for (int j = 1; j < takenPosList.Count; j++)
        {
            // place end room
            if (j == takenPosList.Count - 1 - roomsAdded)
            {
                CreateRoom(j, mData.endRoom);
            }
            // place shop room
            else if (j == takenPosList.Count - shopIndex - roomsAdded)
            {
                CreateRoom(j, mData.shopRoom);
            }
            // place other rooms
            else
            {
                CreateRoom(j, GetRandomRoomFromType((int)takenPosList[j].z));
            }
        }
    }

    private void CreateRoom(int posInList, GameObject roomObject)
    {
        // place room
        createdObj = Instantiate(roomObject);
        createdObj.transform.SetParent(mapContainer.transform);
        createdObj.transform.localPosition = new Vector3(takenPosList[posInList].x, takenPosList[posInList].y, 0);
        createdObj.transform.localScale = new Vector3(1, 1, 1);
        takenObjectsList.Add(createdObj);
    }

    private void ConfigureRoomDoors()
    {
        for (int i = 0; i < takenObjectsList.Count; i++)
        {
            RoomController rData = takenObjectsList[i].GetComponent<RoomController>();
            // check all directions for spaces or rooms
            // up
            if (CheckAdjacentSpaceTaken(takenPosList[i], 1))
            {
                rData.isSpaceOccupied[0] = true;
            }
            // down
            if (CheckAdjacentSpaceTaken(takenPosList[i], 2))
            {
                rData.isSpaceOccupied[1] = true;
            }
            // left
            if (CheckAdjacentSpaceTaken(takenPosList[i], 3))
            {
                rData.isSpaceOccupied[2] = true;
            }
            // right
            if (CheckAdjacentSpaceTaken(takenPosList[i], 4))
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
                if (takenPosList.Contains(new Vector3(pos.x, pos.y + mData.roomSpacing, 1)) ||
                    takenPosList.Contains(new Vector3(pos.x, pos.y + mData.roomSpacing, 2)) ||
                    takenPosList.Contains(new Vector3(pos.x, pos.y + mData.roomSpacing, 3)))
                {
                    return true;
                }
                break;
            case 2:
                // down
                if (takenPosList.Contains(new Vector3(pos.x, pos.y - mData.roomSpacing, 1)) ||
                    takenPosList.Contains(new Vector3(pos.x, pos.y - mData.roomSpacing, 2)) ||
                    takenPosList.Contains(new Vector3(pos.x, pos.y - mData.roomSpacing, 3)))
                {
                    return true;
                }
                break;
            case 3:
                // left
                if (takenPosList.Contains(new Vector3(pos.x - mData.roomSpacing, pos.y, 1)) ||
                    takenPosList.Contains(new Vector3(pos.x - mData.roomSpacing, pos.y, 2)) ||
                    takenPosList.Contains(new Vector3(pos.x - mData.roomSpacing, pos.y, 3)))
                {
                    return true;
                }
                break;
            case 4:
                // right
                if (takenPosList.Contains(new Vector3(pos.x + mData.roomSpacing, pos.y, 1)) ||
                    takenPosList.Contains(new Vector3(pos.x + mData.roomSpacing, pos.y, 2)) ||
                    takenPosList.Contains(new Vector3(pos.x + mData.roomSpacing, pos.y, 3)))
                {
                    return true;
                }
                break;
            case 5:
                // up
                if (takenPosList.Contains(new Vector3(pos.x, pos.y + mData.roomSpacing, 1)) ||
                    takenPosList.Contains(new Vector3(pos.x, pos.y + mData.roomSpacing, 2)) ||
                    takenPosList.Contains(new Vector3(pos.x, pos.y + mData.roomSpacing, 3)))
                {
                    return true;
                }
                // down
                if (takenPosList.Contains(new Vector3(pos.x, pos.y - mData.roomSpacing, 1)) ||
                    takenPosList.Contains(new Vector3(pos.x, pos.y - mData.roomSpacing, 2)) ||
                    takenPosList.Contains(new Vector3(pos.x, pos.y - mData.roomSpacing, 3)))
                {
                    return true;
                }
                // left
                if (takenPosList.Contains(new Vector3(pos.x - mData.roomSpacing, pos.y, 1)) ||
                    takenPosList.Contains(new Vector3(pos.x - mData.roomSpacing, pos.y, 2)) ||
                    takenPosList.Contains(new Vector3(pos.x - mData.roomSpacing, pos.y, 3)))
                {
                    return true;
                }
                // right
                if (takenPosList.Contains(new Vector3(pos.x + mData.roomSpacing, pos.y, 1)) ||
                    takenPosList.Contains(new Vector3(pos.x + mData.roomSpacing, pos.y, 2)) ||
                    takenPosList.Contains(new Vector3(pos.x + mData.roomSpacing, pos.y, 3)))
                {
                    return true;
                }
                break;
        }
        return false;
    }

    private List<Vector2> ConfigureListForMiniMap(List<Vector3> listToBeConfigured, int miniMapNum)
    {
        List<Vector2> newList = new List<Vector2>();

        for (int i = 0; i < listToBeConfigured.Count; i++)
        {
            newList.Add(new Vector2(listToBeConfigured[i].x * miniMap[miniMapNum].mData.roomSpacing / mData.roomSpacing, listToBeConfigured[i].y * miniMap[miniMapNum].mData.roomSpacing / mData.roomSpacing));
        }

        return newList;
    }

    private GameObject GetRandomRoomFromType(int type)
    {
        switch (type)
        {
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
    
    private int GetRandomStepDir()
    {
        return stepDirList[Random.Range(0, stepDirList.Count)];
    }
}
