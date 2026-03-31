using System.Collections;
using UnityEngine;
using TMPro;
using System;

public class BattleUIManager : MonoBehaviour, IBattleUIService
{
    [SerializeField] private TMP_Text _battleTimerText;
    [SerializeField] private TMP_Text _killText;
    [SerializeField] private float _killTextDisplayTime;
    [SerializeField] private string _playerKilledSentence;
    [SerializeField] private string _enemyKilledSentence;
    [SerializeField] private TMP_Text _beforeBattleTimerText;
    [SerializeField] private TMP_Text _battleProgressText;
    [SerializeField] private float _progressTextDisplayTime;
    private BattleSceneManager _battleSceneManager;

    public void Initialize(BattleSceneManager battleSceneManager)
    {
        _battleSceneManager = battleSceneManager;
        _battleSceneManager.TimerUpdated += UpdateTimer;

        _killText.gameObject.SetActive(false);
        _battleProgressText.gameObject.SetActive(false);
        _beforeBattleTimerText.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        _battleSceneManager.TimerUpdated -= UpdateTimer;
    }

    public void UpdateTimer(float time)
    {
        var leftTime = TimeSpan.FromSeconds(Mathf.Max(0f, time));
        _battleTimerText.text = $"{leftTime.Minutes:00}:{leftTime.Seconds:00}";
    }

    public void OnPlayerKilled()
    {
        _killText.text = _playerKilledSentence;
        StartCoroutine(ShowKillTextForSeconds(_killTextDisplayTime));
    }

    public void OnEnemyKilled()
    {
        _killText.text = _playerKilledSentence;
        StartCoroutine(ShowKillTextForSeconds(_killTextDisplayTime));
    }

    public IEnumerator OnNextWaveStarted(string text)
    {
        _battleProgressText.text = $"{text} Wave Start";
        yield return StartCoroutine(ShowProgressTextForSeconds(_progressTextDisplayTime));
    }

    public IEnumerator OnThisWaveCleared(string text)
    {
        _battleProgressText.text = $"{text} Wave Clear";
        yield return StartCoroutine(ShowProgressTextForSeconds(_progressTextDisplayTime));
    }

    public IEnumerator OnGameCleared()
    {
        _battleProgressText.text = "Game Clear!";
        yield return StartCoroutine(ShowProgressTextForSeconds(_progressTextDisplayTime));
    }

    public IEnumerator OnGameOvered()
    {
        _battleProgressText.text = "Game Over";
        yield return StartCoroutine(ShowProgressTextForSeconds(_progressTextDisplayTime));
    }

    public IEnumerator PlayCountdown(float countdownTime)
    {
        WaitForSeconds waitForSeconds = new (1f);
        _beforeBattleTimerText.gameObject.SetActive(true);

        for (int i = Mathf.CeilToInt(countdownTime); i >= 1; i--)
        {
            _beforeBattleTimerText.text = i.ToString();
            yield return waitForSeconds;
        }

        _beforeBattleTimerText.gameObject.SetActive(false);
    }

    public IEnumerator ShowKillTextForSeconds(float seconds)
    {
        _killText.gameObject.SetActive(true);
        yield return new WaitForSeconds(seconds);
        _killText.gameObject.SetActive(false);
    }

    public IEnumerator ShowProgressTextForSeconds(float seconds)
    {
        _battleProgressText.gameObject.SetActive(true);
        yield return new WaitForSeconds(seconds);
        _battleProgressText.gameObject.SetActive(false);
    }
}
