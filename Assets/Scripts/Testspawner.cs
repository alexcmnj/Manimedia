using UnityEngine;

public class Testspawner : MonoBehaviour
{
    public GameObject grainPrefab;
    public int maxGrains = 100;
    public float spawnRate = 0.1f;

    private int count = 0;
    private float timer = 0f;

    void Update()
    {
        if (count >= maxGrains) return;

        timer += Time.deltaTime;
        if (timer >= spawnRate)
        {
            Vector3 pos = new Vector3(
                Random.Range(-0.5f, 0.5f),
                5f,
                Random.Range(-0.5f, 0.5f)
            );
            Instantiate(grainPrefab, pos, Random.rotation);
            count++;
            timer = 0f;
        }
    }
}