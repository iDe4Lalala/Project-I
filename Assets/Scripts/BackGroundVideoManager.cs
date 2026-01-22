using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class BackGroundVideoManager : MonoBehaviour
{
    [SerializeField] private VideoPlayer _videoPlayer;
    [SerializeField] private string _battleSceneName;

    private void Awake()
    {    
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == _battleSceneName)
        {
            _videoPlayer.gameObject.SetActive(false);
            var managers = Object.FindObjectsByType<BackGroundVideoManager>(FindObjectsSortMode.None);
            if (managers.Length <= 1) return;
            Destroy(gameObject);
        }
        else
        {
            _videoPlayer.gameObject.SetActive(true);
        }
    }
}
