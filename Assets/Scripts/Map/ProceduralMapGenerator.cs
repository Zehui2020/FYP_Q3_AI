using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralMapGenerator : MonoBehaviour
{
    [SerializeField] private MapData mData;
    [SerializeField] private GameObject mapContainer;
    [SerializeField] public int mapSeed = 0;

    private List<Vector2> availablePosList = new List<Vector2>();
    private List<Vector3> takenPosList = new List<Vector3>();
    private List<GameObject> availableObjectsList = new List<GameObject>();
    private List<GameObject> takenObjectsList = new List<GameObject>();
    private List<int> stepDirList = new List<int>
    {
        -1, -1, 1, 1, 0
    };
    private int currDir;
    private MiniMapGenerator miniMap;
    private GameObject createdObj;
    private Vector2 startPos;
    private Vector2 currPos;
    private int currIndicatorNode;
    private bool isPathDone = false;

    private void Start()
    {
        miniMap = GameObject.FindGameObjectWithTag("MiniMap").GetComponent<MiniMapGenerator>();

        if (!CompareTag("MiniMap"))
        {
            SetSeed(mapSeed);
            StartMapGeneration();
            miniMap.StartMapGeneration(mapSeed, availablePosList, takenPosList);
        }
    }

    private void Update()
    {
        // debug inputs
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!CompareTag("MiniMap"))
            {
                RandomizeSeed();
                StartMapGeneration();
                miniMap.StartMapGeneration(mapSeed, availablePosList, takenPosList);
            }
        }

        HandleMinimapIndicator();
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
        PlaceRooms();
        ConfigureRoomDoors();
    }

    private void ResetMap()
    {
        foreach (GameObject obj in takenObjectsList)
        {
            Destroy(obj);
        }
        foreach (GameObject obj in availableObjectsList)
        {
            Destroy(obj);
        }
        takenObjectsList.Clear();
        availableObjectsList.Clear();
        takenPosList.Clear();
        availablePosList.Clear();
        isPathDone = false;
    }

    private void RandomizeSeed()
    {
        mapSeed = Random.Range(0, 1000000000);
        Random.seed = mapSeed;
    }

    public void SetSeed(int seed)
    {
        mapSeed = seed;
        Random.seed = mapSeed;
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

    private void PlaceRooms()
    {
        // delete rooms if too many
        for (int j = 0; j < takenPosList.Count; j++)
        {
            if (j > mData.minMaxRoomAmt.y)
            {
                // place room
                availablePosList.Add(takenPosList[j]);
                takenPosList.RemoveAt(j);
            }
        }
        // add rooms if too little
        for (int i = 0; i < availablePosList.Count; i++)
        {
            if (takenPosList.Count < mData.minMaxRoomAmt.x)
            {
                // place room
                takenPosList.Add(new Vector3(availablePosList[i].x, availablePosList[i].y, 1));
                availablePosList.RemoveAt(i);
            }
        }
        // place normal rooms
        for (int j = 0; j < takenPosList.Count; j++)
        {
            // place room
            createdObj = Instantiate(GetRandomRoomFromType((int)takenPosList[j].z));
            createdObj.transform.SetParent(mapContainer.transform);
            createdObj.transform.localPosition = takenPosList[j];
            takenObjectsList.Add(createdObj);
        }
        // place remaining type 0 rooms
        for (int i = 0; i < availablePosList.Count; i++)
        {
            // place room
            createdObj = Instantiate(GetRandomRoomFromType(0));
            createdObj.transform.SetParent(mapContainer.transform);
            createdObj.transform.localPosition = availablePosList[i];
            availableObjectsList.Add(createdObj);
        }
        // position indicator
        currIndicatorNode = 0;
    }

    private void ConfigureRoomDoors()
    {
        for (int i = 0; i < takenObjectsList.Count; i++)
        {
            RoomController rData = takenObjectsList[i].GetComponent<RoomController>();
            // check all directions for spaces or rooms
            // up
            if (takenPosList.Contains(new Vector3(takenPosList[i].x, takenPosList[i].y + mData.roomSpacing, 1)) ||
                takenPosList.Contains(new Vector3(takenPosList[i].x, takenPosList[i].y + mData.roomSpacing, 2)) ||
                takenPosList.Contains(new Vector3(takenPosList[i].x, takenPosList[i].y + mData.roomSpacing, 3)))
            {
                rData.isSpaceOccupied[0] = true;
            }
            // down
            if (takenPosList.Contains(new Vector3(takenPosList[i].x, takenPosList[i].y - mData.roomSpacing, 1)) ||
                takenPosList.Contains(new Vector3(takenPosList[i].x, takenPosList[i].y - mData.roomSpacing, 2)) ||
                takenPosList.Contains(new Vector3(takenPosList[i].x, takenPosList[i].y - mData.roomSpacing, 3)))
            {
                rData.isSpaceOccupied[1] = true;
            }
            // left
            if (takenPosList.Contains(new Vector3(takenPosList[i].x - mData.roomSpacing, takenPosList[i].y, 1)) ||
                takenPosList.Contains(new Vector3(takenPosList[i].x - mData.roomSpacing, takenPosList[i].y, 2)) ||
                takenPosList.Contains(new Vector3(takenPosList[i].x - mData.roomSpacing, takenPosList[i].y, 3)))
            {
                rData.isSpaceOccupied[2] = true;
            }
            // right
            if (takenPosList.Contains(new Vector3(takenPosList[i].x + mData.roomSpacing, takenPosList[i].y, 1)) ||
                takenPosList.Contains(new Vector3(takenPosList[i].x + mData.roomSpacing, takenPosList[i].y, 2)) ||
                takenPosList.Contains(new Vector3(takenPosList[i].x + mData.roomSpacing, takenPosList[i].y, 3)))
            {
                rData.isSpaceOccupied[3] = true;
            }
            // update doors
            rData.UpdateDoors();
            rData.ToggleRoomCover(false);
        }
    }

    private void HandleMinimapIndicator()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            RoomController rData = takenObjectsList[currIndicatorNode].GetComponent<RoomController>();
            if (rData.isSpaceOccupied[0])
            {
                for (int i = 0; i < takenPosList.Count; i++)
                {
                    if (takenPosList[i].x == takenPosList[currIndicatorNode].x &&
                        takenPosList[i].y == takenPosList[currIndicatorNode].y + mData.roomSpacing)
                    {
                        currIndicatorNode = i;
                        break;
                    }
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            RoomController rData = takenObjectsList[currIndicatorNode].GetComponent<RoomController>();
            if (rData.isSpaceOccupied[1])
            {
                for (int i = 0; i < takenPosList.Count; i++)
                {
                    if (takenPosList[i].x == takenPosList[currIndicatorNode].x &&
                        takenPosList[i].y == takenPosList[currIndicatorNode].y - mData.roomSpacing)
                    {
                        currIndicatorNode = i;
                        break;
                    }
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            RoomController rData = takenObjectsList[currIndicatorNode].GetComponent<RoomController>();
            if (rData.isSpaceOccupied[2])
            {
                for (int i = 0; i < takenPosList.Count; i++)
                {
                    if (takenPosList[i].x == takenPosList[currIndicatorNode].x - mData.roomSpacing &&
                        takenPosList[i].y == takenPosList[currIndicatorNode].y)
                    {
                        currIndicatorNode = i;
                        break;
                    }
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            RoomController rData = takenObjectsList[currIndicatorNode].GetComponent<RoomController>();
            if (rData.isSpaceOccupied[3])
            {
                for (int i = 0; i < takenPosList.Count; i++)
                {
                    if (takenPosList[i].x == takenPosList[currIndicatorNode].x + mData.roomSpacing &&
                        takenPosList[i].y == takenPosList[currIndicatorNode].y)
                    {
                        currIndicatorNode = i;
                        break;
                    }
                }
            }
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
    
    private int GetRandomStepDir()
    {
        return stepDirList[Random.Range(0, stepDirList.Count)];
    }
}
