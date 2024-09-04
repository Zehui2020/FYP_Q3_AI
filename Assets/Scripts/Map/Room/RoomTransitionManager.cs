using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTransitionManager : MonoBehaviour
{
    [SerializeField] private MiniMapController mmController;
    [SerializeField] private GameObject player;
    private List<RoomController> rooms = new List<RoomController>();

    public void TeleportPlayer(int dir)
    {
        StartCoroutine(TeleportRoutine(dir));
    }

    private IEnumerator TeleportRoutine(int dir)
    {
        // move indicator on mini map
        mmController.MoveNode(dir);
        // find new room to tp to
        RoomController roomToTP = rooms[mmController.GetCurrIndicatorNode()];
        PlayerController.Instance.FadeOut();

        yield return new WaitForSeconds(0.3f);

        // teleport player to room
        player.transform.position = roomToTP.GetTPPoint(dir).position;
        PlayerController.Instance.FadeIn();
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
        Vector3 targetPos = rooms[0].GetSpawnPoint().position;
        player.transform.position = new Vector3(targetPos.x, targetPos.y, 0);
    }
}