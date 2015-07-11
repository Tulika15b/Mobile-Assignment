using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocationFinderApp.Model;
using System.IO;
using System.IO.IsolatedStorage;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace LocationFinderApp.Utilities
{
    public class PersistantDataStorage
    {
        public void saveUserDetails(User user)
        {        
            XmlWriterSettings x_W_Settings = new XmlWriterSettings();
            x_W_Settings.Indent = true;
            using (IsolatedStorageFile ISF = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!ISF.FileExists("userDetails.xml"))
                {

                    using (IsolatedStorageFileStream stream = ISF.OpenFile("userDetails.xml", FileMode.CreateNew))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(User));
                        using (XmlWriter xmlWriter = XmlWriter.Create(stream, x_W_Settings))
                        {

                            serializer.Serialize(xmlWriter, user);

                        }
                        stream.Close();
                    }

                }
                else
                {                    
                    XDocument loadedData;

                    if (user != null)
                    {
                        User usr = user;
                        using (Stream stream = ISF.OpenFile("userDetails.xml", FileMode.Open, FileAccess.ReadWrite))
                        {

                            loadedData = XDocument.Load(stream);

                            loadedData.Descendants("User").Single().SetElementValue("UserId", usr.userName);
                            loadedData.Descendants("User").Single().SetElementValue("Latitude", usr.location.Latitude);
                            loadedData.Descendants("User").Single().SetElementValue("Longitude", usr.location.Longitude);
                            loadedData.Descendants("User").Single().SetElementValue("LastUpdatedOn", usr.LastSubmittedDateTime);

                            stream.Close();
                           
                        }

                        // Save To ISOconfig.xml File 
                        using (IsolatedStorageFileStream newStream = new IsolatedStorageFileStream("userDetails.xml", FileMode.Create, ISF))
                        {
                            loadedData.Save(newStream);
                            newStream.Close();
                        }

                        
                    }

                }
            }
        }


        public User readUserDetails()
        {
            User newUser = null;
            using (IsolatedStorageFile ISF = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (ISF.FileExists("userDetails.xml"))
                {
                    newUser = new User();
                    using (IsolatedStorageFileStream str = ISF.OpenFile("userDetails.xml", FileMode.Open))
                    {

                        XmlSerializer serializer = new XmlSerializer(typeof(User));
                        newUser = (User)serializer.Deserialize(str);
                        str.Close();
                    }
                }
            }

            return newUser;
        }

        public void deleteUserDetails(string UserId)
        {
            using (IsolatedStorageFile isoStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (isoStorage.FileExists("userDetails.xml"))
                {
                    isoStorage.DeleteFile("userDetails.xml");
                }
              
            }
        }
    }
}
