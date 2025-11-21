# Deadly Duet - Quick Reference

## Combat Flow
```
InputReader.OnButtonClick() 
  → Battlestate.battle(a1, a2)
    → Action_resolution.resolve_actions() → outputs[4]
    → Battlestate.computeHealthChange() → Player.health_change[4]
    → Player.applyHealthChange() → update HP
```

## File Map

**Definitions.cs** - Data structures (MusicalNote, Chord, Melody, Action)  
**Action_resolution.cs** - RPS combat logic, note resolution  
**Battlestate.cs** - Combat orchestrator, health calculation  
**Player.cs** - Player state, Monster/combo system  
**InputReader.cs** - UI → combat bridge  
**SceneController.cs** - Scene transitions  
**PitchChanger.cs** - Audio (not integrated yet)

## Key Data

**note_effects[7]** (Action_resolution.cs):
```
C=Damage, D=Heal, E=LifeSteal, F=Damage, G=Heal, A=LifeSteal, B=Heal
```

**base_resolution_matrix[7,7]** (Action_resolution.cs):
- Tonic (C, E, A) beats Dominant (G, B)
- Dominant (G, B) beats Subdominant (D, F)  
- Subdominant (D, F) beats Tonic (C, E, A)
- Row=P1 note, Col=P2 note, Value: 1=P1 wins, -1=P2 wins, 0=tie

## Core Functions

**Battlestate.battle(a1, a2)** - Main entry point  
**Action_resolution.resolve_actions(a1, a2)** - Returns outputs[4]  
**Battlestate.decideWiningNoteHealthChange()** - Applies Heal/Damage/LifeSteal  
**Player.applyHealthChange()** - Updates HP from health_change[4]

## TODO

### UI & Gameplay components 
- Health bars
- Character art placeholder section
- Note input section
- End of turn phrase playback

### Mechanics updates
- Verify Tonic/Subdominant/Dominant values match design doc
- Verify note pair matrix changes
- Set HP and other combat scalars to be accurate
- Add a game end state (HP <= 0)
- Add in proper character classes and combos 