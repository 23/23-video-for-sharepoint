using System;
using System.Runtime.InteropServices;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Serialization;

using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.WebPartPages;
using System.ComponentModel;
using System.Web.UI.HtmlControls;
using System.Collections.Specialized;

namespace Twentythree
{
    [Guid("1bfbf8cc-7cdb-4a4e-a26e-c29a0499bb5e")]
    public class TwentythreeButton : System.Web.UI.WebControls.WebParts.WebPart
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

        private string _ButtonValue;
        [WebBrowsable(true),
         Personalizable(PersonalizationScope.Shared),
         WebDescription("Launch button value"),
         Category("Settings"),
         WebDisplayName("Launch button value")]
        public string ButtonValue
        {
            get { return _ButtonValue; }
            set { _ButtonValue = value; }
        }

        public TwentythreeButton()
        {
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            // Get the session token
            string ButtonUri = "";

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

                ButtonUri = _Session.ReturnURL;
            }

            // Create the container control
            HtmlGenericControl ContainerControl = new HtmlGenericControl("div");
            this.Controls.Add(ContainerControl);

            // Create the button control
            HtmlGenericControl ButtonControl = new HtmlGenericControl("input");
            ButtonControl.Attributes["type"] = "button";
            ButtonControl.Attributes["onclick"] = "javascript:location.href='" + ButtonUri + "';";
            ButtonControl.Attributes["value"] = (!String.IsNullOrEmpty(_ButtonValue) ? _ButtonValue.Replace("\"", "&quot;") : "Launch");
            ContainerControl.Controls.Add(ButtonControl);
        }
    }
}
