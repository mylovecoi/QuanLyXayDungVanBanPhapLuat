using DataAccess.Entities.Systems;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Dynamic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Text;
using System.Security.Cryptography;
using UI.ViewModels;
using Microsoft.AspNetCore.Components.Forms;

namespace UI.Helper
{
    public class FuntionGlobal
    {
        public static string GetSsAdmin(ISession session, string key)
        {
            string new_str = "";
            if (session != null)
            {
                string ssAdminJson = session?.GetString("SsAdmin") ?? string.Empty;

                if (!string.IsNullOrEmpty(ssAdminJson))
                {
                    dynamic sessionInfo = JsonConvert.DeserializeObject(ssAdminJson) ?? new ExpandoObject(); ;

                    if (sessionInfo != null)
                    {
                        if (sessionInfo[key] != null)
                        {
                            new_str = sessionInfo[key].ToString();
                        }
                    }
                }
            }
            return new_str;
        }

        public static Guid GetSsAdminId(ISession? session)
        {
            if (session == null)
            {
                throw new ArgumentNullException(nameof(session));
            }

            string ssAdminJson = session.GetString("SsAdmin") ?? string.Empty;

            if (!string.IsNullOrEmpty(ssAdminJson))
            {
                try
                {
                    var sessionInfo = JsonConvert.DeserializeObject<JObject>(ssAdminJson);

                    //if (sessionInfo != null && sessionInfo["Id"] != null)
                    //{
                    //    return Guid.Parse(sessionInfo["Id"].ToString());
                    //}
                    if (sessionInfo?["Id"] != null)
                    {
                        // Đảm bảo rằng "Id" là một chuỗi hợp lệ để chuyển đổi thành Guid
                        if (Guid.TryParse(sessionInfo["Id"]?.ToString(), out var result))
                        {
                            return result;
                        }
                    }
                }
                catch (JsonException)
                {
                    // Handle JSON parsing errors if necessary
                    // Log the exception or take appropriate action
                }
            }

            return Guid.Empty;
        }

        public static Guid GetSsAdminDonViId(ISession? session)
        {
            if (session == null)
            {
                throw new ArgumentNullException(nameof(session));
            }

            string ssAdminJson = session.GetString("SsAdmin") ?? string.Empty;

            if (!string.IsNullOrEmpty(ssAdminJson))
            {
                try
                {
                    var sessionInfo = JsonConvert.DeserializeObject<JObject>(ssAdminJson);

                    //if (sessionInfo != null && sessionInfo["Id"] != null)
                    //{
                    //    return Guid.Parse(sessionInfo["Id"].ToString());
                    //}
                    if (sessionInfo?["DanhMucDonViId"] != null)
                    {
                        // Đảm bảo rằng "Id" là một chuỗi hợp lệ để chuyển đổi thành Guid
                        if (Guid.TryParse(sessionInfo["DanhMucDonViId"]?.ToString(), out var result))
                        {
                            return result;
                        }
                    }
                }
                catch (JsonException)
                {
                    // Handle JSON parsing errors if necessary
                    // Log the exception or take appropriate action
                }
            }

            return Guid.Empty;
        }

        public static bool IsVNId(ISession? session)
        {
            if (session == null) return false;

            var ssAdminJson = session.GetString("SsAdmin");
            if (string.IsNullOrEmpty(ssAdminJson)) return false;

            var sessionInfo = JsonConvert.DeserializeObject<JObject>(ssAdminJson);
            if (sessionInfo?["VNId"] == null) return false;

            return bool.TryParse(sessionInfo["VNId"]?.ToString(), out var result) && result;
        }

