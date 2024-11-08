using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapManager : MonoBehaviour
{
    [Header("Tile Slicing")]
    [SerializeField] private ImageSaver imageSaver;
    [SerializeField] private string fileName;
    [SerializeField] private List<Rect> tileRects;
    private List<Sprite> slicedSprites = new();

    [Header("Tilemap Manipulation")]
    [SerializeField] private List<Tilemap> tilemaps;
    public List<TileBase> targetTiles;
    private List<Tile> replacedTiles = new();

    [Header("Initing")]
    [SerializeField] private string startLevel = string.Empty;

    private void Start()
    {
        if (string.IsNullOrEmpty(startLevel))
            SliceTexture();
        else 
            SliceTexture(startLevel);
    }

    public void SliceTexture()
    {
        if (targetTiles.Count != tileRects.Count)
        {
            Debug.LogError("Insufficient Tiles To Set!");
            return;
        }

        Texture2D texture = imageSaver.GetTextureFromLocalDisk(fileName + "_" + GameData.Instance.currentLevel);

        foreach (Rect rect in tileRects)
        {
            Sprite newSprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f), 302);
            slicedSprites.Add(newSprite);
        }

        AssignTileSprites();
    }
    public void SliceTexture(string levelName)
    {
        if (targetTiles.Count != tileRects.Count)
        {
            Debug.LogError("Insufficient Tiles To Set!");
            return;
        }

        Texture2D texture = imageSaver.GetTextureFromLocalDisk(fileName + "_" + levelName);
        foreach (Rect rect in tileRects)
        {
            Sprite newSprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f), 302);
            slicedSprites.Add(newSprite);
        }

        AssignTileSprites();
    }

    public List<Sprite> GetAllTileSprites()
    {
        List<Sprite> sprites = new();

        Texture2D texture = imageSaver.GetTextureFromLocalDisk(fileName + "_" + GameData.Instance.currentLevel);

        foreach (Rect rect in tileRects)
        {
            Sprite newSprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f), 302);
            sprites.Add(newSprite);
        }

        return sprites;
    }

    public void AssignTileSprites()
    {
        List<Tile> tilesToReplace = new();

        for (int i = 0; i < slicedSprites.Count; i++)
        {
            Tile tile = ScriptableObject.CreateInstance<Tile>();
            tile.sprite = slicedSprites[i];
            tilesToReplace.Add(tile);

            foreach (Tilemap tilemap in tilemaps)
            {
                if (replacedTiles.Count == 0)
                    tilemap.SwapTile(targetTiles[i], tile);
                else
                    tilemap.SwapTile(replacedTiles[i], tile);
            }
        }

        replacedTiles.Clear();
        replacedTiles.AddRange(tilesToReplace);
        slicedSprites.Clear();
    }
}