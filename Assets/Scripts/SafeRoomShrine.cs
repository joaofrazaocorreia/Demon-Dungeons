using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class to trigger the events of healing and giving upgrades when the player touches the shrines.
/// </summary>
public class SafeRoomShrine : MonoBehaviour
{
    [SerializeField] private GameObject chooseBlessingMenu;
    [SerializeField] private GameObject upgradeBlessingMenu;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private BlessingManager blessingManager;

    private PlayerMovement pm;
    private SaveDataManager saveDataManager;
    private float menuCooldown;
    private bool blessingReceived;
    public bool BlessingReceived { get => blessingReceived; set { blessingReceived = value; } }
    private List<(string, Blessing)> blessingsToChoose;

    private void Start()
    {
        pm = FindObjectOfType<PlayerMovement>();
        saveDataManager = FindObjectOfType<SaveDataManager>();
        saveDataManager.SetCurrentSafeRoom(this);

        if (saveDataManager.GetSavedGameData().safeRoomShrine.gotBlessing)
            GotBlessing();
        else AllowNewBlessing();

        menuCooldown = 0f;

        blessingsToChoose = blessingManager.GetRandomBlessings(3, true);
    }

    private void Update()
    {
        if (menuCooldown > 0)
            menuCooldown -= Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerHealth ph = other.gameObject.GetComponent<PlayerHealth>();
        if(ph != null)
        {
            ph.Regen(ph.MaxHealth);
        }

        if(menuCooldown <= 0 && !upgradeBlessingMenu.activeSelf && !chooseBlessingMenu.activeSelf)
        {
            if(pm != null)
                pm.ShowCursor();

            uiManager.OpenBlessingsMenus(blessingReceived, blessingsToChoose, this);
            menuCooldown = 2f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(chooseBlessingMenu.activeSelf)
            chooseBlessingMenu.SetActive(false);

        if(upgradeBlessingMenu.activeSelf)
            upgradeBlessingMenu.SetActive(false);

        if(pm != null)
            pm.HideCursor();
    }

    public void GotBlessing()
    {
        blessingReceived = true;
        Debug.Log("Got blessing");
    }

    public void AllowNewBlessing()
    {
        blessingReceived = false;
        Debug.Log("New blessing available");
    }

    private void OnDestroy()
    {
        AllowNewBlessing();
    }

    [System.Serializable]
    public struct SaveData
    {
        public bool gotBlessing;
    }

    public SaveData GetSaveData()
    {
        SaveData saveData;

        saveData.gotBlessing = blessingReceived;

        return saveData;
    }

    public void LoadSaveData(SaveData saveData)
    {
        blessingReceived = saveData.gotBlessing;
    }
}
