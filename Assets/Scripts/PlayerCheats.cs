using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCheats : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public PlayerHealth playerHealth;
    public PlayerStats playerStats;
    public MapGenerator mapGenerator;


    // Update is called once per frame
    void Update()
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
            if(mapGenerator.CurrentEndingTile != null)
                playerMovement.MoveTo(mapGenerator.CurrentEndingTile.transform.position + new Vector3(0, 5, 0));
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            mapGenerator.LayerCount = 3;
            mapGenerator.StartDeletingMap(true, true);
        }

        // IMPORTANT
        //
        // TURN THIS INTO A CHEAT-UPGRADE LATER, INSTEAD OF A KEYBIND
        //
        // IMPORTANT
        if (Input.GetKeyDown(KeyCode.G))
        {
            playerMovement.SetMaxStamina(1000000f);
            playerMovement.AddStamina(1000000f);
            playerMovement.SpeedMultiplier = 2.5f;
            
            playerHealth.ToggleGodmode();

            playerStats.DamageMultiplier = 2.0f;
        }
    }
}
