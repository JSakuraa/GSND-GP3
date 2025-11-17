using MusicDefinitions;
using UnityEngine;


namespace MusicDefinitions
{
    public enum MusicalNote
    {
        G = 0, B = 1, D = 2, F = 3, C = 4, E = 5, A = 6
    }
    public class Chord
    {
        public MusicalNote[] notes { get; set; }
        public Chord(MusicalNote n1, MusicalNote n2, MusicalNote n3)
        {
            notes = new MusicalNote[3];
            notes[0] = n1;
            notes[1] = n2;
            notes[2] = n3;
        }
        public Chord(MusicalNote[] n)
        {
            notes = new MusicalNote[3];
            notes[0] = n[0];
            notes[1] = n[1];
            notes[2] = n[2];
        }
        public override string ToString()
        {
            return $"Chord notes: {notes[0]}, {notes[1]}, {notes[2]}";
        }
    }

    public class Melody
    {
        public MusicalNote[] notes { get; set; }
        public Melody(MusicalNote n1, MusicalNote n2, MusicalNote n3, MusicalNote n4)
        {
            notes = new MusicalNote[4];
            notes[0] = n1;
            notes[1] = n2;
            notes[2] = n3;
            notes[3] = n4;
        }
        public Melody(MusicalNote[] n)
        {
            notes = new MusicalNote[4];
            notes[0] = n[0];
            notes[1] = n[1];
            notes[2] = n[2];
            notes[3] = n[3];
        }
        public override string ToString()
        {
            return $"Melody notes: {notes[0]}, {notes[1]}, {notes[2]}, {notes[3]}";
        }
    }

    public class Action
    {
        public Chord chord { get; set; }
        public Melody melody { get; set; }
        public Action(Chord c, Melody m)
        {
            chord = c;
            melody = m;
        }
        public override string ToString()
        {
            return $"Chord: {chord}, Melody: {melody}";
        }
    }



}

public class Translations: MonoBehaviour
{
    public static MusicalNote note_from_char(char s)
    {
        MusicalNote note = (MusicalNote)System.Enum.Parse(typeof(MusicalNote), s.ToString());
        return note;

    }
    public static MusicalNote[] notes_from_string(string s)
    {
        MusicalNote[] notes = new MusicalNote[s.Length];
        int i = 0;
        foreach (char character in s)
        {
            notes[i] = note_from_char(character);
            i++;
            }
        return notes;
    }
}