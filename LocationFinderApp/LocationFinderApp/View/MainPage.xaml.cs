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
using System.Threading;

namespace LocationFinderApp
{
    public partial class MainPage : PhoneApplicationPage
    {

        ViewModel viewModel = new ViewModel();
        DateTime lastSubmittedDateTime;
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

        /// <summary>
        /// Event when page is Navigated From
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            //This implies app is going to the background state; 
            //Hence Save the state 
            saveUserInfo();

           
        }

        /// <summary>
        /// Event when page is Navigated To from App.xaml page
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            newUser = viewModel.GetUserSavedData();
           
            if(newUser != null)
            {
               //this implies it is not the first time of app
               //Hence read the saved data and display on Screen
                isFirstTime = false;
                Lat.Text = newUser.location.Latitude;
                Long.Text = newUser.location.Longitude;
                fetchLocation(isFirstTime);
                lastSubmitted_txt_blk.Text = viewModel.getLastSubmittedTime(newUser.LastSubmittedDateTime);
                LocationUserName.Text = newUser.userName;

            }
           else
           {
               Lat.Text = "0.0";
               Long.Text = "0.0";
               fetchLocation(isFirstTime);
               
           }
            
        }

        /// <summary>
        /// Function to send coordinates to Http call
        /// </summary>
        public void sendLocation()
        {
            try
            {
                if (Lat.Text != String.Empty && Long.Text != String.Empty)
                {
                    string response = viewModel.sendLocationData();
                    lastSubmittedDateTime = DateTime.Now;
                    getRelativeLastSubmittedTime(DateTime.Now);

                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
            
        }   

      
        /// <summary>
        /// Function to fetch Location coordinates to be printed on screen
        /// </summary>
        /// <param name="isFirstTime"></param>
        public async void fetchLocation(bool isFirstTime)
        {
            try
            {

                User user = await viewModel.fetchLocation(isFirstTime);
                
                if (user != null)
                {
                        Lat.Text = user.location.Latitude;
                        Long.Text = user.location.Longitude;
                        lastSubmitted_txt_blk.Text = user.lastUpdatedOn;
                    
                }
           
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            
        }

        /// <summary>
        /// Callback for submit button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Submit_Button_Click(object sender, RoutedEventArgs e)
        {
            //Call to make a http post to server
            sendLocation();
            lastSubmittedDateTime = DateTime.Now;

           // getRelativeLastSubmittedTime(DateTime.Now);
        }

        /// <summary>
        /// Function to handle the text in texbox change event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if(textBox.Text != null)
            {
                PhoneApplicationService.Current.State[Constants.USERNAME] = textBox.Text;
            }
        }

        /// <summary>
        /// Save User info present on screen
        /// </summary>
        private void saveUserInfo()
        {
            User newUser = new User();
            newUser.userName = LocationUserName.Text;
            newUser.location.Longitude = Long.Text;
            newUser.location.Latitude = Lat.Text;
            newUser.lastUpdatedOn = lastSubmitted_txt_blk.Text;
            newUser.LastSubmittedDateTime = lastSubmittedDateTime;

            viewModel.saveUserData(newUser);
        }

        /// <summary>
        /// Get Relative time from last submission time till now
        /// </summary>
        /// <param name="startTime"></param>
        private void getRelativeLastSubmittedTime(DateTime startTime)
        {
           var time =  viewModel.getLastSubmittedTime(startTime);
           lastSubmitted_txt_blk.Text = time;
        }

    }
}