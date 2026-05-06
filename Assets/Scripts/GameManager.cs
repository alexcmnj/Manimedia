using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public enum GamePhase
    {
        Falling,
        Selection,
        Blowing,
        Finished
    }

    public GamePhase currentPhase = GamePhase.Falling;

    [Header("Referencias")]
    public Spawnpoint grainSpawner;
    public Windcontroller windController;
    public WallSelector wallSelector;
    public UIManager uiManager;

    [Header("Objetivo")]
    public float targetPercentage = 0.85f;

    [Header("Inactividad")]
    public float inactivityLimit = 5f;
    private float inactivityTimer = 0f;

    [Header("Logo final")]
    public Logoreveal logoRevealer;

    [Header("Cámara")]
    public Cameraorbit cameraOrbit;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (currentPhase == GamePhase.Blowing)
        {
            CalculateProgress();
            CheckInactivity();
        }
    }

    public void SetPhase(GamePhase phase)
    {
        currentPhase = phase;

        if (phase == GamePhase.Selection)
        {
            wallSelector.Show();
        }

        if (phase == GamePhase.Blowing)
        {
            uiManager.ShowBlowingUI();
            Debug.Log($"✅ Fase Blowing iniciada. Total granos: {Spawnpoint.allGrains.Count}");
        }
    }

    public void OnSpawningFinished()
    {
        SetPhase(GamePhase.Selection);
    }

    void CalculateProgress()
    {
        int grainsOutside = 0;
        int totalGrains = 0;

        foreach (Rigidbody rb in Spawnpoint.allGrains)
        {
            if (rb == null) continue;
            totalGrains++;

            // Cuenta como éxito SOLO si el grano salió del contenedor
            // en la dirección del viento
            if (windController.IsInTargetZone(rb.position))
                grainsOutside++;
        }

        if (totalGrains == 0) return;

        float ratio = (float)grainsOutside / totalGrains;

        uiManager.UpdateProgressBar(ratio);

        Debug.Log($"[Progress] Fuera del recipiente (dirección viento): {grainsOutside}/{totalGrains} = {ratio:P0}");

        if (ratio >= targetPercentage)
        {
            FinishGame();
        }
    }

    void FinishGame()
    {
        currentPhase = GamePhase.Finished;
        windController.SetActive(false);
        uiManager.ShowFinishedUI();

        if (logoRevealer != null)
            logoRevealer.Reveal();

        if (cameraOrbit != null)
            cameraOrbit.SetLocked(true);

        Debug.Log("🏁 Juego terminado.");
    }

    void CheckInactivity()
    {
        float micLevel = windController.GetMicLevel();

        if (micLevel > 0.01f)
        {
            inactivityTimer = 0f;
        }
        else
        {
            inactivityTimer += Time.deltaTime;

            if (inactivityTimer >= inactivityLimit)
            {
                grainSpawner.StartSpawning();
                inactivityTimer = 0f;
            }
        }
    }

    public void ResetSimulation()
    {
        grainSpawner.ResetAll();
        windController.ResetWind();
        uiManager.ResetUI();

        if (cameraOrbit != null)
            cameraOrbit.SetLocked(false);

        if (logoRevealer != null)
            logoRevealer.ResetLogo();

        currentPhase = GamePhase.Falling;
        grainSpawner.StartSpawning();
    }




}