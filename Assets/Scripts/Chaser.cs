using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Controla o Lumberjack: segue o jogador com um atraso, replica mudanças de lane e saltos.
public class Chaser : MonoBehaviour
{
    [Header("Configuração")]
    public Transform player;
    public float followDistance = 10f;       // distância normal atrás do jogador
    public float chaseDistance = 5f;         // distância quando o jogador tropeça
    public float normalSpeed = 1f;           // multiplicador de velocidade normal
    public float chaseSpeedMultiplier = 2f;  // multiplicador quando persegue
    public float laneDistance = 2.5f;
    public float moveLaneDelay = 0.5f;       // atraso a replicar o movimento do jogador

    public float chaserJumpForce = 7f;
    public float chaseDuration = 2f;
    public float jumpDelay = 0.3f;

    private int currentChaserLane = 1;
    private float targetX = 0f;
    private float currentFollowDistance;
    private bool isChasing = false;
    private bool pendingJump = false;
    private Rigidbody chaserRb;

    void Start()
    {
        currentFollowDistance = followDistance;
        chaserRb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (GameManager.Instance.IsWaiting || GameManager.Instance.IsGameOver) return;

        float targetZ = player.position.z - currentFollowDistance;

        // Velocidade base é a do jogo; quando persegue, fecha distância de forma proporcional
        float distanceToTarget = Mathf.Abs(transform.position.z - targetZ);
        float catchUpSpeed = isChasing ? distanceToTarget * chaseSpeedMultiplier : 0f;
        float totalSpeed = GameManager.Instance.CurrentSpeed + catchUpSpeed;

        Vector3 pos = transform.position;
        pos.z = Mathf.MoveTowards(pos.z, targetZ, totalSpeed * Time.deltaTime);
        pos.x = Mathf.MoveTowards(pos.x, targetX, totalSpeed * Time.deltaTime);
        transform.position = pos;
    }

    void FixedUpdate()
    {
        if (pendingJump)
        {
            chaserRb.AddForce(Vector3.up * chaserJumpForce, ForceMode.Impulse);
            pendingJump = false;
        }
    }

    // Recebe a nova posição X do jogador e replica-a com atraso
    public void OnPlayerChangeLane(float newTargetX)
    {
        StartCoroutine(ReplicateLane(newTargetX));
    }

    IEnumerator ReplicateLane(float newTargetX)
    {
        yield return new WaitForSeconds(moveLaneDelay);

        // Converte posição X para índice de lane (0, 1, 2)
        int newLane = Mathf.RoundToInt(newTargetX / laneDistance) + 1;

        if (newLane != currentChaserLane)
        {
            currentChaserLane = newLane;
            targetX = newTargetX;
        }
    }

    // Ativa o modo de perseguição por chaseDuration segundos
    public void StartChasing()
    {
        isChasing = true;
        currentFollowDistance = chaseDistance;
        StartCoroutine(StopChasing());
    }

    IEnumerator StopChasing()
    {
        yield return new WaitForSeconds(chaseDuration);
        isChasing = false;
        currentFollowDistance = followDistance;
    }

    // Replica o salto do jogador com um pequeno atraso
    public void OnPlayerJump()
    {
        StartCoroutine(ReplicateJump());
    }

    IEnumerator ReplicateJump()
    {
        yield return new WaitForSeconds(jumpDelay);
        pendingJump = true;
    }
}
