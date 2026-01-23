using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ResultSceneManager : MonoBehaviour
{
    [SerializeField] private string _nextSceneName;     // 次のシーンの名前
    [SerializeField] private TextMeshProUGUI _resultText; // 結果表示用テキスト
    [SerializeField] private InGameDataBase _inGameDataBase;

    private void Start()
    {
        if (_inGameDataBase.IsGameCleared)
        {
            _resultText.text = "Game Clear";
        }
        else
        {
            _resultText.text = "Game Over";
        }
    }

    public void LoadOtherScene()
    {
        /// <summary>
        /// スタートシーンへ移動する
        /// </summary>
        
        SceneManager.LoadScene(_nextSceneName);
    }
}
