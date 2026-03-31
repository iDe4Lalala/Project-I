using System;
using UnityEngine;

public class PlayerInputController : MonoBehaviour, IPlayerInput
{
    [SerializeField] private float _movementThreshold;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _minWorkableNormalY;
    public bool IsMoving { get; private set; }
    public bool IsGround { get; private set; }
    public event Action LandedGround;
    private PlayerComponents _playerComponents;
    private Rigidbody _rigidbody;
    private HumanDataBase _playerData;
    private float _sprintSpeed;
    private Vector2 _pendingRecoil;
    private int _groundContactCount;

    public void Initialize(PlayerComponents playerComponents)
    {
        _sprintSpeed = 1f;
        _pendingRecoil = Vector2.zero;

        _playerComponents = playerComponents;
        _playerData = playerComponents.HumanDataBase;
        _rigidbody = _playerComponents.Rigidbody;
    }

    public void AddRecoil(Vector2 recoil)
    {
        _pendingRecoil += recoil;
    }

    public void SetMove(Vector2 direction)
    {
        Vector3 forward = _playerComponents.Camera.transform.forward;
        Vector3 right = _playerComponents.Camera.transform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 move = 
            _playerData.MovementSpeed * direction.y * forward * _sprintSpeed + 
            _playerData.MovementSpeed * direction.x * right;
        transform.position += move * Time.fixedDeltaTime;
        
        IsMoving = direction.magnitude > _movementThreshold;
    }

    public void SetLookDelta(Vector2 delta)
    {
        Vector2 total = delta + _pendingRecoil;
        
        if (total == Vector2.zero) return;

        float verticalInput = total.x * _playerData.RotationSpeed;
        float horizontalInput = -total.y * _playerData.RotationSpeed;

        horizontalInput = 
            Mathf.Clamp(horizontalInput, _playerData.TurningMinAngle, _playerData.TurningMaxAngle);

        _playerComponents.Camera.transform.localRotation *=  Quaternion.Euler(horizontalInput, 0, 0);
        gameObject.transform.localRotation *= Quaternion.Euler(0, verticalInput, 0);

        _pendingRecoil = Vector2.zero;
    }

    public void Jump()
    {
        var velocity = _rigidbody.linearVelocity;
        velocity.y = _playerData.JumpForce;
        _rigidbody.linearVelocity = velocity;
    }

    public void StartSprint()
    {
        _sprintSpeed = _playerData.SprintSpeed;
    }

    public void StopSprint()
    {
        _sprintSpeed = 1f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & _groundLayer) == 0) return;

        for (int i = 0; i < collision.contactCount; i++)
        {
            if (collision.GetContact(i).normal.y >= _minWorkableNormalY)
            {
                _groundContactCount++;
                if (!IsGround)
                {
                    IsGround = true;
                    LandedGround?.Invoke();
                }
                return;
            }
        }
    }

    public void OnCollisionExit(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & _groundLayer) == 0) return;

        _groundContactCount = Mathf.Max(0, _groundContactCount - 1);
        if (_groundContactCount == 0)
        {
            IsGround = false;
        }
    }
}
