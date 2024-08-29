using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTransitionManager : MonoBehaviour
{
    private MiniMapController miniMap;
    private GameObject player;
    private List<RoomController> rooms = new List<RoomController>();

    private void Start()
    {
        miniMap = GameObject.FindGameObjectWithTag("MiniMapController").GetComponent<MiniMapController>();
        player = GameObject.FindGameObjectWithTag("Player");
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Room"))
        {
            rooms.Add(obj.GetComponent<RoomController>());
            obj.GetComponent<RoomController>().roomTransitionManager = this;
        }
    }

    public void TeleportPlayer(int dir)
    {
        // move indicator on mini map
        miniMap.MoveNode(dir);
        // find new room to tp to
        RoomController roomToTP = rooms[miniMap.map.GetCurrIndicatorNode()];
        // teleport player to room
        player.transform.position = roomToTP.GetTPPoint(dir).position;
    }
}
