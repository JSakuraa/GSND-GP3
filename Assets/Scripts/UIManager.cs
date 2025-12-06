using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using MusicDefinitions;
using BattleDefinitions;

public class UIManager : MonoBehaviour
{
    // Singleton pattern for easy access
    public static UIManager Instance { get; private set; }

    public System.Action onBattleAnimationComplete;

    [Header("Player References")]
    public PlayerUI player1;
    public PlayerUI player2;

    [Header("Health Bars")]
    public Slider player1HealthBar;
    public Slider player2HealthBar;
    public TextMeshProUGUI player1HealthText;
    public TextMeshProUGUI player2HealthText;

    [Header("Note Display UI")]
    public Transform[] player1NoteSpawnPoints; // Array of 4 transforms
    public Transform[] player2NoteSpawnPoints; // Array of 4 transforms

    [Header("Note Icon Prefabs")]
    public GameObject noteIconC;
    public GameObject noteIconD;
    public GameObject noteIconE;
    public GameObject noteIconF;
    public GameObject noteIconG;
    public GameObject noteIconA;
    public GameObject noteIconB;

    [Header("Health Change Indicators")]
    public TextMeshProUGUI player1HealthChangeText;
    public TextMeshProUGUI player2HealthChangeText;

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

    [Header("Animation Settings")]
    public float healthBarAnimationSpeed = 2f;
    public float hurtFlashDuration = 0.5f;
    public float healGlowDuration = 0.8f;
    public Color hurtColor = Color.red;
    public Color healColor = Color.green;

    [Header("Playback Settings")]
    public float notePlaybackDelay = 0.5f; // Time between notes during playback

    // Store spawned icons so we can modify them later
    private List<GameObject> player1SpawnedIcons = new List<GameObject>();
    private List<GameObject> player2SpawnedIcons = new List<GameObject>();

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

    // === PLAY MELODY (for playback button) ===
    public void PlayMelody(string[] melody, PlayerUI playerUI)
    {
        StartCoroutine(PlayMelodySequence(melody, playerUI));
    }

    private IEnumerator PlayMelodySequence(string[] melody, PlayerUI playerUI)
    {
        foreach (string note in melody)
        {
            PlayNote(note, playerUI);
            yield return new WaitForSeconds(notePlaybackDelay);
        }
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
        // Always reset to white first to ensure clean state
        Color baseColor = Color.white;
        Vector3 originalPosition = sprite.transform.localPosition;
        float elapsed = 0f;

        while (elapsed < hurtFlashDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / hurtFlashDuration;

            // Lerp from hurt color back to white
            sprite.color = Color.Lerp(hurtColor, baseColor, t);

            float shakeAmount = 10f * (1 - t);
            sprite.transform.localPosition = originalPosition + new Vector3(
                Random.Range(-shakeAmount, shakeAmount),
                Random.Range(-shakeAmount, shakeAmount),
                0
            );

            yield return null;
        }

        // Ensure we end at white
        sprite.color = baseColor;
        sprite.transform.localPosition = originalPosition;
    }

    // === PLAYER HEAL ===
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
        // Always reset to white first to ensure clean state
        Color baseColor = Color.white;
        Vector3 originalScale = sprite.transform.localScale;
        float elapsed = 0f;

        while (elapsed < healGlowDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / healGlowDuration;

            // Lerp from heal color back to white
            sprite.color = Color.Lerp(healColor, baseColor, t);

            float scale = 1f + Mathf.Sin(t * Mathf.PI) * 0.1f;
            sprite.transform.localScale = originalScale * scale;

            yield return null;
        }

        // Ensure we end at white
        sprite.color = baseColor;
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

    // === DISPLAY BATTLE RESULTS ===
    public void DisplayBattleResults(update_info updateInfo, Action player1Action, Action player2Action, bool player1GoesFirst)
    {
        StartCoroutine(AnimateBattleSequence(updateInfo, player1Action, player2Action, player1GoesFirst));
    }

