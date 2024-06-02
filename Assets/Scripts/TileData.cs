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
    public bool                 spawnsBreakables;
    public int                  numberID;
    
    /* ID:
        1 - 50 : regular (corridor) tiles
        51 - 100 : regular (room) tiles
        101 - 150 : walls
        151 - 160 : starting tiles
        161 - 170 : enemy gates
        171 - 180 : ending tiles
        181 - 190 : safe rooms
        191 - 200 : boss rooms
    */
}
