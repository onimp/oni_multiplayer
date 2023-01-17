# ONI multiplayer mod (WORK IN PROGRESS / PROTOTYPE)

This is a mod which adds multiplayer support to Oxygen not included game.

## Current stage and status

Status: In development
Stage: Early WIP and proof of concept

Working functionality:

- Buttons in UI (Join/Load/Create MP game)
- Ability to join/invite friends via Steam overlay UI
- Loading game save to all new joiners
- Showing cursor of all active players
- Syncing mouse tool actions (dig/build/mop/harvest/etc see tasks list below for more info)

That is it. Nothing else is working yet. No actions will be synchronised yet. It is just a proof of concept.

## Game mechanics

All players play all together and share controls over a single colony. Order given by a player might be overruled by
another.
There is no difference between different players, all players are equal.
It is possible to look and control different asteroids at the same time as well.

## Underhood mechanics idea

Idea is based on the assumption that the game engine will run the same without any user input even on different
machines.
So if user input will be the same on different machines - then their separate simulations should run the same.
To avoid accumulated errors (if any) it is proposed to do periodic (one per game day) hard syncs by loading game save
files.
[Optional] To make smoother experience or if simulations will be running too different it is feasible to do periodic
small world syncs for smaller areas of different layers (e.g. sync gases within an area 16x16 every 30 seconds).

## How to install

To install locally copy release content (with folder) to %USERPROFILE%\Documents\Klei\OxygenNotIncluded\mods\Local (if
some folders are missing, please create them).

## How to use

### Host:

- Host a game either via 'New multiplayer game' or via 'Load multiplayer game'
- Wait until the game is loaded and overlay is opened automatically
- Invite friends via Steam overlay

### Player/Friend:

- Run the game first
- Join invite (from already opened game) or join via Steam overlay
- Wait until game is loaded

# Long TODO List

- Basic features
    - Handle game user actions
        - Implement effects (apply user action to the game) for following actions
          - Cancel
          - Demolish
          - Priority
          - Disinfect
          - Attack
          - Capture
          - Empty pipe
          - Mop
          - Harvest
          - Move debris
        - Handle building settings
        - Handle colonies settings (food/priorities/etc)
    - Every game day hard syncs
    - Measuring amount of out-dated objects
        - Crucial for decision about soft/hard syncs approach
    - Performance assessment (network/memory/cpu)
- Improvements
    - Correctly handling DLC vs Vanilla game versions
    - Game loading indicator
    - InGame Players information
        - Who is in the game
        - Where they are
        - Their ping
        - Ability to kick/vote for kick
    - Launching game straight away
        - Right now it shows a warning about wrong url format
    - In game Lobby browser
    - Translation support
- Bux fixes
    - Correct handling of UI buttons (creating after joined, joining a second time, etc)
    - Cursor draw method is error prone (double phantom cursors, inverted, etc)
- GitHub
    - Add build releases to GitHub
    - Better issue categories :)
    - Build and code cleanup
    - Better README
