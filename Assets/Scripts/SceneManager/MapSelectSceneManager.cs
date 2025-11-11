using UnityEngine;
using UnityEngine.SceneManagement;

public class MapSelectSceneManager : MonoBehaviour
{
    /// <summary>
    ///  マップ選択シーンを管理する
    /// </summary>
    
    [SerializeField] private string _nextSceneName;     // 次のシーンの名前

    public void LoadOtherScene()
    {
        /// <summary>
        /// 武器選択シーンへ移動する
        /// </summary>
        
        SceneManager.LoadScene(_nextSceneName);
    }
}