        public static bool CheckPermission(ISession session, string roles, string key)
        {
            string per = session?.GetString("Permissions") ?? string.Empty;
            if (!string.IsNullOrEmpty(per))
            {
                if (JsonConvert.DeserializeObject(per) is IEnumerable<dynamic> permissionInfo && permissionInfo.Any())
                {
                    foreach (var item in permissionInfo)
                    {
                        // Kiểm tra xem item có null không trước khi so sánh giá trị
                        if (item != null)
                        {
                            if (item["Role"] != null && item[key] == true && item["Role"] == roles)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        public static string BCryptHash(string input)
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(input);
            return hashedPassword;
        }

        public static string GetSystemDefaultPassword()
        {
            return BCryptHash("Life@2012!");
        }

        //Convert Date- DateTime
        public static string ConvertDateToText(DateTime input)
        {

            string date_convert = input.Date.ToString("dd/MM/yyyy");
            if (date_convert == "01/01/0001")
            {
                return " Ngày .. tháng .. năm ....";
            }
            else
            {
                string new_str = "";
                new_str += " Ngày " + input.ToString("dd");
                new_str += " tháng " + input.ToString("MM");
                new_str += " năm " + input.ToString("yyyy");
                return new_str;
            }
        }

        public static string ConvertDateToISO(DateTime input)
        {
            string new_str = input.Date.ToString("yyyy-MM-dd");
            return new_str;
        }

        public static string ConvertDateToStr(DateTime input)
        {

            string new_str = input.Date.ToString("dd/MM/yyyy");
            if (new_str == "01/01/0001")
            {
                return "";
            }
            else
            {
                return new_str;
            }
        }

        public static string ConvertDateTimeToStr(DateTime input)
        {
            string new_str = input.ToString("dd/MM/yyyy HH:mm:ss", new CultureInfo("vi-VN"));
            return new_str;
        }

        public static string ConvertDateTimeToText(DateTime input)
        {
            string Hour = input.Hour < 10 ? "0" + input.Hour.ToString() : input.Hour.ToString();
            string Min = input.Minute < 10 ? "0" + input.Minute.ToString() : input.Minute.ToString();
            string Day = input.Day < 10 ? "0" + input.Day.ToString() : input.Day.ToString();
            string Month = input.Month < 10 ? "0" + input.Month.ToString() : input.Month.ToString();
            string Year = input.Year.ToString();

            string new_str = Hour + " giờ " + Min + " phút";
            new_str += ", ngày " + Day + " tháng " + Month + " năm " + Year;
            return new_str;
        }

        //End Convert Date- DateTime


        //Convert String
        public static double ConvertStrToDbl(string input)
        {
            double new_dbl = 0;
            if (!string.IsNullOrEmpty(input))
            {
                string numericString = Regex.Replace(input, @"[^\d,]", "").Replace(',', '.');

                // Lấy thông tin về cài đặt vùng của hệ thống
                CultureInfo culture = CultureInfo.CurrentCulture;

                // Chỉ định định dạng số cho việc chuyển đổi
                NumberFormatInfo numberFormat = new NumberFormatInfo();
                numberFormat.NumberDecimalSeparator = culture.NumberFormat.NumberDecimalSeparator;
                numberFormat.NumberGroupSeparator = culture.NumberFormat.NumberGroupSeparator;

                // Kiểm tra xem chuỗi sau khi loại bỏ các ký tự không phải số có thể được chuyển đổi thành double không
                if (double.TryParse(numericString, NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, numberFormat, out double result))
                {
                    new_dbl = result;
                }
            }
            return new_dbl;
        }
        public static string ConvertStrToStyle(string strStyle)
        {
            if (string.IsNullOrEmpty(strStyle))
            {
                return "";
            }

            List<string> list_style = new List<string>(strStyle.Split(","));
            StringBuilder HtmlStyle = new StringBuilder();

            if (list_style.Contains("Chữ in hoa"))
            {
                HtmlStyle.Append("text-transform: uppercase;");
            }
            if (list_style.Contains("Chữ in đậm"))
            {
                HtmlStyle.Append("font-weight: bold;");
            }
            if (list_style.Contains("Chữ in nghiêng"))
            {
                HtmlStyle.Append("font-style: italic;");
            }

            return HtmlStyle.ToString();
        }

        //End Convert String

        //Convert Double
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

        public static string ConvertDblToVNCurrency(double input, bool suffix = true)
        {
            if (input != 0)
            {
                string[] unitNumbers = new string[] { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
                string[] placeValues = new string[] { "", "nghìn", "triệu", "tỷ" };
                bool isNegative = false;

                // -12345678.3445435 => "-12345678"
                string sNumber = input.ToString("#");
                double number = Convert.ToDouble(sNumber);
                if (number < 0)
                {
                    number = -number;
                    sNumber = number.ToString();
                    isNegative = true;
                }


                int ones, tens, hundreds;

                int positionDigit = sNumber.Length;   // last -> first

                string result = " ";


                if (positionDigit == 0)
                    result = unitNumbers[0] + result;
                else
                {
                    // 0:       ###
                    // 1: nghìn ###,###
                    // 2: triệu ###,###,###
                    // 3: tỷ    ###,###,###,###
                    int placeValue = 0;

                    while (positionDigit > 0)
                    {
                        // Check last 3 digits remain ### (hundreds tens ones)
                        tens = hundreds = -1;
                        ones = Convert.ToInt32(sNumber.Substring(positionDigit - 1, 1));
                        positionDigit--;
                        if (positionDigit > 0)
                        {
                            tens = Convert.ToInt32(sNumber.Substring(positionDigit - 1, 1));
                            positionDigit--;
                            if (positionDigit > 0)
                            {
                                hundreds = Convert.ToInt32(sNumber.Substring(positionDigit - 1, 1));
                                positionDigit--;
                            }
                        }

                        if ((ones > 0) || (tens > 0) || (hundreds > 0) || (placeValue == 3))
                            result = placeValues[placeValue] + result;

                        placeValue++;
                        if (placeValue > 3) placeValue = 1;

                        if ((ones == 1) && (tens > 1))
                            result = "một " + result;
                        else
                        {
                            if ((ones == 5) && (tens > 0))
                                result = "lăm " + result;
                            else if (ones > 0)
                                result = unitNumbers[ones] + " " + result;
                        }
                        if (tens < 0)
                            break;
                        else
                        {
                            if ((tens == 0) && (ones > 0)) result = "lẻ " + result;
                            if (tens == 1) result = "mười " + result;
                            if (tens > 1) result = unitNumbers[tens] + " mươi " + result;
                        }
                        if (hundreds < 0) break;
                        else
                        {
                            if ((hundreds > 0) || (tens > 0) || (ones > 0))
                                result = unitNumbers[hundreds] + " trăm " + result;
                        }
                        result = " " + result;
                    }
                }
                result = result.Trim();
                string[] str = result.Split(" ");
                int len_array = str.Count() - 1;
                string new_str = str[0].Substring(0, 1).ToUpper() + str[0].Substring(1);
                for (int i = 1; i <= len_array; i++)
                {
                    new_str += " " + str[i];
                }
                if (isNegative) result = "Âm " + new_str;
                return new_str + (suffix ? " đồng chẵn %." : "");
            }
            else
            {
                return "";
            }
        }

        public static string ConvertDblToText(double input, bool suffix = true)
        {
            if (input != 0)
            {
                string[] unitNumbers = new string[] { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
                string[] placeValues = new string[] { "", "nghìn", "triệu", "tỷ" };
                bool isNegative = false;

                // -12345678.3445435 => "-12345678"
                string sNumber = input.ToString("#");
                double number = Convert.ToDouble(sNumber);
                if (number < 0)
                {
                    number = -number;
                    sNumber = number.ToString();
                    isNegative = true;
                }


                int ones, tens, hundreds;

                int positionDigit = sNumber.Length;   // last -> first

                string result = " ";


                if (positionDigit == 0)
                    result = unitNumbers[0] + result;
                else
                {
                    // 0:       ###
                    // 1: nghìn ###,###
                    // 2: triệu ###,###,###
                    // 3: tỷ    ###,###,###,###
                    int placeValue = 0;

                    while (positionDigit > 0)
                    {
                        // Check last 3 digits remain ### (hundreds tens ones)
                        tens = hundreds = -1;
                        ones = Convert.ToInt32(sNumber.Substring(positionDigit - 1, 1));
                        positionDigit--;
                        if (positionDigit > 0)
                        {
                            tens = Convert.ToInt32(sNumber.Substring(positionDigit - 1, 1));
                            positionDigit--;
                            if (positionDigit > 0)
                            {
                                hundreds = Convert.ToInt32(sNumber.Substring(positionDigit - 1, 1));
                                positionDigit--;
                            }
                        }

                        if ((ones > 0) || (tens > 0) || (hundreds > 0) || (placeValue == 3))
                            result = placeValues[placeValue] + result;

                        placeValue++;
                        if (placeValue > 3) placeValue = 1;

                        if ((ones == 1) && (tens > 1))
                            result = "một " + result;
                        else
                        {
                            if ((ones == 5) && (tens > 0))
                                result = "lăm " + result;
                            else if (ones > 0)
                                result = unitNumbers[ones] + " " + result;
                        }
                        if (tens < 0)
                            break;
                        else
                        {
                            if ((tens == 0) && (ones > 0)) result = "lẻ " + result;
                            if (tens == 1) result = "mười " + result;
                            if (tens > 1) result = unitNumbers[tens] + " mươi " + result;
                        }
                        if (hundreds < 0) break;
                        else
                        {
                            if ((hundreds > 0) || (tens > 0) || (ones > 0))
                                result = unitNumbers[hundreds] + " trăm " + result;
                        }
                        result = " " + result;
                    }
                }
                result = result.Trim();
                string[] str = result.Split(" ");
                int len_array = str.Count() - 1;
                string new_str = str[0].Substring(0, 1).ToUpper() + str[0].Substring(1);
                for (int i = 1; i <= len_array; i++)
                {
                    new_str += " " + str[i];
                }
                if (isNegative) new_str = "Âm " + new_str;
                return new_str;
            }
            else
            {
                return "";
            }
        }
        //End Convert Double
        //Convert Int
        public static string ConvertIntToRoman(int number)
        {
            if (number < 1 || number > 30)
                return "";

            string[] romanOnes = { "", "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX" };
            string[] romanTens = { "", "X", "XX", "XXX" };

            int ones = number % 10;
            int tens = number / 10;

            return romanTens[tens] + romanOnes[ones];
        }

        public static string ConvertIntToAlphabet(int input)
        {
            if (input < 1)
                return "";

            string new_str = "";
            while (input > 0)
            {
                int remainder = (input - 1) % 26;
                new_str = (char)('A' + remainder) + new_str;
                input = (input - 1) / 26;
            }
            return new_str;
        }
        //End Convert Int     

        public static string GetBackIn(int input)
        {
            StringBuilder sb = new StringBuilder();
            if (input > 0)
            {
                for (int i = 0; i < input; i++)
                {
                    sb.Append("&emsp;");
                }
            }
            return sb.ToString();
        }

        public static string GetClassTable(int input)
        {
            string[] classArray = { "table-primary", "table-success", "table-warning", "table-info", "table-active" };
            return classArray[input % 5];
        }

        public static string GetStyleNotiRead(bool isRead)
        {
            if (!isRead)
            {
                return "font-weight: bold;";
            }
            else
            {
                return "";
            }
        }

        public static string GetClassTableWithStatus(string? status)
        {
            string new_str;
            switch (status)
            {
                case "CC":
                    new_str = "table-info";
                    break;
                case "CD":
                    new_str = "table-warning";
                    break;
                case "BTL":
                    new_str = "table-danger";
                    break;
                case "DD":
                    new_str = "table-success";
                    break;
                case "CB":
                    new_str = "table-primary";
                    break;
                default:
                    new_str = "table-active";
                    break;
            }
            return new_str;
        }

        public static List<int> GetRangePage(int pageTotal, int pageCurrent)
        {
            if (pageTotal <= 0 || pageCurrent <= 0) return new List<int>(); // Handle invalid input

            int start, stop;
            if (pageTotal <= 5)
            {
                start = 1;
                stop = pageTotal;
            }
            else if (6 == pageTotal)
            {
                if (pageCurrent < 4)
                {
                    start = 1;
                    stop = 4;
                }
                else if (pageTotal - 3 < pageCurrent || pageCurrent < pageTotal)
                {
                    start = pageTotal - 3;
                    stop = pageTotal;
                }
                else
                {
                    start = pageCurrent - 1;
                    stop = pageCurrent + 1;
                }
            }
            else
            {
                if (pageCurrent <= 4)
                {
                    start = 1;
                    stop = 5;
                }
                else if (pageCurrent >= pageTotal - 3) // 4 trang cuối
                {
                    start = pageTotal - 4;
                    stop = pageTotal;
                }
                else
                {
                    start = pageCurrent - 2;
                    stop = pageCurrent + 2;
                }
            }
            return Enumerable.Range(start, stop - start + 1).ToList();
        }

        public static VMPageInfo GetPageInfo(int totalRecord, string search, int pageSize, int pageCurrent)
        {
            int pageTotal = Convert.ToInt32(Math.Ceiling(totalRecord / (double)pageSize));
            var pageInfo = new VMPageInfo
            {
                Search = search,
                TotalRecord = totalRecord,
                PageSize = pageSize,
                PageCurrent = pageCurrent,
                PageTotal = pageTotal,
                PageRange = GetRangePage(pageTotal, pageCurrent),
                RecordStart = ((pageCurrent - 1) * pageSize) + 1
            };
            return pageInfo;
        }

        public static VMPageInfoWithData<T> GetPageInfo<T>(int totalRecord, string search, int pageSize, int pageCurrent, List<T> data)
        {
            var basePageInfo = GetPageInfo(totalRecord, search, pageSize, pageCurrent);
            return new VMPageInfoWithData<T>(basePageInfo, data);
        }

        public static string StrToRoman(int number)
        {
            if (number < 1) return string.Empty;

            var map = new[]
            {
                    new { Value = 1000, Symbol = "M" },
                    new { Value = 900, Symbol = "CM" },
                    new { Value = 500, Symbol = "D" },
                    new { Value = 400, Symbol = "CD" },
                    new { Value = 100, Symbol = "C" },
                    new { Value = 90, Symbol = "XC" },
                    new { Value = 50, Symbol = "L" },
                    new { Value = 40, Symbol = "XL" },
                    new { Value = 10, Symbol = "X" },
                    new { Value = 9, Symbol = "IX" },
                    new { Value = 5, Symbol = "V" },
                    new { Value = 4, Symbol = "IV" },
                    new { Value = 1, Symbol = "I" }
                };

            var result = "";
            foreach (var entry in map)
            {
                while (number >= entry.Value)
                {
                    result += entry.Symbol;
                    number -= entry.Value;
                }
            }
            return result;
        }

        public static string ToSentenceCase(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            input = input.Trim();

            return char.ToUpper(input[0]) + input.Substring(1).ToLower();
        }

        public static string CapitalizeEachWord(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            var words = input.ToLower().Split(' ');
            for (int i = 0; i < words.Length; i++)
            {
                if (words[i].Length > 0)
                    words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1);
            }

            return string.Join(" ", words);
        }

        public static string CapitalizeAll(string? input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }
            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
            return textInfo.ToTitleCase(input.ToLower());
        }
    }
}
