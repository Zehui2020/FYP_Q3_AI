using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    [SerializeField] private GameObject cover;
    [SerializeField] private List<GameObject> doors;
    [SerializeField] private List<Transform> tpPoints;
    [SerializeField] private Transform spawnPoint;

    public List<bool> isSpaceOccupied = new List<bool> { false, false, false, false };
    public RoomTransitionManager roomTransitionManager;

    private void Start()
    {
        UpdateDoors();
    }

    public void UpdateDoors()
    {
        for (int i = 0; i < doors.Count; i++)
        {
            doors[i].SetActive(!isSpaceOccupied[i]);
        }
    }

    public void ToggleRoomCover(bool covered)
    {
        cover.SetActive(covered);
    }

    public void OnTriggerTransition(int dir)
    {
        // notify manager to tp player
        roomTransitionManager.TeleportPlayer(dir);
    }

    public Transform GetTPPoint(int dir)
    {
        // invert dir to find tp point
        switch (dir)
        {
            case 1:
                return tpPoints[1];
            case 2:
                return tpPoints[0];
            case 3:
                return tpPoints[3];
            case 4:
                return tpPoints[2];
        }
        return null;
    }

    public Transform GetSpawnPoint()
    {
        return spawnPoint;
    }
}
