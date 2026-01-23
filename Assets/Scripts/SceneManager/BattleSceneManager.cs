using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Events;

public class BattleSceneManager : MonoBehaviour
{
    [field: SerializeField] public GameObject RiflePrefab { get; private set; }
    [field: SerializeField] public PlayerUIManager PlayerUIManager { get; private set; }
    [field: SerializeField] public BattleUIManager BattleUIManager { get; private set; }
    [field: SerializeField] public UnityEvent<bool> OnKilled { get; private set; }
    [SerializeField] private HumanDataBase _playerDataBase;
    [SerializeField] private Transform _playerRespawnPoint;
    [SerializeField] private string _nextSceneName;
    [SerializeField] private Canvas _battleCanvas;
    [SerializeField] private float _battleTimer;
    [SerializeField] private EnemyAppearanceManager _enemyAppearanceManager;
    [SerializeField] private GameObject _diedCameraPosition;
    
    public event Action<float> OnTimerUpdated;
    private float _currentTimer;
    private PlayerComponents _playerComponents;
    private int _lifePoint;
    private GameObject _playerCamera;

    private void OnEnable()
    {
        BattleUIManager.OnStartBattle += OnStartBattle;
        _currentTimer = _battleTimer;
        _lifePoint = _playerDataBase.LifePoint;

        OnTimerUpdated?.Invoke(_battleTimer);
        GeneratePlayer();
    }

    private void OnDisable()
    {
        BattleUIManager.OnStartBattle -= OnStartBattle;
    }
    
    private void Update()
    {
        if(_enemyAppearanceManager.CurrentWaveState == EnemyWaveState.Waiting) return;

        if (_currentTimer >= 0)
        {
            _currentTimer -= Time.deltaTime;
            _currentTimer = Mathf.Max(0, _currentTimer);
            OnTimerUpdated?.Invoke(_currentTimer);
        }
        else
        {
            _enemyAppearanceManager.SetEnemyWaveState(EnemyWaveState.Finished);
            LoadOtherScene();
        }
    }

    public void OnStartBattle()
    {
        _enemyAppearanceManager.SetEnemyWaveState(EnemyWaveState.Cleared);
        _enemyAppearanceManager.StartNextWave();
        BattleUIManager.OnNextWaveStarted(_enemyAppearanceManager.EnemyWaveDataBaseList[0].WaveText);
    }

    public void LoadOtherScene()
    {
        SceneManager.LoadScene(_nextSceneName);
    }

    private void GeneratePlayer()
    {
        if (_playerComponents != null) return;
        GameObject player = Instantiate(_playerDataBase.HumanObject);
        SetPlayerRespawnPoint(player);

        _playerComponents = player.GetComponent<PlayerComponents>();
        _playerComponents.AspectRatioManager.SetCanvas(_battleCanvas);
        _playerComponents.PlayerManager.OnDied += OnPlayerDied;

        GameObject rifle = Instantiate(RiflePrefab, _playerComponents.RifleSocket.transform);
        _playerComponents.SetRifleManager(rifle);

        PlayerUIManager.SetPlayer(player);
    }

    private void SetPlayerRespawnPoint(GameObject human)
    {
        human.transform.SetPositionAndRotation(_playerRespawnPoint.position, _playerRespawnPoint.rotation);
        return;
    }

    private void OnPlayerDied(GameObject player)
    {
        ChangeCameraPosition(_playerComponents.Camera.transform);

        Destroy(player);
        _playerComponents = null;
        OnKilled?.Invoke(false);

        _lifePoint--;
        if (_lifePoint > 0)
        {
            GeneratePlayer();
            return;
        }
        PlayerUIManager.DisplayOrHideCursor(true);
        LoadOtherScene();
    }

    private void ChangeCameraPosition(Transform cameraTransform)
    {
        cameraTransform.SetParent(null);
        cameraTransform.position = _diedCameraPosition.transform.position;
        cameraTransform.rotation = _diedCameraPosition.transform.rotation;
        for (int i = cameraTransform.childCount - 1; i >= 0; i--)
        {
            Destroy(cameraTransform.GetChild(i).gameObject);
        }
        PlayerUIManager.gameObject.SetActive(false);
    }
}