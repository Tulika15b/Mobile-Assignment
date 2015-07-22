using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace LocationFinderApp.Utilities
{
     public class HttpServiceRequestClass
    {
        public HttpWebRequest createHttpRequest(string baseURL)
        {

            HttpWebRequest clientReq = WebRequest.CreateHttp(new Uri(baseURL));

            clientReq.AllowReadStreamBuffering = false;
            clientReq.ContentType = "application/x-www-form-urlencoded";
            clientReq.Method = "POST";
            NetworkCredential cred = new NetworkCredential(Constants.CREDS_USERNAME, Constants.CREDS_PASSWORD);
            clientReq.Credentials = cred;

            return clientReq;
        }

        public async Task<HttpWebRequest> sendHttpRequest(IAsyncResult asynchronousResult, string postData)
        {

            HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;

                // End the operation
                Stream postStream = request.EndGetRequestStream(asynchronousResult);

                // Convert the string into a byte array. 
                byte[] byteArray = Encoding.UTF8.GetBytes(postData);

                // Write to the request stream.

                await postStream.WriteAsync(byteArray, 0, postData.Length);
                postStream.Close();
                
            
           

            return request;
        }

        public async Task<string> receiveHttpResonse(HttpWebRequest request, IAsyncResult asynchronousResult)
        {
            string responseString = String.Empty;
            HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(asynchronousResult);
            Stream streamResponse = response.GetResponseStream();
            StreamReader streamRead = new StreamReader(streamResponse);
            responseString = await streamRead.ReadToEndAsync();

            //var userResp = JsonConvert.DeserializeObject<T>(responseString);
            // Close the stream object

            streamRead.Close();
            streamResponse.Close();
            // Release the HttpWebResponse
            response.Close();

            HttpStatusCode responseCode = response.StatusCode;
            if (responseCode == HttpStatusCode.Created)
            {
                return Constants.SUCCESS;
            }
            else
            {
                return Constants.ERROR;
            }

        }

    }
}