    private IEnumerator AnimateBattleSequence(update_info updateInfo, Action player1Action, Action player2Action, bool player1GoesFirst)
    {
        // Clear previous icons
        ClearNoteIcons();

        // Clear health change text
        if (player1HealthChangeText != null) player1HealthChangeText.text = "";
        if (player2HealthChangeText != null) player2HealthChangeText.text = "";

        // Determine which action goes first based on turn order
        Action firstAction = player1GoesFirst ? player1Action : player2Action;
        Action secondAction = player1GoesFirst ? player2Action : player1Action;
        PlayerUI firstPlayer = player1GoesFirst ? player1 : player2;
        PlayerUI secondPlayer = player1GoesFirst ? player2 : player1;
        Transform[] firstSpawnPoints = player1GoesFirst ? player1NoteSpawnPoints : player2NoteSpawnPoints;
        Transform[] secondSpawnPoints = player1GoesFirst ? player2NoteSpawnPoints : player1NoteSpawnPoints;
        List<GameObject> firstIconList = player1GoesFirst ? player1SpawnedIcons : player2SpawnedIcons;
        List<GameObject> secondIconList = player1GoesFirst ? player2SpawnedIcons : player1SpawnedIcons;

        // PHASE 1: First player plays their melody
        Debug.Log($"{firstPlayer.playerName} plays melody first...");
        for (int i = 0; i < firstAction.melody.notes.Length; i++)
        {
            MusicalNote note = firstAction.melody.notes[i];

            // Play the note
            PlayNote(note.ToString(), firstPlayer);

            // Spawn the icon at the correct position
            SpawnNoteIcon(note, firstSpawnPoints[i], firstIconList);

            yield return new WaitForSeconds(notePlaybackDelay);
        }

        yield return new WaitForSeconds(0.5f);

        // PHASE 2: Second player plays their melody
        Debug.Log($"{secondPlayer.playerName} plays melody second...");
        for (int i = 0; i < secondAction.melody.notes.Length; i++)
        {
            MusicalNote note = secondAction.melody.notes[i];

            // Play the note
            PlayNote(note.ToString(), secondPlayer);

            // Spawn the icon at the correct position
            SpawnNoteIcon(note, secondSpawnPoints[i], secondIconList);

            yield return new WaitForSeconds(notePlaybackDelay);
        }

        yield return new WaitForSeconds(0.5f);

        // PHASE 3: Highlight winning notes one by one
        Debug.Log("Comparing notes...");
        for (int i = 0; i < updateInfo.Note_winners.Length; i++)
        {
            // Check the raw resolution outcome
            double outcome = updateInfo.resolution_outcomes[i];

            if (outcome == 0)
            {
                // Tie - dim both icons
                HighlightIcon(player1SpawnedIcons[i], false);
                HighlightIcon(player2SpawnedIcons[i], false);
            }
            else if (outcome > 0)
            {
                // Player 1 won this note
                HighlightIcon(player1SpawnedIcons[i], true);
                HighlightIcon(player2SpawnedIcons[i], false);
            }
            else // outcome < 0
            {
                // Player 2 won this note
                HighlightIcon(player1SpawnedIcons[i], false);
                HighlightIcon(player2SpawnedIcons[i], true);
            }

            yield return new WaitForSeconds(0.6f);
        }

        yield return new WaitForSeconds(0.5f);

        // PHASE 4: Cast spell animations for each player based on effects
        Debug.Log("Casting spells...");

        // Calculate total health changes to determine actual outcome
        double player1TotalChange = 0;
        double player2TotalChange = 0;

        for (int i = 0; i < updateInfo.p1hp_change.Length; i++)
        {
            player1TotalChange += updateInfo.p1hp_change[i];
            player2TotalChange += updateInfo.p2hp_change[i];
        }

        // Show animations based on NET health change
        // Player 1 effects
        if (player1TotalChange < 0)
        {
            PlayerHurt(player1);
            ShowEffect("hurt", player1);
        }
        else if (player1TotalChange > 0)
        {
            PlayerHeal(player1);
            ShowEffect("heal", player1);
        }

        // Player 2 effects
        if (player2TotalChange < 0)
        {
            PlayerHurt(player2);
            ShowEffect("hurt", player2);
        }
        else if (player2TotalChange > 0)
        {
            PlayerHeal(player2);
            ShowEffect("heal", player2);
        }

        // Show melody effect if there is one
        if (updateInfo.winning_melody_effect != null)
        {
            PlayerUI melodyWinner = (updateInfo.melody_winner == updateInfo.player1) ? player1 : player2;
            ShowEffect("buff", melodyWinner);
        }

        yield return new WaitForSeconds(1f);

        // PHASE 5: Update health bars and show change indicators
        Debug.Log("Updating health...");

        // Show health change text indicators
        if (player1HealthChangeText != null)
        {
            string changeText = FormatHealthChange(player1TotalChange);
            player1HealthChangeText.text = changeText;
            StartCoroutine(FadeOutHealthChangeText(player1HealthChangeText));
        }

        if (player2HealthChangeText != null)
        {
            string changeText = FormatHealthChange(player2TotalChange);
            player2HealthChangeText.text = changeText;
            StartCoroutine(FadeOutHealthChangeText(player2HealthChangeText));
        }

        // Update health bars
        AdjustHealthBar(player1, (int)updateInfo.player1.health);
        AdjustHealthBar(player2, (int)updateInfo.player2.health);

        yield return new WaitForSeconds(2f);

        // Clear icons for next turn
        ClearNoteIcons();

        Debug.Log("Battle animation sequence complete");

        if (onBattleAnimationComplete != null)
        {
            onBattleAnimationComplete.Invoke();
        }
    }

