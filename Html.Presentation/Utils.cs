using System;
using System.Collections.Generic;
using System.Text;

namespace HtmlUiTest
{
    internal class Utils
    {
        public static bool StrEquals(string left, string right)
        {
            return string.Equals(left, right, StringComparison.Ordinal);
        }
    }
}
