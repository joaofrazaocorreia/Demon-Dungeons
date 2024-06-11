using UnityEngine;

/// <summary>
/// Class to trigger attack damage to the player when the boss attacks.
/// </summary>
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

    /// <summary>
    /// Damages the player once if he's in the hitbox and notes it.
    /// </summary>
    /// <param name="other"></param>
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
