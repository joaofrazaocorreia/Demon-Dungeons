using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCurrency : MonoBehaviour
{
    [SerializeField] private UIManager  uiManager;
    private int essence;
    public int Essence
    { 
        get => essence;

        set
        {
            essence = Mathf.Max(value, 0);
            UpdateUI();
        }
    }

    private void Awake()
    {
        essence = 0;
    }

    private void UpdateUI()
    {
        uiManager.SetEssenceText(Essence);
    }
}
