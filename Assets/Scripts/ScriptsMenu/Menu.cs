using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Menu : MonoBehaviour
{

    public GameObject gameObjectsAparecer;

    public GameObject gameObjectsSumir;
    public void botaoNovoJogo()
    {
        SceneManager.LoadScene("Casa Inicial");
    }
    public void botaoSair()
    {
        Application.Quit();
    }

    public void botaoTrocar()
    {
        gameObjectsAparecer.SetActive(true);
        gameObjectsSumir.SetActive(false);
    }
}
