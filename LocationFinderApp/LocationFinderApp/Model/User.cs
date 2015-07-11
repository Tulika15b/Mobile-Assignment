using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.ComponentModel;

namespace LocationFinderApp.Model
{
    public class User :INotifyPropertyChanged
    {
        public string userName;
        public Location location = new Location();
        public string lastUpdatedOn;
        public string errorMsg;

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public string LastUpdatedOn
        {
            get
            {
                return lastUpdatedOn;
            }
            set
            {
                if (value != lastUpdatedOn)
                {
                    lastUpdatedOn = value;
                    NotifyPropertyChanged("Longitude");
                }
            }
        }



    }
}
