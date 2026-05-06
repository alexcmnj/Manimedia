using UnityEngine;
using System.Collections.Generic;

public class Spawnpoint : MonoBehaviour
{
    public static List<Rigidbody> allGrains = new List<Rigidbody>();

    //public GameObject grainPrefab;
    public GameObject Mani;
    public GameObject Mani2;

    public float spawnRate = 0.05f;
    public int maxGrains = 400;

    public float spawnRadius = 0.3f;
    public float spawnHeight = 5f;

    private float timer = 0f;
    private int spawnedThisCycle = 0;
    private bool spawning = true;

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

            Invoke(nameof(NotifyFinished), 2f);
        }
    }

    void NotifyFinished()
    {
        GameManager.Instance.OnSpawningFinished();
    }

    public void StartSpawning()
    {
        spawning = true;
        spawnedThisCycle = 0;
    }

    public void ResetAll()
    {
        foreach (Transform t in transform)
            Destroy(t.gameObject);

        allGrains.Clear();
    }
}