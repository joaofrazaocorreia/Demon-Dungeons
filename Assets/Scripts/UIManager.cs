using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private RectTransform _staminaFill;

    private float _staminaFillSize;

    private void Awake()
    {
        _staminaFillSize = _staminaFill.rect.height;
    }

    public void SetStaminaFill(float ratio)
    {
        _staminaFill.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ratio * _staminaFillSize);
    }
}
