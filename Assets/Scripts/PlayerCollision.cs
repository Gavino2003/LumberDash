using UnityEngine;

// Deteta colisões com obstáculos e distingue impacto frontal (Game Over imediato)
// de impacto lateral (empurra o jogador; ao 2.º impacto dentro da janela de tempo é Game Over).
public class PlayerCollision : MonoBehaviour
{
    public float lateralThreshold = 0.5f;   // diferença mínima em X para considerar lateral
    public float lateralImpactWindow = 3f;  // janela de tempo para contar 2 impactos laterais

    private PlayerMovement playerMovement;
    private int lateralImpactCount = 0;
    private float lateralImpactTimer = 0f;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (lateralImpactCount > 0)
        {
            lateralImpactTimer += Time.deltaTime;
            if (lateralImpactTimer >= lateralImpactWindow)
            {
                lateralImpactCount = 0;
                lateralImpactTimer = 0f;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            float diff = other.transform.position.x - transform.position.x;

            if (Mathf.Abs(diff) < lateralThreshold)
            {
                // Impacto frontal direto — Game Over imediato
                playerMovement.PlayFrontHitAnimation();
                GameManager.Instance.GameOver();
            }
            else
            {
                // Impacto lateral — abranda e regista; ao 2.º impacto é Game Over
                GameManager.Instance.ApplyHitSlow();
                RegisterLateralImpact(diff);
            }
        }
    }

    void RegisterLateralImpact(float diff)
    {
        AudioManager.Instance.PlayLateralImpact();
        lateralImpactCount++;

        if (diff < 0)
            playerMovement.PushToLane(1);
        else
            playerMovement.PushToLane(-1);

        if (lateralImpactCount >= 2)
        {
            playerMovement.PlayDeathAnimation();
            GameManager.Instance.GameOver();
            lateralImpactCount = 0;
            lateralImpactTimer = 0f;
        }
    }

}