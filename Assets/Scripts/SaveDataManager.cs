using System.IO;
using System.Linq;
using UnityEngine;

public class SaveDataManager : MonoBehaviour
{
    private bool hasData;
    private BlessingManager blessingManager;
    private PlayerHealth playerHealth;
    private MapGenerator mapGenerator;
    private PlayerCurrency playerCurrency;
    private SafeRoomShrine safeRoomShrine;
    private SaveFileHandler saveFileHandler;
    private string saveFilePath;

    public int SaveFileNumber { get; private set; }
    public int DungeonCount { get; set; }
    public int FloorCount { get; set; }
    public int LayerCount { get; set; }
    public int BlessingCount { get; set; }
    public int EssenceCount { get; set; }
    public int LivesCount { get; set; }


    private void Start()
    {
        blessingManager = FindObjectOfType<BlessingManager>();
        playerHealth = FindObjectOfType<PlayerHealth>();
        mapGenerator = FindObjectOfType<MapGenerator>();
        playerCurrency = FindObjectOfType<PlayerCurrency>();
        safeRoomShrine = FindObjectOfType<SafeRoomShrine>();
        saveFileHandler = FindObjectOfType<SaveFileHandler>();

        switch (File.ReadLines(Application.persistentDataPath + "/SaveFileNumber").First())
        {
            case "1":
                SaveFileNumber = 1;
                break;
            case "2":
                SaveFileNumber = 2;
                break;
            case "3":
                SaveFileNumber = 3;
                break;
            default:
                SaveFileNumber = 1;
                break;
        }

        saveFilePath = Application.persistentDataPath + "/SaveFile" + SaveFileNumber;

        LoadGameData();
        SaveGameData();
    }

    public void SetCurrentSafeRoom(SafeRoomShrine s)
    {
        safeRoomShrine = s;
        
        string jsonSaveData = File.ReadAllText(saveFilePath);
        GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(jsonSaveData);

        s.LoadSaveData(saveData.safeRoomShrine);
    }

    public struct GameSaveData
    {
        public PlayerHealth.SaveData playerHealth;
        public PlayerCurrency.SaveData playerCurrency;
        public MapGenerator.SaveData mapGenerator;
        public BlessingManager.SaveData blessingManager;
        public SafeRoomShrine.SaveData safeRoomShrine;
    }

    public void SaveGameData()
    {
        GameSaveData saveData;

        saveData.playerHealth = playerHealth.GetSaveData();
        Debug.Log("saved playerhealth: " + saveData.playerHealth.lives + " lives");
        saveData.playerCurrency = playerCurrency.GetSaveData();
        Debug.Log("saved playerCurrency: " + saveData.playerCurrency.essence + " essence");
        saveData.mapGenerator = mapGenerator.GetSaveData();
        Debug.Log("saved mapGenerator: " + saveData.mapGenerator.layers + " layers");
        Debug.Log("saved mapGenerator: " + saveData.mapGenerator.floors + " floors");
        Debug.Log("saved mapGenerator: " + saveData.mapGenerator.dungeons + " dungeons");
        saveData.blessingManager = blessingManager.GetSaveData();
        Debug.Log("saved blessingManager: " + saveData.blessingManager.blessings);
        saveData.safeRoomShrine = safeRoomShrine.GetSaveData();
        Debug.Log("saved safeRoomShrine: " + saveData.safeRoomShrine.gotBlessing + " gotBlessing");

        string jsonSaveData = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(saveFilePath, jsonSaveData);

        print("Game saved.");
    }

    public void LoadGameData()
    {
        if (File.Exists(saveFilePath))
        {
            string jsonSaveData = File.ReadAllText(saveFilePath);
            GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(jsonSaveData);

            playerHealth.LoadSaveData(saveData.playerHealth);
            Debug.Log("loaded playerhealth: " + saveData.playerHealth.lives + " lives");
            playerCurrency.LoadSaveData(saveData.playerCurrency);
            Debug.Log("loaded playerCurrency: " + saveData.playerCurrency.essence + " essence");
            mapGenerator.LoadSaveData(saveData.mapGenerator);
            Debug.Log("loaded mapGenerator: " + saveData.mapGenerator.layers + " layers");
            Debug.Log("loaded mapGenerator: " + saveData.mapGenerator.floors + " floors");
            Debug.Log("loaded mapGenerator: " + saveData.mapGenerator.dungeons + " dungeons");
            blessingManager.LoadSaveData(saveData.blessingManager);
            Debug.Log("loaded blessingManager: " + saveData.blessingManager.blessings);
            safeRoomShrine.LoadSaveData(saveData.safeRoomShrine);
            Debug.Log("loaded safeRoomShrine: " + saveData.safeRoomShrine.gotBlessing + " gotBlessing");

            print("Game loaded.");
        }

        else
            print("File not found.");
    }

    public GameSaveData GetSaveGameData()
    {
        
        string jsonSaveData = File.ReadAllText(saveFilePath);
        GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(jsonSaveData);

        return saveData;
    }

    public void ScheduleSaveFileForDeletion()
    {
        saveFileHandler.SaveFileScheduledForDeletion = SaveFileNumber; 
       
    }
}