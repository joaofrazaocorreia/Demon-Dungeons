using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private UIManager      _uiManager;
    [SerializeField] private PlayerHealth   _playerHealth;
    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] public float          _baseAttackDamage;
    [SerializeField] public float          _baseAttackCooldown;
    [SerializeField] private float          _baseAttackRange;

    public float DamageMultiplier { get; set; }


    private void Start()
    {
        DamageMultiplier = 1.0f;
    }

    
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            _playerHealth.Damage(10);
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            _playerHealth.Regen(10);
        }

        if(Input.GetKeyDown(KeyCode.Y))
        {
            _playerHealth.BecomeInvulnerable(3f);
        }
    }
}
