using UnityEngine;

public class EssenceDrop : Drop
{
    private int essenceValue;
    public int Value { get => essenceValue; set{ essenceValue = Mathf.Max(value, 0); }}
    [SerializeField] private AudioClip pickupSound;

    private void Start()
    {
        type = Type.Essence;
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerCurrency playerCurrency = other.GetComponent<PlayerCurrency>();

        if (playerCurrency)
        {
            playerCurrency.Essence += essenceValue;
            GetComponent<AudioSource>().PlayOneShot(pickupSound);
            
            Destroy(gameObject);
        }
    }
}
