# SuperAdventure
Role playing game written in C#!

The basics - a player can move between locations, collect quests, and battle monsters</br>
It consists of 2 projects, Engine and SuperAdventure.</br>
Engine handles the logic of the program while SuperAdventure handles the UI</br>

Engine:</br>
World.cs: creates our game world and populates it with the necessary objects (Locations, items, monsters, quests)</br>
RandomNumberGenerator.cs: overrides c#'s RandomNumberGenerator for our purposes</br>
Item.cs: Item base class</br>
LivingCreature.cs: creature base class</br>
Quest.cs: quest base class</br>
