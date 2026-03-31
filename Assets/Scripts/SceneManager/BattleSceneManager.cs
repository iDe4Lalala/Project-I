using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;
using System.Collections;

public class BattleSceneManager : MonoBehaviour
{
    [field: SerializeField] public WeaponDataBase WeaponDataBase { get; private set; }
    [field: SerializeField] public PlayerUIManager PlayerUIManager { get; private set; }
    [field: SerializeField] public BattleUIManager BattleUIManager { get; private set; }
    [field: SerializeField] public List<EnemyWaveDataBase> EnemyWaveDataBaseList { get; private set; }
    [SerializeField] private BattleStateMachine _battleStateMachine;
    [SerializeField] private Transform _playerSpawnPoint;
    [SerializeField] private Transform[] _enemySpawnPoints;
    [SerializeField] private HumanDataBase _playerDataBase;
    [SerializeField] private HumanDataBase _enemyDataBase;
    [SerializeField] private string _nextSceneName;
    [SerializeField] private float _battleTimer;
    [SerializeField] private Camera _diedCamera;
    [SerializeField] private InGameDataBase _inGameDataBase;
    public event Action<float> TimerUpdated;
    public event Action PlayerLifePointBecameZero;
    public event Action TimeExpired;
    public event Action PlayerRespawning;
    private float _currentTimer;
    private GameObject _player;
    private PlayerComponents _playerComponents;
    private int _currentLifePoint;
    private IGenerator _playerGenerator;
    private IGenerator _enemyGenerator;
    private IWeaponGenerator _weaponGenerator;
    private BattleContext _battleContext;
    private ISpawnPointProvider _playerSpawnPointProvider;
    private ISpawnPointProvider _enemySpawnPointProvider;
    private InputSystem_Actions _inputSystemActions;
    private PlayerInputHandler _playerInputHandler;
    private IBattleUIService _battleUIService;
    private IPlayerUIService _playerUIService;
    private AudioListener _diedCameraAudioListener;
    private bool _isTimerRunning;

    private void Awake()
    {
        _currentLifePoint = _playerDataBase.LifePoint;

        _battleUIService = BattleUIManager;
        _playerUIService = PlayerUIManager;
        _inputSystemActions = new InputSystem_Actions();
        _playerInputHandler = new PlayerInputHandler();
        _playerSpawnPointProvider = new PlayerSpawnPointProvider(_playerSpawnPoint);
        _enemySpawnPointProvider = new EnemySpawnPointProvider(_enemySpawnPoints);
        _weaponGenerator = new WeaponGenerator(WeaponDataBase);
        _playerGenerator = new PlayerGenerator(_playerDataBase, _weaponGenerator, _playerInputHandler);
        _enemyGenerator = new EnemyGenerator(_enemyDataBase, _weaponGenerator);

        _battleContext = new BattleContext(
            _battleStateMachine, this, _playerGenerator, _enemyGenerator, _weaponGenerator,
            _battleUIService, _playerUIService, _playerSpawnPointProvider, _enemySpawnPointProvider, _playerDataBase,
            _enemyDataBase, WeaponDataBase, _playerSpawnPoint, _enemySpawnPoints, EnemyWaveDataBaseList
        );

        _diedCameraAudioListener = _diedCamera.GetComponent<AudioListener>();

        _battleStateMachine.ChangingScene += OnBattleEnded;
        _battleStateMachine.PlayerGenerated += OnPlayerGenerated;
        _battleStateMachine.BattleStarted += OnBattleStarted;
        _playerInputHandler.InitializeInputSystem(_inputSystemActions);
        _battleStateMachine.Initialize(_battleContext);
        _battleUIService.Initialize(this);
    }

    private void OnEnable()
    {
        _isTimerRunning = false;
        _currentTimer = _battleTimer;
        TimerUpdated?.Invoke(_battleTimer);
    }

    private void OnDisable()
    {
        _battleStateMachine.ChangingScene -= OnBattleEnded;
        _battleStateMachine.PlayerGenerated -= OnPlayerGenerated;
        _battleStateMachine.BattleStarted -= OnBattleStarted;
        _playerInputHandler.Dispose();
    }
    
    private void Update()
    {
        if (!_isTimerRunning) return;

        if (_currentTimer < 0)
        {
            TimeExpired?.Invoke();
            LoadOtherScene();
            return;
        }

        _currentTimer -= Time.deltaTime;
        _currentTimer = Mathf.Max(0, _currentTimer);
        TimerUpdated?.Invoke(_currentTimer);
    }

    public void LoadOtherScene()
    {
        SceneManager.LoadScene(_nextSceneName);
    }

    private void OnPlayerGenerated(GameObject player)
    {
        if(_player != null || _playerComponents != null) return;
        _player = player;
        _playerComponents = _player.GetComponent<PlayerComponents>();
        _playerComponents.PlayerManager.OnDied += OnPlayerDied;

        _playerComponents.Camera.enabled = true;
        _diedCameraAudioListener.enabled = false;
        _diedCamera.enabled = false;

        _playerUIService.ShowPlayerLifePoint(_currentLifePoint);
    }

    private void OnPlayerDied()
    {
        _isTimerRunning = false;
        StartCoroutine(PlayerDiedRoutine());
    }

    private IEnumerator PlayerDiedRoutine()
    {
        _diedCamera.enabled = true;
        _diedCameraAudioListener.enabled = true;
        _playerComponents.Camera.enabled = false;

        _playerComponents.PlayerManager.OnDied -= OnPlayerDied;
        Destroy(_player);
        _player = null;
        _playerComponents = null;

        _battleUIService.OnPlayerKilled();

        if (_currentLifePoint <= 0)
        {
            _playerUIService.DisplayOrHideCursor(true);
            PlayerLifePointBecameZero?.Invoke();
            yield break;
        }
        _currentLifePoint--;

        yield return _battleUIService.PlayCountdown(3f);
        _playerUIService.ShowPlayerLifePoint(_currentLifePoint);

        _battleStateMachine.GeneratePlayer();
        yield return null;
        _battleStateMachine.EnterAliveState();
        _isTimerRunning = true;

        PlayerRespawning?.Invoke();
    }

    private void OnBattleStarted()
    {
        _isTimerRunning = true;
    }

    public void OnBattleEnded(BattleResultType result)
    {
        _inGameDataBase.SetBattleResult(result);

        _playerUIService.DisplayOrHideCursor(true);
        LoadOtherScene();
    }
}
