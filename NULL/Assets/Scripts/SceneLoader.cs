using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    void Awake()
    {
        
        SceneManager.LoadSceneAsync("MassageScene", LoadSceneMode.Additive);
        SceneManager.LoadSceneAsync("UIScene", LoadSceneMode.Additive);
    }
}