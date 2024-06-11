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

    private struct GameSaveData
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
        saveData.playerCurrency = playerCurrency.GetSaveData();
        saveData.mapGenerator = mapGenerator.GetSaveData();
        saveData.blessingManager = blessingManager.GetSaveData();
        saveData.safeRoomShrine = safeRoomShrine.GetSaveData();

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
            playerCurrency.LoadSaveData(saveData.playerCurrency);
            mapGenerator.LoadSaveData(saveData.mapGenerator);
            blessingManager.LoadSaveData(saveData.blessingManager);
            safeRoomShrine.LoadSaveData(saveData.safeRoomShrine);

            print("Game loaded.");
        }

        else
            print("File not found.");
    }
}