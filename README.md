# SuperAdventure
Role playing game written in C#!

The basics - a player can move between locations, collect quests, and battle monsters
It consists of 2 projects, Engine and SuperAdventure.
Engine handles the logic of the program while SuperAdventure handles the UI

Engine:
World.cs: creates our game world and populates it with the necessary objects (Locations, items, monsters, quests)
RandomNumberGenerator.cs: overrides c#'s RandomNumberGenerator for our purposes.
Item.cs: Item base class
LivingCreature.cs: creature base class
Quest.cs: quest base class
