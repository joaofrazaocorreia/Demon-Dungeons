using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that defines Tiles and their properties.
/// </summary>
public class Tile : MonoBehaviour
{
    [SerializeField] private TileData tileData;
    private bool isSetup = false;
    private bool colliding = false;

    [HideInInspector] public TileData      TileData { get => tileData; }
    [HideInInspector] public Transform     exits;
    [HideInInspector] public Transform     model;
    [HideInInspector] public TileCollision tileCollision;
    [HideInInspector] public bool          Colliding { get => colliding; }

    public void Setup()
    {
        if(!isSetup)
        {
            exits = transform.Find("Exits");
            model = transform.Find("Model");
            tileCollision = transform.GetComponentInChildren<TileCollision>();
            isSetup = true;
        }
    }

    /// <summary>
    /// Checks if the tile is colliding with another.
    /// </summary>
    public void FixedUpdate()
    {
        if(isSetup)
            colliding = tileCollision.CollisionDetected;
    }
}
