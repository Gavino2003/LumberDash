using UnityEngine;

// Faz a moeda rodar no eixo Y e deteta colisão com o jogador para a recolher.
public class CoinBehaviour : MonoBehaviour
{
    public float rotationSpeed = 180f;

    void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AudioManager.Instance.PlayCoin();
            GameManager.Instance.AddCoin();
            Destroy(gameObject);
        }
    }
}