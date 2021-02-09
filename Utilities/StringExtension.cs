using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiConnect.Utilities
{
    public static class StringExtension
    {
        public static string TotTitleCase(this string str)
        {
            str = str.Replace("\"", "").ToLower();
            str = char.ToUpper(str[0]) + str.Substring(1);

            return str;
        }
    }
}
