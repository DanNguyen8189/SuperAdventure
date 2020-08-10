using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.ComponentModel;


namespace Engine
{
    public class Player : LivingCreature
    {
        //backing variables
        private int _gold;
        private int _experiencePoints;
        public Monster _currentMonster;

        public int Gold
        {
            get { return _gold; }
            set
            {
                _gold = value;
                OnPropertyChanged("Gold");
            }
        }

        public int ExperiencePoints
        {
            get { return _experiencePoints; }
            private set
            {
                _experiencePoints = value;
                OnPropertyChanged("ExperiencePoints");
                OnPropertyChanged("Level");
            }
        }
        public int Level
        {
            get { return ((ExperiencePoints / 100) + 1); }
        }
        public Weapon CurrentWeapon { get; set; }
        private Location _currentLocation;
        public Location CurrentLocation
        {
            get { return _currentLocation; }
            set
            {
                _currentLocation = value;
                OnPropertyChanged("CurrentLocation");
            }
        }
        public BindingList<InventoryItem> Inventory { get; set; }
        public BindingList<PlayerQuest> Quests { get; set; }
        public List<Weapon> Weapons
        {
            get { return Inventory.Where(x => x.Details is Weapon).Select(x => x.Details as Weapon).ToList(); }
        }
        public List<HealingPotion> Potions
        {
            get { return Inventory.Where(x => x.Details is HealingPotion).Select(x => x.Details as HealingPotion).ToList();  }
        } 
        private Player(int currentHitPoints, int maximumHitPoints, int gold, int experiencePoints) : base(currentHitPoints, maximumHitPoints)
        {
            Gold = gold;
            ExperiencePoints = experiencePoints;

            Inventory = new BindingList<InventoryItem>();
            Quests = new BindingList<PlayerQuest>();
        }

        /* FUnction to create starter character with beginner stats and items*/
        public static Player CreateDefaultPlayer()
        {
            Player player = new Player(10, 10, 20, 0);
            player.Inventory.Add(new InventoryItem(World.ItemByID(World.ITEM_ID_RUSTY_SWORD), 1));
            player.CurrentLocation = World.LocationByID(World.LOCATION_ID_HOME);

            return player;
        }

