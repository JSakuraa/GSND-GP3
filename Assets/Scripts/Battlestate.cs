using BattleDefinitions;
using MusicDefinitions;
using UnityEngine;
public class Battlestate : MonoBehaviour
{
    [Header("Player 1 health")]
    [Tooltip("The starting health points for player 1")]
    public double player1_health = 8;
    [Header("Player 2 health")]
    [Tooltip("The starting health points for player 2")]
    public double player2_health = 8;

    [Header("Player 1")]
    [Tooltip("The Player 1 Player object")]
    public Player player1;
    [Header("Player 2")]
    [Tooltip("The Player 2 Player object")]
    public Player player2;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Start battle state");
        player1 = new Killer("Alice");
        player2 = new Killer("Alex");
        player1.health = player1_health;
        player2.health = player2_health;
        player1.enemy = player2;
        player2.enemy = player1;

    }

    // Update is called once per frame
    void Update()
    {

    }
    public static void applyMelodyEffect(Action a1, Action a2, double[] outputs)
    {

        double potency = 0;
        Action winner = a1;
        potency = getPotency(outputs);

        if (potency <= 0)
        {
            winner = a2;
            potency = -potency;
        }
        SpecialMelody effect = winner.player.monster.findFromMelody(winner.melody);
        if (effect != null)
        {
            effect.effect.apply(winner.player, winner.player.enemy, potency);
        }
    }
    public static double getPotency(double[] outputs)
    {
        double potency = 0;
        for (int j = 0; j < outputs.Length; j++)
        {
            potency += outputs[j];
        }
        return potency;
    }

    public static void computeHealthChange(Action a1, Action a2, double[] outputs)
    {
        for (int j = 0; j < outputs.Length; j++)
        {
            decideWiningNoteHealthChange(a1, a2, j, outputs);
        }
    }
    public static void decideWiningNoteHealthChange(Action a1, Action a2, int j, double[] outputs)
    {
        Action winner = a1;
        double potency = outputs[j];
        if (potency < 0)
        {
            winner = a2;
            potency = -potency;
        }
        MusicalNote winNote = winner.melody.notes[j];
        NoteEffect neffect = Actionresolution.note_effects[(int)winNote];
        Debug.Log($"winner {winner.player.name} winning note: {winNote}, with effect {neffect} and potency {potency}");
        if (neffect == NoteEffect.Heal)
        {
            winner.player.health_change[j] = 2 * potency;
            winner.player.enemy.health_change[j] = 0;

        }
        if (neffect == NoteEffect.LifeSteal)
        {
            winner.player.health_change[j] = potency;
            winner.player.enemy.health_change[j] = -potency;

        }
        //adjusted scalar for damage to be 3 per win
        if (neffect == NoteEffect.Damage)
        {
            winner.player.health_change[j] = 0;
            winner.player.enemy.health_change[j] = -3 * potency;

        }
    }
    public static void battle(Action a1, Action a2)
    {
        Debug.Log($"Player 1 is {a1.player.name} and chose action {a1} ");
        Debug.Log($"Player 2 is {a2.player.name} and chose action {a2} ");
        double[] outputs = Actionresolution.resolve_actions(a1, a2);
        Debug.Log($"action outcomes: {Actionresolution.GenerateArrayDefinitionString1D(outputs)}");
        computeHealthChange(a1, a2, outputs);
        applyMelodyEffect(a1, a2, outputs);
        a1.player.applyHealthChange();
        a2.player.applyHealthChange();
        Debug.Log(a1.player.info());
        Debug.Log(a2.player.info());
    }
}
