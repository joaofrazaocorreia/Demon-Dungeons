using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Image _loadingScreen;
    [SerializeField] private TextMeshProUGUI _loadingText;
    [SerializeField] private RectTransform _healthFill;
    [SerializeField] private RectTransform _healthBG;
    [SerializeField] private RectTransform _staminaFill;
    [SerializeField] private RectTransform _staminaBG;
    [SerializeField] private Color _staminaFillDefaultColor;
    [SerializeField] private Color _staminaFillStaggerColor;
    [SerializeField] private Color _healthFillDefaultColor;
    [SerializeField] private Color _healthFillDangerColor;
    [SerializeField] private Color _healthFillInvulnerableColor;
    [SerializeField] private RectTransform _bossHealthFill;
    [SerializeField] private RectTransform _bossHealthBG;
    [SerializeField] private Color _bossHealthFillDefaultColor;
    [SerializeField] private Color _bossHealthFillVulnerableColor;

    private float _staminaFillSize;

    private void Awake()
    {
        _staminaFillSize = _staminaFill.rect.height;
    }

    public void SetStaminaFill(float ratio)
    {
        _staminaFill.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ratio * _staminaFillSize);
    }

    public void SetStaminaColor(float amount, float limit)
    {
        Color color = amount >= limit ? _staminaFillDefaultColor : _staminaFillStaggerColor;

        _staminaFill.transform.GetComponent<RawImage>().color = color;
        _staminaBG.transform.GetComponent<RawImage>().color = color * 0.75f;
    }

    public void SetHealthFill(float ratio)
    {
        _healthFill.localScale = new Vector3(1, ratio, 1);
    }

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

    public void ToggleBossHPBar()
    {
        _bossHealthBG.gameObject.SetActive(!_bossHealthBG.gameObject.activeSelf);
    }

    public void SetBossHealthFill(float ratio)
    {
        _bossHealthFill.localScale = new Vector3(1, ratio, 1);
    }

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

    public IEnumerator FadeOutScreen(Image screen)
    {
        yield return new WaitForSeconds(2.5f);


        while(screen.color.a > 0)
        {
            float alpha = Mathf.Max(screen.color.a - Time.deltaTime * 2.5f, 0f);

            screen.color = new Color(screen.color.r, screen.color.g, screen.color.b, alpha);
            yield return new WaitForSeconds(0.01f);
        }

        screen.gameObject.SetActive(false);
    }

    public IEnumerator FadeInScreen(Image screen)
    {
        screen.gameObject.SetActive(true);

        while(screen.color.a < 1)
        {
            float alpha = Mathf.Min(screen.color.a + Time.deltaTime * 5f, 1f);

            screen.color = new Color(screen.color.r, screen.color.g, screen.color.b, alpha);
            yield return new WaitForSeconds(0.01f);
        }
    }
    public IEnumerator LoadingScreenText()
    {
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
    }

    public void FadeInLoadingScreen()
    {
        if(_loadingText.transform.parent.gameObject.activeSelf)
            StartCoroutine(LoadingScreenText());

        StartCoroutine(FadeInScreen(_loadingScreen));
    }

    public void FadeOutLoadingScreen()
    {
        StartCoroutine(FadeOutScreen(_loadingScreen));
    }
}
