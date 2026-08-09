using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMover : MonoBehaviour
{
    #region Variables
    [Header("Movement")]
    [SerializeField] private float _maxSpeed = 6f;
    [SerializeField] private float _acceleration = 60f;   
    [SerializeField] private float _deceleration = 80f;   
    [SerializeField, Range(0f, 1f)] private float _airControlMultiplier = 0.4f; 

    [Header("Ground Check")]
    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private float _groundCheckRadius = 0.25f;
    [SerializeField] private LayerMask _groundLayer;

    private Rigidbody _rigidbody;

    public bool IsGrounded { get; private set; }

    #endregion 
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.freezeRotation = true; 
    }

    private void FixedUpdate()
    {
        //Should check ground even if the input mode is different
        CheckGround();

        if (InputManager.Instance.CurrentInputMode != InputMode.Player)
            return;

        HandlePhysicsMovement();
    }

    private void HandlePhysicsMovement()
    {
        //Get input
        Vector2 move = InputManager.Instance.PlayerMovement;
        Vector3 inputDir = new Vector3(move.x, 0f, move.y);

        if (inputDir.sqrMagnitude > 1f)
        {
            inputDir.Normalize();
        }

        Vector3 newVelocity = inputDir * _maxSpeed;

        float rateOfChange = inputDir.sqrMagnitude > 0.01f ? _acceleration : _deceleration;
        if (!IsGrounded)
        {
            rateOfChange *= _airControlMultiplier;
        }

        Vector3 currentVelocity = _rigidbody.linearVelocity;
        Vector3 horizontalVelocity = new Vector3(currentVelocity.x, 0f, currentVelocity.z);
        Vector3 newHorizontalVelocity = Vector3.MoveTowards(horizontalVelocity, newVelocity, rateOfChange * Time.fixedDeltaTime);

        _rigidbody.linearVelocity = new Vector3(newHorizontalVelocity.x, currentVelocity.y, newHorizontalVelocity.z);
    }

    private void CheckGround()
    {
        if (_groundCheckPoint == null)
        {
            IsGrounded = false;
            return;
        }

        IsGrounded = Physics.CheckSphere(_groundCheckPoint.position, _groundCheckRadius, _groundLayer, QueryTriggerInteraction.Ignore);
    }

    private void OnDrawGizmosSelected()
    {
        if (_groundCheckPoint == null) return;
        Gizmos.color = IsGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(_groundCheckPoint.position, _groundCheckRadius);
    }
}