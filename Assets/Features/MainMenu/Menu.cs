using UnityEngine;
using UnityEngine.SceneManagement;


public class Menu : MonoBehaviour
{
    [SerializeField] private  GameObject _encyclopedia;
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
        _encyclopedia.SetActive(true);
    }

    public void CloseEncyclopedia()
        {
            _encyclopedia.SetActive(false);
    }
}
