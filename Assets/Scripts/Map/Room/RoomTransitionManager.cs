using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RoomTransitionManager : MonoBehaviour
{
    private MiniMapController mmController;
    private GameObject player;
    public List<RoomController> rooms = new List<RoomController>();

    private void Awake()
    {
        mmController = GameObject.FindGameObjectWithTag("MiniMapController").GetComponent<MiniMapController>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void TeleportPlayer(int dir)
    {
        // move indicator on mini map
        mmController.MoveNode(dir);
        // find new room to tp to
        RoomController roomToTP = rooms[mmController.GetCurrIndicatorNode()];
        // teleport player to room
        player.transform.position = roomToTP.GetTPPoint(dir).position;
    }

    public void ResetRooms(List<GameObject> list)
    {
        rooms.Clear();
        for (int i = 0; i < list.Count; i++)
        {
            rooms.Add(list[i].GetComponent<RoomController>());
            list[i].GetComponent<RoomController>().roomTransitionManager = this;
        }
        ResetPlayer();
    }

    public void ResetPlayer()
    {
        player.transform.position = rooms[0].GetSpawnPoint().position;
    }
}
