using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBarUI : MonoBehaviour
{
    [Header("Player 1 Health Bar")]
    public Image player1HealthFill;
    public TextMeshProUGUI player1HealthText;

    [Header("Player 2 Health Bar")]
    public Image player2HealthFill;
    public TextMeshProUGUI player2HealthText;

    [Header("Settings")]
    public double maxHealth = 8.0;

    private Battlestate battlestate;

    void Start()
    {
        battlestate = GetComponent<Battlestate>();
        if (battlestate == null)
        {
            battlestate = FindObjectOfType<Battlestate>();
        }

        // Initialize health bars
        UpdateHealthBars();
    }

    void Update()
    {
        // Update health bars every frame to catch any changes
        UpdateHealthBars();
    }

    public void UpdateHealthBars()
    {
        if (battlestate == null) return;

        // Get Player 1 health
        double p1Health = maxHealth;
        string p1Name = "Player 1";

        if (battlestate.player1 != null)
        {
            p1Health = battlestate.player1.health;
            p1Name = battlestate.player1.name;
        }
        else
        {
            // Fallback to inspector values if player objects aren't created yet
            p1Health = battlestate.player1_health;
        }

        // Update Player 1 UI
        float healthPercent1 = (float)(p1Health / maxHealth);
        if (player1HealthFill != null)
        {
            player1HealthFill.fillAmount = Mathf.Clamp01(healthPercent1);
        }

        if (player1HealthText != null)
        {
            player1HealthText.text = $"{p1Name}: {p1Health:F1}/{maxHealth}";
        }

        // Get Player 2 health
        double p2Health = maxHealth;
        string p2Name = "Player 2";

        if (battlestate.player2 != null)
        {
            p2Health = battlestate.player2.health;
            p2Name = battlestate.player2.name;
        }
        else
        {
            // Fallback to inspector values if player objects aren't created yet
            p2Health = battlestate.player2_health;
        }

        // Update Player 2 UI
        float healthPercent2 = (float)(p2Health / maxHealth);
        if (player2HealthFill != null)
        {
            player2HealthFill.fillAmount = Mathf.Clamp01(healthPercent2);
        }

        if (player2HealthText != null)
        {
            player2HealthText.text = $"{p2Name}: {p2Health:F1}/{maxHealth}";
        }
    }
}