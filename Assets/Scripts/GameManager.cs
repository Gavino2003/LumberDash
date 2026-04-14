using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public float hitSpeedMultiplier = 0.5f;
    public float hitSlowDuration = 2f;
    private float speedBeforeHit = 0f;

    [Header("Cameras")]
    public CameraManager cameraManager;

    [Header("Velocidade")]
    public float initialSpeed = 8f;
    public float maxSpeed = 20f;
    public float speedIncreaseRate = 0.1f;

    [Header("UI")]
    public TextMeshProUGUI distanceText;
    public GameObject distancePanel;

    [Header("Moedas")]
    public TextMeshProUGUI coinText;

    public float CurrentSpeed { get; private set; }
    public bool IsGameOver { get; private set; }
    public bool IsWaiting { get; private set; } = true;


    private bool isPlayingIntro = false;
    private float distance = 0f;
    private int coins = 0;
    private PlayerMovement player;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        distancePanel.gameObject.SetActive(false);
        CurrentSpeed = initialSpeed;
        player = FindFirstObjectByType<PlayerMovement>();
    }

    void Update()
    {
        if (IsWaiting && !isPlayingIntro)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                isPlayingIntro = true;
                StartCoroutine(StartGame());
            }
            return;
        }

        if (IsWaiting || IsGameOver) return;

        CurrentSpeed = Mathf.Min(CurrentSpeed + speedIncreaseRate * Time.deltaTime, maxSpeed);
        distance += CurrentSpeed * Time.deltaTime;
        distanceText.text = Mathf.FloorToInt(distance) + "m";
    }

    IEnumerator StartGame()
    {
        yield return StartCoroutine(cameraManager.PlayIntro());
        yield return StartCoroutine(cameraManager.TransitionToGame());
        AudioManager.Instance.FadeUp();
        AudioManager.Instance.StartFootsteps();
        IsWaiting = false;
        isPlayingIntro = false;
        distancePanel.gameObject.SetActive(true);
        player.StartRunning();
    }

   public void GameOver()
{
    if (IsGameOver) return;
    AudioManager.Instance.StopFootsteps();
    AudioManager.Instance.PlayGameOver();
    IsGameOver = true;
    player.StopPlayer();
    cameraManager.ShakeCamera();
    cameraManager.PlayLumberjackHidden();
    Debug.Log("GAME OVER");
    StartCoroutine(ResetAfterDelay(3f));
}

    IEnumerator ResetAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ResetGame();
    }

    void ResetGame()
{
    IsWaiting = true;
    isPlayingIntro = false;
    IsGameOver = false;
    CurrentSpeed = initialSpeed;
    distance = 0f;
    distanceText.text = "0m";
    distancePanel.gameObject.SetActive(false);
    cameraManager.ResetToMenu();

    foreach (GameObject obstacle in GameObject.FindGameObjectsWithTag("Obstacle"))
        Destroy(obstacle);

    foreach (GameObject coin in GameObject.FindGameObjectsWithTag("Coin"))
        Destroy(coin);

    FindFirstObjectByType<InfiniteGround>().ResetGround();
    player.ResetPlayer();
    player.StartWaiting(); // só aqui depois de tudo resetado
}

    public void AddCoin()
    {
        coins++;
        coinText.text = coins.ToString();
    }

    public void ApplyHitSlow()
    {
        speedBeforeHit = CurrentSpeed;
        CurrentSpeed *= hitSpeedMultiplier;
        StartCoroutine(RestoreSpeed());
    }

    IEnumerator RestoreSpeed()
    {
        yield return new WaitForSeconds(hitSlowDuration);
        CurrentSpeed = speedBeforeHit;
    }
}