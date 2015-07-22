using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationFinderApp.Utilities
{
    public class Constants
    {
        public const string URI = "http://gentle-tor-1851.herokuapp.com/events";
        public const string ERROR_MSG = "Check your Internet Connection, or if the problem persists contact system administrator";
        public const string LAST_UPDATED = "Last Submitted on {0}";
        public const string USERNAME = "USERNAME";
        public const string CREDS_USERNAME = "tulika";
        public const string CREDS_PASSWORD = "phil53";
        public const string ERROR = "ERROR";
        public const string SUCCESS = "SUCCESS";
        public const string LOCATION = "LOCATION";
        public const string SUBMIT_MSG = "Location submitted successfully!!";

        public const string SENDING_LOCATION = "Sending Location...";
        public const string FETCHING_LOCATION = "Fetching Location...";
        public const string DEFAULT_COORDINATE = "0.0000";

        public const string TOAST_TITLE = "Locater App";
        public const string TOAST_MSG = "Location submitted at ";

        public const string ERR_CONNECTION_FAILED = "Please check your Internet connection !!";
        public const string ERR_CONNECTION_TIMEOUT = "Connection with server timedOut, please try again";
        public const string ERR_REQUEST_CANCELLED = "Please check your Internet connection !!";
        public const string ERR_LOCATION_SUBMISSION = "";

        public const int GPS_MOVEMENT_THRESHHOLD = 10;
    }
}
