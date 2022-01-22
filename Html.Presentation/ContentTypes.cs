/* Html.Presentation
 * Copyright (c) 2022,  MSDN.WhiteKnight (https://github.com/MSDN-WhiteKnight) 
 * License: MIT */

namespace Html.Presentation
{
    public static class ContentTypes
    {
        public const string PlainText = "text/plain";
        public const string HTML = "text/html";
        public const string JavaScript = "text/javascript";
        public const string Binary = "application/octet-stream";

        internal static bool IsTextContentType(string contentType)
        {
            return Utils.StrEquals(contentType, PlainText) || Utils.StrEquals(contentType, HTML) ||
                   Utils.StrEquals(contentType, JavaScript);
        }
    }
}
