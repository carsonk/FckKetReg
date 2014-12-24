using FckKetReg.Models;
using System;
using System.Collections.Specialized;

namespace FckKetReg
{
    /// <summary>
    /// Handles requests at a high level to JWEB.
    /// </summary>
    class RequestManager
    {
        private string userID = "";
        private string password = "";

        public string RegistrationPIN { get; set; }
        public Term CurrentTerm { get; set; }
       
        private CookiedWebClient webClient;

        private const string BASE_URL = "https://jweb.kettering.edu/cku1/";
        private const string LOGIN_URL = BASE_URL + "twbkwbis.P_WWWLogin";
        private const string VAL_URL = BASE_URL + "twbkwbis.P_ValLogin";
        private const string PIN_URL = BASE_URL + "bwskfreg.P_AltPin";
        private const string CHECK_PIN_URL = BASE_URL + "bwskfreg.P_CheckAltPin";

        public string Output { get; private set; }
        public string CurrentHTML { get; private set; }

        // Contain matches for errors.
        string[] knownErrors = {
                                   "Term not available for Registration processing.",
                                   "Invalid Alternate PIN."
                               };

        public RequestManager(string userID, string password)
        {
            LogToOutput("Preparing request system.");
            webClient = new CookiedWebClient();
            this.userID = userID;
            this.password = password;
            RegistrationPIN = "";
            CurrentTerm = new Term("201501");
        }

        /// <summary>
        /// Attempts login to JWEB.
        /// </summary>
        /// <returns>True if login succeeded.</returns>
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
            if (!loginStatus) LogToOutput("Login failed. Check credentials or connection.");
            else LogToOutput("Successfully logged in.");
            return loginStatus;
        }

        /// <summary>
        /// Accessor function 
        /// </summary>
        /// <returns></returns>
        public bool AccessRegistrationPage()
        {
            return AccessRegistrationPage(false);
        }

        /// <summary>
        /// Attempts to access Add/Drop Classes page.
        /// </summary>
        /// <param name="triedLogin">Whether or not login has already been attempted.
        /// Will loop back with false param if initial page access fails to prevent
        /// infinite login attempt loops.</param>
        /// <returns>True if page is reached.</returns>
        private bool AccessRegistrationPage(bool triedLogin)
        {
            // Pin page returns either term dropdown or 
            string pinPage;
            string altPinPageGet = webClient.DownloadString(PIN_URL);

            if (altPinPageGet.Contains(TERM_PAGE_SAMPLE))
            {
                LogToOutput("Selecting a term.");
                NameValueCollection termPost = new NameValueCollection { { "term_in", CurrentTerm.GetTermCode() } };
                byte[] termResponse = webClient.UploadValues(PIN_URL, termPost);
                pinPage = System.Text.Encoding.Default.GetString(termResponse);
            }
            else if (altPinPageGet.Contains(LOGIN_SAMPLE))
            {
                // Not logged in.
                // Give logging in a try.
                if(!triedLogin) {
                    LogToOutput("Not logged in. Attempting login and trying again.");
                    AttemptLogin();
                    return AccessRegistrationPage(true);
                }

                LogToOutput("Was not able to login.");
                return false;
            }
            else if (altPinPageGet.Contains(PIN_PAGE_SAMPLE))
            {
                // Went straight to pin page.
                pinPage = altPinPageGet;
            }
            else if(altPinPageGet.Contains(REG_PAGE_SAMPLE))
            {
                return true;
            }
            else
            {
                LogToOutput("Arrived at unknown page.");
                return false;
            }

            string termError = FindKnownError(pinPage);
            if (termError != null)
            {
                LogToOutput("Page returned error: " + termError);
                return false;
            }

            if (pinPage.Contains(PIN_PAGE_SAMPLE))
            {
                LogToOutput("Did not reach PIN page.");
                return false;
            }

            // Tries to pass registration PIN.
            LogToOutput("Passing entered PIN.");
            NameValueCollection pinPost = new NameValueCollection { { "pin", RegistrationPIN } };
            byte[] pinEnteredResponseByteArray = webClient.UploadValues(CHECK_PIN_URL, pinPost);
            string pinEnteredResponse = 
                System.Text.Encoding.Default.GetString(pinEnteredResponseByteArray);

            if (pinEnteredResponse.Contains(REG_PAGE_SAMPLE))
            {
                LogToOutput("Successfully reached Add/Drop Classes page.");
                return true;
            }

            string knownError = FindKnownError(pinEnteredResponse);
            if (knownError == null)
            {
                LogToOutput("No known error found.");
            }
            else
            {
                LogToOutput("Known error found: " + knownError);
            }

            return false;
        }

        public bool AddClasses()
        {

            return false;
        }

        private string FindKnownError(string check)
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

        private string LogToOutput(string toAdd)
        {
            Output += "[+]" + DateTime.Now + ": " + toAdd + "\n";
            return toAdd;
        }

        private const string LOGIN_SAMPLE = "Disclosure to unauthorized parties violates the";
        private const string PIN_PAGE_SAMPLE = "Please enter your Alternate Personal Identification Number (PIN) for verification, then click Login.";
        private const string TERM_PAGE_SAMPLE = "Select a Term:";
        private const string REG_PAGE_SAMPLE = "Use this interface to add or drop classes for the selected term.";
    }
}
