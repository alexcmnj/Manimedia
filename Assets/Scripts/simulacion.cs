using UnityEngine;


public class simulacion : MonoBehaviour
{
    public Spawnpoint spawner;

    void Update()
    {
        // Iniciar caída de granos
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (spawner != null)
                spawner.StartSpawning();
        }

        // Reiniciar simulación completa
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (GameManager.Instance != null)
                GameManager.Instance.ResetSimulation();
        }
    }
}