using UnityEngine;

public class Test : MonoBehaviour
{
    [Header("Referencias")]
    public Transform blowOrigin; // desde dónde sale el viento (puede ser la cámara)
    public Vector3 blowDirection = Vector3.forward;
    public float blowForce = 50f;

    void Update()
    {
        // Presiona la barra espaciadora para soplar
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ApplyBlow();
        }
    }

    void ApplyBlow()
    {
        if (Spawnpoint.allGrains.Count == 0)
        {
            Debug.LogWarning("⚠ No hay granos para soplar!");
            return;
        }

        foreach (Rigidbody rb in Spawnpoint.allGrains)
        {
            if (rb == null) continue;

            // fuerza simple hacia la dirección del viento
            rb.AddForce(blowDirection.normalized * blowForce, ForceMode.Impulse);
        }

        Debug.Log("💨 Viento aplicado a " + Spawnpoint.allGrains.Count + " granos.");
    }
}