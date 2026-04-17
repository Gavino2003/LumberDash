using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// Singleton que gere o estado global do jogo: velocidade, distância, moedas, Game Over e reset.
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
    public TextMeshProUGUI highScoreText;

    [Header("Moedas")]
    public TextMeshProUGUI coinText;

    [Header("Loja")]
    public Button distanceBoostButton;
    public GameObject boostActiveIndicator;
    private const int boostCost = 150;

    [Header("Tutorial")]
    public GameObject[] tutorialImages;
    public float pulseMin = 0.9f;
    public float pulseMax = 1.1f;
    public float pulseSpeed = 2f;

    public float CurrentSpeed { get; private set; }
    public bool IsGameOver { get; private set; }
    public bool IsWaiting { get; private set; } = true;
    private bool isPlayingIntro = false;
    private float distance = 0f;
    private int coins = 0;
    private bool distanceBoostActive = false;
    private int highScore = 0;
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

        coins = PlayerPrefs.GetInt("Coins", 0);
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        UpdateCoinUI();
        UpdateHighScoreUI();
        UpdateBoostButton();
        SetWaitingUIVisible(true);
        if (boostActiveIndicator != null) boostActiveIndicator.SetActive(false);
    }

    void Update()
    {
        if (IsWaiting && !isPlayingIntro)
        {
            PulseTutorial();

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                isPlayingIntro = true;
                StartCoroutine(StartGame());
            }
            return;
        }

        if (IsWaiting || IsGameOver) return;

        CurrentSpeed = Mathf.Min(CurrentSpeed + speedIncreaseRate * Time.deltaTime, maxSpeed);
        distance += CurrentSpeed * Time.deltaTime * (distanceBoostActive ? 2f : 1f);
        distanceText.text = Mathf.FloorToInt(distance) + "m";
    }

    // Animação de pulse suave nas imagens de tutorial enquanto o jogo aguarda input
    void PulseTutorial()
    {
        float scale = Mathf.Lerp(pulseMin, pulseMax, (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f);
        foreach (GameObject img in tutorialImages)
            if (img != null) img.transform.localScale = Vector3.one * scale;
    }

    IEnumerator StartGame()
    {
        foreach (GameObject img in tutorialImages)
            if (img != null) img.SetActive(false);

        yield return StartCoroutine(cameraManager.PlayIntro());
        yield return StartCoroutine(cameraManager.TransitionToGame());
        AudioManager.Instance.FadeUp();
        AudioManager.Instance.StartFootsteps();
        IsWaiting = false;
        isPlayingIntro = false;
        SetWaitingUIVisible(false);
        distancePanel.gameObject.SetActive(true);
        if (boostActiveIndicator != null) boostActiveIndicator.SetActive(distanceBoostActive);
        player.StartRunning();
    }

    public void GameOver()
    {
        if (IsGameOver) return;
        AudioManager.Instance.StopFootsteps();
        AudioManager.Instance.PlayGameOver();
        IsGameOver = true;
        distanceBoostActive = false;
        if (boostActiveIndicator != null) boostActiveIndicator.SetActive(false);

        int distInt = Mathf.FloorToInt(distance);
        if (distInt > highScore)
        {
            highScore = distInt;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
            UpdateHighScoreUI();
        }

        player.StopPlayer();
        cameraManager.ShakeCamera();
        cameraManager.PlayLumberjackHidden();
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
        UpdateBoostButton();
        SetWaitingUIVisible(true);
        cameraManager.ResetToMenu();
        foreach (GameObject obstacle in GameObject.FindGameObjectsWithTag("Obstacle"))
            Destroy(obstacle);
        foreach (GameObject coin in GameObject.FindGameObjectsWithTag("Coin"))
            Destroy(coin);
        FindFirstObjectByType<InfiniteGround>().ResetGround();
        player.ResetPlayer();
        player.StartWaiting();

        foreach (GameObject img in tutorialImages)
            if (img != null) img.SetActive(true);
    }

    public void AddCoin()
    {
        coins++;
        PlayerPrefs.SetInt("Coins", coins);
        PlayerPrefs.Save();
        UpdateCoinUI();
        UpdateBoostButton();
    }

    public void BuyDistanceBoost()
    {
        if (coins < boostCost || distanceBoostActive) return;
        coins -= boostCost;
        PlayerPrefs.SetInt("Coins", coins);
        PlayerPrefs.Save();
        distanceBoostActive = true;
        UpdateCoinUI();
        UpdateBoostButton();
        if (boostActiveIndicator != null) boostActiveIndicator.SetActive(true);
    }

    void UpdateCoinUI()
    {
        if (coinText != null) coinText.text = coins.ToString();
    }

    void UpdateHighScoreUI()
    {
        if (highScoreText != null) highScoreText.text = "Best: " + highScore + "m";
    }

    void UpdateBoostButton()
    {
        if (distanceBoostButton != null)
            distanceBoostButton.interactable = coins >= boostCost && !distanceBoostActive;
    }

    void SetWaitingUIVisible(bool visible)
    {
        if (highScoreText != null) highScoreText.gameObject.SetActive(visible);
        if (distanceBoostButton != null) distanceBoostButton.gameObject.SetActive(visible);
    }

    // Reduz temporariamente a velocidade após colisão lateral
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