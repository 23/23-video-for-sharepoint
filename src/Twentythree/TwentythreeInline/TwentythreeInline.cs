using System;
using System.Runtime.InteropServices;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Serialization;

using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.WebPartPages;
using System.Web.UI.HtmlControls;
using System.ComponentModel;
using System.Collections.Specialized;

namespace Twentythree
{
    [Guid("f6c83006-86d0-4837-894f-1861d312014f")]
    public class TwentythreeInline : System.Web.UI.WebControls.WebParts.WebPart
    {
        private string _ConsumerDomain;
        [WebBrowsable(true),
         Personalizable(PersonalizationScope.Shared),
         WebDescription("23 video consumer domain"),
         Category("Settings"),
         WebDisplayName("23 video consumer domain")]
        public string ConsumerDomain
        {
            get { return _ConsumerDomain; }
            set { _ConsumerDomain = value; }
        }

        private string _ConsumerKey;
        [WebBrowsable(true),
         Personalizable(PersonalizationScope.Shared),
         WebDescription("23 video consumer key"),
         Category("Settings"),
         WebDisplayName("23 video consumer key")]
        public string ConsumerKey
        {
            get { return _ConsumerKey; }
            set { _ConsumerKey = value; }
        }


        private string _ConsumerSecret;
        [WebBrowsable(true),
         Personalizable(PersonalizationScope.Shared),
         WebDescription("23 video consumer secret"),
         Category("Settings"),
         WebDisplayName("23 video consumer secret")]
        public string ConsumerSecret
        {
            get { return _ConsumerSecret; }
            set { _ConsumerSecret = value; }
        }

        private string _AccessToken;
        [WebBrowsable(true),
         Personalizable(PersonalizationScope.Shared),
         WebDescription("23 video access token"),
         Category("Settings"),
         WebDisplayName("23 video access token")]
        public string AccessToken
        {
            get { return _AccessToken; }
            set { _AccessToken = value; }
        }

        private string _AccessTokenSecret;
        [WebBrowsable(true),
         Personalizable(PersonalizationScope.Shared),
         WebDescription("23 video access token secret"),
         Category("Settings"),
         WebDisplayName("23 video access token secret")]
        public string AccessTokenSecret
        {
            get { return _AccessTokenSecret; }
            set { _AccessTokenSecret = value; }
        }

        private string _InlineStyle;
        [WebBrowsable(true),
         Personalizable(PersonalizationScope.Shared),
         WebDescription("Inline CSS style"),
         Category("Frame appearance"),
         WebDisplayName("Inline CSS style")]
        public string InlineStyle
        {
            get { return _InlineStyle; }
            set { _InlineStyle = value; }
        }

        public TwentythreeInline()
        {
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            // Get the session token
            string IframeUri = "";

            if ((!String.IsNullOrEmpty(_ConsumerDomain)) &&
                (!String.IsNullOrEmpty(_ConsumerKey)) &&
                (!String.IsNullOrEmpty(_ConsumerSecret)) &&
                (!String.IsNullOrEmpty(_AccessToken)) &&
                (!String.IsNullOrEmpty(_AccessTokenSecret)))
            {
                // Parse any given 23_return_url path
                NameValueCollection Query = Page.Request.QueryString;

                var ReturnURL = "/";

                if (!String.IsNullOrEmpty(Query.Get("23_return_url")))
                {
                    ReturnURL = Query.Get("23_return_url");
                }

                Twentythree.IAPIProvider _APIProvider = new Twentythree.APIProvider(
                        _ConsumerDomain,
                        _ConsumerKey,
                        _ConsumerSecret,
                        _AccessToken,
                        _AccessTokenSecret
                    );

                Twentythree.SessionService _SessionService = new Twentythree.SessionService(_APIProvider);
                Twentythree.Domain.Session _Session = _SessionService.GetToken(ReturnURL);

                IframeUri = _Session.ReturnURL;
            }

            // Build the width and height
            string _Width = (String.IsNullOrEmpty(Width.ToString()) ? "100%" : Width.ToString());
            string _Height = (String.IsNullOrEmpty(Height.ToString()) ? "100%" : Height.ToString());

            // Create the container control
            HtmlGenericControl ContainerControl = new HtmlGenericControl("div");
            ContainerControl.Attributes["width"] = _Width;
            ContainerControl.Attributes["height"] = _Height;
            this.Controls.Add(ContainerControl);

            // Create the inline frame control
            HtmlGenericControl InlineFrameControl = new HtmlGenericControl("iframe");
            InlineFrameControl.Attributes["width"] = _Width;
            InlineFrameControl.Attributes["height"] = _Height;
            InlineFrameControl.Attributes["src"] = IframeUri;
            InlineFrameControl.Attributes["style"] = _InlineStyle;
            ContainerControl.Controls.Add(InlineFrameControl);
        }
    }
}
