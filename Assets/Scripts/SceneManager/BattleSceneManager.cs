using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Events;

public class BattleSceneManager : MonoBehaviour
{
    /// <summary>
    /// 戦闘シーンを管理する
    /// </summary>

    [SerializeField] private HumanDataBase _playerDataBase;   // 人のデータベースリスト
    [field: SerializeField] public GameObject RiflePrefab { get; private set; }  // ライフル
    [SerializeField] private Transform _playerRespawnPoint;
    [SerializeField] private string _nextSceneName;   // 結果シーンの名前
    [SerializeField] private Canvas _battleCanvas;
    [SerializeField] private float _battleTimer;   // 戦闘時間
    [field: SerializeField] public PlayerUIManager PlayerUIManager { get; private set; }
    [SerializeField] private BattleUIManager _battleUIManager;
    [SerializeField] private EnemyAppearanceManager _enemyAppearanceManager;
    [field: SerializeField] public UnityEvent<bool> OnKilled { get; private set; } // キルが発生した時のイベント
    public event Action<float> OnTimerUpdated;   // タイマーが更新された時のイベント

    private float _currentTimer;
    private PlayerComponents _playerComponents;

    private void OnEnable()
    {
        // 戦闘前の初期化
        _battleUIManager.OnStartBattle += OnStartBattle;
        _currentTimer = _battleTimer;

        OnTimerUpdated?.Invoke(_battleTimer);
        GeneratePlayer();
    }

    private void OnDisable()
    {
        _battleUIManager.OnStartBattle -= OnStartBattle;
    }

    private void GeneratePlayer()
    {
        /// <summary>
        /// プレイヤーを生成する
        /// </summary>
        
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
        /// <summary>
        /// プレイヤーのリスポーンポイントを設定する
        /// </summary>
        
        human.transform.SetPositionAndRotation(_playerRespawnPoint.position, _playerRespawnPoint.rotation);
        return;
    }

    private void Update()
    {
        if(_enemyAppearanceManager.CurrentWaveState == EnemyWaveState.Waiting) return;

        // タイマーを更新
        if (_currentTimer >= 0)
        {
            _currentTimer -= Time.deltaTime;
            OnTimerUpdated?.Invoke(_currentTimer);
        }
        else
        {
            // シーン遷移
            _enemyAppearanceManager.SetEnemyWaveState(EnemyWaveState.Finished);
            LoadOtherScene();
        }
    }

    public void OnStartBattle()
    {
        /// <summary>
        /// 戦闘開始時の処理
        /// </summary>

        _enemyAppearanceManager.SetEnemyWaveState(EnemyWaveState.Cleared);
        _enemyAppearanceManager.StartNextWave();
        _battleUIManager.OnNextWaveStarted(_enemyAppearanceManager.EnemyWaveDataBaseList[0].WaveText);
    }

    private void OnPlayerDied(GameObject player)
    {
        /// <summary>
        /// プレイヤーが死亡した時の処理
        /// </summary>
        Destroy(player);
        _playerComponents = null;
        OnKilled?.Invoke(false);

        GeneratePlayer();
    }

    public void LoadOtherScene()
    {
        /// <summary>
        /// 他のシーンをロードする
        /// </summary>
        
        SceneManager.LoadScene(_nextSceneName);
    }
}