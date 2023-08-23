# Debug info

## Running builds

Builds are stored in Assets folder just to simplify storing them with git.
If you are using copy of a repo, they should be copied outside the folder.
Otherwise, just download the .zip file you need.

The folder contains build for Windows and build for Linux:

- unzip coresponding build
- on Windows, run Zanzibar.exe
- on Linux, run LinuxBuild.x86_64

## Starting a game

Choose any number of players (up to 6),
fill their names (only non-empty names will be counted)
and hit New Game button.

If you want to continue last game, hit Continue instead.
Please note that starting a new game erases all previously stored game data.
That means only one game can be player at a time.

## Interaction

Play using:

- mouse (clicking nodes and main menu buttons)
  - click on figure activates it (it gets bigger)
  - next click on the same figure deactivates it
    - if it made some steps, it is now considered moved
    and cannot be moved again this turn
    - if it didn't move,
    it is not considered as moved and can be activated again
  - click on neighbouring node moves active figure there
    - if there is other figure and a decision is needed to be made,
    the other figure gets bigger
    and waits for clicking the new node to be moved to
- WASD / arrows (movement + rotation)
- Q / E (down / up)
- R to return to the center of map
- 1 / 2 / 3 (alphanumeric) to show quests, while holding:
  - Enter to activate shown quest
  - Shift + N to replace shown quest by a random new quest
- Shift + N to get random new quest (without holding 1 / 2 / 3)
- Space + Enter to skip remaining time of turn
- Escape to return to main menu

## Rules

- the goal of the game is to get as much points as you can
- one turn lasts at most 120 seconds, but can be skipped
- you can move up to three figures each turn
- each figure can be either:
  - moved to or from the bench (the starting position of figures)
  - moved at most *number displayed on figure* steps
- figure can be moved to a certain node if either:
  - the node is empty
  - there is other player's figure with **greater** number
    - other figure can be moved to some **empty neighbouring** node
  - there is other player's figure with **same or greater** number,
  but the node is **surrounded** -- all neighbouring nodes are occupied
    - other figure can be moved to **any empty** node

In the original game, you can only have two quest at the same time
and only submit one at the end of your turn.
Quest cannot repeat and when one player finishes a certain amount of quests,
the game ends.
These rules can still be used by the players,
but are not enforced by the game.

Due to different purposes of using this game (see Why),
it is possible to have at most three quests
and change and submit them at any time during turn.
The only exception is when any of the figures was moved
to or from the bench -- in that case, no quests can be submitted
for the remaining of the turn.
The playing time is also not enforced by the game.

Each quest has a name and a list of numbers:

- the name indicates the nodes that need to be occupied and count
for a given quest, those nodes are also visually indicated while
the quest is shown
- the numbers are the points player gets by submitting a quest,
according to the number of own figures occupying nodes shown by
the quest -- first corresponds to 0 figures, second to 1 figure...

## Why

I'll explain later. (= #TODO)
