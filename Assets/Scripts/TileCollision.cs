using UnityEngine;

/// <summary>
/// Class that handles all possible scenarios of tile collision for map generation.
/// </summary>
public class TileCollision : MonoBehaviour
{
    private bool collisionDetected;

    public bool CollisionDetected { get => collisionDetected; }

    private void Awake()
    {
        collisionDetected = false;
    }

    public void OnTriggerEnter(Collider other)
    {
        collisionDetected = true;
    }

    public void OnTriggerStay(Collider other)
    {
        collisionDetected = true;
    }

    public void OnTriggerExit(Collider other)
    {
        collisionDetected = false;
    }

    public void OnCollisionEnter(Collision collision)
    {
        collisionDetected = true;
    }

    public void OnCollisionStay(Collision collision)
    {
        collisionDetected = true;
    }

    public void OnCollisionExit(Collision collision)
    {
        collisionDetected = false;
    }
}
