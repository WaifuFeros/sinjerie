using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager : MonoBehaviour
{
    public static SceneLoadManager Instance;

    public string ActiveSceneName { get; private set; }
    public string CurrentLoadingSceneName { get; private set; }
    public bool SceneLoadingLocked { get; private set; }

    public static bool IsManagerSceneLoaded { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            IsManagerSceneLoaded = true;
            ActiveSceneName = SceneManager.GetActiveScene().name;
            DontDestroyOnLoad(gameObject);
            return;
        }

        Destroy(gameObject);
    }

    public void LoadSceneAsActive(string sceneName)
    {
        if (SceneLoadingLocked)
            return;

        SceneManager.sceneLoaded += OnGameSceneLoaded;
        CurrentLoadingSceneName = sceneName;
        SceneLoadingLocked = true;
        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
    }

    private void OnGameSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == CurrentLoadingSceneName)
        {
            SceneManager.SetActiveScene(scene);
            SceneManager.sceneLoaded -= OnGameSceneLoaded;
            SceneManager.UnloadSceneAsync(ActiveSceneName);
            ActiveSceneName = CurrentLoadingSceneName;
            CurrentLoadingSceneName = null;
            SceneLoadingLocked = false;
        }
    }
}
