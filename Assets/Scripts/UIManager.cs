using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private RectTransform _healthFill;
    [SerializeField] private RectTransform _staminaFill;

    private float _staminaFillSize;
    private Color _staminaFillDefaultColor;
    private Color _staminaFillStaggerColor;

    private void Awake()
    {
        _staminaFillSize = _staminaFill.rect.height;
        _staminaFillDefaultColor = _staminaFill.transform.GetComponent<RawImage>().color;
        _staminaFillStaggerColor = Color.red;
    }

    public void SetStaminaFill(float ratio)
    {
        _staminaFill.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ratio * _staminaFillSize);
    }

    public void SetStaminaColor(float amount, float limit)
    {
        _staminaFill.transform.GetComponent<RawImage>().color = amount >= limit ?
             _staminaFillDefaultColor : _staminaFillStaggerColor;
    }

    public void SetHealthFill(float ratio)
    {
        _healthFill.localScale = new Vector3(1, ratio, 1);
    }
}