        /* Function to create saved player data on disk */
        public static Player CreatePlayerFromXmlString(string xmlPlayerData)
        {
            try
            {
                XmlDocument playerData = new XmlDocument();

                playerData.LoadXml(xmlPlayerData);

                int currentHitPoints = Convert.ToInt32(playerData.SelectSingleNode("/Player/Stats/CurrentHitPoints").InnerText);
                int maximumHitPoints = Convert.ToInt32(playerData.SelectSingleNode("/Player/Stats/MaximumHitPoints").InnerText);
                int gold = Convert.ToInt32(playerData.SelectSingleNode("/Player/Stats/Gold").InnerText);
                int experiencePoints = Convert.ToInt32(playerData.SelectSingleNode("/Player/Stats/ExperiencePoints").InnerText);

                Player player = new Player(currentHitPoints, maximumHitPoints, gold, experiencePoints);

                int currentLocationID = Convert.ToInt32(playerData.SelectSingleNode("/Player/Stats/CurrentLocation").InnerText);
                player.CurrentLocation = World.LocationByID(currentLocationID);

                if (playerData.SelectSingleNode("/Player/Stats/CurrentWeapon") != null)
                {
                    int currentWeaponID = Convert.ToInt32(playerData.SelectSingleNode("/Player/Stats/CurrentWeapon").InnerText);
                    player.CurrentWeapon = (Weapon)World.ItemByID(currentWeaponID);
                }

                foreach (XmlNode node in playerData.SelectNodes("/Player/InventoryItems/InventoryItem"))
                {
                    int id = Convert.ToInt32(node.Attributes["ID"].Value);
                    int quantity = Convert.ToInt32(node.Attributes["Quantity"].Value);

                    for (int i = 0; i < quantity; i++)
                    {
                        player.AddItemToInventory(World.ItemByID(id));
                    }
                }

                foreach (XmlNode node in playerData.SelectNodes("/Player/PlayerQuests/PlayerQuest"))
                {
                    int id = Convert.ToInt32(node.Attributes["ID"].Value);
                    bool isCompleted = Convert.ToBoolean(node.Attributes["IsCompleted"].Value);

                    PlayerQuest playerQuest = new PlayerQuest(World.QuestByID(id));
                    playerQuest.IsCompleted = isCompleted;

                    player.Quests.Add(playerQuest);
                }

                return player;
            }
            catch
            {
                // If there was an error with the XML data, return a default player object
                return Player.CreateDefaultPlayer();
            }
        }
        /* Save the player data for the next time the game is played*/
        public string ToXmlString()
        {
            XmlDocument playerData = new XmlDocument();

            // Create the top-level XML node
            XmlNode player = playerData.CreateElement("Player");
            playerData.AppendChild(player);

            // Create the "Stats" child node to hold the other player statistics nodes
            XmlNode stats = playerData.CreateElement("Stats");
            player.AppendChild(stats);

            // Create the child nodes for the "Stats" node
            XmlNode currentHitPoints = playerData.CreateElement("CurrentHitPoints");
            currentHitPoints.AppendChild(playerData.CreateTextNode(this.CurrentHitPoints.ToString()));
            stats.AppendChild(currentHitPoints);

            XmlNode maximumHitPoints = playerData.CreateElement("MaximumHitPoints");
            maximumHitPoints.AppendChild(playerData.CreateTextNode(this.MaximumHitPoints.ToString()));
            stats.AppendChild(maximumHitPoints);

            XmlNode gold = playerData.CreateElement("Gold");
            gold.AppendChild(playerData.CreateTextNode(this.Gold.ToString()));
            stats.AppendChild(gold);

            XmlNode experiencePoints = playerData.CreateElement("ExperiencePoints");
            experiencePoints.AppendChild(playerData.CreateTextNode(this.ExperiencePoints.ToString()));
            stats.AppendChild(experiencePoints);

            XmlNode currentLocation = playerData.CreateElement("CurrentLocation");
            currentLocation.AppendChild(playerData.CreateTextNode(this.CurrentLocation.ID.ToString()));
            stats.AppendChild(currentLocation);

            if (CurrentWeapon != null)
            {
                XmlNode currentWeapon = playerData.CreateElement("CurrentWeapon");
                currentWeapon.AppendChild(playerData.CreateTextNode(this.CurrentWeapon.ID.ToString()));
                stats.AppendChild(currentWeapon);
            }

            // Create the "InventoryItems" child node to hold each InventoryItem node
            XmlNode inventoryItems = playerData.CreateElement("InventoryItems");
            player.AppendChild(inventoryItems);

            // Create an "InventoryItem" node for each item in the player's inventory
            foreach (InventoryItem item in this.Inventory)
            {
                XmlNode inventoryItem = playerData.CreateElement("InventoryItem");

                XmlAttribute idAttribute = playerData.CreateAttribute("ID");
                idAttribute.Value = item.Details.ID.ToString();
                inventoryItem.Attributes.Append(idAttribute);

                XmlAttribute quantityAttribute = playerData.CreateAttribute("Quantity");
                quantityAttribute.Value = item.Quantity.ToString();
                inventoryItem.Attributes.Append(quantityAttribute);

                inventoryItems.AppendChild(inventoryItem);
            }

            // Create the "PlayerQuests" child node to hold each PlayerQuest node
            XmlNode playerQuests = playerData.CreateElement("PlayerQuests");
            player.AppendChild(playerQuests);

            // Create a "PlayerQuest" node for each quest the player has acquired
            foreach (PlayerQuest quest in this.Quests)
            {
                XmlNode playerQuest = playerData.CreateElement("PlayerQuest");

                XmlAttribute idAttribute = playerData.CreateAttribute("ID");
                idAttribute.Value = quest.Details.ID.ToString();
                playerQuest.Attributes.Append(idAttribute);

                XmlAttribute isCompletedAttribute = playerData.CreateAttribute("IsCompleted");
                isCompletedAttribute.Value = quest.IsCompleted.ToString();
                playerQuest.Attributes.Append(isCompletedAttribute);

                playerQuests.AppendChild(playerQuest);
            }

            return playerData.InnerXml; // The XML document, as a string, so we can save the data to disk
        }

