using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour
{
    [SerializeField] private GameObject essenceDrop;
    [SerializeField] private GameObject healthDrop;
    [SerializeField] private GameObject lifeDrop;
    [SerializeField] private GameObject model;
    [SerializeField] private GameObject particles;
    [HideInInspector] public bool hasBeenHit;
    public int minValue = 10;
    public int maxValue = 25;
    public int minHealth = 20;
    public int maxHealth = 40;
    public int livesAmount = 1;
    public Transform drops;

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
            Vector3 displacement;

            GameObject newDrop;

            if(Random.Range(0, 100) < 5)
            {
                displacement = new Vector3(Random.Range(-0.5f, 0.5f), 0, Random.Range(-0.5f, 0.5f));

                newDrop = Instantiate(lifeDrop, pos + displacement, Quaternion.identity);
                newDrop.GetComponent<LifeDrop>().Count = livesAmount;
                newDrop.transform.parent = drops;
            }

            if(Random.Range(0, 100) < 20)
            {
                displacement = new Vector3(Random.Range(-0.5f, 0.5f), 0, Random.Range(-0.5f, 0.5f));

                newDrop = Instantiate(healthDrop, pos + displacement, Quaternion.identity);
                newDrop.GetComponent<HealthDrop>().Amount = Random.Range(minHealth, maxHealth + 1);
                newDrop.transform.parent = drops;
            }

            if(Random.Range(0, 100) < 95)
            {
                displacement = new Vector3(Random.Range(-0.5f, 0.5f), 0, Random.Range(-0.5f, 0.5f));

                newDrop = Instantiate(essenceDrop, pos + displacement, Quaternion.identity);
                newDrop.GetComponent<EssenceDrop>().Value = Random.Range(minValue, maxValue + 1);
                newDrop.transform.parent = drops;
            }

            model.SetActive(false);
            particles.SetActive(true);
        }
    }
}
