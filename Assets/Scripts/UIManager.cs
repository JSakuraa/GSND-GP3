using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using MusicDefinitions;

public class UIManager : MonoBehaviour
{
    // Singleton pattern for easy access
    public static UIManager Instance { get; private set; }

    [Header("Player References")]
    public PlayerUI player1;
    public PlayerUI player2;

    [Header("Health Bars")]
    public Slider player1HealthBar;
    public Slider player2HealthBar;
    public TextMeshProUGUI player1HealthText;
    public TextMeshProUGUI player2HealthText;

    [Header("Effect Spawns")]
    public Transform player1EffectSpawn;
    public Transform player2EffectSpawn;

    [Header("Particle Effects")]
    public GameObject healEffectPrefab;
    public GameObject hurtEffectPrefab;
    public GameObject buffEffectPrefab;
    public GameObject debuffEffectPrefab;

    [Header("Audio Sources")]
    public AudioSource noteAudioSource;
    public AudioSource chordAudioSource1;
    public AudioSource chordAudioSource2;
    public AudioSource chordAudioSource3;
    public AudioSource melodyAudioSource;

    [Header("Animation Settings")]
    public float healthBarAnimationSpeed = 2f;
    public float hurtFlashDuration = 0.5f;
    public float healGlowDuration = 0.8f;
    public Color hurtColor = Color.red;
    public Color healColor = Color.green;

    [Header("Battle State Reference")]
    public Battlestate battleState;

    // Call this when a battle completes to show all animations
    public void DisplayBattleResults(update_info updateInfo, Action player1Action, Action player2Action)
    {
        StartCoroutine(AnimateBattleSequence(updateInfo, player1Action, player2Action));
    }

    private IEnumerator AnimateBattleSequence(update_info updateInfo, Action player1Action, Action player2Action)
    {
        // Show each note resolution one by one
        for (int i = 0; i < updateInfo.Note_winners.Length; i++)
        {
            // Determine which players are affected
            PlayerUI winner = (updateInfo.Note_winners[i] == updateInfo.player1) ? player1 : player2;
            PlayerUI loser = (updateInfo.Note_winners[i] == updateInfo.player1) ? player2 : player1;

            // Get the winning note from the appropriate action
            Action winningAction = (updateInfo.Note_winners[i] == updateInfo.player1) ? player1Action : player2Action;
            MusicalNote winningNote = winningAction.melody.notes[i];

            // Play the note sound
            PlayNote(winningNote.ToString(), winner);

            yield return new WaitForSeconds(0.3f);

            // Show effects based on note effect type
            NoteEffect effect = updateInfo.Winning_note_effects[i];

            if (effect == NoteEffect.Heal)
            {
                PlayerHeal(winner);
                ShowEffect("heal", winner);
            }
            else if (effect == NoteEffect.Damage)
            {
                PlayerHurt(loser);
                ShowEffect("hurt", loser);
            }
            else if (effect == NoteEffect.LifeSteal)
            {
                PlayerHeal(winner);
                ShowEffect("heal", winner);
                yield return new WaitForSeconds(0.2f);
                PlayerHurt(loser);
                ShowEffect("hurt", loser);
            }

            yield return new WaitForSeconds(0.5f);
        }

        // Update health bars after all note animations
        AdjustHealthBar(player1, (int)updateInfo.player1.health);
        AdjustHealthBar(player2, (int)updateInfo.player2.health);

        // Show melody effect if there is one
        if (updateInfo.winning_melody_effect != null)
        {
            PlayerUI melodyWinner = (updateInfo.melody_winner == updateInfo.player1) ? player1 : player2;
            ShowEffect("buff", melodyWinner);
        }

        yield return new WaitForSeconds(1f);

        Debug.Log("Battle animation sequence complete");
    }