        /* Function to move the player to a new location */
        public void MoveTo(Location newLocation)
        {
            //Does the location have any required items
            if (!HasRequiredItemToEnterThisLocation(newLocation))
            {
                // rtbMessages.Text += "You must have a " + newLocation.ItemRequiredToEnter.Name + " to enter this location." + Environment.NewLine;
                return;
            }

            // Update the player's current location
            CurrentLocation = newLocation;

            // Show/hide available movement buttons
            // btnNorth.Visible = (newLocation.LocationToNorth != null);
            // btnEast.Visible = (newLocation.LocationToEast != null);
            // btnSouth.Visible = (newLocation.LocationToSouth != null);
            // btnWest.Visible = (newLocation.LocationToWest != null);

            // Display current location name and description
            // rtbLocation.Text = newLocation.Name + Environment.NewLine;
            // rtbLocation.Text += newLocation.Description + Environment.NewLine;

            // Completely heal the player
            CurrentHitPoints = MaximumHitPoints;

            // Update Hit Points in UI
            // lblHitPoints.Text = _player.CurrentHitPoints.ToString(); // data binding takes care of this

            // Does the location have a quest?
            if (newLocation.QuestAvailableHere != null)
            {
                // See if the player already has the quest, and if they've completed it
                bool playerAlreadyHasQuest = HasThisQuest(newLocation.QuestAvailableHere);
                bool playerAlreadyCompletedQuest = CompletedThisQuest(newLocation.QuestAvailableHere);

                // See if the player already has the quest
                if (playerAlreadyHasQuest)
                {
                    // If the player has not completed the quest yet
                    if (!playerAlreadyCompletedQuest)
                    {
                        // See if the player has all the items needed to complete the quest
                        bool playerHasAllItemsToCompleteQuest = HasAllQuestCompletionItems(newLocation.QuestAvailableHere);

                        // The player has all items required to complete the quest
                        if (playerHasAllItemsToCompleteQuest)
                        {
                            completeQuest(newLocation);
                        }
                    }
                }
                else
                {
                    // The player does not already have the quest

                    // Display the messages
                    /* rtbMessages.Text += "You received the " + newLocation.QuestAvailableHere.Name + " quest." + Environment.NewLine;
                    rtbMessages.Text += newLocation.QuestAvailableHere.Description + Environment.NewLine;
                    rtbMessages.Text += "To complete this, come back with:" + Environment.NewLine; */
                    foreach (QuestCompletionItem qci in newLocation.QuestAvailableHere.QuestCompletionItems)
                    {
                        if (qci.Quantity == 1)
                        {
                            RaiseMessage(qci.Quantity.ToString() + " " + qci.Details.Name + Environment.NewLine);
                        }
                        else
                        {
                            RaiseMessage(qci.Quantity.ToString() + " " + qci.Details.NamePlural + Environment.NewLine);
                        }
                    }
                    // rtbMessages.Text += Environment.NewLine;

                    // Add the quest to the player's quest list
                    Quests.Add(new PlayerQuest(newLocation.QuestAvailableHere));
                }
            }

            // Does the location have a monster?
            if (newLocation.MonsterLivingHere != null)
            {
                RaiseMessage("You see a " + newLocation.MonsterLivingHere.Name + "!" + Environment.NewLine);

                // Make a new monster and add its loot table, using the values from the standard monster in the World.Monster list
                Monster standardMonster = World.MonsterByID(newLocation.MonsterLivingHere.ID);

                _currentMonster = new Monster(standardMonster.ID, standardMonster.Name, standardMonster.MaximumDamage,
                    standardMonster.RewardExperiencePoints, standardMonster.RewardGold, standardMonster.CurrentHitPoints, standardMonster.MaximumHitPoints);

                foreach (LootItem lootItem in standardMonster.LootTable)
                {
                    _currentMonster.LootTable.Add(lootItem);
                }

                // Prepare UI for combat
                /* cboWeapons.Visible = Weapons.Any();
                cboPotions.Visible = Potions.Any();
                btnUseWeapon.Visible = Weapons.Any();
                btnUsePotion.Visible = Potions.Any();*/
            }
            else
            {
                _currentMonster = null;

                /*cboWeapons.Visible = false;
                cboPotions.Visible = false;
                btnUseWeapon.Visible = false;
                btnUsePotion.Visible = false;*/
            }
        }

