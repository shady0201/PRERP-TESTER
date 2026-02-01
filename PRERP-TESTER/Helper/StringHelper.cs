using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRERP_TESTER.Helper
{
    public static class StringHelper
    {
        public static string RemoveSign4VietnameseString(string str)
        {
            if (string.IsNullOrEmpty(str)) return str;

            string strFormD = str.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new();

            foreach (char c in strFormD)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(c);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }
            return sb.ToString().Normalize(NormalizationForm.FormC)
                     .Replace('đ', 'd').Replace('Đ', 'D');
        }
    }
}
