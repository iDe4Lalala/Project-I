using UnityEngine;

public class PlayerInputController : MonoBehaviour, IPlayerInput
{
    private PlayerManager _playerManager;
    private PlayerComponents _playerComponents;
    private Rigidbody _rigidbody;
    private HumanDataBase _playerData;
    private Vector2 _recoilDelta;
    private float _sprintSpeed;
    private Quaternion _cameraRotation;
    private Quaternion _characterRotation;

    private void Initialize(PlayerManager playerManager, PlayerComponents playerComponents)
    {
        _playerManager = playerManager;
        _playerData = playerComponents.HumanDataBase;
        _playerComponents = playerComponents;

        _rigidbody = _playerComponents.Rigidbody;
        _cameraRotation = _playerComponents.Camera.transform.localRotation;
        _characterRotation = gameObject.transform.localRotation;
    }

    public void SetMove(Vector2 direction)
    {
        if (direction == Vector2.zero) return;
        gameObject.transform.position += 
            _playerData.MovementSpeed * direction.y * _playerComponents.Camera.transform.forward * _sprintSpeed + 
            _playerData.MovementSpeed * direction.x * _playerComponents.Camera.transform.right;
    }

    public void SetLookDelta(Vector2 delta)
    {
        if(delta == Vector2.zero) return;
        _cameraRotation *= Quaternion.Euler(-delta.y * _playerData.RotationSpeed - _recoilDelta.y, 0, 0);
        _characterRotation *= Quaternion.Euler(0, delta.x * _playerData.RotationSpeed + _recoilDelta.x, 0);

        _cameraRotation = ClampRotation(_cameraRotation);
        _playerComponents.Camera.transform.localRotation = _cameraRotation;
        gameObject.transform.localRotation = _characterRotation;

        _recoilDelta = Vector2.zero;
    }

    public void Jump()
    {
        if(_playerManager.JumpCount >= _playerManager.CanJumpCount) return;
        _rigidbody.linearVelocity = new Vector3(0, _playerData.JumpForce, 0);
    }

    public void StartSprint()
    {
        _sprintSpeed = _playerData.SprintSpeed;
    }

    public void StopSprint()
    {
        _sprintSpeed = 1f;
    }

    public void ReceiveRecoil(Vector2 recoil)
    {
        _recoilDelta = recoil;
    }

    private Quaternion ClampRotation(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1f;
        
        float angleX = Mathf.Atan(q.x) * Mathf.Rad2Deg * 2f;
        angleX = Mathf.Clamp(angleX, _playerData.TurningMinAngle, _playerData.TurningMaxAngle);
        q.x = Mathf.Tan(angleX * Mathf.Deg2Rad * 0.5f);
        return q;
    }
}
