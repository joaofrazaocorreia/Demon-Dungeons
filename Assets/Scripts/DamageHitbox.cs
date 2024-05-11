using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageHitbox : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [HideInInspector] public List<Enemy> enemiesHit;

    private void Awake()
    {
        enemiesHit = new List<Enemy>();
        GetComponent<BoxCollider>().enabled = false;
        Debug.Log("awake");
        Debug.Log(GetComponent<BoxCollider>().enabled);
    }

    private void OnTriggerStay(Collider other)
    {
        Enemy enemy = other.gameObject.GetComponent<Enemy>();

        if (enemy != null && enemy.Health > 0 && !enemiesHit.Contains(enemy))
        {
            enemy.Damage(playerStats.BaseAttackDamage);
            Debug.Log($"hit {enemy.name} for {playerStats._baseAttackDamage} damage, {enemy.Health} HP");

            enemiesHit.Add(enemy);
        }
    }
}