        /* Functions to move the player a specific direction */
        public void MoveNorth()
        {
            if (CurrentLocation.LocationToNorth != null)
            {
                MoveTo(CurrentLocation.LocationToNorth);
            }
        }

        public void MoveEast()
        {
            if (CurrentLocation.LocationToEast != null)
            {
                MoveTo(CurrentLocation.LocationToEast);
            }
        }

        public void MoveSouth()
        {
            if (CurrentLocation.LocationToSouth != null)
            {
                MoveTo(CurrentLocation.LocationToSouth);
            }
        }

        public void MoveWest()
        {
            if (CurrentLocation.LocationToWest != null)
            {
                MoveTo(CurrentLocation.LocationToWest);
            }
        }
        void completeQuest(Location newLocation)
        {
            // Display message
            /* rtbMessages.Text += Environment.NewLine;
            rtbMessages.Text += "You just completed the '" + newLocation.QuestAvailableHere.Name + "' quest." + Environment.NewLine; */

            // Remove quest items from inventory
            RemoveQuestCompletionItems(newLocation.QuestAvailableHere);

            // Give quest rewards
            RaiseMessage("You received: " + Environment.NewLine);
            RaiseMessage(newLocation.QuestAvailableHere.RewardExperiencePoints.ToString() + " experience points" + Environment.NewLine);
            RaiseMessage(newLocation.QuestAvailableHere.RewardGold.ToString() + " gold" + Environment.NewLine);
            RaiseMessage(newLocation.QuestAvailableHere.RewardItem.Name + Environment.NewLine);
            RaiseMessage(Environment.NewLine);

            AddExperiencePoints(newLocation.QuestAvailableHere.RewardExperiencePoints);
            AddGold(newLocation.QuestAvailableHere.RewardGold);

            // Add the reward item to the player's inventory
            AddItemToInventory(newLocation.QuestAvailableHere.RewardItem);

            // Mark the quest as completed
            MarkQuestCompleted(newLocation.QuestAvailableHere);
            // updatePlayerStats();
        }
        public bool HasRequiredItemToEnterThisLocation(Location location)
        {
            if (location.ItemRequiredToEnter == null)
            {
                // There is no required item for this location, so return "true"
                return true;
            }

            // See if the player has the required item in their inventory
            /* foreach (InventoryItem ii in Inventory)
            {
                if (ii.Details.ID == location.ItemRequiredToEnter.ID)
                {
                    // We found the required item, so return "true"
                    return true;
                }
            }*/
            return Inventory.Any(ii => ii.Details.ID == location.ItemRequiredToEnter.ID);

            // We didn't find the required item in their inventory, so return "false"
            // return false;
        }

        /* Function to notify UI when Inventory changes */
        private void RaiseInventoryChangedEvent(Item item)
        {
            if (item is Weapon)
            {
                OnPropertyChanged("Weapons");
            }

            if (item is HealingPotion)
            {
                OnPropertyChanged("Potions");
            }
        }

