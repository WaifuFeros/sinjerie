using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using FMODUnity;


public class Menu : MonoBehaviour
{
    [SerializeField] private EventReference clickSound;
    [SerializeField] private  GameObject _EncyclopediaPanel;
    [SerializeField] private  GameObject _ParametresPanel;
    [SerializeField] private Button _ParametresButton;
    [SerializeField] private Button _EncyclopediaButton;
    [SerializeField] private Button _StartButton;
    [SelectScene]
    [SerializeField] private string _gameSceneName;
    public SaveScript _saveScript;

    [SelectScene]
    public string managersScene;

    private void Awake()
    {
        if (!string.IsNullOrWhiteSpace(managersScene) && SceneLoadManager.IsManagerSceneLoaded == false)
            SceneManager.LoadScene(managersScene, LoadSceneMode.Additive);

        if (_saveScript != null)
        {
            _saveScript.LoadInfo();
        }
    }
    public void StartGame()
    {
        TransitionManager.Instance.TransitionWithAction(() => {
            SceneLoadManager.Instance.LoadSceneAsActive(_gameSceneName);
            //SceneManager.sceneLoaded += OnGameSceneLoaded;
            //SceneManager.LoadScene(_gameSceneName, LoadSceneMode.Additive);

        });
    }

    //private void OnGameSceneLoaded(Scene scene, LoadSceneMode mode)
    //{
    //    if (scene.name == _gameSceneName)
    //    {
    //        SceneManager.SetActiveScene(scene);
    //        SceneManager.sceneLoaded -= OnGameSceneLoaded;
    //        SceneManager.UnloadSceneAsync("HomeMenu");
    //    }
    //}

    public void OpenEncyclopedia()
    {
        _EncyclopediaPanel.SetActive(true);
        _ParametresButton.gameObject.SetActive(false);
        _StartButton.gameObject.SetActive(false);
        _EncyclopediaButton.gameObject.SetActive(false);
    }

    public void CloseEncyclopedia()
    {
        _EncyclopediaPanel.SetActive(false);
        _ParametresButton.gameObject.SetActive(true);
        _StartButton.gameObject.SetActive(true);
        _EncyclopediaButton.gameObject.SetActive(true);

    }

    public void OpenParametres()
    {
        _ParametresPanel.SetActive(true);
        _ParametresButton.gameObject.SetActive(false);
        _StartButton.gameObject.SetActive(false);
        _EncyclopediaButton.gameObject.SetActive(false);
    }

    public void CloseParametres()
    {
        _ParametresPanel.SetActive(false);
        _ParametresButton.gameObject.SetActive(true);
        _StartButton.gameObject.SetActive(true);
        _EncyclopediaButton.gameObject.SetActive(true);
    }
}
