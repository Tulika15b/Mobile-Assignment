# Mobile-Assignment
Windows Phone 8 Location Updater

Code Compilation Instructions :
Prerequisites : 1. Windows 8 System
                2. Visual Studio 2012 or above with Windows 8.0 SDK
                
For Compiling the code After you download the project as a zip file, unzip it and open the solution file .sln present in the folder.

The App primarily fetches the user`s current location, by making use of GeoLocator API of Windows Phone 8, and displays the location coordinates onto the screen.
For first time, App Activation : The coordinates are printed on the screen and also are sent to a server in the form of POST call using HttpWebRequest.
If the user presses the start button on phone or the App goes into the Background, App`s "RunningInBackground" event gets fired which makes the Http Call again.
Also while in the App, the user has an option to submit the phone`s location using a SUBMIT Button.





P.S : The work is still in progress

                
