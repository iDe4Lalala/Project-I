using UnityEngine;
using UnityEngine.SceneManagement;

public class StartSceneManager : MonoBehaviour
{
    [SerializeField] private string _nextSceneName;

    public void LoadOtherScene()
    {
        SceneManager.LoadScene(_nextSceneName);
    }
}
