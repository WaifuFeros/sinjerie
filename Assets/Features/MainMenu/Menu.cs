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

        });
    }

    public void StartTutorialGame()
    {
        TutorialManager.Instance.IsTutorial = true;
        TransitionManager.Instance.TransitionWithAction(() => {
            SceneLoadManager.Instance.LoadSceneAsActive(_gameSceneName);

        });
    }

    public void OpenEncyclopedia()
    {
        _EncyclopediaPanel.SetActive(true);
    }

    public void CloseEncyclopedia()
    {
        _EncyclopediaPanel.SetActive(false);

    }

    public void OpenParametres()
    {
        _ParametresPanel.SetActive(true);
    }

    public void CloseParametres()
    {
        _ParametresPanel.SetActive(false);
    }
}
