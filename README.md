# README

## Pacwall code structure
The game code can be categorized in 4 parts:
1. **Player related:**
   * Path: Assets/Scripts/player
   * Keeps track of player related things
     * **Player**: Player movement
     * **PlayerInput**: Uses Joystick to move send movement directions to player
2. **Grid related:**
   * Path: Assets/Scripts/grid
   * Keeps track of grid and NPCs on grid
     * **MazeGrid**: Maintains grid space coordinate system and position of different item types in that space.
     * **Ghost**: Runs after player. Fast/slow speed is controlled in editor.
     * **TmpWallItem**: Only to contain grid space position
3. **Input related:**
   * Path: Assets/Joystick Pack
   * Keeps track of joystick based input system
     * **Joystick**: Base of all different Joysticks
     * **FloatingJoystick**: This the one being used. It has a minor modification over the original.
4. **Uncategorized:**
   * Path: Assets/Scripts/*
   * Top level scripts that aren't categorized anywhere:
     * **GameMan**: Game manager. Glues all the pieces together and takes care of start/restart.
     * **GameUI**: Basic UI for the game.
