using UnityEngine;

/// <summary>
/// Class to trigger the events of healing and giving upgrades when the player touches the shrines.
/// </summary>
public class SafeRoomShrine : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<PlayerHealth>() != null)
        {
            other.gameObject.GetComponent<PlayerHealth>().Regen(10000f);
        }
    }
}
