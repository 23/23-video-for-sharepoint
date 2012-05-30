using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using System.Configuration;
using Visual;

namespace Visual.Sharepoint
{
    public class VisualWebPart : Microsoft.SharePoint.WebPartPages.WebPart
    {
        public VisualWebPart()
        {
            // Set the export mode to the default
            this.ExportMode = WebPartExportMode.All;
        }
    }
}
