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
        bool tracking = false;
        Location loc = new Location();
        DispatcherTimer dispatcherTimer = new DispatcherTimer();

        HttpServiceRequestClass req = new HttpServiceRequestClass();
      //  bool alreadyInStack = true;

        // Constructor
        public MainPage()
        {
            InitializeComponent();      
      
        }

        #region UI_CONTROLS
        /// <summary>
        /// Function to set the progress indicator on screen
        /// </summary>
        /// <param name="value"></param>
        /// <param name="progressIndicatorText"></param>
        public void SetProgressIndicator(bool value, string progressIndicatorText)
        {
            SystemTray.ProgressIndicator = new ProgressIndicator();
            SystemTray.ProgressIndicator.IsIndeterminate = value;
            SystemTray.ProgressIndicator.IsVisible = value;
            if (value == true)
            {
                SystemTray.ProgressIndicator.Text = progressIndicatorText;
            }
        }

        /// <summary>
        /// Function to handle the text in texbox change event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox.Text != null)
            {
                PhoneApplicationService.Current.State[Constants.USERNAME] = textBox.Text;
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
            SetProgressIndicator(true, Constants.SENDING_LOCATION);
            sendLocationData();

        }
        #endregion

        #region PAGE_NAVIGATION_FUNCTIONS
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
          //  if (alreadyInStack)
           // {
                newUser = viewModel.GetUserSavedData();
                DateTime defaultDT = new DateTime();

                if (newUser != null)
                {
                    //this implies it is not the first time of app
                    //Hence read the saved data and display on Screen
                    isFirstTime = false;
                    Lat.Text = newUser.location.Latitude;
                    Long.Text = newUser.location.Longitude;
                    fetchLocation(isFirstTime);

                    //Calc relative time from last Submitted date and time, that was saved in the IsolatedStorage file
                    DateTime backgroundSubmissionDT = new DateTime();
                    backgroundSubmissionDT = viewModel.getLastSubmittedTimeInBackground();
                    if (backgroundSubmissionDT.Equals(defaultDT))
                    {
                        lastSubmittedDateTime = newUser.LastSubmittedDateTime;
                    }
                    else
                    {
                        lastSubmittedDateTime = backgroundSubmissionDT;
                    }
                    getRelativeLastSubmittedTime(lastSubmittedDateTime);
                    LocationUserName.Text = newUser.userName;

                }
                else
                {
                    Lat.Text = Constants.DEFAULT_COORDINATE;
                    Long.Text = Constants.DEFAULT_COORDINATE;
                    fetchLocation(isFirstTime);

                }           
          //  }
          //  alreadyInStack = false;
           
            
        }

        #endregion

        

        #region FETCH_LOCATION_COORDINATES
        /// <summary>
        /// Function to fetch Location coordinates to be printed on screen
        /// </summary>
        /// <param name="isFirstTime"></param>
        
        public void fetchLocation(bool isFirstTime)
        {
            // newUser = new User();
            Dispatcher.BeginInvoke(() =>
            {
                SetProgressIndicator(true, "Fetching Location");
            });
            Geolocator geoLocator = new Geolocator();

            if (!tracking)
            {
                geoLocator.DesiredAccuracy = PositionAccuracy.High;
                geoLocator.MovementThreshold = Constants.GPS_MOVEMENT_THRESHHOLD;
                geoLocator.PositionChanged += geoLocator_PositionChanged;
                geoLocator.StatusChanged += geoLocator_StatusChanged;
                tracking = true;

            }
            else
            {
                geoLocator.PositionChanged -= geoLocator_PositionChanged;
                geoLocator.StatusChanged -= geoLocator_StatusChanged;
                geoLocator = null;

                tracking = false;
            }

        }

        void geoLocator_StatusChanged(Geolocator sender, StatusChangedEventArgs args)
        {
            var statusGPS = String.Empty;
            if(args.Status == PositionStatus.Ready)
            {
                //GPS is ready with location coordinates
            }
            else 
            {
                //Issues with Phones GPS, kindly check 
                SetProgressIndicator(false, null);
            }
        }

        void geoLocator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            if (!(App.isRunningInBackground))
            {

                //App in activated or resumed state

                Dispatcher.BeginInvoke(() =>
                {
                    Lat.Text = args.Position.Coordinate.Point.Position.Latitude.ToString(Constants.DEFAULT_COORDINATE);
                    Long.Text = args.Position.Coordinate.Point.Position.Longitude.ToString(Constants.DEFAULT_COORDINATE);
                    SetProgressIndicator(false, null);
                    if (isFirstTime)
                    {
                        SetProgressIndicator(false, null);
                        sendLocationData();
                    }
                    isFirstTime = false;
                });
               
            }
        }
        #endregion

        #region SEND_LOCATION

        public void sendLocationData()
        {
            loc.Latitude = Lat.Text;
            loc.Longitude = Long.Text;

            if (App.isRunningInBackground)
            {
                viewModel.saveLastSubmittedTimeInBackground(lastSubmittedDateTime);
                ShellToast toast = new ShellToast();
                toast.Title = Constants.TOAST_TITLE;
                toast.Content = String.Format(Constants.TOAST_MSG + lastSubmittedDateTime.ToString());
                toast.Show();
                //  alreadyInStack = true;

            }

            //Create Http Request
            HttpWebRequest clientReq = req.createHttpRequest(Constants.URI);
            clientReq.BeginGetRequestStream(new AsyncCallback(GetRequestStreamCallback), clientReq);

        }
        private async void GetRequestStreamCallback(IAsyncResult asynchronousResult)
        {

            string postdata = "data=" + LocationUserName + " is now at " + loc.Latitude + "/" + loc.Longitude;

            try
            {
                HttpWebRequest requestObj = await req.sendHttpRequest(asynchronousResult, postdata);
                requestObj.BeginGetResponse(new AsyncCallback(GetResponseCallback), requestObj);
            }
            catch (WebException ex)
            {
                if (ex.Status != WebExceptionStatus.RequestCanceled)
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        SetProgressIndicator(false, null);
                        MessageBox.Show(Constants.ERR_REQUEST_CANCELLED);
                    });


                }
                else if (ex.Status != WebExceptionStatus.ConnectFailure)
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        SetProgressIndicator(false, null);
                        MessageBox.Show(Constants.ERR_CONNECTION_FAILED);
                    });
                }
                else if (ex.Status != WebExceptionStatus.Timeout)
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        SetProgressIndicator(false, null);
                        MessageBox.Show(Constants.ERR_CONNECTION_TIMEOUT);
                    });
                }

            }
            catch (Exception ex)
            {
                Dispatcher.BeginInvoke(() =>
                {
                    SetProgressIndicator(false, null);
                    MessageBox.Show(ex.Message);
                });
            }

        }

        private async void GetResponseCallback(IAsyncResult asynchronousResult)
        {

            HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;

            // End the operation
            try
            {
                var responseString = await req.receiveHttpResonse(request, asynchronousResult);

                if (responseString != null)
                {
                    if (responseString.Equals(Constants.SUCCESS))
                    {
                        Dispatcher.BeginInvoke(() =>
                        {
                            SetProgressIndicator(false, null);
                            MessageBox.Show(Constants.SUBMIT_MSG);

                            lastSubmittedDateTime = DateTime.Now;
                            getRelativeLastSubmittedTime(DateTime.Now);
                        });
                    }
                    else
                    {
                        Dispatcher.BeginInvoke(() =>
                        {
                            SetProgressIndicator(false, null);
                            MessageBox.Show(Constants.ERROR);
                        });
                    }
                }
            }
            catch (WebException ex)
            {
                if (ex.Status != WebExceptionStatus.RequestCanceled)
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        SetProgressIndicator(false, null);
                        MessageBox.Show(Constants.ERR_REQUEST_CANCELLED);
                    });


                }
                else if (ex.Status != WebExceptionStatus.ConnectFailure)
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        SetProgressIndicator(false, null);
                        MessageBox.Show(Constants.ERR_CONNECTION_FAILED);
                    });
                }
                else if (ex.Status != WebExceptionStatus.Timeout)
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        SetProgressIndicator(false, null);
                        MessageBox.Show(Constants.ERR_CONNECTION_TIMEOUT);
                    });
                }
            }
            catch (Exception ex)
            {
                Dispatcher.BeginInvoke(() =>
                {
                    SetProgressIndicator(false, null);
                    MessageBox.Show(ex.Message);
                });
            }
        }

        #endregion

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

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            // Updating the Label which displays the current second
            getRelativeLastSubmittedTime(lastSubmittedDateTime);

        }
        /// <summary>
        /// Get Relative time from last submission time till now
        /// </summary>
        /// <param name="startTime"></param>
        private void getRelativeLastSubmittedTime(DateTime startTime)
        {
            lastSubmittedDateTime = startTime;
            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 5);
            dispatcherTimer.Start();
           var time =  viewModel.getLastSubmittedTime(startTime);
           
           lastSubmitted_txt_blk.Text = time;
        }


      

    }
}
