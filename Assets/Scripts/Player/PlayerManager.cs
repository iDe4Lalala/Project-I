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
    [SerializeField] private ViewRifleAnimationManager _viewRifleAnimationManager;

    public int PlayerHP { get; private set; }
    private int _ammoCount;
    private float _shootTimer;
    private Vector2 _recoil;
    public event Action OnDamaged;
    public event Action<GameObject> OnDied;
    private bool _isReloading;

    [SerializeField] private PlayerComponents _playerComponents;
    [SerializeField] private PlayerInputController _playerInputController;
    [SerializeField] private PlayerAnimationContoller _playerAnimationController;
    [SerializeField] private PlayerAudioController _playerAudioController;
    [field: SerializeField] public int CanJumpCount { get; private set; }
    public int JumpCount { get; private set; }
    private PlayerState _playerState;
    private PlayerInputHandler _playerInputHandler;
    private IPlayerInput _playerInput;
    private IWeaponCommand _weaponCommand;

    private void Start()
    {
        _isReloading = false;
        PlayerHP = _playerComponents.HumanDataBase.HumanHP;
        _ammoCount = _playerComponents.RifleManager.WeaponDataBase.MagazineCapacity;

        _playerInputController.LandedGround += OnLandedGround;
        _playerState = PlayerState.PreBattle;
        _playerInputController.Initialize(_playerComponents);
        _playerAnimationController.Initialize(_playerComponents);
        _playerAudioController.Initialize(_playerComponents);
    }

    public void SetInputHandler(PlayerInputHandler inputHandler)
    {
        _playerInputHandler = inputHandler;
    }

    private void OnDestroy()
    {
        _playerInputController.LandedGround -= OnLandedGround;
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

    private void Update()
    {
        _shootTimer += Time.deltaTime;
        _playerInputHandler.ConsumePerFrameInput();
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
        _playerAnimationController.SetMove(_playerInputController.IsMoving);
        _playerAudioController.UpdateFootstep(_playerInputController.IsGround, move.magnitude);
        _playerAnimationController.SetIsGround(_playerInputController.IsGround);
    }

    public void RequestLook(Vector2 delta)
    {
        if(!CanControl()) return;
        _playerInput.SetLookDelta(delta);
    }

    public void RequestJump()
    {
        if(!CanControl()) return;
        if(!CanJump()) return;

        JumpCount++;
        _playerInput.Jump();
        _playerAnimationController.SetJump();
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

    private bool CanJump()
    {
        return JumpCount < CanJumpCount;
    }

    private void OnLandedGround()
    {
        JumpCount = 0;
        _playerAnimationController.SetIsGround(_playerInputController.IsGround);
    }
}
