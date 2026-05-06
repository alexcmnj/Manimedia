using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawnpoint : MonoBehaviour
{
    public static List<Rigidbody> allGrains = new List<Rigidbody>();

    public GameObject Mani;
    public GameObject Mani2;

    public float spawnRate = 0.05f;
    public int maxGrains = 400;

    public float spawnRadius = 0.3f;
    public float spawnHeight = 5f;

    [Header("Skip")]
    public Transform container; // Asignar: el GameObject "Container" en el Inspector

    private float timer = 0f;
    private int spawnedThisCycle = 0;
    private bool spawning = true;
    private bool skipping = false;

    private Bounds containerBounds;

    void Update()
    {
        if (!spawning) return;

        timer += Time.deltaTime;

        if (timer >= spawnRate && spawnedThisCycle < maxGrains)
        {
            SpawnGrain();
            timer = 0f;
        }
    }

    void SpawnGrain()
    {
        Vector3 pos = new Vector3(
            transform.position.x + Random.Range(-spawnRadius, spawnRadius),
            transform.position.y + spawnHeight,
            transform.position.z + Random.Range(-spawnRadius, spawnRadius)
        );

        GameObject prefabToUse = (Random.value > 0.5f) ? Mani : Mani2;
        GameObject grain = Instantiate(prefabToUse, pos, Random.rotation, transform);

        Rigidbody rb = grain.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            allGrains.Add(rb);
        }

        spawnedThisCycle++;

        if (spawnedThisCycle >= maxGrains)
        {
            spawning = false;
            if (!skipping)
                Invoke(nameof(NotifyFinished), 2f);
        }
    }

    public void SkipSpawning()
    {
        if (!spawning || skipping) return;

        if (container == null)
        {
            Debug.LogError("SkipSpawning: asigna el Transform 'Container' en el Inspector.");
            return;
        }

        containerBounds = GetContainerBounds();
        skipping = true;
        StartCoroutine(TeleportAndSettle());
    }

    /// <summary>
    /// Suma los bounds de todos los Colliders hijos del Container
    /// pero EXCLUYE los que están en la parte superior (tapa abierta)
    /// usando solo los colliders cuyo centro esté debajo del centro del conjunto.
    /// </summary>
    Bounds GetContainerBounds()
    {
        Collider[] cols = container.GetComponentsInChildren<Collider>();
        if (cols.Length == 0)
        {
            Debug.LogWarning("No se encontraron Colliders en Container.");
            return new Bounds(container.position, Vector3.one);
        }

        // Primero calcular el centro aproximado de todos
        Bounds full = cols[0].bounds;
        for (int i = 1; i < cols.Length; i++)
            full.Encapsulate(cols[i].bounds);

        // Usar el interior: reducir un 20% en X y Z para quedar dentro de las paredes
        // y usar solo la mitad inferior en Y
        Vector3 interiorSize = new Vector3(
            full.size.x * 0.7f,
            full.size.y * 0.5f,
            full.size.z * 0.7f
        );
        Vector3 interiorCenter = new Vector3(
            full.center.x,
            full.min.y + interiorSize.y * 0.5f + 0.05f,
            full.center.z
        );

        return new Bounds(interiorCenter, interiorSize);
    }

    IEnumerator TeleportAndSettle()
    {
        // 1. Instanciar granos faltantes dentro del recipiente
        int remaining = maxGrains - spawnedThisCycle;
        for (int i = 0; i < remaining; i++)
        {
            Vector3 pos = RandomInContainer();
            GameObject prefabToUse = (Random.value > 0.5f) ? Mani : Mani2;
            GameObject grain = Instantiate(prefabToUse, pos, Random.rotation, transform);
            Rigidbody rb = grain.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
                allGrains.Add(rb);
            }
        }

        spawnedThisCycle = maxGrains;
        spawning = false;

        yield return new WaitForFixedUpdate();

        // 2. Poner kinematic y teleportar
        foreach (Rigidbody rb in allGrains)
        {
            if (rb == null) continue;
            rb.isKinematic = true;
            rb.position = RandomInContainer();
        }

        yield return new WaitForFixedUpdate();

        
        foreach (Rigidbody rb in allGrains)
        {
            if (rb == null) continue;
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        yield return new WaitForSeconds(1.5f);

        skipping = false;
        NotifyFinished();
    }

    Vector3 RandomInContainer()
    {
        return new Vector3(
            Random.Range(containerBounds.min.x, containerBounds.max.x),
            Random.Range(containerBounds.min.y, containerBounds.max.y),
            Random.Range(containerBounds.min.z, containerBounds.max.z)
        );
    }

    void NotifyFinished()
    {
        GameManager.Instance.OnSpawningFinished();
    }

    public void StartSpawning()
    {
        spawning = true;
        skipping = false;
        spawnedThisCycle = 0;
        timer = 0f;
    }

    public void ResetAll()
    {
        StopAllCoroutines();
        CancelInvoke();

        foreach (Transform t in transform)
            Destroy(t.gameObject);

        allGrains.Clear();
        spawning = false;
        skipping = false;
        spawnedThisCycle = 0;
        timer = 0f;
    }
}