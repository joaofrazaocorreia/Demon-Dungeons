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
    private float menuCooldown;
    private bool blessingReceived;
    public bool BlessingReceived { get => blessingReceived; set { blessingReceived = value; } }
    private List<(string, Blessing)> blessingsToChoose;

    private void Start()
    {
        pm = FindObjectOfType<PlayerMovement>();
        menuCooldown = 0f;
        blessingReceived = false;

        blessingsToChoose = blessingManager.GetRandomBlessings(3);
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
}
