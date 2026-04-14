using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    public float lateralThreshold = 0.5f;
    public float lateralImpactWindow = 3f;

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
    Debug.Log("Impacto FRONTAL - Game Over");
    playerMovement.PlayFrontHitAnimation();
    GameManager.Instance.GameOver();
}
            else
{
    GameManager.Instance.ApplyHitSlow();
    RegisterLateralImpact(diff);
}
        }
    }

  void RegisterLateralImpact(float diff)
{
    AudioManager.Instance.PlayLateralImpact();
    lateralImpactCount++;

    if (lateralImpactCount >= 2)
    {
        // Primeiro empurra para a lane, depois morre
        if (diff < 0)
            playerMovement.PushToLane(1);
        else
            playerMovement.PushToLane(-1);

        playerMovement.PlayDeathAnimation();
        GameManager.Instance.GameOver();
        lateralImpactCount = 0;
        lateralImpactTimer = 0f;
    }
    else
    {
        if (diff < 0)
            playerMovement.PushToLane(1);
        else
            playerMovement.PushToLane(-1);
    }
    
}

}