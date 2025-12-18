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
    [field: SerializeField] public UnityEvent<bool> OnKilled { get; private set; } // キルが発生した時のイベント
    public event Action<float> OnTimerUpdated;   // タイマーが更新された時のイベント

    private bool _isBattle;    // 戦闘中かどうか
    private PlayerComponents _playerComponents;

    private void OnEnable()
    {
        // 戦闘前の初期化
        _isBattle = false;
        _battleUIManager.OnStartBattle += OnStartBattle;
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
        if(!_isBattle) return;

        // タイマーを更新
        if (_battleTimer >= 0)
        {
            _battleTimer -= Time.deltaTime;
            OnTimerUpdated?.Invoke(_battleTimer);
        }
        else
        {
            // シーン遷移
            _isBattle = false;
            LoadOtherScene();
        }
    }

    public void OnStartBattle(bool isStart)
    {
        /// <summary>
        /// 戦闘開始時の処理
        /// </summary>
        
        _isBattle = isStart;
    }

    private void OnPlayerDied(GameObject player)
    {
        /// <summary>
        /// プレイヤーが死亡した時の処理
        /// </summary>
        Destroy(player);
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