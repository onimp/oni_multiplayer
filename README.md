# ONI multiplayer mod (WORN IN PROGRESS / PROTOTYPE)

This is is a mod which adds multiplayer support to Oxygen not included game.

## Current stage and status

Status: In development
Stage: Early WIP and proof of concept

Working functionality:

- Buttons in UI (Join/Load/Create MP game)
- Ability to join/invite friends via Steam overlay UI
- Loading game save to all new joiners
- Showing cursor of all active players

That is it. Nothing else is working yet. No any actions will be synchronized yet. It is just a proof of concept.

## Game mechanics

All players play all together and shares controls over single colony. Order given by a player might be over ruled by
another.
There is no difference between different player, all players are equal.
It is possible to look and control different asteroids at the same time as well.

## Underhood mechanics idea

Idea is based on the assumption that the game engine will run the same without any user input even on different
machines.
So if user input will be the same on different machines - then their separate simulations should run the same.
To avoid accumulated errors (if any) it is proposed to do periodic (one per game day) hard syncs by loading game save
files.
[Optional] To make smoother experience or if simulations will be running too different it is feasible to do periodic
small world syncs for smaller areas of different layers (e.g. sync gasses within area 16x16 every 30 seconds).

## How to install

TODO

## How to use

### Host:
- Host a game either via 'New multiplayer game' or via 'Load multiplayer game'
- Invite friends via Steam overlay

### Player/Friend:
- Run the game first
- Join invite (from already opened game) or join via Steam overlay
- Wait until game is loaded


# Long TODO List

- Handle game user actions
  - Handle UI hotkeys first (dig/mop/build/disinfect/etc) 
  - Handle building settings
  - Handle colonies settings (food/priorities/etc)
- Measuring amount of out-dated objects
  - Crucial for decision about soft/hard syncs approach
- Add build releases to GitHub
- Correctly handling DLC vs Vanilla game versions
- Correct handling of UI buttons (creating after joined, joining a second time, etc)
- In game Lobby browser
- Launching game straight away
  - Right now it shows a warning about wrong url format
- InGame Players information
  - Who is in the game
  - Where they are
  - Their ping
  - Ability to kick/vote for kick
- Game loading indicator
- Performance assessment
- Better issue categories :)
- Build and code cleanup
- Better README