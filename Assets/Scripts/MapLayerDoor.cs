using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MapLayerDoor : MonoBehaviour
{
    public enum TypeOfDoor { LayerExit, SafeRoomExit}

    [SerializeField] private TypeOfDoor typeOfDoor;
    [SerializeField] private MapGenerator mapGenerator;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerMovement>() != null)
        {
            if (typeOfDoor == TypeOfDoor.LayerExit)
            {
                mapGenerator.StartDeletingMap(true, true);
            }

            else if (typeOfDoor == TypeOfDoor.SafeRoomExit)
            {
                mapGenerator.LayerCount++;
                mapGenerator.StartDeletingMap(true, false);
                if (mapGenerator.LayerCount == 3) Debug.Log("boss room!");
            }
        }
    }
}
