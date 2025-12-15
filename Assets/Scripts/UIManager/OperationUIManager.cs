using UnityEngine;
using UnityEngine.UI;

public class OperationUIManager : MonoBehaviour
{
    /// <summary>
    /// 操作に関するUIを管理する
    /// </summary>
    
    [SerializeField] private FixedJoystick _fixedJoystick;      // ジョイスティック(プレイヤー移動用)
    [SerializeField] private FloatingJoystick _floatingJoystick;    // ジョイスティック(プレイヤー視点操作用)
    [SerializeField] private Button _jumpButton;    // ジャンプボタン
    [SerializeField] private Button _shootingButton;    // 射撃ボタン
    [SerializeField] private bool _isUseJoystick;   // 入力方式がジョイスティックか

    private GameObject _player; // プレイヤー
    private PlayerComponents _playerComponents;
    private float _xMovement;   // 水平方向の移動入力
    private float _zMovement;   // 垂直方向の移動入力
    private float _xRotation;   // 水平方向の視点入力
    private float _yRotation;   // 垂直方向の視点入力

    private void Start()
    {
        if(_isUseJoystick)
        {
            SetMethods();
        }
        else
        {
            DisplayeOrHideOperationButtons(false);
            FixCursorAndHide();
        }
    }

    private void DisplayeOrHideOperationButtons(bool isDisplay)
    {
        /// <summary>
        /// ジョイスティックとボタンを表示/非表示にする
        /// </summary>
        
        _fixedJoystick.gameObject.SetActive(isDisplay);
        _floatingJoystick.gameObject.SetActive(isDisplay);
        _jumpButton.gameObject.SetActive(isDisplay);
        _shootingButton.gameObject.SetActive(isDisplay);
    }

    private void FixCursorAndHide()
    {
        // カーソルを画面中央に固定して非表示(キーマウ操作時)
        Cursor.lockState = CursorLockMode.Locked; 
        Cursor.visible = false;
    }

    private void SetMethods()
    {
        /// <summary>
        /// ボタンのメソッドを設定する
        /// </summary>
        
        _jumpButton.onClick.AddListener(() => _playerComponents.PlayerManager.OnJumpButtonDown());
        _shootingButton.onClick.AddListener(() => _playerComponents.RifleManager.ShootByRifle());
    }

    public void SetPlayer(GameObject player)
    {
        /// <summary>
        /// プレイヤーを設定する
        /// </summary>
        
        if(player == null) return;

        _player = player;
        _playerComponents = player.GetComponent<PlayerComponents>();
    }

    private void Update()
    {
        if (_player == null) return;
        if(_isUseJoystick)      // ジョイスティックによる入力
        {
            _xMovement = _fixedJoystick.Horizontal;
            _zMovement = _fixedJoystick.Vertical;
            _xRotation = _floatingJoystick.Horizontal;
            _yRotation = _floatingJoystick.Vertical;
        }
        else    // キーマウによる入力
        {
            _xMovement = Input.GetAxis("Horizontal");
            _zMovement = Input.GetAxis("Vertical");
            _xRotation = Input.GetAxis("Mouse X");
            _yRotation = Input.GetAxis("Mouse Y");
        }

        // プレイヤーの移動と視点操作の入力を反映
        _playerComponents.PlayerManager.SetMovementInput(_xMovement, _zMovement);
        _playerComponents.PlayerManager.SetRotationInput(_xRotation, _yRotation);

        // ジャンプと射撃の入力(キーマウ操作時のみ)
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
