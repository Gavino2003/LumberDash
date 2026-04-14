using UnityEngine;

// Move o obstáculo para trás à velocidade atual do jogo e destrói-o quando sai do ecrã.
public class ObstacleMovement : MonoBehaviour
{
    void Update()
    {
        if (GameManager.Instance.IsGameOver || GameManager.Instance.IsWaiting) return;

        transform.Translate(Vector3.back * GameManager.Instance.CurrentSpeed * Time.deltaTime, Space.World);

        if (transform.position.z < -15f)
            Destroy(gameObject);
    }
}