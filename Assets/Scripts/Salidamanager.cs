using UnityEngine;
using UnityEngine.SceneManagement;

public class Salidamanager : MonoBehaviour
{

    public void OnStartClicked()
    {
        SceneManager.LoadScene("Inicio");
    }

    public void OnExitClicked()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;  // detiene el Play Mode
        #else
                        Application.Quit();  // funciona en la build final
        #endif
    }
}
