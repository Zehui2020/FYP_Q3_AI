using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;

public class ProceduralMapGenerator : MonoBehaviour
{
    [SerializeField] private MapData mData;
    [SerializeField] private GameObject mapContainer;
    [SerializeField] private int mapSeed = 0;

    private GameObject createdObj;
    private Vector2 startPos;
    private Vector2 currPos;
    private List<Vector2> availableRoomPosList = new List<Vector2>();
    private List<GameObject> mainpathList = new List<GameObject>();
    private List<GameObject> fillSpaceList = new List<GameObject>();
    private List<int> stepDirList = new List<int>
    {
        -1, -1, 1, 1, 0
    };
    private int currDir;
    private bool isPathDone = false;

    private void Start()
    {
        Random.seed = mapSeed;
        StartMapGeneration();
    }

    private void Update()
    {
        // debug inputs
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ResetMap();
            RandomizeSeed();
            StartMapGeneration();
        }
    }

    private void StartMapGeneration()
    {
        // set all possible positions into a list
        for (int y = 0; y < mData.mapSize.y; y++)
        {
            for (int x = 0; x < mData.mapSize.x; x++)
            {
                availableRoomPosList.Add(new Vector2(x * mData.roomSpacing, -y * mData.roomSpacing));
            }
        }
        // pick random space in the top row
        int randX = (int)Random.Range(0, mData.mapSize.x) * mData.roomSpacing;
        startPos = new Vector2(randX, 0);
        currPos = startPos;
        // place room
        createdObj = Instantiate(GetRandomRoomFromType(1));
        createdObj.transform.SetParent(mapContainer.transform);
        createdObj.transform.position = currPos;
        mainpathList.Add(createdObj);
        availableRoomPosList.Remove(currPos);
        // get random step
        currDir = GetRandomStepDir();
        DoStep();
        //fill spaces
        FillRemainingSpaces();
    }

    private void ResetMap()
    {
        foreach (GameObject obj in mainpathList)
        {
            Destroy(obj);
        }
        foreach (GameObject obj in fillSpaceList)
        {
            Destroy(obj);
        }
        mainpathList.Clear();
        fillSpaceList.Clear();
        availableRoomPosList.Clear();
        isPathDone = false;
    }

    private void RandomizeSeed()
    {
        mapSeed = Random.Range(0, 1000000000);
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
                // place room
                createdObj = Instantiate(GetRandomRoomFromType(1));
                createdObj.transform.SetParent(mapContainer.transform);
                currPos = new Vector2(tempPosX, currPos.y);
                createdObj.transform.position = currPos;
                mainpathList.Add(createdObj);
                availableRoomPosList.Remove(currPos);
                // get random step
                if (Random.Range(0, 3) == 0)
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
        Destroy(createdObj);
        createdObj = Instantiate(GetRandomRoomFromType(2));
        createdObj.transform.SetParent(mapContainer.transform);
        createdObj.transform.position = currPos;
        mainpathList.Add(createdObj);
        // place room
        createdObj = Instantiate(GetRandomRoomFromType(1));
        createdObj.transform.SetParent(mapContainer.transform);
        currPos += new Vector2(0, -mData.roomSpacing);
        if (currPos.y < -(mData.mapSize.y - 2) * mData.roomSpacing)
        {
            isPathDone = true;
        }
        createdObj.transform.position = currPos;
        mainpathList.Add(createdObj);
        availableRoomPosList.Remove(currPos);
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

    private void FillRemainingSpaces()
    {
        for (int i = 0; i < availableRoomPosList.Count; i++)
        {
            // place room
            createdObj = Instantiate(GetRandomRoomFromType(0));
            createdObj.transform.SetParent(mapContainer.transform);
            createdObj.transform.position = availableRoomPosList[i];
            fillSpaceList.Add(createdObj);
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
