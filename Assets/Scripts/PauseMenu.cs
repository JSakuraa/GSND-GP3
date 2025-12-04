using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using System.Collections.Generic;

public class PauseMenu : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pauseMenuPanel;
    public GameObject rulesPanel;

    public UnityEngine.UI.Slider volumeSlider;

    private bool isPaused = false;
    private const string VOLUME_PREF_KEY = "MasterVolume";

    void Start()
    {
        // Make sure menus are hidden at start
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);
        if (rulesPanel != null)
            rulesPanel.SetActive(false);
    }

    void Update()
    {
        var keyboard = UnityEngine.InputSystem.Keyboard.current;

        if (keyboard == null) return;
        // Toggle pause with Escape key
        if (keyboard.escapeKey.wasPressedThisFrame)
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Pause()
    {
        isPaused = true;
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f; // Freeze game
    }

    public void Resume()
    {
        isPaused = false;
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);
        if (rulesPanel != null)
            rulesPanel.SetActive(false);
        Time.timeScale = 1f; // Unfreeze game
    }

    public void ShowRules()
    {
        if (rulesPanel != null)
        {
            rulesPanel.SetActive(true);
            if (pauseMenuPanel != null)
                pauseMenuPanel.SetActive(false);
        }
    }

    public void HideRules()
    {
        if (rulesPanel != null)
        {
            rulesPanel.SetActive(false);
            if (pauseMenuPanel != null)
                pauseMenuPanel.SetActive(true);
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // Unfreeze before reloading
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f; // Unfreeze before changing scenes
        SceneManager.LoadScene("MainMenu"); // Change to your main menu scene name
    }

    public void SetVolume(float volume)
    {
        // Set the AudioListener volume (affects all audio)
        AudioListener.volume = volume;

        // Save the volume preference
        PlayerPrefs.SetFloat(VOLUME_PREF_KEY, volume);
        PlayerPrefs.Save();

        Debug.Log($"Volume set to: {volume}");
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}