using UnityEngine;
using MusicDefinitions;
using BattleFefinitions;

public class Runner : MonoBehaviour
{
    [ContextMenu("Run test code")]
    void quickrun()
    {
        Debug.Log("Running test code");
        Action a1 = new Action(new Chord(0, 1, 3), new Melody(3, 3, 3, 3));
        Action a2 = new Action(new Chord(0, 1, 3), new Melody(6, 6, 6, 6));
        Player p1 = new Killer("Alice");
        Player p2 = new Killer("Alex");
        p1.enemy = p2;
        p2.enemy = p1;
        a1.player = p1;
        a2.player = p2;
        Battlestate.battle(a1, a2);
    }
}
