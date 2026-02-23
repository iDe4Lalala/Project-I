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


    private void OnEnable()
    {
        _hitCrossHair.SetActive(false);
    }

    private void Start()
    {
        SwitchOperationButtonsDisplay(false);
        DisplayOrHideCursor(false);
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
