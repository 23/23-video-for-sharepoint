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
    [Guid("f1982186-5a9d-4233-a120-c9df5d0133d0")]
    public class VisualInline : Microsoft.SharePoint.WebPartPages.WebPart
    {
        private bool _error = false;

        private string _inlineStyle;
        [WebBrowsable(true),
         Personalizable(PersonalizationScope.Shared),
         WebDescription("Inline CSS style"),
         Category("Frame appearance"),
         WebDisplayName("Inline CSS style")]
        public string InlineStyle
        {
            get { return _inlineStyle; }
            set { _inlineStyle = value; }
        }

        public VisualInline()
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

                    // Parse any given 23_return_url path
                    NameValueCollection query = Page.Request.QueryString;

                    var returnUrl = "/";

                    if (!String.IsNullOrEmpty(query.Get("23_return_url")))
                    {
                        returnUrl = query.Get("23_return_url");
                    }

                    // Get the session token
                    ISessionService sessionService = new SessionService(Utilities.ApiProvider);
                    Domain.Session session = sessionService.GetToken(returnUrl);

                    string iframeUri = session.ReturnURL;

                    // Build the width and height
                    string width = (String.IsNullOrEmpty(Width.ToString()) ? "100%" : Width.ToString());
                    string height = (String.IsNullOrEmpty(Height.ToString()) ? "100%" : Height.ToString());

                    // Create the container control
                    HtmlGenericControl ContainerControl = new HtmlGenericControl("div");
                    ContainerControl.Attributes["width"] = width;
                    ContainerControl.Attributes["height"] = height;
                    this.Controls.Add(ContainerControl);

                    // Create the inline frame control
                    HtmlGenericControl InlineFrameControl = new HtmlGenericControl("iframe");
                    InlineFrameControl.Attributes["width"] = width;
                    InlineFrameControl.Attributes["height"] = height;
                    InlineFrameControl.Attributes["src"] = iframeUri;
                    InlineFrameControl.Attributes["style"] = _inlineStyle;
                    InlineFrameControl.Attributes["frameborder"] = "0";
                    ContainerControl.Controls.Add(InlineFrameControl);
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
