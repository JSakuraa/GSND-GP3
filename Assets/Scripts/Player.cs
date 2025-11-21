using UnityEngine;
using MusicDefinitions;
using BattleDefinitions;


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
    public double max_health = 8;
    public double health = 8;
    public PlayerEffect[] effects;
    public Player enemy;
    public string type;
    public string name="None";
    public Monster monster;

    public double[] health_change = { 0, 0, 0, 0 };

    public void applyHealthChange()
    {
        for (int j = 0; j < health_change.Length; j++)
        {
            if (health + health_change[j] >= max_health)
            {

            }
            else if (health + health_change[j]<=0)
            {
                health = 0;
            }
            else {
                health += health_change[j];
            }
            //health_change[j] = 0;
        }
    }

    public string info()
    {
        return $"PLayer {name} of type {type}, health: {health}, monster {monster} health change: {Actionresolution.GenerateArrayDefinitionString1D(health_change)}";
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
        combos = new SpecialMelody[] { new Execute() };
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