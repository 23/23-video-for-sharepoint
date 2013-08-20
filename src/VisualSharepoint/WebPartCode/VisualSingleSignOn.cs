using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using Visual.Domain;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Web.UI.HtmlControls;

namespace Visual.Sharepoint
{
    [Guid("8C53E1C7-2C2F-4124-B7FE-3D7A1301B688")]
    public class VisualSingleSignOn : Microsoft.SharePoint.WebPartPages.WebPart
    {
        private bool _sendEmail;
        private bool _sendFullName;
        private string _redirectText;
        private Domain.Session _session;

        [WebBrowsable(true),
         DefaultValue(false),
         Personalizable(PersonalizationScope.Shared),
         WebDescription("Send the user e-mail with the request"),
         Category("Settings"),
         WebDisplayName("Send the user e-mail with the request")]
        public bool SendEmail
        {
            get { return _sendEmail; }
            set { _sendEmail = value; }
        }

        [WebBrowsable(true),
         DefaultValue(false),
         Personalizable(PersonalizationScope.Shared),
         WebDescription("Send the user full name with the request"),
         Category("Settings"),
         WebDisplayName("Send the user full name with the request")]
        public bool SendFullname
        {
            get { return _sendFullName; }
            set { _sendFullName = value; }
        }

        [WebBrowsable(true),
         DefaultValue("Go to video site"),
         Personalizable(PersonalizationScope.Shared),
         WebDescription("Redirect Button text"),
         Category("Settings"),
         WebDisplayName("Redirect Button text")]
        public string RedirectText
        {
            get { return _redirectText; }
            set { _redirectText = value; }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            string debug = Context.Request.QueryString["debug"];

            if (WebPartManager.DisplayMode == WebPartManager.EditDisplayMode)
                debug = "yes";

            IApiProvider apiProvider = Utilities.ApiProvider;
            _session = null;
            string return_url = Context.Request.QueryString["return_url"];
            if (return_url == null)
            {
                return_url = "/";
                debug = "yes"; // Do not instant redirect
            }
            else
            {
                return_url = HttpUtility.UrlDecode(return_url);
            }

            if (apiProvider != null)
            {
                SPUser user = SPContext.Current.Web.CurrentUser;
                string email = null;
                string fullname = null;

                if (user != null)
                {
                    email = _sendEmail ? user.Email : null;
                    fullname = _sendFullName ? user.Name : null;
                }

                ISessionService sessionService = new SessionService(apiProvider);
                _session = sessionService.GetToken(return_url, email, fullname);
            }

            // Do not redirect if debug is set
            if (debug == null)
            {
                if (_session != null)
                {
                    SPUtility.Redirect(_session.ReturnURL, SPRedirectFlags.Default, Context);
                }
            }
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            HtmlGenericControl button = new HtmlGenericControl("input");
            button.Attributes["type"] = "button";
            button.Attributes["onclick"] = "javascript:location.href='" + _session.ReturnURL + "';";
            button.Attributes["value"] = (!String.IsNullOrEmpty(RedirectText) ? RedirectText.Replace("\"", "&quot;") : "Go to video site");
            this.Controls.Add(button);
        }
    }
}
