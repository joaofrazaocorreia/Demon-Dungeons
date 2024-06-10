using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Class that generates the dungeon maps and spawns enemies.
/// </summary>
public class MapGenerator : MonoBehaviour
{
    [SerializeField] private bool startGeneratingOnPlay;
    [SerializeField] private UIManager uIManager;
    [SerializeField] private PlayerMovement player;
    [SerializeField] private List<GameObject> layer1EnemyPrefabs;
    [SerializeField] private List<GameObject> layer2EnemyPrefabs;
    [SerializeField] private List<GameObject> layer3EnemyPrefabs;
    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private List<GameObject> breakablesPrefabs;
    [SerializeField] private List<Tile> tiles;
    [SerializeField] private Transform map;
    [SerializeField] private Transform enemies;
    [SerializeField] private Transform waypoints;
    [SerializeField] private Transform breakables;
    [SerializeField] public Transform drops;
    [SerializeField] private Transform boss;
    [SerializeField] private int seed = 0;
    [SerializeField] private float generationIntervals = 0.01f;
    [SerializeField] private float minStartingMapSize = 35f;
    [SerializeField] private float minTilesToSpawnEnding = 50f;
    [SerializeField] private float minTilesToCloseMap = 60f;
    [SerializeField] private float minDistanceToEnding = 100f;
    [SerializeField] private float minTotalTiles = 80f;
    [SerializeField] private float enemyCount = 50f;
    [SerializeField] private float enemyLimit = 250f;
    [SerializeField] private float breakableCount = 30f;
    [SerializeField] private float layerIncrements = 5f;

    private SaveDataManager saveDataManager;
    private Vector3 startingCoords;
    private bool hasEnemyGate;
    private bool hasEnding;
    private bool validMap;
    private List<Coroutine> coroutinesQueue;
    private float coroutinesQueueDeadTimer;
    private Tile currentStartingTile;
    private Tile currentEndingTile;
    private Tile currentGateTile;
    private List<GameObject> currentEnemies;

    public Tile CurrentStartingTile { get => currentStartingTile; }
    public Tile CurrentEndingTile { get => currentEndingTile; }
    public Tile CurrentGateTile { get => currentGateTile; }
    public int LayerCount { get; set; }
    public int FloorCount { get; set; }
    public int DungeonCount { get; set; }
    public bool IsInSafeRoom{ get; set; }
    public List<GameObject> CurrentEnemies { get => currentEnemies; }
    public float EnemyLimit { get => enemyLimit; }

    private void Awake()
    {
        foreach (Tile t in tiles)
            t.Setup();

        saveDataManager = FindObjectOfType<SaveDataManager>();

        coroutinesQueue = new List<Coroutine>();
        coroutinesQueueDeadTimer = 0f;

        currentEnemies = new List<GameObject>();

        startingCoords = new Vector3(100f, 50f, 100f);
        hasEnemyGate = false;
        hasEnding = false;
        validMap = false;
        LayerCount = saveDataManager.CheckLayerCountData(0);
        FloorCount = saveDataManager.CheckFloorCountData(0);
        DungeonCount = saveDataManager.CheckDungeonCountData(1);

        if (seed != 0)
        {
            // System.Random()
            Random.InitState(seed);
            Debug.Log("seed init: " + seed);
        }

        if(startGeneratingOnPlay)
        {
            StartCoroutine(Begin());
        }
    }

    /// <summary>
    /// Initial coroutine to wait for the project to load before generating the
    /// map, then creates the first Safe Room.
    /// </summary>
    /// <returns></returns>
    private IEnumerator Begin()
    {
        yield return new WaitForSeconds(0.001f);
        CreateSafeRoom();
    }

