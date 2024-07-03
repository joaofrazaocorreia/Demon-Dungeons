using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Class that handles the UIs in the scene.
/// </summary>
public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private TextMeshProUGUI _blessingPausedStats;
    [SerializeField] private GameObject _settingsMenu;
    [SerializeField] private Image _loadingScreen;
    [SerializeField] private TextMeshProUGUI _loadingText;
    [SerializeField] private RectTransform _healthFill;
    [SerializeField] private RectTransform _healthBG;
    [SerializeField] private RectTransform _staminaFill;
    [SerializeField] private RectTransform _staminaBG;
    [SerializeField] private TextMeshProUGUI _livesCount;
    [SerializeField] private TextMeshProUGUI _essenceCount;
    [SerializeField] private Color _staminaFillDefaultColor;
    [SerializeField] private Color _staminaFillStaggerColor;
    [SerializeField] private Color _healthFillDefaultColor;
    [SerializeField] private Color _healthFillDangerColor;
    [SerializeField] private Color _healthFillInvulnerableColor;
    [SerializeField] private RectTransform _bossHealthFill;
    [SerializeField] private RectTransform _bossHealthBG;
    [SerializeField] private Color _bossHealthFillDefaultColor;
    [SerializeField] private Color _bossHealthFillVulnerableColor;
    [SerializeField] private GameObject _blessingChoiceMenu;
    [SerializeField] private Button _blessingChoice1;
    [SerializeField] private Button _blessingChoice2;
    [SerializeField] private Button _blessingChoice3;
    [SerializeField] private GameObject _blessingUpgradeMenu;
    [SerializeField] private Button _blessingUpgradeButton;
    [SerializeField] private TextMeshProUGUI _blessingUpgradeCost;
    [SerializeField] private TextMeshProUGUI _blessingUpgradeName;
    [SerializeField] private TextMeshProUGUI _blessingUpgradeStats;
    [SerializeField] private Image _blessingUpgradeImage;
    [SerializeField] private GameObject _blessingInventoryPrefab;
    [SerializeField] private Transform _blessingsUpgradeList;
    [SerializeField] private SafeRoomShrine _safeRoomShrine;

    private PlayerCurrency _playerCurrency;
    private PlayerMovement _playerMovement;
    private BlessingManager _blessingManager;
    private SaveDataManager _saveDataManager;
    private bool loading;
    private bool paused;
    private float _staminaFillSize;
    private float originalTimeScale;

    private void Awake()
    {
        _saveDataManager = FindObjectOfType<SaveDataManager>();

        _staminaFillSize = _staminaFill.rect.height;
        loading = false;
        paused = false;
        originalTimeScale = Time.timeScale;
        _playerCurrency = FindObjectOfType<PlayerCurrency>();
        _playerMovement = FindObjectOfType<PlayerMovement>();
        _blessingManager = FindObjectOfType<BlessingManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (!paused)
        {
            Time.timeScale = 0;
            _blessingPausedStats.text = WriteStatsText(_blessingManager.PlayerBlessings);
            _pauseMenu.SetActive(true);
            _playerMovement.ShowCursor();
            paused = true;
        }

        else if (_settingsMenu.activeSelf)
            ToggleSettingsMenu();
            
        else
        {
            Time.timeScale = originalTimeScale;
            _pauseMenu.SetActive(false);
            _playerMovement.HideCursor();
            paused = false;
        }
    }

    public void ToggleSettingsMenu()
    {
        _settingsMenu.SetActive(!_settingsMenu.activeSelf);
        _pauseMenu.SetActive(!_settingsMenu.activeSelf);
    }

    public void SaveAndQuit()
    {
        _saveDataManager.SaveGameData();
        
        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// Sets the size of the Stamina fill according to the ratio of stamina spent.
    /// </summary>
    /// <param name="ratio">Current stamina divided by the max stamina value.</param>
    public void SetStaminaFill(float ratio)
    {
        _staminaFill.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ratio * _staminaFillSize);
    }

    /// <summary>
    /// Updates the color of the stamina bar if it's below the stagger limit.
    /// </summary>
    /// <param name="amount">The current amount of stamina the player has.</param>
    /// <param name="limit">The limit of stamina for the bar to change color.</param>
    public void SetStaminaColor(float amount, float limit)
    {
        Color color = amount >= limit ? _staminaFillDefaultColor : _staminaFillStaggerColor;

        _staminaFill.transform.GetComponent<RawImage>().color = color;
        _staminaBG.transform.GetComponent<RawImage>().color = color * 0.75f;
    }

    /// <summary>
    /// Sets the size of the Health fill according to the ratio of health lost.
    /// </summary>
    /// <param name="ratio">Current health divided by the max health value.</param>
    public void SetHealthFill(float ratio)
    {
        if(ratio != float.NaN)
            _healthFill.localScale = new Vector3(1, ratio, 1);
        else 
            _healthFill.localScale = new Vector3(1, 1, 1);
    }

    /// <summary>
    /// Updates the color of the health bar if it's below the danger limit or if
    /// the player is invulnerable.
    /// </summary>
    /// <param name="amount">The current amount of health the player has.</param>
    /// <param name="limit">The limit of health for the bar to change color.</param>
    /// <param name="invulnerable">Whether the player is currently immune to 
    /// damage or not.</param>
    public void SetHealthColor(float amount, float limit, bool invulnerable)
    {
        Color color;

        if (invulnerable)
            color = _healthFillInvulnerableColor;
        
        else
            color = amount >= limit ? _healthFillDefaultColor : _healthFillDangerColor;

        _healthFill.transform.GetComponent<RawImage>().color = color;
        _healthBG.transform.GetComponent<RawImage>().color = color * 0.75f;
    }

    public void SetLivesText(int lives)
    {
        _livesCount.text = $"x {lives}";
    }

    public void SetEssenceText(int essence)
    {
        _essenceCount.text = $"{essence}";
    }

    /// <summary>
    /// Enables the boss HP bar.
    /// </summary>
    public void ToggleBossHPBar()
    {
        _bossHealthBG.gameObject.SetActive(!_bossHealthBG.gameObject.activeSelf);
    }

    /// <summary>
    /// Sets the size of the fill in the Boss Health Bar according to how much 
    /// damage he has taken.
    /// </summary>
    /// <param name="ratio">Current boss HP divided by the boss's max HP value.</param>
    public void SetBossHealthFill(float ratio)
    {
        _bossHealthFill.localScale = new Vector3(1, ratio, 1);
    }

    /// <summary>
    /// Changes the boss's health bar color depending on whether it is 
    /// vulnerable to attacks or not.
    /// </summary>
    /// <param name="vulnerable"></param>
    public void SetBossHealthColor(bool vulnerable)
    {
        Color color;

        if (vulnerable)
            color = _bossHealthFillVulnerableColor;
        else
            color = _bossHealthFillDefaultColor;

        _bossHealthFill.transform.GetComponent<RawImage>().color = color;
        _bossHealthBG.transform.GetComponent<RawImage>().color = color * 0.75f;
    }

    /// <summary>
    /// Coroutine that causes the given screen to fade out and disable.
    /// </summary>
    /// <param name="screen">The screen to fade out.</param>
    /// <returns></returns>
    public IEnumerator FadeOutScreen(Image screen)
    {
        yield return new WaitForSeconds(2.5f);


        while(screen.color.a > 0)
        {
            float alpha = Mathf.Max(screen.color.a - Time.fixedDeltaTime * 2.5f, 0f);

            screen.color = new Color(screen.color.r, screen.color.g, screen.color.b, alpha);
            yield return new WaitForSeconds(0.01f);
        }

        screen.gameObject.SetActive(false);
    }

    /// <summary>
    /// Coroutine that causes the given screen to enable and fade in.
    /// </summary>
    /// <param name="screen">The screen to fade in.</param>
    /// <returns></returns>
    public IEnumerator FadeInScreen(Image screen)
    {
        screen.gameObject.SetActive(true);

        while(screen.color.a < 1)
        {
            float alpha = Mathf.Min(screen.color.a + Time.fixedDeltaTime * 5f, 1f);

            screen.color = new Color(screen.color.r, screen.color.g, screen.color.b, alpha);
            yield return new WaitForSeconds(0.01f);
        }
    }

    /// <summary>
    /// Coroutine that changes the Text on the loading screen.
    /// </summary>
    /// <returns></returns>
    public IEnumerator LoadingScreenText()
    {
        loading = true;
        int numOfDots = 0;
        while(_loadingText.transform.parent.gameObject.activeSelf)
        {
            string text = "Loading Map";
            for(int i = 0; i < numOfDots; i++)
            {
                text += " .";
            }
            _loadingText.text = text;

            if (++numOfDots > 3)
                numOfDots = 0;

            yield return new WaitForSeconds(0.5f);
        }

        loading = false;
    }

    /// <summary>
    /// Fades the loading screen into the screen.
    /// </summary>
    public void FadeInLoadingScreen()
    {
        StartCoroutine(FadeInScreen(_loadingScreen));

        if(_loadingText.transform.parent.gameObject.activeSelf && !loading)
            StartCoroutine(LoadingScreenText());
    }

    /// <summary>
    /// Fades the loading screen out of the screen.
    /// </summary>
    public void FadeOutLoadingScreen()
    {
        StartCoroutine(FadeOutScreen(_loadingScreen));
    }

    public void OpenBlessingsMenus(bool hasChosenUpgrade, List<(string, Blessing)>  blessings, SafeRoomShrine _shrine)
    {
        if(!hasChosenUpgrade)
        {
            OpenBlessingChoiceMenu(blessings, _shrine);
        }

        else
        {
            OpenBlessingUpgradeMenu();
        }
    }

    private void OpenBlessingChoiceMenu(List<(string, Blessing)>  blessings, SafeRoomShrine _shrine)
    {
        _blessingChoiceMenu.SetActive(true);
        _blessingUpgradeMenu.SetActive(false);
        _playerMovement.ShowCursor();

        List<Button> choiceButtons = new List<Button>
        {
            _blessingChoice1,
            _blessingChoice2,
            _blessingChoice3
        };

        for (int i = 0; i < choiceButtons.Count; i++)
        {
            int index = i;
            BlessingChoiceButton bData = choiceButtons[index].GetComponent<BlessingChoiceButton>();

            choiceButtons[i].onClick.RemoveAllListeners();
            choiceButtons[i].onClick.AddListener(() => _blessingManager.AddBlessing(blessings[index]));
            choiceButtons[i].onClick.AddListener(() => _blessingChoiceMenu.SetActive(false));
            choiceButtons[i].onClick.AddListener(() => OpenBlessingUpgradeMenu());
            choiceButtons[i].onClick.AddListener(() => _shrine.BlessingReceived = true);

            bData._name.text = blessings[index].Item1;
            bData._info.text = WriteStatsText(blessings[index]);
        }
    }

    private void OpenBlessingUpgradeMenu()
    {
        _blessingChoiceMenu.SetActive(false);
        _blessingUpgradeMenu.SetActive(true);
        _playerMovement.ShowCursor();

        _blessingUpgradeName.text = "";
        _blessingUpgradeStats.text = "";
        _blessingUpgradeButton.gameObject.SetActive(false);
        _blessingUpgradeImage.gameObject.SetActive(false);

        List<GameObject> toDestroy = new List<GameObject>();

        for(int i = 0; i < _blessingsUpgradeList.childCount; i++)
        {
            toDestroy.Add(_blessingsUpgradeList.GetChild(i).gameObject);
        }

        foreach (GameObject go in toDestroy)
        {
            Destroy(go);
        }


        foreach((string, Blessing) kv in _blessingManager.PlayerBlessings)
        {
            GameObject blessingButton = Instantiate(_blessingInventoryPrefab, _blessingsUpgradeList);

            TextMeshProUGUI[] texts = blessingButton.GetComponentsInChildren<TextMeshProUGUI>();

            texts[0].text = kv.Item1;
            texts[1].text = $"Tier {kv.Item2.UpgradeTier}";

            Button button = blessingButton.GetComponent<Button>();

            button.onClick.AddListener(() => _blessingUpgradeName.text = kv.Item1);
            button.onClick.AddListener(() => _blessingUpgradeStats.text = WriteStatsText(kv));
            button.onClick.AddListener(() => _blessingUpgradeButton.gameObject.SetActive(kv.Item2.UpgradeTier < kv.Item2.Rarity));
            button.onClick.AddListener(() => _blessingUpgradeCost.text = 
            $"{_blessingManager.IncrementalUpgradeCost * kv.Item2.UpgradeTier} Essence");
            button.onClick.AddListener(() => _blessingUpgradeCost.color = 
                _playerCurrency.Essence >= _blessingManager.IncrementalUpgradeCost * kv.Item2.UpgradeTier ?
                    Color.white : Color.red);

            button.onClick.AddListener(() => _blessingUpgradeButton.onClick.RemoveAllListeners());
            button.onClick.AddListener(() => _blessingUpgradeButton.onClick.AddListener(() => 
                _blessingManager.UpgradeBlessing(kv)));

            button.onClick.AddListener(() => _blessingUpgradeButton.onClick.AddListener(() => 
                texts[1].text = $"Tier {kv.Item2.UpgradeTier}"));

            button.onClick.AddListener(() => _blessingUpgradeButton.onClick.AddListener(() => 
                _blessingUpgradeButton.gameObject.SetActive(kv.Item2.UpgradeTier < kv.Item2.Rarity)));

            button.onClick.AddListener(() => _blessingUpgradeButton.onClick.AddListener(() => 
                _blessingUpgradeStats.text = WriteStatsText(kv)));

            button.onClick.AddListener(() => _blessingUpgradeButton.onClick.AddListener(() =>
                _blessingUpgradeCost.text = $"{_blessingManager.IncrementalUpgradeCost * kv.Item2.UpgradeTier} Essence"));

            button.onClick.AddListener(() => _blessingUpgradeButton.onClick.AddListener(() => 
                _blessingUpgradeCost.color = _playerCurrency.Essence >= _blessingManager.
                    IncrementalUpgradeCost * kv.Item2.UpgradeTier ? Color.white : Color.red));
        }

        _blessingsUpgradeList.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 110 / 2 * _blessingsUpgradeList.childCount);
    }

    private string WriteStatsText((string, Blessing) kv)
    {
        string text = $"Tier {kv.Item2.UpgradeTier} / {kv.Item2.Rarity} \n";
        foreach ((string, float) stat in kv.Item2.Stats)
        {
            if (stat.Item2 > 0.0f)
                text += $"+{stat.Item2 * 100}% {stat.Item1} \n";
            
            if (stat.Item2 < 0.0f)
                text += $"-{stat.Item2 * 100}% {stat.Item1} \n";
        }

        return text;
    }

    private string WriteStatsText(List<(string, Blessing)> blessingList)
    {
        Blessing reference = new Blessing(0, 0f);

        string text = $"Blessings received: {blessingList.Count} \n\n";
        for(int i = 0; i < reference.Stats.Count; i++)
        {
            float totalValue = 0.0f;

            foreach((string, Blessing) b in blessingList)
            {
                totalValue += b.Item2.Stats[i].Item2;
            }

            text += $"{100 + (totalValue * 100)}% {reference.Stats[i].Item1} ";

            if (totalValue > 0f)
                text += $" (+{totalValue * 100}%)";
            
            if (totalValue < 0f)
                text += $" (-{totalValue * 100}%)";

            text += "\n";
        }

        return text;
    }
}
