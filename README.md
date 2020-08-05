# SuperAdventure
Role playing game written in C#!

## Game basics
Here are the basics:
* The player goes to locations.
* The player may need to have certain items to enter a location.
* The location might have a quest available.
* To complete a quest, the player must collect certain items and turn them in.
* The player can collect items by going to a location and fighting monsters there.
* The player fights monsters with weapons.
* The player can use a healing potion while fighting.
* The player receives loot items after defeating a monster.
* After turning in the quest, the player receives reward items.

## Files/Structure
The game consists of 2 projects, Engine and SuperAdventure.</br>
Engine handles the logic of the program while SuperAdventure handles the UI</br>

Engine:</br>
* World.cs: creates our game world and populates it with the necessary objects (Locations, items, monsters, quests)</br>
* RandomNumberGenerator.cs: overrides c#'s RandomNumberGenerator for our purposes</br>
* Item.cs: Item base class</br>
* LivingCreature.cs: creature base class</br>
* Quest.cs: quest base class</br>
