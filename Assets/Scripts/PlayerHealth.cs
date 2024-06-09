using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Class that handles the player's health.
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private UIManager  _uiManager;
    [SerializeField] private int        _startingLives = 3;
    [SerializeField] public float       _baseMaxHealth = 100f;
    [SerializeField] private Animator   _animator;

    private float _maxHealth;

    public float DefenseMultiplier { get; set; }
    public float HealthRegenMultiplier { get; set; }
    public float BaseMaxHealth { get => _baseMaxHealth; }
    public float MaxHealth { get => _maxHealth; set{ _maxHealth = value; UpdateUI(); } }
    public int Lives
    {
        get => _lives;
        
        set
        { 
            _lives = Mathf.Clamp(value, 0, 99);
            UpdateUI();
        }  
    }

    public float Health { get => _health; set{ _health = Mathf.Clamp(value, 0f, _maxHealth); } }

    private int _lives;
    private float _health;
    private bool _invulnerable;
    
    private bool _godmode;
    private float _invulnerabilityTimer;

    private void Start()
    {
        _lives = _startingLives;
        _maxHealth = _baseMaxHealth;
        Health = _maxHealth;
        DefenseMultiplier = 1.0f;
        HealthRegenMultiplier = 1.0f;
        _godmode = false;

        UpdateUI();
    }

    /// <summary>
    /// Checks whether the player is still invulnerable and forces them to die
    /// if they're supposed to be dead.
    /// </summary>
    private void FixedUpdate()
    {
        if (Health == 0 && _animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "PlayerDeath")
            _animator.SetTrigger("Die");
        
        
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
        _uiManager.SetLivesText(Lives);
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
        if (Health == 0 && _animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "PlayerDeath")
            _animator.SetTrigger("Die");
        
        if (_invulnerable || Health <= 0 || _godmode)
            return;
        
        Health -= amount / DefenseMultiplier;

        UpdateUI();

        if (Health == 0)
             _animator.SetTrigger("Die");

        else if(amount / DefenseMultiplier > 0)
            _animator.SetTrigger("Hurt");
    }

    /// <summary>
    /// Restarts the scene if the player dies.
    /// </summary>
    public void Respawn()
    {
        if(--Lives > 0)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        
        else
            SceneManager.LoadScene(0);
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
    /// Toggles godmode.
    /// </summary>
    public void ToggleGodmode()
    {
        _godmode = !_godmode;
        UpdateUI();
    }
}
