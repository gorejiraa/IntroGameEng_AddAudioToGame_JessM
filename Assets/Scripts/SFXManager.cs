using System.Collections;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    [Header("Sources")]
    public AudioSource SFXaudioSource;         // one-shot SFX source
    public AudioSource BgMusicAudioSource;     // separate source for background music (child "BgMusic")

    [Header("SFX Clips")]
    public AudioClip playerShoot;
    public AudioClip asteroidExplosion;
    public AudioClip playerDamage;
    public AudioClip playerExplosion;
    public AudioClip milestoneExplosion; // optional special milestone SFX

    [Header("BG Music Clips")]
    public AudioClip BgMusicTitleScreen;
    public AudioClip BgMusicGameplay;

    [Header("Advanced Settings")]
    [Tooltip("Random pitch variation for frequently played SFX (± percentage, e.g. 0.05 = ±5%)")]
    public float pitchVariationPercent = 0.05f; // ±5% default
    [Tooltip("How much to increase music speed (pitch) per wave, e.g. 0.02 = +2% per wave")]
    public float tempoIncreasePerWave = 0.02f;
    public float maxMusicPitch = 1.5f; // clamp top

    [Tooltip("Play milestone every N asteroids destroyed")]
    public int milestoneEvery = 10;

    // internal state
    private int asteroidKillCount = 0;
    private Coroutine musicFadeCoroutine;

    void Awake()
    {
        // If not assigned in inspector, try to find them
        if (SFXaudioSource == null) SFXaudioSource = GetComponent<AudioSource>();
        if (BgMusicAudioSource == null)
        {
            Transform bg = transform.Find("BgMusic");
            if (bg != null) BgMusicAudioSource = bg.GetComponent<AudioSource>();
        }

        // safety checks
        if (SFXaudioSource == null) Debug.LogWarning("SFXManager: SFXaudioSource not assigned/found.");
        if (BgMusicAudioSource == null) Debug.LogWarning("SFXManager: BgMusicAudioSource not assigned/found.");
    }

    // general purpose play with optional pitch variation
    public void PlayOneShot(AudioClip clip, float volume = 1f, bool applyPitchVariation = false)
    {
        if (clip == null || SFXaudioSource == null) return;

        float priorPitch = SFXaudioSource.pitch;
        if (applyPitchVariation)
        {
            float variation = Random.Range(-pitchVariationPercent, pitchVariationPercent);
            SFXaudioSource.pitch = 1f + variation;
        }
        SFXaudioSource.PlayOneShot(clip, volume);
        if (applyPitchVariation) SFXaudioSource.pitch = priorPitch;
    }

    // called in PlayerController script
    public void PlayerShoot()
    {
        // apply pitch variation to minimize ear fatigue
        PlayOneShot(playerShoot, 1f, true);
    }

    // called in PlayerController script
    public void PlayerDamage()
    {
        PlayOneShot(playerDamage);
    }

    // called in PlayerController script (player death)
    public void PlayerExplosion()
    {
        PlayOneShot(playerExplosion);
    }

    // called in the AsteroidDestroy script
    public void AsteroidExplosion()
    {
        asteroidKillCount++;
        // play normal explosion
        PlayOneShot(asteroidExplosion);

        // milestone check
        if (milestoneExplosion != null && milestoneEvery > 0 && (asteroidKillCount % milestoneEvery == 0))
        {
            PlayOneShot(milestoneExplosion);
        }
    }

    // reset kill counter (optional call when level restarts)
    public void ResetAsteroidCount()
    {
        asteroidKillCount = 0;
    }

    // Background music controls
    public void BGMusicMainMenu(float fadeTime = 0.5f)
    {
        StartBgMusicClip(BgMusicTitleScreen, fadeTime);
    }

    public void BGMusicGameplay(float fadeTime = 0.5f)
    {
        StartBgMusicClip(BgMusicGameplay, fadeTime);
    }

    private void StartBgMusicClip(AudioClip clip, float fadeTime)
    {
        if (BgMusicAudioSource == null || clip == null) return;

        if (musicFadeCoroutine != null) StopCoroutine(musicFadeCoroutine);
        musicFadeCoroutine = StartCoroutine(CrossfadeMusic(clip, fadeTime));
    }

    private IEnumerator CrossfadeMusic(AudioClip newClip, float fadeTime)
    {
        // fade out
        float startVolume = BgMusicAudioSource.volume;
        for (float t = 0f; t < fadeTime; t += Time.deltaTime)
        {
            BgMusicAudioSource.volume = Mathf.Lerp(startVolume, 0f, t / fadeTime);
            yield return null;
        }
        BgMusicAudioSource.volume = 0f;
        BgMusicAudioSource.clip = newClip;
        BgMusicAudioSource.loop = true;
        BgMusicAudioSource.Play();

        // fade in
        for (float t = 0f; t < fadeTime; t += Time.deltaTime)
        {
            BgMusicAudioSource.volume = Mathf.Lerp(0f, startVolume, t / fadeTime);
            yield return null;
        }
        BgMusicAudioSource.volume = startVolume;

        musicFadeCoroutine = null;
    }

    // Adjust music tempo (pitch) based on current wave index
    // Call this from your WaveManager when a new wave spawns.
    public void SetMusicPitchForWave(int waveIndex)
    {
        if (BgMusicAudioSource == null) return;
        float targetPitch = 1f + (waveIndex * tempoIncreasePerWave);
        targetPitch = Mathf.Clamp(targetPitch, 0.5f, maxMusicPitch);
        BgMusicAudioSource.pitch = targetPitch;
    }
}