        /* Function to remove item from inventory */
        public void RemoveItemFromInventory(Item itemToRemove, int quantity = 1)
        {
            InventoryItem item = Inventory.SingleOrDefault(ii => ii.Details.ID == itemToRemove.ID);

            if (item == null)
            {
                // The item is not in the player's inventory, so ignore it.
                // We might want to raise an error for this situation
            }
            else
            {
                // They have the item in their inventory, so decrease the quantity
                item.Quantity -= quantity;

                // Don't allow negative quantities.
                // We might want to raise an error for this situation
                if (item.Quantity < 0)
                {
                    item.Quantity = 0;
                }

                // If the quantity is zero, remove the item from the list
                if (item.Quantity == 0)
                {
                    Inventory.Remove(item);
                }

                // Notify the UI that the inventory has changed
                RaiseInventoryChangedEvent(itemToRemove);
            }
        }
        public bool HasThisQuest(Quest quest)
        {
            return Quests.Any(pq => pq.Details.ID == quest.ID);
        }

        public bool CompletedThisQuest(Quest quest)
        {
            foreach (PlayerQuest playerQuest in Quests)
            {
                if (playerQuest.Details.ID == quest.ID)
                {
                    return playerQuest.IsCompleted;
                }
            }

            return false;
        }

        public bool HasAllQuestCompletionItems(Quest quest)
        {
            /*
            // See if the player has all the items needed to complete the quest here
            foreach (QuestCompletionItem qci in quest.QuestCompletionItems)
            {
                bool foundItemInPlayersInventory = false;

                // Check each item in the player's inventory, to see if they have it, and enough of it
                foreach (InventoryItem ii in Inventory)
                {
                    if (ii.Details.ID == qci.Details.ID) // The player has the item in their inventory
                    {
                        foundItemInPlayersInventory = true;

                        if (ii.Quantity < qci.Quantity) // The player does not have enough of this item to complete the quest
                        {
                            return false;
                        }
                    }
                }

                // The player does not have any of this quest completion item in their inventory
                if (!foundItemInPlayersInventory)
                {
                    return false;
                }
            }

            // If we got here, then the player must have all the required items, and enough of them, to complete the quest.
            return true; */
            // See if the player has all the items needed to complete the quest here
            foreach (QuestCompletionItem qci in quest.QuestCompletionItems)
            {
                // Check each item in the player's inventory, to see if they have it, and enough of it
                if (!Inventory.Any(ii => ii.Details.ID == qci.Details.ID && ii.Quantity >= qci.Quantity))
                {
                    return false;
                }
            }

            // If we got here, then the player must have all the required items, and enough of them, to complete the quest.
            return true;
        }

        public void RemoveQuestCompletionItems(Quest quest)
        {
            foreach (QuestCompletionItem qci in quest.QuestCompletionItems)
            {
                /*foreach (InventoryItem ii in Inventory)
                {
                    if (ii.Details.ID == qci.Details.ID)
                    {
                        // Subtract the quantity from the player's inventory that was needed to complete the quest
                        ii.Quantity -= qci.Quantity;
                        break;
                    }
                }*/

                // Subtract the quantity from the player's inventory that was needed to complete the quest
                InventoryItem item = Inventory.SingleOrDefault(ii => ii.Details.ID == qci.Details.ID);

                if (item != null)
                {
                    RemoveItemFromInventory(item.Details, qci.Quantity);
                }
            }
        }

        public void AddItemToInventory(Item itemToAdd, int quantity=1)
        {
            /* foreach (InventoryItem ii in Inventory)
            {
                if (ii.Details.ID == itemToAdd.ID)
                {
                    // They have the item in their inventory, so increase the quantity by one
                    ii.Quantity++;

                    return; // We added the item, and are done, so get out of this function
                }
            }

            // They didn't have the item, so add it to their inventory, with a quantity of 1
            Inventory.Add(new InventoryItem(itemToAdd, 1)); */

            InventoryItem item = Inventory.SingleOrDefault(ii => ii.Details.ID == itemToAdd.ID);

            if (item == null)
            {
                // They didn't have the item, so add it to their inventory
                Inventory.Add(new InventoryItem(itemToAdd, quantity));
            }
            else
            {
                // They have the item in their inventory, so increase the quantity
                item.Quantity += quantity;
            }

            RaiseInventoryChangedEvent(itemToAdd);
        }

