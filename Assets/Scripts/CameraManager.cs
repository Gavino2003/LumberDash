using UnityEngine;
using System.Collections;

// Gere a câmera: posição de menu, sequência de intro cinemática, transição para gameplay e follow do jogador.
public class CameraManager : MonoBehaviour
{
    [Header("Intro")]
    public Animator lumberjackAnimator;
    public Animator grootAnimator;
    public string lumberjackIntroAnim = "LumberjackJump";
    public string grootScaredAnim = "GrootScared";
    public string lumberjackHiddenAnim = "LumberjackHidden";
    public float grootAnimDelay = 1.5f;
    public float introDuration = 3f;
    public Transform lumberjackParent;
    public Vector3 lumberjackResetPosition;

    [Header("Camera Shake")]
    public float shakeDuration = 0.3f;
    public float shakeMagnitude = 0.1f;
    public float shakeDelay = 0.3f;

    [Header("UI Waiting")]
    public GameObject flowers;
    public GameObject trees;

    [Header("Targets")]
    public Transform menuCameraTransform;
    public Transform gameCameraTransform;
    public Transform player;

    [Header("Transição")]
    public float transitionDuration = 1.5f;
    public AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Audio Intro")]
    public float surpriseSoundDelay = 0.5f;
    public float screamSoundDelay = 1.5f;

    [Header("Follow")]
    public float smoothSpeed = 20f;

    private Vector3 offset;
    private bool isFollowing = false;
    private bool shakePlayed = false;
    private bool surprisePlayed = false;
    private bool screamPlayed = false;
    private bool grootPlayed = false;

    void Start()
    {
        transform.position = menuCameraTransform.position;
        transform.rotation = menuCameraTransform.rotation;
    }

    void FixedUpdate()
    {
        if (!isFollowing) return;

        Vector3 targetPosition = new Vector3(
            player.position.x + offset.x,
            transform.position.y,
            transform.position.z
        );

        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.fixedDeltaTime);
    }

    public void PlayLumberjackHidden()
    {
        lumberjackAnimator.Play(lumberjackHiddenAnim);
    }

    public void ShakeCamera()
    {
        StartCoroutine(Shake());
    }

    public IEnumerator PlayIntro()
    {
        AudioManager.Instance.FadeDown();
        lumberjackAnimator.Play(lumberjackIntroAnim);

        float elapsed = 0f;

        while (elapsed < introDuration)
        {
            elapsed += Time.deltaTime;

            if (elapsed >= shakeDelay && !shakePlayed)
            {
                shakePlayed = true;
                ShakeCamera();
            }

            if (elapsed >= surpriseSoundDelay && !surprisePlayed)
            {
                surprisePlayed = true;
                AudioManager.Instance.PlaySurprise();
            }

            if (elapsed >= screamSoundDelay && !screamPlayed)
            {
                screamPlayed = true;
                AudioManager.Instance.PlayScream();
            }

            if (elapsed >= grootAnimDelay && !grootPlayed)
            {
                grootPlayed = true;
                grootAnimator.Play(grootScaredAnim);
            }

            yield return null;
        }

        shakePlayed = false;
        surprisePlayed = false;
        screamPlayed = false;
        grootPlayed = false;
    }

    public IEnumerator TransitionToGame()
    {
        Vector3 startPos = menuCameraTransform.position;
        Quaternion startRot = menuCameraTransform.rotation;
        Vector3 endPos = gameCameraTransform.position;
        Quaternion endRot = gameCameraTransform.rotation;
        float timer = 0f;
        trees.SetActive(false);

        while (timer < transitionDuration)
        {
            timer += Time.deltaTime;
            float t = transitionCurve.Evaluate(timer / transitionDuration);
            transform.position = Vector3.Lerp(startPos, endPos, t);
            transform.rotation = Quaternion.Lerp(startRot, endRot, t);
            yield return null;
        }

        flowers.SetActive(false);
        transform.position = endPos;
        transform.rotation = endRot;
        offset = transform.position - player.position;
        isFollowing = true;
    }

    public void ResetToMenu()
    {
        trees.SetActive(true);
        flowers.SetActive(true);
        isFollowing = false;
        transform.position = menuCameraTransform.position;
        transform.rotation = menuCameraTransform.rotation;
        lumberjackParent.position = lumberjackResetPosition;
    }

    IEnumerator Shake()
    {
        Vector3 originalPos = transform.position;
        float timer = 0f;

        while (timer < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;

            transform.position = new Vector3(
                originalPos.x + x,
                originalPos.y + y,
                originalPos.z
            );

            timer += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPos;
    }
}