# Mobile-Assignment
Windows Phone 8 Location Updater(The App is also compatible with Windows Phone 8.1 devices)

<b>Code Compilation Instructions :</b>
<b>Prerequisites</b> : 1. Windows 8 System
                2. Visual Studio 2012 or above with Windows 8.0 SDK
                
For Compiling the code After you download the project as a zip file, unzip it and open the solution file .sln present in the folder.

<b>For Building a .xap file and running the App on a device or an emulator :</b>
1. Open the project in Visual Studio 2012(or above), by double clicking the .sln file
2. In the toolbar select either the device plugged in(Windows Phone 8/8.1) or the emulators present.
3. Click on deploy solution in Build toolbar.
4. Also, the .xap file can be deployed to the device using the Application "Windows Phone Application Deployment 8.0".

<b>ABOUT THE APP</b> :

The App primarily fetches the phone current location, by making use of <b>GeoLocator API</b> of Windows Phone 8, and displays the location coordinates onto the screen.
For first time, App Activation : The coordinates are printed on the screen and also are sent to a server in the form of POST call using HttpWebRequest.
If the user presses the start button on phone or the App goes into the Background, App`s <b>"RunningInBackground"</b> event gets fired which makes the Http Call again.
Also while in the App, the user has an option to submit the phone`s location using a SUBMIT Button.

<b>ABOUT THE CODE :</b>

The code has been designed keeping in mind the MVVM design pattern for Windows Phone 8.

1. DataModel

The User class - Contains information like username, Location, isLastSubmitted check, LastSubmitted time string.
Location class - Contains the latitude and longitude as string. It is also a member of User class.

The classes make use of INotifyPropertyChanged which informs the controls, which bind to those properties, that the properties have been changed.

2.View
MainPage.xaml : It contains the screen layout of the App
MainPage.xaml.cs : This file contains the code behind of the screen i.e It has methods to bind the Location details, takes care of username text change, starts the submit functionality, Contains Page events like OnNavigatedTo, OnNavigatedFrom, saving persistant data(function called when page state  navigates from OnNavigatedFrom).

3.ViewModel
ViewModel class acts as an layer between the DataModel and View. It provides the View classes with the data,associated with the Model classes, required to populate them. It contains code for saving data to and fetching data from IsolatedStorageSettings.

4. App.xaml and App.xaml.cs :
It contains the PhoneApplication state events like Activated, Deactivated, Closing, isRunningInBackground, Launching.
This is the first xaml page that gets hit when the App is loaded. For sending location when app moves in background, the isRunninginBackground event is changed to call the MainPage`s send Location function. Here a bool flag is also set to true so that further the App gets to know that the App is running in background.

5.Utilities
This folder contains classes like Constanst, RelativeTimeConvertor, HttpServiceRequestClass for modularizing the code.

6.For Data Persistance, IsolatedStorsgeSettings is being used, which stores data in the form of key-value pair and has an Application scope.

7.Code Logic :
a. When the user selects the App from Phones Start Page, the App.xaml.cs is called, which fires its Application_Activated event.
This further takes the control to the very first page of the Application frame, here it is "MainPage.xaml,cs"
Next the MainPage.xaml.cs constructor is called and on its loading, its OnNavigatedTo event is fired.
In the OnNavigatedTo, It fetches the data stored in IsolatedStorageSettings.
i) If it is the first time, then this object would be null and default values would be printed to the screen.
Also the FetchLocation function is called to fetch the coordinates of device. After which the SendLocation functionality is called.
ii) If it is not the first time, the data fetched from IsolatedStorageSettings is popluated to the screen.
Also the fetchLocation function is called to check if new coordinates are present. For showing the last submission relative time, it first checks for the lastSubmittedDateTime in background, if that doesn`t provide a default value that means the App came from background and hence the relative time is calculated from this DateTime.

Further, if the user presses the start button, and the App goes into Background, first the pages OnNavigatedFrom event is fired which stores the current data displayed on screen to the IsolatedStorageSettings. Then it calls the SubmitLocation 
function.

Also while in the App, if the user clicks on submit button the SendLocation function is called.

The RelativeTimeUpdate String that we see below the Submit button, is calculated from difference of the submitTime, that was stored when a SendLocation was initiated, and the DateTime.Now. 



                
