using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartSceneManager : MonoBehaviour
{
    /// <summary>
    /// スタートシーンを管理する
    /// </summary>
    
    [SerializeField] private string _nextSceneName;     // 次のシーンの名前

    public void LoadOtherScene()
    {
        /// <summary>
        /// マップ選択シーンへ移動する
        /// </summary>
        
        SceneManager.LoadScene(_nextSceneName);
    }
}
