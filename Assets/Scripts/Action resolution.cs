using UnityEngine;
using MusicDefinitions;


public class Actionresolution : MonoBehaviour
{
    [Header("Player 1 health")]
    [Tooltip("Player 1 health")]
    public int player1_health = 10;
    [Header("Player 2 health")]
    [Tooltip("Player 2 health")]
    public int player2_health = 10;

    static double[,] base_resolution_matrix = {
    {  0,  0,  1,  1, -1, -1, -1},
    {  0,  0,  1,  1, -1, -1, -1},
    { -1, -1,  0,  0,  1,  1,  1},
    { -1, -1,  0,  0,  1,  1,  1},
    {  1,  1, -1, -1,  0,  0,  0},
    {  1,  1, -1, -1,  0,  0,  0},
    {  1,  1, -1, -1,  0,  0,  0}
    };

    public void act()

    {
        Chord c = new Chord(MusicalNote.A, MusicalNote.B, MusicalNote.C);

        Melody m = new Melody(MusicalNote.A, MusicalNote.B, MusicalNote.C, MusicalNote.D);
        print("haha!");
        print(c);
        print(m);
        Action a = new Action(c, m);
        c.notes[2] = MusicalNote.F;
        print(a);
        MusicalNote[] xxx= { MusicalNote.A, MusicalNote.F, MusicalNote.D };
        Chord c2 = new Chord(xxx);
        MusicalNote[] xxx2= { MusicalNote.G, MusicalNote.A, MusicalNote.C, MusicalNote.A };
        Melody m2 = new Melody(xxx2);
        Action a2 = new Action(c2, m2);
        print(a2);
        print($"{a.melody.notes[1]} and {a2.melody.notes[1]} resolve to {base_resolution_matrix[(int)a.melody.notes[1], (int)a2.melody.notes[1]]}");
        string enumString = "A";
        MusicalNote note = (MusicalNote)System.Enum.Parse(typeof(MusicalNote), enumString);
        print(MusicalNote.A == note);
        resolve_actions(a, a2);
    }

    public static double[] resolve_actions(Action a1, Action a2)
    {
        double[,] temp_res_matrix = base_resolution_matrix.Clone() as double[,];
        /*
        temp_res_matrix[0, 0] = 1000;
        print(GenerateArrayDefinitionString(base_resolution_matrix));
        print(GenerateArrayDefinitionString(temp_res_matrix));
        print(GenerateArrayDefinitionString1D(resolve_melodies(a1.melody, a2.melody, temp_res_matrix)));
        temp_res_matrix[0, 0] = 0;
        print("Player 1 action:");
        print(a1);
        print("Player 2 action:");
        print(a2);
        print("base interactions:");
        print(GenerateArrayDefinitionString(temp_res_matrix));
        print("Applying chord effects");
        */
        chord_effects(a1.chord, a2.chord, temp_res_matrix);
        //print(GenerateArrayDefinitionString(temp_res_matrix));
        double[] outputs = resolve_melodies(a1.melody, a2.melody, temp_res_matrix);
        //print("melody battle outputs");
        //print(GenerateArrayDefinitionString1D(outputs));
        return outputs;


    }
    public static double resolve_notes(MusicalNote n1, MusicalNote n2, double[,] matrix)
    {
        return matrix[(int)n1, (int)n2];
    }

    public static double[] resolve_melodies(Melody m1, Melody m2, double[,] matrix)
    {
        double[] outcomes = new double[m1.notes.Length];
        for (int i = 0; i < m1.notes.Length; i++)
        {
            outcomes[i] = matrix[(int)m1.notes[i], (int)m2.notes[i]];
        }
        return outcomes;
    }
    public static void chord_effects(Chord c1, Chord c2, double[,] matrix)
    {
        for (int i = 0; i < c1.notes.Length; i++)
        {
            for (int j = 0; j < matrix.GetLength(0); j++)
            {
                matrix[(int)c1.notes[i], j] += 0.5;
                matrix[j, (int)c2.notes[i]] -= 0.5;

            }

        }
    }

    public static string GenerateArrayDefinitionString<T>(T[,] matrix) //thank you Google AI
    {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);
        var sb = new System.Text.StringBuilder();

        sb.Append("{"); // Outer opening brace for the whole array

        for (int i = 0; i < rows; i++)
        {
            sb.Append("{"); // Inner opening brace for the row

            for (int j = 0; j < cols; j++)
            {
                sb.Append(matrix[i, j]);
                // Add a comma if it's not the last element in the row
                if (j < cols - 1)
                {
                    sb.Append(", ");
                }
            }

            sb.Append("}"); // Inner closing brace for the row

            // Add a comma and a newline if it's not the last row
            if (i < rows - 1)
            {
                sb.Append(",\n ");
            }
        }

        sb.Append("}"); // Outer closing brace

        return sb.ToString();
    }
    
public static string GenerateArrayDefinitionString1D<T>(T[] array)//thank you Google AI
    {
        if (array == null)
        {
            return "null";
        }

        // Use String.Join to efficiently concatenate all elements with a comma and space
        string joinedElements = System.String.Join(", ", array);

        // Wrap the joined elements in curly braces
        return $"{{{joinedElements}}}";
    }





}
