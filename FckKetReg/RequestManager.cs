using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FckKetReg
{
    class RequestManager
    {
        private string userID = "";
        private string password = "";

        public string RegistrationPIN { get; set; }
        public string Term { get; set; }

        private CookiedWebClient webClient;

        private const string BASE_URL = "https://jweb.kettering.edu/cku1/";
        private const string LOGIN_URL = BASE_URL + "twbkwbis.P_WWWLogin";
        private const string VAL_URL = BASE_URL + "twbkwbis.P_ValLogin";
        private const string PIN_URL = BASE_URL + "bwskfreg.P_AltPin";
        private const string CHECK_PIN_URL = BASE_URL + "bwskfreg.P_CheckAltPin";

        public string Output { get; private set; }

        // Contain matches for errors.
        string[] knownErrors = {
                                   "Term not available for Registration processing.",
                                   "Invalid Alternate PIN."
                               };

        public RequestManager(string userID, string password)
        {
            Output = "Preparing requst system. \n";
            webClient = new CookiedWebClient();
            this.userID = userID;
            this.password = password;
        }

        public bool AttemptLogin() 
        {
            // Establish presense.
            webClient.DownloadString(LOGIN_URL);

            webClient.Headers.Add("Referer", LOGIN_URL); // Lie, just in case.

            NameValueCollection creds = new NameValueCollection
            {
                {"sid", userID},
                {"PIN", password}
            };
            
            byte[] valResponseByteArray = webClient.UploadValues(VAL_URL, creds);
            string valResponse = System.Text.Encoding.Default.GetString(valResponseByteArray);

            bool loginStatus = (valResponse.Contains("url=/cku1/twbkwbis.P_GenMenu"));
            if (!loginStatus) Output += "Login failed. \n";
            return loginStatus;
        }

        public bool AccessRegistrationPage()
        {
            // Pin page returns either term dropdown or 
            string pinPage;
            string altPinPageGet = webClient.DownloadString(PIN_URL);

            // If term selection arrives, pick correct term.
            if (altPinPageGet.Contains("Select a Term: "))
            {
                NameValueCollection termPost = new NameValueCollection { { "term_in", Term } };
                byte[] termResponse = webClient.UploadValues(PIN_URL, termPost);
                pinPage = System.Text.Encoding.Default.GetString(termResponse);
            }
            else
            {
                pinPage = altPinPageGet;
            }

            // Handle the Alt PIN page.
            string termError = findKnownError(pinPage);
            if (termError == null)
            {
                NameValueCollection pinPost = new NameValueCollection { { "pin", RegistrationPIN } };
                byte[] pinEnteredResponseByteArray = webClient.UploadValues(CHECK_PIN_URL, pinPost);
                string pinEnteredResponse = 
                    System.Text.Encoding.Default.GetString(pinEnteredResponseByteArray);
                Output += "PIN Page Response: \n" + pinEnteredResponse;
            }

            return false;
        }

        private string findKnownError(string check)
        {
            foreach(string error in knownErrors)
            {
                if(check.Contains(error))
                {
                    return error;
                }
            }

            return null;
        }
    }

    public class CookiedWebClient : WebClient
    {
        public CookieContainer cookieContainer = new CookieContainer();
        public Uri responseUri = null;

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest request = base.GetWebRequest(address);

            if (request is HttpWebRequest)
            {
                (request as HttpWebRequest).CookieContainer = cookieContainer;
                (request as HttpWebRequest).AllowAutoRedirect = true;
            }

            return request;
        }

        protected override WebResponse GetWebResponse(WebRequest request, IAsyncResult result)
        {
            WebResponse response = base.GetWebResponse(request, result);
            ReadCookies(response);
            responseUri = response.ResponseUri;
            return response;
        }

        protected override WebResponse GetWebResponse(WebRequest request)
        {
            WebResponse response = base.GetWebResponse(request);
            ReadCookies(response);
            responseUri = response.ResponseUri;
            return response;
        }

        private void ReadCookies(WebResponse r)
        {
            var response = r as HttpWebResponse;
            if (response != null)
            {
                CookieCollection cookies = response.Cookies;
                foreach (Cookie cookie in cookies) {
                    cookie.Path = String.Empty;
                }
                cookieContainer.Add(cookies);
            }
        }
    }
}
