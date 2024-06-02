using UnityEngine;

/// <summary>
/// Defines the Scriptable Object to store the data and properties of every Enemy.
/// </summary>
[CreateAssetMenu(menuName = "EnemyData", fileName = "EnemyData")]
public class EnemyData : ScriptableObject
{
    public float maxHealth;
    public float maxIdleTime;
    public float sensingRange;
    public float alertRange;
    public float detectionRange;
    public float detectionAngle;
    public float attackRange;
    public float attackDamage;
    public float attackCooldown;
    public float hitStunCooldown;
    public float walkSpeed;
    public float runSpeed;
    public GameObject drop;
    public float dropRate;
}
