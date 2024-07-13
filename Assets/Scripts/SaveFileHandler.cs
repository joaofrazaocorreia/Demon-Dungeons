using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveFileHandler : MonoBehaviour
{
    private bool isDestroyedOnLoad = false;
    private bool triggerLoseScreen = false;
    private bool triggerWinScreen = false;
    private int saveFileScheduledForDeletion = 0;

    public bool TriggerLoseScreen { get => triggerLoseScreen; set{ triggerLoseScreen = value; }}
    public bool TriggerWinScreen { get => triggerWinScreen; set{ triggerWinScreen = value; }}
    public int SaveFileScheduledForDeletion { get => saveFileScheduledForDeletion; set{ saveFileScheduledForDeletion = Math.Clamp(value, 0, 3); }}
    private SettingsMenu settingsMenu;
    private string saveSettingsPath;

    void Awake()
    {
        if(!isDestroyedOnLoad)
        {
            if (FindObjectsOfType<SaveFileHandler>().Length > 1)
                Destroy(gameObject);

            isDestroyedOnLoad = true;
            DontDestroyOnLoad(this);
        }

        saveSettingsPath = Application.persistentDataPath + "/Settings";
    }

    
    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        settingsMenu = FindObjectOfType<SettingsMenu>();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        settingsMenu = FindObjectOfType<SettingsMenu>();

        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            MainMenuScripts mainMenuScripts = FindObjectOfType<MainMenuScripts>();

            if (triggerLoseScreen)
            {
                mainMenuScripts.LoseScreen();
                triggerLoseScreen = false;
            }

            if (triggerWinScreen)
            {
                mainMenuScripts.WinScreen();
                triggerWinScreen = false;
            }
                
            if (saveFileScheduledForDeletion > 0)
            {
                mainMenuScripts.DeleteSaveFile(saveFileScheduledForDeletion);
                saveFileScheduledForDeletion = 0;
            }   
        }
    }

    public struct SettingsSaveData
    {
        public float masterVolume;
        public float musicVolume;
        public float charactersVolume;
        public float backgroundVolume;
    }

    public void SaveSettingsData()
    {
        SettingsSaveData saveData;

        saveData.masterVolume = settingsMenu.MasterSlider;
        saveData.musicVolume = settingsMenu.MusicSlider;
        saveData.charactersVolume = settingsMenu.CharactersSlider;
        saveData.backgroundVolume = settingsMenu.BackgroundSlider;

        string jsonSaveData = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(saveSettingsPath, jsonSaveData);

        print("Settings saved.");
    }

    public void LoadSettingsData()
    {
        if (File.Exists(saveSettingsPath))
        {
            string jsonSaveData = File.ReadAllText(saveSettingsPath);
            SettingsSaveData saveData = JsonUtility.FromJson<SettingsSaveData>(jsonSaveData);

            settingsMenu.LoadSliderValues(saveData.masterVolume, saveData.musicVolume,
                saveData.charactersVolume, saveData.backgroundVolume);

            print("Settings loaded.");
        }

        else
            print("File not found.");
    }
}
