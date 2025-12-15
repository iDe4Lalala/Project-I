using System.Collections;
using UnityEngine;
using TMPro;

public class BattleUIManager : MonoBehaviour
{
    /// <summary>
    /// 戦闘に関するUIを管理する
    /// </summary>
    
    [SerializeField] private TMP_Text _battleTimerText;     // 残り戦闘時間のテキスト
    [SerializeField] private float _battleTimer;   // 戦闘時間
    [SerializeField] private TMP_Text _killText;    // kill時テキスト
    [SerializeField] private float _killTextDisplayTime;   // killテキスト表示時間
    [SerializeField] private string _playerKillSentence;   // プレイヤーがキルした時の文章
    [SerializeField] private string _enemyKillSentence;    // 敵がキルした時の文章
    [SerializeField] private BattleSceneManager _battleSceneManager;

    private void OnEnable()
    {
        // killテキストを非表示
        _killText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if(!_battleSceneManager.IsBeforeBattle) return;

        // タイマーを更新
        if (_battleTimer >= 0)
        {
            _battleTimer -= Time.deltaTime;
            _battleTimerText.text = _battleTimer.ToString("f2");
        }
        else
        {
            // シーン遷移
            _battleSceneManager.LoadOtherScene();
        }
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
}
