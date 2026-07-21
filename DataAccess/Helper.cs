using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Text;

namespace DataAccess
{
    public class Helper
    {
        public static Guid GetSsAdminGuid(ISession? session)
        {
            if (session != null)
            {
                string ssAdminJson = session.GetString("SsAdmin") ?? string.Empty;

                if (!string.IsNullOrEmpty(ssAdminJson))
                {
                    var sessionInfo = JsonConvert.DeserializeObject<JObject>(ssAdminJson);
                    Console.WriteLine($"✅ JSON Parsed: {sessionInfo?.ToString()}");

                    if (sessionInfo?["Id"] != null)
                    {
                        if (Guid.TryParse(sessionInfo["Id"]?.ToString(), out var result))
                        {

                            return result;
                        }
                    }
                }
            }
            return Guid.Empty;
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

        public static string ConvertStrToSlug(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // 1. Chuyển về chữ thường
            string result = input.ToLowerInvariant();

            // 2. Bỏ dấu tiếng Việt
            result = RemoveDiacritics(result);

            // 3. Thay \ và / thành dấu gạch ngang trước
            result = Regex.Replace(result, @"[\\/]", "-");

            // 4. Loại bỏ ký tự không hợp lệ (chỉ giữ a-z, 0-9, space, -)
            result = Regex.Replace(result, @"[^a-z0-9\s-]", "");

            // 5. Chuyển khoảng trắng thành -
            result = Regex.Replace(result, @"\s+", "-");

            // 6. Gom các dấu - liền nhau
            result = Regex.Replace(result, @"-+", "-");

            // 7. Trim đầu và cuối
            result = result.Trim('-');

            return result;
        }

        // Hàm bỏ dấu tiếng Việt
        private static string RemoveDiacritics(string input)
        {
            string unicode = input.ToLower();
            unicode = Regex.Replace(unicode, @"[á|à|ạ|ả|ã|â|ấ|ầ|ậ|ẩ|ẫ|ă|ắ|ằ|ặ|ẳ|ẵ]", "a");
            unicode = Regex.Replace(unicode, @"[é|è|ẹ|ẻ|ẽ|ê|ế|ề|ệ|ể|ễ]", "e");
            unicode = Regex.Replace(unicode, @"[ó|ò|ọ|ỏ|õ|ô|ố|ồ|ộ|ổ|ỗ|ơ|ớ|ờ|ợ|ở|ỡ]", "o");
            unicode = Regex.Replace(unicode, @"[í|ì|ị|ỉ|ĩ]", "i");
            unicode = Regex.Replace(unicode, @"[ý|ỳ|ỵ|ỷ|ỹ]", "y");
            unicode = Regex.Replace(unicode, @"[ú|ù|ụ|ủ|ũ|ư|ứ|ừ|ự|ử|ữ]", "u");
            unicode = Regex.Replace(unicode, @"[đ]", "d");
            return unicode;
        }
    }
}
