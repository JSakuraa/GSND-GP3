using UnityEngine;

public class BackgroundMusicManager : MonoBehaviour
{
    [Header("Music Settings")]
    [SerializeField] private AudioClip[] backgroundMusicTracks;
    [SerializeField] private bool shufflePlaylist = false;
    [SerializeField] private float volume = 0.5f;
    [SerializeField] private bool fadeInOnStart = true;
    [SerializeField] private float fadeInDuration = 2f;
    [SerializeField] private float fadeDuration = 1f; // Duration for pause/resume fades
    [SerializeField] private float crossfadeDuration = 2f; // Duration when changing tracks

    private AudioSource audioSource;
    private bool isTurnPhase = false;
    private float targetVolume;
    private Coroutine fadeCoroutine;
    private int currentTrackIndex = 0;
    private int[] shuffledIndices;

    void Awake()
    {
        // Set up AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = false; // We'll handle looping manually to switch tracks
        audioSource.playOnAwake = false;
        audioSource.volume = fadeInOnStart ? 0f : volume;
        targetVolume = volume;

        // Set up shuffle if enabled
        if (shufflePlaylist)
        {
            InitializeShuffledPlaylist();
        }
    }

    void Start()
    {
        // Start playing music
        if (backgroundMusicTracks != null && backgroundMusicTracks.Length > 0)
        {
            PlayCurrentTrack();

            if (fadeInOnStart)
            {
                FadeToVolume(volume, fadeInDuration);
            }
        }
        else
        {
            Debug.LogWarning("BackgroundMusicManager: No background music tracks assigned!");
        }
    }

    void Update()
    {
        // Check if current track finished and play next
        if (audioSource.isPlaying == false && backgroundMusicTracks != null && backgroundMusicTracks.Length > 0)
        {
            PlayNextTrack();
        }
    }

    /// <summary>
    /// Initialize shuffled playlist
    /// </summary>
    private void InitializeShuffledPlaylist()
    {
        if (backgroundMusicTracks == null || backgroundMusicTracks.Length == 0) return;

        shuffledIndices = new int[backgroundMusicTracks.Length];
        for (int i = 0; i < shuffledIndices.Length; i++)
        {
            shuffledIndices[i] = i;
        }

        // Fisher-Yates shuffle
        for (int i = shuffledIndices.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            int temp = shuffledIndices[i];
            shuffledIndices[i] = shuffledIndices[j];
            shuffledIndices[j] = temp;
        }
    }

    /// <summary>
    /// Play the current track in the playlist
    /// </summary>
    private void PlayCurrentTrack()
    {
        if (backgroundMusicTracks == null || backgroundMusicTracks.Length == 0) return;

        int trackIndex = shufflePlaylist ? shuffledIndices[currentTrackIndex] : currentTrackIndex;
        audioSource.clip = backgroundMusicTracks[trackIndex];
        audioSource.Play();
    }

    /// <summary>
    /// Move to and play the next track
    /// </summary>
    private void PlayNextTrack()
    {
        if (backgroundMusicTracks == null || backgroundMusicTracks.Length == 0) return;

        currentTrackIndex++;

        // Loop back to start
        if (currentTrackIndex >= backgroundMusicTracks.Length)
        {
            currentTrackIndex = 0;

            // Re-shuffle if shuffle is enabled
            if (shufflePlaylist)
            {
                InitializeShuffledPlaylist();
            }
        }

        PlayCurrentTrack();

        // Maintain current volume state
        if (!isTurnPhase)
        {
            audioSource.volume = volume;
        }
        else
        {
            audioSource.volume = 0f;
        }
    }

    /// <summary>
    /// Call this when turn phase begins - music will fade out
    /// </summary>
    public void OnTurnPhaseStart()
    {
        if (isTurnPhase) return;

        isTurnPhase = true;
        FadeToVolume(0f, fadeDuration);
    }

