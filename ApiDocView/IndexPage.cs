/* ApiDocView
 * Copyright (c) 2022,  MSDN.WhiteKnight (https://github.com/MSDN-WhiteKnight) 
 * License: MIT */
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Html.Presentation;

namespace ApiDocView
{
    class IndexPage : Page
    {
        public IndexPage()
        {
            this.Name = "index.html";
            this.Html = ReadFromResource(this.Name);
        }

        public override void OnLoad(LoadEventArgs args)
        {
            if (!args.HasField("file")) return;

            object file = args.GetField("file");
            string srctext;
            string filename = string.Empty;

            if (file is ContentData)
            {
                ContentData data = (ContentData)file;
                srctext = Encoding.UTF8.GetString(data.Content);
                filename = data.FileName;
            }
            else srctext = file.ToString();

            if (Utils.StrEquals(Path.GetExtension(filename), ".xml"))
            {
                srctext = DocsViewer.GetFromXML(srctext);
            }

            string html = DocsViewer.RenderDocument(srctext);
            args.SendCustomResponse = true;
            args.CustomResponse = ContentData.FromHTML(html);
            return;
        }
    }
}
