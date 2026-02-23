using UnityEngine;
using System;

public enum PlayerState
{
    PreBattle = 0,
    Alive = 1,
    Dead = 2,
}

public class PlayerManager : MonoBehaviour, IDamageable
{
    [SerializeField] private string _groundTagName;
    [SerializeField] private float _footSoundThreshold;
    [SerializeField] private float _movementThreshold;
    [SerializeField] private ViewRifleAnimationManager _viewRifleAnimationManager;

    public int PlayerHP { get; private set; }
    public int JumpCount { get; private set; }
    private Vector3 _joystickVector;
    private bool _isMoving;
    private int _ammoCount;
    private float _shootTimer;
    private Vector2 _recoil;
    public event Action OnDamaged;
    public event Action<GameObject> OnDied;
    private bool _isReloading;

    [SerializeField] private PlayerComponents _playerComponents;
    [field: SerializeField] public int CanJumpCount { get; private set; }
    private PlayerState _playerState;
    private IPlayerInput _playerInput;
    private IWeaponCommand _weaponCommand;

    void Start()
    {
        _playerState = PlayerState.PreBattle;
        _isReloading = false;
        PlayerHP = _playerComponents.HumanDataBase.HumanHP;
        _ammoCount = _playerComponents.RifleManager.WeaponDataBase.MagazineCapacity;
    }

    public void SetMovementInput(float x, float z, float sprintSpeed = 1f)
    {
        // アニメーションは別クラスで
        _isMoving = Mathf.Abs(x) > _movementThreshold || Mathf.Abs(z) > _movementThreshold;
        _playerComponents.Animator.SetBool("IsMoving", _isMoving);

        // _joystickVector = Vector3.right * x + Vector3.up * z;

        // 音の再生は別クラスで
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

        // if (_joystickVector == Vector3.zero) return;
        // gameObject.transform.position += 
        //     _playerComponents.HumanDataBase.MovementSpeed * z * _playerComponents.Camera.transform.forward * sprintSpeed + 
        //     _playerComponents.HumanDataBase.MovementSpeed * x * _playerComponents.Camera.transform.right;
    }

    public void OnJumpButtonDown()
    {
        // アニメーションは別クラスで
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
        // Weapon側で処理する
        if (_isReloading) return;
        _isReloading = true;
        _viewRifleAnimationManager.PlayReloadAnimation();
    }

    public void FinishedReload()
    {
        // Weapon側で処理する
        _isReloading = false;
        _ammoCount = _playerComponents.RifleManager.WeaponDataBase.MagazineCapacity;
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col == null) return;
        if (col.gameObject.transform.parent.tag != _groundTagName
            && col.gameObject.transform.parent.parent.tag != _groundTagName) return;
        JumpCount = 0;

        // アニメーションは別クラスで
        if (_playerComponents.Animator.GetBool("IsGround")) return;
        _playerComponents.Animator.SetBool("IsGround", true);
    }

    private void Update()
    {
        _shootTimer += Time.deltaTime;
    }

    public void CheckCanShoot()
    {
        // Weapon側で処理する
        if (_shootTimer <= 1f / _playerComponents.RifleManager.WeaponDataBase.FireRate) return;
        _shootTimer = 0;
        if (_ammoCount <= 0) return;
        _ammoCount--;

        _recoil = _playerComponents.RifleManager.ShootByRifle();
        // SetRotationInput(0, 0, _recoil);
    }

    private bool CanControl()
    {
        return _playerState == PlayerState.Alive;
    }

    public void RequestMove(Vector2 move)
    {
        if(!CanControl()) return;
        _playerInput.SetMove(move);
    }

    public void RequestLook(Vector2 delta)
    {
        if(!CanControl()) return;
        _playerInput.SetLookDelta(delta);
    }

    public void RequestJump()
    {
        if(!CanControl()) return;
        _playerInput.Jump();
    }

    public void RequestStartSprint()
    {
        if(!CanControl()) return;
        _playerInput.StartSprint();
    }

    public void RequestStopSprint()
    {
        if(!CanControl()) return;
        _playerInput.StopSprint();
    }

    public void RequestStartFire()
    {
        if(!CanControl()) return;
        _weaponCommand.StartFire();
    }

    public void RequestStopFire()
    {
        if(!CanControl()) return;
        _weaponCommand.StopFire();
    }

    public void RequestReload()
    {
        if(!CanControl()) return;
        _weaponCommand.Reload();
    }
}
