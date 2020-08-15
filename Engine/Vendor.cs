using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Engine
{
    public class Vendor : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public BindingList<InventoryItem> Inventory { get; private set; }
        public Vendor(string name)
        {
            Name = name;
            Inventory = new BindingList<InventoryItem>();
        }

        public void AddItemToInventory(Item itemToAdd, int quantity)
        {
            InventoryItem item = Inventory.SingleOrDefault(ii => ii.Details.ID == itemToAdd.ID);
            if (item == null)
            {
                Inventory.Add(new InventoryItem(itemToAdd, quantity));
            }
            else
            {
                item.Quantity += quantity;
            }
            OnPropertyChanged("Inventory");
        }

        public void RemoveItemFromInventory(Item itemToRemove, int quantity)
        {
            InventoryItem item = Inventory.SingleOrDefault(ii => ii.Details.ID == itemToRemove.ID);
            if (item == null)
            {
                return;
            }
            else 
            {
                if (item.Quantity < quantity)
                {
                    Inventory.Remove(item);
                }
                else
                {
                    item.Quantity -= quantity;
                }
            }
            OnPropertyChanged("Inventory");
        }

        // property changed handler
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
