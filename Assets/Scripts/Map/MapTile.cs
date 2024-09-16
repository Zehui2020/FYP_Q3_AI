using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTile : MonoBehaviour
{
    [SerializeField] public List<GameObject> availableTilesUp;
    [SerializeField] public List<GameObject> availableTilesDown;
    [SerializeField] public List<GameObject> availableTilesLeft;
    [SerializeField] public List<GameObject> availableTilesRight;
}