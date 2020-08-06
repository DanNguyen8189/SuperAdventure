using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Engine
{
    /*Item type in player's inventory*/
    public class InventoryItem : INotifyPropertyChanged
    {
        // backing variables
        private Item _details;
        private int _quantity;

        public Item Details
        {
            get { return _details; }
            set
            {
                _details = value;
                OnPropertyChanged("Details");
            }
        }

        public int Quantity
        {
            get { return _quantity; }
            set
            {
                _quantity = value;
                OnPropertyChanged("Quantity");
                OnPropertyChanged("Description");
            }
        }
        public String Description
        {
            get { return Quantity > 1 ? Details.NamePlural : Details.Name; }
        }
        public InventoryItem(Item details, int quantity)
        {
            Details = details;
            Quantity = quantity;
        }

        /* Event that the UI will subscribe to*/
        public event PropertyChangedEventHandler PropertyChanged;

        /* Function to check if anything is subscribed to the event. If yea, raise the event */
        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
