using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private UIManager  _uiManager;
    [SerializeField] private float      _maxHealth;

    public float DefenseMultiplier { get; set; }
    public float HealthRegenMultiplier { get; set; }

    private float _health;
    private bool _invulnerable;
    private float _invulnerabilityTimer;

    private void Start()
    {
        _health = _maxHealth;
        DefenseMultiplier = 1.0f;
        HealthRegenMultiplier = 1.0f;

        UpdateUI();
    }

    private void FixedUpdate()
    {
        if (_invulnerable && _invulnerabilityTimer <= 0f)
        {
            _invulnerable = false;
            UpdateUI();
        }

        else if (_invulnerable)
            _invulnerabilityTimer -= Time.deltaTime;
            
    }

    private void UpdateUI()
    {
        _uiManager.SetHealthFill(_health / _maxHealth);
        _uiManager.SetHealthColor(_health, _maxHealth * 0.25f, _invulnerable);
    }

    public bool IsFullHealth() => _health == _maxHealth;

    public void Regen(float amount)
    {
        float healAmount = amount * HealthRegenMultiplier;

        _health = Mathf.Min(_health + healAmount, _maxHealth);

        UpdateUI();
    }

    public void Damage(float amount)
    {
        if (_invulnerable)
            return;
        
        float damageAmount = amount * DefenseMultiplier;

        _health = Mathf.Max(_health - damageAmount, 0f);

        UpdateUI();

        if (_health == 0)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void BecomeInvulnerable(float durationInSeconds)
    {
        _invulnerable = true;
        _invulnerabilityTimer = durationInSeconds;
        UpdateUI();
    }

    public void SetMaxHealth(float value)
    {
        _maxHealth = value;
        UpdateUI();
    }
}
