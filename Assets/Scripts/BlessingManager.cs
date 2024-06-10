using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BlessingManager : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerAttacks playerAttacks;
    [SerializeField] private PlayerCurrency playerCurrency;
    [SerializeField] private int incrementalUpgradeCost;

    private Dictionary<string, Blessing> blessingsList;
    private List<(string, Blessing)> playerBlessings;
    public List<(string, Blessing)> PlayerBlessings { get => playerBlessings; }
    public int IncrementalUpgradeCost { get => incrementalUpgradeCost; }

    private void Start()
    {
        blessingsList = new Dictionary<string, Blessing>()
        {
            {"Instant Strength", new Blessing(1f, attack: 0.2f)},
            {"Instant Sturdy", new Blessing(1f, defense: 0.2f)},
            {"Instant Swiftness", new Blessing(1f, speed: 0.2f)},
            {"Instant Vitality", new Blessing(1f, maxHealth: 0.2f)},
            {"Instant Endurance", new Blessing(1f, maxStamina: 0.2f)},
            {"Instant Regeneration", new Blessing(1f, healthRegen: 0.15f)},
            {"Instant Energy", new Blessing(1f, staminaRegen: 0.15f)},
            {"Instant Conviction", new Blessing(1f, stagger: 0.2f)},
            {"Instant Wealth", new Blessing(1f, money: 0.2f)},

            {"Lesser Strength", new Blessing(3f, attack: 0.1f)},
            {"Lesser Sturdy", new Blessing(3f, defense: 0.1f)},
            {"Lesser Swiftness", new Blessing(3f, speed: 0.1f)},
            {"Lesser Vitality", new Blessing(3f, maxHealth: 0.1f)},
            {"Lesser Endurance", new Blessing(3f, maxStamina: 0.1f)},
            {"Lesser Handling", new Blessing(3f, attackSpeed: 0.1f)},
            {"Lesser Regeneration", new Blessing(3f, healthRegen: 0.05f)},
            {"Lesser Energy", new Blessing(3f, staminaRegen: 0.05f)},
            {"Lesser Conviction", new Blessing(3f, stagger: 0.05f)},
            {"Lesser Ability", new Blessing(3f, staminaCost: -0.1f)},
            {"Lesser Wealth", new Blessing(3f, money: 0.1f)},

            {"Enhanced Strength", new Blessing(5f, attack: 0.15f, maxStamina: -2, maxHealth: -2)},
            {"Enhanced Sturdy", new Blessing(5f, defense: 0.15f, attack: -0.05f, speed: -0.05f)},
            {"Enhanced Swiftness", new Blessing(5f, attack: 0.15f, defense: -0.05f, maxHealth: -2)},

            {"Greater Strength", new Blessing(7f, attack: 0.25f, maxStamina: -5, maxHealth: -5, attackSpeed: -0.10f)},

            {"GodMode", new Blessing(0f, attack: 100f, maxStamina: 100000f, maxHealth: 100000f)},
        };

        playerBlessings = new List<(string, Blessing)>();
    }

    public void AddBlessing((string, Blessing) kv)
    {
        playerBlessings.Add(kv);
        UpdateValues();
    }

    public void AddSpecificBlessing(string name)
    {
        if(blessingsList[name] != null)
            AddBlessing((name, blessingsList[name]));
    }

    public List<(string, Blessing)> GetRandomBlessings(int amount, bool differentFromOwned = false)
    {
        List<(string, Blessing)> chosenBlessings = new List<(string, Blessing)>();
        List<(string, Blessing)> blessingsToCopy = new List<(string, Blessing)>();

        for(int i = 0; i < amount; i++)
        {
            List<(string, Blessing)> blessings = new List<(string, Blessing)>();
            int roll = Random.Range(1, 11);

            foreach(KeyValuePair<string, Blessing> kv in blessingsList)
            {
                if (kv.Value.Rarity <= roll && kv.Value.Rarity > 0 && !blessingsToCopy.Contains((kv.Key, kv.Value)) && (differentFromOwned && !playerBlessings.Contains((kv.Key, kv.Value)) || !differentFromOwned))
                {
                    blessings.Add((kv.Key, kv.Value));
                }
            }

            if (blessings.Count > 0){
                blessingsToCopy.Add(blessings[Random.Range(0, blessings.Count)]);}

            else
                blessingsToCopy.Add(("Null", new Blessing(0f)));
        }


        foreach((string, Blessing) kv in blessingsToCopy)
        {
            chosenBlessings.Add((kv.Item1, new Blessing(kv.Item2.Rarity,
                kv.Item2.Attack, kv.Item2.Defense, kv.Item2.Speed, kv.Item2.MaxHealth,
                kv.Item2.MaxStamina, kv.Item2.AttackSpeed, kv.Item2.HealthRegen,
                kv.Item2.StaminaRegen, kv.Item2.Stagger, kv.Item2.StaminaCost, kv.Item2.Money)));
        }

        return chosenBlessings;
    }

    public (string, Blessing) GetRandomBlessing(bool differentFromOwned = false)
    {
        return GetRandomBlessings(1, differentFromOwned)[0];
    }

    public void UpgradeBlessing((string, Blessing) kv)
    {
        if(playerCurrency.Essence >= incrementalUpgradeCost * kv.Item2.UpgradeTier)
        {
            playerCurrency.Essence -= incrementalUpgradeCost * kv.Item2.UpgradeTier;

            kv.Item2.Upgrade();

            UpdateValues();
        }
    }

    public void UpdateValues()
    {
        float atkMult = 1.0f;
        foreach ((string,Blessing) kv in playerBlessings)
        {
            atkMult += kv.Item2.Attack;
        }
        playerAttacks.DamageMultiplier = atkMult;

        
        float defMult = 1.0f;
        foreach ((string,Blessing) kv in playerBlessings)
        {
            defMult += kv.Item2.Defense;
        }
        playerHealth.DefenseMultiplier = defMult;

        
        float speMult = 1.0f;
        foreach ((string,Blessing) kv in playerBlessings)
        {
            speMult += kv.Item2.Speed;
        }
        playerMovement.SpeedMultiplier = speMult;


        float maxHPMult = playerHealth.BaseMaxHealth;
        foreach ((string,Blessing) kv in playerBlessings)
        {
            maxHPMult += kv.Item2.MaxHealth * playerHealth.BaseMaxHealth;
        }
        playerHealth.MaxHealth = maxHPMult;
        playerHealth.Regen(maxHPMult);


        float maxStaMult = playerMovement.BaseMaxStamina;
        foreach ((string,Blessing) kv in playerBlessings)
        {
            maxStaMult += kv.Item2.MaxStamina * playerMovement.BaseMaxStamina;
        }
        playerMovement.SetMaxStamina(maxStaMult);
        playerMovement.AddStamina(maxStaMult);

        
        float atkSpeMult = 1.0f;
        foreach ((string,Blessing) kv in playerBlessings)
        {
            atkSpeMult += kv.Item2.AttackSpeed;
        }
        playerAttacks.AttackSpeedMultiplier = atkSpeMult;

        
        float HPreg = 1.0f;
        foreach ((string,Blessing) kv in playerBlessings)
        {
            HPreg += kv.Item2.HealthRegen;
        }
        playerHealth.HealthRegenMultiplier = HPreg;

        
        float staReg = 1.0f;
        foreach ((string,Blessing) kv in playerBlessings)
        {
            staReg += kv.Item2.StaminaRegen;
        }
        playerMovement.StaminaRegenMultiplier = staReg;

        
        float stag = 1.0f;
        foreach ((string,Blessing) kv in playerBlessings)
        {
            stag += kv.Item2.Stagger;
        }
        playerMovement.StaggerRegenMultiplier = stag;


        float staCst = 1.0f;
        foreach ((string,Blessing) kv in playerBlessings)
        {
            staCst += kv.Item2.StaminaCost;
        }
        playerMovement.StaminaCostMultiplier = staCst;


        float mon = 1.0f;
        foreach ((string,Blessing) kv in playerBlessings)
        {
            mon += kv.Item2.Money;
        }
        playerCurrency.EssenceMultiplier = mon;

        Debug.Log($"atk {atkMult}, def {defMult}, speed {speMult}, maxHP {maxHPMult}," + 
            $" maxStamina {maxStaMult}, atkSpeed {atkSpeMult}, HPregen {HPreg}, " + 
            $"staminaRegen {staReg}, stagger {stag}, staminaCost {staCst}, money {mon}");
    }
}
