using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EssenceDrop : MonoBehaviour
{
    private int essenceValue;
    public int Value { get => essenceValue; set{ essenceValue = Mathf.Max(value, 0); }}

    private void Start()
    {
        Value = 10;
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerCurrency playerCurrency = other.GetComponent<PlayerCurrency>();

        if (playerCurrency)
        {
            playerCurrency.Essence += essenceValue;
            
            Destroy(gameObject);
        }
    }
}
