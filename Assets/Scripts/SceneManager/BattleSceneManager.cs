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
    [SerializeField] private TMP_Text _beforeBattleTimerText;   // 戦闘前のタイマーのテキスト
    [SerializeField] private TMP_Text _battleStartedText;   // 戦闘開始のテキスト
    [SerializeField] private float _beforeBattleTimer;   // 戦闘前のタイマーの時間
    [SerializeField] private float _startedTextDisplayTime;   // 戦闘開始テキストの表示時間
    [SerializeField] private string _resultSceneName;   // 結果シーンの名前
    [SerializeField] private OperationUIManager _operationUIManager;
    [SerializeField] private Canvas _battleCanvas;
    [SerializeField] private UnityEvent<bool> OnKilledEnemy;   // 敵をキルした時のイベント
    
    public bool IsBeforeBattle { get; private set; }    // 戦闘前かどうか
    private GameObject _player;     // プレイヤー
    private List<GameObject> _enemyList = new();   // 敵のリスト
    private Dictionary<HumanType, GameObject> _humanDict;  // 人の辞書

    private void OnEnable()
    {
        // 戦闘前の初期化
        IsBeforeBattle = true;
        _battleStartedText.gameObject.SetActive(false);
        _beforeBattleTimerText.gameObject.SetActive(true);
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
            _player = Instantiate(playerObject, Vector3.zero, Quaternion.identity);
            PlayerComponents playerComponents = _player.GetComponent<PlayerComponents>();
            playerComponents.AspectRatioManager.SetCanvas(_battleCanvas);
            GameObject rifle = Instantiate(_riflePrefab, playerComponents.RifleSocket.transform);
            playerComponents.SetRifleManager(rifle);
            _operationUIManager.SetPlayer(_player);
        }
    }

    private void GenerateEnemies()
    {
        /// <summary>
        /// 敵を生成する
        /// </summary>
        
        if (_humanDict.TryGetValue(HumanType.Enemy, out GameObject enemyObject))    // 敵を生成
        {
            GameObject enemy = Instantiate(enemyObject,
                new Vector3(Random.Range(-10f, 10f), 0, Random.Range(-10f, 10f)), Quaternion.identity);
            EnemyComponents enemyComponents = enemy.GetComponent<EnemyComponents>();
            GameObject rifle = Instantiate(_riflePrefab, enemyComponents.RifleSocket.transform);
            enemyComponents.SetRifleManager(rifle);
            _enemyList.Add(enemy);
            enemyComponents.EnemyManager.OnDeath += OnEnemyDied;
        }
    }

    private void OnEnemyDied(GameObject enemy)
    {
        /// <summary>
        /// 敵が死亡した時の処理
        /// </summary>

        _enemyList.Remove(enemy);
        enemy.GetComponent<EnemyComponents>().EnemyManager.OnDeath -= OnEnemyDied;
        Destroy(enemy);
        OnKilledEnemy?.Invoke(true);

        CountNumberOfEnemies();
    }
    
    private void CountNumberOfEnemies()
    {
        /// <summary>
        /// 敵の数を数える
        /// </summary>

        if (_enemyList.Count > 0) return;
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
