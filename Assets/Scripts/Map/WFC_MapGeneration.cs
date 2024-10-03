using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WFC_MapGeneration : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] private Vector2 mapSize;
    [SerializeField] private float tileSize;
    [SerializeField] private int borderThickness;
    [SerializeField] private int mapSeed;
    [SerializeField] private ChestSpawnChance chestSpawn;
    [Header("Main Map Generation")]
    [SerializeField] private MapTileController tileController;
    [SerializeField] private List<GameObject> startingTilePrefabs;
    [SerializeField] private List<GameObject> allTilePrefabs;
    [Header("Border Generation")]
    [Header("Edges")]
    [SerializeField] private List<GameObject> topBorderTiles;
    [SerializeField] private List<GameObject> bottomBorderTiles;
    [SerializeField] private List<GameObject> leftBorderTiles;
    [SerializeField] private List<GameObject> rightBorderTiles;
    [Header("Corners")]
    [SerializeField] private List<GameObject> topLeftCornerTiles;
    [SerializeField] private List<GameObject> topRightCornerTiles;
    [SerializeField] private List<GameObject> bottomLeftCornerTiles;
    [SerializeField] private List<GameObject> bottomRightCornerTiles;
    [Header("Perimeter")]
    [SerializeField] private List<GameObject> solidTile;
    [Header("Other")]
    [SerializeField] private GameObject doorPrefab;
    [SerializeField] private GameObject portalPrefab;
    [SerializeField] private List<Chest> chestsInMap;
    [SerializeField] private TilemapManager tilemapManager;
    [SerializeField] private ItemStats itemStats;
    [SerializeField] private ParallaxEffect[] bgs;
    [SerializeField] private Transform cam;
    [SerializeField] private FogOfWar fow;

    private List<MapTile> mapTiles = new List<MapTile>();
    private List<Sprite> tileSprites = new();
    private Vector2 currTile;
    private Vector2 startingPos;
    private List<Vector2> collapsableTiles = new List<Vector2>();
    private List<Vector2> collapsedTiles = new List<Vector2>();
    private List<int> collapsableTileNum = new List<int>();

    public List<Portal> portalsInMap;

    public void InitMapGenerator()
    {
        tileSprites = tilemapManager.GetAllTileSprites();

        SetSeed();
        tileController.InitMapTiles();
        GenerateMap();

        foreach (ParallaxEffect parallaxEffect in bgs)
            parallaxEffect.InitParallaxEffect((mapSize.y + 1) * tileSize);
        SetAStarNavMesh();
    }

    public void SetSeed()
    {
        if (mapSeed == 0)
            mapSeed = Random.Range(0, 1000000000);

        Random.InitState(mapSeed);
    }

    public void GenerateMap()
    {
        allTilePrefabs.AddRange(tileController.allTilePrefabs);
        allTilePrefabs.AddRange(tileController.constantTilePrefabs);
        mapSize -= Vector2.one * 2;
        // init maptiles list
        for (int i = 0; i < mapSize.x * mapSize.y; i++)
        {
            mapTiles.Add(null);
        }
        // randomize starting node
        currTile = new Vector2(Random.Range(0, (int)mapSize.x), Random.Range(0, (int)mapSize.y));
        startingPos = (currTile * tileSize) - new Vector2((mapSize.x - 1) * tileSize / 2, (mapSize.y - 1) * tileSize / 2);
        // set random starting room tile
        int randomIndex = Random.Range(0, startingTilePrefabs.Count);
        mapTiles[(int)currTile.x + (int)(currTile.y * mapSize.x)] = InstantiateTile(startingTilePrefabs[randomIndex], currTile * tileSize).GetComponent<MapTile>();
        portalsInMap.Add(mapTiles[(int)currTile.x + (int)(currTile.y * mapSize.x)].doorTransform.GetComponent<Portal>());
        // add new collapsable tiles
        collapsedTiles.Add(currTile);
        AddCollapsableTiles();
        // set rooms
        SetNextTile();
        // set borders
        SetBorderTiles();
        transform.position -= new Vector3((mapSize.x - 1) * tileSize / 2, (mapSize.y - 1) * tileSize / 2, 0);
        cam.localPosition = new Vector3(tileSize, tileSize, -10);
        // get list of chests
        InitChests();
        // set door
        InitDoor();
        mapSize += Vector2.one * 2;
        if (fow != null)
            fow.Initialize();
        Debug.Log("Generation Complete!");
    }

    private void SetNextTile()
    {
        // find tile with least amount of collapse options
        Vector2 tileToCollapse = Vector2.zero;
        int leastTilesToCollapse = -1;
        for (int i = 0; i < collapsableTiles.Count; i++)
        {
            if (CheckTileInBounds(collapsableTiles[i]))
            {
                if (tileToCollapse == Vector2.zero)
                {
                    tileToCollapse = collapsableTiles[i];
                    leastTilesToCollapse = collapsableTileNum[i];
                }
                else if (collapsableTileNum[i] <= leastTilesToCollapse)
                {
                    tileToCollapse = collapsableTiles[i];
                    leastTilesToCollapse = collapsableTileNum[i];
                }
            }
        }
        if (tileToCollapse == null || leastTilesToCollapse < 0)
            return;
        // set curr tile to it
        currTile = tileToCollapse;
        // check neighbouring tiles for placeable tiles in current tile
        List<GameObject> availableTiles = GetAvailableTilesList();
        if (availableTiles.Count == 0)
            Debug.Log(currTile);
        // choose random tile
        GameObject tileToSet = availableTiles[Random.Range(0, availableTiles.Count)];
        if (tileToSet == null)
            return;
        // set room tile
        mapTiles[(int)currTile.x + (int)(currTile.y * mapSize.x)] = InstantiateTile(tileToSet, currTile * tileSize).GetComponent<MapTile>();
        // remove tile from collapsable list
        collapsableTiles.Remove(tileToCollapse);
        collapsedTiles.Add(tileToCollapse);
        collapsableTileNum.Remove(leastTilesToCollapse);
        // add new collapsable tiles
        AddCollapsableTiles();
        // set next room
        SetNextTile();
    }

    private void SetBorderTiles()
    {
        // check through all border edges
        // top
        for (int i = 0; i < mapSize.x; i++)
        {
            Vector2 tilePos = new Vector2(i, mapSize.y);
            // check tile available up with top edge border tiles
            List<GameObject> availableTiles = GetAvailableBorderTilesList(tilePos - Vector2.up, Vector2.up);
            // choose random tile
            GameObject tileToSet = availableTiles[Random.Range(0, availableTiles.Count)];
            if (tileToSet == null)
                return;
            // set room
            mapTiles.Add(tileToSet.GetComponent<MapTile>());
            // place room
            InstantiateTile(tileToSet, tilePos * tileSize);
        }
        // bottom
        for (int i = 0; i < mapSize.x; i++)
        {
            Vector2 tilePos = new Vector2(i, -1);
            // check tile available up with top edge border tiles
            List<GameObject> availableTiles = GetAvailableBorderTilesList(tilePos - Vector2.down, Vector2.down);
            // choose random tile
            GameObject tileToSet = availableTiles[Random.Range(0, availableTiles.Count)];
            if (tileToSet == null)
                return;
            // set room
            mapTiles.Add(tileToSet.GetComponent<MapTile>());
            // place room
            InstantiateTile(tileToSet, tilePos * tileSize);
        }
        // left
        for (int i = 0; i < mapSize.y; i++)
        {
            Vector2 tilePos = new Vector2(-1, i);
            // check tile available up with top edge border tiles
            List<GameObject> availableTiles = GetAvailableBorderTilesList(tilePos - Vector2.left, Vector2.left);
            // choose random tile
            GameObject tileToSet = availableTiles[Random.Range(0, availableTiles.Count)];
            if (tileToSet == null)
                return;
            // set room
            mapTiles.Add(tileToSet.GetComponent<MapTile>());
            // place room
            InstantiateTile(tileToSet, tilePos * tileSize);
        }
        // right
        for (int i = 0; i < mapSize.y; i++)
        {
            Vector2 tilePos = new Vector2(mapSize.x, i);
            // check tile available up with top edge border tiles
            List<GameObject> availableTiles = GetAvailableBorderTilesList(tilePos - Vector2.right, Vector2.right);
            // choose random tile
            GameObject tileToSet = availableTiles[Random.Range(0, availableTiles.Count)];
            if (tileToSet == null)
                return;
            // set room
            mapTiles.Add(tileToSet.GetComponent<MapTile>());
            // place room
            InstantiateTile(tileToSet, tilePos * tileSize);
        }
        // place corners
        GameObject corner;
        // top left corner
        corner = InstantiateTile(topLeftCornerTiles[Random.Range(0, topLeftCornerTiles.Count)], new Vector2(-1, mapSize.y) * tileSize);
        // set room
        mapTiles.Add(corner.GetComponent<MapTile>());

        // top right corner
        corner = InstantiateTile(topRightCornerTiles[Random.Range(0, topRightCornerTiles.Count)], new Vector2(mapSize.x, mapSize.y) * tileSize);
        // set room
        mapTiles.Add(corner.GetComponent<MapTile>());

        // bottom left corner
        corner = InstantiateTile(bottomLeftCornerTiles[Random.Range(0, bottomLeftCornerTiles.Count)], new Vector2(-1, -1) * tileSize);
        // set room
        mapTiles.Add(corner.GetComponent<MapTile>());

        // bottom right corner
        corner = InstantiateTile(bottomRightCornerTiles[Random.Range(0, bottomRightCornerTiles.Count)], new Vector2(mapSize.x, -1) * tileSize);
        // set room
        mapTiles.Add(corner.GetComponent<MapTile>());

        PlaceSolidBorderTiles();
    }

    private GameObject InstantiateTile(GameObject target, Vector3 targetPos)
    {
        MapTile newTile = Instantiate(target, transform).GetComponent<MapTile>();
        newTile.transform.localPosition = targetPos;

        for (int i = 0; i < tileSprites.Count; i++)
        {
            Tile tile = ScriptableObject.CreateInstance<Tile>();
            tile.sprite = tileSprites[i];
            newTile.InitializeTile(tilemapManager.targetTiles[i], tile);
        }

        return newTile.gameObject;
    }

    private void PlaceSolidBorderTiles()
    {
        for (int i = -borderThickness - 1; i < mapSize.x + borderThickness + 1; i++)
        {
            for (int j = -borderThickness - 1; j < mapSize.y + borderThickness + 1; j++)
            {
                if (i < -1 || i > mapSize.x || j < -1 || j > mapSize.y)
                    InstantiateTile(solidTile[Random.Range(0, solidTile.Count)], new Vector2(i, j) * tileSize);
            }
        }
    }

    private void SetAStarNavMesh()
    {
        AstarPath.active.data.gridGraph.RelocateNodes(center: ((mapSize - Vector2.one) * tileSize / 2) + new Vector2(0, 0.5f), rotation: Quaternion.Euler(-90, 0, 0), nodeSize: 1.0f);
        AstarPath.active.Scan();
    }

    private void InitChests()
    {
        // get all possible chest transforms
        List<Transform> chests = new List<Transform>();
        for (int i = 0; i < mapTiles.Count; i++)
        {
            if (mapTiles[i].chestTransform != null)
                chests.Add(mapTiles[i].chestTransform);
        }
        // finalize chest amt
        for (int i = chests.Count; i > chestSpawn.maxChests; i--)
        {
            chests.RemoveAt(Random.Range(0, chests.Count));
        }
        // place chests
        if (chests.Count == 0)
            return;

        // legendary
        int randomChestIndex = Random.Range(0, chests.Count);
        chestsInMap.Add(Instantiate(chestSpawn.chestTypes[0], chests[randomChestIndex], false).GetComponent<Chest>());
        chests.RemoveAt(randomChestIndex);

        // other chests
        while (chests.Count > 0)
        {
            int chestChance = Random.Range(0, 100) + 1;
            int count = 0;
            for (int i = 1; i < chestSpawn.chestTypes.Count; i++)
            {
                count += chestSpawn.spawnChances[i];
                if (chestChance < count)
                {
                    if (chests.Count == 0)
                        break;
                    randomChestIndex = Random.Range(0, chests.Count);
                    GameObject obj = Instantiate(chestSpawn.chestTypes[i]);
                    chestsInMap.Add(obj.GetComponent<Chest>());
                    obj.transform.SetParent(chests[randomChestIndex]);
                    obj.transform.localPosition = Vector3.zero;
                    chests.RemoveAt(randomChestIndex);
                    break;
                }
            }
        }

        //Black Card
        for (int i = 0; i < itemStats.blackCardChestAmount; i++)
            chestsInMap[i].SetCost(0);
    }

    private void InitDoor()
    {
        PlayerController.Instance.transform.position = GetStartingPos();

        List<Transform> doors = new List<Transform>();
        Transform doorTransform = null;
        // get doors
        for (int i = 0; i < mapTiles.Count; i++)
        {
            if (mapTiles[i].doorTransform != null)
                doors.Add(mapTiles[i].doorTransform);
        }
        // choose furthest door
        for (int i = 0; i < doors.Count; i++)
        {
            if (doorTransform == null || Vector2.Distance(doorTransform.position, PlayerController.Instance.transform.position) < Vector2.Distance(doors[i].position, PlayerController.Instance.transform.position))
                doorTransform = doors[i];
        }
        // set door
        Instantiate(doorPrefab, doorTransform, false);

        doors.Remove(doorTransform);
        for (int i = doors.Count - 1; i > doors.Count - 4; i--)
        {
            portalsInMap.Add(Instantiate(portalPrefab, doors[i], false).GetComponent<Portal>());
        }

        PlayerController.Instance.portalController.PositionPortals(portalsInMap);
    }

    private List<GameObject> GetAvailableBorderTilesList(Vector2 checkTilePos, Vector2 direction)
    {
        List<GameObject> availableTiles = new List<GameObject>();
        List<GameObject> tilesToRemove = new List<GameObject>();
        List<GameObject> placeableTiles;
        if (direction == Vector2.up)
        {
            placeableTiles = mapTiles[(int)checkTilePos.x + (int)(checkTilePos.y * mapSize.x)].availableTilesUp;
            // get list of tiles that are available in each list
            availableTiles.AddRange(allTilePrefabs);
            for (int i = 0; i < availableTiles.Count; i++)
            {
                if (!placeableTiles.Contains(availableTiles[i]) ||
                    !topBorderTiles.Contains(availableTiles[i]))
                {
                    tilesToRemove.Add(availableTiles[i]);
                }
            }
            for (int i = 0; i < tilesToRemove.Count; i++)
            {
                availableTiles.Remove(tilesToRemove[i]);
            }
        }
        else if (direction == Vector2.down)
        {
            placeableTiles = mapTiles[(int)checkTilePos.x + (int)(checkTilePos.y * mapSize.x)].availableTilesDown;
            // get list of tiles that are available in each list
            availableTiles.AddRange(allTilePrefabs);
            for (int i = 0; i < availableTiles.Count; i++)
            {
                if (!placeableTiles.Contains(availableTiles[i]) ||
                    !bottomBorderTiles.Contains(availableTiles[i]))
                {
                    tilesToRemove.Add(availableTiles[i]);
                }
            }
            for (int i = 0; i < tilesToRemove.Count; i++)
            {
                availableTiles.Remove(tilesToRemove[i]);
            }
        }
        else if (direction == Vector2.left)
        {
            placeableTiles = mapTiles[(int)checkTilePos.x + (int)(checkTilePos.y * mapSize.x)].availableTilesLeft;
            // get list of tiles that are available in each list
            availableTiles.AddRange(allTilePrefabs);
            for (int i = 0; i < availableTiles.Count; i++)
            {
                if (!placeableTiles.Contains(availableTiles[i]) ||
                    !leftBorderTiles.Contains(availableTiles[i]))
                {
                    tilesToRemove.Add(availableTiles[i]);
                }
            }
            for (int i = 0; i < tilesToRemove.Count; i++)
            {
                availableTiles.Remove(tilesToRemove[i]);
            }
        }
        else if (direction == Vector2.right)
        {
            placeableTiles = mapTiles[(int)checkTilePos.x + (int)(checkTilePos.y * mapSize.x)].availableTilesRight;
            // get list of tiles that are available in each list
            availableTiles.AddRange(allTilePrefabs);
            for (int i = 0; i < availableTiles.Count; i++)
            {
                if (!placeableTiles.Contains(availableTiles[i]) ||
                    !rightBorderTiles.Contains(availableTiles[i]))
                {
                    tilesToRemove.Add(availableTiles[i]);
                }
            }
            for (int i = 0; i < tilesToRemove.Count; i++)
            {
                availableTiles.Remove(tilesToRemove[i]);
            }
        }
        return availableTiles;
    }

    private List<GameObject> GetAvailableTilesList()
    {
        // get list of placeable tiles from each direction
        List<GameObject> availableTilesFromUp = GetAvailableTilesListFromNeighbour(Vector2.up);
        List<GameObject> availableTilesFromDown = GetAvailableTilesListFromNeighbour(Vector2.down);
        List<GameObject> availableTilesFromLeft = GetAvailableTilesListFromNeighbour(Vector2.left);
        List<GameObject> availableTilesFromRight = GetAvailableTilesListFromNeighbour(Vector2.right);

        List<string> NameUp = new List<string>();
        List<string> NameDown = new List<string>();
        List<string> NameLeft = new List<string>();
        List<string> NameRight = new List<string>();
        foreach (GameObject obj in availableTilesFromUp)
        {
            obj.name = obj.name.Replace("(Clone)", "").Trim();
            NameUp.Add(obj.name);
        }
        foreach (GameObject obj in availableTilesFromDown)
        {
            obj.name = obj.name.Replace("(Clone)", "").Trim();
            NameDown.Add(obj.name);
        }
        foreach (GameObject obj in availableTilesFromLeft)
        {
            obj.name = obj.name.Replace("(Clone)", "").Trim();
            NameLeft.Add(obj.name);
        }
        foreach (GameObject obj in availableTilesFromRight)
        {
            obj.name = obj.name.Replace("(Clone)", "").Trim();
            NameRight.Add(obj.name);
        }

        // get list of tiles that are available in each list
        List<GameObject> availableTiles = new List<GameObject>();
        List<GameObject> tilesToRemove = new List<GameObject>();
        availableTiles.AddRange(allTilePrefabs);
        for (int i = 0; i < availableTiles.Count; i++)
        {
            if (!NameUp.Contains(availableTiles[i].name) ||
                !NameDown.Contains(availableTiles[i].name) ||
                !NameLeft.Contains(availableTiles[i].name) ||
                !NameRight.Contains(availableTiles[i].name))
            {
                tilesToRemove.Add(availableTiles[i]);
            }
        }
        for (int i = 0; i < tilesToRemove.Count; i++)
        {
            availableTiles.Remove(tilesToRemove[i]);
        }
        return availableTiles;
    }

    private List<GameObject> GetAvailableTilesListFromNeighbour(Vector2 direction)
    {
        Vector2 checkTilePos = currTile + direction;
        // check if not out of the map and if the space has an existing tile
        if (CheckTileInBounds(checkTilePos))
        {
            if (mapTiles[(int)checkTilePos.x + (int)(checkTilePos.y * mapSize.x)] != null)
            {
                if (direction == Vector2.up)
                {
                    return mapTiles[(int)checkTilePos.x + (int)(checkTilePos.y * mapSize.x)].availableTilesDown;
                }
                else if (direction == Vector2.down)
                {
                    return mapTiles[(int)checkTilePos.x + (int)(checkTilePos.y * mapSize.x)].availableTilesUp;
                }
                else if (direction == Vector2.left)
                {
                    return mapTiles[(int)checkTilePos.x + (int)(checkTilePos.y * mapSize.x)].availableTilesRight;
                }
                else if (direction == Vector2.right)
                {
                    return mapTiles[(int)checkTilePos.x + (int)(checkTilePos.y * mapSize.x)].availableTilesLeft;
                }
            }
        }
        return allTilePrefabs;
    }

    private void AddCollapsableTiles()
    {
        // add to list of placeable tiles in each direction and start from least amount of tiles
        if (CheckTileInBounds(currTile + Vector2.up) && mapTiles[(int)currTile.x + (int)((currTile.y + 1) * mapSize.x)] == null)
        {
            if (!collapsedTiles.Contains(currTile + Vector2.up) && !collapsableTiles.Contains(currTile + Vector2.up))
            {
                collapsableTiles.Add(currTile + Vector2.up);
                collapsableTileNum.Add(GetCurrentRoomTile().availableTilesUp.Count);
            }
        }
        if (CheckTileInBounds(currTile + Vector2.down) && mapTiles[(int)currTile.x + (int)((currTile.y - 1) * mapSize.x)] == null)
        {
            if (!collapsedTiles.Contains(currTile + Vector2.down) && !collapsableTiles.Contains(currTile + Vector2.down))
            {
                collapsableTiles.Add(currTile + Vector2.down);
                collapsableTileNum.Add(GetCurrentRoomTile().availableTilesDown.Count);
            }
        }
        if (CheckTileInBounds(currTile + Vector2.left) && mapTiles[(int)(currTile.x - 1) + (int)(currTile.y * mapSize.x)] == null)
        {
            if (!collapsedTiles.Contains(currTile + Vector2.left) && !collapsableTiles.Contains(currTile + Vector2.left))
            {
                collapsableTiles.Add(currTile + Vector2.left);
                collapsableTileNum.Add(GetCurrentRoomTile().availableTilesLeft.Count);
            }
        }
        if (CheckTileInBounds(currTile + Vector2.right) && mapTiles[(int)(currTile.x + 1) + (int)(currTile.y * mapSize.x)] == null)
        {
            if (!collapsedTiles.Contains(currTile + Vector2.right) && !collapsableTiles.Contains(currTile + Vector2.right))
            {
                collapsableTiles.Add(currTile + Vector2.right);
                collapsableTileNum.Add(GetCurrentRoomTile().availableTilesRight.Count);
            }
        }
    }

    private bool CheckTileInBounds(Vector2 pos)
    {
        return pos.x >= 0 && pos.x < mapSize.x && pos.y >= 0 && pos.y < mapSize.y;
    }

    private MapTile GetCurrentRoomTile()
    {
        if (CheckTileInBounds(currTile))
            return mapTiles[(int)currTile.x + (int)(currTile.y * mapSize.x)];
        return null;
    }

    public Vector2 GetStartingPos()
    {
        return startingPos;
    }
}
