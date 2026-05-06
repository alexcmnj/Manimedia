using UnityEngine;

public class Windcontroller : MonoBehaviour
{
    public Transform containerCenter;
    public Vector3 containerSize = new Vector3(4f, 2f, 4f);

    public float windForceMultiplier = 1200f;
    public float micSensitivity = 100f;
    public float noiseThreshold = 0.01f;

    [Header("Zona objetivo")]
    [Range(0.1f, 0.9f)]
    [Tooltip("Fracción del contenedor (desde la pared downwind) que cuenta como zona objetivo.")]
    public float targetZoneFraction = 0.30f;

    private Vector3 windDirection;
    private bool active = true;
    private bool hasDirection = false;

    private AudioClip micClip;
    private string micDevice;
    private Vector3 windOriginPoint;

    void Start()
    {
        if (Microphone.devices.Length > 0)
        {
            micDevice = Microphone.devices[0];
            micClip = Microphone.Start(micDevice, true, 1, 44100);
            Debug.Log("✅ Micrófono detectado: " + micDevice);
        }
        else
        {
            Debug.LogWarning("⚠️ No se detectó ningún micrófono.");
        }
    }

    void Update()
    {
        if (!active || !hasDirection) return;

        float micLevel = GetMicLevel();
        if (micLevel > noiseThreshold)
        {
            ApplyWind(micLevel * micSensitivity * windForceMultiplier);
        }
    }

    void ApplyWind(float force)
    {
        foreach (Rigidbody rb in Spawnpoint.allGrains)
        {
            if (rb == null) continue;
            // Solo aplicar viento a granos dentro del recipiente
            if (!IsInside(rb.position)) continue;
            rb.AddForce(windDirection * force, ForceMode.Force);
        }
    }

    public void SetWindOrigin(Vector3 origin, Vector3 direction)
    {
        windOriginPoint = origin;
        windDirection = direction.normalized;
        hasDirection = true;
        active = true;
        Debug.Log("✅ Viento activado. Dirección: " + windDirection);
    }

    /// <summary>
    /// ✅ Chequea los TRES ejes. Antes solo X y Z, así que granos
    /// que salían volando hacia arriba seguían contando como "dentro".
    /// </summary>
    public bool IsInside(Vector3 pos)
    {
        Vector3 local = pos - containerCenter.position;
        return Mathf.Abs(local.x) < containerSize.x * 0.5f &&
               Mathf.Abs(local.y) < containerSize.y * 0.5f &&
               Mathf.Abs(local.z) < containerSize.z * 0.5f;
    }

    /// <summary>
    /// Un grano cuenta como éxito si está FUERA del contenedor.
    /// No importa por qué lado salió — ya no está en el recipiente.
    /// </summary>
    public bool IsInTargetZone(Vector3 pos)
    {
        // Si está dentro del contenedor, NO cuenta
        return !IsInside(pos);
    }

    public float GetMicLevel()
    {
        if (micClip == null) return 0f;

        float[] samples = new float[128];
        int pos = Microphone.GetPosition(micDevice) - 128;
        if (pos < 0) return 0f;

        micClip.GetData(samples, pos);

        float max = 0f;
        foreach (float s in samples)
        {
            float val = Mathf.Abs(s);
            if (val > max) max = val;
        }
        return max;
    }

    public void SetActive(bool value) => active = value;

    public void ResetWind()
    {
        hasDirection = false;
        active = false;
    }
}