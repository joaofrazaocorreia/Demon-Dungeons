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
    [SerializeField] public float          _baseAttackComboDelay;
    [SerializeField] public float          _baseAttackComboTimeLimit;
    [SerializeField] private float          _baseAttackRange;

    public float DamageMultiplier { get; set; }
    public float CooldownMultiplier { get; set; }
    public float ComboDelayMultiplier { get; set; }
    public float ComboTimeLimitMultiplier { get; set; }

    public float BaseAttackDamage { get => _baseAttackDamage * DamageMultiplier; set { _baseAttackDamage = value; } }
    public float BaseAttackCooldown { get => _baseAttackCooldown / CooldownMultiplier; set { _baseAttackCooldown = value; } }
    public float BaseAttackComboDelay { get => _baseAttackComboDelay / ComboDelayMultiplier; set { _baseAttackComboDelay = value; } }
    public float BaseAttackComboTimeLimit { get => _baseAttackComboTimeLimit * ComboTimeLimitMultiplier; set { _baseAttackComboTimeLimit = value; } }


    private void Start()
    {
        DamageMultiplier = 1.0f;
        CooldownMultiplier = 1.0f;
        ComboDelayMultiplier = 1.0f;
        ComboTimeLimitMultiplier = 1.0f;
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
