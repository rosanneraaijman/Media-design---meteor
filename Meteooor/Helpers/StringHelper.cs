using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Meteooor.Helpers
{
    public static class StringHelper
    {
        public static string SanitizeString(string input)
        {
            int endStartTag = input.IndexOf(">");
            int beginEndTag = input.LastIndexOf("</div");

            if (beginEndTag < 0)
            {
                beginEndTag = input.LastIndexOf("<");
            }

            return input.Substring(endStartTag + 1, (beginEndTag - endStartTag) - 1).Trim();
        }
    }
}