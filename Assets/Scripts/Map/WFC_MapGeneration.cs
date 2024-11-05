using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WFC_MapGeneration : MonoBehaviour
{
    [Header("Map Settings")]
    [SerializeField] private MapData mData;
    [Header("Map Object Prefabs")]
    [SerializeField] private GameObject fogPrefab;
    [SerializeField] private GameObject doorPrefab;
    [SerializeField] private GameObject portalPrefab;
    [Header("Background & Tilemap")]
    [SerializeField] private TilemapManager tilemapManager;
    [SerializeField] private ParallaxEffect[] bgs;
    [Header("Items")]
    [SerializeField] private ItemStats itemStats;
    [Header("Expanded Map Camera")]
    [SerializeField] private Camera cam;
    [SerializeField] private TilesetLoader tilesetLoader;
    [Header("Debug Settings")]
    [SerializeField] private bool noFog;

    // tile settings
    private ChestSpawnChance chestSpawn;
    private MapTileController mapTileController;
    private Vector2 mapSize;
    private float mapTileSize;
    private int solidTileBorderThickness;
    private int mapSeed;
    // tile prefabs
    private List<GameObject> startingTilePrefabs = new();
    private List<GameObject> allTilePrefabs = new();
    private List<GameObject> borderTilePrefabs = new();
    private List<GameObject> shopTilePrefabs = new();
    private List<GameObject> topEdgeTilePrefabs = new();
    private List<GameObject> bottomEdgeTilePrefabs = new();
    private List<GameObject> leftEdgeTilePrefabs = new();
    private List<GameObject> rightEdgeTilePrefabs = new();
    private List<GameObject> TLCornerTilePrefabs = new();
    private List<GameObject> TRCornerTilePrefabs = new();
    private List<GameObject> BLCornerTilePrefabs = new();
    private List<GameObject> BRCornerTilePrefabs = new();
    private List<GameObject> solidTilePrefabs = new();
    // others
    private List<MapTile> mapTileList = new();
    private List<Vector2> collapsableTilePosList = new();
    private List<Vector2> collapsedTilePosList = new();
    private List<int> collapsableTileAmtList = new();
    private List<GameObject> mapFogList = new();
    private List<Portal> mapPortalList = new();
    private List<Chest> mapChestList = new();
    private List<Sprite> tileSprites = new();
    private Vector2 currTilePos;
    private Vector2 startingTilePos;
    private FogOfWar fogOfWar;
    private bool isShopPlaced = false;

    public void InitMapGenerator()
    {
        mapTileController = GetComponent<MapTileController>();
        fogOfWar = Camera.main.GetComponent<FogOfWar>();
        tileSprites = tilemapManager.GetAllTileSprites();

        AssignMapVariables();
        GenerateMap();

        foreach (ParallaxEffect parallaxEffect in bgs)
            parallaxEffect.InitParallaxEffect((mapSize.y + 1) * mapTileSize);

        tilesetLoader.SetEnvironmentProps(mData.propTiles);
    }

    public void SetSeed()
    {
        if (mapSeed == 0)
            mapSeed = Random.Range(0, 1000000000);

        Random.InitState(mapSeed);
    }

    public void GenerateMap()
    {
        // compensate for border
        mapSize -= Vector2.one * 2;
        // create map list
        for (int i = 0; i < mapSize.x * mapSize.y; i++)
            mapTileList.Add(null);
        // set start tile
        SetStartTile();
        // set tiles
        SetNextTile();
        // set border tiles
        SetBorderTiles();
        // set positions
        transform.position -= new Vector3((mapSize.x - 1) * mapTileSize / 2, (mapSize.y - 1) * mapTileSize / 2, 0);
        cam.transform.localPosition -= transform.position - new Vector3(0, 0, -10);
        cam.orthographicSize = cam.transform.localPosition.x > cam.transform.localPosition.y ? 
            cam.transform.localPosition.x + 50 : cam.transform.localPosition.y + 50;
        mapSize += Vector2.one * 2;
        // set chests
        InitChests();
        // set doors & portals
        InitDoor();
        // start fog of war
        if (fogOfWar != null)
            fogOfWar.Initialize(mapFogList);
        // set A* path
        SetAStarNavMesh();
        // debug
        Debug.Log("Generation Complete!");
    }

    private void AssignMapVariables()
    {
        chestSpawn = mData.chestSpawn;
        mapSize = mData.mapSize;
        mapTileSize = mData.mapTileSize;
        solidTileBorderThickness = mData.solidTileBorderThickness;
        mapSeed = mData.mapSeed;
        SetSeed();

        mapTileController.InitMapTiles(mData);
        startingTilePrefabs.AddRange(mapTileController.startTilePrefabs);
        allTilePrefabs.AddRange(mapTileController.autoInitTilePrefabs);
        allTilePrefabs.AddRange(mapTileController.deadEndTilePrefabs);
        allTilePrefabs.AddRange(mapTileController.uniqueTilePrefabs);
        allTilePrefabs.AddRange(mapTileController.solidTilePrefabs);
        borderTilePrefabs.AddRange(mapTileController.borderTilePrefabs);
        allTilePrefabs.AddRange(mapTileController.shopTilePrefabs);
        shopTilePrefabs.AddRange(mapTileController.shopTilePrefabs);
        for (int i = 0; i < borderTilePrefabs.Count; i++)
        {
            if (borderTilePrefabs[i].name.Contains("1U_1D"))
            {
                if (borderTilePrefabs[i].name.Contains("0L"))
                    leftEdgeTilePrefabs.Add(borderTilePrefabs[i]);
                if (borderTilePrefabs[i].name.Contains("0R"))
                    rightEdgeTilePrefabs.Add(borderTilePrefabs[i]);
            }
            if (borderTilePrefabs[i].name.Contains("1L_1R"))
            {
                if (borderTilePrefabs[i].name.Contains("0U"))
                    topEdgeTilePrefabs.Add(borderTilePrefabs[i]);
                if (borderTilePrefabs[i].name.Contains("0D"))
                    bottomEdgeTilePrefabs.Add(borderTilePrefabs[i]);
            }
            if (borderTilePrefabs[i].name.Contains("0U_1D_0L_1R"))
                TLCornerTilePrefabs.Add(borderTilePrefabs[i]);
            if (borderTilePrefabs[i].name.Contains("0U_1D_1L_0R"))
                TRCornerTilePrefabs.Add(borderTilePrefabs[i]);
            if (borderTilePrefabs[i].name.Contains("1U_0D_0L_1R"))
                BLCornerTilePrefabs.Add(borderTilePrefabs[i]);
            if (borderTilePrefabs[i].name.Contains("1U_0D_1L_0R"))
                BRCornerTilePrefabs.Add(borderTilePrefabs[i]);
        }
        solidTilePrefabs.AddRange(mapTileController.solidTilePrefabs);
    }

    private void SetStartTile()
    {
        // randomize starting node
        currTilePos = new Vector2(Random.Range(0, (int)mapSize.x), Random.Range(0, (int)mapSize.y));
        startingTilePos = (currTilePos * mapTileSize) - new Vector2((mapSize.x - 1) * mapTileSize / 2, (mapSize.y - 1) * mapTileSize / 2);
        // set random starting room tile
        int randomIndex = Random.Range(0, startingTilePrefabs.Count);
        mapTileList[(int)currTilePos.x + (int)(currTilePos.y * mapSize.x)] = InstantiateTile(startingTilePrefabs[randomIndex], currTilePos * mapTileSize, true).GetComponent<MapTile>();
        mapPortalList.Add(mapTileList[(int)currTilePos.x + (int)(currTilePos.y * mapSize.x)].doorTransform.GetComponent<Portal>());
        // add new collapsable tiles
        collapsedTilePosList.Add(currTilePos);
        AddCollapsableTiles();
    }

    private void SetNextTile()
    {
        // find tile with least amount of collapse options
        Vector2 tileToCollapse = Vector2.zero;
        int leastTilesToCollapse = -1;
        for (int i = 0; i < collapsableTilePosList.Count; i++)
        {
            if (CheckTileInBounds(collapsableTilePosList[i]))
            {
                if (tileToCollapse == Vector2.zero)
                {
                    tileToCollapse = collapsableTilePosList[i];
                    leastTilesToCollapse = collapsableTileAmtList[i];
                }
                else if (collapsableTileAmtList[i] <= leastTilesToCollapse)
                {
                    tileToCollapse = collapsableTilePosList[i];
                    leastTilesToCollapse = collapsableTileAmtList[i];
                }
            }
        }
        if (tileToCollapse == null || leastTilesToCollapse < 0)
            return;
        // set curr tile to it
        currTilePos = tileToCollapse;
        // check neighbouring tiles for placeable tiles in current tile
        List<GameObject> availableTiles = GetAvailableTilesList();
        if (availableTiles.Count == 0)
            Debug.Log(currTilePos);
        // choose random tile
        int ranIndex = Random.Range(0, availableTiles.Count);
        GameObject tileToSet = availableTiles[ranIndex];
        if (tileToSet == null)
            return;
        // set room tile
        mapTileList[(int)currTilePos.x + (int)(currTilePos.y * mapSize.x)] = InstantiateTile(tileToSet, currTilePos * mapTileSize, true).GetComponent<MapTile>();
        // remove tile from collapsable list
        collapsableTilePosList.Remove(tileToCollapse);
        collapsedTilePosList.Add(tileToCollapse);
        collapsableTileAmtList.Remove(leastTilesToCollapse);
        // add new collapsable tiles
        AddCollapsableTiles();
        // set next tile
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
            Vector2 pos = tilePos - Vector2.up;
            if (mapTileList[(int)pos.x + (int)(pos.y * mapSize.x)].name.Contains("XU"))
                break;
            List<GameObject> availableTiles = GetAvailableBorderTilesList(tilePos - Vector2.up, Vector2.up);
            // choose random tile
            GameObject tileToSet = availableTiles[Random.Range(0, availableTiles.Count)];
            if (tileToSet == null)
                return;
            // set room
            mapTileList.Add(tileToSet.GetComponent<MapTile>());
            // place room
            InstantiateTile(tileToSet, tilePos * mapTileSize, true);
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
            mapTileList.Add(tileToSet.GetComponent<MapTile>());
            // place room
            InstantiateTile(tileToSet, tilePos * mapTileSize, true);
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
            mapTileList.Add(tileToSet.GetComponent<MapTile>());
            // place room
            InstantiateTile(tileToSet, tilePos * mapTileSize, true);
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
            mapTileList.Add(tileToSet.GetComponent<MapTile>());
            // place room
            InstantiateTile(tileToSet, tilePos * mapTileSize, true);
        }
        // place corners
        GameObject corner;
        // top left corner
        corner = InstantiateTile(TLCornerTilePrefabs[Random.Range(0, TLCornerTilePrefabs.Count)], new Vector2(-1, mapSize.y) * mapTileSize, true);
        // set room
        mapTileList.Add(corner.GetComponent<MapTile>());

        // top right corner
        corner = InstantiateTile(TRCornerTilePrefabs[Random.Range(0, TRCornerTilePrefabs.Count)], new Vector2(mapSize.x, mapSize.y) * mapTileSize, true);
        // set room
        mapTileList.Add(corner.GetComponent<MapTile>());

        // bottom left corner
        corner = InstantiateTile(BLCornerTilePrefabs[Random.Range(0, BLCornerTilePrefabs.Count)], new Vector2(-1, -1) * mapTileSize, true);
        // set room
        mapTileList.Add(corner.GetComponent<MapTile>());

        // bottom right corner
        corner = InstantiateTile(BRCornerTilePrefabs[Random.Range(0, BRCornerTilePrefabs.Count)], new Vector2(mapSize.x, -1) * mapTileSize, true);
        // set room
        mapTileList.Add(corner.GetComponent<MapTile>());

        PlaceSolidBorderTiles();
    }

    private GameObject InstantiateTile(GameObject target, Vector3 targetPos, bool placeFog)
    {
        MapTile newTile = Instantiate(target, transform).GetComponent<MapTile>();
        newTile.transform.localPosition = targetPos;
        if (placeFog && !noFog)
            mapFogList.Add(Instantiate(fogPrefab, newTile.transform, false));

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
        for (int i = -solidTileBorderThickness - 1; i < mapSize.x + solidTileBorderThickness + 1; i++)
        {
            for (int j = -solidTileBorderThickness - 1; j < mapSize.y + solidTileBorderThickness + 1; j++)
            {
                if (i < -1 || i > mapSize.x || j < -1 || j > mapSize.y)
                    InstantiateTile(solidTilePrefabs[Random.Range(0, solidTilePrefabs.Count)], new Vector2(i, j) * mapTileSize, false);
            }
        }
    }

    private void SetAStarNavMesh()
    {
        AstarPath.active.data.gridGraph.RelocateNodes(center: ((mapSize - Vector2.one) * mapTileSize / 2) + new Vector2(0, 0.5f), rotation: Quaternion.Euler(-90, 0, 0), nodeSize: 1.0f);
        AstarPath.active.Scan();
    }

    private void InitChests()
    {
        // get all possible chest transforms
        List<Transform> chests = new List<Transform>();
        for (int i = 0; i < mapTileList.Count; i++)
        {
            if (mapTileList[i].chestTransform != null)
                chests.Add(mapTileList[i].chestTransform);
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
        mapChestList.Add(Instantiate(chestSpawn.chestTypes[0], chests[randomChestIndex], false).GetComponent<Chest>());
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
                    mapChestList.Add(obj.GetComponent<Chest>());
                    obj.transform.SetParent(chests[randomChestIndex]);
                    obj.transform.localPosition = Vector3.zero;
                    chests.RemoveAt(randomChestIndex);
                    break;
                }
            }
        }

        //Black Card
        for (int i = 0; i < itemStats.blackCardChestAmount; i++)
            mapChestList[i].SetCost(0);
    }

    private void InitDoor()
    {
        PlayerController.Instance.transform.position = GetStartingPos();

        List<Transform> doors = new List<Transform>();
        Transform doorTransform = null;
        // get doors
        for (int i = 0; i < mapTileList.Count; i++)
        {
            if (mapTileList[i].doorTransform != null)
                doors.Add(mapTileList[i].doorTransform);
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
            mapPortalList.Add(Instantiate(portalPrefab, doors[i], false).GetComponent<Portal>());
        }

        PlayerController.Instance.portalController.PositionPortals(mapPortalList, cam);
    }

    private List<GameObject> GetAvailableBorderTilesList(Vector2 checkTilePos, Vector2 direction)
    {
        List<GameObject> availableTiles = new List<GameObject>();
        List<GameObject> tilesToRemove = new List<GameObject>();
        List<GameObject> placeableTiles;
        if (direction == Vector2.up)
        {
            placeableTiles = mapTileList[(int)checkTilePos.x + (int)(checkTilePos.y * mapSize.x)].availableTilesUp;
            // get list of tiles that are available in each list
            availableTiles.AddRange(allTilePrefabs);
            for (int i = 0; i < availableTiles.Count; i++)
            {
                if (!placeableTiles.Contains(availableTiles[i]) ||
                    !topEdgeTilePrefabs.Contains(availableTiles[i]))
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
            placeableTiles = mapTileList[(int)checkTilePos.x + (int)(checkTilePos.y * mapSize.x)].availableTilesDown;
            // get list of tiles that are available in each list
            availableTiles.AddRange(allTilePrefabs);
            for (int i = 0; i < availableTiles.Count; i++)
            {
                if (!placeableTiles.Contains(availableTiles[i]) ||
                    !bottomEdgeTilePrefabs.Contains(availableTiles[i]))
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
            placeableTiles = mapTileList[(int)checkTilePos.x + (int)(checkTilePos.y * mapSize.x)].availableTilesLeft;
            // get list of tiles that are available in each list
            availableTiles.AddRange(allTilePrefabs);
            for (int i = 0; i < availableTiles.Count; i++)
            {
                if (!placeableTiles.Contains(availableTiles[i]) ||
                    !leftEdgeTilePrefabs.Contains(availableTiles[i]))
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
            placeableTiles = mapTileList[(int)checkTilePos.x + (int)(checkTilePos.y * mapSize.x)].availableTilesRight;
            // get list of tiles that are available in each list
            availableTiles.AddRange(allTilePrefabs);
            for (int i = 0; i < availableTiles.Count; i++)
            {
                if (!placeableTiles.Contains(availableTiles[i]) ||
                    !rightEdgeTilePrefabs.Contains(availableTiles[i]))
                {
                    tilesToRemove.Add(availableTiles[i]);
                }
            }
            for (int i = 0; i < tilesToRemove.Count; i++)
            {
                availableTiles.Remove(tilesToRemove[i]);
            }
        }
        if (!isShopPlaced)
        {
            List<GameObject> availableShopTiles = new();
            for (int i = 0; i < shopTilePrefabs.Count; i++)
            {
                if (availableTiles.Contains(shopTilePrefabs[i]))
                {
                    availableShopTiles.Add(shopTilePrefabs[i]);
                }
            }
            if (availableShopTiles.Count > 0)
            {
                isShopPlaced = true;
                return availableShopTiles;
            }
        }
        else
        {
            tilesToRemove.Clear();
            for (int i = 0; i < availableTiles.Count; i++)
            {
                if (shopTilePrefabs.Contains(availableTiles[i]))
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

        List<string> NameUp = new();
        List<string> NameDown = new();
        List<string> NameLeft = new();
        List<string> NameRight = new();
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
        List<GameObject> availableTiles = new();
        List<GameObject> tilesToRemove = new();
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
        Vector2 checkTilePos = currTilePos + direction;
        // check if not out of the map and if the space has an existing tile
        if (CheckTileInBounds(checkTilePos))
        {
            if (mapTileList[(int)checkTilePos.x + (int)(checkTilePos.y * mapSize.x)] != null)
            {
                if (direction == Vector2.up)
                {
                    return mapTileList[(int)checkTilePos.x + (int)(checkTilePos.y * mapSize.x)].availableTilesDown;
                }
                else if (direction == Vector2.down)
                {
                    return mapTileList[(int)checkTilePos.x + (int)(checkTilePos.y * mapSize.x)].availableTilesUp;
                }
                else if (direction == Vector2.left)
                {
                    return mapTileList[(int)checkTilePos.x + (int)(checkTilePos.y * mapSize.x)].availableTilesRight;
                }
                else if (direction == Vector2.right)
                {
                    return mapTileList[(int)checkTilePos.x + (int)(checkTilePos.y * mapSize.x)].availableTilesLeft;
                }
            }
        }
        return allTilePrefabs;
    }

    private void AddCollapsableTiles()
    {
        // add to list of placeable tiles in each direction and start from least amount of tiles
        if (CheckTileInBounds(currTilePos + Vector2.up) && mapTileList[(int)currTilePos.x + (int)((currTilePos.y + 1) * mapSize.x)] == null)
        {
            if (!collapsedTilePosList.Contains(currTilePos + Vector2.up) && !collapsableTilePosList.Contains(currTilePos + Vector2.up))
            {
                collapsableTilePosList.Add(currTilePos + Vector2.up);
                collapsableTileAmtList.Add(GetCurrentRoomTile().availableTilesUp.Count);
            }
        }
        if (CheckTileInBounds(currTilePos + Vector2.down) && mapTileList[(int)currTilePos.x + (int)((currTilePos.y - 1) * mapSize.x)] == null)
        {
            if (!collapsedTilePosList.Contains(currTilePos + Vector2.down) && !collapsableTilePosList.Contains(currTilePos + Vector2.down))
            {
                collapsableTilePosList.Add(currTilePos + Vector2.down);
                collapsableTileAmtList.Add(GetCurrentRoomTile().availableTilesDown.Count);
            }
        }
        if (CheckTileInBounds(currTilePos + Vector2.left) && mapTileList[(int)(currTilePos.x - 1) + (int)(currTilePos.y * mapSize.x)] == null)
        {
            if (!collapsedTilePosList.Contains(currTilePos + Vector2.left) && !collapsableTilePosList.Contains(currTilePos + Vector2.left))
            {
                collapsableTilePosList.Add(currTilePos + Vector2.left);
                collapsableTileAmtList.Add(GetCurrentRoomTile().availableTilesLeft.Count);
            }
        }
        if (CheckTileInBounds(currTilePos + Vector2.right) && mapTileList[(int)(currTilePos.x + 1) + (int)(currTilePos.y * mapSize.x)] == null)
        {
            if (!collapsedTilePosList.Contains(currTilePos + Vector2.right) && !collapsableTilePosList.Contains(currTilePos + Vector2.right))
            {
                collapsableTilePosList.Add(currTilePos + Vector2.right);
                collapsableTileAmtList.Add(GetCurrentRoomTile().availableTilesRight.Count);
            }
        }
    }

    private bool CheckTileInBounds(Vector2 pos)
    {
        return pos.x >= 0 && pos.x < mapSize.x && pos.y >= 0 && pos.y < mapSize.y;
    }

    private MapTile GetCurrentRoomTile()
    {
        if (CheckTileInBounds(currTilePos))
            return mapTileList[(int)currTilePos.x + (int)(currTilePos.y * mapSize.x)];
        return null;
    }

    public Vector2 GetStartingPos()
    {
        return startingTilePos;
    }
}
