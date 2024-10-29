using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilesetLoader : MonoBehaviour
{
    [SerializeField] private ImageSaver imageSaver;
    [SerializeField] private List<Tile> tiles = new();
    [SerializeField] private List<string> tileNames = new();

    private void Start()
    {
        for (int i = 0; i < tiles.Count; i++)
            tiles[i].sprite = imageSaver.GetSpriteFromLocalDisk(tileNames[i]);
    }
}