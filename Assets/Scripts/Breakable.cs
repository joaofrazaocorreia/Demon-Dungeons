using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour
{
    [SerializeField] private GameObject essenceDrop;
    [SerializeField] private GameObject model;
    [SerializeField] private GameObject particles;
    [HideInInspector] public bool hasBeenHit;
    public int dropMinValue;
    public int dropMaxValue;

    private void Start()
    {
        hasBeenHit = false;
    }
    
    public void Break()
    {
        if (!hasBeenHit)
        {
            hasBeenHit = true;

            Vector3 pos = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);

            GameObject newDrop = Instantiate(essenceDrop, pos, Quaternion.identity);
            newDrop.GetComponent<EssenceDrop>().Value = Random.Range(dropMinValue, dropMaxValue + 1);

            model.SetActive(false);
            particles.SetActive(true);
        }
    }
}
