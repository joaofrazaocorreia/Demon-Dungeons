using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlessingManager : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerAttacks playerAttacks;
    [SerializeField] private PlayerCurrency playerCurrency;
    [SerializeField] private int incrementalUpgradeCost = 125;
    [SerializeField] private int maxTier = 10;
    [SerializeField] private Sprite[] sprites;

    private Dictionary<string, Blessing> blessingsList;
    private List<(string, Blessing)> playerBlessings;
    public List<(string, Blessing)> PlayerBlessings { get => playerBlessings; }
    public int IncrementalUpgradeCost { get => incrementalUpgradeCost; }
    public int TotalBlessingCount { get => blessingsList.Count; }
    public int MaxUpgradeTier { get => maxTier; }

    private void Start()
    {
        blessingsList = new Dictionary<string, Blessing>()
        {
            {"Instant Strength", new Blessing(1, 1f, sprites[0], attack: 0.2f)},
            {"Instant Sturdy", new Blessing(2, 1f, sprites[1], defense: 0.2f)},
            {"Instant Swiftness", new Blessing(3, 1f, sprites[2], speed: 0.2f)},
            {"Instant Vitality", new Blessing(4, 1f, sprites[1], maxHealth: 0.2f)},
            {"Instant Endurance", new Blessing(5, 1f, sprites[1], maxStamina: 0.2f)},
            {"Instant Regeneration", new Blessing(6, 1f, sprites[1], healthRegen: 0.15f)},
            {"Instant Energy", new Blessing(7, 1f, sprites[2], staminaRegen: 0.15f)},
            {"Instant Conviction", new Blessing(8, 1f, sprites[2], stagger: 0.2f)},
            {"Instant Wealth", new Blessing(9, 1f, sprites[3], money: 0.2f)},

            {"Weak Strength", new Blessing(10, 3f, sprites[0], attack: 0.1f)},
            {"Weak Sturdy", new Blessing(11, 3f, sprites[1], defense: 0.1f)},
            {"Weak Swiftness", new Blessing(12, 3f, sprites[2], speed: 0.1f)},
            {"Weak Vitality", new Blessing(13, 3f, sprites[1], maxHealth: 0.1f)},
            {"Weak Endurance", new Blessing(14, 3f, sprites[1], maxStamina: 0.1f)},
            {"Weak Handling", new Blessing(15, 3f, sprites[0], attackSpeed: 0.1f)},
            {"Weak Regeneration", new Blessing(16, 3f, sprites[1], healthRegen: 0.05f)},
            {"Weak Energy", new Blessing(17, 3f, sprites[2], staminaRegen: 0.05f)},
            {"Weak Conviction", new Blessing(18, 3f, sprites[2], stagger: 0.05f)},
            {"Weak Ability", new Blessing(19, 3f, sprites[2], staminaCost: -0.1f)},
            {"Weak Wealth", new Blessing(20, 3f, sprites[3], money: 0.1f)},

            {"Enhanced Strength", new Blessing(21, 5f, sprites[0], attack: 0.15f, maxStamina: -0.08f, maxHealth: -0.08f)},
            {"Enhanced Sturdy", new Blessing(22, 5f, sprites[1], defense: 0.15f, attack: -0.08f, speed: -0.08f)},
            {"Enhanced Swiftness", new Blessing(23, 5f, sprites[2], speed: 0.15f, defense: -0.08f, maxHealth: -0.08f)},

            {"Great Strength", new Blessing(24, 7f, sprites[0], attack: 0.3f, maxStamina: -0.15f, maxHealth: -0.15f, stagger: -0.08f)},
            {"Great Sturdy", new Blessing(25, 7f, sprites[1], defense: 0.3f, attack: -0.15f, speed: -0.15f, attackSpeed: -0.08f)},
            {"Great Swiftness", new Blessing(26, 7f, sprites[2], speed: 0.3f, defense: -0.15f, maxHealth: -0.15f, healthRegen: -0.08f)},

            {"GodMode", new Blessing(0, 0f, sprites[3], attack: 100f, maxStamina: 100000f, maxHealth: 100000f)},
        };

        playerBlessings = new List<(string, Blessing)>();
    }

    public void AddBlessing((string, Blessing) kv, int tier = 1)
    {
        playerBlessings.Add((kv.Item1, kv.Item2));
        playerBlessings[playerBlessings.Count-1].Item2.UpgradeTier = tier;

        UpdateValues();
    }

    public void AddSpecificBlessing(string name, int tier = 1)
    {
        if(blessingsList[name] != null)
            AddBlessing((name, blessingsList[name]), tier);
    }

    public void AddSpecificBlessing(int ID, int tier = 1)
    {
        foreach(KeyValuePair<string, Blessing> kv in blessingsList)
        {
            if (kv.Value.ID == ID)
            {
                AddBlessing((kv.Key, kv.Value), tier);
                break;
            }
        }
    }

    public void ClearPlayerBlessings()
    {
        playerBlessings = new List<(string, Blessing)>();
        UpdateValues();
    }

    public List<(string, Blessing)> GetRandomBlessings(int amount, bool differentFromOwned = false)
    {
        List<(string, Blessing)> chosenBlessings = new List<(string, Blessing)>();
        List<(string, Blessing)> blessingsToCopy = new List<(string, Blessing)>();

        for(int i = 0; i < amount; i++)
        {
            List<(string, Blessing)> blessings = new List<(string, Blessing)>();
            int roll = Random.Range(1, maxTier+1);

            foreach(KeyValuePair<string, Blessing> kv in blessingsList)
            {
                if (kv.Value.Rarity <= roll && kv.Value.Rarity > 0 && !blessingsToCopy.Contains((kv.Key, kv.Value))
                    && (differentFromOwned && !playerBlessings.Contains((kv.Key, kv.Value)) || !differentFromOwned))
                {
                    blessings.Add((kv.Key, kv.Value));
                }
            }

            if (blessings.Count > 0){
                blessingsToCopy.Add(blessings[Random.Range(0, blessings.Count)]);}

            else
                blessingsToCopy.Add(("Null", new Blessing(0, 0f, sprites[0])));
        }


        foreach((string, Blessing) kv in blessingsToCopy)
        {
            chosenBlessings.Add((kv.Item1, new Blessing(kv.Item2.ID, kv.Item2.Rarity, kv.Item2.Sprite,
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

    [System.Serializable]
    public struct SaveData
    {
        public List<int> blessings;
    }

    public SaveData GetSaveData()
    {
        SaveData saveData;

        saveData.blessings = new List<int>();

        for(int i = 0; i < playerBlessings.Count; i++)
        {
            int id = playerBlessings[i].Item2.ID;
            int tier = playerBlessings[i].Item2.UpgradeTier;

            saveData.blessings.Add((id * 1000) + tier);
        }

        return saveData;
    }

    public void LoadSaveData(SaveData saveData)
    {
        ClearPlayerBlessings();

        foreach(int i in saveData.blessings)
        {
            int id = (int) Mathf.Floor(i / 1000);
            int tier = 1;

            int inverseData = int.Parse(i.ToString().Reverse().ToList()[0].ToString());

            // tier = (int)inverseData[0] + (int)inverseData[1] * 10;
            tier = inverseData;

            Debug.Log(inverseData);

            AddSpecificBlessing(id, tier); 
        }
    }
}
