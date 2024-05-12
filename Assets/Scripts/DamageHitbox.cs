using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageHitbox : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [HideInInspector] public List<Enemy> enemiesHit;
    [HideInInspector] public bool hitBoss;


    private void Awake()
    {
        enemiesHit = new List<Enemy>();
        hitBoss = false;
        GetComponent<BoxCollider>().enabled = false;
    }

    private void OnTriggerStay(Collider other)
    {
        Enemy enemy = other.gameObject.GetComponent<Enemy>();
        EnemyBoss boss = other.gameObject.GetComponent<EnemyBoss>();

        if (enemy != null && enemy.Health > 0 && !enemiesHit.Contains(enemy))
        {
            enemy.Damage(playerStats.BaseAttackDamage);
            Debug.Log($"hit {enemy.name} for {playerStats.BaseAttackDamage} damage, {enemy.Health} HP");

            enemiesHit.Add(enemy);
        }

        else if (boss != null && boss.Health > 0 && !hitBoss)
        {
            boss.Damage(playerStats.BaseAttackDamage);
            Debug.Log($"hit {boss.name} for {playerStats.BaseAttackDamage} damage, {boss.Health} HP");

            hitBoss = true;
        }
    }
}