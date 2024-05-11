using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private UIManager  _uiManager;
    [SerializeField] private float      _maxHealth;
    [SerializeField] private Animator   _animator;

    public float DefenseMultiplier { get; set; }
    public float HealthRegenMultiplier { get; set; }
    public float Health { get => _health; set{ _health = Mathf.Clamp(value, 0f, _maxHealth); } }

    private float _health;
    private bool _invulnerable;
    
    private bool _godmode;
    private float _invulnerabilityTimer;

    private void Start()
    {
        Health = _maxHealth;
        DefenseMultiplier = 1.0f;
        HealthRegenMultiplier = 1.0f;
        _godmode = false;

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
        _uiManager.SetHealthFill(Health / _maxHealth);
        _uiManager.SetHealthColor(Health, _maxHealth * 0.25f, _invulnerable || _godmode);
    }

    public bool IsFullHealth() => Health == _maxHealth;

    public void Regen(float amount)
    {
        Health += amount * HealthRegenMultiplier;
        UpdateUI();
    }

    public void Damage(float amount)
    {
        if (_invulnerable || Health <= 0 || _godmode)
            return;
        
        Health -= amount / DefenseMultiplier;

        UpdateUI();

        if (Health == 0)
            _animator.SetTrigger("Die");
            
        else 
            _animator.SetTrigger("Hurt");
    }

    public void Respawn()
    {
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

    public void ToggleGodmode()
    {
        _godmode = !_godmode;
        UpdateUI();
    }
}
