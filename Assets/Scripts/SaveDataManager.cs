using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class SaveDataManager : MonoBehaviour
{
    private bool hasData;
    private BlessingManager blessingManager;
    private PlayerHealth playerHealth;
    private MapGenerator mapGenerator;
    private PlayerCurrency playerCurrency;

    public int SaveFileNumber { get; private set; }
    public int DungeonCount { get; set; }
    public int FloorCount { get; set; }
    public int LayerCount { get; set; }
    public int BlessingCount { get; set; }
    public int EssenceCount { get; set; }
    public int LivesCount { get; set; }

    private IEnumerator Start()
    {
        blessingManager = FindObjectOfType<BlessingManager>();
        playerHealth = FindObjectOfType<PlayerHealth>();
        mapGenerator = FindObjectOfType<MapGenerator>();
        playerCurrency = FindObjectOfType<PlayerCurrency>();

        yield return new WaitForSeconds(0.5f);


        if (PlayerPrefs.HasKey("CurrentSaveFile"))
            SaveFileNumber = PlayerPrefs.GetInt("CurrentSaveFile");
        else
            SaveFileNumber = 1;


        if (PlayerPrefs.HasKey("hasData" + SaveFileNumber))
        {
            if (PlayerPrefs.GetInt("hasData" + SaveFileNumber) != 0)
            {
                hasData = true;
            }

            else hasData = false;
        }

        else hasData = false;

        Debug.Log($"has data on this file? {hasData}");

        if(hasData)
        {
            LoadData(0);
            UpdateGameData();
        }

        else
        {
            hasData = true;

            SaveGameData();
        }
    }

    public void SaveGameData()
    {
        DungeonCount = mapGenerator.DungeonCount;
        FloorCount = mapGenerator.FloorCount;
        LayerCount = mapGenerator.LayerCount;
        BlessingCount = blessingManager.PlayerBlessings.Count;
        EssenceCount = playerCurrency.Essence;
        LivesCount = playerHealth.Lives;

        if(!mapGenerator.IsInSafeRoom)
        {
            FloorCount--;
            LayerCount--;
        }

        PlayerPrefs.SetInt("hasData", 1);
        PlayerPrefs.SetInt("DungeonCount", DungeonCount);
        PlayerPrefs.SetInt("FloorCount", FloorCount);
        PlayerPrefs.SetInt("LayerCount", LayerCount);
        PlayerPrefs.SetInt("BlessingCount", BlessingCount);
        PlayerPrefs.SetInt("EssenceCount", EssenceCount);
        PlayerPrefs.SetInt("LivesCount", LivesCount);
        
        PlayerPrefs.Save();


        SaveBlessings();
        
        PlayerPrefs.Save();
    }

    public void UpdateGameData()
    {
        mapGenerator.DungeonCount = DungeonCount;
        mapGenerator.FloorCount = FloorCount;
        mapGenerator.LayerCount = LayerCount;
        playerCurrency.Essence = EssenceCount;
        playerHealth.Lives = LivesCount;

        if(BlessingCount == 0)
        {
            blessingManager.ClearPlayerBlessings();
        }

        SaveGameData();
    }

    public void LoadData(int defaultValue)
    {
        DungeonCount = PlayerPrefs.GetInt("DungeonCount", defaultValue);
        FloorCount = PlayerPrefs.GetInt("FloorCount", defaultValue);
        LayerCount = PlayerPrefs.GetInt("LayerCount", defaultValue);
        BlessingCount = PlayerPrefs.GetInt("BlessingCount", defaultValue);
        EssenceCount = PlayerPrefs.GetInt("EssenceCount", defaultValue);
        LivesCount = PlayerPrefs.GetInt("LivesCount", defaultValue);

        LoadBlessings();
    }

    public void LoadBlessings()
    {
        blessingManager.ClearPlayerBlessings();

        for(int i = 1; i < blessingManager.TotalBlessingCount; i++)
        {
            for (int j = 0; j < blessingManager.MaxUpgradeTier; j++)
            {
                string saveName = "Blessing" + i + "Tier" + j + "File" + SaveFileNumber;
                if (PlayerPrefs.HasKey(saveName))
                {   
                    for(int k = 0; i < PlayerPrefs.GetInt(saveName); k++)
                        blessingManager.AddSpecificBlessing(i, j);
                }
            }
        }
    }

    public void SaveBlessings()
    {
        string saveName;

        for (int i = 1; i < blessingManager.TotalBlessingCount; i++)
        {
            for (int j = 0; j < blessingManager.MaxUpgradeTier; j++)
            {
                saveName = "Blessing" + i + "Tier" + j + "File" + SaveFileNumber;
                PlayerPrefs.SetInt(saveName, 0);
            }
        }

        foreach((string, Blessing) kv in blessingManager.PlayerBlessings)
        {
            saveName = "Blessing" + kv.Item2.ID + "Tier" + kv.Item2.UpgradeTier + "File" + SaveFileNumber;

            PlayerPrefs.SetInt(saveName, PlayerPrefs.GetInt(saveName) + 1);
        }

        BlessingCount = blessingManager.PlayerBlessings.Count;
        PlayerPrefs.SetInt("BlessingCount", BlessingCount);
  
        PlayerPrefs.Save();
    }

    public int CheckFloorCountData(int defaultValue)
    {
        if (PlayerPrefs.HasKey("FloorCount"))
        {
            return PlayerPrefs.GetInt("FloorCount");
        }

        else return defaultValue;
    }

    public int CheckDungeonCountData(int defaultValue)
    {
        if (PlayerPrefs.HasKey("DungeonCount"))
        {
            return PlayerPrefs.GetInt("DungeonCount");
        }

        else return defaultValue;
    }

    public int CheckLayerCountData(int defaultValue)
    {
        if (PlayerPrefs.HasKey("LayerCount"))
        {
            return PlayerPrefs.GetInt("LayerCount");
        }

        else return defaultValue;
    }

    public int CheckBlessingCountData(int defaultValue)
    {
        if (PlayerPrefs.HasKey("BlessingCount"))
        {
            return PlayerPrefs.GetInt("BlessingCount");
        }

        else return defaultValue;
    }

    public int CheckEssenceCountData(int defaultValue)
    {
        if (PlayerPrefs.HasKey("EssenceCount"))
        {
            return PlayerPrefs.GetInt("EssenceCount");
        }

        else return defaultValue;
    }

    public int CheckLivesCountData(int defaultValue)
    {
        if (PlayerPrefs.HasKey("LivesCount"))
        {
            return PlayerPrefs.GetInt("LivesCount");
        }

        else return defaultValue;
    }
}
