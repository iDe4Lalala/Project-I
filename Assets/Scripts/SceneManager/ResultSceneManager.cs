using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ResultSceneManager : MonoBehaviour
{
    [SerializeField] private string _nextSceneName;
    [SerializeField] private TextMeshProUGUI _resultText;
    [SerializeField] private InGameDataBase _inGameDataBase;

    private void Start()
    {
        _resultText.text = _inGameDataBase.BattleResult switch
        {
            BattleResultType.GameClear => "Game Clear",
            BattleResultType.GameOver => "Game Over",
            _ => "Game Over",
        };
    }

    public void LoadOtherScene()
    {
        SceneManager.LoadScene(_nextSceneName);
    }
}
