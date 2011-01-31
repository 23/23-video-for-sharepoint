using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using System.Configuration;

namespace Visual.Sharepoint
{
    [Guid("a4d167a2-80fa-4700-9559-2b8e861e6b56")]
    public class VisualEmbed : VisualWebPart, IWebEditable
    {
        private bool _error = false;
        private string _photoId = null;
        private string _photoToken = null;

        [Personalizable(PersonalizationScope.Shared)]
        public string PhotoId
        {
            get { return _photoId; }
            set { _photoId = value; }
        }

        [Personalizable(PersonalizationScope.Shared)]
        public string PhotoToken
        {
            get { return _photoToken; }
            set { _photoToken = value; }
        }

        /// <summary>
        /// Time to draw!
        /// </summary>
        protected override void CreateChildControls()
        {
            if (!_error)
            {
                try
                {
                    // Determine the width and height
                    int width = ((String.IsNullOrEmpty(Width) || (Width.Substring(Width.Length - 2, 2) != "px")) ? 640 : Convert.ToInt32(Width.Substring(0, Width.Length - 2)));
                    int? height = null;
                    if ((!String.IsNullOrEmpty(Height)) && (Width.Substring(Width.Length - 2, 2) == "px")) height = Convert.ToInt32(Height.Substring(0, Height.Length - 2));

                    // Build the embed code
                    this.Controls.Add(new LiteralControl(Utilities.EmbedCode(PhotoId, PhotoToken, width, height)));
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

        /// <summary>
        /// Return the custom editor for selecting videos
        /// </summary>
        /// <returns>EditorPartCollection containing the VisualEmbedEditorPart</returns>
        EditorPartCollection IWebEditable.CreateEditorParts()
        {
            List<EditorPart> editors = new List<EditorPart>();
            VisualEmbedEditorPart editorPart = new VisualEmbedEditorPart();
            editorPart.ID = this.ID + "_editorPart";
            editors.Add(editorPart);
            return new EditorPartCollection(editors);
        }

        object IWebEditable.WebBrowsableObject
        {
            get { return this; }
        }
    }
}
