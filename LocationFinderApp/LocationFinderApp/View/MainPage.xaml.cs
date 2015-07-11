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
        User newUser = new User();
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
            newUser = viewModel.GetUserSavedData();
           
            if(newUser != null)
            {
               //this implies it is not the first time of app
               //Hence read the saved data and display on Screen
                isFirstTime = false;
                fetchLocation(isFirstTime);
                lastSubmitted_txt_blk.Text = viewModel.getLastSubmittedTime(newUser.LastSubmittedDateTime);
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
            {
                string response = viewModel.sendLocationData();
                getRelativeLastSubmittedTime(DateTime.Now);
               
            }
            
        }   

      
        public async void fetchLocation(bool isFirstTime)
        {
            var user = await viewModel.fetchLocation(isFirstTime);
            Lat.Text = user.location.Latitude;
            Long.Text = user.location.Longitude;
           // getRelativeLastSubmittedTime(DateTime.Now);

            
        }

        private void Submit_Button_Click(object sender, RoutedEventArgs e)
        {
            //Call to make a http post to server
            sendLocation();
            lastUpdatedLocationOn = DateTime.Now.ToString();

            getRelativeLastSubmittedTime(DateTime.Now);
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
            newUser.lastUpdatedOn = lastSubmitted_txt_blk.Text;
            viewModel.saveUserData(newUser);
        }

        private void getRelativeLastSubmittedTime(DateTime startTime)
        {
           var time =  viewModel.getLastSubmittedTime(startTime);
           lastSubmitted_txt_blk.Text = time;
        }

    }
}