using System.Collections;
using UnityEngine;
using TMPro;
using System;

public class BattleUIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _battleTimerText;
    [SerializeField] private TMP_Text _killText;
    [SerializeField] private float _killTextDisplayTime;
    [SerializeField] private string _playerKillSentence;
    [SerializeField] private string _enemyKillSentence;
    [SerializeField] private TMP_Text _beforeBattleTimerText;
    [SerializeField] private TMP_Text _battleProgressText;
    [SerializeField] private float _beforeBattleTimer;
    [SerializeField] private float _progressTextDisplayTime;
    [SerializeField] private BattleSceneManager _battleSceneManager;
    [SerializeField] private EnemyAppearanceManager _enemyAppearanceManager;

    public event Action OnStartBattle;

    private void OnEnable()
    {
        _battleSceneManager.OnTimerUpdated += SetBattleTimer;
        _enemyAppearanceManager.OnWaveStarted += OnNextWaveStarted;

        _killText.gameObject.SetActive(false);
        _battleProgressText.gameObject.SetActive(false);
        _beforeBattleTimerText.gameObject.SetActive(true);

        StartCoroutine(ShowBeforeBattleTimerText());
    }

    private void OnDisable()
    {
        _battleSceneManager.OnTimerUpdated -= SetBattleTimer;
        _enemyAppearanceManager.OnWaveStarted -= OnNextWaveStarted;
    }

    public void SetBattleTimer(float time)
    {
        int m = (int)(time / 60);
        int s = (int)(time % 60);

        _battleTimerText.text = $"{m:00}:{s:00}";
    }

    public void OnKill(bool isPlayerKill)
    {
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

    public void OnNextWaveStarted(string waveText)
    {
        _battleProgressText.text = $"{waveText} Wave Start";
        StartCoroutine(ShowBattleStartedText());
    }

    public void OnThisWaveCleared(string waveText)
    {
        _battleProgressText.text = $"{waveText} Wave Clear";
        StartCoroutine(ShowBattleStartedText());
    }

    private IEnumerator ShowKillText()
    {
        _killText.gameObject.SetActive(true);
        yield return new WaitForSeconds(_killTextDisplayTime);
        _killText.gameObject.SetActive(false);
    }

    private IEnumerator ShowBeforeBattleTimerText()
    {
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
        _battleProgressText.gameObject.SetActive(true);
        yield return new WaitForSeconds(_progressTextDisplayTime);
        _battleProgressText.gameObject.SetActive(false);
    }
}
