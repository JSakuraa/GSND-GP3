A “phrase” is a full 8 round melody with 4 note pairs, with each player playing 4 notes and 1 chord.
Winning a note pair means winning the Rock Paper Scissors round for the note pair based on the 3 groups.

Assume all notes are in the standard C major scale with no sharps or flats.

Mechanics to implement in full:

Each player has 8 max health points
Tonic notes heal the player 2 points on win, Dominant notes damage the enemy 3 points on win, Subdominant notes steal 1 HP on win.
Tonic, Dominant, and subdominant have a rock paper scissors relation where Tonic -> Dominant -> Subominant -> Tonic 
Chords give notes in them a 50% chance to win in a tie, where chords are guaranteed to be regular triads.

E.g. 
P1: C D E A, Cmaj 
vs
P2 : C B D G, Gmaj 
Produces

C v C (EV 0.5 for P1), D vs B (P2 deals 2 dmg), E vs D (P2 steals 1 HP), A vs G (P1 Heals 2).

The net point differential for the turn is:

P1 heals 2 or 4 HP
P2 steals 1 hp and deals 2 more dmg

A “combo” is 3 notes that are present in a player’s 4 note melody regardless of position or order. The 2 characters get specific combo profiles. 

Character 1 (Control player, Classical musician):

C F G = Peaceful combo (one of each cat, major) -> Halve intensity of all effects for this phrase 

D A B = Angsty combo -> Double intensity of all effects for this phrase

C E A = Defender’s combo -> All healing effects are doubled in the next phrase

Character 2 (Risk taker, the jazz musician):

D F B = Unstable combo -> Flip all heal and damage effects for the phrase

F A B  = Gambler’s combo -> Whoever won more total note pairs for the phrase, double all of the effects of their played notes.

G D B =  Attacker’s combo -> All damaging effects are doubled in the next phrase
