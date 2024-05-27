using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class to store various Player cheats to help with testing and demonstrating.
/// </summary>
public class PlayerCheats : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public PlayerHealth playerHealth;
    public PlayerStats playerStats;
    public MapGenerator mapGenerator;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

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
            if(mapGenerator.CurrentEndingTile != null)
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

            playerStats.DamageMultiplier = 100.0f;
        }
    }
}