    /// <summary>
    /// Call this when turn phase ends - music will fade back in
    /// </summary>
    public void OnTurnPhaseEnd()
    {
        if (!isTurnPhase) return;

        isTurnPhase = false;
        FadeToVolume(volume, fadeDuration);
    }

    /// <summary>
    /// Fade to a target volume over specified duration
    /// </summary>
    private void FadeToVolume(float target, float duration)
    {
        // Stop any existing fade
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(FadeCoroutine(target, duration));
    }

    private System.Collections.IEnumerator FadeCoroutine(float target, float duration)
    {
        float startVolume = audioSource.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            audioSource.volume = Mathf.Lerp(startVolume, target, t);
            yield return null;
        }

        audioSource.volume = target;
        fadeCoroutine = null;
    }

    /// <summary>
    /// Manually set the music volume
    /// </summary>
    public void SetVolume(float newVolume)
    {
        volume = Mathf.Clamp01(newVolume);
        targetVolume = volume;

        if (!isTurnPhase)
        {
            audioSource.volume = volume;
        }
    }

    /// <summary>
    /// Stop the music completely
    /// </summary>
    public void StopMusic()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        audioSource.Stop();
    }

    /// <summary>
    /// Change the background music track
    /// </summary>
    public void ChangeMusic(AudioClip newClip, bool fadeTransition = true)
    {
        if (newClip == null) return;

        if (fadeTransition)
        {
            StartCoroutine(CrossfadeMusic(newClip));
        }
        else
        {
            audioSource.clip = newClip;
            audioSource.Play();
        }
    }

    /// <summary>
    /// Skip to the next track in the playlist
    /// </summary>
    public void SkipToNextTrack(bool fadeTransition = true)
    {
        if (backgroundMusicTracks == null || backgroundMusicTracks.Length == 0) return;

        if (fadeTransition)
        {
            StartCoroutine(CrossfadeToNextTrack());
        }
        else
        {
            PlayNextTrack();
        }
    }

    /// <summary>
    /// Play a specific track by index
    /// </summary>
    public void PlayTrack(int trackIndex, bool fadeTransition = true)
    {
        if (backgroundMusicTracks == null || trackIndex < 0 || trackIndex >= backgroundMusicTracks.Length) return;

        currentTrackIndex = trackIndex;

        if (fadeTransition)
        {
            StartCoroutine(CrossfadeToCurrentTrack());
        }
        else
        {
            PlayCurrentTrack();
        }
    }

    private System.Collections.IEnumerator CrossfadeMusic(AudioClip newClip)
    {
        // Fade out current music
        yield return FadeCoroutine(0f, crossfadeDuration);

        // Switch clip
        audioSource.clip = newClip;
        audioSource.Play();

        // Fade in new music (only if not in turn phase)
        if (!isTurnPhase)
        {
            yield return FadeCoroutine(volume, crossfadeDuration);
        }
    }

    private System.Collections.IEnumerator CrossfadeToNextTrack()
    {
        // Fade out current music
        yield return FadeCoroutine(0f, crossfadeDuration);

        // Move to next track
        currentTrackIndex++;
        if (currentTrackIndex >= backgroundMusicTracks.Length)
        {
            currentTrackIndex = 0;
            if (shufflePlaylist)
            {
                InitializeShuffledPlaylist();
            }
        }

        PlayCurrentTrack();

        // Fade in new music (only if not in turn phase)
        if (!isTurnPhase)
        {
            yield return FadeCoroutine(volume, crossfadeDuration);
        }
    }

    private System.Collections.IEnumerator CrossfadeToCurrentTrack()
    {
        // Fade out current music
        yield return FadeCoroutine(0f, crossfadeDuration);

        PlayCurrentTrack();

        // Fade in new music (only if not in turn phase)
        if (!isTurnPhase)
        {
            yield return FadeCoroutine(volume, crossfadeDuration);
        }
    }
}