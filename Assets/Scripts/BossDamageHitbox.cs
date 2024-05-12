using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDamageHitbox : MonoBehaviour
{
    [HideInInspector] public bool hitPlayer;
    private EnemyBoss enemyBoss;

    private void Awake()
    {
        hitPlayer = false;
        enemyBoss = transform.parent.GetComponent<EnemyBoss>();

        if(GetComponent<BoxCollider>() != null)
            GetComponent<BoxCollider>().enabled = false;
        
        else GetComponent<CapsuleCollider>().enabled = false;
    }

    
    private void OnTriggerStay(Collider other)
    {
        PlayerHealth player = other.gameObject.GetComponent<PlayerHealth>();

        if (player != null && player.Health > 0 && !hitPlayer)
        {
            player.Damage(enemyBoss.CurrentAttackDamage);

            hitPlayer = true;
        }
    }

}
