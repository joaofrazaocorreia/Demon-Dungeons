using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private UIManager  _uiManager;
    [SerializeField] private int        _maxHealth;

    private int _health;

    private void Start()
    {
        _health = _maxHealth;

        UpdateUI();
    }

    private void UpdateUI()
    {
        _uiManager.SetHealthFill((float)_health / _maxHealth);
    }

    public bool IsFullHealth()
    {
        return _health == _maxHealth;
    }

    public void Regen(int amount)
    {
        _health = Mathf.Min(_health + amount, _maxHealth);

        UpdateUI();
    }

    public void Damage(int amount)
    {
        _health = Mathf.Max(_health - amount, 0);

        UpdateUI();

        if (_health == 0)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
