using UnityEngine;

// Spawna decorações laterais (árvores, etc.) periodicamente à frente do jogador
// para criar a sensação de profundidade na floresta.
public class SideWalls : MonoBehaviour
{
    public Transform player;
    public GameObject[] leftDecorations;
    public GameObject[] rightDecorations;

    public float spawnInterval = 3f;
    public float spawnZ = 20f;
    public float leftX = -6f;
    public float rightX = 6f;
    public float destroyZ = -20f;
    public float randomXVariation = 1f;

    private float timer = 0f;

    void Update()
    {
        if (GameManager.Instance.IsGameOver || GameManager.Instance.IsWaiting) return;
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnDecoration();
        }
    }

    void SpawnDecoration()
    {
        if (leftDecorations.Length > 0)
        {
            GameObject prefab = leftDecorations[Random.Range(0, leftDecorations.Length)];
            float x = leftX + Random.Range(-randomXVariation, randomXVariation);
            Vector3 pos = new Vector3(x, 0f, player.position.z + spawnZ);
            Instantiate(prefab, pos, prefab.transform.rotation);
        }

        if (rightDecorations.Length > 0)
        {
            GameObject prefab = rightDecorations[Random.Range(0, rightDecorations.Length)];
            float x = rightX + Random.Range(-randomXVariation, randomXVariation);
            Vector3 pos = new Vector3(x, 0f, player.position.z + spawnZ);
            Instantiate(prefab, pos, prefab.transform.rotation);
        }
    }
}