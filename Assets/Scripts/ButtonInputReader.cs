using BattleDefinitions;
using MusicDefinitions;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class ButtonInputReader : MonoBehaviour
{
    [Header("UI References")]
    public TMPro.TMP_Text displayText;
    public TMPro.TMP_Text turnIndicatorText;
    public TMPro.TMP_Text chordDisplayText;
    public TMPro.TMP_Text melodyDisplayText;

    [Header("Button Panels")]
    public GameObject chordButtons;
    public GameObject noteButtons;
    public GameObject confirmButton;
    public GameObject backspaceButton;

    [Header("Battle State")]
    public Battlestate bs;

    [Header("UI Manager")]
    public UIManager uiManager;

    // Track current turn and input state
    private enum TurnPhase { Player1Chord, Player1Melody, Player2Chord, Player2Melody, BattleResolution }
    private TurnPhase currentPhase = TurnPhase.Player1Chord;

    // Store selected notes
    private Chord selectedChord;
    private string selectedChordName; // Track chord name for UI
    private List<MusicalNote> currentMelodyNotes = new List<MusicalNote>();

    // Store complete actions
    private Action player1Action;
    private Action player2Action;

    // Preset chords
    private Dictionary<string, Chord> presetChords = new Dictionary<string, Chord>()
    {
        { "CMajor", new Chord(MusicalNote.C, MusicalNote.E, MusicalNote.G) },
        { "DMinor", new Chord(MusicalNote.D, MusicalNote.F, MusicalNote.A) },
        { "EMinor", new Chord(MusicalNote.E, MusicalNote.G, MusicalNote.B) },
        { "FMajor", new Chord(MusicalNote.F, MusicalNote.A, MusicalNote.C) },
        { "GMajor", new Chord(MusicalNote.G, MusicalNote.B, MusicalNote.D) },
        { "AMinor", new Chord(MusicalNote.A, MusicalNote.C, MusicalNote.E) },
        { "BDim", new Chord(MusicalNote.B, MusicalNote.D, MusicalNote.F) }
    };

    private const int MELODY_SIZE = 4;

    void Start()
    {
        // Initialize battle state players first
        bs.init();  // Make sure players are created

        // Connect PlayerUI to game logic Players
        if (uiManager != null)
        {
            // The playerName in PlayerUI should match the names in Battlestate
            // Or we can just directly link them
            ConnectPlayersToUI();

            // Initialize health bars
            uiManager.AdjustHealthBar(uiManager.player1, (int)bs.player1.health);
            uiManager.AdjustHealthBar(uiManager.player2, (int)bs.player2.health);
        }

        UpdateDisplay();
        UpdateTurnIndicator();
        ShowChordButtons();
    }

    // Add this Update method to ButtonInputReader
    void Update()
    {
        // Get current keyboard state
        var keyboard = Keyboard.current;

        if (keyboard == null) return;  // No keyboard connected

        // Keyboard input for notes
        if (keyboard.cKey.wasPressedThisFrame)
        {
            OnNoteButtonClicked("C");
        }
        else if (keyboard.dKey.wasPressedThisFrame)
        {
            OnNoteButtonClicked("D");
        }
        else if (keyboard.eKey.wasPressedThisFrame)
        {
            OnNoteButtonClicked("E");
        }
        else if (keyboard.fKey.wasPressedThisFrame)
        {
            OnNoteButtonClicked("F");
        }
        else if (keyboard.gKey.wasPressedThisFrame)
        {
            OnNoteButtonClicked("G");
        }
        else if (keyboard.aKey.wasPressedThisFrame)
        {
            OnNoteButtonClicked("A");
        }
        else if (keyboard.bKey.wasPressedThisFrame)
        {
            OnNoteButtonClicked("B");
        }

        // Keyboard input for chords (number keys 1-7)
        if (keyboard.digit1Key.wasPressedThisFrame || keyboard.numpad1Key.wasPressedThisFrame)
        {
            OnChordButtonClicked("CMajor");
        }
        else if (keyboard.digit2Key.wasPressedThisFrame || keyboard.numpad2Key.wasPressedThisFrame)
        {
            OnChordButtonClicked("DMinor");
        }
        else if (keyboard.digit3Key.wasPressedThisFrame || keyboard.numpad3Key.wasPressedThisFrame)
        {
            OnChordButtonClicked("EMinor");
        }
        else if (keyboard.digit4Key.wasPressedThisFrame || keyboard.numpad4Key.wasPressedThisFrame)
        {
            OnChordButtonClicked("FMajor");
        }
        else if (keyboard.digit5Key.wasPressedThisFrame || keyboard.numpad5Key.wasPressedThisFrame)
        {
            OnChordButtonClicked("GMajor");
        }
        else if (keyboard.digit6Key.wasPressedThisFrame || keyboard.numpad6Key.wasPressedThisFrame)
        {
            OnChordButtonClicked("AMinor");
        }
        else if (keyboard.digit7Key.wasPressedThisFrame || keyboard.numpad7Key.wasPressedThisFrame)
        {
            OnChordButtonClicked("BDim");
        }

        // Backspace key
        if (keyboard.backspaceKey.wasPressedThisFrame)
        {
            OnBackspaceButtonClicked();
        }

        // Enter/Return key for confirm
        if (keyboard.enterKey.wasPressedThisFrame || keyboard.numpadEnterKey.wasPressedThisFrame)
        {
            OnConfirmButtonClicked();
        }
    }

    private void ConnectPlayersToUI()
    {
        // Make sure PlayerUI objects exist
        if (uiManager.player1 == null || uiManager.player2 == null)
        {
            Debug.LogError("PlayerUI objects not assigned in UIManager!");
            return;
        }

        // Connect them (optional, depending on if you need this link)
        uiManager.player1.playerName = bs.player1.name;
        uiManager.player2.playerName = bs.player2.name;
    }

    // Replace OnChordButtonClicked with this version that has detailed debugging:
    public void OnChordButtonClicked(string chordName)
    {
        if (currentPhase != TurnPhase.Player1Chord && currentPhase != TurnPhase.Player2Chord)
            return;

        if (presetChords.ContainsKey(chordName))
        {
            selectedChord = presetChords[chordName];
            selectedChordName = chordName;
            UpdateChordDisplay(chordName);
            confirmButton.SetActive(true);

            // Play chord sound through UIManager - with detailed debugging
            if (uiManager == null)
            {
                Debug.LogError("UIManager is null!");
                return;
            }

            if (uiManager.player1 == null)
            {
                Debug.LogError("UIManager.player1 is null!");
                return;
            }

            if (uiManager.player2 == null)
            {
                Debug.LogError("UIManager.player2 is null!");
                return;
            }

            PlayerUI currentPlayer = (currentPhase == TurnPhase.Player1Chord) ?
                uiManager.player1 : uiManager.player2;

            if (currentPlayer == null)
            {
                Debug.LogError("currentPlayer is null!");
                return;
            }

            if (currentPlayer.instrument == null)
            {
                Debug.LogError($"currentPlayer.instrument is null for {currentPlayer.playerName}!");
                return;
            }

            Debug.Log($"Playing chord {chordName} for {currentPlayer.playerName} on {currentPlayer.instrument.instrumentName}");
            uiManager.PlayChord(chordName, currentPlayer);
        }
        else
        {
            Debug.LogError($"Chord '{chordName}' not found in preset chords!");
        }
    }

    // Called by individual note buttons
    public void OnNoteButtonClicked(string noteName)
    {
        if (currentPhase != TurnPhase.Player1Melody && currentPhase != TurnPhase.Player2Melody)
            return;

        MusicalNote selectedNote = (MusicalNote)System.Enum.Parse(typeof(MusicalNote), noteName);

        if (currentMelodyNotes.Count < MELODY_SIZE)
        {
            currentMelodyNotes.Add(selectedNote);
            UpdateMelodyDisplay();

            // Play note sound through UIManager
            PlayerUI currentPlayer = (currentPhase == TurnPhase.Player1Melody) ?
                uiManager.player1 : uiManager.player2;

            if (currentPlayer != null && currentPlayer.instrument != null)
            {
                uiManager.PlayNote(noteName, currentPlayer);
            }

            // Only show confirm when melody is complete
            if (currentMelodyNotes.Count == MELODY_SIZE)
            {
                confirmButton.SetActive(true);
            }
        }
    }

    public void OnBackspaceButtonClicked()
    {
        if (currentPhase == TurnPhase.Player1Melody || currentPhase == TurnPhase.Player2Melody)
        {
            if (currentMelodyNotes.Count > 0)
            {
                currentMelodyNotes.RemoveAt(currentMelodyNotes.Count - 1);
                UpdateMelodyDisplay();

                // Hide confirm button if melody is no longer complete
                if (currentMelodyNotes.Count < MELODY_SIZE)
                {
                    confirmButton.SetActive(false);
                }
            }
        }
    }

    public void OnConfirmButtonClicked()
    {
        switch (currentPhase)
        {
            case TurnPhase.Player1Chord:
                if (selectedChord != null)
                {
                    currentPhase = TurnPhase.Player1Melody;
                    UpdateTurnIndicator();
                    ShowMelodyButtons();  // This now handles all button visibility
                }
                break;

            case TurnPhase.Player1Melody:
                if (currentMelodyNotes.Count == MELODY_SIZE)
                {
                    player1Action = new Action(
                        selectedChord,
                        new Melody(currentMelodyNotes.ToArray()),
                        bs.player1
                    );

                    currentPhase = TurnPhase.Player2Chord;
                    selectedChord = null;
                    selectedChordName = null;
                    currentMelodyNotes.Clear();
                    chordDisplayText.text = "Chord: Not selected";
                    melodyDisplayText.text = "Melody: (0/4)";
                    UpdateTurnIndicator();
                    ShowChordButtons();  // This now handles all button visibility
                }
                break;

            case TurnPhase.Player2Chord:
                if (selectedChord != null)
                {
                    currentPhase = TurnPhase.Player2Melody;
                    UpdateTurnIndicator();
                    ShowMelodyButtons();  // This now handles all button visibility
                }
                break;

            case TurnPhase.Player2Melody:
                if (currentMelodyNotes.Count == MELODY_SIZE)
                {
                    player2Action = new Action(
                        selectedChord,
                        new Melody(currentMelodyNotes.ToArray()),
                        bs.player2
                    );

                    ResolveBattle();
                }
                break;
        }
    }


    private void ResolveBattle()
    {
        currentPhase = TurnPhase.BattleResolution;

        // Hide input buttons during battle animation
        chordButtons.SetActive(false);
        noteButtons.SetActive(false);
        confirmButton.SetActive(false);
        if (backspaceButton != null)
            backspaceButton.SetActive(false);

        // Execute the battle (game logic)
        bs.battle(player1Action, player2Action);

        // Trigger UI animations
        if (uiManager != null)
        {
            uiManager.DisplayBattleResults(bs.last_update, player1Action, player2Action);
        }

        // Update text display
        UpdateDisplay();

        // Check for game over
        if (bs.player1.health <= 0 || bs.player2.health <= 0)
        {
            Invoke("HandleGameOver", 3f);
        }
        else
        {
            Invoke("StartNextTurn", 3f);
        }
    }

    // Update HandleGameOver to hide backspace
    private void HandleGameOver()
    {
        chordButtons.SetActive(false);
        noteButtons.SetActive(false);
        confirmButton.SetActive(false);
        if (backspaceButton != null)
            backspaceButton.SetActive(false);

        if (bs.player1.health <= 0 && bs.player2.health <= 0)
        {
            turnIndicatorText.text = "Draw!";
        }
        else if (bs.player1.health <= 0)
        {
            turnIndicatorText.text = "Player 2 Wins!";
        }
        else
        {
            turnIndicatorText.text = "Player 1 Wins!";
        }
    }

    private void StartNextTurn()
    {
        currentPhase = TurnPhase.Player1Chord;
        selectedChord = null;
        selectedChordName = null;
        currentMelodyNotes.Clear();
        chordDisplayText.text = "Chord: Not selected";
        melodyDisplayText.text = "Melody: (0/4)";
        UpdateTurnIndicator();
        ShowChordButtons();
    }

    private void ShowChordButtons()
    {
        chordButtons.SetActive(true);
        noteButtons.SetActive(false);
        confirmButton.SetActive(false);
        if (backspaceButton != null)
            backspaceButton.SetActive(false);  // Hide backspace during chord selection
    }

    private void ShowMelodyButtons()
    {
        chordButtons.SetActive(false);
        noteButtons.SetActive(true);
        confirmButton.SetActive(false);
        if (backspaceButton != null)
            backspaceButton.SetActive(true);  // Show backspace during melody selection
    }

    private void UpdateDisplay()
    {
        if (displayText != null)
        {
            displayText.text = $"P1 Health: {bs.player1.health:F0} | P2 Health: {bs.player2.health:F0}";
        }
    }

    private void UpdateTurnIndicator()
    {
        if (turnIndicatorText == null) return;

        switch (currentPhase)
        {
            case TurnPhase.Player1Chord:
                turnIndicatorText.text = "Player 1: Select a Chord";
                break;
            case TurnPhase.Player1Melody:
                turnIndicatorText.text = "Player 1: Select Melody (4 notes)";
                break;
            case TurnPhase.Player2Chord:
                turnIndicatorText.text = "Player 2: Select a Chord";
                break;
            case TurnPhase.Player2Melody:
                turnIndicatorText.text = "Player 2: Select Melody (4 notes)";
                break;
            case TurnPhase.BattleResolution:
                turnIndicatorText.text = "Battle Resolution...";
                break;
        }
    }

    private void UpdateChordDisplay(string chordName)
    {
        if (chordDisplayText == null) return;

        if (string.IsNullOrEmpty(chordName))
        {
            chordDisplayText.text = "Chord: Not selected";
        }
        else
        {
            chordDisplayText.text = $"Chord: {chordName}";
        }
    }

    private void UpdateMelodyDisplay()
    {
        if (melodyDisplayText == null) return;

        string display = "Melody: ";
        foreach (MusicalNote note in currentMelodyNotes)
        {
            display += note.ToString() + " ";
        }
        display += $"({currentMelodyNotes.Count}/{MELODY_SIZE})";

        melodyDisplayText.text = display;
    }
}