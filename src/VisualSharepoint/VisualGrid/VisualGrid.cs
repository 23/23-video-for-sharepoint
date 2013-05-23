using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;

namespace Visual.Sharepoint.VisualGrid
{
    [Guid("edd43a69-ebc4-4c68-8541-082089b4fd3d")]
    public class VisualGrid : Microsoft.SharePoint.WebPartPages.WebPart, IWebEditable
    {
        private string _albumId = null;
        private List<string> _tags = null;
        private int _count = 9;
        private int _rowCount = 3;
        private string _order = "PublishedDescending";
        private string _tagMode = "Any";

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
        public int RowCount
        {
            get { return _rowCount; }
            set { _rowCount = value; }
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

        public VisualGrid()
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

                this.Controls.Add(new LiteralControl("<table class=\"visual-grid\">"));

                IApiProvider apiProvider = Utilities.ApiProvider;
                if (apiProvider != null)
                {
                    IPhotoService photoService = new PhotoService(apiProvider);
                    List<Domain.Photo> photos = photoService.GetList(listParameters);

                    int rowNumber = 0;
                    int rowCount = (int)Math.Ceiling((decimal)photos.Count / (decimal)_rowCount);
                    int colNumber = 0;

                    foreach (Domain.Photo photo in photos)
                    {
                        string showVideoCall = "showVideo('" + Utilities.EmbedCode(photo.PhotoId.Value.ToString(), photo.Token, 640, null).Replace("\"", "&#34;").Replace("'", "\\'") + "'); return false;";

                        if (colNumber == 0) this.Controls.Add(new LiteralControl("<tr" + (rowNumber + 1 == rowCount ? " class=\"last\"" : "") + ">"));

                        this.Controls.Add(new LiteralControl("<td onclick=\"" + showVideoCall + "\"" + ((colNumber == _rowCount - 1) ? " class=\"last\"" : "") + ">"));
                        this.Controls.Add(new LiteralControl("<div class=\"visual-grid-image\"><img src=\"http://" + Configuration.Domain + photo.Small.Download + "\" /></div>"));
                        this.Controls.Add(new LiteralControl("<div class=\"visual-grid-meta\">"));
                        this.Controls.Add(new LiteralControl("<a href=\"#\" onclick=\"" + showVideoCall + "\">" + photo.Title + "</a>"));
                        this.Controls.Add(new LiteralControl("<p>" + photo.ContentText + "</p>"));
                        this.Controls.Add(new LiteralControl("<div class=\"visual-grid-date\">" + photo.OriginalDateDate + "</div>"));
                        if (photo.ViewCount != null) this.Controls.Add(new LiteralControl("<div class=\"visual-grid-views\">" + photo.ViewCount.Value.ToString() + " views</div>"));
                        this.Controls.Add(new LiteralControl("</div>"));
                        this.Controls.Add(new LiteralControl("</td>"));

                        colNumber++;
                        if (colNumber == _rowCount)
                        {
                            this.Controls.Add(new LiteralControl("</tr>"));
                            colNumber = 0;
                            rowNumber++;
                        }
                    }

                    if (colNumber > 0)
                    {
                        while (colNumber < _rowCount)
                        {
                            this.Controls.Add(new LiteralControl("<td class=\"visual-grid-empty\">&nbsp;</td>"));
                            colNumber++;
                        }

                        this.Controls.Add(new LiteralControl("</tr>"));
                    }
                }

                this.Controls.Add(new LiteralControl("</table>"));
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
        /// <returns>EditorPartCollection containing the VisualGridEditorPart</returns>
        EditorPartCollection IWebEditable.CreateEditorParts()
        {
            List<EditorPart> editors = new List<EditorPart>();
            VisualGridEditorPart editorPart = new VisualGridEditorPart();
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
