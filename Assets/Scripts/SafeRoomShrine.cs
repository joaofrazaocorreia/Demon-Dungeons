using UnityEngine;

/// <summary>
/// Class to trigger the events of healing and giving upgrades when the player touches the shrines.
/// </summary>
public class SafeRoomShrine : MonoBehaviour
{
    [SerializeField] private GameObject chooseBlessingMenu;
    [SerializeField] private GameObject upgradeBlessingMenu;

    private bool blessingReceived;
    public bool BlessingReceived { get => blessingReceived; set { blessingReceived = value; } }

    private void Start()
    {
        blessingReceived = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        PlayerHealth ph = other.gameObject.GetComponent<PlayerHealth>();
        if(ph != null)
        {
            ph.Regen(ph.MaxHealth);
        }

        PlayerMovement pm = other.gameObject.GetComponent<PlayerMovement>();
        if(pm != null)
        {
            pm.ShowCursor();
        }

        chooseBlessingMenu.SetActive(!blessingReceived);
        upgradeBlessingMenu.SetActive(blessingReceived);
    }

    public void BlessingGiverUsed()
    {
        blessingReceived = true;
    }

    public void BlessingGiverReset()
    {
        blessingReceived = false;
    }
}
