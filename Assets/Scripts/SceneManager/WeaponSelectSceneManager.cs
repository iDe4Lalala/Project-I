using UnityEngine;
using UnityEngine.SceneManagement;

public class WeaponSelectSceneManager : MonoBehaviour
{
    /// <summary>
    /// 武器選択シーンを管理する
    /// </summary>
    
    [SerializeField] private string _nextSceneName;     // 次のシーンの名前

    public void LoadOtherScene()
    {
        /// <summary>
        /// バトルシーンへ移動する
        /// </summary>

        SceneManager.LoadScene(_nextSceneName);
    }
}
