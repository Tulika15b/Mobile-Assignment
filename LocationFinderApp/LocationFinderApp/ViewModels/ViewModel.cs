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
        User newUser = new User();
        HttpServiceRequestClass req = new HttpServiceRequestClass();


        #region ISOLATEDSTORAGE_SETTINGS_RELATED
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
        /// Function to save the Last submission time when location submitted in background
        /// </summary>
        /// <param name="timeNow"></param>
        public void saveLastSubmittedTimeInBackground(DateTime timeNow)
        {
            if(userSettings.Count > 0)
            {
                if(userSettings.Contains("lastSubmissionTime"))
                {
                    userSettings["lastSubmissionTime"] = timeNow;
                }
                else
                {
                    userSettings.Add("lastSubmissionTime", timeNow);
                }
            }
        }

        /// <summary>
        /// Function to get the last submitted time when the app submitted location in background
        /// </summary>
        /// <returns></returns>
        public DateTime getLastSubmittedTimeInBackground()
        {
            DateTime inBackgroundLocationSubmissionTime = new DateTime();
            if (userSettings.Count > 0)
            {
                if (userSettings.Contains("lastSubmissionTime"))
                {
                   inBackgroundLocationSubmissionTime = (DateTime) userSettings["lastSubmissionTime"];
                   userSettings.Remove("lastSubmissionTime");
                }
                
            }
            return inBackgroundLocationSubmissionTime;
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
        #endregion

        #region SEND_LOCATION
        /// <summary>
        /// Send location function initiates the 
        /// </summary>
        /// <returns></returns>
        public void sendLocationData(Location location)
        {
            isLastSubmitted = true;
            newUser.location.Latitude = location.Latitude;
            newUser.location.Longitude = location.Longitude;

            //Create Http Request
            HttpWebRequest clientReq = req.createHttpRequest(Constants.URI);

            clientReq.BeginGetRequestStream(new AsyncCallback(GetRequestStreamCallback), clientReq);
             
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

        #endregion

        /// <summary>
        /// Calculate the relative time from last submission
        /// </summary>
        /// <param name="submittedTime"></param>
        /// <returns></returns>
        public string getLastSubmittedTime(DateTime submittedTime)
        {
           
            var nowTime = DateTime.Now;
                var lastSubmittedTime = submittedTime;
                newUser.LastUpdatedOn = RelativeTimeConvertor.calculateRelativeTime(lastSubmittedTime);
           
            return newUser.LastUpdatedOn;
        }
        
        
    }
}
