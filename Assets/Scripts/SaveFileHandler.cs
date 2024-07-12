using System;
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

    void Awake()
    {
        if(!isDestroyedOnLoad)
        {
            if (FindObjectsOfType<SaveFileHandler>().Length > 1)
                Destroy(gameObject);

            isDestroyedOnLoad = true;
            DontDestroyOnLoad(this);
        }
    }

    
    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
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
 
}
