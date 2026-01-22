using UnityEngine;
using TMPro;
using System.Collections;

public class PlayerUIManager : MonoBehaviour
{
    [SerializeField] private BattleUIManager _battleUIManager; 
    [SerializeField] private TMP_Text _playerCurrentHP;
    [SerializeField] private TMP_Text _playerMaxHP;
    [SerializeField] private GameObject _hitCrossHair;
    [SerializeField] private GameObject _operationButtonParent;
    [SerializeField] private bool _isUseJoystick;   // 入力方式がジョイスティックか
    [SerializeField] private float _hitCrossHairDisplayTime;   // ヒット時クロスヘアの表示時間

    private PlayerComponents _playerComponents;
    private OperationButtonComponents _operationButtonsComponents;
    private float _xMovement;   // 水平方向の移動入力
    private float _zMovement;   // 垂直方向の移動入力
    private float _xRotation;   // 水平方向の視点入力
    private float _yRotation;   // 垂直方向の視点入力
    private bool _isWaiting;

    private void OnEnable()
    {
        _hitCrossHair.SetActive(false);
        _isWaiting = true;
        _battleUIManager.OnStartBattle += () => _isWaiting = false;
    }

    private void OnDisable()
    {
        _battleUIManager.OnStartBattle -= () => _isWaiting = false;
    }

    private void Start()
    {
        _operationButtonsComponents = _operationButtonParent.GetComponent<OperationButtonComponents>();

        if(_isUseJoystick)
        {
            SetMethods();
        }
        else
        {
            SwitchOperationButtonsDisplay(false);
            DisplayOrHideCursor(false);
        }
    }

    private void SwitchOperationButtonsDisplay(bool isDisplay)
    {
        /// <summary>
        /// 操作ボタンの表示・非表示を切り替える
        /// </summary>
        
        _operationButtonParent.SetActive(isDisplay);
    }

    public void DisplayOrHideCursor(bool isDisplay)
    {
        if (isDisplay)
        {
            Cursor.lockState = CursorLockMode.None; 
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked; 
        }
        Cursor.visible = isDisplay;
    }

    private void SetMethods()
    {
        /// <summary>
        /// ボタンのメソッドを設定する
        /// </summary>
        
        _operationButtonsComponents.JumpButton.onClick.AddListener(() => _playerComponents.PlayerManager.OnJumpButtonDown());
        _operationButtonsComponents.ShootingButton.onClick.AddListener(() => _playerComponents.RifleManager.ShootByRifle());
    }

    public void SetPlayer(GameObject player)
    {
        /// <summary>
        /// プレイヤーを設定する
        /// </summary>
        
        if(player == null) return;

        _playerComponents = player.GetComponent<PlayerComponents>();
        
        _playerComponents.PlayerManager.OnDamaged += OnPlayerDamaged;
        _playerMaxHP.text = _playerComponents.HumanDataBase.HumanHP.ToString();
        _playerCurrentHP.text = _playerComponents.HumanDataBase.HumanHP.ToString();
    }

    private void OnPlayerDamaged()
    {
        /// <summary>
        /// プレイヤーのHPを減少させる
        /// </summary>
        
        _playerCurrentHP.text = _playerComponents.PlayerManager.PlayerHP.ToString();

        if (_playerComponents.PlayerManager.PlayerHP <= 0)
        {
            _playerComponents.PlayerManager.OnDamaged -= OnPlayerDamaged;
            _playerComponents = null;
        }
    }

    public IEnumerator ShowHitCrossHair()
    {
        /// <summary>
        /// ヒットマークのアニメーション
        /// </summary>
        
        _hitCrossHair.SetActive(true);
        yield return new WaitForSeconds(_hitCrossHairDisplayTime);
        _hitCrossHair.SetActive(false);
    }

    private void Update()
    {
        if (_playerComponents == null) return;
        if (_isWaiting) return;
        
        if(_isUseJoystick)
        {
            // ジョイスティックによる入力
            _xMovement = _operationButtonsComponents.FixedJoystick.Horizontal;
            _zMovement = _operationButtonsComponents.FixedJoystick.Vertical;
            _xRotation = _operationButtonsComponents.FloatingJoystick.Horizontal;
            _yRotation = _operationButtonsComponents.FloatingJoystick.Vertical;
        }
        else
        {
            // キーマウによる入力
            _xMovement = Input.GetAxis("Horizontal");
            _zMovement = Input.GetAxis("Vertical");
            _xRotation = Input.GetAxis("Mouse X");
            _yRotation = Input.GetAxis("Mouse Y");
        }

        // プレイヤーの移動と視点操作の入力を反映
        if (Input.GetKey(KeyCode.LeftShift))
        {
            _playerComponents.PlayerManager.SetMovementInput(_xMovement, _zMovement, _playerComponents.HumanDataBase.SprintSpeed);
        }
        else
        {
            _playerComponents.PlayerManager.SetMovementInput(_xMovement, _zMovement);
        }
        _playerComponents.PlayerManager.SetRotationInput(_xRotation, _yRotation);

        // ジャンプと射撃の入力(キーマウ操作時のみ)
        if(_isUseJoystick) return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _playerComponents.PlayerManager.OnJumpButtonDown();
        }
        if (Input.GetKey(KeyCode.Mouse0))
        {
            _playerComponents.PlayerManager.CheckCanShoot();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            _playerComponents.PlayerManager.Reload();
        }
    }
}
