using FckKetReg.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;

namespace FckKetReg
{
    /// <summary>
    /// Handles requests at a high level to JWEB.
    /// </summary>
    class RequestManager
    {
        private string userID = "";
        private string password = "";

        private string _registrationPIN;
        private Term _currentTerm;

        public Queue<string> CRNs { get; set; } // Class registration numbers.

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

        public RequestManager(string userID, string password, string registrationPIN, Term registeringForTerm)
        {
            LogToOutput("Preparing request system.");
            webClient = new CookiedWebClient();
            this.userID = userID;
            this.password = password;
            _registrationPIN = registrationPIN;
            _currentTerm = registeringForTerm;
            CRNs = new Queue<string>();
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
            CurrentHTML = valResponse;

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
                NameValueCollection termPost = new NameValueCollection { { "term_in", _currentTerm.GetTermCode() } };
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

            CurrentHTML = pinPage;

            string termError = FindKnownError(pinPage);
            if (termError != null)
            {
                LogToOutput("Page returned error: " + termError);
                return false;
            }

            if (!pinPage.Contains(PIN_PAGE_SAMPLE))
            {
                LogToOutput("Did not reach PIN page.");
                return false;
            }

            // Tries to pass registration PIN.
            LogToOutput("Passing entered PIN.");
            NameValueCollection pinPost = new NameValueCollection { { "pin", _registrationPIN } };
            byte[] pinEnteredResponseByteArray = webClient.UploadValues(CHECK_PIN_URL, pinPost);
            string pinEnteredResponse = 
                System.Text.Encoding.Default.GetString(pinEnteredResponseByteArray);
            CurrentHTML = pinEnteredResponse;

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
            return AddClasses(false);
        }

        /// <summary>
        /// Registers for classes.
        /// </summary>
        /// <param name="triedLogin">Whether or not login has already been tried.</param>
        /// <returns>True if registration succeeded.</returns>
        public bool AddClasses(bool triedLogin)
        {
            string regPage = webClient.DownloadString(PIN_URL);

            if (regPage.Contains(REG_PAGE_SAMPLE))
            {
                string regPostData = "term_in=" + _currentTerm.GetTermCode();
                foreach(string number in CRNs)
                {
                    regPostData += "&RSTS_IN=RW&CRN_IN=" + number;
                    regPostData += "&assoc_term_in=&start_date_in=&end_date_in=";
                }
                regPostData += "&regs_row=0&wait_row=0&add_row=10&REG_BTN=Submit Changes";
                regPostData = HttpUtility.UrlEncode(regPostData);
                webClient.UploadString(PIN_URL, regPostData);
            }
            else if(regPage.Contains(LOGIN_SAMPLE))
            {
                // Tries going through login and stuff again.
                if(AccessRegistrationPage() && triedLogin == false)
                {
                    LogToOutput("Failed to reach registration page. Trying login and creds.");
                    return AddClasses(true);
                } else
                {
                    LogToOutput("Failed to reach registration page again. Quitting.");
                }
            }

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
