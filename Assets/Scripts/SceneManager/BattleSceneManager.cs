using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class BattleSceneManager : MonoBehaviour
{
    /// <summary>
    /// 戦闘シーンを管理する
    /// </summary>

    [field: SerializeField] public List<HumanDataBase> HumanDataBasesList { get; private set; }   // 人のデータベースリスト
    [SerializeField] private TMP_Text _beforeBattleTimerText;   // 戦闘前のタイマーのテキスト
    [SerializeField] private TMP_Text _battleStartedText;   // 戦闘開始のテキスト
    [SerializeField] private float _beforeBattleTimer;   // 戦闘前のタイマーの時間
    [SerializeField] private float _startedTextDisplayTime;   // 戦闘開始テキストの表示時間
    [SerializeField] private string _resultSceneName;   // 結果シーンの名前
    [SerializeField] private OperationUIManager _operationUIManager;
    [SerializeField] private Canvas _battleCanvas;
    
    public bool IsBeforeBattle { get; private set; }    // 戦闘前かどうか
    private GameObject _player;     // プレイヤー

    private void OnEnable()
    {
        // 戦闘前の初期化
        IsBeforeBattle = true;
        _battleStartedText.gameObject.SetActive(false);
        _beforeBattleTimerText.gameObject.SetActive(true);
        GenerateHumans();
    }

    private void GenerateHumans()
    {
        /// <summary>
        /// 人を生成する
        /// </summary>
        
        foreach (var humanDataBase in HumanDataBasesList)
        {
            if (humanDataBase.HumanType == HumanType.Player)    // プレイヤーを生成
            {
                _player = Instantiate(humanDataBase.HumanObject, Vector3.zero, Quaternion.identity);
                _player.GetComponent<PlayerComponents>().AspectRatioManager.SetCanvas(_battleCanvas);
                _operationUIManager.SetPlayer(_player);
            }
            else if (humanDataBase.HumanType == HumanType.Enemy)    // 敵を生成
            {
                Instantiate(humanDataBase.HumanObject, 
                    new Vector3(Random.Range(-10f, 10f), 0, Random.Range(-10f, 10f)), Quaternion.identity);
            }
        }
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
