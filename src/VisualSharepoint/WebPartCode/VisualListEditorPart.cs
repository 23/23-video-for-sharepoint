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
    public class VisualListEditorPart : System.Web.UI.WebControls.WebParts.EditorPart
    {
        protected Panel EditorPanel;

        protected Panel CountPanel;
        protected TextBox Count;

        protected Panel ChannelsPanel;
        protected DropDownList Channels;

        protected Panel TagsPanel;
        protected ListBox Tags;

        protected Panel OrderPanel;
        protected DropDownList Order;

        private List<Domain.Tag> GetTags(IApiProvider apiProvider)
        {
            List<Domain.Tag> result = new List<Domain.Tag>();
            ITagService tagService = new TagService(apiProvider);

            bool done = false;
            int page = 1;
            while (!done)
            {
                List<Domain.Tag> tags = tagService.GetList(new TagListParameters
                {
                    OrderBy = TagListSort.Tag,
                    Order = GenericSort.Ascending,
                    Size = 100,
                    PageOffset = page++,
                    ExcludeMachineTags = false,
                    ReformatTags = false
                });

                if (tags.Count > 0)
                    result.AddRange(tags);
                if (tags.Count < 100)
                    done = true;
            }

            return result;
        }

        protected override void CreateChildControls()
        {
            // Define a readable title
            this.Title = "Video list";

            // Add the containing panel
            EditorPanel = new Panel();
            EditorPanel.CssClass = "ms-ToolPartSpacing";
            this.Controls.Add(EditorPanel);
        
            // Check that we actually have everything configured
            IApiProvider apiProvider = Utilities.ApiProvider;

            if (apiProvider != null)
            {
                // * Count

                CountPanel = new Panel();
                CountPanel.Controls.Add(new LiteralControl("<div class=\"UserSectionHead\">Number of videos</div>"));
                CountPanel.Controls.Add(new LiteralControl("<div class=\"UserSectionBody\"><div class=\"UserControlGroup\"><nobr>"));
                Count = new TextBox();
                Count.CssClass = "UserInput";
                Count.Width = new Unit("50px", CultureInfo.InvariantCulture);

                // Finish it up
                Count.AutoPostBack = false;
                CountPanel.Controls.Add(Count);
                CountPanel.Controls.Add(new LiteralControl("</nobr></div></div>"));

                EditorPanel.Controls.Add(CountPanel);

                // * Album

                // Add the album selection panel
                ChannelsPanel = new Panel();
                ChannelsPanel.Controls.Add(new LiteralControl("<div class=\"UserSectionHead\">Channel</div>"));
                ChannelsPanel.Controls.Add(new LiteralControl("<div class=\"UserSectionBody\"><div class=\"UserControlGroup\"><nobr>"));
                Channels = new DropDownList();
                Channels.CssClass = "UserInput";

                // Get a list of videos to throw in there
                IAlbumService albumService = new AlbumService(apiProvider);
                List<Domain.Album> albums = albumService.GetList(new AlbumListParameters
                {
                    IncludeHidden = false,
                    Size = 500,
                    OrderBy = AlbumListSort.Title,
                    Order = GenericSort.Ascending
                });

                Channels.Items.Add(new ListItem("All channels", "-"));

                foreach (Domain.Album album in albums)
                {
                    if (album.AlbumId != null) Channels.Items.Add(new ListItem(String.IsNullOrEmpty(album.Title) ? "(no title)" : album.Title, album.AlbumId.Value.ToString()));
                }

                // Finish it up
                Channels.AutoPostBack = false;
                ChannelsPanel.Controls.Add(Channels);
                ChannelsPanel.Controls.Add(new LiteralControl("</nobr></div></div>"));

                EditorPanel.Controls.Add(ChannelsPanel);

                // * Tags
                TagsPanel = new Panel();

                // Description
                TagsPanel.Controls.Add(new LiteralControl("<div class=\"UserSectionHead\">Tags</div>"));
                TagsPanel.Controls.Add(new LiteralControl("<div class=\"UserSectionBody\"><div class=\"UserControlGroup\"><nobr>"));

                // Available
                Tags = new ListBox();
                Tags.CssClass = "UserInput";
                Tags.Width = new Unit("176px", CultureInfo.InvariantCulture);
                Tags.Height = new Unit("400px", CultureInfo.InvariantCulture);
                Tags.SelectionMode = ListSelectionMode.Multiple;

                List<Domain.Tag> tags = GetTags(apiProvider);

                foreach (Domain.Tag tag in tags)
                {
                    Tags.Items.Add(tag.Name);
                }

                TagsPanel.Controls.Add(Tags);

                TagsPanel.Controls.Add(new LiteralControl("</nobr></div></div>"));
                EditorPanel.Controls.Add(TagsPanel);

                // * Order

                OrderPanel = new Panel();
                OrderPanel.Controls.Add(new LiteralControl("<div class=\"UserSectionHead\">Sorting</div>"));
                OrderPanel.Controls.Add(new LiteralControl("<div class=\"UserSectionBody\"><div class=\"UserControlGroup\"><nobr>"));
                Order = new DropDownList();
                Order.CssClass = "UserInput";

                Order.Items.Add(new ListItem("Most recently uploaded first", "UploadedAscending"));
                Order.Items.Add(new ListItem("Most recently uploaded last", "UploadedDescending"));
                Order.Items.Add(new ListItem("Most views first", "ViewsAscending"));
                Order.Items.Add(new ListItem("Least views first", "ViewsDescending"));
                Order.Items.Add(new ListItem("Most recently published first", "PublishedDescending"));
                Order.Items.Add(new ListItem("Most recently published last", "PublishedAscending"));

                Order.AutoPostBack = false;
                OrderPanel.Controls.Add(Order);
                OrderPanel.Controls.Add(new LiteralControl("</nobr></div></div>"));

                EditorPanel.Controls.Add(OrderPanel);
            }
            
            base.CreateChildControls();
            this.ChildControlsCreated = true;
        }

        public override void SyncChanges()
        {
            // Make sure that all is set up
            EnsureChildControls();

            // Update the editor
            VisualList webPart = (VisualList)WebPartToEdit;
            if (webPart != null)
            {
                Count.Text = webPart.Count.ToString();
                
                if ((!String.IsNullOrEmpty(webPart.AlbumId)) && (Channels.Items.FindByValue(webPart.AlbumId) != null))
                    Channels.SelectedValue = webPart.AlbumId;
                else Channels.SelectedValue = "-";

                if (webPart.Tags != null)
                {
                    foreach (string tag in webPart.Tags)
                    {
                        ListItem listItem = Tags.Items.FindByValue(tag);
                        if (listItem != null) listItem.Selected = true;
                    }
                }

                Order.SelectedValue = webPart.Order;
            }
            
            return;
        }
        public override bool ApplyChanges()
        {
            // Make sure that all is set up
            EnsureChildControls();

            // Update the web part
            VisualList webPart = (VisualList)WebPartToEdit;
            if (webPart != null)
            {
                // Count
                webPart.Count = Convert.ToInt32(Count.Text);

                // Album
                webPart.AlbumId = ((String.IsNullOrEmpty(Channels.SelectedValue)) || (Channels.SelectedValue == "-") ? null : Channels.SelectedValue);

                // Tags
                List<string> tags = new List<string>();

                foreach (ListItem tag in Tags.Items)
                {
                    if (tag.Selected) tags.Add(tag.Value);
                }

                webPart.Tags = tags;

                webPart.Order = Order.SelectedValue;
            }

            return true;
        }
    }
}