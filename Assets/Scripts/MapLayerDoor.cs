using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Class to trigger layer progression when the player reaches an exit.
/// </summary>
public class MapLayerDoor : MonoBehaviour
{
    public enum TypeOfDoor { LayerExit, SafeRoomExit}

    [SerializeField] private TypeOfDoor typeOfDoor;
    private MapGenerator mapGenerator;

    private void Start()
    {
        mapGenerator = GameObject.Find("World").GetComponent<MapGenerator>();
    }
    
    /// <summary>
    /// Generates the appropriate type of map when the player passes an exit.
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerMovement>() != null)
        {
            other.GetComponent<PlayerMovement>().MoveTo(new Vector3(0, 20, 0));
            
            if (typeOfDoor == TypeOfDoor.LayerExit)
            {
                mapGenerator.StartDeletingMap(true, true);
            }

            else if (typeOfDoor == TypeOfDoor.SafeRoomExit)
            {
                mapGenerator.LayerCount++;
                mapGenerator.StartDeletingMap(true, false);
            }
        }
    }
}
