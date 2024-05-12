using UnityEngine;

/// <summary>
/// Defines the Scriptable Object to store the data and properties of every Tile.
/// </summary>
[CreateAssetMenu(menuName = "TileData", fileName = "TileData")]
public class TileData : ScriptableObject
{
    public enum Type { Regular, Wall, Start, End, EnemyGate, SafeRoom, BossRoom };

    public Type                 type;
    public bool                 spawnsEnemies;
    public int                  numberID;
    
    /* ID:
        1 - 100 : regular tiles
        101 - 150 : walls
        151 - 160 : starting tiles
        161 - 170 : enemy gates
        171 - 180 : ending tiles
        181 - 190 : safe rooms
        191 - 200 : boss rooms
    */
}
