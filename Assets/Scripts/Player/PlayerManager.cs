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
    [SerializeField] private PlayerComponents _playerComponents;
    [SerializeField] private PlayerInputController _playerInputController;  // IPlayerInputと統一する？
    [SerializeField] private PlayerAnimationContoller _playerAnimationController;
    [SerializeField] private PlayerAudioController _playerAudioController;
    [SerializeField] private LayerMask _targetMask;
    [SerializeField] private int _canJumpCount;

    public int PlayerHP { get; private set; }
    public event Action OnDied;
    public event Action<int> PlayerHPUpdated;
    private int _jumpCount;
    private PlayerState _playerState;
    private PlayerInputHandler _playerInputHandler;
    private IPlayerInput _playerInput;
    private IWeaponCommand _weaponCommand;
    private IFireRuntime _fireRuntime;
    private IReloadRuntime _reloadRuntime;

    private void Start()
    {
        _playerInputController.LandedGround += OnLandedGround;
        _playerState = PlayerState.PreBattle;
        _playerInputController.Initialize(_playerComponents);
        _playerAnimationController.Initialize(_playerComponents);
        _playerAudioController.Initialize(_playerComponents);

        PlayerHP = _playerComponents.HumanDataBase.HumanHP;
        PlayerHPUpdated?.Invoke(PlayerHP);
    }

    private void OnDestroy()
    {
        _playerInputController.LandedGround -= OnLandedGround;
    }

    public void Initialize(PlayerInputHandler playerInputHandler)
    {
        if(playerInputHandler == null) return;
        _playerInput = _playerInputController;
        _playerInputHandler = playerInputHandler;
        _weaponCommand = _playerComponents.RifleManager;
        _fireRuntime = _playerComponents.RifleManager;
        _reloadRuntime = _playerComponents.RifleManager;

        _playerComponents.RifleManager.Initialize(_targetMask);
    }

    public void SetAliveState()
    {
        _playerState = PlayerState.Alive;
    }

    public void TakeDamage(int damage)
    {
        PlayerHP -= damage;
        PlayerHP = Mathf.Max(PlayerHP, 0);
        PlayerHPUpdated?.Invoke(PlayerHP);

        if (PlayerHP > 0) return;
        _playerState = PlayerState.Dead;
        OnDied?.Invoke();
    }

    private void Update()
    {
        if(!CanControl()) return;
        if(_playerInputHandler == null || _fireRuntime == null || _reloadRuntime == null) return;
        _playerInputHandler.ConsumePerFrameInput();
        Vector2 recoil = _fireRuntime.TryFire(Time.deltaTime, 
            _playerComponents.Camera.transform.position, _playerComponents.Camera.transform.forward);
        _playerInputController.AddRecoil(recoil);
        _reloadRuntime.UpdateReload(Time.deltaTime);
    }

    private bool CanControl()
    {
        return _playerState == PlayerState.Alive;
    }

    public void RequestMove(Vector2 move)
    {
        if(!CanControl()) return;
        if(_playerInput == null) return;
        _playerInput.SetMove(move);
        _playerAnimationController.SetMove(_playerInputController.IsMoving);
        _playerAudioController.UpdateFootstep(_playerInputController.IsGround, move.magnitude);
        _playerAnimationController.SetIsGround(_playerInputController.IsGround);
    }

    public void RequestLook(Vector2 delta)
    {
        if(!CanControl()) return;
        if(_playerInput == null) return;
        _playerInput.SetLookDelta(delta);
    }

    public void RequestJump()
    {
        if(!CanControl()) return;
        if(!CanJump()) return;
        if(_playerInput == null) return;

        _jumpCount++;
        _playerInput.Jump();
        _playerAnimationController.SetJump();
    }

    public void RequestStartSprint()
    {
        if(!CanControl()) return;
        if(_playerInput == null) return;
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
        if(_playerInput == null) return;
        _weaponCommand.StartFire();
    }

    public void RequestStopFire()
    {
        if(!CanControl()) return;
        if(_playerInput == null) return;
        _weaponCommand.StopFire();
    }

    public void RequestReload()
    {
        if(!CanControl()) return;
        if(_playerInput == null) return;
        _weaponCommand.Reload();
    }

    private bool CanJump()
    {
        return _jumpCount < _canJumpCount;
    }

    private void OnLandedGround()
    {
        _jumpCount = 0;
        _playerAnimationController.SetIsGround(_playerInputController.IsGround);
    }
}
