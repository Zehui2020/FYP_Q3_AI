using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    [SerializeField] private GameObject cover;
    [SerializeField] private List<GameObject> doors;
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
            doors[i].SetActive(isSpaceOccupied[i]);
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
                return doors[1].transform;
            case 2:
                return doors[0].transform;
            case 3:
                return doors[3].transform;
            case 4:
                return doors[2].transform;
        }
        return null;
    }

    public Transform GetSpawnPoint()
    {
        return spawnPoint;
    }
}
