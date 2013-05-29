using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using System.Configuration;
using Visual;
using Microsoft.SharePoint;
using Visual.Domain;

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
                                       Configuration.AccessTokenSecret,
                                       Configuration.HttpSecure);
            }
            private set { }
        }
        
        /// <summary>
        /// Creates an embed code. Defaults aspect ratio to 16:9
        /// </summary>
        public static string EmbedCode(string photoId, string photoToken, int width, int? height, bool autoPlay)
        {
            string domain = Configuration.Domain;
            bool httpsOnly = Configuration.HttpSecure;
            string widthString = width.ToString();
            string heightString = (height == null ? Math.Round(width / 16.0 * 9.0).ToString() : height.Value.ToString());
            string protocol = httpsOnly ? "https" : "http";
            string apText = autoPlay ? "&autoPlay=1" : "";
            return "<iframe src=\"" + protocol + "://" + domain + "/v.ihtml?token=" + photoToken + "&photo%5fid=" + photoId + apText + "\" width=\"" + widthString + "\" height=\"" + heightString + "\" frameborder=\"0\" border=\"0\" scrolling=\"no\"></iframe>";
        }

        public static PhotoBlock GetVideoSize(Domain.Photo photo, VideoSize size)
        {
            switch (size)
            {
                default:
                case VideoSize.Small:
                    return photo.Small;

                case VideoSize.Medium:
                    return photo.Medium;

                case VideoSize.Standard:
                    return photo.Standard;

                case VideoSize.Large:
                    return photo.Large;
            }
        }
    }
}
