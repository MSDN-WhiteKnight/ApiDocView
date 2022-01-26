/* ApiDocView
 * Copyright (c) 2022,  MSDN.WhiteKnight (https://github.com/MSDN-WhiteKnight) 
 * License: MIT */
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
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
            if (!args.HasField("file") && !args.HasField("filename"))
            {
                args.SendCustomResponse = true;
                string filelist = DocsViewer.RenderFileList();
                string ret = this.Html.Replace("<div id=\"content\"/>", filelist);
                args.CustomResponse = ContentData.FromHTML(ret);
                return;
            }

            string srctext = string.Empty;

            if (args.HasField("file"))
            {
                object file = args.GetField("file");
                
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
            }
            else if (args.HasField("filename"))
            {
                string filename = WebUtility.UrlDecode(args.GetField("filename").ToString());
                string filepath = Path.Combine(DocsViewer.SrcDirectory, filename);
                                
                srctext = File.ReadAllText(filepath);

                if (Utils.StrEquals(Path.GetExtension(filename), ".xml"))
                {
                    srctext = DocsViewer.GetFromXML(srctext);
                }
            }

            string html = DocsViewer.RenderDocument(srctext);
            args.SendCustomResponse = true;
            args.CustomResponse = ContentData.FromHTML(html);
        }
    }
}
