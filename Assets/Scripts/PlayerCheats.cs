using UnityEngine;

/// <summary>
/// Class to store various Player cheats to help with testing and demonstrating.
/// </summary>
public class PlayerCheats : MonoBehaviour
{
    [SerializeField] private bool enableCheats;
    public PlayerMovement playerMovement;
    public PlayerHealth playerHealth;
    public PlayerAttacks playerAttacks;
    public PlayerCurrency playerCurrency;
    public MapGenerator mapGenerator;
    public BlessingManager blessingManager;


    // Update is called once per frame
    void Update()
    {
        if (enableCheats)
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                playerHealth.Damage(15);
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                playerHealth.Regen(15);
            }

            if (Input.GetKeyDown(KeyCode.Y))
            {
                if (mapGenerator.CurrentEndingTile != null)
                    playerMovement.MoveTo(mapGenerator.CurrentEndingTile.transform.position + new Vector3(0, 5, 0));
            }

            if (Input.GetKeyDown(KeyCode.U))
            {
                mapGenerator.LayerCount = 3;
                mapGenerator.StartDeletingMap(true, true);
            }

            // This cheat will later be turned into an Upgrade instead, to better work
            // with the multipliers.
            if (Input.GetKeyDown(KeyCode.G))
            {
                playerMovement.SetMaxStamina(1000000f);
                playerMovement.AddStamina(1000000f);
                playerMovement.SpeedMultiplier = 2.5f;

                playerHealth.ToggleGodmode();

                playerAttacks.DamageMultiplier = 100.0f;
            }

            if (Input.GetKeyDown(KeyCode.B))
            {
                playerCurrency.Essence += 10;
            }

            if (Input.GetKeyDown(KeyCode.N))
            {
                playerCurrency.Essence -= 10;
            }

            if (Input.GetKeyDown(KeyCode.M))
            {
                playerHealth.Lives++;
            }

            if (Input.GetKeyDown(KeyCode.Z))
            {
                (string, Blessing) rand = blessingManager.GetRandomBlessings(1)[0];
                blessingManager.AddBlessing(rand);

                Debug.Log("Added " + rand.Item1);
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                blessingManager.AddSpecificBlessing("Lesser Endurance");

                Debug.Log("Added Lesser Endurance");

                blessingManager.AddSpecificBlessing("Lesser Energy");

                Debug.Log("Added Lesser Energy");
            }
        }
    }
}