    // Helper method to convert MusicalNote enum to string
    private string MusicalNoteToString(MusicalNote note)
    {
        return note.ToString();
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // === BUTTON PRESS HANDLER ===
    public void ButtonPressed(string input, PlayerUI currentPlayer)
    {
        if (IsNote(input))
        {
            PlayNote(input, currentPlayer);
        }
        else if (IsChord(input))
        {
            PlayChord(input, currentPlayer);
        }
        else
        {
            Debug.LogWarning($"Unknown button input: {input}");
        }
    }

    private bool IsNote(string input)
    {
        string[] notes = { "C", "D", "E", "F", "G", "A", "B" };
        foreach (string note in notes)
        {
            if (input.Equals(note, System.StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }

    private bool IsChord(string input)
    {
        string[] chords = { "CMajor", "DMinor", "EMinor", "FMajor", "GMajor", "AMinor", "BDiminished" };
        foreach (string chord in chords)
        {
            if (input.Equals(chord, System.StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }

    // === PLAY NOTE (from specific player's instrument) ===
    public void PlayNote(string noteName, PlayerUI playerUI)
    {
        if (playerUI == null || playerUI.instrument == null)
        {
            Debug.LogWarning("PlayerUI or instrument is null!");
            return;
        }

        AudioClip noteClip = playerUI.instrument.GetNoteClip(noteName);

        if (noteClip != null && noteAudioSource != null)
        {
            noteAudioSource.PlayOneShot(noteClip);
            Debug.Log($"Playing {playerUI.playerName}'s {playerUI.instrument.instrumentName} note: {noteName}");

            // Optional: Animate instrument when playing note
            if (playerUI.instrument.instrumentSprite != null)
            {
                StartCoroutine(InstrumentPlayAnimation(playerUI.instrument.instrumentSprite));
            }
        }
        else
        {
            Debug.LogWarning($"Note clip missing for: {noteName} on {playerUI.instrument.instrumentName}");
        }
    }

    // Small bounce animation when instrument plays a note
    private IEnumerator InstrumentPlayAnimation(Image sprite)
    {
        Vector3 originalScale = sprite.transform.localScale;
        float duration = 0.2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float scale = 1f + Mathf.Sin(t * Mathf.PI) * 0.05f;
            sprite.transform.localScale = originalScale * scale;
            yield return null;
        }

        sprite.transform.localScale = originalScale;
    }

    // === PLAY CHORD (by combining notes) ===
    public void PlayChord(string chordName, PlayerUI playerUI)
    {
        string[] chordNotes = GetChordNotes(chordName);

        if (chordNotes == null || chordNotes.Length == 0)
        {
            Debug.LogWarning($"Unknown chord: {chordName}");
            return;
        }

        if (playerUI == null || playerUI.instrument == null)
        {
            Debug.LogWarning("PlayerUI or instrument is null!");
            return;
        }

        // Play all notes of the chord simultaneously
        AudioSource[] chordSources = { chordAudioSource1, chordAudioSource2, chordAudioSource3 };

        for (int i = 0; i < chordNotes.Length && i < chordSources.Length; i++)
        {
            AudioClip noteClip = playerUI.instrument.GetNoteClip(chordNotes[i]);
            if (noteClip != null && chordSources[i] != null)
            {
                chordSources[i].PlayOneShot(noteClip);
            }
        }

        // Animate instrument when playing chord
        if (playerUI.instrument.instrumentSprite != null)
        {
            StartCoroutine(InstrumentPlayAnimation(playerUI.instrument.instrumentSprite));
        }

        Debug.Log($"Playing {chordName} chord ({string.Join(", ", chordNotes)}) on {playerUI.instrument.instrumentName}");
    }

    private string[] GetChordNotes(string chordName)
    {
        switch (chordName)
        {
            case "CMajor":
                return new string[] { "C", "E", "G" };
            case "DMinor":
                return new string[] { "D", "F", "A" };
            case "EMinor":
                return new string[] { "E", "G", "B" };
            case "FMajor":
                return new string[] { "F", "A", "C" };
            case "GMajor":
                return new string[] { "G", "B", "D" };
            case "AMinor":
                return new string[] { "A", "C", "E" };
            case "BDiminished":
                return new string[] { "B", "D", "F" };
            default:
                return null;
        }
    }

    // === PLAY OUTCOME (Melody + Chord) ===
    public void PlayOutcome(string[] melody, string chord, PlayerUI playerUI)
    {
        StartCoroutine(PlayMelodyWithChord(melody, chord, playerUI));
    }

    private IEnumerator PlayMelodyWithChord(string[] melody, string chord, PlayerUI playerUI)
    {
        // Start playing the chord as backing (combined notes)
        PlayChord(chord, playerUI);

        // Wait a brief moment for chord to establish
        yield return new WaitForSeconds(0.2f);

        // Play each note in the melody from the player's instrument
        foreach (string note in melody)
        {
            PlayNote(note, playerUI);
            yield return new WaitForSeconds(0.5f);
        }

        Debug.Log($"Finished playing {playerUI.playerName}'s melody with {chord} chord");
    }

    // === ADJUST HEALTH BAR ===
    public void AdjustHealthBar(PlayerUI playerUI, int newHealth)
    {
        Slider targetBar = (playerUI == player1) ? player1HealthBar : player2HealthBar;
        TextMeshProUGUI targetText = (playerUI == player1) ? player1HealthText : player2HealthText;

        StartCoroutine(AnimateHealthBar(targetBar, targetText, newHealth, playerUI.maxHealth));
    }

    private IEnumerator AnimateHealthBar(Slider healthBar, TextMeshProUGUI healthText, int targetHealth, int maxHealth)
    {
        float targetValue = (float)targetHealth / maxHealth;
        float startValue = healthBar.value;
        float elapsed = 0f;
        float duration = 0.3f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            healthBar.value = Mathf.Lerp(startValue, targetValue, elapsed / duration);

            int displayHealth = Mathf.RoundToInt(healthBar.value * maxHealth);
            healthText.text = $"{displayHealth}/{maxHealth}";

            yield return null;
        }

        healthBar.value = targetValue;
        healthText.text = $"{targetHealth}/{maxHealth}";
    }

    // === PLAYER HURT ===
    // This animates both the player (musician) and their instrument (monster)
    public void PlayerHurt(PlayerUI playerUI)
    {
        // Animate the player/musician sprite
        if (playerUI.playerSprite != null)
        {
            StartCoroutine(HurtAnimation(playerUI.playerSprite));
        }

        // Animate the instrument/monster sprite
        if (playerUI.instrument != null && playerUI.instrument.instrumentSprite != null)
        {
            StartCoroutine(HurtAnimation(playerUI.instrument.instrumentSprite));
        }
    }

    private IEnumerator HurtAnimation(Image sprite)
    {
        Color originalColor = sprite.color;
        Vector3 originalPosition = sprite.transform.localPosition;
        float elapsed = 0f;

        while (elapsed < hurtFlashDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / hurtFlashDuration;

            sprite.color = Color.Lerp(hurtColor, originalColor, t);

            float shakeAmount = 10f * (1 - t);
            sprite.transform.localPosition = originalPosition + new Vector3(
                Random.Range(-shakeAmount, shakeAmount),
                Random.Range(-shakeAmount, shakeAmount),
                0
            );

            yield return null;
        }

        sprite.color = originalColor;
        sprite.transform.localPosition = originalPosition;
    }

    // === PLAYER HEAL ===
    // This animates both the player (musician) and their instrument (monster)
    public void PlayerHeal(PlayerUI playerUI)
    {
        // Animate the player/musician sprite
        if (playerUI.playerSprite != null)
        {
            StartCoroutine(HealAnimation(playerUI.playerSprite));
        }

        // Animate the instrument/monster sprite
        if (playerUI.instrument != null && playerUI.instrument.instrumentSprite != null)
        {
            StartCoroutine(HealAnimation(playerUI.instrument.instrumentSprite));
        }
    }

    private IEnumerator HealAnimation(Image sprite)
    {
        Color originalColor = sprite.color;
        Vector3 originalScale = sprite.transform.localScale;
        float elapsed = 0f;

        while (elapsed < healGlowDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / healGlowDuration;

            sprite.color = Color.Lerp(healColor, originalColor, t);

            float scale = 1f + Mathf.Sin(t * Mathf.PI) * 0.1f;
            sprite.transform.localScale = originalScale * scale;

            yield return null;
        }

        sprite.color = originalColor;
        sprite.transform.localScale = originalScale;
    }

    // === SHOW EFFECT ===
    public void ShowEffect(string effectType, PlayerUI playerUI)
    {
        Transform spawnPoint = (playerUI == player1) ? player1EffectSpawn : player2EffectSpawn;

        GameObject effectPrefab = null;

        switch (effectType.ToLower())
        {
            case "heal":
                effectPrefab = healEffectPrefab;
                break;
            case "hurt":
                effectPrefab = hurtEffectPrefab;
                break;
            case "buff":
                effectPrefab = buffEffectPrefab;
                break;
            default:
                Debug.LogWarning($"Unknown effect type: {effectType}");
                return;
        }

        if (effectPrefab != null && spawnPoint != null)
        {
            GameObject effect = Instantiate(effectPrefab, spawnPoint.position, Quaternion.identity, spawnPoint);
            Destroy(effect, 2f);
            Debug.Log($"Spawned {effectType} effect for {playerUI.playerName}");
        }
    }

    // === SHOW DEBUFF ===
    public void ShowDebuff(PlayerUI playerUI)
    {
        Transform spawnPoint = (playerUI == player1) ? player1EffectSpawn : player2EffectSpawn;

        if (debuffEffectPrefab != null && spawnPoint != null)
        {
            GameObject effect = Instantiate(debuffEffectPrefab, spawnPoint.position, Quaternion.identity, spawnPoint);
            Destroy(effect, 2f);
            Debug.Log($"Spawned debuff effect for {playerUI.playerName}");
        }
    }
}