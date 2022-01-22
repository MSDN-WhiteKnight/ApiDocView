/* Html.Presentation
 * Copyright (c) 2022,  MSDN.WhiteKnight (https://github.com/MSDN-WhiteKnight) 
 * License: MIT */
using System;
using System.Collections.Generic;
using System.Text;

namespace Html.Presentation
{
    public sealed class ContentData
    {
        public byte[] Content { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }

        public ContentData()
        {
            this.FileName = string.Empty;
        }

        public static ContentData FromText(string str)
        {
            ContentData ret = new ContentData();
            ret.Content = Encoding.UTF8.GetBytes(str);
            ret.ContentType = ContentTypes.PlainText;
            return ret;
        }

        public static ContentData FromHTML(string str)
        {
            ContentData ret = new ContentData();
            ret.Content = Encoding.UTF8.GetBytes(str);
            ret.ContentType = ContentTypes.HTML;
            return ret;
        }

        public static ContentData FromJS(string str)
        {
            ContentData ret = new ContentData();
            ret.Content = Encoding.UTF8.GetBytes(str);
            ret.ContentType = ContentTypes.JavaScript;
            return ret;
        }

        public bool IsText
        {
            get { return string.IsNullOrEmpty(this.ContentType) || ContentTypes.IsTextContentType(this.ContentType); }
        }

        public override string ToString()
        {
            if (this.Content == null) return base.ToString();

            if (string.IsNullOrEmpty(this.ContentType) || ContentTypes.IsTextContentType(this.ContentType))
            {
                return Encoding.UTF8.GetString(this.Content);
            }
            else
            {
                return "Binary data (" + this.Content.Length + " bytes)";
            }
        }
    }
}
