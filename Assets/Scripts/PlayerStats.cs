using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private UIManager      _uiManager;
    [SerializeField] private PlayerHealth   _playerHealth;
    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private DamageHitbox   _hitbox;
    [SerializeField] private float          _baseAttackDamage;
    [SerializeField] private float          _baseAttackCooldown;
    [SerializeField] private float          _baseAttackComboDelay;
    [SerializeField] private float          _baseAttackComboTimeLimit;
    [SerializeField] private float          _baseAttackRange;
    
    private bool    _baseAttack;
    private float   _baseAttackNum;
    private float   _baseAttackLimit;
    private float   _baseAttackCooldownTimer;

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
        
        _baseAttack      = false;
        _baseAttackNum   = 1;
        _baseAttackLimit = 2;
        _baseAttackCooldownTimer = 0f;
    }

    private void CheckForBaseAttack()
    {
        if(Input.GetButton("BaseAttack"))
        {
            if(_baseAttackCooldownTimer <= 0)
            {
                _baseAttack = true;
            }

            else if(BaseAttackCooldown - _baseAttackCooldownTimer > BaseAttackComboDelay
                && BaseAttackCooldown - _baseAttackCooldownTimer < BaseAttackComboTimeLimit
                    && _baseAttackNum <= _baseAttackLimit)
            {
                _baseAttack = true;
            }
        }
    }

    
    private void Update()
    {
        if (!_playerMovement.Dead)
        {
            CheckForBaseAttack();
        }
    }

    private void FixedUpdate()
    {
        UpdateBaseAttack();
    }

    private void UpdateBaseAttack()
    {
        if(_baseAttack && BaseAttackCooldown - _baseAttackCooldownTimer >= BaseAttackComboDelay)
        {
            _baseAttack = false;

            string animation = "Attack" + _baseAttackNum;

            _playerMovement.Attacking = true;
            _baseAttackCooldownTimer = BaseAttackCooldown;

            _playerMovement.Animator.SetTrigger(animation);

            if (_baseAttackNum >= _baseAttackLimit)
                _baseAttackNum = 1;

            else _baseAttackNum++;
        }

        if(_baseAttackCooldownTimer > 0)
        {
            if (_baseAttackCooldownTimer + BaseAttackComboTimeLimit < BaseAttackCooldown  && _baseAttackNum != 1)
            {
                _baseAttackNum = 1;
                _playerMovement.Animator.SetTrigger("Idle");
                _playerMovement.Attacking = false;
            }
                
            _baseAttackCooldownTimer -= Time.fixedDeltaTime;

            if (_baseAttackCooldownTimer <= 0)
            {
                _playerMovement.Attacking = false;
            }
        }
    }
    private void ResetHitbox()
    {
        _hitbox.enemiesHit = new List<Enemy>();
    }
}
