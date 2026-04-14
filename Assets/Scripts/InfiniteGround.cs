using UnityEngine;

public class InfiniteGround : MonoBehaviour
{
    public GameObject groundTilePrefab;
    public int numberOfTiles = 5;
    public float tileLength = 20f;

    private GameObject[] tiles;
    private float spawnZ = 0f;
    private float playerZ = 0f;

    void Start()
    {
        tiles = new GameObject[numberOfTiles];

        for (int i = 0; i < numberOfTiles; i++)
        {
            tiles[i] = Instantiate(groundTilePrefab, new Vector3(0f, 0f, spawnZ), Quaternion.identity);
            spawnZ += tileLength;
        }
    }

    void Update()
    {
        if (GameManager.Instance.IsGameOver || GameManager.Instance.IsWaiting) return;
        // Move todos os tiles
        foreach (GameObject tile in tiles)
        {
            tile.transform.Translate(Vector3.back * GameManager.Instance.CurrentSpeed * Time.deltaTime);

            // Se o tile passou atrás do player reposiciona à frente
            if (tile.transform.position.z < playerZ - tileLength)
            {
                float furthestZ = GetFurthestTileZ();
                tile.transform.position = new Vector3(0f, 0f, furthestZ + tileLength);
            }
        }
    }
public void ResetGround()
{
    float resetZ = 0f;
    foreach (GameObject tile in tiles)
    {
        tile.transform.position = new Vector3(0f, 0f, resetZ);
        resetZ += tileLength;
    }
}
    float GetFurthestTileZ()
    {
        float furthest = float.MinValue;
        foreach (GameObject tile in tiles)
        {
            if (tile.transform.position.z > furthest)
                furthest = tile.transform.position.z;
        }
        return furthest;
    }
}