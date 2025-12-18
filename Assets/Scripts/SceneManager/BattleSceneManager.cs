using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Events;

public class BattleSceneManager : MonoBehaviour
{
    /// <summary>
    /// 戦闘シーンを管理する
    /// </summary>

    [SerializeField] private HumanDataBase _playerDataBase;   // 人のデータベースリスト
    [field: SerializeField] public GameObject RiflePrefab { get; private set; }  // ライフル
    [SerializeField] private Transform _playerRespawnPoint;
    [SerializeField] private TMP_Text _beforeBattleTimerText;   // 戦闘前のタイマーのテキスト
    [SerializeField] private TMP_Text _battleStartedText;   // 戦闘開始のテキスト
    [SerializeField] private float _beforeBattleTimer;   // 戦闘前のタイマーの時間
    [SerializeField] private float _startedTextDisplayTime;   // 戦闘開始テキストの表示時間
    [SerializeField] private string _nextSceneName;   // 結果シーンの名前
    [SerializeField] private Canvas _battleCanvas;
    [field: SerializeField] public PlayerUIManager PlayerUIManager { get; private set; }
    [field: SerializeField] public UnityEvent<bool> OnKilled { get; private set; }   // キルが発生した時のイベント
    
    public bool IsBeforeBattle { get; private set; }    // 戦闘前かどうか
    private PlayerComponents _playerComponents;

    private void OnEnable()
    {
        // 戦闘前の初期化
        IsBeforeBattle = true;
        _battleStartedText.gameObject.SetActive(false);
        _beforeBattleTimerText.gameObject.SetActive(true);

        GeneratePlayer();
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

    private void OnPlayerDied(GameObject player)
    {
        /// <summary>
        /// プレイヤーが死亡した時の処理
        /// </summary>
        Destroy(player);
        OnKilled?.Invoke(false);

        GeneratePlayer();
    }

    private void Update()
    {
        // 戦闘前のタイマーを更新
        if (IsBeforeBattle)
        {
            _beforeBattleTimer -= Time.deltaTime;
            _beforeBattleTimerText.text = Mathf.CeilToInt(_beforeBattleTimer).ToString();

            if (_beforeBattleTimer <= 0)
            {
                IsBeforeBattle = false;
                _beforeBattleTimerText.gameObject.SetActive(false);
                StartCoroutine(ShowBattleStartedText());
            }
        }
    }

    public void LoadOtherScene()
    {
        /// <summary>
        /// 他のシーンをロードする
        /// </summary>
        
        SceneManager.LoadScene(_nextSceneName);
    }

    private IEnumerator ShowBattleStartedText()
    {
        /// <summary>
        /// 戦闘開始のテキストのアニメーション
        /// </summary>
        
        _battleStartedText.gameObject.SetActive(true);
        yield return new WaitForSeconds(_startedTextDisplayTime);
        _battleStartedText.gameObject.SetActive(false);
    }
}
