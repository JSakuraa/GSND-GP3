using BattleDefinitions;
using MusicDefinitions;
using UnityEngine;
public class Battlestate : MonoBehaviour
{

    [Header("Player 1 starting HP")]
    [Tooltip("Player 1 starting HP")]
    public double player1_health = 100;
    [Header("Player 2 starting HP")]
    [Tooltip("Player 2 starting HP")]
    public double player2_health = 100;
    [Header("Player 1")]
    [Tooltip("The Player 1 Player object")]
    public Player player1;
    [Header("Player 2")]
    [Tooltip("The Player 2 Player object")]
    public Player player2;

    public update_info last_update; // contains all info on the battle ourcome for UI, print(last_update) for text explanation.

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        init();

    }
    public void init()
    {
        Debug.Log("Start battle state");
        player1 = new Killer("Alice");
        player2 = new Killer("Alex");
        player1.health = player1_health;
        player2.health = player2_health;
        player1.enemy = player2;
        player2.enemy = player1;
        last_update = new update_info();
        last_update.player1 = player1;
        last_update.player2 = player2;   
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void applyMelodyEffect(Action a1, Action a2, double[] outputs)
    {

        double potency = 0;
        Action winner = a1;
        potency = getPotency(outputs);
        if (potency == 0)
        {
            Debug.Log("Tie, no melody resolved");
            return;
        }
        last_update.winning_melody_effect = null;

        if (potency <= 0)
        {
            winner = a2;
            potency = -potency;
        }
        SpecialMelody effect = winner.player.monster.findFromMelody(winner.melody);
        if (effect != null)
        {
            effect.effect.apply(winner.player, winner.player.enemy, potency);
            last_update.winning_melody_effect = effect.effect;
        }
        last_update.melody_winner = winner.player;
        
    }
    public double getPotency(double[] outputs)
    {
        double potency = 0;
        for (int j = 0; j < outputs.Length; j++)
        {
            potency += outputs[j];
        }
        return potency;
    }

    public void computeHealthChange(Action a1, Action a2, double[] outputs)
    {
        NoteEffect[] neffects = new NoteEffect[outputs.Length];
        Player[] winners = new Player[outputs.Length];
        for (int j = 0; j < outputs.Length; j++)
        {
            (winners[j], neffects[j]) = decideWiningNoteHealthChange(a1, a2, j, outputs);
        }
        last_update.Note_winners = winners;
        last_update.Winning_note_effects = neffects;

    }
    public (Player, NoteEffect) decideWiningNoteHealthChange(Action a1, Action a2, int j, double[] outputs)
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
            winner.player.health_change[j] = 2 * potency*winner.player.heal_power;
            winner.player.enemy.health_change[j] = 0;

        }
        if (neffect == NoteEffect.LifeSteal)
        {
            winner.player.health_change[j] = potency*winner.player.life_steal_power;
            winner.player.enemy.health_change[j] = -potency*winner.player.life_steal_power;

        }
        //adjusted scalar for damage to be 3 per win
        if (neffect == NoteEffect.Damage)
        {
            winner.player.health_change[j] = 0;
            winner.player.enemy.health_change[j] = -3 * potency*winner.player.damage_power;

        }
        return (winner.player, neffect);
    }
    public void battle(Action a1, Action a2)
    {
        Debug.Log($"Player 1 is {a1.player.name} and chose action {a1} ");
        Debug.Log($"Player 2 is {a2.player.name} and chose action {a2} ");
        double[] outputs = Actionresolution.resolve_actions(a1, a2);
        Debug.Log($"action outcomes: {Actionresolution.GenerateArrayDefinitionString1D(outputs)}");
        computeHealthChange(a1, a2, outputs);
        applyMelodyEffect(a1, a2, outputs);
        a1.player.applyHealthChange();
        a2.player.applyHealthChange();
        a1.player.applyPlayerEffects();
        a2.player.applyPlayerEffects();
        last_update.p1hp_change = a1.player.health_change;
        last_update.p2hp_change = a2.player.health_change;
        Debug.Log(a1.player.info());
        Debug.Log(a2.player.info());
    }

    public void lastBattleOutcome()
    {
        
    }
}

public class update_info
{
    public Player melody_winner;
    public MelodyEffect winning_melody_effect;
    public Player[] Note_winners;
    public NoteEffect[] Winning_note_effects;
    public double[] p1hp_change;
    public double[] p2hp_change;

    public Player player1;
    public Player player2;

    public override string ToString()
    {
        string s = "none";
        if (winning_melody_effect != null) {
            s = $"{winning_melody_effect}";
        }
        return $"The last battle had the following results:\n" +
        $" the first note was won by : {Note_winners[0].name}, had the effect: {Winning_note_effects[0]} and resulted in the following health poiint changes: {player1.name} : {player1.health_change[0]} and {player2.name} : {player2.health_change[0]}.\n" +
        $" the second note was won by : {Note_winners[1].name}, had the effect: {Winning_note_effects[1]} and resulted in the following health poiint changes: {player1.name} : {player1.health_change[1]} and {player2.name} : {player2.health_change[1]}.\n" +
        $" the third note was won by : {Note_winners[2].name}, had the effect: {Winning_note_effects[2]} and resulted in the following health poiint changes: {player1.name} : {player1.health_change[2]} and {player2.name} : {player2.health_change[2]}.\n" +
        $" the third note was won by : {Note_winners[3].name}, had the effect: {Winning_note_effects[3]} and resulted in the following health poiint changes: {player1.name} : {player1.health_change[3]} and {player2.name} : {player2.health_change[3]}.\n" +
        $"The overall melody winner was {melody_winner.name} and the effect of their melody was {s}";
    }
}
