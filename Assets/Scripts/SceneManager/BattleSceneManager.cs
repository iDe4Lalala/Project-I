using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class BattleSceneManager : MonoBehaviour
{
    [field: SerializeField] public List<HumanDataBase> HumanDataBasesList { get; private set; }
    [SerializeField] private TMP_Text _beforeBattleTimerText;
    [SerializeField] private TMP_Text _battleStartedText;
    [SerializeField] private float _beforeBattleTimer;
    [SerializeField] private float _startedTextDisplayTime;
    [SerializeField] private string _resultSceneName;
    [SerializeField] private OperationUIManager _operationUIManager;
    
    public bool IsBeforeBattle { get; private set; }
    private GameObject _player;

    private void OnEnable()
    {
        IsBeforeBattle = true;
        _battleStartedText.gameObject.SetActive(false);
        _beforeBattleTimerText.gameObject.SetActive(true);
        GenerateHumans();
    }

    private void GenerateHumans()
    {
        foreach (var humanDataBase in HumanDataBasesList)
        {
            if (humanDataBase.HumanType == HumanType.Player)
            {
                _player = Instantiate(humanDataBase.HumanObject, Vector3.zero, Quaternion.identity);
                _operationUIManager.SetPlayer(_player);
            }
            else if (humanDataBase.HumanType == HumanType.Enemy)
            {
                Instantiate(humanDataBase.HumanObject, new Vector3(Random.Range(-10f, 10f), 0, Random.Range(-10f, 10f)), Quaternion.identity);
            }
        }
    }

    private void Update()
    {
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
        SceneManager.LoadScene(_resultSceneName);
    }

    private IEnumerator ShowBattleStartedText()
    {
        _battleStartedText.gameObject.SetActive(true);
        yield return new WaitForSeconds(_startedTextDisplayTime);
        _battleStartedText.gameObject.SetActive(false);
    }
}
