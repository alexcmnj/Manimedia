using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI General")]
    public Slider progressBar;
    public TextMeshProUGUI timerText;

    [Header("Paneles")]
    public GameObject blowingPanel;
    public GameObject finishedPanel;   // tu panel anterior (puedes desactivarlo si ya no lo usas)
    public GameObject victoryPanel;    // ✅ Panel nuevo de victoria

    [Header("Victory Panel - Referencias")]
    public TextMeshProUGUI victoryTitleText;   // "¡Ganaste!" o lo que quieras
    public TextMeshProUGUI victoryTimeText;    // Muestra el tiempo final
    public Button restartButton;
    public Button continueButton;

    [Header("Escenas")]
    public string outroSceneName = "Salida"; 

    private float elapsedTime = 0f;
    private bool counting = false;

    // ─────────────────────────────────────────────────────────────
    void Awake() => Instance = this;

    void Start()
    {
        ResetUI();

        // Asignar listeners a los botones
        if (restartButton != null)
            restartButton.onClick.AddListener(OnRestart);

        if (continueButton != null)
            continueButton.onClick.AddListener(OnContinue);
    }

    void Update()
    {
        if (!counting) return;

        elapsedTime += Time.deltaTime;
        int min = (int)(elapsedTime / 60f);
        int sec = (int)(elapsedTime % 60f);
        timerText.text = $"{min:00}:{sec:00}";
    }

    // ─── Mostrar UI de soplido ────────────────────────────────────
    public void ShowBlowingUI()
    {
        blowingPanel.SetActive(true);
        counting = true;
        elapsedTime = 0f;
    }

    // ─── Mostrar panel de victoria ────────────────────────────────
    public void ShowFinishedUI()
    {
        counting = false;

        // Ocultar UI de juego
        blowingPanel.SetActive(false);

        // Mostrar panel de victoria después de un delay
        // para que el usuario vea el logo emerger primero
        StartCoroutine(ShowVictoryAfterDelay());
    }

    System.Collections.IEnumerator ShowVictoryAfterDelay()
    {
        yield return new WaitForSeconds(9.5f);

        // Tiempo final formateado
        int min = (int)(elapsedTime / 60f);
        int sec = (int)(elapsedTime % 60f);

        if (victoryTitleText != null)
            victoryTitleText.text = "¡Ganaste!";

        if (victoryTimeText != null)
            victoryTimeText.text = $"Tiempo: {min:00}:{sec:00}";

        if (victoryPanel != null)
            victoryPanel.SetActive(true);
    }

    // ─── Botón Reiniciar ──────────────────────────────────────────
    void OnRestart()
    {
        // Ocultar panel antes de reiniciar
        if (victoryPanel != null)
            victoryPanel.SetActive(false);

        GameManager.Instance.ResetSimulation();
    }

    // ─── Botón Continuar ──────────────────────────────────────────
    void OnContinue()
    {
        SceneManager.LoadScene(outroSceneName);
    }

    // ─── Helpers públicos ─────────────────────────────────────────
    public void UpdateProgressBar(float ratio) => progressBar.value = ratio;

    public float GetElapsedTime() => elapsedTime;

    public void ResetUI()
    {
        progressBar.value = 0f;
        elapsedTime = 0f;
        counting = false;

        blowingPanel.SetActive(false);

        if (finishedPanel != null)
            finishedPanel.SetActive(false);

        if (victoryPanel != null)
            victoryPanel.SetActive(false);

        if (timerText != null)
            timerText.text = "00:00";
    }
}