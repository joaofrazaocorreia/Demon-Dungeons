using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCurrency : MonoBehaviour
{
    [SerializeField] private UIManager  uiManager;
    private int essence;
    public float EssenceMultiplier {get; set;}
    public int Essence
    { 
        get => essence;

        set
        {
            essence = Mathf.Max(value * (int) EssenceMultiplier, 0);
            UpdateUI();
        }
    }



    private void Awake()
    {
        essence = 0;
        EssenceMultiplier = 1.0f;
    }

    private void UpdateUI()
    {
        uiManager.SetEssenceText(Essence);
    }
}