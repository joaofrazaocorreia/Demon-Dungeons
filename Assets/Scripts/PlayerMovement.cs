using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private UIManager _uiManager;

    [SerializeField] private float  _forwardAcceleration;
    [SerializeField] private float  _backwardAcceleration;
    [SerializeField] private float  _strafeAcceleration;
    [SerializeField] private float  _gravityAcceleration;
    [SerializeField] private float  _jumpAcceleration;
    [SerializeField] private float  _maxForwardVelocity;
    [SerializeField] private float  _maxBackwardVelocity;
    [SerializeField] private float  _maxStrafeVelocity;
    [SerializeField] private float  _maxFallVelocity;
    [SerializeField] private float  _rotationVelocityFactor;
    [SerializeField] private int    _maxStamina;
    [SerializeField] private int    _staminaRegenRate;
    [SerializeField] private int    _sprintStaminaCost;
    [SerializeField] private int    _sprintVelocityFactor;
    [SerializeField] private int    _dashStaminaCost;
    [SerializeField] private int    _dashVelocity;
    [SerializeField] private float  _dashDuration;

    private CharacterController _controller;
    private Vector3 _acceleration;
    private Vector3 _velocity;
    private Vector3 _motion;
    private bool    _moving;
    private float   _stamina;
    private bool    _jump;
    private bool    _sprint;
    private bool    _sprintResting;
    private bool    _dash;
    private float   _dashTimer;
    private float   _sinPI4;

    private void Start()
    {
        _controller     = GetComponent<CharacterController>();
        _acceleration   = Vector3.zero;
        _velocity       = Vector3.zero;
        _motion         = Vector3.zero;
        _stamina        = _maxStamina;
        _moving         = false;
        _jump           = false;
        _sprint         = false;
        _sprintResting  = false;
        _sinPI4         = Mathf.Sin(Mathf.PI / 4);

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
        CheckForJump();
        CheckForSprint();
        CheckForSprintRest();
        CheckForDash();
    }

    private void UpdateRotation()
    {
        if (!Input.GetButton("Camera"))
        {
            float rotation = Input.GetAxis("Mouse X") * _rotationVelocityFactor;

            transform.Rotate(0f, rotation, 0f);
        }
    }

    private void CheckForJump()
    {
        if (Input.GetButtonDown("Jump") && _controller.isGrounded)
            _jump = true;
    }

    private void CheckForSprint()
    {
        _sprint = Input.GetButton("Sprint") && _controller.isGrounded && _stamina > 0f && !_sprintResting;
    }

    private void CheckForSprintRest()
    {
        if (_sprintResting && !Input.GetButton("Sprint"))
            _sprintResting = false;
    }

    private void CheckForDash()
    {
        if (Input.GetButtonDown("Dash") && _stamina > _dashStaminaCost)
            _dash = true;
    }

    private void FixedUpdate()
    {
        UpdateAcceleration();
        UpdateVelocity();
        UpdateMotion();
        RegenStamina();
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
        if (_jump)
            _acceleration.y = _jumpAcceleration;
        else
            _acceleration.y = _gravityAcceleration;
    }

    private void UpdateVelocity()
    {
        _velocity += _acceleration * Time.fixedDeltaTime;

        UpdateForwardVelocity();
        UpdateStrafeVelocity();
        UpdateVerticalVelocity();
        UpdateDash();
        UpdateSprint();
    }

    private void UpdateForwardVelocity()
    {
        if (_acceleration.z == 0f || (_acceleration.z * _velocity.z < 0f))
            _velocity.z = 0f;
        else if (_acceleration.x == 0f)
            _velocity.z = Mathf.Clamp(_velocity.z, _maxBackwardVelocity, _maxForwardVelocity);
        else
            _velocity.z = Mathf.Clamp(_velocity.z, _maxBackwardVelocity * _sinPI4, _maxForwardVelocity * _sinPI4);
    }

    private void UpdateStrafeVelocity()
    {
        if (_acceleration.x == 0f || (_acceleration.x * _velocity.x < 0f))
            _velocity.x = 0f;
        else if (_acceleration.z == 0f)
            _velocity.x = Mathf.Clamp(_velocity.x, -_maxStrafeVelocity, _maxStrafeVelocity);
        else
            _velocity.x = Mathf.Clamp(_velocity.x, -_maxStrafeVelocity * _sinPI4, _maxStrafeVelocity * _sinPI4);
    }

    private void UpdateVerticalVelocity()
    {
        if (_controller.isGrounded && !_jump)
            _velocity.y = -0.1f;
        else if (_dashTimer <= 0f)
            _velocity.y = Mathf.Max(_velocity.y, _maxFallVelocity);

        _jump = false;
    }

    private void UpdateDash()
    {
        if (_dash)
        {
            _dash       = false;
            _velocity.z = _dashVelocity;
            _dashTimer  = _dashDuration;
            DecStamina(_dashStaminaCost);
        }
        else if (_dashTimer > 0f)
        {
            _velocity.z = _dashVelocity;
            _dashTimer -= Time.fixedDeltaTime;
        }
    }

    private void UpdateSprint()
    {
        if (_sprint && _moving && _dashTimer <= 0f)
        {
            _velocity.z *= _sprintVelocityFactor;
            _velocity.x *= _sprintVelocityFactor;
            DecStamina(_sprintStaminaCost * Time.fixedDeltaTime);

            if (_stamina == 0f)
                _sprintResting = true;
        }
    }

    private void UpdateMotion()
    {
        _motion = _velocity * Time.fixedDeltaTime;

        _motion = transform.TransformVector(_motion);

        _controller.Move(_motion);

        _moving = _motion.z != 0f || _motion.x != 0f;
    }

    private void RegenStamina()
    {
        if (_stamina < _maxStamina && (!_sprint || !_moving))
            AddStamina(_staminaRegenRate * Time.fixedDeltaTime);
    }

    private void AddStamina(float amount)
    {
        _stamina = Mathf.Min(_stamina + amount, _maxStamina);

        UpdateUI();
    }

    private void DecStamina(float amount)
    {
        _stamina = Mathf.Max(_stamina - amount, 0f);

        UpdateUI();
    }

    private void UpdateUI()
    {
        _uiManager.SetStaminaFill(_stamina / _maxStamina);
    }
}
