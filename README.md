
# Initial X
https://user-images.githubusercontent.com/102978464/166326809-a2885098-9145-44c5-8d25-00da3196f260.mp4

##  About 
Initial X is a 2D top down view platform game made in Unity, inspired from Rally X. The player needs to collect a certain amount of flags (depending on the level) while avoiding the enemy cars, to advance to the next level. 
The player:
  - Starts with 3 lifes.
  - Has a power, which leaves behind a temporary block, as a way to defend.
  - When an enemy is out of the camera view, it will show a bubble according in which direction he is.
  - Updates the score, for each flag he gets.

## Enemy AI
 - Red Car: it's constantly updating a new route to reach the player for each new position he's in.
 - Pink Car: wanders around the map in search for a purpose.

## Level
Is generated statically with a JSON file. For each "1" in the file it would generate a block and "0" an empty space.
(Currently trying to develop a way to automatically generate a level).
