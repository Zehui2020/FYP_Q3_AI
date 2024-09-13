using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    [SerializeField] private List<GameObject> doors;
    [SerializeField] private GameObject cover;

    public List<bool> isSpaceOccupied = new List<bool> { false, false, false, false };

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
}
