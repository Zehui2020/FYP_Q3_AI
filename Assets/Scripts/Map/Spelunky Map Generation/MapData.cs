using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class MapData : ScriptableObject
{
    public GameObject startRoom;
    public GameObject endRoom;
    public GameObject shopRoom;
    public List<GameObject> enemyRoom;
    public List<GameObject> puzzleRoom;
    public List<GameObject> eliteRoom;
    public List<Vector2> minMaxRoomTypeAmt;
    public int roomSpacing;
    public Vector2 mapSize;
    public Vector2 minMaxRoomAmt;
}
