using System;
using System.Collections.Generic;

using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Globalization;

namespace Visual.Sharepoint
{
    /// <summary>
    /// Editor controller for the embed webpart
    /// </summary>
    public class VisualEmbedEditorPart : System.Web.UI.WebControls.WebParts.EditorPart
    {
        protected Panel EditorPanel;

        // Web part specific controls
        protected DropDownList Videos;
        protected Panel VideoPanel;

        protected override void CreateChildControls()
        {
            // Define a readable title
            this.Title = "Embed";

            // Add the containing panel
            EditorPanel = new Panel();
            EditorPanel.CssClass = "ms-ToolPartSpacing";
            this.Controls.Add(EditorPanel);
        
            // Check that we actually have everything configured
            IApiProvider apiProvider = Utilities.ApiProvider;

            if (apiProvider != null)
            {
                // Add the video selection panel
                VideoPanel = new Panel();
                VideoPanel.Controls.Add(new LiteralControl("<div class=\"UserSectionHead\">Video to embed</div>"));
                VideoPanel.Controls.Add(new LiteralControl("<div class=\"UserSectionBody\"><div class=\"UserControlGroup\"><nobr>"));
                Videos = new DropDownList();
                Videos.CssClass = "UserInput";

                // Get a list of videos to throw in there
                IPhotoService photoService = new PhotoService(apiProvider);
                List<Domain.Photo> photos = photoService.GetList(new PhotoListParameters
                {
                    IncludeUnpublished = false,
                    Size = 200
                });

                foreach (Domain.Photo photo in photos)
                {
                    if (photo.PhotoId != null) Videos.Items.Add(new ListItem(String.IsNullOrEmpty(photo.Title) ? "(no title)" : photo.Title, photo.PhotoId.Value.ToString()));
                }

                // Finish it up
                Videos.AutoPostBack = false;
                VideoPanel.Controls.Add(Videos);
                VideoPanel.Controls.Add(new LiteralControl("</nobr></div></div>"));

                EditorPanel.Controls.Add(VideoPanel);
            }

            base.CreateChildControls();
            this.ChildControlsCreated = true;
        }

        public override void SyncChanges()
        {
            // Make sure that all is set up
            EnsureChildControls();

            // Update the editor
            VisualEmbed webPart = (VisualEmbed)WebPartToEdit;
            if (webPart != null)
            {
                if (Videos != null) Videos.SelectedValue = webPart.PhotoId;
            }
            
            return;
        }
        public override bool ApplyChanges()
        {
            // Make sure that all is set up
            EnsureChildControls();

            // Update the web part
            VisualEmbed webPart = (VisualEmbed)WebPartToEdit;
            if (webPart != null)
            {
                // Get the token for the photo first
                if (Videos != null)
                {
                    IPhotoService photoService = new PhotoService(Utilities.ApiProvider);
                    Domain.Photo photo = photoService.Get(Convert.ToInt32(Videos.SelectedValue));

                    webPart.PhotoId = Videos.SelectedValue;
                    webPart.PhotoToken = photo.Token;
                }
            }

            return true;
        }
    }
}