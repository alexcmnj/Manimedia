using UnityEngine;
using UnityEngine.SceneManagement;

public class Iniciomanager : MonoBehaviour
{
    public GameObject instructionsPanel;

    public void OnStartClicked()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void OnInstructionsClicked()
    {
        instructionsPanel.SetActive(true);
    }

    public void OnCloseInstructions()
    {
        instructionsPanel.SetActive(false);
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