    /// <summary>
    /// Checks two player cheats to manually generate and delete maps.
    /// </summary>
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.H))
        {
            StopAllCoroutines();
            StartCoroutine(DeleteMap(false));
        }

        if(Input.GetKeyDown(KeyCode.J))
        {
            CreateMapStart();
        }

        UpdateCoroutines();
    }

    /// <summary>
    /// Checks if the coroutine queue is empty or dead to mark it as finished.
    /// </summary>
    private void UpdateCoroutines()
    {
        if (coroutinesQueue.Count > 0)
        {
            if (coroutinesQueue.First() == null)
            {
                coroutinesQueueDeadTimer -= Time.deltaTime;

                if (coroutinesQueueDeadTimer <= 0)
                {
                    coroutinesQueue = new List<Coroutine>();
                }
            }

            // Resets the map if it didn't meet the criteria and is done generating.
            if (coroutinesQueueDeadTimer <= 0)
            {
                if (!hasEnding || !hasEnemyGate || map.childCount <
                    minTotalTiles + (LayerCount * layerIncrements) ||
                        (currentEndingTile.transform.position - currentStartingTile.
                            transform.position).magnitude <= minDistanceToEnding)
                {
                    StopAllCoroutines();
                    StartCoroutine(DeleteMap(true));
                }

                // Spawns the enemies if the map is accepted.
                else if (!validMap)
                {
                    validMap = true;
                    GetComponent<NavMeshSurface>().BuildNavMesh();
                    StartGeneratingEnemies();
                    StartGeneratingBreakables();

                    player.MoveTo(currentStartingTile.transform.position + new Vector3(0, 2, -5));
                    Debug.Log($"Finished map with {map.childCount} tiles");
                    
                    uIManager.FadeOutLoadingScreen();
                }
            }
        }
    }

    /// <summary>
    /// Creates the starting tile and begins generating the map from there.
    /// </summary>
    private void CreateMapStart()
    {
        if (coroutinesQueue.Count > 0)
            return;
        
        uIManager.FadeInLoadingScreen();

        List<Tile> startingTiles = tiles.Where(tile => tile.TileData.type == TileData.Type.Start).ToList();
        Tile start = Instantiate(startingTiles[Random.Range(0, startingTiles.Count)],
            parent: map, position: startingCoords, rotation: Quaternion.identity);
        currentStartingTile = start;

        switch(LayerCount)
                {
                    case 1:
                        foreach(GameObject go in layer1EnemyPrefabs)
                            currentEnemies.Add(go);
                        break;

                    case 2:
                        foreach(GameObject go in layer2EnemyPrefabs)
                            currentEnemies.Add(go);
                        break;

                    case 3:
                        foreach(GameObject go in layer3EnemyPrefabs)
                            currentEnemies.Add(go);
                        break;

                    default:
                        foreach(GameObject go in layer1EnemyPrefabs)
                            currentEnemies.Add(go);
                        foreach(GameObject go in layer2EnemyPrefabs)
                            currentEnemies.Add(go);
                        foreach(GameObject go in layer3EnemyPrefabs)
                            currentEnemies.Add(go);
                        break;
                }

        player.MoveTo(new Vector3(0, 20, 0));

        StartGeneratingExits(start);
    }

    /// <summary>
    /// Coroutine to generate the map procedurally by filling each tile's exits.
    /// </summary>
    /// <param name="tile">The tile whose exits are being filled.</param>
    private void StartGeneratingExits(Tile tile)
    {
        while(true)
        {
            if (coroutinesQueue.Count() == 0)
            {
                coroutinesQueue.Add(StartCoroutine(GenerateExits(tile)));
                coroutinesQueueDeadTimer = 1f;
                break;
            }

            else
                continue;
        }
        
    }

    /// <summary>
    /// Coroutine that randomly generates a new tile for every empty exit in every existing tile, with filters for specific tile numbers.
    /// </summary>
    /// <param name="tile">The tile whose exits are being filled.</param>
    /// <returns></returns>
    private IEnumerator GenerateExits(Tile tile)
    {
        tile.Setup();
        
        while (tile.exits.childCount > 0)
        {
            List<Tile> tilesToChoose;
            Tile newTile;
            bool tileAccepted = false;

            // Picks a random exit from the given tile to generate the next tile
            Transform chosenExit = tile.exits.GetChild(Random.Range(0, tile.exits.childCount));

            // Different tiles can be chosen/forced depending on the amount of tiles already generated
            if (map.childCount < minStartingMapSize + (LayerCount * layerIncrements))
                tilesToChoose = tiles.Where(tile => tile.exits.childCount >= 2 && tile.TileData.type == TileData.Type.Regular).ToList();

            else if (map.childCount >= minTilesToSpawnEnding - 1 + (LayerCount * layerIncrements) && !hasEnemyGate)
            {
                tilesToChoose = tiles.Where(tile => tile.TileData.type == TileData.Type.EnemyGate).ToList();
                hasEnemyGate = true;
            }

            else if (map.childCount >= minTilesToSpawnEnding + (LayerCount * layerIncrements) && !hasEnding && hasEnemyGate)
            {
                tilesToChoose = tiles.Where(tile => tile.TileData.type == TileData.Type.End).ToList();
                hasEnding = true;
            }

            else if (map.childCount > minTilesToCloseMap + (LayerCount * layerIncrements))
                tilesToChoose = tiles.Where(tile => tile.exits.childCount <= 2 && tile.TileData.type == TileData.Type.Regular).ToList();

            else
                tilesToChoose = tiles.Where(tile => tile.TileData.type == TileData.Type.Regular).ToList();


            while (!tileAccepted)
            {
                // If no suitable tiles are found, a wall is created instead
                if(tilesToChoose.Count <= 0)
                    tilesToChoose = tiles.Where(tile => tile.TileData.type == TileData.Type.Wall).ToList();


                // Picks a random tile from the filtered ones and instantiates it
                int randomIndex = Random.Range(0, tilesToChoose.Count);
                newTile = Instantiate(tilesToChoose[randomIndex], parent: map, position: startingCoords, rotation: Quaternion.Euler(Vector3.zero));

                yield return new WaitForSeconds(generationIntervals);


                // Picks a random exit from the chosen random tile to connect to the current exit
                Transform chosenEntrance = newTile.exits.GetChild(Random.Range(0, newTile.exits.childCount));


                // Rotates the new tile to match the direction of its exit
                Quaternion pivotRotation = Quaternion.Euler(Vector3.up * (chosenEntrance.eulerAngles.y - newTile.transform.eulerAngles.y));

                newTile.transform.rotation = Quaternion.Euler(Vector3.up * chosenEntrance.eulerAngles.y);

                yield return new WaitForSeconds(generationIntervals);


                for (int i = 0; i < newTile.transform.childCount; i++)
                {
                    newTile.transform.GetChild(i).rotation = Quaternion.Euler(Vector3.up * (newTile.transform.GetChild(i).eulerAngles.y - pivotRotation.eulerAngles.y));
                }

                yield return new WaitForSeconds(generationIntervals);


                // Changes the pivot of the new tile to its exit so it rotates around it
                Vector3 pivotVector = chosenEntrance.position - newTile.transform.position;
                newTile.transform.position += pivotVector;

                yield return new WaitForSeconds(generationIntervals);


                for (int i = 0; i < newTile.transform.childCount; i++)
                {
                    newTile.transform.GetChild(i).position -= pivotVector;
                }

                yield return new WaitForSeconds(generationIntervals);


                // Rotates the new tile to match the inverse direction of the exit to connect
                newTile.transform.rotation = Quaternion.Euler(Vector3.up * (chosenExit.eulerAngles.y + 180));

                yield return new WaitForSeconds(generationIntervals);


                // Moves the tile to allign both exits
                Vector3 distanceVector = chosenExit.position - newTile.transform.position;
                newTile.transform.position += distanceVector;

                yield return new WaitForSeconds(generationIntervals);
                newTile.model.gameObject.SetActive(false);
                yield return new WaitForSeconds(generationIntervals);


                if (newTile.tileCollision.CollisionDetected && newTile.TileData.type != TileData.Type.Wall)
                {
                    yield return new WaitForSeconds(generationIntervals*10);

                    if (newTile.TileData.type == TileData.Type.EnemyGate)
                        hasEnemyGate = false;

                    if (newTile.TileData.type == TileData.Type.End)
                        hasEnding = false;

                    Destroy(newTile.gameObject);
                    tilesToChoose.Remove(tilesToChoose[randomIndex]);

                    yield return new WaitForSeconds(generationIntervals);

                    continue;
                }
                
                newTile.model.gameObject.SetActive(true);


                // Destroys both connected exits to prevent them from being connected again
                Destroy(chosenExit.gameObject);
                Destroy(chosenEntrance.gameObject);
                tileAccepted = true;

                if (newTile.TileData.type == TileData.Type.EnemyGate)
                {
                    hasEnemyGate = true;
                    currentGateTile = newTile;
                }

                if (newTile.TileData.type == TileData.Type.End)
                {
                    hasEnding = true;
                    currentEndingTile = newTile;
                }


                yield return new WaitForSeconds(generationIntervals);

                if(coroutinesQueue.Count > 0)
                    coroutinesQueue.Remove(coroutinesQueue[0]);
                // Generates the exits for the new tile, to keep the loop going until there's no more exits left
                StartGeneratingExits(newTile);
            }
        }
    }

    /// <summary>
    /// Begins deleting the current map.
    /// </summary>
    /// <param name="regenerateMap">Whether a new map should be generated right after.</param>
    /// <param name="createSafeRoom">Whether the new map should always be a safe room.</param>
    public void StartDeletingMap(bool regenerateMap = false, bool createSafeRoom = false)
    {
        StopAllCoroutines();
        StartCoroutine(DeleteMap(regenerateMap, createSafeRoom));
        uIManager.FadeInLoadingScreen();
    }

    /// <summary>
    /// Coroutine to delete all map tiles and enemies currently generated.
    /// </summary>
    /// <param name="regenerateMap">Whether a new map should be generated right after.</param>
    /// <param name="createSafeRoom">Whether the new map should always be a safe room.</param>
    /// <returns></returns>
    private IEnumerator DeleteMap(bool regenerateMap = false, bool createSafeRoom = false)
    {
        // Clear all current instances of tile generation to prevent softlocking the deletion while the map is being made
        coroutinesQueue = new List<Coroutine>();
        StartCoroutine(DeleteEnemies());
        StartCoroutine(DeleteBreakables());
        StartCoroutine(DeleteDrops());

        Debug.Log($"Deleting map ({map.childCount} tiles)");
        while(map.childCount > 0)
        {
            Destroy(map.GetChild(0).gameObject);
            yield return new WaitForSeconds(0.001f);
        }

        // Resets Navmesh and map requirements
        GetComponent<NavMeshSurface>().RemoveData();
        coroutinesQueue = new List<Coroutine>();
        hasEnemyGate = false;
        hasEnding = false;
        validMap = false;


        if (regenerateMap)
        {
            if (createSafeRoom)
                CreateSafeRoom();

            else if (LayerCount % 4 == 0)
                CreateBossRoom();

            else
                CreateMapStart();
        }
    }

    /// <summary>
    /// Coroutine to delete all enemies currently generated.
    /// </summary>
    /// <returns></returns>
    private IEnumerator DeleteEnemies()
    {
        Debug.Log($"Deleting enemies ({enemies.childCount} entities)");
        while(enemies.childCount > 0)
        {
            Destroy(enemies.GetChild(0).gameObject);
            yield return new WaitForSeconds(0.001f);
        }
    }

    /// <summary>
    /// Starts generating enemies on the map.
    /// </summary>
    private void StartGeneratingEnemies()
    {
        StartCoroutine(SpawnEnemies());
    }

    /// <summary>
    /// Coroutine to generate the enemies procedurally.
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpawnEnemies()
    {
        List<Transform> tilesWithEnemies = new List<Transform>();

        // Filters which tiles can spawn enemies.
        for(int i = 0; i < map.childCount; i++)
        {
            if(map.GetChild(i).GetComponent<Tile>().TileData.spawnsEnemies)
                tilesWithEnemies.Add(map.GetChild(i));
        }

        while(enemies.childCount < Mathf.Min(enemyCount + (LayerCount * layerIncrements), enemyLimit) && tilesWithEnemies.Count > 0)
        {
            // Choses the spot to spawn the enemy.
            Transform chosenSpot;
            Transform chosenTile = tilesWithEnemies[Random.Range(0, tilesWithEnemies.Count)].Find("EnemySpawns");

            if(chosenTile.childCount > 0)
                chosenSpot = chosenTile.GetChild(Random.Range(0, chosenTile.childCount));
            
            else
            {
                tilesWithEnemies.Remove(chosenTile.parent);
            
                yield return new WaitForSeconds(generationIntervals);
                continue;
            }

            // Generates a waypoint on the enemy's spawn for other enemies to visit it too.
            GameObject newWaypoint = new GameObject("waypoint");
            NavMeshHit closestHit;
            newWaypoint.transform.parent = waypoints;
            newWaypoint.transform.position = chosenSpot.position;
            
            yield return new WaitForSeconds(generationIntervals);

            // Instantiates the enemy.
            if(NavMesh.SamplePosition(chosenSpot.position + new Vector3(0, 1, 0), out closestHit, 500, 1))
            {
                GameObject newEnemy = Instantiate(currentEnemies[Random.Range(0, currentEnemies.Count)], position:closestHit.position, Quaternion.identity);

                newEnemy.transform.parent = enemies;
                newEnemy.GetComponent<Enemy>().playerHealth = player.GetComponent<PlayerHealth>();
                newEnemy.GetComponent<Enemy>().mapGenerator = this;
                newEnemy.name = "Enemy " + enemies.childCount;

                foreach(Transform t in enemies)
                    t.GetComponent<Enemy>().waypoints.Add(newWaypoint.transform);
            }
        
            Destroy(chosenSpot.gameObject);
            
            yield return new WaitForSeconds(generationIntervals);
        }

        coroutinesQueue = new List<Coroutine>();
    }

    private IEnumerator DeleteDrops()
    {
        Debug.Log($"Deleting drops ({drops.childCount} entities)");
        while(drops.childCount > 0)
        {
            Destroy(drops.GetChild(0).gameObject);
            yield return new WaitForSeconds(0.001f);
        }
    }
    
    private IEnumerator DeleteBreakables()
    {
        Debug.Log($"Deleting breakables ({breakables.childCount} entities)");
        while(breakables.childCount > 0)
        {
            Destroy(breakables.GetChild(0).gameObject);
            yield return new WaitForSeconds(0.001f);
        }
    }

    /// <summary>
    /// Starts generating Breakables on the map.
    /// </summary>
    private void StartGeneratingBreakables()
    {
        StartCoroutine(SpawnBreakables());
    }

    /// <summary>
    /// Coroutine to generate the breakables procedurally.
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpawnBreakables()
    {
        List<Transform> tilesWithBreakables = new List<Transform>();

        // Filters which tiles can spawn enemies.
        for(int i = 0; i < map.childCount; i++)
        {
            if(map.GetChild(i).GetComponent<Tile>().TileData.spawnsBreakables)
                tilesWithBreakables.Add(map.GetChild(i));
        }

        while(breakables.childCount < breakableCount + (LayerCount * layerIncrements) && tilesWithBreakables.Count > 0)
        {
            // Choses the spot to spawn the enemy.
            Transform chosenSpot;
            Transform chosenTile = tilesWithBreakables[Random.Range(0, tilesWithBreakables.Count)].Find("BreakableSpawns");

            if(chosenTile.childCount > 0)
                chosenSpot = chosenTile.GetChild(Random.Range(0, chosenTile.childCount));
            
            else
            {
                tilesWithBreakables.Remove(chosenTile.parent);
            
                yield return new WaitForSeconds(generationIntervals);
                continue;
            }

            NavMeshHit closestHit;
            yield return new WaitForSeconds(generationIntervals);

            // Instantiates the enemy.
            if(NavMesh.SamplePosition(chosenSpot.position + new Vector3(0, 1, 0), out closestHit, 500, 1))
            {
                GameObject newBreakable;
                newBreakable = Instantiate(breakablesPrefabs[Random.Range(0, breakablesPrefabs.Count)], position:closestHit.position, Quaternion.identity);

                newBreakable.transform.parent = breakables;
                newBreakable.GetComponent<Breakable>().minValue = 10;
                newBreakable.GetComponent<Breakable>().maxValue = 25;
                newBreakable.GetComponent<Breakable>().minHealth = 10;
                newBreakable.GetComponent<Breakable>().maxHealth = 40;
                newBreakable.GetComponent<Breakable>().livesAmount = 1;
                newBreakable.GetComponent<Breakable>().drops = drops;
                newBreakable.name = "Breakable " + breakables.childCount;
            }
        
            Destroy(chosenSpot.gameObject);
            
            yield return new WaitForSeconds(generationIntervals);
        }

        coroutinesQueue = new List<Coroutine>();
    }

    /// <summary>
    /// Creates the safe room tile.
    /// </summary>
    private void CreateSafeRoom()
    {
        uIManager.FadeInLoadingScreen();

        IsInSafeRoom = true;

        List<Tile> safeRoomTiles = tiles.Where(tile => tile.TileData.type == TileData.Type.SafeRoom).ToList();
        currentStartingTile = Instantiate(safeRoomTiles[Random.Range(0, safeRoomTiles.Count)], parent: map, position: startingCoords, rotation: Quaternion.identity);

        player.MoveTo(currentStartingTile.transform.position + new Vector3(0, 2, -19f));

        uIManager.FadeOutLoadingScreen();
    }

    /// <summary>
    /// Creates the boss room tile.
    /// </summary>
    private void CreateBossRoom()
    {
        uIManager.FadeInLoadingScreen();

        List<Tile> bossRoomTiles = tiles.Where(tile => tile.TileData.type == TileData.Type.BossRoom).ToList();
        currentStartingTile = Instantiate(bossRoomTiles[0], parent: map, position: startingCoords, rotation: Quaternion.identity);
        GetComponent<NavMeshSurface>().BuildNavMesh();

        NavMeshHit closestHit;
        if (NavMesh.SamplePosition(currentStartingTile.transform.position + new Vector3(0,0,30), out closestHit, 500, 1))
        {
            GameObject newBoss = Instantiate(bossPrefab, position: closestHit.position, Quaternion.Euler(new Vector3(0, 180, 0)));
            newBoss.transform.parent = boss;
            newBoss.name = "Demon General";
        }

        player.MoveTo(currentStartingTile.transform.position + new Vector3(0, 2, -25));

        uIManager.FadeOutLoadingScreen();
    }
}
