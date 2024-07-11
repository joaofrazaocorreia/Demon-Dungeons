using System.IO;
using UnityEngine;

public class MainMenuScripts : MonoBehaviour
{
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject loseScreen;
    [SerializeField] private SaveFile[] saveFiles;

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SetSaveFileText(int saveFileNumber)
    {
        File.Delete(Application.persistentDataPath + "/SaveFileNumber");

        File.WriteAllText(Application.persistentDataPath + "/SaveFileNumber", saveFileNumber.ToString());
    }

    public void WinScreen()
    {
        winScreen.SetActive(true);
    }

    public void LoseScreen()
    {
        loseScreen.SetActive(true);
    }

    public void DeleteSaveFile(int index)
    {
        saveFiles[index].DeleteFile(index);
    }
}
