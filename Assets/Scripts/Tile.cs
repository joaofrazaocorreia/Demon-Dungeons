using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private TileData tileData;
    private bool exitsSet = false;

    [HideInInspector] public TileData TileData { get => tileData; }
    [HideInInspector] public Transform exits;

    public void SetExits()
    {
        if(!exitsSet)
        {
            exits = transform.Find("Exits");
            exitsSet = true;
        }
    }
}