    // === ICON MANAGEMENT ===

    // Spawn a note icon at the specified position
    private void SpawnNoteIcon(MusicalNote note, Transform spawnPoint, List<GameObject> iconList)
    {
        if (spawnPoint == null)
        {
            Debug.LogWarning("Spawn point is null!");
            return;
        }

        GameObject iconPrefab = GetNoteIconPrefab(note);

        if (iconPrefab == null)
        {
            Debug.LogWarning($"No icon prefab for note: {note}");
            return;
        }

        // Instantiate the icon at the spawn point
        GameObject spawnedIcon = Instantiate(iconPrefab, spawnPoint.position, Quaternion.identity, spawnPoint);

        // Store reference so we can modify it later
        iconList.Add(spawnedIcon);

        // Optional: Add a pop-in animation
        StartCoroutine(PopInAnimation(spawnedIcon.transform));
    }

    // Get the correct icon prefab for a musical note
    private GameObject GetNoteIconPrefab(MusicalNote note)
    {
        switch (note)
        {
            case MusicalNote.C: return noteIconC;
            case MusicalNote.D: return noteIconD;
            case MusicalNote.E: return noteIconE;
            case MusicalNote.F: return noteIconF;
            case MusicalNote.G: return noteIconG;
            case MusicalNote.A: return noteIconA;
            case MusicalNote.B: return noteIconB;
            default: return null;
        }
    }

    // Highlight icon as winner (green glow) or loser (red/dimmed)
    private void HighlightIcon(GameObject icon, bool isWinner)
    {
        if (icon == null) return;

        Image iconImage = icon.GetComponent<Image>();
        if (iconImage == null) return;

        if (isWinner)
        {
            // Green glow for winner
            iconImage.color = Color.green;
            StartCoroutine(PulseIcon(icon.transform));
        }
        else
        {
            // Red/dimmed for loser
            iconImage.color = new Color(1f, 0.3f, 0.3f, 0.6f); // Red and semi-transparent
        }
    }

    // Pop-in animation when icon spawns
    private IEnumerator PopInAnimation(Transform iconTransform)
    {
        Vector3 originalScale = Vector3.one;
        iconTransform.localScale = Vector3.zero;

        float duration = 0.2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            // Ease out back for a bounce effect
            float scale = Mathf.Lerp(0f, 1f, t);
            if (t > 0.5f)
            {
                scale = 1f + (1f - t) * 0.2f; // Slight overshoot
            }
            iconTransform.localScale = originalScale * scale;
            yield return null;
        }

