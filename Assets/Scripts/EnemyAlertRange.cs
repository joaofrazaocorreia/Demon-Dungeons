using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAlertRange : MonoBehaviour
{
    [HideInInspector] public List<Enemy> enemiesInRange;

    private void Awake()
    {
        enemiesInRange = new List<Enemy>();
    }

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

    private void OnTriggerExit(Collider other)
    {
        if(other.GetComponent<Enemy>() != null && enemiesInRange.Contains
            (other.GetComponent<Enemy>()))
        {
            enemiesInRange.Remove(other.GetComponent<Enemy>());
        }
    }
}
