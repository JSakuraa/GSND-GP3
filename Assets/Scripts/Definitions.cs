using BattleDefinitions;
using MusicDefinitions;
using UnityEngine;


namespace MusicDefinitions
{

    public enum MusicalNote
    {

        //Notes did not correspond to basic C major scale, fixed.

             C = 0, D = 1, E = 2, F = 3, G = 4, A = 5, B = 6
    }
    public enum NoteEffect
    {
        Heal = 0, LifeSteal= 1, Damage=2
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

namespace BattleDefinitions
{

    public class Effect
    {
        public string name;

        public Player subj;
        public Player obj;

        public double power;

        public virtual void apply()
        {
            Debug.Log($"applying effect {name} on self {subj.name} and enemy {obj.name} with potency {power}");
        }
        public virtual void apply(Player self, Player enemy, double potency)
        {
            Debug.Log($"applying effect {name} on self {self.name} and enemy {enemy.name} with potency {potency}");
        }
        public override string ToString()
        {
            return $"Effect: {name}";
        }
    }
    public class HealEffect : Effect
    {
        public HealEffect()
        {
            name = "base Heal";
            power = 10;
        }
        public HealEffect(Player self, Player enemy, double p)
        {
            name = "base Heal";
            power = p;
            subj = self;
            obj = enemy;
        }
        public override void apply()
        {
            base.apply();
            subj.getHealed(power);
        }

    }
    public class DamageEffect : Effect
    {
        public DamageEffect()
        {
            name = "base Damage";
            power = 10;
        }
        public DamageEffect(Player self, Player enemy, double p)
        {
            name = "base Damge";
            power = p;
            subj = self;
            obj = enemy;
        }
        public override void apply()
        {
            base.apply();
            obj.getDamaged(power);
        }

    }
    public class LifeStealEffect : Effect
    {
        public LifeStealEffect()
        {
            name = "base Life Steal";
            power = 10;
        }
        public LifeStealEffect(Player self, Player enemy, double p)
        {
            name = "base Life Steal";
            power = p;
            subj = self;
            obj = enemy;
        }
        public override void apply()
        {
            base.apply();
            subj.getHealed(power);
            obj.getDamaged(power);
        }

    }

    public class MelodyEffect : Effect
    {
        public override void apply(Player self, Player enemy, double potency)
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
        double min_potency;
        double low_hp;
        public TouchOfDeath()
        {
            min_potency = 0;
            low_hp = 100;
            name = "Touch of Death";
        }
        public TouchOfDeath(double mp, double lhp)
        {
            min_potency = mp;
            low_hp = lhp;
            name = "Touch of Death";
        }
        public override void apply(Player self, Player enemy, double potency)
        {
            base.apply(self, enemy, potency);
            if ((potency >= min_potency) && (self.health <= low_hp))
            {
                enemy.health = 0;
                Debug.Log($"Touch of death kills {enemy.name}");
            }

        }
    }

    public class HealingSong : MelodyEffect
    {
        double min_potency;
        double heal_hp;
        public HealingSong()
        {
            min_potency = 0;
            heal_hp = 100;
            name = "Healing Song";
        }
        public HealingSong(double mp, double lhp)
        {
            min_potency = mp;
            heal_hp = lhp;
            name = "Healing Song";
        }
        public override void apply(Player self, Player enemy, double potency)
        {
            base.apply(self, enemy, potency);
            if ((potency >= min_potency))
            {
                self.changeHealth(heal_hp * potency);
                Debug.Log($"Player {self.name} heals from song: {heal_hp * potency}");
            }

        }
    }

    public class DamagingSong : MelodyEffect
    {
        double min_potency;
        double damage_hp;
        public DamagingSong()
        {
            min_potency = 0;
            damage_hp = 10;
            name = "Damaging Song";
        }
        public DamagingSong(double mp, double lhp)
        {
            min_potency = mp;
            damage_hp = lhp;
            name = "Damaging Song";
        }
        public override void apply(Player self, Player enemy, double potency)
        {
            base.apply(self, enemy, potency);
            if ((potency >= min_potency))
            {
                enemy.changeHealth(-damage_hp * potency);
                Debug.Log($"Player {enemy.name} takes damage from song: {damage_hp * potency}");
            }

        }
    }

    public class Amplify : MelodyEffect
    {
        double min_potency;
        double amplify_mult;
        public Amplify()
        {
            min_potency = 0;
            amplify_mult = 2;
            name = "Amplify";
        }
        public Amplify(double mp, double lhp)
        {
            min_potency = mp;
            amplify_mult = lhp;
            name = "Amplify";
        }
        public override void apply(Player self, Player enemy, double potency)
        {
            base.apply(self, enemy, potency);
            if ((potency >= min_potency))
            {
                self.damage_power *= amplify_mult;
                self.heal_power *= amplify_mult;
                self.life_steal_power *= amplify_mult;
                Debug.Log($"Player {self.name} is amplified by: {amplify_mult}");
            }

        }
    }
    public class Condence : MelodyEffect
    {
        double min_potency;
        double amplify_mult;
        public Condence()
        {
            min_potency = 0;
            amplify_mult = 1 / 2;
            name = "Condence";
        }
        public Condence(double mp, double lhp)
        {
            min_potency = mp;
            amplify_mult = lhp;
            name = "Condence";
        }
        public override void apply(Player self, Player enemy, double potency)
        {
            base.apply(self, enemy, potency);
            if ((potency >= min_potency))
            {
                enemy.damage_power *= amplify_mult;
                enemy.heal_power *= amplify_mult;
                enemy.life_steal_power *= amplify_mult;
                Debug.Log($"Player {enemy.name} is condenced by: {amplify_mult}");
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
            SpecialMelody target = null;
            foreach (SpecialMelody sm in combos)
            {

                if (sm.matchMelody(m))
                {
                    target = sm;

                    Debug.Log($"found combo: {sm}");
                    return target; //return only first

                }

            }
            return target;
        }

        virtual public bool matchMelody(Melody m)
        {
            if (melody.extEquals(m))
            {
                Debug.Log($"found combo: {this}");
                return true;

            }
            return false;
        }


    }

    public class SpecialMelodyShort : SpecialMelody
    {
        //only first three notes of the melody must be played to activate the effects
        public override bool matchMelody(Melody m)
        {
            if (melody.notes[0] == m.notes[0] && melody.notes[1] == m.notes[1] && melody.notes[2] == m.notes[2])
            {
                return true;
            }
            if (melody.notes[0] == m.notes[1] && melody.notes[1] == m.notes[2] && melody.notes[2] == m.notes[3])
            {
                return true;
            }
            return false;
        }
    }

    public class Execute : SpecialMelody
    {
        public Execute() : base()
        {
            name = "Execute";
            melody = new Melody(3, 3, 3, 3);
            effect = new TouchOfDeath();

        }
    }

    public class ExecuteShort : SpecialMelodyShort
    {
        public ExecuteShort() : base()
        {
            name = "Execute short";
            melody = new Melody(3, 3, 3, 3);
            effect = new TouchOfDeath(2, 50);
        }
    }

    public class MinorHeal : SpecialMelodyShort
    {
        public MinorHeal() : base()
        {
            name = "Minor heal";
            melody = new Melody(0, 3, 4, 0);
            effect = new HealingSong(0, 25);
        }
    }
    
    public class MinorAmplify : SpecialMelodyShort
    {
        public MinorAmplify() : base()
        {
            name = "Minor amplify";
            melody = new Melody(1, 5, 6, 0);
            effect = new Amplify(0,3);
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