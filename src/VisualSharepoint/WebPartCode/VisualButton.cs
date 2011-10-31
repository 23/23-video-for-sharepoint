using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls.WebParts;

namespace Visual.Sharepoint
{
    [Guid("f38c22b6-121b-47cb-b44a-87de779b2cbc")]
    public class VisualButton : Microsoft.SharePoint.WebPartPages.WebPart
    {
        private bool _error = false;

        private string _buttonValue;
        [WebBrowsable(true),
         Personalizable(PersonalizationScope.Shared),
         WebDescription("Launch button value"),
         Category("Settings"),
         WebDisplayName("Launch button value")]
        public string ButtonValue
        {
            get { return _buttonValue; }
            set { _buttonValue = value; }
        }

        public VisualButton()
        {
            this.ExportMode = WebPartExportMode.All;
        }

        /// <summary>
        /// Create all your controls here for rendering.
        /// Try to avoid using the RenderWebPart() method.
        /// </summary>
        protected override void CreateChildControls()
        {
            if (!_error)
            {
                try
                {
                    base.CreateChildControls();

                    // Get the session token
                    ISessionService sessionService = new SessionService(Utilities.ApiProvider);
                    Domain.Session session = sessionService.GetToken("/");

                    string buttonUri = session.ReturnURL;

                    // Create the container control
                    HtmlGenericControl containerControl = new HtmlGenericControl("div");
                    this.Controls.Add(containerControl);

                    // Create the button control
                    HtmlGenericControl buttonControl = new HtmlGenericControl("input");
                    buttonControl.Attributes["type"] = "button";
                    buttonControl.Attributes["onclick"] = "javascript:location.href='" + buttonUri + "';";
                    buttonControl.Attributes["value"] = (!String.IsNullOrEmpty(_buttonValue) ? _buttonValue.Replace("\"", "&quot;") : "Launch");
                    containerControl.Controls.Add(buttonControl);
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }
            }
        }

        /// <summary>
        /// Ensures that the CreateChildControls() is called before events.
        /// Use CreateChildControls() to create your controls.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            if (!_error)
            {
                try
                {
                    base.OnLoad(e);
                    this.EnsureChildControls();

                    // Your code here...
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }
            }
        }

        /// <summary>
        /// Clear all child controls and add an error message for display.
        /// </summary>
        /// <param name="ex"></param>
        private void HandleException(Exception ex)
        {
            this._error = true;
            this.Controls.Clear();
            this.Controls.Add(new LiteralControl(ex.Message));
        }
    }
}
