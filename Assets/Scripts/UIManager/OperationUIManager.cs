using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class OperationUIManager : MonoBehaviour
{
    [SerializeField] private FixedJoystick _fixedJoystick;
    [SerializeField] private FloatingJoystick _floatingJoystick;
    [SerializeField] private Button _jumpButton;
    [SerializeField] private Button _shootingButton;
    [SerializeField] private bool _isUseJoystick;
    [SerializeField] private PlayerUIManager _playerUIManager;

    private GameObject _player;
    private PlayerComponents _playerComponents;
    private float _xMovement;
    private float _zMovement;
    private float _xRotation;
    private float _yRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // カーソルを画面中央に固定
        Cursor.visible = false;                   // カーソルを非表示
    }

    private void SetMethods()
    {
        _jumpButton.onClick.AddListener(() => _playerComponents.PlayerManager.OnJumpButtonDown());
        _shootingButton.onClick.AddListener(() => _playerComponents.RifleManager.ShootByRifle());
    }

    public void SetPlayer(GameObject player)
    {
        if(player == null) return;
        _player = player;
        _playerComponents = player.GetComponent<PlayerComponents>();
        _playerComponents.PlayerManager.SetCamera(Camera.main);
        if(!_isUseJoystick) return;
        SetMethods();
        _playerUIManager.SetPlayerHP(_playerComponents.PlayerManager.PlayerHP);
    }

    private void Update()
    {
        if (_player == null) return;
        if(_isUseJoystick)
        {
            _xMovement = _fixedJoystick.Horizontal;
            _zMovement = _fixedJoystick.Vertical;
            _xRotation = _floatingJoystick.Horizontal;
            _yRotation = _floatingJoystick.Vertical;
        }
        else
        {
            _xMovement = Input.GetAxis("Horizontal");
            _zMovement = Input.GetAxis("Vertical");
            _xRotation = Input.GetAxis("Mouse X");
            _yRotation = Input.GetAxis("Mouse Y");
        }
        _playerComponents.PlayerManager.SetMovementInput(_xMovement, _zMovement);
        _playerComponents.PlayerManager.SetRotationInput(_xRotation, _yRotation);

        if(_isUseJoystick) return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _playerComponents.PlayerManager.OnJumpButtonDown();
        }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            _playerComponents.RifleManager.ShootByRifle();
        }
    }
}
