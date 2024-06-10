using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blessing
{
    private float attack;
    private float defense;
    private float speed;
    private float maxHealth;
    private float maxStamina;
    private float attackSpeed;
    private float healthRegen;
    private float staminaRegen;
    private float stagger;
    private float staminaCost;
    private float money;
    private int upgradeTier;
    private float rarity;

    public float Attack { get => attack * upgradeTier; }
    public float Defense { get => defense * upgradeTier; }
    public float Speed { get => speed * upgradeTier; }
    public float MaxHealth { get => maxHealth * upgradeTier; }
    public float MaxStamina { get => maxStamina * upgradeTier; }
    public float AttackSpeed { get => attackSpeed * upgradeTier; }
    public float HealthRegen { get => healthRegen * upgradeTier; }
    public float StaminaRegen { get => staminaRegen * upgradeTier; }
    public float Stagger { get => stagger * upgradeTier; }
    public float StaminaCost { get => staminaCost * upgradeTier; }
    public float Money { get => money * upgradeTier; }
    public int UpgradeTier { get => upgradeTier; set{ upgradeTier = (int) Mathf.Min(value, rarity); }}
    public float Rarity { get => rarity; }

    public List<(string, float)> Stats { get => new List<(string, float)>() { ("Attack", Attack),
        ("Defense", Defense), ("Speed", Speed), ("Max Health", MaxHealth), ("Max Stamina", MaxStamina),
        ("Attack Speed", AttackSpeed), ("Health Regen", HealthRegen), ("Stamina Regen", StaminaRegen),
        ("Stagger", Stagger),("Stamina Cost", StaminaCost), ("Extra Essence", Money) }; }

    public Blessing(float rarity, float attack = 0.0f, float defense = 0.0f, float speed = 0.0f, float maxHealth = 0.0f,
        float maxStamina = 0.0f, float attackSpeed = 0.0f, float healthRegen = 0.0f, float staminaRegen = 0.0f,
            float stagger = 0.0f, float staminaCost = 0.0f, float money = 0.0f)
    {
        InitializeStats(attack, defense, speed, maxHealth, maxStamina, attackSpeed, healthRegen, staminaRegen, stagger, staminaCost, money);
        InitializeMisc(rarity);
    }

    public void InitializeStats(float atk, float def, float spe, float maxHP, float maxSta,
        float atkSpe, float HPreg, float staReg, float stag, float staCst, float mon)
    {
        attack = atk;
        defense = def;
        speed = spe;
        maxHealth = maxHP;
        maxStamina = maxSta;
        attackSpeed = atkSpe;
        healthRegen = HPreg;
        staminaRegen = staReg;
        stagger = stag;
        staminaCost = staCst;
        money = mon;
    }

    public void InitializeMisc(float rar)
    {
        upgradeTier = 1;
        rarity = rar;
    }

    public void Upgrade()
    {
        upgradeTier = (int) Mathf.Min(upgradeTier + 1, rarity);
    }
}
