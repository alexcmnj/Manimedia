using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public Slider progressBar;
    public TextMeshProUGUI timerText;

    public GameObject blowingPanel;
    public GameObject finishedPanel;

    [Header("Botón Saltar simulación")]
    public Button skipButton; // Asignar en el Inspector

    private float elapsedTime = 0f;
    private bool counting = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        ResetUI();

        if (skipButton != null)
            skipButton.onClick.AddListener(OnSkipClicked);
    }

    void Update()
    {
        if (!counting) return;

        elapsedTime += Time.deltaTime;

        int min = (int)(elapsedTime / 60f);
        int sec = (int)(elapsedTime % 60f);

        timerText.text = $"{min:00}:{sec:00}";
    }

    // ──────────────────────────────────────────────
    // Botón Saltar
    // ──────────────────────────────────────────────

    void OnSkipClicked()
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.currentPhase != GameManager.GamePhase.Falling) return;

        GameManager.Instance.grainSpawner.SkipSpawning();

        // Ocultar el botón una vez usado
        if (skipButton != null)
            skipButton.gameObject.SetActive(false);
    }

    // ──────────────────────────────────────────────
    // Fases de UI
    // ──────────────────────────────────────────────

    public void ShowBlowingUI()
    {
        blowingPanel.SetActive(true);
        counting = true;
        elapsedTime = 0f;

        // El botón ya no tiene sentido en la fase de soplido
        if (skipButton != null)
            skipButton.gameObject.SetActive(false);
    }

    public void ShowFinishedUI()
    {
        counting = false;
        finishedPanel.SetActive(true);
    }

    public void UpdateProgressBar(float ratio)
    {
        progressBar.value = ratio;
    }

    public float GetElapsedTime()
    {
        return elapsedTime;
    }

    public void ResetUI()
    {
        progressBar.value = 0f;
        elapsedTime = 0f;
        counting = false;

        blowingPanel.SetActive(false);
        finishedPanel.SetActive(false);

        if (timerText != null)
            timerText.text = "00:00";

        // Mostrar el botón saltar al reiniciar (empieza la fase Falling de nuevo)
        if (skipButton != null)
            skipButton.gameObject.SetActive(true);
    }
}