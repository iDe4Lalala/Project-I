using UnityEngine;
using TMPro;
using System.Collections;

public class PlayerUIManager : MonoBehaviour, IPlayerUIService
{
    [SerializeField] private TMP_Text _playerHPText;
    [SerializeField] private TMP_Text _playerAmmoText;
    [SerializeField] private TMP_Text _playerLifePointText;
    [SerializeField] private GameObject _hitCrossHair;
    [SerializeField] private float _hitCrossHairDisplayTime;
    private PlayerComponents _playerComponents;
    private float _hitCrossHairTimer;
    private bool _isHitCrossHairTimerRunning;
    private int _playerMaxHP;


    private void OnEnable()
    {
        if (_hitCrossHair == null) return;
        _hitCrossHair.SetActive(false);
        _hitCrossHairTimer = 0f;
        _isHitCrossHairTimerRunning = false;
        DisplayOrHideCursor(false);
    }

    private void OnDisable()
    {
        UnbindPlayerEvents();
    }

    public void SetVisible(bool isVisible)
    {
        gameObject.SetActive(isVisible);
    }

    public void Initialize(PlayerComponents playerComponents)
    {
        UnbindPlayerEvents();
        _playerComponents = playerComponents;
        _playerMaxHP = _playerComponents.HumanDataBase.HumanHP;
        BindPlayerEvents();
    }

    private void OnPlayerDead()
    {
        UnbindPlayerEvents();
        _playerComponents = null;
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

    private void BindPlayerEvents()
    {
        if (_playerComponents == null) return;
        _playerComponents.PlayerManager.OnDied += OnPlayerDead;
        _playerComponents.RifleManager.AmmoCountUpdated += ShowLeftAmmoCount;
        _playerComponents.RifleManager.EnemyWasHit += () => 
            StartCoroutine(ShowHitCrossHairForSeconds(_hitCrossHairDisplayTime));
        _playerComponents.PlayerManager.PlayerHPUpdated += ShowPlayerHP;
    }

    private void UnbindPlayerEvents()
    {
        if (_playerComponents == null) return;
        _playerComponents.PlayerManager.OnDied -= OnPlayerDead;
        _playerComponents.RifleManager.AmmoCountUpdated -= ShowLeftAmmoCount;
        _playerComponents.RifleManager.EnemyWasHit -= () => 
            StartCoroutine(ShowHitCrossHairForSeconds(_hitCrossHairDisplayTime));
        _playerComponents.PlayerManager.PlayerHPUpdated -= ShowPlayerHP;
    }

    public IEnumerator ShowHitCrossHairForSeconds(float seconds)
    {
    if (_hitCrossHair == null) yield break;
    _hitCrossHairTimer = Mathf.Max(_hitCrossHairTimer, seconds);
    if (_isHitCrossHairTimerRunning) yield break;

    _isHitCrossHairTimerRunning = true;
    _hitCrossHair.SetActive(true);
    while (_hitCrossHairTimer > 0f)
    {
        _hitCrossHairTimer -= Time.deltaTime;
        yield return null;
    }

    _hitCrossHair.SetActive(false);
    _isHitCrossHairTimerRunning = false;
    }

    public void ShowPlayerHP(int currentHP)
    {
        if (_playerHPText == null) return;
        currentHP = Mathf.Max(0, currentHP);
        _playerHPText.text = $"HP: {currentHP} / {_playerMaxHP}";
    }

    public void ShowLeftAmmoCount(int currentAmmoCount)
    {
        if (_playerAmmoText == null) return;
        currentAmmoCount = Mathf.Max(0, currentAmmoCount);
        _playerAmmoText.text = $"Ammo: {currentAmmoCount}";
    }

    public void ShowPlayerLifePoint(int currentLifePoint)
    {
        if (_playerLifePointText == null) return;
        currentLifePoint = Mathf.Max(0, currentLifePoint);
        _playerLifePointText.text = $"LP: {currentLifePoint}";
    }
}
