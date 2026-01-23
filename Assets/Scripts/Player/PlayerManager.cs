using UnityEngine;
using System;

public class PlayerManager : MonoBehaviour, IDamageable
{
    [SerializeField] private PlayerComponents _playerComponents;
    [SerializeField] private int _canJumpCount;
    [SerializeField] private string _groundTagName;
    [SerializeField] private float _footSoundThreshold;
    [SerializeField] private float _movementThreshold;
    [SerializeField] private ViewRifleAnimationManager _viewRifleAnimationManager;

    public int PlayerHP { get; private set; }
    private Quaternion _cameraRotation;
    private Quaternion _characterRotation;
    private Rigidbody _rigidbody;
    private int _jumpCount;
    private Vector3 _joystickVector;
    private Transform _headRotation;
    private bool _isMoving;
    private int _ammoCount;
    private float _shootTimer;
    private Vector2 _recoil;
    private Vector2 _currentRecoil;
    private Vector2 _targetRecoil;
    public event Action OnDamaged;
    public event Action<GameObject> OnDied;
    private bool _isReloading;

    void Start()
    {
        _isReloading = false;
        _rigidbody = _playerComponents.Rigidbody;
        PlayerHP = _playerComponents.HumanDataBase.HumanHP;
        _cameraRotation = _playerComponents.Camera.transform.localRotation;
        _characterRotation = gameObject.transform.localRotation;
        _headRotation = _playerComponents.Animator.GetBoneTransform(HumanBodyBones.Head);
        _ammoCount = _playerComponents.RifleManager.WeaponDataBase.MagazineCapacity;
    }

    public void SetMovementInput(float x, float z, float sprintSpeed = 1f)
    {

        _isMoving = Mathf.Abs(x) > _movementThreshold || Mathf.Abs(z) > _movementThreshold;
        _playerComponents.Animator.SetBool("IsMoving", _isMoving);

        _joystickVector = Vector3.right * x + Vector3.up * z;

        if (_joystickVector.magnitude > _footSoundThreshold && !_playerComponents.FootstepAudioSource.isPlaying && _playerComponents.Animator.GetBool("IsGround"))
        {
            _playerComponents.FootstepAudioSource.PlayOneShot(_playerComponents.FootstepAudioClip);
        }
        else if (!_playerComponents.Animator.GetBool("IsGround"))
        {
            _playerComponents.FootstepAudioSource.Stop();
        }
        else if (_joystickVector.magnitude <= _footSoundThreshold && _playerComponents.FootstepAudioSource.isPlaying)
        {
            _playerComponents.FootstepAudioSource.Stop();
        }

        if (_joystickVector == Vector3.zero) return;
        gameObject.transform.position += 
            _playerComponents.HumanDataBase.MovementSpeed * z * _playerComponents.Camera.transform.forward * sprintSpeed + 
            _playerComponents.HumanDataBase.MovementSpeed * x * _playerComponents.Camera.transform.right;
    }

    public void SetRotationInput(float x, float y, Vector2 recoil = default)
    {
        _cameraRotation *= Quaternion.Euler(-y * _playerComponents.HumanDataBase.RotationSpeed - recoil.y, 0, 0);
        _characterRotation *= Quaternion.Euler(0, x * _playerComponents.HumanDataBase.RotationSpeed + recoil.x, 0);

        _cameraRotation = ClampRotation(_cameraRotation);
        _playerComponents.Camera.transform.localRotation = _cameraRotation;
        gameObject.transform.localRotation = _characterRotation;
    }

    public void OnJumpButtonDown()
    {
        if (_jumpCount >= _canJumpCount) return;
        _rigidbody.linearVelocity = new Vector3(0, _playerComponents.HumanDataBase.JumpForce, 0);
        _jumpCount++;

        _playerComponents.Animator.SetTrigger("Jump");
        if (!_playerComponents.Animator.GetBool("IsGround")) return;
        _playerComponents.Animator.SetBool("IsGround", false);
    }

    public void TakeDamage(int damage)
    {
        PlayerHP -= damage;
        PlayerHP = Mathf.Max(PlayerHP, 0);
        OnDamaged?.Invoke();

        if (PlayerHP <= 0)
        {
            OnDied?.Invoke(gameObject);
        }
    }

    public void Reload()
    {
        if (_isReloading) return;
        _isReloading = true;
        _viewRifleAnimationManager.PlayReloadAnimation();
    }

    public void FinishedReload()
    {
        _isReloading = false;
        _ammoCount = _playerComponents.RifleManager.WeaponDataBase.MagazineCapacity;
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col == null) return;
        if (col.gameObject.transform.parent.tag != _groundTagName
            && col.gameObject.transform.parent.parent.tag != _groundTagName) return;
        _jumpCount = 0;

        if (_playerComponents.Animator.GetBool("IsGround")) return;
        _playerComponents.Animator.SetBool("IsGround", true);
    }

    private Quaternion ClampRotation(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1f;
        
        float angleX = Mathf.Atan(q.x) * Mathf.Rad2Deg * 2f;
        angleX = Mathf.Clamp(angleX, _playerComponents.HumanDataBase.TurningMinAngle, _playerComponents.HumanDataBase.TurningMaxAngle);
        q.x = Mathf.Tan(angleX * Mathf.Deg2Rad * 0.5f);
        return q;
    }

    private void Update()
    {
        _shootTimer += Time.deltaTime;
    }

    public void CheckCanShoot()
    {
        if (_shootTimer <= 1f / _playerComponents.RifleManager.WeaponDataBase.FireRate) return;
        _shootTimer = 0;
        if (_ammoCount <= 0) return;
        _ammoCount--;

        _recoil = _playerComponents.RifleManager.ShootByRifle();
        SetRotationInput(0, 0, _recoil);
    }
}
