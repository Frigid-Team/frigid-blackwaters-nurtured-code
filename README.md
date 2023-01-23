# Frigid Blackwaters Code

http://frigidblackwaters.com/

## An Overview
Hello! This repo contains all of the C# script files used in Frigid Blackwaters, including scripts used for tools and gameplay. 

Frigid Blackwaters is separated into 4 major assemblies:
- Game: This is the primary location for all scripts associated with gameplay systems. Some examples include mobs, level generation, and damage detection. 
- Core: For all logic/code that doesn't specifically involve itself with Frigid's gameplay. 
- Utility: Several classes that help makes our lives easier. Doesn't contain any logic used by the game. 
- Cheats: A very small assembly that contains only code that helps developers cheat through the project for debugging purposes. 

Each of these assemblies has their own folder within the scripts folder, and each system is commonly grouped under a folder as well. For example, the Mobs system that concerns itself with all logic pertaining to enemies, the player and NPCs in the game is located under Scripts/Game/Mobs. 

The entrypoint of the game is in TiledLevel. This is where all the code for the level is spawned (as 1x1 tiles in the world), which all the creatures in the game follow. We also have UI code in Scripts/Game/HUD and Scripts/Game/Menus.  
