using UnityEngine;

/// <summary>
/// Class that defines the player's movement and respective values.
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private UIManager    _uiManager;
    [SerializeField] private PlayerHealth _playerHealth;
    [SerializeField] private PlayerAttacks  _playerAttacks;
    [SerializeField] private Animator     _animator;

    [SerializeField] private float  _forwardAcceleration;
    [SerializeField] private float  _backwardAcceleration;
    [SerializeField] private float  _strafeAcceleration;
    [SerializeField] private float  _gravityAcceleration;
    [SerializeField] private float  _maxForwardVelocity;
    [SerializeField] private float  _maxBackwardVelocity;
    [SerializeField] private float  _maxStrafeVelocity;
    [SerializeField] private float  _maxFallVelocity;
    [SerializeField] private float  _rotationVelocityFactor;
    [SerializeField] private float  _maxStamina;
    [SerializeField] private float  _staminaRegenRate;
    [SerializeField] private float  _staggerLimit;
    [SerializeField] private float  _staggerCooldown;
    [SerializeField] private float  _staggerRegenRate;
    [SerializeField] private float  _sprintStaminaCost;
    [SerializeField] private float  _sprintVelocityFactor;
    [SerializeField] private float  _rollStaminaCost;
    [SerializeField] private float  _rollVelocity;
    [SerializeField] private float  _rollDuration;

    public float StaminaRegenMultiplier { get; set; }
    public float StaggerRegenMultiplier { get; set; }
    public float StaminaCostMultiplier { get; set; }
    public float SpeedMultiplier { get; set; }
    public bool Dead { get => _dead; set{ _dead = value; }}
    
    public bool Attacking { get => _attacking; set{ _attacking = value; }}
    public Animator Animator { get => _animator; }

    private CharacterController _controller;
    private Vector3 _acceleration;
    private Vector3 _velocity;
    private Vector3 _motion;
    private bool    _dead;
    private bool    _moving;
    private bool    _attacking;
    private float   _stamina;
    private float   _staggerTimer;
    private bool    _sprint;
    private bool    _sprintResting;
    private bool    _roll;
    private float   _rollTimer;
    private float   _sinPI4;

    private void Start()
    {
        _controller      = GetComponent<CharacterController>();
        _acceleration    = Vector3.zero;
        _velocity        = Vector3.zero;
        _motion          = Vector3.zero;
        _stamina         = _maxStamina;
        _staggerTimer    = 0;
        _dead            = false;
        _moving          = false;
        _attacking       = false;
        _sprint          = false;
        _sprintResting   = false;
        _sinPI4          = Mathf.Sin(Mathf.PI / 4);
        
        StaminaRegenMultiplier = 1.0f;
        StaggerRegenMultiplier = 1.0f;
        StaminaCostMultiplier = 1.0f;
        SpeedMultiplier = 1.0f;

        HideCursor();
        UpdateUI();
    }

    /// <summary>
    /// Locks the cursor on the screen's center.
    /// </summary>
    private void HideCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    /// <summary>
    /// Updates the player's camera rotation and the movement abilities if it's not dead.
    /// </summary>
    private void Update()
    {
        UpdateRotation();

        if (!_dead)
        {
            CheckForSprint();
            CheckForSprintRest();
            CheckForRoll();
        }
    }

    /// <summary>
    /// Updates the camera rotation angle.
    /// </summary>
    private void UpdateRotation()
    {
        if (!Input.GetButton("Camera"))
        {
            float rotation = Input.GetAxis("Mouse X") * _rotationVelocityFactor;

            transform.Rotate(0f, rotation, 0f);
        }
    }

    /// <summary>
    /// Checks if the player is currently sprinting.
    /// </summary>
    private void CheckForSprint()
    {
        _sprint = Input.GetButton("Sprint") && _controller.isGrounded && !_sprintResting;
    }

    /// <summary>
    /// Checks if the player is currently not using sprint. (resting and regenerating stamina)
    /// </summary>
    private void CheckForSprintRest()
    {
        if (_sprintResting && !Input.GetButton("Sprint") && _stamina >= _staggerLimit)
            _sprintResting = false;
    }

    /// <summary>
    /// Checks if the player rolled.
    /// </summary>
    private void CheckForRoll()
    {
        if (Input.GetButtonDown("Roll") && _stamina > _rollStaminaCost && _stamina >= _staggerLimit)
        {
            _roll = true;
            _animator.ResetTrigger("Hurt");
            _animator.SetTrigger("Roll");
        }
    }

    /// <summary>
    /// Updates the player's movement if it's not dead.
    /// </summary>
    private void FixedUpdate()
    {
        if (!_dead)
        { 
            CheckHealth();
            UpdateAcceleration();
            UpdateVelocity();
            UpdateMotion();
            RegenStamina();
        }
    }

    /// <summary>
    /// Updates all directional accelerations.
    /// </summary>
    private void UpdateAcceleration()
    {
        UpdateForwardAcceleration();
        UpdateStrafeAcceleration();
        UpdateVerticalAcceleration();
    }

    /// <summary>
    /// Updates acceleration moving forward or backwards.
    /// </summary>
    private void UpdateForwardAcceleration()
    {
        float forwardAxis = Input.GetAxis("Forward");

        if (forwardAxis > 0f)
            _acceleration.z = _forwardAcceleration;
        else if (forwardAxis < 0f)
            _acceleration.z = _backwardAcceleration;
        else
            _acceleration.z = 0f;
    }

    /// <summary>
    /// Updates acceleration moving sideways.
    /// </summary>
    private void UpdateStrafeAcceleration()
    {
        float strafeAxis = Input.GetAxis("Strafe");

        if (strafeAxis > 0f)
            _acceleration.x = _strafeAcceleration;
        else if (strafeAxis < 0f)
            _acceleration.x = -_strafeAcceleration;
        else
            _acceleration.x = 0f;
    }

    /// <summary>
    /// Updates vertical acceleration (gravity).
    /// </summary>
    private void UpdateVerticalAcceleration()
    {
        
        _acceleration.y = _gravityAcceleration;
    }

    /// <summary>
    /// Updates all the directional velocities and if the player is sprinting or rolling.
    /// </summary>
    private void UpdateVelocity()
    {
        _velocity += _acceleration * Time.fixedDeltaTime;

        UpdateForwardVelocity();
        UpdateStrafeVelocity();
        UpdateVerticalVelocity();
        UpdateRoll();
        UpdateSprint();
    }

    /// <summary>
    /// Updates velocity moving forward or backwards.
    /// </summary>
    private void UpdateForwardVelocity()
    {
        if (_acceleration.z == 0f || (_acceleration.z * _velocity.z < 0f))
            _velocity.z = 0f;
        else if (_acceleration.x == 0f)
            _velocity.z = Mathf.Clamp(_velocity.z, _maxBackwardVelocity * SpeedMultiplier, _maxForwardVelocity * SpeedMultiplier);
        else
            _velocity.z = Mathf.Clamp(_velocity.z, _maxBackwardVelocity * _sinPI4 * SpeedMultiplier, _maxForwardVelocity * _sinPI4 * SpeedMultiplier);
    }

    /// <summary>
    /// Updates velocity moving sideways.
    /// </summary>
    private void UpdateStrafeVelocity()
    {
        if (_acceleration.x == 0f || (_acceleration.x * _velocity.x < 0f))
            _velocity.x = 0f;
        else if (_acceleration.z == 0f)
            _velocity.x = Mathf.Clamp(_velocity.x, -_maxStrafeVelocity * SpeedMultiplier, _maxStrafeVelocity * SpeedMultiplier);
        else
            _velocity.x = Mathf.Clamp(_velocity.x, -_maxStrafeVelocity * _sinPI4 * SpeedMultiplier, _maxStrafeVelocity * _sinPI4 * SpeedMultiplier);
    }

    /// <summary>
    /// Updates velocity moving vertically (falling).
    /// </summary>
    private void UpdateVerticalVelocity()
    {
        if (_controller.isGrounded)
            _velocity.y = -0.1f;
        else if (_rollTimer <= 0f)
            _velocity.y = Mathf.Max(_velocity.y, _maxFallVelocity);
    }

    /// <summary>
    /// Triggers the roll and updates the player's movement if the roll is being used.
    /// </summary>
    private void UpdateRoll()
    {
        if (_roll)
        {
            _roll       = false;
            _playerHealth.BecomeInvulnerable(_rollDuration * 0.5f);
            _velocity.z = _rollVelocity * SpeedMultiplier;
            _rollTimer  = _rollDuration;
            DecStamina(_rollStaminaCost * StaminaCostMultiplier);
        }
        else if (_rollTimer > 0f)
        {
            _velocity.z = _rollVelocity * SpeedMultiplier;
            _rollTimer -= Time.fixedDeltaTime;

            if (_rollTimer < 0)
                _velocity.z = 0f;
        }
    }

    /// <summary>
    /// Triggers the sprint and updates the player's movement.
    /// </summary>
    private void UpdateSprint()
    {
        if (_sprint && _moving && _rollTimer <= 0f)
        {
            _velocity.z *= _sprintVelocityFactor;
            _velocity.x *= _sprintVelocityFactor;
            DecStamina(_sprintStaminaCost * Time.fixedDeltaTime * StaminaCostMultiplier);
        }

        if ((_stamina < _staggerLimit && !_sprint) || _stamina == 0f)
                _sprintResting = true;
    }

    /// <summary>
    /// Updates the amount of distance the player moves every frame.
    /// </summary>
    private void UpdateMotion()
    {
        _animator.SetFloat("Velocity", _velocity.magnitude);
        _motion = _velocity * Time.fixedDeltaTime;

        _motion = transform.TransformVector(_motion);

        if(_attacking)
            _controller.Move(_motion / 2);
        
        else
            _controller.Move(_motion);

        _moving = _motion.z != 0f || _motion.x != 0f;
    }

    /// <summary>
    /// Updates the stamina regenration when the player is resting.
    /// </summary>
    private void RegenStamina()
    {
        if (_stamina < _maxStamina && (!_sprint || !_moving) && _staggerTimer <= 0)
            AddStamina(_staminaRegenRate * Time.fixedDeltaTime * StaminaRegenMultiplier);
        else
            _staggerTimer = Mathf.Max(_staggerTimer - _staggerRegenRate * Time.fixedDeltaTime * StaggerRegenMultiplier, 0f);
    }

    /// <summary>
    /// Adds a given amount of stamina to the player.
    /// </summary>
    /// <param name="amount">The amount of stamina received.</param>
    public void AddStamina(float amount)
    {
        _stamina = Mathf.Min(_stamina + amount, _maxStamina);

        UpdateUI();
    }

    /// <summary>
    /// Removes a given amount of stamina from the player.
    /// </summary>
    /// <param name="amount">The amount of stamina spent.</param>
    public void DecStamina(float amount)
    {
        _stamina = Mathf.Max(_stamina - amount, 0f);
        _staggerTimer = _staggerCooldown;

        UpdateUI();
    }

    /// <summary>
    /// Updates the stamina bar based on the current amount of stamina.
    /// </summary>
    private void UpdateUI()
    {
        _uiManager.SetStaminaFill(_stamina / _maxStamina);
        _uiManager.SetStaminaColor(_stamina, _staggerLimit);
    }

    /// <summary>
    /// Sets the max value of the player's stamina.
    /// </summary>
    /// <param name="value">The new max value of stamina.</param>
    public void SetMaxStamina(float value)
    {
        _maxStamina = value;
        UpdateUI();
    }

    /// <summary>
    /// Sets the new minimum value of stamina at which the player needs to rest 
    /// before using any more stamina abilities.
    /// </summary>
    /// <param name="value"></param>
    public void SetStaggerLimit(float value)
    {
        _staggerLimit = value;
        UpdateUI();
    }

    /// <summary>
    /// Moves the player towards a position ignoring collision.
    /// </summary>
    /// <param name="endPos"></param>
    public void MoveTo(Vector3 endPos)
    {
        GetComponent<CharacterController>().enabled = false;

        transform.position = endPos;
        
        GetComponent<CharacterController>().enabled = true;

        _controller.transform.rotation = Quaternion.identity;
    }

    /// <summary>
    /// Checks if the player is dead.
    /// </summary>
    private void CheckHealth()
    {
        if (_playerHealth.Health <= 0)
            _dead = true;
    }
}
