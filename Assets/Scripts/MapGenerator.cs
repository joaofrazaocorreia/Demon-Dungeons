using System.Collections;
using System.Linq;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private Tile[] tiles;
    [SerializeField] private Transform map;

    private Vector3 startingCoords;

    private void Awake()
    {
        foreach (Tile t in tiles)
            t.SetExits();

        startingCoords = new Vector3(100f, 50f, 100f);
    }


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.H))
        {
            StartCoroutine(DeleteMap());
        }

        if(Input.GetKeyDown(KeyCode.J))
        {
            CreateMap();
        }
    }

    // Creates the starting tile and begins generating the map from there
    private void CreateMap()
    {
        Tile[] startingTiles = tiles.Where(tile => tile.TileData.type == TileData.Type.Start).ToArray();
        //Tile[] startingTiles = tiles.Where(tile => tile.exits.childCount >= 4).ToArray();
        Tile start = Instantiate(startingTiles[Random.Range(0, startingTiles.Length)], parent:map, position:startingCoords, rotation:new Quaternion());

        StartGeneratingExits(start);
    }

    // Coroutine to generate the map procedurally
    private void StartGeneratingExits(Tile tile)
    {
        StartCoroutine(GenerateExits(tile));
    }

    // Randomly generates a new tile for every empty exit in every existing tile, with filters for specific tile numbers
    private IEnumerator GenerateExits(Tile tile)
    {
        while (tile.exits.childCount > 0)
        {
            Tile[] tilesToChoose;
            Tile newTile;

            // Picks a random exit from the given tile to generate the next tile
            Transform chosenExit = tile.exits.GetChild(Random.Range(0, tile.exits.childCount));

            // Different tiles can be chosen/forced depending on the amount of tiles already generated
            if (map.childCount < 20)
                tilesToChoose = tiles.Where(tile => tile.exits.childCount >= 2 && tile.TileData.type == TileData.Type.Regular).ToArray();

            else if (map.childCount == 34)
                tilesToChoose = tiles.Where(tile => tile.exits.childCount >= 3 && tile.TileData.type == TileData.Type.Regular).ToArray();

            else if (map.childCount == 35)
                tilesToChoose = tiles.Where(tile => tile.TileData.type == TileData.Type.EnemyGate).ToArray();

            else if (map.childCount == 36)
                tilesToChoose = tiles.Where(tile => tile.TileData.type == TileData.Type.End).ToArray();

            else if (map.childCount > 40)
                tilesToChoose = tiles.Where(tile => tile.exits.childCount <= 1 && tile.TileData.type == TileData.Type.Regular).ToArray();

            else
                tilesToChoose = tiles.Where(tile => tile.TileData.type == TileData.Type.Regular).ToArray();


            // Picks a random tile from the filtered ones and instantiates it
            newTile = Instantiate(tilesToChoose[Random.Range(0, tilesToChoose.Length)], parent: map, position: startingCoords, rotation: Quaternion.Euler(Vector3.zero));


            // Picks a random exit from the chosen random tile to connect to the current exit
            Transform chosenEntrance = newTile.exits.GetChild(Random.Range(0, newTile.exits.childCount));


            // Rotates the new tile to match the direction of its exit
            Quaternion pivotRotation = Quaternion.Euler(Vector3.up * (chosenEntrance.eulerAngles.y - newTile.transform.eulerAngles.y));

            newTile.transform.rotation = Quaternion.Euler(Vector3.up * chosenEntrance.eulerAngles.y);
            
            for (int i = 0; i < newTile.transform.childCount; i++)
            {
                newTile.transform.GetChild(i).rotation = Quaternion.Euler(Vector3.up * (newTile.transform.GetChild(i).eulerAngles.y - pivotRotation.eulerAngles.y));
            }
            

            // Changes the pivot of the new tile to its exit so it rotates around it
            Vector3 pivotVector = chosenEntrance.position - newTile.transform.position;
            newTile.transform.position += pivotVector;
            

            for (int i = 0; i < newTile.transform.childCount; i++)
            {
                newTile.transform.GetChild(i).position -= pivotVector;
            }
            

            // Rotates the new tile to match the inverse direction of the exit to connect
            newTile.transform.rotation = Quaternion.Euler(Vector3.up * (chosenExit.eulerAngles.y + 180));
            

            // Moves the tile to allign both exits
            Vector3 distanceVector = chosenExit.position - newTile.transform.position;
            newTile.transform.position += distanceVector;


            // Destroys both connected exits to prevent them from being connected again
            Destroy(chosenExit.gameObject);
            Destroy(chosenEntrance.gameObject);


            yield return new WaitForSeconds(0.01f);

            // Generates the exits for the new tile, to keep the loop going until there's no more exits left
            StartGeneratingExits(newTile);
        }
    }

    // Coroutine to delete all map tiles currently generated
    private IEnumerator DeleteMap()
    {
        while(map.childCount > 0)
        {
            Destroy(map.GetChild(0).gameObject);
            yield return new WaitForSeconds(0.001f);
        }
    }
}
