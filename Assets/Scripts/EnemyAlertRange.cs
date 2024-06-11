using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class to trigger and alert enemies to attack the player.
/// </summary>
public class EnemyAlertRange : MonoBehaviour
{
    [HideInInspector] public List<Enemy> enemiesInRange;

    private void Awake()
    {
        enemiesInRange = new List<Enemy>();
    }

    /// <summary>
    /// Stores all the enemies within range to alert later.
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Enemy>() != null && Physics.Linecast
            (transform.position, other.transform.position,
                out RaycastHit hitInfo) && hitInfo.collider.transform ==
                    other.transform)
        {
            enemiesInRange.Add(other.GetComponent<Enemy>());
        }
    }

    /// <summary>
    /// Removes any enemies that left the alerting range.
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if(other.GetComponent<Enemy>() != null && enemiesInRange.Contains
            (other.GetComponent<Enemy>()))
        {
            enemiesInRange.Remove(other.GetComponent<Enemy>());
        }
    }
}
