using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    /*Quest that a player has w/ bool for whether player has completed quest or not*/
    public class PlayerQuest : INotifyPropertyChanged
    {
        private Quest _details { get; set; }
        private bool _isCompleted { get; set; }

        public Quest Details
        {
            get { return _details;  }
            set
            {
                _details = value;
                OnPropertyChanged("Details");
            }
        }
        public bool IsCompleted
        {
            get { return _isCompleted; }
            set
            {
                _isCompleted = value;
                OnPropertyChanged("IsCompleted");
                OnPropertyChanged("Name");
            }
        }
        public String Name
        {
            get { return Details.Name; }
        }
        public PlayerQuest(Quest details)
        {
            Details = details;
            IsCompleted = false;
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
