using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageHitbox : MonoBehaviour
{
    [SerializeField] private PlayerStats  _playerStats;

    void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.gameObject.GetComponent<Enemy>();

        if (enemy != null && enemy.Health > 0)
        {
            enemy.Damage(_playerStats._baseAttackDamage);
            Debug.Log($"hit {enemy} for {_playerStats._baseAttackDamage} damage, {enemy.Health} HP");
        }
    }
}
