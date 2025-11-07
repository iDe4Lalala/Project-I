using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartSceneManager : MonoBehaviour
{
    [SerializeField] private string _nextSceneName;

    public void moveMapSerect()
    {
        SceneManager.LoadScene(_nextSceneName);
    }
}