        public void MarkQuestCompleted(Quest quest)
        {
            // Find the quest in the player's quest list
            foreach (PlayerQuest pq in Quests)
            {
                if (pq.Details.ID == quest.ID)
                {
                    // Mark it as completed
                    pq.IsCompleted = true;

                    return; // We found the quest, and marked it complete, so get out of this function
                }
            }
        }

        /* Function to add experience points and increase max health if level-up*/
        public void AddExperiencePoints(int experiencePointsToAdd)
        {
            ExperiencePoints += experiencePointsToAdd;
            MaximumHitPoints = (Level * 10);
        }
        
        public void AddGold(int goldToAdd)
        {
            Gold += goldToAdd;
            // TODO max gold?
        } 

        /* Take gold from player but return false if player has insufficient funds */
        public bool SubtractGold(int goldToSubtract)
        {
            if (Gold - goldToSubtract < 0)
            {
                return false;
            }
            Gold -= goldToSubtract;
            return true;
        }
        /* Function for monster attack */
        public void MonsterAttack()
        {
            // monster attacks
            int damageToPlayer = Engine.RandomNumberGenerator.NumberBetween(0, _currentMonster.MaximumDamage);
            RaiseMessage(_currentMonster.Name + " attacks and deals " + damageToPlayer.ToString() + " damage!" + Environment.NewLine);
            CurrentHitPoints -= damageToPlayer;
            // updatePlayerStats();
            if (CurrentHitPoints <= 0)
            {
                // player defeated
                RaiseMessage("You've been defeated!" + Environment.NewLine);
                MoveTo(World.LocationByID(World.LOCATION_ID_HOME));
            }
        }
        public void winBattle()
        {
            RaiseMessage("You've defeated " + _currentMonster.Name);

            //give player rewards
            AddExperiencePoints(_currentMonster.RewardExperiencePoints);
            AddGold(_currentMonster.RewardGold);

            // Collect chance loot from loot table
            List<LootItem> lootCollected = new List<LootItem>();
            int rngNumber = 0;
            foreach (LootItem li in _currentMonster.LootTable)
            {
                rngNumber = Engine.RandomNumberGenerator.NumberBetween(0, 100);
                if (rngNumber <= li.DropPercentage)
                {
                    lootCollected.Add(li);
                }
            }
            /* Make sure player gets at least one item */
            if (lootCollected.Count == 0)
            {
                foreach (LootItem li in _currentMonster.LootTable)
                {
                    if (li.IsDefaultItem)
                    {
                        lootCollected.Add(li);
                        break;
                    }
                }
            }
            foreach (LootItem li in lootCollected)
            {
                bool playerHasItem = false;
                foreach (InventoryItem ii in Inventory)
                {
                    if (ii.Details.ID == li.Details.ID)
                    {
                        ii.Quantity += 1;
                        playerHasItem = true;
                    }
                }
                if (playerHasItem == false)
                {
                    // player doesn't have it yet, add it to their inventory
                    Inventory.Add(new InventoryItem(li.Details, 1));
                }
                RaiseMessage("You've received " + li.Details.Name + Environment.NewLine);
            }
            // If no items were randomly selected, then add the default loot item(s).
            /*if (lootCollected.Count == 0)
            {
                foreach (LootItem li in _currentMonster.LootTable)
                {
                    if (li.IsDefaultItem)
                    {
                        _player.Inventory.Add(new InventoryItem(li.Details, 1));
                        rtbMessages.Text += "You've received " + li.Details.Name;
                        break;
                    }
                }
            }*/
        }
        public event EventHandler<MessageEventArgs> OnMessage;
        private void RaiseMessage(string message, bool addExtraNewLine = false)
        {
            if (OnMessage != null)
            {
                OnMessage(this, new MessageEventArgs(message, addExtraNewLine));
            }
        }
    }
}
