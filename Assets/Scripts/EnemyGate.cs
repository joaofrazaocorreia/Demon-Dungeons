using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyGate : MonoBehaviour
{
    [SerializeField] private int extraEnemyRequirement = 5;
    [SerializeField] private float respawnTime = 10f;
    [SerializeField] private Transform enemies;
    [SerializeField] private Transform spawnPos;

    private int deadEnemyCounter;

    private void Start()
    {
        deadEnemyCounter = 0;
    }

    public void QueueEnemyForRespawn(PlayerHealth ph, MapGenerator mg, List<Transform> wp)
    {
        if (enemies.childCount < mg.EnemyLimit)
        {
            GameObject newEnemy = mg.CurrentEnemies[Random.Range(0, mg.CurrentEnemies.Count)];
            StartCoroutine(RespawnQueue(newEnemy, ph, mg, wp));

            if (++deadEnemyCounter == extraEnemyRequirement)
            {
                deadEnemyCounter = 0;
                
                newEnemy = mg.CurrentEnemies[Random.Range(0, mg.CurrentEnemies.Count)];

                StartCoroutine(RespawnQueue(newEnemy, ph, mg, wp));
            }
        }
    }

    private IEnumerator RespawnQueue(GameObject enemy, PlayerHealth ph, MapGenerator mg, List<Transform> wp)
    {
        yield return new WaitForSeconds(respawnTime);

        Vector3 displacement = new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5));
        NavMeshHit closestHit;

        // Instantiates the enemy.
        if (NavMesh.SamplePosition(spawnPos.position + displacement + new Vector3(0, 1, 0), out closestHit, 500, 1))
        {
            GameObject newEnemy = Instantiate(enemy, closestHit.position, Quaternion.identity);

            newEnemy.transform.parent = enemies;
            newEnemy.GetComponent<Enemy>().playerHealth = ph;
            newEnemy.GetComponent<Enemy>().mapGenerator = mg;
            newEnemy.GetComponent<Enemy>().waypoints = wp;
            newEnemy.name = "Enemy " + enemies.childCount + " (R)";

            
            newEnemy.GetComponent<Enemy>().BecomeAlerted();
        }
    }
}
