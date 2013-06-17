using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using Visual.Domain;

namespace Visual.Sharepoint
{
    [Guid("edd43a69-ebc4-4c68-8541-082089b4fd3d")]
    public class VisualList : Microsoft.SharePoint.WebPartPages.WebPart, IWebEditable
    {
        private string _albumId = null;
        private List<string> _tags = null;
        private int _count = 10;
        private string _order = "PublishedDescending";
        private string _tagMode = "Any";
        private VideoSize _size = VideoSize.Small;
        private bool _clickToPlay = false;

        [Personalizable(PersonalizationScope.Shared)]
        public string AlbumId
        {
            get { return _albumId; }
            set { _albumId = value; }
        }

        [Personalizable(PersonalizationScope.Shared)]
        public List<string> Tags
        {
            get { return _tags; }
            set { _tags = value; }
        }

        [Personalizable(PersonalizationScope.Shared)]
        public int Count
        {
            get { return _count; }
            set { _count = value; }
        }

        [Personalizable(PersonalizationScope.Shared)]
        public string Order
        {
            get { return _order; }
            set { _order = value; }
        }

        [Personalizable(PersonalizationScope.Shared)]
        public string TagMode
        {
            get { return _tagMode; }
            set { _tagMode = value; }
        }

        [Personalizable(PersonalizationScope.Shared)]
        public VideoSize Size
        {
            get { return _size; }
            set { _size = value; }
        }

        [Personalizable(PersonalizationScope.Shared)]
        public bool ClickToPlay
        {
            get { return _clickToPlay; }
            set { _clickToPlay = value; }
        }

        public VisualList()
        {
            this.ExportMode = WebPartExportMode.All;
        }

        /// <summary>
        /// Create all your controls here for rendering.
        /// Try to avoid using the RenderWebPart() method.
        /// </summary>
        protected override void CreateChildControls()
        {
            try
            {
                base.CreateChildControls();

                // * Make the query
                PhotoListParameters listParameters = new PhotoListParameters
                {
                    AlbumId = null,
                    IncludeUnpublished = false,
                    Size = _count
                };

                if (!String.IsNullOrEmpty(_albumId)) listParameters.AlbumId = Convert.ToInt32(_albumId);

                switch (_order)
                {
                    case "CreatedAscending":
                    case "PublishedAscending":
                    case "UploadedAscending":
                    case "ViewsAscending":
                        listParameters.Order = GenericSort.Ascending;
                        break;

                    default:
                        listParameters.Order = GenericSort.Descending;
                        break;
                }

                switch (_order)
                {
                    case "CreatedAscending":
                    case "CreatedDescending":
                        listParameters.OrderBy = PhotoListSort.Created;
                        break;

                    case "UploadedAscending":
                    case "UploadedDescending":
                        listParameters.OrderBy = PhotoListSort.Uploaded;
                        break;

                    case "ViewsAscending":
                    case "ViewsDescending":
                        listParameters.OrderBy = PhotoListSort.Views;
                        break;

                    default:
                        listParameters.OrderBy = PhotoListSort.Published;
                        break;
                }

                if ((_tags != null) && (_tags.Count > 0))
                {
                    listParameters.TagMode = (_tagMode == "All" ? PhotoTagMode.And : PhotoTagMode.Any);
                    listParameters.Tags = _tags;
                }

                this.Controls.Add(new LiteralControl("<ul class=\"visual-list\">"));

                IApiProvider apiProvider = Utilities.ApiProvider;
                if (apiProvider != null)
                {
                    IPhotoService photoService = new PhotoService(apiProvider);
                    List<Domain.Photo> photos = photoService.GetList(listParameters);

                    string protocol = Configuration.HttpSecure ? "https://" : "http://";

                    foreach (Domain.Photo photo in photos)
                    {
                        PhotoBlock size = Utilities.GetVideoSize(photo, Size);
                        string pid = "photo" + photo.PhotoId.ToString();
                        string showVideoCall;
                        if (_clickToPlay)
                        {
                            showVideoCall = "showVideo('" + pid + "','" + Utilities.EmbedCode(photo.PhotoId.Value.ToString(), photo.Token, size.Width.Value, null, true).Replace("\"", "&#34;").Replace("'", "\\'") + "'); return false;";
                        }
                        else
                        {
                            showVideoCall = "showVideo('" + Utilities.EmbedCode(photo.PhotoId.Value.ToString(), photo.Token, 640, null, false).Replace("\"", "&#34;").Replace("'", "\\'") + "'); return false;";
                        }
                        this.Controls.Add(new LiteralControl("<li>"));
                        this.Controls.Add(new LiteralControl("<div class=\"visual-list-image\"><a href=\"#\" id=\"" + pid + "\" onclick=\"" + showVideoCall + "\"><img src=\"" + protocol + Configuration.Domain + size.Download + "\" /></a></div>"));
                        this.Controls.Add(new LiteralControl("<div class=\"visual-list-meta\">"));
                        this.Controls.Add(new LiteralControl(photo.Title));
                        this.Controls.Add(new LiteralControl("<p>" + photo.ContentText + "</p>"));
                        this.Controls.Add(new LiteralControl("<div class=\"visual-list-date\">" + photo.OriginalDateDate + "</div>"));
                        if (photo.ViewCount != null) this.Controls.Add(new LiteralControl("<div class=\"visual-list-views\">" + photo.ViewCount.Value.ToString() + " views</div>"));
                        this.Controls.Add(new LiteralControl("</div>"));
                        this.Controls.Add(new LiteralControl("</li>"));
                    }
                }

                this.Controls.Add(new LiteralControl("</ul>"));
                if (_clickToPlay)
                {
                    this.Controls.Add(new LiteralControl("<script src=\"/_layouts/23video/23video.js\"></script>"));
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        /// <summary>
        /// Ensures that the CreateChildControls() is called before events.
        /// Use CreateChildControls() to create your controls.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            try
            {
                base.OnLoad(e);
                this.EnsureChildControls();
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        /// <summary>
        /// Clear all child controls and add an error message for display.
        /// </summary>
        /// <param name="ex"></param>
        private void HandleException(Exception ex)
        {
            this.Controls.Clear();
            this.Controls.Add(new LiteralControl(ex.Message));
        }

        /// <summary>
        /// Return the custom editor for selecting albums and tags
        /// </summary>
        /// <returns>EditorPartCollection containing the VisualListEditorPart</returns>
        EditorPartCollection IWebEditable.CreateEditorParts()
        {
            List<EditorPart> editors = new List<EditorPart>();
            VisualListEditorPart editorPart = new VisualListEditorPart();
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
