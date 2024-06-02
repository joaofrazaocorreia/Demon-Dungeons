using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class to trigger base attack damage to enemies when the player attacks.
/// </summary>
public class DamageHitbox : MonoBehaviour
{
    [SerializeField] private PlayerAttacks playerAttacks;
    [HideInInspector] public List<Enemy> enemiesHit;
    [HideInInspector] public bool hitBoss;


    private void Awake()
    {
        enemiesHit = new List<Enemy>();
        hitBoss = false;
        GetComponent<BoxCollider>().enabled = false;
    }

    /// <summary>
    /// Hits every enemy/boss within the collider only once until the list of hit enemies is reset
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerStay(Collider other)
    {
        Enemy enemy = other.gameObject.GetComponent<Enemy>();
        EnemyBoss boss = other.gameObject.GetComponent<EnemyBoss>();

        // Stores enemies hit in a list after damaging them
        if (enemy != null && enemy.Health > 0 && !enemiesHit.Contains(enemy))
        {
            enemy.Damage(playerAttacks.BaseAttackDamage);
            Debug.Log($"hit {enemy.name} for {playerAttacks.BaseAttackDamage} damage, {enemy.Health} HP");

            enemiesHit.Add(enemy);
        }

        // Notes that the boss was hit after damaging them
        else if (boss != null && boss.Health > 0 && !hitBoss)
        {
            boss.Damage(playerAttacks.BaseAttackDamage);
            Debug.Log($"hit {boss.name} for {playerAttacks.BaseAttackDamage} damage, {boss.Health} HP");

            hitBoss = true;
        }
    }
}