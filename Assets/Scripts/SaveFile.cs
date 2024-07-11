using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveFile : MonoBehaviour
{
    [SerializeField] private int saveFileNumber;
    [SerializeField] private TextMeshProUGUI fileNameText;
    [SerializeField] private TextMeshProUGUI lastFloorText;
    [SerializeField] private TextMeshProUGUI floorCount;
    [SerializeField] private TextMeshProUGUI blessingCount;
    [SerializeField] private TextMeshProUGUI essenceCount;
    [SerializeField] private GameObject deleteFileButton;
    [SerializeField] private GameObject confirmationMenu;

    private float confirmationTimer;
    private MainMenuScripts mainMenuScripts;
    private string saveFilePath;

    private void Start()
    {
        mainMenuScripts = FindObjectOfType<MainMenuScripts>();
        saveFilePath = Application.persistentDataPath + "/SaveFile" + saveFileNumber.ToString();
        confirmationTimer = 0f;

        UpdateValues();
    }

    public void LoadFile()
    {
        Debug.Log("load " + saveFileNumber);
        mainMenuScripts.SetSaveFileText(saveFileNumber);
        
        SceneManager.LoadScene(1);
    }

    public void UpdateValues()
    {
        Debug.Log(File.Exists(saveFilePath));
        
        if (File.Exists(saveFilePath))
        {
            deleteFileButton.SetActive(true);

            string jsonSaveData = File.ReadAllText(saveFilePath);
            GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(jsonSaveData);

            fileNameText.text = $"File {saveFileNumber} - Dungeon {saveData.mapGenerator.dungeons}";

            lastFloorText.text = $"Floor {saveData.mapGenerator.layers}";
            floorCount.text = $"{saveData.mapGenerator.floors} Floors";
            blessingCount.text = $"{saveData.blessingManager.blessings.Count} Blessings";
            essenceCount.text = $"{saveData.playerCurrency.essence} Essence";
        }

        else
        {
            deleteFileButton.SetActive(false);

            fileNameText.text = $"File {saveFileNumber} - Empty";

            lastFloorText.text = $"Floor 0";
            floorCount.text = $"0 Floors";
            blessingCount.text = $"0 Blessings";
            essenceCount.text = $"0 Essence";
        }
    }

    public void AskToConfirmDeletion()
    {
        confirmationMenu.SetActive(true);
        confirmationTimer = 3f;
    }

    private void Update()
    {
        if(confirmationMenu.activeSelf && confirmationTimer <= 0)
        {
            confirmationMenu.SetActive(false);
        }

        else confirmationTimer -= Time.deltaTime;
    }

    public void DeleteFile(int saveFileNumber)
    {
        Debug.Log("delete " + saveFileNumber);
        confirmationMenu.SetActive(false);

        string filePath = Application.persistentDataPath + "/SaveFile" + saveFileNumber.ToString();
        File.Delete(filePath);
        
        UpdateValues();
    }

    private struct GameSaveData
    {
        public PlayerHealth.SaveData    playerHealth;
        public PlayerCurrency.SaveData  playerCurrency;
        public MapGenerator.SaveData    mapGenerator;
        public BlessingManager.SaveData   blessingManager;
    }
}
