using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocationFinderApp.Model;
using System.Collections.ObjectModel;
using System.IO.IsolatedStorage;
using Windows.Devices.Geolocation;
using LocationFinderApp.Utilities;
using System.Net;
using System.Windows.Threading;


namespace LocationFinderApp.ViewModels
{
    public class ViewModel
    {
        private IsolatedStorageSettings userSettings = IsolatedStorageSettings.ApplicationSettings;

        public bool isLastSubmitted = false;
        public bool isFirstTime;
        DateTime submissionTime;
        User newUser = new User();
        HttpServiceRequestClass req = new HttpServiceRequestClass();

        /// <summary>
        /// Function to fetch userSavedData from the IsolatedStorageSettings
        /// </summary>
        /// <returns></returns>
        public User GetUserSavedData()
        {
           if(userSettings.Count > 0)
           {
               if(userSettings.Contains("NewUser"))
               {
                   newUser = (User)userSettings["NewUser"];
                   return newUser;
               }
               
           }
            
           return null;
          
            
        }

        /// <summary>
        /// Function to save userData to the IsolatedStorageSettings
        /// </summary>
        /// <param name="saveNewUser"></param>
        public void saveUserData(User saveNewUser)
        {
            newUser = saveNewUser;
            if (userSettings.Contains("NewUser"))
            {
                userSettings["NewUser"] = saveNewUser;
            }
            else
            {
                userSettings.Add("NewUser", saveNewUser);
            }
           
        }

        /// <summary>
        /// Function to fetch location using GeoLocation API
        /// </summary>
        /// <param name="checkIfFirstTime"></param>
        /// <returns></returns>
        public async Task<User> fetchLocation(bool checkIfFirstTime)
        {
            isFirstTime = checkIfFirstTime;
            Geolocator geoLocator = new Geolocator();
            geoLocator.DesiredAccuracy = PositionAccuracy.High;
            geoLocator.MovementThreshold = 100;
            geoLocator.PositionChanged += geoLocator_PositionChanged;
            geoLocator.StatusChanged += geoLocator_StatusChanged;
            
            try
            {
                Geoposition geoPosition = await geoLocator.GetGeopositionAsync(
                         maximumAge: TimeSpan.FromMinutes(5),
                         timeout: TimeSpan.FromSeconds(10)
                );

                newUser.location.Latitude = geoPosition.Coordinate.Point.Position.Latitude.ToString();
                newUser.location.Longitude = geoPosition.Coordinate.Point.Position.Longitude.ToString();

            }
            catch(Exception ex)
            {
                newUser.errorMsg = ex.Message;
                newUser.errorCode = Constants.ERROR;
            }

            return newUser;           
        }

        void geoLocator_StatusChanged(Geolocator sender, StatusChangedEventArgs args)
        {
            
        }

        /// <summary>
        /// callback when position changed event is fired by the Geolocation api
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void geoLocator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            if (App.isRunningInBackground)
            {
                Microsoft.Phone.Shell.ShellToast toast = new Microsoft.Phone.Shell.ShellToast();
                
            }
            else
            {
                //App in activated or resumed state
                newUser.location.Latitude = args.Position.Coordinate.Latitude.ToString("0.00");
                newUser.location.Longitude = args.Position.Coordinate.Longitude.ToString("0.00");
                if(isFirstTime)
                {
                    sendLocationData();
                      getLastSubmittedTime(DateTime.Now);
                }
            }
        }
        
        /// <summary>
        /// Send location function initiates the 
        /// </summary>
        /// <returns></returns>
        public string sendLocationData()
        {
            isLastSubmitted = true;
           // submissionTime = DateTime.Now;

            //Create Http Request
            HttpWebRequest clientReq = req.createHttpRequest(Constants.URI);
            
            clientReq.BeginGetRequestStream(new AsyncCallback(GetRequestStreamCallback), clientReq);
            if(newUser.errorMsg != null)
            {
                return Constants.SUCCESS;
            }
            else
            {
                return Constants.ERROR;
            }          
        }

        
        private async void GetRequestStreamCallback(IAsyncResult asynchronousResult)
        {
            if(newUser.userName == null)
            {
                newUser.userName = "John Doe";
            }
            string postdata = "data="+ newUser.userName + " is now at "+ newUser.location.Latitude+"/"+ newUser.location.Longitude;
           
            try
            {
                HttpWebRequest requestObj = await req.sendHttpRequest(asynchronousResult, postdata);
                requestObj.BeginGetResponse(new AsyncCallback(GetResponseCallback), requestObj);
            }
            catch (WebException ex)
            {
                if (ex.Status != WebExceptionStatus.RequestCanceled)
                {
                     newUser.errorMsg = ex.Message;

                }
                
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
                    if(responseString.Equals(Constants.ERROR))
                    {
                        newUser.errorMsg = "Please Try Submitting again";
                    }
                }
            }
            catch (Exception ex)
            {
                newUser.errorMsg = ex.Message;
            }
        }

        /// <summary>
        /// Calculate the relative time from last submission
        /// </summary>
        /// <param name="submittedTime"></param>
        /// <returns></returns>
        public string getLastSubmittedTime(DateTime submittedTime)
        {
            if(isLastSubmitted)
            {
                var lastSubmittedTime = submittedTime;
                newUser.LastUpdatedOn = RelativeTimeConvertor.calculateRelativeTime(lastSubmittedTime);
            }
            return newUser.LastUpdatedOn;
        }
        
        
    }
}
