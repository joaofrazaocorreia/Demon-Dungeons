using UnityEngine;

public class HealthDrop : Drop
{
    private int healthAmount;
    public int Amount { get => healthAmount; set{ healthAmount = Mathf.Max(value, 0); }}
    [SerializeField] private AudioClip pickupSound;

    private void Start()
    {
        type = Type.Life;
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

        if (playerHealth && playerHealth.Health != playerHealth.MaxHealth)
        {
            playerHealth.Regen(healthAmount);
            GetComponent<AudioSource>().PlayOneShot(pickupSound);
            
            Destroy(gameObject);
        }
    }
}
