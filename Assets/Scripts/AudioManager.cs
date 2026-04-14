using UnityEngine;
using System.Collections;

// Singleton que gere todos os sons do jogo: música (com fade), SFX e footsteps em loop.
// Usa 3 AudioSources separados: musicSource, sfxSource e footstepsSource.
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Volumes")]
    public float normalMusicVolume = 1f;
    public float lowMusicVolume = 0.2f;
    public float sfxVolume = 1f;
    public float fadeSpeed = 2f;

    [Header("Sons")]
    public AudioClip jumpSound;
    public AudioClip rollSound;
    public AudioClip lateralImpactSound;
    public AudioClip gameOverSound;
    public AudioClip coinSound;

    [Header("Volumes SFX")]
    public float jumpVolume = 1f;
    public float rollVolume = 1f;
    public float lateralImpactVolume = 1f;
    public float gameOverVolume = 1f;
    public float coinVolume = 1f;

    [Header("Sons Intro")]
    public AudioClip surpriseSound;
    public AudioClip screamSound;
    public float surpriseVolume = 1f;
    public float screamVolume = 1f;

    [Header("Footsteps")]
    public AudioClip footstepsSound;
    public float footstepsVolume = 1f;

    private AudioSource musicSource;
    private AudioSource sfxSource;
    private AudioSource footstepsSource;

    void Awake()
    {
        Instance = this;
        AudioSource[] sources = GetComponents<AudioSource>();
        musicSource = sources[0];
        sfxSource = sources[1];
        footstepsSource = sources[2];
        sfxSource.volume = sfxVolume;
        footstepsSource.volume = footstepsVolume;
    }

    public void StartFootsteps()
    {
        footstepsSource.clip = footstepsSound;
        footstepsSource.loop = true;
        footstepsSource.volume = footstepsVolume;
        footstepsSource.Play();
    }

    public void StopFootsteps()
    {
        footstepsSource.Stop();
    }

    public void FadeDown()
    {
        StartCoroutine(FadeTo(lowMusicVolume));
    }

    public void FadeUp()
    {
        StartCoroutine(FadeTo(normalMusicVolume));
    }

    IEnumerator FadeTo(float targetVolume)
    {
        while (Mathf.Abs(musicSource.volume - targetVolume) > 0.01f)
        {
            musicSource.volume = Mathf.MoveTowards(musicSource.volume, targetVolume, fadeSpeed * Time.deltaTime);
            yield return null;
        }
        musicSource.volume = targetVolume;
    }

    public void PlayJump()
    {
        sfxSource.PlayOneShot(jumpSound, jumpVolume);
    }

    public void PlayRoll()
    {
        sfxSource.PlayOneShot(rollSound, rollVolume);
    }

    public void PlayLateralImpact()
    {
        sfxSource.PlayOneShot(lateralImpactSound, lateralImpactVolume);
    }

    public void PlayGameOver()
    {
        sfxSource.PlayOneShot(gameOverSound, gameOverVolume);
    }

    public void PlaySurprise()
    {
        sfxSource.PlayOneShot(surpriseSound, surpriseVolume);
    }

    public void PlayScream()
    {
        sfxSource.PlayOneShot(screamSound, screamVolume);
    }

    public void PlayCoin()
    {
        sfxSource.PlayOneShot(coinSound, coinVolume);
    }
}