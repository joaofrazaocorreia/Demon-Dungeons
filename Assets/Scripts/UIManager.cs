using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class that handles the UIs in the scene.
/// </summary>
public class UIManager : MonoBehaviour
{
    [SerializeField] private BlessingManager _blessingManager;
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
    [SerializeField] private Transform _blessingsUpgradeList;

    private bool loading;

    private float _staminaFillSize;

    private void Awake()
    {
        _staminaFillSize = _staminaFill.rect.height;
        loading = false;
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
        _healthFill.localScale = new Vector3(1, ratio, 1);
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

    public void OpenBlessingChoice()
    {

    }

    public void OpenBlessingUpgrades()
    {
        
    }
}
