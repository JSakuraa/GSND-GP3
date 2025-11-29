using UnityEngine;
using MusicDefinitions;
using BattleDefinitions;
using System.Collections.Generic;

public class PlayerWraper : MonoBehaviour
{
    public Player player;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
public class Player
{
    public double max_health = 100;
    public double health = 100;
    public List<PlayerEffect> effects;
    public Player enemy;
    public string type;
    public string name="None";
    public Monster monster;

    public double damage_power = 10;
    public double heal_power = 8;
    public double life_steal_power = 6;


    public double damage_mult = 1;

    public double heal_mult=1;

    public double damage_mult_mod=1;

    public double heal_mult_mod=1;

    public double[] health_change = { 0, 0, 0, 0 };

    public void applyHealthChange()
    {
        apply_mult();
        for (int j = 0; j < health_change.Length; j++)
        {
            if (health + health_change[j] >= max_health)
            {
                health = max_health;
            }
            else if (health + health_change[j] <= 0)
            {
                health = 0;
            }
            else
            {
                health += health_change[j];
            }
            //health_change[j] = 0;
        }
    }

    public void getHealed(double power)
    {
        changeHealth(power * heal_mult);
    }

    public void getDamaged(double power)
    {
        changeHealth(-power * damage_mult);
    }
    public void changeHealth(double hpchange)
    {
        if (health + hpchange >= max_health)
        {
            health = max_health;
        }
        else if (health + hpchange <= 0)
        {
            health = 0;
        }
        else
        {
            health += hpchange;
        }
    }

    void apply_mult()
    {
        for (int j = 0; j < health_change.Length; j++)
        {
            if (health_change[j] > 0)
            {
                health_change[j] *= heal_mult * heal_mult_mod;
            }
            if (health_change[j] < 0)
            {
                health_change[j] *= damage_mult * damage_mult_mod;
            }
        }
        damage_mult_mod = 1;
        heal_mult_mod = 1;
        Debug.Log("post apply mult: "+info());
    }

    public string info()
    {
        return $"PLayer {name} of type {type}, health: {health}, monster {monster} health change: {Actionresolution.GenerateArrayDefinitionString1D(health_change)}\n" +
        $"Damage {damage_power}, Heal {heal_power}, Life steal {life_steal_power}";
    }

}

public class Monster
{
    public string name;
    public SpecialMelody[] combos;

    public SpecialMelody findFromMelody(Melody m)
    {
        return SpecialMelody.findFromMelody(combos, m);
    }
}

public class Unseen : Monster
{
    public Unseen()
    {
        name = "the Unseen";
        combos = new SpecialMelody[] { new Execute(), new MinorHeal(), new MinorAmplify() };
    }
    public override string ToString()
        {
            return $"Monster: {name}";
        }
}

public class Killer : Player
{
    public Killer()
    {
        type = "The Killer";
        monster = new Unseen();
    }
    public Killer(string n)
    {
        name = n;
        type = "The Killer";
        monster = new Unseen();
    }
    public override string ToString()
    {
        return $"Player: {type}";
    }
}