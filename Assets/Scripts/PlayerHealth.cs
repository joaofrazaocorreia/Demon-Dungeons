using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Class that handles the player's health.
/// </summary>
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

    /// <summary>
    /// Checks whether the player is still invulnerable.
    /// </summary>
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

    /// <summary>
    /// Updates the health bar according to the player's health.
    /// </summary>
    private void UpdateUI()
    {
        _uiManager.SetHealthFill(Health / _maxHealth);
        _uiManager.SetHealthColor(Health, _maxHealth * 0.25f, _invulnerable || _godmode);
    }

    /// <summary>
    /// Checks if the player is at full health.
    /// </summary>
    /// <returns>True if full health, false otherwise.</returns>
    public bool IsFullHealth() => Health == _maxHealth;

    /// <summary>
    /// Heals the player by a given amount.
    /// </summary>
    /// <param name="amount">The amount to heal.</param>
    public void Regen(float amount)
    {
        Health += amount * HealthRegenMultiplier;
        UpdateUI();
    }

    /// <summary>
    /// Damages the player by a given amount.
    /// </summary>
    /// <param name="amount">The amount to damage.</param>
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

    /// <summary>
    /// Restarts the scene if the player dies.
    /// </summary>
    public void Respawn()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Player becomes briefly invulnerable.
    /// </summary>
    /// <param name="durationInSeconds">Duration in seconds of the invulnerability.</param>
    public void BecomeInvulnerable(float durationInSeconds)
    {
        _invulnerable = true;
        _invulnerabilityTimer = durationInSeconds;
        UpdateUI();
    }

    /// <summary>
    /// Sets the max health value.
    /// </summary>
    /// <param name="value">The value of the new max hp.</param>
    public void SetMaxHealth(float value)
    {
        _maxHealth = value;
        UpdateUI();
    }

    /// <summary>
    /// Toggles godmode.
    /// </summary>
    public void ToggleGodmode()
    {
        _godmode = !_godmode;
        UpdateUI();
    }
}
