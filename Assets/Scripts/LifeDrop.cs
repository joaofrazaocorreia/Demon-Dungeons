using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeDrop : Drop
{
    private int lifesCount;
    public int Count { get => lifesCount; set{ lifesCount = Mathf.Max(value, 0); }}

    private void Start()
    {
        type = Type.Life;
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

        if (playerHealth)
        {
            playerHealth.Lives += lifesCount;
            
            Destroy(gameObject);
        }
    }
}
