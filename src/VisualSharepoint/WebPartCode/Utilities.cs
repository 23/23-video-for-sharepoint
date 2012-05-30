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
            string domain = Configuration.Domain;
            string widthString = width.ToString();
            string heightString = (height == null ? Math.Round(width / 16.0 * 9.0).ToString() : height.Value.ToString());

            return "<iframe src=\"http://" + domain + "/v.ihtml?token=" + photoToken + "&photo%5fid=" + photoId + "\" width=\"" + widthString + "\" height=\"" + heightString + "\" frameborder=\"0\" border=\"0\" scrolling=\"no\"></iframe>";
        }
    }
}
