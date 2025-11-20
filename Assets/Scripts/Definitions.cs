using BattleFefinitions;
using MusicDefinitions;
using UnityEngine;


namespace MusicDefinitions
{

    public enum MusicalNote
    {
        G = 0, B = 1, D = 2, F = 3, C = 4, E = 5, A = 6
    }
    public enum NoteEffect
    {
        Heal = 0, LifeSteal= 1,Damage=2
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
        public Chord(int n1, int n2, int n3)
        {
            notes = new MusicalNote[3];
            notes[0] = (MusicalNote)n1;
            notes[1] = (MusicalNote)n2;
            notes[2] = (MusicalNote)n3;
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
        public Melody(int n1, int n2, int n3, int n4)
        {
            notes = new MusicalNote[4];
            notes[0] = (MusicalNote)n1;
            notes[1] = (MusicalNote)n2;
            notes[2] = (MusicalNote)n3;
            notes[3] = (MusicalNote)n4;
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
        public bool extEquals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        Melody other = (Melody)obj;
        if (notes.Length != other.notes.Length)
            {
                return false;
        }
        for (int j = 0; j < notes.Length; j++)
            {
                if (notes[j] != other.notes[j])
                {
                    return false;
                }
            }
        return true;
    }
    }

    public class Action
    {
        public Player player;
        public Chord chord { get; set; }
        public Melody melody { get; set; }
        public Action(Chord c, Melody m)
        {
            chord = c;
            melody = m;
        }
        public Action(Chord c, Melody m, Player p)
        {
            player = p;
            chord = c;
            melody = m;
        }
        public override string ToString()
        {
            return $"Chord: {chord}, Melody: {melody}";
        }
    }



}

namespace BattleFefinitions
{
    public class MelodyEffect
    {
        public string name;
        public virtual void apply(Player self, Player enemy, double potency)
        {
            Debug.Log($"applying effect {name} on self {self.name} and enemy {enemy.name} with potency {potency}");
        }
        public override string ToString()
        {
            return $"Melody effect: {name}";
        }
    }
    public class TouchOfDeath : MelodyEffect
    {
        public TouchOfDeath()
        {
            name = "Touch of Death";
        }
        public override void apply(Player self, Player enemy, double potency)
        {
            base.apply(self, enemy, potency);
            if ((potency >= 0) && (self.health <= 100))
            {
                enemy.health = 0;
                Debug.Log($"Touch of death kills {enemy.name}");
            }

        }
    }
    public class PlayerEffect
    {
        public string name;
        Player carrier;
        public void apply(Player self)
        {
            return;
        }
        public override string ToString()
        {
            return $"Player effect: {name}";
        }
    }
    public class DoT : PlayerEffect
    {
        public DoT()
        {
            name = "Damage over time";
        }
        Player carrier;
        double potency = 1;
        new public void apply(Player self)
        {
            carrier.health -= potency;
        }
    }
    public class SpecialMelody
    {
        public Melody melody;
        public MelodyEffect effect;
        public string name;
        public SpecialMelody()
        {

        }
        public SpecialMelody(Melody m, MelodyEffect e)
        {
            melody = m;
            effect = e;
        }
        public override string ToString()
        {
            return $"Special Melody: {name}, melody: {melody}, melody effect:{effect}";
        }
        public static SpecialMelody findFromMelody(SpecialMelody[] combos, Melody m)
        {
            SpecialMelody target=null;
            foreach (SpecialMelody sm in combos)
            {
                if (sm.melody.extEquals(m))
                {
                    target = sm;
                    Debug.Log($"found combo: {sm}");
                
                }
            }
            return target;
        }
    }
    public class Execute : SpecialMelody
    {
        public Execute():base()
        {   
            name="Execute";
            melody  = new Melody(3, 3, 3, 3);
            effect  = new TouchOfDeath();
            
        }
    }
}


public class Translations : MonoBehaviour
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