using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [Header("Player Info")]
    public string playerName;
    public int currentHealth;
    public int maxHealth = 100;

    [Header("Visual - Musician")]
    public Image playerSprite;  // The human musician

    [Header("Instrument")]
    public Instrument instrument;  // Reference to their monster/instrument
}