        iconTransform.localScale = originalScale;
    }

    // Pulse animation for winning icon
    private IEnumerator PulseIcon(Transform iconTransform)
    {
        Vector3 originalScale = iconTransform.localScale;
        float duration = 0.4f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float scale = 1f + Mathf.Sin(t * Mathf.PI) * 0.2f;
            iconTransform.localScale = originalScale * scale;
            yield return null;
        }

        iconTransform.localScale = originalScale;
    }

    // Clear all spawned note icons
    private void ClearNoteIcons()
    {
        foreach (GameObject icon in player1SpawnedIcons)
        {
            if (icon != null)
            {
                Destroy(icon);
            }
        }
        player1SpawnedIcons.Clear();

        foreach (GameObject icon in player2SpawnedIcons)
        {
            if (icon != null)
            {
                Destroy(icon);
            }
        }
        player2SpawnedIcons.Clear();
    }

    // Helper method to format health change text
    private string FormatHealthChange(double change)
    {
        if (change > 0)
        {
            return $"<color=green>+{change:F0}</color>";
        }
        else if (change < 0)
        {
            return $"<color=red>{change:F0}</color>";
        }
        else
        {
            return "";
        }
    }

    // Coroutine to fade out health change text
    private IEnumerator FadeOutHealthChangeText(TextMeshProUGUI textComponent)
    {
        yield return new WaitForSeconds(1.5f);

        float duration = 1f;
        float elapsed = 0f;
        Color originalColor = textComponent.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            textComponent.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        textComponent.text = "";
        textComponent.color = originalColor; // Reset alpha for next time
    }

    public void UpdateMelodyPreview(List<MusicalNote> melodyNotes, bool isPlayer1)
    {
        Transform[] spawnPoints = isPlayer1 ? player1NoteSpawnPoints : player2NoteSpawnPoints;
        List<GameObject> iconList = isPlayer1 ? player1SpawnedIcons : player2SpawnedIcons;

        // Clear existing preview icons
        ClearPreviewIcons(isPlayer1);

        // Spawn icon for each selected note
        for (int i = 0; i < melodyNotes.Count && i < spawnPoints.Length; i++)
        {
            SpawnPreviewIcon(melodyNotes[i], spawnPoints[i], iconList);
        }
    }

    // Spawn a preview icon (no animation, just appears)
    private void SpawnPreviewIcon(MusicalNote note, Transform spawnPoint, List<GameObject> iconList)
    {
        if (spawnPoint == null)
        {
            Debug.LogWarning("Spawn point is null!");
            return;
        }

        GameObject iconPrefab = GetNoteIconPrefab(note);

        if (iconPrefab == null)
        {
            Debug.LogWarning($"No icon prefab for note: {note}");
            return;
        }

        // Instantiate the icon at the spawn point (no animation)
        GameObject spawnedIcon = Instantiate(iconPrefab, spawnPoint.position, Quaternion.identity, spawnPoint);

        // Make it slightly transparent to show it's a preview
        Image iconImage = spawnedIcon.GetComponent<Image>();
        if (iconImage != null)
        {
            Color color = iconImage.color;
            color.a = 0.7f; // Slightly transparent
            iconImage.color = color;
        }

        // Store reference
        iconList.Add(spawnedIcon);
    }

    // Clear only preview icons for one player
    private void ClearPreviewIcons(bool isPlayer1)
    {
        List<GameObject> iconList = isPlayer1 ? player1SpawnedIcons : player2SpawnedIcons;

        foreach (GameObject icon in iconList)
        {
            if (icon != null)
            {
                Destroy(icon);
            }
        }
        iconList.Clear();
    }

    // Clear all preview icons (both players)
    public void ClearAllPreviewIcons()
    {
        ClearPreviewIcons(true);
        ClearPreviewIcons(false);
    }
}

