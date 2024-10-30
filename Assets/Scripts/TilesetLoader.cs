using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static MapData;

public class TilesetLoader : MonoBehaviour
{
    [SerializeField] private ImageSaver imageSaver;
    [SerializeField] private Tilemap tilemap;

    public void SetEnvironmentProps(List<PropTileData> tiles)
    {
        Tilemap[] tilemaps = FindObjectsOfType<Tilemap>();

        for (int i = 0; i < tiles.Count; i++)
        {
            Tile tile = ScriptableObject.CreateInstance<Tile>();
            tile.sprite = imageSaver.GetTileSprite(tiles[i].tileName, tiles[i].tileSize);

            foreach (Tilemap tilemap in tilemaps)
                tilemap.SwapTile(tiles[i].tile, tile);
        }
    }
}