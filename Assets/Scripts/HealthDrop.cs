using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthDrop : Drop
{
    private int healthAmount;
    public int Amount { get => healthAmount; set{ healthAmount = Mathf.Max(value, 0); }}

    private void Start()
    {
        type = Type.Life;
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

        if (playerHealth && playerHealth.Health != playerHealth.MaxHealth)
        {
            playerHealth.Regen(healthAmount);
            Debug.Log("Health: " + healthAmount);
            
            Destroy(gameObject);
        }
    }
}
