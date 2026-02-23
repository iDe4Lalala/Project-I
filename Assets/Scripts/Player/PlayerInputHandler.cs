using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler
{
    private PlayerManager _playerManager;
    private InputSystem_Actions _inputSystemActions;

    private void Initialize(PlayerManager playerManager, InputSystem_Actions inputActions)
    {
        _playerManager = playerManager;
        _inputSystemActions = inputActions;
        InitializeEvents();
        _inputSystemActions.Enable();
    }

    private void Dispose()
    {
        _inputSystemActions.Disable();
        DisposeEvents();
    }

    private void InitializeEvents()
    {
        _inputSystemActions.Player.Move.performed += OnMovePerformed;
        _inputSystemActions.Player.Move.canceled += OnMoveCanceled;
        _inputSystemActions.Player.Look.performed += OnLookPerformed;
        _inputSystemActions.Player.Look.canceled += OnLookCanceled;
        _inputSystemActions.Player.Jump.performed += OnJumpPerformed;
        _inputSystemActions.Player.Sprint.started += OnSprintStarted;
        _inputSystemActions.Player.Sprint.canceled += OnSprintCanceled;
        _inputSystemActions.Player.Fire.started += OnFireStarted;
        _inputSystemActions.Player.Fire.canceled += OnFireCanceled;
        _inputSystemActions.Player.Reload.performed += OnReloadPerformed;
    }

    private void DisposeEvents()
    {
        _inputSystemActions.Player.Move.performed -= OnMovePerformed;
        _inputSystemActions.Player.Move.canceled -= OnMoveCanceled;
        _inputSystemActions.Player.Look.performed -= OnLookPerformed;
        _inputSystemActions.Player.Look.canceled -= OnLookCanceled;
        _inputSystemActions.Player.Jump.performed -= OnJumpPerformed;
        _inputSystemActions.Player.Sprint.started -= OnSprintStarted;
        _inputSystemActions.Player.Sprint.canceled -= OnSprintCanceled;
        _inputSystemActions.Player.Fire.started -= OnFireStarted;
        _inputSystemActions.Player.Fire.canceled -= OnFireCanceled;
        _inputSystemActions.Player.Reload.performed -= OnReloadPerformed;
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        Vector2 move = context.ReadValue<Vector2>();
        _playerManager.RequestMove(move);
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        _playerManager.RequestMove(Vector2.zero);
    }

    private void OnLookPerformed(InputAction.CallbackContext context)
    {
        Vector2 delta = context.ReadValue<Vector2>();
        _playerManager.RequestLook(delta);
    }

    private void OnLookCanceled(InputAction.CallbackContext context)
    {
        _playerManager.RequestLook(Vector2.zero);
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        _playerManager.RequestJump();
    }

    private void OnSprintStarted(InputAction.CallbackContext context)
    {
        _playerManager.RequestStartSprint();
    }

    private void OnSprintCanceled(InputAction.CallbackContext context)
    {
        _playerManager.RequestStopSprint();
    }

    private void OnFireStarted(InputAction.CallbackContext context)
    {
        _playerManager.RequestStartFire();
    }

    private void OnFireCanceled(InputAction.CallbackContext context)
    {
        _playerManager.RequestStopFire();
    }

    private void OnReloadPerformed(InputAction.CallbackContext context)
    {
        _playerManager.RequestReload();
    }
}
