using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private RectTransform _healthFill;
    [SerializeField] private RectTransform _healthBG;
    [SerializeField] private RectTransform _staminaFill;
    [SerializeField] private RectTransform _staminaBG;
    [SerializeField] private Color _staminaFillDefaultColor;
    [SerializeField] private Color _staminaFillStaggerColor;
    [SerializeField] private Color _healthFillDefaultColor;
    [SerializeField] private Color _healthFillDangerColor;
    [SerializeField] private Color _healthFillInvulnerableColor;

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
}
