using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private PlayerMovement player;
    [SerializeField] private List<Tile> tiles;
    [SerializeField] private Transform map;
    [SerializeField] private float generationIntervals = 0.01f;

    private Vector3 startingCoords;
    private bool hasEnemyGate;
    private bool hasEnding;
    private List<Coroutine> coroutinesQueue;
    private float coroutinesQueueDeadTimer;
    private Tile currentStartingTile;
    private Tile currentEndingTile;

    private void Awake()
    {
        foreach (Tile t in tiles)
            t.Setup();


        coroutinesQueue = new List<Coroutine>();
        coroutinesQueueDeadTimer = 0f;

        startingCoords = new Vector3(100f, 50f, 100f);
        hasEnemyGate = false;
        hasEnding = false;
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
            CreateMap();
        }

        if (coroutinesQueue.Count > 0)
        {
            if (coroutinesQueue.First() == null)
                coroutinesQueueDeadTimer -= Time.deltaTime;

            if (coroutinesQueueDeadTimer <= 0)
            {
                if (!hasEnding || !hasEnemyGate)
                {
                    StopAllCoroutines();
                    StartCoroutine(DeleteMap(true));
                }

                if (player.transform.position.y < startingCoords.y)
                {
                    player.MoveTo(startingCoords);
                }
            }
        }

    }

    // Creates the starting tile and begins generating the map from there
    private void CreateMap()
    {
        if (coroutinesQueue.Count > 0)
            return;

        List<Tile> startingTiles = tiles.Where(tile => tile.TileData.type == TileData.Type.Start).ToList();
        //List<Tile> startingTiles = tiles.Where(tile => tile.exits.childCount >= 4).ToList();
        Tile start = Instantiate(startingTiles[Random.Range(0, startingTiles.Count)], parent: map, position: startingCoords, rotation: new Quaternion());
        currentStartingTile = start;

        player.MoveTo(currentStartingTile.transform.position + new Vector3(0, 500, 0));

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
        while (tile.exits.childCount > 0)
        {
            List<Tile> tilesToChoose;
            Tile newTile;
            bool tileAccepted = false;

            // Picks a random exit from the given tile to generate the next tile
            Transform chosenExit = tile.exits.GetChild(Random.Range(0, tile.exits.childCount));

            // Different tiles can be chosen/forced depending on the amount of tiles already generated
            if (map.childCount < 20)
                tilesToChoose = tiles.Where(tile => tile.exits.childCount >= 2 && tile.TileData.type == TileData.Type.Regular).ToList();

            else if (map.childCount == 34)
                tilesToChoose = tiles.Where(tile => tile.exits.childCount >= 3 && tile.TileData.type == TileData.Type.Regular).ToList();

            else if (map.childCount >= 35 && !hasEnemyGate)
            {
                tilesToChoose = tiles.Where(tile => tile.TileData.type == TileData.Type.EnemyGate).ToList();
                hasEnemyGate = true;
            }

            else if (map.childCount >= 36 && !hasEnding && hasEnemyGate)
            {
                tilesToChoose = tiles.Where(tile => tile.TileData.type == TileData.Type.End).ToList();
                hasEnding = true;
            }

            else if (map.childCount > 40)
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

        player.MoveTo(currentStartingTile.transform.position + new Vector3(0, 3.5f, 0));
    }

    // Coroutine to delete all map tiles currently generated
    private IEnumerator DeleteMap(bool regenerate)
    {
        // Clear all current instances of tile generation to prevent softlocking the deletion while the map is being made
        coroutinesQueue = new List<Coroutine>();

        Debug.Log("DELETING MAP");
        while(map.childCount > 0)
        {
            Destroy(map.GetChild(0).gameObject);
            yield return new WaitForSeconds(0.001f);
        }

        coroutinesQueue = new List<Coroutine>();
        hasEnemyGate = false;
        hasEnding = false;

        if(regenerate)
        {
            Debug.Log("REGENERATING MAP");
            CreateMap();
        }
    }
}
