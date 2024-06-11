using UnityEngine;

public class PlayerCurrency : MonoBehaviour
{
    [SerializeField] private UIManager  uiManager;
    
    private SaveDataManager saveDataManager;
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
        saveDataManager = FindObjectOfType<SaveDataManager>();

        essence = 0;
        EssenceMultiplier = 1.0f;
    }

    private void UpdateUI()
    {
        uiManager.SetEssenceText(Essence);
    }


    [System.Serializable]
    public struct SaveData
    {
        public int essence;
    }

    public SaveData GetSaveData()
    {
        SaveData saveData;

        saveData.essence = Essence;

        return saveData;
    }

    public void LoadSaveData(SaveData saveData)
    {
        Essence = saveData.essence;
    }
}
