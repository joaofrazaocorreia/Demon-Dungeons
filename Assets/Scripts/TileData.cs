using UnityEngine;

[CreateAssetMenu(menuName = "TileData", fileName = "TileData")]
public class TileData : ScriptableObject
{
    public enum Type { Regular, Start, End, EnemyGate, SafeRoom, BossRoom };

    public Type                 type;
    public bool                 spawnsEnemies;
}
