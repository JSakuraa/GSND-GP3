using UnityEngine;
using UnityEngine.UI;

public class Instrument : MonoBehaviour
{
    [Header("Instrument Info")]
    public string instrumentName;

    [Header("Visual - Monster")]
    public Image instrumentSprite;  // The monster that is the instrument

    [Header("Note Audio Clips")]
    public AudioClip noteC;
    public AudioClip noteD;
    public AudioClip noteE;
    public AudioClip noteF;
    public AudioClip noteG;
    public AudioClip noteA;
    public AudioClip noteB;

    public AudioClip GetNoteClip(string noteName)
    {
        switch (noteName.ToUpper())
        {
            case "C": return noteC;
            case "D": return noteD;
            case "E": return noteE;
            case "F": return noteF;
            case "G": return noteG;
            case "A": return noteA;
            case "B": return noteB;
            default:
                Debug.LogWarning($"Unknown note: {noteName}");
                return null;
        }
    }
}