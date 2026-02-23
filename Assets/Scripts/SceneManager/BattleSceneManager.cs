using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;
using UnityEngine.Events;

public class BattleSceneManager : MonoBehaviour
{
    [field: SerializeField] public WeaponDataBase WeaponDataBase { get; private set; }
    [field: SerializeField] public PlayerUIManager PlayerUIManager { get; private set; }
    [field: SerializeField] public BattleUIManager BattleUIManager { get; private set; }
    [field: SerializeField] public List<EnemyWaveDataBase> EnemyWaveDataBaseList { get; private set; }
    [field: SerializeField] public UnityEvent<bool> OnKilled { get; private set; }

    [SerializeField] private BattleStateMachine _battleStateMachine;
    [SerializeField] private Transform _playerSpawnPoint;
    [SerializeField] private Transform[] _enemySpawnPoints;
    [SerializeField] private HumanDataBase _playerDataBase;
    [SerializeField] private HumanDataBase _enemyDataBase;
    [SerializeField] private WeaponDataBase _weaponDataBase;
    [SerializeField] private string _nextSceneName;
    [SerializeField] private Canvas _battleCanvas;
    [SerializeField] private float _battleTimer;
    [SerializeField] private EnemyAppearanceManager _enemyAppearanceManager;
    [SerializeField] private GameObject _diedCameraPosition;

    public event Action<float> TimerUpdated;
    private float _currentTimer;
    private PlayerComponents _playerComponents;
    private int _lifePoint;
    private IGenerator _playerGenerator;
    private IGenerator _enemyGenerator;
    private IGenerator _weaponGenerator;
    private BattleContext _battleContext;
    private ISpawnPointProvider _playerSpawnPointProvider;
    private ISpawnPointProvider _enemySpawnPointProvider;
    
    private void Awake()
    {
        _playerSpawnPointProvider = new PlayerSpawnPointProvider(_playerSpawnPoint);
        _enemySpawnPointProvider = new EnemySpawnPointProvider(_enemySpawnPoints);
        _weaponGenerator = new WeaponGenerator(WeaponDataBase);
        _playerGenerator = new PlayerGenerator(_playerDataBase, _weaponGenerator);
        _enemyGenerator = new EnemyGenerator(_enemyDataBase, _weaponGenerator);
        // BattleContextの受け取りをinterfaceに
        _battleContext = new BattleContext(
            _battleStateMachine, _playerGenerator, _enemyGenerator, _weaponGenerator,
            BattleUIManager, _playerSpawnPointProvider, _enemySpawnPointProvider, _playerDataBase,
            _enemyDataBase, WeaponDataBase, _playerSpawnPoint, _enemySpawnPoints, EnemyWaveDataBaseList
        );
        _battleStateMachine.ChangingScene += OnBattleEnded;
        _battleStateMachine.Initialize(_battleContext);
    }

    private void OnEnable()
    {
        BattleUIManager.OnStartingBattle += OnStartBattle;
        _currentTimer = _battleTimer;
        _lifePoint = _playerDataBase.LifePoint;

        TimerUpdated?.Invoke(_battleTimer);
        GeneratePlayer();
    }

    private void OnDisable()
    {
        BattleUIManager.OnStartingBattle -= OnStartBattle;
    }
    
    private void Update()
    {
        if(_enemyAppearanceManager.CurrentWaveState == EnemyWaveState.Waiting) return;

        if (_currentTimer >= 0)
        {
            _currentTimer -= Time.deltaTime;
            _currentTimer = Mathf.Max(0, _currentTimer);
            TimerUpdated?.Invoke(_currentTimer);
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
        GameObject player = _playerGenerator.Generate(_playerSpawnPoint);

        _playerComponents = player.GetComponent<PlayerComponents>();
        _playerComponents.AspectRatioManager.SetCanvas(_battleCanvas);
        _playerComponents.PlayerManager.OnDied += OnPlayerDied;

        GameObject rifle = _weaponGenerator.Generate(_playerComponents.WeaponSocket.transform);
        _playerComponents.SetRifleManager(rifle);

        PlayerUIManager.SetPlayer(player);
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

    public void OnBattleEnded(BattleResultType result)
    {
        if (result == BattleResultType.GameClear)
        {
            // 結果を保持しつつシーン遷移
        }
        PlayerUIManager.DisplayOrHideCursor(true);
        LoadOtherScene();
    }
}