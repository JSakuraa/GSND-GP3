using UnityEngine;
using MusicDefinitions;
using TMPro; // For standard UI InputField
// using TMPro; // For TextMeshPro InputField

public class InputReader : MonoBehaviour
{
    public TMP_InputField chord1;
    public TMP_InputField melody1;
    public TMP_InputField chord2;
    public TMP_InputField melody2; // Drag your InputField here in the Inspector
    // OR
    // public TMP_InputField inputField; // If using TextMeshPro

    public TMPro.TMP_Text displayText; // Optional: To display the read text
    // OR
    // public TMPro.TMP_Text displayText; // If using TextMeshPro

    public void OnButtonClick()
    {
        string chord1_text = chord1.text;
        string chord2_text = chord2.text;
        Action a1 = new Action(new Chord(Translations.notes_from_string(chord1.text)), new Melody(Translations.notes_from_string(melody1.text)));
        Action a2 = new Action(new Chord(Translations.notes_from_string(chord2.text)), new Melody(Translations.notes_from_string(melody2.text)));
        print(a1);
        print(a2);
        Debug.Log("User entered: " + chord1_text);
        double[] outputs = Actionresolution.resolve_actions(a1, a2);
        string s = Actionresolution.GenerateArrayDefinitionString1D(outputs);
        if (displayText != null)
        {
            displayText.text = "Action resolution: " + s;
        }
    }
}
