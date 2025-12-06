using BattleDefinitions;
using MusicDefinitions;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class ButtonInputReader : MonoBehaviour
{
    [Header("UI References")]
    public TMPro.TMP_Text turnIndicatorText;

    [Header("Button Panels")]
    public GameObject noteButtons;
    public GameObject confirmButton;
    public GameObject backspaceButton;
    public GameObject playbackButton;

    [Header("Battle State")]
    public Battlestate bs;

    [Header("UI Manager")]
    public UIManager uiManager;

    [Header("Background Music")]
    private BackgroundMusicManager bgMusic;

    // Track current turn and input state
    private enum TurnPhase { FirstPlayerMelody, SecondPlayerMelody, BattleResolution }
    private TurnPhase currentPhase = TurnPhase.FirstPlayerMelody;

    // Track who goes first this round
    private bool player1GoesFirst = true;

    // Store selected notes
    private List<MusicalNote> currentMelodyNotes = new List<MusicalNote>();

    // Store complete actions
    private Action player1Action;
    private Action player2Action;

    // Constants
    private const int MELODY_SIZE = 4;

    void Start()
    {
        // Initialize battle state players first
        bs.init();

        // Connect PlayerUI to game logic Players
        if (uiManager != null)
        {
            ConnectPlayersToUI();

            // Initialize health bars
            uiManager.AdjustHealthBar(uiManager.player1, (int)bs.player1.health);
            uiManager.AdjustHealthBar(uiManager.player2, (int)bs.player2.health);
        }
        bgMusic = FindObjectOfType<BackgroundMusicManager>();

        UpdateTurnIndicator();
        ShowMelodyButtons();
    }

    private void ConnectPlayersToUI()
    {
        if (uiManager.player1 == null || uiManager.player2 == null)
        {
            Debug.LogError("PlayerUI objects not assigned in UIManager!");
            return;
        }

        uiManager.player1.playerName = bs.player1.name;
        uiManager.player2.playerName = bs.player2.name;
    }

    void Update()
    {
        var keyboard = UnityEngine.InputSystem.Keyboard.current;

        if (keyboard == null) return;

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

        if (keyboard.backspaceKey.wasPressedThisFrame)
        {
            OnBackspaceButtonClicked();
        }

        if (keyboard.enterKey.wasPressedThisFrame || keyboard.numpadEnterKey.wasPressedThisFrame)
        {
            OnConfirmButtonClicked();
        }

        if (keyboard.spaceKey.wasPressedThisFrame)
        {
            OnPlaybackButtonClicked();
        }
    }

    // Helper method to determine which player is currently selecting
    private PlayerUI GetCurrentPlayer()
    {
        if (currentPhase == TurnPhase.FirstPlayerMelody)
        {
            return player1GoesFirst ? uiManager.player1 : uiManager.player2;
        }
        else if (currentPhase == TurnPhase.SecondPlayerMelody)
        {
            return player1GoesFirst ? uiManager.player2 : uiManager.player1;
        }
        return null;
    }

    // Helper method to determine if current player is player1
    private bool IsCurrentPlayerOne()
    {
        if (currentPhase == TurnPhase.FirstPlayerMelody)
        {
            return player1GoesFirst;
        }
        else if (currentPhase == TurnPhase.SecondPlayerMelody)
        {
            return !player1GoesFirst;
        }
        return true;
    }

    // Called by individual note buttons
    public void OnNoteButtonClicked(string noteName)
    {
        if (currentPhase != TurnPhase.FirstPlayerMelody && currentPhase != TurnPhase.SecondPlayerMelody)
            return;

        MusicalNote selectedNote = (MusicalNote)System.Enum.Parse(typeof(MusicalNote), noteName);

        if (currentMelodyNotes.Count < MELODY_SIZE)
        {
            currentMelodyNotes.Add(selectedNote);

            // Update icon preview
            bool isPlayer1 = IsCurrentPlayerOne();
            uiManager.UpdateMelodyPreview(currentMelodyNotes, isPlayer1);

            // Play note sound through UIManager
            PlayerUI currentPlayer = GetCurrentPlayer();

            if (currentPlayer != null && currentPlayer.instrument != null)
            {
                uiManager.PlayNote(noteName, currentPlayer);
            }

            // Show confirm and playback when melody is complete
            if (currentMelodyNotes.Count == MELODY_SIZE)
            {
                confirmButton.SetActive(true);
                playbackButton.SetActive(true);
            }
            else
            {
                // Show playback as soon as there's at least one note
                if (currentMelodyNotes.Count > 0)
                {
                    playbackButton.SetActive(true);
                }
            }
        }
    }

    // Called by Backspace button
    public void OnBackspaceButtonClicked()
    {
        if (currentPhase == TurnPhase.FirstPlayerMelody || currentPhase == TurnPhase.SecondPlayerMelody)
        {
            if (currentMelodyNotes.Count > 0)
            {
                currentMelodyNotes.RemoveAt(currentMelodyNotes.Count - 1);

                // Update icon preview
                bool isPlayer1 = IsCurrentPlayerOne();
                uiManager.UpdateMelodyPreview(currentMelodyNotes, isPlayer1);

                // Hide confirm button if melody is no longer complete
                if (currentMelodyNotes.Count < MELODY_SIZE)
                {
                    confirmButton.SetActive(false);
                }

                // Hide playback if no notes
                if (currentMelodyNotes.Count == 0)
                {
                    playbackButton.SetActive(false);
                }
            }
        }
    }

    // Called by Playback button - plays current melody
    public void OnPlaybackButtonClicked()
    {
        if (currentMelodyNotes.Count == 0)
            return;

        PlayerUI currentPlayer = GetCurrentPlayer();

        if (currentPlayer != null && uiManager != null)
        {
            // Convert List to array for PlayMelody
            string[] melodyNotes = new string[currentMelodyNotes.Count];
            for (int i = 0; i < currentMelodyNotes.Count; i++)
            {
                melodyNotes[i] = currentMelodyNotes[i].ToString();
            }

            uiManager.PlayMelody(melodyNotes, currentPlayer);
        }
    }

    // Called by Confirm button
    public void OnConfirmButtonClicked()
    {
        if (currentMelodyNotes.Count != MELODY_SIZE)
            return;

        switch (currentPhase)
        {
            case TurnPhase.FirstPlayerMelody:
                // First player confirms their melody
                if (player1GoesFirst)
                {
                    // Player 1 is going first
                    player1Action = new Action(
                        new Melody(currentMelodyNotes.ToArray()),
                        bs.player1
                    );
                }
                else
                {
                    // Player 2 is going first
                    player2Action = new Action(
                        new Melody(currentMelodyNotes.ToArray()),
                        bs.player2
                    );
                }

                // Clear preview icons
                uiManager.ClearAllPreviewIcons();

                // Move to second player's turn
                currentPhase = TurnPhase.SecondPlayerMelody;
                currentMelodyNotes.Clear();
                UpdateTurnIndicator();
                ShowMelodyButtons();
                break;

            case TurnPhase.SecondPlayerMelody:
                // Second player confirms their melody
                if (player1GoesFirst)
                {
                    // Player 2 is going second
                    player2Action = new Action(
                        new Melody(currentMelodyNotes.ToArray()),
                        bs.player2
                    );
                }
                else
                {
                    // Player 1 is going second
                    player1Action = new Action(
                        new Melody(currentMelodyNotes.ToArray()),
                        bs.player1
                    );
                }

                // Execute battle
                ResolveBattle();
                break;
        }
    }

    private void ResolveBattle()
    {
        currentPhase = TurnPhase.BattleResolution;

        // Hide input buttons during battle animation
        noteButtons.SetActive(false);
        confirmButton.SetActive(false);
        if (backspaceButton != null)
            backspaceButton.SetActive(false);
        if (playbackButton != null)
            playbackButton.SetActive(false);

        // Always pass actions as (player1Action, player2Action) to battle logic
        bs.battle(player1Action, player2Action);

        // Subscribe to animation complete event
        if (uiManager != null)
        {
            uiManager.onBattleAnimationComplete = OnBattleAnimationComplete;
            uiManager.DisplayBattleResults(bs.last_update, player1Action, player2Action, player1GoesFirst);
        }
    }

    // NEW: Called when UIManager finishes all animations
    private void OnBattleAnimationComplete()
    {
        // Unsubscribe from event
        if (uiManager != null)
        {
            uiManager.onBattleAnimationComplete = null;
        }

        // Check for game over
        if (bs.player1.health <= 0 || bs.player2.health <= 0)
        {
            HandleGameOver();
        }
        else
        {
            StartNextTurn();
        }
    }

    private void StartNextTurn()
    {
        // Toggle who goes first for the next round
        player1GoesFirst = !player1GoesFirst;

        // Always start at first player phase
        currentPhase = TurnPhase.FirstPlayerMelody;

        currentMelodyNotes.Clear();
        UpdateTurnIndicator();
        ShowMelodyButtons();

        // Make sure all icons are cleared for new turn
        uiManager.ClearAllPreviewIcons();
    }

    private void HandleGameOver()
    {
        noteButtons.SetActive(false);
        confirmButton.SetActive(false);
        if (backspaceButton != null)
            backspaceButton.SetActive(false);
        if (playbackButton != null)
            playbackButton.SetActive(false);

        if (bs.player1.health <= 0 && bs.player2.health <= 0)
        {
            turnIndicatorText.text = "Draw!";
        }
        else if (bs.player1.health <= 0)
        {
            turnIndicatorText.text = "Player 2 Wins!";
            SceneManager.LoadScene("P2Wins");
        }
        else
        {
            turnIndicatorText.text = "Player 1 Wins!";
            SceneManager.LoadScene("P1Wins");
        }
    }

    private void ShowMelodyButtons()
    {

        if (bgMusic != null)
            bgMusic.OnTurnPhaseStart();

        noteButtons.SetActive(true);
        confirmButton.SetActive(false);
        if (backspaceButton != null)
            backspaceButton.SetActive(true);
        if (playbackButton != null)
            playbackButton.SetActive(false);
    }

    private void UpdateTurnIndicator()
    {
        if (turnIndicatorText == null) return;

        switch (currentPhase)
        {
            case TurnPhase.FirstPlayerMelody:
                if (player1GoesFirst)
                    turnIndicatorText.text = "Player 1: Select Melody (4 notes)";
                else
                    turnIndicatorText.text = "Player 2: Select Melody (4 notes)";
                break;

            case TurnPhase.SecondPlayerMelody:
                if (player1GoesFirst)
                    turnIndicatorText.text = "Player 2: Select Melody (4 notes)";
                else
                    turnIndicatorText.text = "Player 1: Select Melody (4 notes)";
                break;

            case TurnPhase.BattleResolution:
                turnIndicatorText.text = "Battle Resolution...";
                break;
        }
    }
}