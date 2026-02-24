using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Menu : MonoBehaviour
{
    [SerializeField] private  GameObject _EncyclopediaPanel;
    [SerializeField] private  GameObject _ParametresPanel;
    [SerializeField] private Button _ParametresButton;
    [SerializeField] private Button _EncyclopediaButton;
    [SerializeField] private Button _StartButton;
    [SerializeField] private Button _LeftTheGameButtonButton;
    public SaveScript _saveScript;

    private void Awake()
    {
        if (_saveScript != null)
        {
            _saveScript.LoadInfo();
        }
    }
    public void StartGame()
    {
        SceneManager.LoadScene("EnjminDiff");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OpenEncyclopedia()
    {
        _EncyclopediaPanel.SetActive(true);
        _ParametresButton.gameObject.SetActive(false);
        _StartButton.gameObject.SetActive(false);
        _LeftTheGameButtonButton.gameObject.SetActive(false);
        _EncyclopediaButton.gameObject.SetActive(false);
    }

    public void CloseEncyclopedia()
    {
        _EncyclopediaPanel.SetActive(false);
        _ParametresButton.gameObject.SetActive(true);
        _StartButton.gameObject.SetActive(true);
        _LeftTheGameButtonButton.gameObject.SetActive(true);
        _EncyclopediaButton.gameObject.SetActive(true);

    }

    public void OpenParametres()
    {
        _ParametresPanel.SetActive(true);
        _ParametresButton.gameObject.SetActive(false);
        _StartButton.gameObject.SetActive(false);
        _LeftTheGameButtonButton.gameObject.SetActive(false);
        _EncyclopediaButton.gameObject.SetActive(false);
    }

    public void CloseParametres()
    {
        _ParametresPanel.SetActive(false);
        _ParametresButton.gameObject.SetActive(true);
        _StartButton.gameObject.SetActive(true);
        _LeftTheGameButtonButton.gameObject.SetActive(true);
        _EncyclopediaButton.gameObject.SetActive(true);
    }
}
