using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Events;

public class BattleSceneManager : MonoBehaviour
{
    /// <summary>
    /// 戦闘シーンを管理する
    /// </summary>

    [field: SerializeField] public List<HumanDataBase> HumanDataBasesList { get; private set; }   // 人のデータベースリスト
    [SerializeField] private GameObject _riflePrefab;  // ライフル
    [SerializeField] private Transform _respawnPointParent;
    [SerializeField] private TMP_Text _beforeBattleTimerText;   // 戦闘前のタイマーのテキスト
    [SerializeField] private TMP_Text _battleStartedText;   // 戦闘開始のテキスト
    [SerializeField] private float _beforeBattleTimer;   // 戦闘前のタイマーの時間
    [SerializeField] private float _startedTextDisplayTime;   // 戦闘開始テキストの表示時間
    [SerializeField] private string _resultSceneName;   // 結果シーンの名前
    [SerializeField] private Canvas _battleCanvas;
    [SerializeField] private PlayerUIManager _playerUIManager;
    [SerializeField] private UnityEvent<bool> OnKilled;   // キルが発生した時のイベント
    
    public bool IsBeforeBattle { get; private set; }    // 戦闘前かどうか
    private PlayerComponents _playerComponents;
    private List<EnemyComponents> _enemyComponentsList;
    private Dictionary<HumanType, GameObject> _humanDict;  // 人の辞書
    private Transform[] _respawnPoints;

    private void OnEnable()
    {
        // 戦闘前の初期化
        IsBeforeBattle = true;
        _battleStartedText.gameObject.SetActive(false);
        _beforeBattleTimerText.gameObject.SetActive(true);
        _enemyComponentsList = new List<EnemyComponents>();

        int count = _respawnPointParent.childCount;
        _respawnPoints = new Transform[count];

        for (int i = 0; i < count; i++)
        {
            _respawnPoints[i] = _respawnPointParent.GetChild(i);
        }

        SetHumanDictionary();
        GeneratePlayer();
        GenerateEnemies();
    }

    private void SetHumanDictionary()
    {
        /// <summary>
        /// 人を辞書に設定する
        /// </summary>

        _humanDict = new Dictionary<HumanType, GameObject>();

        foreach (var humanDataBase in HumanDataBasesList)
        {
            _humanDict[humanDataBase.HumanType] = humanDataBase.HumanObject;
        }
    }

    private void GeneratePlayer()
    {
        /// <summary>
        /// プレイヤーを生成する
        /// </summary>
        
        if (_humanDict.TryGetValue(HumanType.Player, out GameObject playerObject))
        {
            GameObject player = Instantiate(playerObject);
            SetRespawnPoint(player, true);

            _playerComponents = player.GetComponent<PlayerComponents>();
            _playerComponents.AspectRatioManager.SetCanvas(_battleCanvas);
            _playerComponents.PlayerManager.OnDied += OnPlayerDied;

            GameObject rifle = Instantiate(_riflePrefab, _playerComponents.RifleSocket.transform);
            _playerComponents.SetRifleManager(rifle);

            _playerUIManager.SetPlayer(player);
        }
    }

    private void GenerateEnemies()
    {
        /// <summary>
        /// 敵を生成する
        /// </summary>
        
        if (_humanDict.TryGetValue(HumanType.Enemy, out GameObject enemyObject))    // 敵を生成
        {
            GameObject enemy = Instantiate(enemyObject);
            SetRespawnPoint(enemy, false);

            EnemyComponents enemyComponents = enemy.GetComponent<EnemyComponents>();
            _enemyComponentsList.Add(enemyComponents);
            enemyComponents.EnemyManager.OnDied += OnEnemyDied;
            enemyComponents.EnemyManager.OnDamaged += OnEnemyDamaged;

            GameObject rifle = Instantiate(_riflePrefab, enemyComponents.RifleSocket.transform);
            enemyComponents.SetRifleManager(rifle);
        }
    }

    private void SetRespawnPoint(GameObject human, bool isPlayer)
    {
        /// <summary>
        /// リスポーンポイントを設定する
        /// </summary>
        
        if (isPlayer)
        {
            human.transform.SetPositionAndRotation(_respawnPoints[0].position, _respawnPoints[0].rotation);
            return;
        }

        int rand = Random.Range(1, _respawnPoints.Length - 1);
        human.transform.SetPositionAndRotation(_respawnPoints[rand].position, _respawnPoints[rand].rotation);
    }

    private void OnEnemyDied(GameObject enemy, EnemyComponents enemyComponents)
    {
        /// <summary>
        /// 敵が死亡した時の処理
        /// </summary>

        enemyComponents.EnemyManager.OnDied -= OnEnemyDied;
        enemyComponents.EnemyManager.OnDamaged -= OnEnemyDamaged;
        
        _enemyComponentsList.Remove(enemyComponents);
        Destroy(enemy);
        OnKilled?.Invoke(true);

        CountNumberOfEnemies();
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

    private void OnEnemyDamaged()
    {
        /// <summary>
        /// 敵がダメージを受けた時の処理
        /// </summary>
        
        StartCoroutine(_playerUIManager.ShowHitCrossHair());
    }
    
    private void CountNumberOfEnemies()
    {
        /// <summary>
        /// 敵の数を数える
        /// </summary>

        if (_enemyComponentsList.Count > 0) return;
        GenerateEnemies();
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
        
        SceneManager.LoadScene(_resultSceneName);
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
