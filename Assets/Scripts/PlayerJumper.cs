using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerMover))]
//Cannot jump unless you can move.
public class PlayerJumper : MonoBehaviour
{
    #region Variables
    [Header("Jump")]
    [SerializeField] private float _jumpForce = 8f;
    [SerializeField] private float _coyoteTime = 0.12f;      
    [SerializeField] private float _jumpBufferTime = 0.12f;  

    [Header("Gravity")]
    [SerializeField] private float _fallGravityMultiplier = 2.5f;     
    [SerializeField] private float _lowJumpGravityMultiplier = 2f;  

    private Rigidbody _rigidbody;
    private PlayerMover _mover;

    private float _coyoteTimer;
    private float _jumpBufferTimer;
    private bool _jumpHeld;
    #endregion
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _mover = GetComponent<PlayerMover>();
    }

    private void Update()
    {
        if (InputManager.Instance.CurrentInputMode != InputMode.Player)
            return;

        _jumpHeld = InputManager.Instance.JumpHeld;

        if (InputManager.Instance.JumpPressed)
        {
            _jumpBufferTimer = _jumpBufferTime;
        }
        else
        {
            _jumpBufferTimer -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        UpdateCoyoteTime();
        TryJump();
        ApplyGravityMultiplier();
    }

    private void UpdateCoyoteTime()
    {
        if (_mover.IsGrounded)
        {
            _coyoteTimer = _coyoteTime;
        }
        else
        {
            _coyoteTimer -= Time.fixedDeltaTime;
        }
    }

    private void TryJump()
    {
        bool canJump = _coyoteTimer > 0f;
        bool wantsJump = _jumpBufferTimer > 0f;

        if (canJump && wantsJump)
        {
            Vector3 velocity = _rigidbody.linearVelocity; 
            velocity.y = 0f; 
            _rigidbody.linearVelocity = velocity;

            _rigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);

            _coyoteTimer = 0f;
            _jumpBufferTimer = 0f; 
        }
    }

    private void ApplyGravityMultiplier()
    {
        Vector3 velocity = _rigidbody.linearVelocity;

        if (velocity.y < 0f)
        {
            velocity += Vector3.up * Physics.gravity.y * (_fallGravityMultiplier - 1f) * Time.fixedDeltaTime;
        }
        else if (velocity.y > 0f && !_jumpHeld)
        {
            velocity += Vector3.up * Physics.gravity.y * (_lowJumpGravityMultiplier - 1f) * Time.fixedDeltaTime;
        }

        _rigidbody.linearVelocity = velocity;
    }
}