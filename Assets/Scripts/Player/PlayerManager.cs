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
    public int PlayerHP { get; private set; }
    public event Action OnDamaged;
    public event Action<GameObject> OnDied;

    [SerializeField] private PlayerComponents _playerComponents;
    [SerializeField] private PlayerInputController _playerInputController;  // IPlayerInputと統一する？
    [SerializeField] private PlayerAnimationContoller _playerAnimationController;
    [SerializeField] private PlayerAudioController _playerAudioController;
    [SerializeField] private LayerMask _targetMask;
    [field: SerializeField] public int CanJumpCount { get; private set; }
    public int JumpCount { get; private set; }
    private PlayerState _playerState;
    private PlayerInputHandler _playerInputHandler;
    private IPlayerInput _playerInput;
    private IWeaponCommand _weaponCommand;
    private IFireRuntime _fireRuntime;
    private IReloadRuntime _reloadRuntime;

    private void Start()
    {
        PlayerHP = _playerComponents.HumanDataBase.HumanHP;

        _playerInputController.LandedGround += OnLandedGround;
        _playerState = PlayerState.PreBattle;
        _playerInputController.Initialize(_playerComponents);
        _playerAnimationController.Initialize(_playerComponents);
        _playerAudioController.Initialize(_playerComponents);
    }

    private void OnDestroy()
    {
        _playerInputController.LandedGround -= OnLandedGround;
    }

    public void Initialize(PlayerInputHandler playerInputHandler, RifleManager rifleManager)
    {
        if(playerInputHandler == null || rifleManager == null) return;
        _playerInput = _playerInputController;
        _playerInputHandler = playerInputHandler;
        _weaponCommand = rifleManager;
        _fireRuntime = rifleManager;
        _reloadRuntime = rifleManager;

        rifleManager.Initialize(_targetMask);
    }

    public void TakeDamage(int damage)
    {
        PlayerHP -= damage;
        PlayerHP = Mathf.Max(PlayerHP, 0);
        OnDamaged?.Invoke();

        if (PlayerHP > 0) return;
        OnDied?.Invoke(gameObject);
    }

    private void Update()
    {
        if(!CanControl()) return;
        if(_playerInputHandler == null || _fireRuntime == null || _reloadRuntime == null) return;
        _playerInputHandler.ConsumePerFrameInput();
        _fireRuntime.TryFire(
            Time.deltaTime, _playerComponents.Camera.transform.position, _playerComponents.Camera.transform.forward);
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

        JumpCount++;
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
        return JumpCount < CanJumpCount;
    }

    private void OnLandedGround()
    {
        JumpCount = 0;
        _playerAnimationController.SetIsGround(_playerInputController.IsGround);
    }
}
