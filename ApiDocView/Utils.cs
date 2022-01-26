/* ApiDocView
 * Copyright (c) 2022,  MSDN.WhiteKnight (https://github.com/MSDN-WhiteKnight) 
 * License: MIT */
using System;
using System.Collections.Generic;
using System.Text;

namespace ApiDocView
{
    internal class Utils
    {
        public static bool StrEquals(string left, string right)
        {
            return string.Equals(left, right, StringComparison.Ordinal);
        }

        public static bool IsDocsFileExtension(string ext)
        {
            return StrEquals(ext, ".txt") || StrEquals(ext, ".md") || StrEquals(ext, ".xml") ||
                StrEquals(ext, ".html");
        }
    }
}
