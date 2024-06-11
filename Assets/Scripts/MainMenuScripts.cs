using System.IO;
using UnityEngine;

public class MainMenuScripts : MonoBehaviour
{
    public void QuitGame()
    {
        Application.Quit();
    }

    public void SetSaveFileText(int saveFileNumber)
    {
        File.Delete(Application.persistentDataPath + "/SaveFileNumber");

        File.WriteAllText(Application.persistentDataPath + "/SaveFileNumber", saveFileNumber.ToString());
    }
}
