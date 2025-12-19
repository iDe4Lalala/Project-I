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
    [SerializeField] private TMP_Text _battleStartedText;   // 戦闘開始のテキスト
    [SerializeField] private float _beforeBattleTimer;   // 戦闘前の待ち時間
    [SerializeField] private float _startedTextDisplayTime;   // 戦闘開始テキストの表示時間
    [SerializeField] private BattleSceneManager _battleSceneManager;
    public event Action OnStartBattle;    // 戦闘開始時のイベント

    private void OnEnable()
    {
        // イベント登録
        _battleSceneManager.OnTimerUpdated += SetBattleTimer;

        // killテキストを非表示
        _killText.gameObject.SetActive(false);
        _battleStartedText.gameObject.SetActive(false);
        _beforeBattleTimerText.gameObject.SetActive(true);

        StartCoroutine(ShowBeforeBattleTimerText());
    }

    private void OnDisable()
    {
        // イベント解除
        _battleSceneManager.OnTimerUpdated -= SetBattleTimer;
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
        StartCoroutine(ShowBattleStartedText());
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
