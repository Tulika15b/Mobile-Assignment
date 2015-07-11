using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using LocationFinderApp.Resources;
using Windows.Devices.Geolocation;
using LocationFinderApp.Model;
using LocationFinderApp.Utilities;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Threading;
using Newtonsoft.Json;
using LocationFinderApp.ViewModels;
using System.IO.IsolatedStorage;

namespace LocationFinderApp
{
    public partial class MainPage : PhoneApplicationPage
    {

        ViewModel viewModel = new ViewModel();
        string lastUpdatedLocationOn;
        bool isFirstTime = true;
        
        // Constructor
        public MainPage()
        {
            InitializeComponent();            
        }


        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            base.OnBackKeyPress(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            //This implies app is going to the background state; 
            //Hence Save the state 
            saveUserInfo();

           
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
           User newUser = viewModel.GetUserSavedData();
           
            if(newUser != null)
            {
               //this implies it is not the first time of app
               //Hence read the saved data and display on Screen
                isFirstTime = false;
                fetchLocation(isFirstTime);
                lastSubmitted_txt_blk.Text = String.Format(Constants.LAST_UPDATED, newUser.LastUpdatedOn);
                LocationUserName.Text = newUser.userName;

            }
           else
           {
               Lat.Text = "0";
               Long.Text = "0";
               
               fetchLocation(isFirstTime);
               

               
           }
            
        }

        public void sendLocation()
        {
            if(Lat.Text != String.Empty && Long.Text != String.Empty)
            viewModel.sendLocationData();
        }   

      
        public async void fetchLocation(bool isFirstTime)
        {
            var loc = await viewModel.fetchLocation(isFirstTime);
            Lat.Text = loc.Latitude;
            Long.Text = loc.Longitude;
        }

        private void Submit_Button_Click(object sender, RoutedEventArgs e)
        {
            //Call to make a http post to server
            sendLocation();
            lastUpdatedLocationOn = DateTime.Now.ToString();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if(textBox.Text != null)
            {
                PhoneApplicationService.Current.State[Constants.USERNAME] = textBox.Text;
            }
        }

        private void saveUserInfo()
        {
            User newUser = new User();
            newUser.userName = LocationUserName.Text;
            newUser.location.Longitude = Long.Text;
            newUser.location.Latitude = Lat.Text;
            newUser.LastUpdatedOn = lastUpdatedLocationOn;
            viewModel.saveUserData(newUser);
        }

    }
}