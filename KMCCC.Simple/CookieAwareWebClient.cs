using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace KMCCC.Simple
{
    public class CookieAwareWebClient : WebClient
    {
        private CookieContainer cc = new CookieContainer();
        private string lastPage;

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest webRequest = base.GetWebRequest(address);
            if (webRequest is HttpWebRequest)
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)webRequest;
                if (CookieSave.cc == null)
                {
                    httpWebRequest.CookieContainer = this.cc;
                    CookieSave.cc = this.cc;
                }
                else
                {
                    httpWebRequest.CookieContainer = CookieSave.cc;
                }
                if (this.lastPage != null)
                {
                    httpWebRequest.Referer = this.lastPage;
                }
            }
            this.lastPage = address.ToString();
            return webRequest;
        }
    }

    public class CookieSave
    {
        public static CookieContainer cc;
    }
}
