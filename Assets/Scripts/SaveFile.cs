using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SaveFile : MonoBehaviour
{
    [SerializeField] private int saveFileNumber;
    [SerializeField] private TextMeshProUGUI fileNameText;
    [SerializeField] private TextMeshProUGUI lastFloorText;
    [SerializeField] private TextMeshProUGUI floorCount;
    [SerializeField] private TextMeshProUGUI upgradeCount;
    [SerializeField] private TextMeshProUGUI essenceCount;
    [SerializeField] private GameObject deleteFileButton;
    [SerializeField] private GameObject confirmationMenu;

    private float confirmationTimer;

    private void Start()
    {
        confirmationTimer = 0f;

        if (PlayerPrefs.GetInt("hasData" + saveFileNumber) == 0)
        {
            deleteFileButton.SetActive(false);
        }

        else
        {
            deleteFileButton.SetActive(true);
        }

        UpdateValues();
    }

    public void LoadFile()
    {
        Debug.Log("load " + saveFileNumber);
        PlayerPrefs.SetInt("CurrentSaveFile", saveFileNumber);
        SceneManager.LoadScene(1);
    }

    public void UpdateValues()
    {
        if(PlayerPrefs.GetInt("DungeonCount" + saveFileNumber) > 0)
            fileNameText.text = $"File {saveFileNumber} - Dungeon {PlayerPrefs.GetInt("DungeonCount" + saveFileNumber)}";
        
        else
            fileNameText.text = $"File {saveFileNumber} - Empty";

        lastFloorText.text = $"Floor {PlayerPrefs.GetInt("FloorCount" + saveFileNumber)}";
        floorCount.text = $"{PlayerPrefs.GetInt("LayerCount" + saveFileNumber)} Floors";
        upgradeCount.text = $"{PlayerPrefs.GetInt("UpgradeCount" + saveFileNumber)} Upgrades";
        essenceCount.text = $"{PlayerPrefs.GetInt("EssenceCount" + saveFileNumber)} Essence";
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

    public void DeleteFile()
    {
        Debug.Log("delete " + saveFileNumber);
        //PlayerPrefs.SetInt("hasData" + saveFileNumber, 0);
    }
}
