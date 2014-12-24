using System;
using System.Net;

namespace FckKetReg
{
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
                foreach (Cookie cookie in cookies)
                {
                    cookie.Path = String.Empty;
                }
                cookieContainer.Add(cookies);
            }
        }
    }
}
