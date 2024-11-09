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
    [SerializeField] private List<string> levelNames = new();

    public struct LevelSprites
    {
        public string level;
        public List<Sprite> tileSprites;

        public LevelSprites(string level, List<Sprite> tileSprites)
        {
            this.level = level;
            this.tileSprites = tileSprites;
        }
    }
    public List<LevelSprites> levelSprites = new();

    private void Start()
    {
        if (string.IsNullOrEmpty(startLevel))
            SliceTexture();
        else 
            SliceTexture(startLevel);

        foreach (LevelManager.LevelData levelData in LevelManager.Instance.levelDatas)
            levelNames.Add(levelData.level);

        foreach (string level in levelNames)
            levelSprites.Add(new LevelSprites(level, GetTileSpritesFromLevel(level)));
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

        AssignTileSprites(slicedSprites);
    }
    public void SliceTexture(string levelName)
    {
        foreach (LevelSprites levelSprite in levelSprites)
        {
            if (levelSprite.level == levelName)
                AssignTileSprites(levelSprite.tileSprites);
        }
    }

    private List<Sprite> GetTileSpritesFromLevel(string levelName)
    {
        List<Sprite> result = new List<Sprite>();

        if (targetTiles.Count != tileRects.Count)
        {
            Debug.LogError("Insufficient Tiles To Set!");
            return null;
        }

        Texture2D texture = imageSaver.GetTextureFromLocalDisk(fileName + "_" + levelName);
        foreach (Rect rect in tileRects)
        {
            Sprite newSprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f), 302, 0, SpriteMeshType.FullRect);
            result.Add(newSprite);
        }

        return result;
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

    public void AssignTileSprites(List<Sprite> sprites)
    {
        List<Tile> tilesToReplace = new();

        for (int i = 0; i < sprites.Count; i++)
        {
            Tile tile = ScriptableObject.CreateInstance<Tile>();
            tile.sprite = sprites[i];
            tilesToReplace.Add(tile);

            foreach (Tilemap tilemap in tilemaps)
            {
                tilemap.enabled = false;

                if (replacedTiles.Count == 0)
                    tilemap.SwapTile(targetTiles[i], tile);
                else
                    tilemap.SwapTile(replacedTiles[i], tile);

                tilemap.enabled = true;
            }
        }

        replacedTiles.Clear();
        replacedTiles.AddRange(tilesToReplace);
        slicedSprites.Clear();
    }
}