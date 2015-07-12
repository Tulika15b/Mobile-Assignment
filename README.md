# Mobile-Assignment
Windows Phone 8 Location Updater(The App is also compatible with Windows Phone 8.1 devices)

Code Compilation Instructions :
Prerequisites : 1. Windows 8 System
                2. Visual Studio 2012 or above with Windows 8.0 SDK
                
For Compiling the code After you download the project as a zip file, unzip it and open the solution file .sln present in the folder.

ABOUT THE APP :

The App primarily fetches the phone current location, by making use of GeoLocator API of Windows Phone 8, and displays the location coordinates onto the screen.
For first time, App Activation : The coordinates are printed on the screen and also are sent to a server in the form of POST call using HttpWebRequest.
If the user presses the start button on phone or the App goes into the Background, App`s "RunningInBackground" event gets fired which makes the Http Call again.
Also while in the App, the user has an option to submit the phone`s location using a SUBMIT Button.

ABOUT THE CODE :

The code has been designed keeping in mind the MVVM design pattern for Windows Phone 8.

1. DataModel

The User class - Contains information like username, Location, isLastSubmitted check, LastSubmitted time string.
Location class - Contains the latitude and longitude as string. It is also a member of User class.

The classes make use of INotifyPropertyChanged which informs the controls, which bind to those properties, that the properties have been changed.

2. View
MainPage.xaml : It contains the screen layout of the App
MainPage.xaml.cs : This file contains the code behind of the screen i.e It has methods to bind the Location details, takes care of username text change, starts the submit functionality, Contains Page events like OnNavigatedTo, OnNavigatedFrom, saving persistant data(function called when page state  navigates from OnNavigatedFrom).

3. ViewModel
ViewModel class acts as an interface between the DataModel and View. It provides the View classes with the data,associated with the Model classes, required to populate them.






P.S : The work is still in progress

                
