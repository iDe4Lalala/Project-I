using System.Collections;
using UnityEngine;
using TMPro;
using System;

public class BattleUIManager : MonoBehaviour, IBattleUIService
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
        _battleSceneManager.OnTimerUpdated += UpdateTimer;
        _enemyAppearanceManager.OnWaveStarted += OnNextWaveStarted;

        _killText.gameObject.SetActive(false);
        _battleProgressText.gameObject.SetActive(false);
        _beforeBattleTimerText.gameObject.SetActive(true);

        StartCoroutine(PlayCountdown(_beforeBattleTimer));
    }

    private void OnDisable()
    {
        _battleSceneManager.OnTimerUpdated -= UpdateTimer;
        _enemyAppearanceManager.OnWaveStarted -= OnNextWaveStarted;
    }

    public void UpdateTimer(float time)
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
        
        StartCoroutine(ShowKillTextForSeconds(_killText.text, _killTextDisplayTime));
    }

    public void OnNextWaveStarted(string waveText)
    {
        _battleProgressText.text = $"{waveText} Wave Start";
        StartCoroutine(ShowProgressTextForSeconds(_battleProgressText.text, _progressTextDisplayTime));
    }

    public void OnThisWaveCleared(string waveText)
    {
        _battleProgressText.text = $"{waveText} Wave Clear";
        StartCoroutine(ShowProgressTextForSeconds(_battleProgressText.text, _progressTextDisplayTime));
    }

    public IEnumerator ShowKillTextForSeconds(string text, float seconds)
    {
        _killText.gameObject.SetActive(true);
        yield return new WaitForSeconds(seconds);
        _killText.gameObject.SetActive(false);
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
        OnStartBattle?.Invoke();
    }

    public IEnumerator ShowProgressTextForSeconds(string text, float seconds)
    {
        _battleProgressText.gameObject.SetActive(true);
        yield return new WaitForSeconds(seconds);
        _battleProgressText.gameObject.SetActive(false);
    }
}
