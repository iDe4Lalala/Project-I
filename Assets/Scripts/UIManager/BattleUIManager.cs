using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleUIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _battleTimerText;
    [SerializeField] private float _battleTimer;
    [SerializeField] private TMP_Text _killText;
    [SerializeField] private float _killTextDisplayTime;
    [SerializeField] private string _playerKillSentence;
    [SerializeField] private string _enemyKillSentence;
    [SerializeField] private BattleSceneManager _battleSceneManager;

    private void OnEnable()
    {
        _killText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if(!_battleSceneManager.IsBeforeBattle)
        {
            if (_battleTimer >= 0)
            {
                _battleTimer -= Time.deltaTime;
                _battleTimerText.text = _battleTimer.ToString("f2");
            }
            else
            {
                _battleSceneManager.LoadOtherScene();
            }
        }
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

    private IEnumerator ShowKillText()
    {
        _killText.gameObject.SetActive(true);
        yield return new WaitForSeconds(_killTextDisplayTime);
        _killText.gameObject.SetActive(false);
    }
}
