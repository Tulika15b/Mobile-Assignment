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


namespace LocationFinderApp.ViewModels
{
    public class ViewModel
    {
        private IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
        PersistantDataStorage persistantDataStorageObj = new PersistantDataStorage();

        public bool isLastSubmitted = false;
        public bool isFirstTime;
        DateTime submissionTime;
        User newUser = new User();
        HttpServiceRequestClass req = new HttpServiceRequestClass();
        Location location = new Location();

        public User GetUserSavedData()
        {
            newUser = persistantDataStorageObj.readUserDetails();
            if(newUser != null)
            {
                return newUser;
            }
            else
            {
                return null;
            }
            
        }

        public void saveUserData(User saveNewUser)
        {
            newUser = saveNewUser;
            persistantDataStorageObj.saveUserDetails(newUser);
        }

        public async Task<User> fetchLocation(bool checkIfFirstTime)
        {
            isFirstTime = checkIfFirstTime;
            newUser = new User();
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

        void geoLocator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            if (App.isRunningInBackground)
            {
                Microsoft.Phone.Shell.ShellToast toast = new Microsoft.Phone.Shell.ShellToast();
                // TODO : Call Submit button functionality
                
            }
            else
            {
                //App in activated or resumed state
                location.Latitude = args.Position.Coordinate.Latitude.ToString("0.00");
                location.Longitude = args.Position.Coordinate.Longitude.ToString("0.00");
                if(isFirstTime)
                {
                    sendLocationData();
                }
            }
        }

        public string sendLocationData()
        {
            isLastSubmitted = true;
            submissionTime = DateTime.Now;

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


        public string getLastSubmittedTime(DateTime submittedTime)
        {
            if(isLastSubmitted)
            {
                var lastSubmittedTime = submissionTime;
                newUser.LastUpdatedOn = RelativeTimeConvertor.calculateRelativeTime(lastSubmittedTime);
            }
            return newUser.LastUpdatedOn;
        }
        
        
    }
}
