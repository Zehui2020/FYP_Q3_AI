using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChunkLoader : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private List<GameObject> chunks = new();
    [SerializeField] private GameObject chest;
    [SerializeField] private GameObject door;
    [SerializeField] private GameObject portal;
    [SerializeField] private int chunkSize;
    [SerializeField] private int index = 0;
    private GameObject obj1, obj2;
    private MapTile tile;

    private void Start()
    {
        LoadChunk(index);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            index--;
            LoadChunk(index);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            index++;
            LoadChunk(index);
        }
    }

    private void LoadChunk(int i)
    {
        Destroy(obj1);
        Destroy(obj2);

        obj1 = Instantiate(chunks[i]);
        obj1.transform.position = Vector3.zero;
        obj1.name = obj1.name.Replace("(Clone)", "").Trim();
        tile = obj1.GetComponent<MapTile>();
        tile.InitializeTile();
        if (tile.doorTransform != null && !tile.doorTransform.name.Contains("Portal"))
            Instantiate(door, tile.doorTransform, false);
        if (tile.chestTransform != null)
            Instantiate(chest, tile.chestTransform, false);

        obj2 = Instantiate(chunks[i]);
        obj2.transform.position = new Vector3(chunkSize, 0, 0);
        obj2.name = obj2.name.Replace("(Clone)", "").Trim();
        tile = obj2.GetComponent<MapTile>();
        tile.InitializeTile();
        if (tile.doorTransform != null && !tile.doorTransform.name.Contains("Portal"))
            Instantiate(portal, tile.doorTransform, false);
        if (tile.chestTransform != null)
            Instantiate(chest, tile.chestTransform, false);

        text.text = obj1.name + "\n[" + index + "]";
        Debug.Log(index);
    }
}
