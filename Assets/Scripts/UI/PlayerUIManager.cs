using UnityEngine;
using TMPro;
using System.Collections;

public class PlayerUIManager : MonoBehaviour, IPlayerUIService
{
    [SerializeField] private BattleUIManager _battleUIManager; 
    [SerializeField] private TMP_Text _playerCurrentHP;
    [SerializeField] private TMP_Text _playerMaxHP;
    [SerializeField] private GameObject _hitCrossHair;
    [SerializeField] private GameObject _operationButtonParent;
    [SerializeField] private bool _isUseJoystick;
    [field: SerializeField] public float HitCrossHairDisplayTime { get; private set; }

    private PlayerComponents _playerComponents;
    private OperationButtonComponents _operationButtonsComponents;
    private float _xMovement;
    private float _zMovement;
    private float _xRotation;
    private float _yRotation;
    private bool _isWaiting;

    private void OnEnable()
    {
        _hitCrossHair.SetActive(false);
        _isWaiting = true;
        _battleUIManager.OnStartingBattle += () => _isWaiting = false;
    }

    private void OnDisable()
    {
        _battleUIManager.OnStartingBattle -= () => _isWaiting = false;
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

    private void Update()
    {
        if (_playerComponents == null) return;
        if (_isWaiting) return;
        
        if(_isUseJoystick)
        {
            _xMovement = _operationButtonsComponents.FixedJoystick.Horizontal;
            _zMovement = _operationButtonsComponents.FixedJoystick.Vertical;
            _xRotation = _operationButtonsComponents.FloatingJoystick.Horizontal;
            _yRotation = _operationButtonsComponents.FloatingJoystick.Vertical;
        }
        else
        {
            _xMovement = Input.GetAxis("Horizontal");
            _zMovement = Input.GetAxis("Vertical");
            _xRotation = Input.GetAxis("Mouse X");
            _yRotation = Input.GetAxis("Mouse Y");
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            _playerComponents.PlayerManager.SetMovementInput(
                _xMovement, _zMovement, _playerComponents.HumanDataBase.SprintSpeed);
        }
        else
        {
            _playerComponents.PlayerManager.SetMovementInput(_xMovement, _zMovement);
        }
        _playerComponents.PlayerManager.SetRotationInput(_xRotation, _yRotation);

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

    public void SetPlayer(GameObject player)
    {
        if(player == null) return;

        _playerComponents = player.GetComponent<PlayerComponents>();
        
        _playerComponents.PlayerManager.OnDamaged += OnPlayerDamaged;
        _playerMaxHP.text = _playerComponents.HumanDataBase.HumanHP.ToString();
        _playerCurrentHP.text = _playerComponents.HumanDataBase.HumanHP.ToString();
    }

    public IEnumerator ShowHitCrossHairForSeconds(float seconds)
    {
        _hitCrossHair.SetActive(true);
        yield return new WaitForSeconds(seconds);
        _hitCrossHair.SetActive(false);
    }

    private void SwitchOperationButtonsDisplay(bool isDisplay)
    {
        _operationButtonParent.SetActive(isDisplay);
    }

    private void SetMethods()
    {
        _operationButtonsComponents.JumpButton.onClick.AddListener(() => _playerComponents.PlayerManager.OnJumpButtonDown());
        _operationButtonsComponents.ShootingButton.onClick.AddListener(() => _playerComponents.RifleManager.ShootByRifle());
    }

    private void OnPlayerDamaged()
    {
        _playerCurrentHP.text = _playerComponents.PlayerManager.PlayerHP.ToString();

        if (_playerComponents.PlayerManager.PlayerHP <= 0)
        {
            _playerComponents.PlayerManager.OnDamaged -= OnPlayerDamaged;
            _playerComponents = null;
        }
    }

    public void ShowPlayerHP(float currentHP)
    {

    }

    public void ShowLeftAmmoCount(int currentAmmoCount)
    {

    }
}
