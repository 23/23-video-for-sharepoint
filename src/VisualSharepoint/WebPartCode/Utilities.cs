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
    public static class Utilities
    {
        public static IApiProvider ApiProvider
        {
            get
            {
                return new ApiProvider(Configuration.Domain,
                                       Configuration.ConsumerKey,
                                       Configuration.ConsumerSecret,
                                       Configuration.AccessToken,
                                       Configuration.AccessTokenSecret);
            }
            private set { }
        }
        
        /// <summary>
        /// Creates an embed code. Defaults aspect ratio to 16:9
        /// </summary>
        public static string EmbedCode(string photoId, string photoToken, int width, int? height)
        {
            string domain = ConfigurationSettings.AppSettings["TwentythreeDomain"];
            string widthString = width.ToString();
            string heightString = (height == null ? Math.Round(width / 16.0 * 9.0).ToString() : height.Value.ToString());

            string result = "<script src=\"http://" + domain + "/resources/um/script/swfobject/swfobject.js\"></script>" +
                            "<div id=\"visual-" + photoId + "\" class=\"embedded-video\"><div class=\"no-flash\"><iframe src=\"http://" + domain + "/" + photoId + ".html?token=" + photoToken + "&photo%5fid=" + photoId + "\" width=\"" + widthString + "\" height=\"" + heightString + "\" frameborder=\"0\" border=\"0\" scrolling=\"no\"></iframe></div></div>" +
                            "<script>swfobject.embedSWF(\"http://" + domain + "/" + photoId + ".swf\", \"visual-" + photoId + "\", \"" + widthString + "\", \"" + heightString + "\", \"9.0.0\", \"/resources/um/script/swfobject/expressInstall.swf\", {\"photo_id\": \"" + photoId + "\", \"token\": \"" + photoToken + "\"}, {allowscriptaccess:'always', allowfullscreen:'true'}, {id:'visual-" + photoId + "', name:'visual-" + photoId + "'});</script>";

            return result;
        }
    }
}
