using Engine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SuperAdventure
{
    public partial class SuperAdventure : Form
    {
        private Player _player;
        private Monster _currentMonster; //the monster the character is currently fighting at current location
        //we do have monsters in the world class, but that's just one instance of the class and it'll go
        //away if the player beats it. So we're holding an instance that the player fights in this file instead
        public SuperAdventure()
        {
            InitializeComponent();

            _player = new Player(10, 10, 20, 0, 1);
            MoveTo(World.LocationByID(World.LOCATION_ID_HOME));
            _player.Inventory.Add(new InventoryItem(World.ItemByID(World.ITEM_ID_RUSTY_SWORD), 1));

            lblHitPoints.Text = _player.CurrentHitPoints.ToString();
            lblGold.Text = _player.Gold.ToString();
            lblExperience.Text = _player.ExperiencePoints.ToString();
            lblLevel.Text = _player.Level.ToString();
        }

        private void SuperAdventure_Load(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            lblGold.Text = "123";
        }

        private void btnNorth_Click(object sender, EventArgs e)
        {

        }

        private void btnEast_Click(object sender, EventArgs e)
        {

        }

        private void btnSouth_Click(object sender, EventArgs e)
        {

        }

        private void btnWest_Click(object sender, EventArgs e)
        {

        }

        private void btnUseWeapon_Click(object sender, EventArgs e)
        {

        }

        private void btnUsePotion_Click(object sender, EventArgs e)
        {

        }

        private void MoveTo(Location newLocation)
        {
            //does location require any items?
            if(newLocation.ItemRequiredToEnter != null)
            {
                //see if player has the item
                bool playerHasRequiredItem = false;
                foreach (InventoryItem item in _player.Inventory)
                {
                    if (newLocation.ItemRequiredToEnter.ID == item.Details.ID)
                    {
                        playerHasRequiredItem = true;
                        break;
                    }
                }

                if (playerHasRequiredItem == false)
                {
                    Console.WriteLine("You need " + newLocation.ItemRequiredToEnter.Name + " to enter here.");
                    rtbMessages.Text += "You must have a " + newLocation.ItemRequiredToEnter.Name + " to enter this location." + Environment.NewLine;
                    return;
                }
            }
            _player.CurrentLocation = newLocation;
            Console.WriteLine("You've entered " + newLocation.Name);
            rtbMessages.Text += "You've entered " + newLocation.Name;

            //show/hide available movement buttons
            btnNorth.Visible = (newLocation.LocationToNorth != null);
            btnEast.Visible = (newLocation.LocationToEast != null);
            btnSouth.Visible = (newLocation.LocationToSouth != null);
            btnWest.Visible = (newLocation.LocationToWest != null);

            //heal player completely
            _player.CurrentHitPoints = _player.MaximumHitPoints;
            lblHitPoints.Text = _player.CurrentHitPoints.ToString();

            //location has a quest?
            if (newLocation.QuestAvailableHere != null)
            {
                bool playerHasQuest = false;
                bool playerHasCompleted = false;
                foreach (PlayerQuest quest in _player.Quests)
                {
                    if (quest.Details.ID == newLocation.QuestAvailableHere.ID)
                    {
                        playerHasQuest = true;
                        playerHasCompleted = quest.IsCompleted;
                        break;
                    }
                }
                if (playerHasQuest == true)
                {
                    if (playerHasCompleted)
                    {
                        return;
                    }
                    else
                    {
                        foreach (InventoryItem item in _player.Inventory)
                        {
                        }
                    }
                }
            }
        }
    }
}
