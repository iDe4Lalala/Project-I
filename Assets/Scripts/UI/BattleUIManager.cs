using System.Collections;
using UnityEngine;
using TMPro;
using System;

public class BattleUIManager : MonoBehaviour
{
    /// <summary>
    /// 戦闘に関するUIを管理する
    /// </summary>
    
    [SerializeField] private TMP_Text _battleTimerText;     // 残り戦闘時間のテキスト
    [SerializeField] private TMP_Text _killText;    // kill時テキスト
    [SerializeField] private float _killTextDisplayTime;   // killテキスト表示時間
    [SerializeField] private string _playerKillSentence;   // プレイヤーがキルした時の文章
    [SerializeField] private string _enemyKillSentence;    // 敵がキルした時の文章
    [SerializeField] private TMP_Text _beforeBattleTimerText;   // 戦闘前のタイマーのテキスト
    [SerializeField] private TMP_Text _battleProgressText;   // 戦闘進行状況のテキスト
    [SerializeField] private float _beforeBattleTimer;   // 戦闘前の待ち時間
    [SerializeField] private float _progressTextDisplayTime;   // 戦闘進行状況テキストの表示時間
    [SerializeField] private BattleSceneManager _battleSceneManager;
    [SerializeField] private EnemyAppearanceManager _enemyAppearanceManager;

    public event Action OnStartBattle;    // 戦闘開始時のイベント

    private void OnEnable()
    {
        // イベント登録
        _battleSceneManager.OnTimerUpdated += SetBattleTimer;
        _enemyAppearanceManager.OnWaveStarted += OnNextWaveStarted;

        _killText.gameObject.SetActive(false);
        _battleProgressText.gameObject.SetActive(false);
        _beforeBattleTimerText.gameObject.SetActive(true);

        StartCoroutine(ShowBeforeBattleTimerText());
    }

    private void OnDisable()
    {
        // イベント解除
        _battleSceneManager.OnTimerUpdated -= SetBattleTimer;
        _enemyAppearanceManager.OnWaveStarted -= OnNextWaveStarted;
    }

    public void SetBattleTimer(float time)
    {
        /// <summary>
        /// 戦闘タイマーを設定する
        /// </summary>
        
        int m = (int)(time / 60);
        int s = (int)(time % 60);

        _battleTimerText.text = $"{m:00}:{s:00}";
    }

    public void OnKill(bool isPlayerKill)
    {
        /// <summary>
        /// kill時のUIを表示する
        /// </summary>
        
        if (isPlayerKill)
        {
            _killText.text = _playerKillSentence;
        }
        else
        {
            _killText.text = _enemyKillSentence;
        }
        
        StartCoroutine(ShowKillText());
    }

    private IEnumerator ShowKillText()
    {
        /// <summary>
        /// killテキストのアニメーション
        /// </summary>

        _killText.gameObject.SetActive(true);
        yield return new WaitForSeconds(_killTextDisplayTime);
        _killText.gameObject.SetActive(false);
    }

    private IEnumerator ShowBeforeBattleTimerText()
    {
        /// <summary>
        /// 戦闘前タイマーのアニメーション
        /// </summary>

        WaitForSeconds waitForSeconds = new (1f);
        _beforeBattleTimerText.gameObject.SetActive(true);

        for (int i = Mathf.CeilToInt(_beforeBattleTimer); i >= 1; i--)
        {
            _beforeBattleTimerText.text = i.ToString();
            yield return waitForSeconds;
        }

        _beforeBattleTimerText.gameObject.SetActive(false);
        OnStartBattle?.Invoke();
    }

    private IEnumerator ShowBattleStartedText()
    {
        /// <summary>
        /// 戦闘開始のテキストのアニメーション
        /// </summary>
        
        _battleProgressText.gameObject.SetActive(true);
        yield return new WaitForSeconds(_progressTextDisplayTime);
        _battleProgressText.gameObject.SetActive(false);
    }

    public void OnNextWaveStarted(string waveText)
    {
        /// <summary>
        /// 次のウェーブ開始時のテキスト表示処理
        /// </summary>
        
        _battleProgressText.text = $"{waveText} Wave Start";
        StartCoroutine(ShowBattleStartedText());
    }

    public void OnThisWaveCleared(string waveText)
    {
        _battleProgressText.text = $"{waveText} Wave Clear";
        StartCoroutine(ShowBattleStartedText());
    }
}
