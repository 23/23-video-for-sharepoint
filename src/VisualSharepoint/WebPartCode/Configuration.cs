using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using System.Configuration;
using Visual;
using Microsoft.SharePoint;

namespace Visual.Sharepoint
{
    public static class Configuration
    {
        private static string GetAppSetting(string identifier)
        {
            try
            {
                string result = ConfigurationSettings.AppSettings[identifier];
                if (String.IsNullOrEmpty(result)) throw new Exception("Configuration value ´" + identifier + "´ not set up in web.config");
                return result;
            }
            catch
            {
                throw new Exception("Configuration value ´" + identifier + "´ not set up in web.config");
            }
        }

        private static bool GetAppSettingBool(string identifier)
        {
            try
            {
                string result = ConfigurationSettings.AppSettings[identifier];
                if (String.IsNullOrEmpty(result)) return false;
                return bool.Parse(result);
            }
            catch
            {
                return false;
            }
        }

        public static string Domain
        {
            get { return GetAppSetting("TwentythreeDomain"); }
            private set { }
        }

        public static string ConsumerKey
        {
            get { return GetAppSetting("TwentythreeConsumerKey"); }
            private set { }
        }

        public static string ConsumerSecret
        {
            get { return GetAppSetting("TwentythreeConsumerSecret"); }
            private set { }
        }

        public static string AccessToken
        {
            get { return GetAppSetting("TwentythreeAccessToken"); }
            private set { }
        }

        public static string AccessTokenSecret
        {
            get { return GetAppSetting("TwentythreeAccessTokenSecret"); }
            private set { }
        }

        public static bool HttpSecure
        {
            get { return GetAppSettingBool("TwentythreeHttpSecure"); }
            private set { }
        }
    }
}
