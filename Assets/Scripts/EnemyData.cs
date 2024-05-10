using UnityEngine;

[CreateAssetMenu(menuName = "EnemyData", fileName = "EnemyData")]
public class EnemyData : ScriptableObject
{
    public float maxHealth;
    public float maxIdleTime;
    public float sensingRange;
    public float detectionRange;
    public float detectionAngle;
    public float attackRange;
    public float attackDamage;
    public float attackCooldown;
    public float hitStunCooldown;
    public float walkSpeed;
    public float runSpeed;
}
