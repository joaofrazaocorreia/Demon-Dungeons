using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private PlayerMovement player;
    [SerializeField] private List<GameObject> enemyPrefabs;
    [SerializeField] private List<Tile> tiles;
    [SerializeField] private Transform map;
    [SerializeField] private Transform enemies;
    [SerializeField] private Transform waypoints;
    [SerializeField] private Transform boss;
    [SerializeField] private float generationIntervals = 0.01f;
    [SerializeField] private float minStartingMapSize = 35f;
    [SerializeField] private float minTilesToSpawnEnding = 50f;
    [SerializeField] private float minTilesToCloseMap = 60f;
    [SerializeField] private float minDistanceToEnding = 100f;
    [SerializeField] private float minTotalTiles = 80f;
    [SerializeField] private float enemyCount = 30f;
    [SerializeField] private float layerIncrements = 5f;

    private Vector3 startingCoords;
    private bool hasEnemyGate;
    private bool hasEnding;
    private bool validMap;
    private List<Coroutine> coroutinesQueue;
    private float coroutinesQueueDeadTimer;
    private Tile currentStartingTile;
    private Tile currentEndingTile;

    public Tile CurrentStartingTile { get => currentStartingTile; }
    public Tile CurrentEndingTile { get => currentEndingTile; }
    public int LayerCount { get; set; }

    private void Awake()
    {
        foreach (Tile t in tiles)
            t.Setup();


        coroutinesQueue = new List<Coroutine>();
        coroutinesQueueDeadTimer = 0f;

        startingCoords = new Vector3(100f, 50f, 100f);
        hasEnemyGate = false;
        hasEnding = false;
        validMap = false;
        LayerCount = 0;
    }


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

                else if (!validMap)
                {
                    validMap = true;
                    GetComponent<NavMeshSurface>().BuildNavMesh();
                    StartGeneratingEnemies();
                    
                    player.MoveTo(currentStartingTile.transform.position + new Vector3(0, 2, -5));
                    Debug.Log($"Finished map with {map.childCount} tiles");
                }
            }
        }
    }

    // Creates the starting tile and begins generating the map from there
    private void CreateMapStart()
    {
        if (coroutinesQueue.Count > 0)
            return;

        List<Tile> startingTiles = tiles.Where(tile => tile.TileData.type == TileData.Type.Start).ToList();
        Tile start = Instantiate(startingTiles[Random.Range(0, startingTiles.Count)],
            parent: map, position: startingCoords, rotation: Quaternion.identity);
        currentStartingTile = start;

        player.MoveTo(new Vector3(0, 20, 0));

        StartGeneratingExits(start);
    }

    // Coroutine to generate the map procedurally
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

    // Randomly generates a new tile for every empty exit in every existing tile, with filters for specific tile numbers
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
                }

                if (newTile.TileData.type == TileData.Type.End)
                {
                    hasEnding = true;
                    currentEndingTile = newTile;
                }


                yield return new WaitForSeconds(generationIntervals);

                coroutinesQueue.Remove(coroutinesQueue[0]);
                // Generates the exits for the new tile, to keep the loop going until there's no more exits left
                StartGeneratingExits(newTile);
            }
        }
    }

    public void StartDeletingMap(bool regenerateMap = false, bool createSafeRoom = false)
    {
        StartCoroutine(DeleteMap(regenerateMap, createSafeRoom));
    }

    // Coroutine to delete all map tiles currently generated
    private IEnumerator DeleteMap(bool regenerateMap = false, bool createSafeRoom = false)
    {
        // Clear all current instances of tile generation to prevent softlocking the deletion while the map is being made
        coroutinesQueue = new List<Coroutine>();
        StartCoroutine(DeleteEnemies());

        Debug.Log($"Deleting map ({map.childCount} tiles)");
        while(map.childCount > 0)
        {
            Destroy(map.GetChild(0).gameObject);
            yield return new WaitForSeconds(0.001f);
        }

        GetComponent<NavMeshSurface>().RemoveData();
        coroutinesQueue = new List<Coroutine>();
        hasEnemyGate = false;
        hasEnding = false;
        validMap = false;

        if (regenerateMap)
        {
            if (createSafeRoom)
                CreateSafeRoom();

            else if (LayerCount >= 3)
                CreateBossRoom();

            else
                CreateMapStart();
        }
    }

    // Coroutine to delete all enemies currently generated
    private IEnumerator DeleteEnemies()
    {
        Debug.Log($"Deleting enemies ({enemies.childCount} entities)");
        while(enemies.childCount > 0)
        {
            Destroy(enemies.GetChild(0).gameObject);
            yield return new WaitForSeconds(0.001f);
        }
    }

    // Coroutine to generate the enemies procedurally
    private void StartGeneratingEnemies()
    {
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        List<Transform> tilesWithEnemies = new List<Transform>();

        for(int i = 0; i < map.childCount; i++)
        {
            if(map.GetChild(i).GetComponent<Tile>().TileData.spawnsEnemies)
                tilesWithEnemies.Add(map.GetChild(i));
        }

        while(enemies.childCount < enemyCount + (LayerCount * layerIncrements) && tilesWithEnemies.Count > 0)
        {
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

            GameObject newWaypoint = new GameObject("waypoint");
            NavMeshHit closestHit;
            newWaypoint.transform.parent = waypoints;
            newWaypoint.transform.position = chosenSpot.position;
            
            yield return new WaitForSeconds(generationIntervals);

            if(NavMesh.SamplePosition(chosenSpot.position + new Vector3(0, 1, 0), out closestHit, 500, 1))
            {
                GameObject newEnemy = Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Count)], position:closestHit.position, Quaternion.identity);
                newEnemy.transform.parent = enemies;
                newEnemy.GetComponent<Enemy>().playerHealth = player.GetComponent<PlayerHealth>();
                newEnemy.name = "Enemy " + enemies.childCount;

                foreach(Transform t in enemies)
                    t.GetComponent<Enemy>().waypoints.Add(newWaypoint.transform);
            }
        
            Destroy(chosenSpot.gameObject);
            
            yield return new WaitForSeconds(generationIntervals);
        }

        coroutinesQueue = new List<Coroutine>();
    }

    // Creates the safe room tile
    private void CreateSafeRoom()
    {
        List<Tile> safeRoomTiles = tiles.Where(tile => tile.TileData.type == TileData.Type.SafeRoom).ToList();
        currentStartingTile = Instantiate(safeRoomTiles[Random.Range(0, safeRoomTiles.Count)], parent: map, position: startingCoords, rotation: Quaternion.identity);

        player.MoveTo(currentStartingTile.transform.position + new Vector3(0, 2, -19f));
    }

    // Creates the boss room tile
    private void CreateBossRoom()
    {
        List<Tile> bossRoomTiles = tiles.Where(tile => tile.TileData.type == TileData.Type.BossRoom).ToList();
        currentStartingTile = Instantiate(bossRoomTiles[0], parent: map, position: startingCoords, rotation: Quaternion.identity);
        GetComponent<NavMeshSurface>().BuildNavMesh();

        NavMeshHit closestHit;
        if (NavMesh.SamplePosition(currentStartingTile.transform.position, out closestHit, 500, 1))
        {
            GameObject newEnemy = Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Count)], position: closestHit.position, Quaternion.Euler(new Vector3(0, 180, 0)));
            newEnemy.transform.parent = boss;
            newEnemy.GetComponent<Enemy>().playerHealth = player.GetComponent<PlayerHealth>();
            newEnemy.name = "Demon General";
        }

        player.MoveTo(currentStartingTile.transform.position + new Vector3(0, 2, -50));
    }
}
