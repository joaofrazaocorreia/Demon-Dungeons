using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private UIManager    _uiManager;
    [SerializeField] private PlayerHealth _playerHealth;
    [SerializeField] private PlayerStats  _playerStats;
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

    private void HideCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

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

    private void UpdateRotation()
    {
        if (!Input.GetButton("Camera"))
        {
            float rotation = Input.GetAxis("Mouse X") * _rotationVelocityFactor;

            transform.Rotate(0f, rotation, 0f);
        }
    }

    private void CheckForSprint()
    {
        _sprint = Input.GetButton("Sprint") && _controller.isGrounded && !_sprintResting;
    }

    private void CheckForSprintRest()
    {
        if (_sprintResting && !Input.GetButton("Sprint") && _stamina >= _staggerLimit)
            _sprintResting = false;
    }

    private void CheckForRoll()
    {
        if (Input.GetButtonDown("Roll") && _stamina > _rollStaminaCost && _stamina >= _staggerLimit)
            _roll = true;
    }

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

    private void UpdateAcceleration()
    {
        UpdateForwardAcceleration();
        UpdateStrafeAcceleration();
        UpdateVerticalAcceleration();
    }

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

    private void UpdateVerticalAcceleration()
    {
        
        _acceleration.y = _gravityAcceleration;
    }

    private void UpdateVelocity()
    {
        _velocity += _acceleration * Time.fixedDeltaTime;

        UpdateForwardVelocity();
        UpdateStrafeVelocity();
        UpdateVerticalVelocity();
        UpdateRoll();
        UpdateSprint();
    }

    private void UpdateForwardVelocity()
    {
        if (_acceleration.z == 0f || (_acceleration.z * _velocity.z < 0f))
            _velocity.z = 0f;
        else if (_acceleration.x == 0f)
            _velocity.z = Mathf.Clamp(_velocity.z, _maxBackwardVelocity * SpeedMultiplier, _maxForwardVelocity * SpeedMultiplier);
        else
            _velocity.z = Mathf.Clamp(_velocity.z, _maxBackwardVelocity * _sinPI4 * SpeedMultiplier, _maxForwardVelocity * _sinPI4 * SpeedMultiplier);
    }

    private void UpdateStrafeVelocity()
    {
        if (_acceleration.x == 0f || (_acceleration.x * _velocity.x < 0f))
            _velocity.x = 0f;
        else if (_acceleration.z == 0f)
            _velocity.x = Mathf.Clamp(_velocity.x, -_maxStrafeVelocity * SpeedMultiplier, _maxStrafeVelocity * SpeedMultiplier);
        else
            _velocity.x = Mathf.Clamp(_velocity.x, -_maxStrafeVelocity * _sinPI4 * SpeedMultiplier, _maxStrafeVelocity * _sinPI4 * SpeedMultiplier);
    }

    private void UpdateVerticalVelocity()
    {
        if (_controller.isGrounded)
            _velocity.y = -0.1f;
        else if (_rollTimer <= 0f)
            _velocity.y = Mathf.Max(_velocity.y, _maxFallVelocity);
    }

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

    private void RegenStamina()
    {
        if (_stamina < _maxStamina && (!_sprint || !_moving) && _staggerTimer <= 0)
            AddStamina(_staminaRegenRate * Time.fixedDeltaTime * StaminaRegenMultiplier);
        else
            _staggerTimer = Mathf.Max(_staggerTimer - _staggerRegenRate * Time.fixedDeltaTime * StaggerRegenMultiplier, 0f);
    }

    public void AddStamina(float amount)
    {
        _stamina = Mathf.Min(_stamina + amount, _maxStamina);

        UpdateUI();
    }

    public void DecStamina(float amount)
    {
        _stamina = Mathf.Max(_stamina - amount, 0f);
        _staggerTimer = _staggerCooldown;

        UpdateUI();
    }

    private void UpdateUI()
    {
        _uiManager.SetStaminaFill(_stamina / _maxStamina);
        _uiManager.SetStaminaColor(_stamina, _staggerLimit);
    }

    public void SetMaxStamina(float value)
    {
        _maxStamina = value;
        UpdateUI();
    }

    public void SetStaggerLimit(float value)
    {
        _staggerLimit = value;
        UpdateUI();
    }

    public void MoveTo(Vector3 endPos)
    {
        GetComponent<CharacterController>().enabled = false;

        transform.position = endPos;
        
        GetComponent<CharacterController>().enabled = true;

        _controller.transform.rotation = Quaternion.identity;
    }

    private void CheckHealth()
    {
        if (_playerHealth.Health <= 0)
            _dead = true;
    }
}
