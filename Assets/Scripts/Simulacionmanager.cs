using UnityEngine;
using UnityEngine.SceneManagement;

public class Simulacionmanager : MonoBehaviour
{
    
    public void OnStartClicked()
    {
        SceneManager.LoadScene("Inicio");
    }

    public void OnExitClicked()
    {
        Application.Quit();
        SceneManager.LoadScene("Inicio");
    }
}
