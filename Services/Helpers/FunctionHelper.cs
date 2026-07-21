using System.Globalization;
using System.Text;

namespace Services.Helpers
{
    public static class FunctionHelper
    {
        public static string GetBackIn(int input)
        {
            StringBuilder sb = new StringBuilder();
            if (input > 0)
            {
                for (int i = 0; i < input; i++)
                {
                    sb.Append("\u2003");
                }
            }
            return sb.ToString();
        }

        public static string ConvertDblToStr(double input)
        {
            if (input == 0)
            {
                return "";
            }
            else
            {
                if (Math.Abs(input % 1) < double.Epsilon)
                {
                    // Nếu là số nguyên, định dạng theo dạng #,###
                    return input.ToString("#,##0").Replace(",", ".");
                }
                else
                {
                    // Nếu không phải là số nguyên, định dạng theo dạng #,###
                    string formatted = input.ToString("#,##0.###");
                    int indexOfDecimal = formatted.IndexOf('.');
                    if (indexOfDecimal != -1)
                    {
                        int lengthToRemove = 0;
                        for (int i = formatted.Length - 1; i > indexOfDecimal; i--)
                        {
                            if (formatted[i] == '0')
                            {
                                lengthToRemove++;
                            }
                            else
                            {
                                break;
                            }
                        }
                        if (lengthToRemove > 0)
                        {
                            // Kiểm tra xem vị trí loại bỏ và số lượng ký tự loại bỏ có hợp lệ không
                            if (indexOfDecimal + 3 + lengthToRemove <= formatted.Length)
                            {
                                formatted = formatted.Remove(indexOfDecimal + 3, lengthToRemove);
                            }
                        }
                    }
                    return formatted.Replace(".", "*").Replace(",", ".").Replace("*", ",");
                }
            }
        }

        public static decimal? ConvertStrToDecimal(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return null;
            }

            // Chuẩn hóa chuỗi từ định dạng Việt Nam (1.234,567) về định dạng C# (1234.567)
            string normalizedValue = input.Replace(".", "").Replace(",", ".");
            return decimal.TryParse(normalizedValue, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result) ? result : (decimal?)null;
        }
    }
}
