using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Services
{
    public class Helper
    {
        public static List<SelectListItem> GetEnumSelectList<T>() where T : Enum
        {
            return [.. Enum.GetValues(typeof(T))
                .Cast<T>()
                .Select(e => new SelectListItem
                {
                    Value = Convert.ToInt32(e).ToString(),
                    Text = GetEnumDescription(e) ?? e.ToString()
                })];
        }

        private static string? GetEnumDescription(Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            if (field == null) return null;

            var attribute = field.GetCustomAttributes(typeof(DescriptionAttribute), false)
                                 .FirstOrDefault() as DescriptionAttribute;

            return attribute?.Description ?? value.ToString();
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

        public static Dictionary<string, string> ConvertStringToDictionary(string parameterString)
        {
            if (string.IsNullOrWhiteSpace(parameterString))
            {
                return new Dictionary<string, string>(); // Trả về Dictionary rỗng nếu input null hoặc rỗng
            }
            return parameterString
                .Split(',')
                .Select(p => p.Split('='))
                .ToDictionary(keyValue => keyValue[0].Trim(), keyValue => keyValue[1].Trim());
        }

        public static void MapProperties<T>(T source, T destination)
        {
            var properties = typeof(T).GetProperties();
            foreach (var prop in properties)
            {
                if (prop.CanWrite) // Chỉ cập nhật nếu có setter
                {
                    var value = prop.GetValue(source);
                    prop.SetValue(destination, value);
                }
            }
        }

        public static (T? model, List<string> errors) MapAndValidate<T>(object request) where T : new()
        {
            if (request == null)
            {
                return (default(T), new List<string> { "Dữ liệu đầu vào không hợp lệ." });
            }

            T model = new T();
            PropertyInfo[] modelProperties = typeof(T).GetProperties();
            var requestProperties = request.GetType().GetProperties().ToDictionary(p => p.Name, p => p);

            List<string> errors = new List<string>();

            foreach (var prop in modelProperties)
            {
                if (prop.Name.Equals("Id", StringComparison.OrdinalIgnoreCase)) continue;

                if (requestProperties.TryGetValue(prop.Name, out var sourceProp) && prop.CanWrite)
                {
                    prop.SetValue(model, sourceProp.GetValue(request));
                }

                // Kiểm tra Required
                var requiredAttr = prop.GetCustomAttribute<RequiredAttribute>();
                if (requiredAttr != null)
                {
                    var value = prop.GetValue(model);
                    if (value == null || (value is string str && string.IsNullOrWhiteSpace(str)))
                    {
                        errors.Add(requiredAttr.ErrorMessage ?? $"{prop.Name} không được để trống.");
                    }
                }
            }

            return errors.Count > 0 ? (default(T), errors) : (model, errors);
        }

        public static object GetValidationErrorMessage(FluentValidation.Results.ValidationResult validationResult)
        {
            if (validationResult == null || validationResult.Errors.Count == 0)
                return "Dữ liệu không hợp lệ.";

            var errors = validationResult.Errors
            .GroupBy(x => x.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => string.Join("; ", g.Select(e => e.ErrorMessage))
            );
            return errors;
        }

        public static Dictionary<string, string> GetValidationErrorsDictionary(FluentValidation.Results.ValidationResult validationResult)
        {
            if (validationResult == null || !validationResult.Errors.Any())
            {
                return new Dictionary<string, string> { { "Global", "Dữ liệu không hợp lệ." } };
            }

            return validationResult.Errors
                .GroupBy(x => x.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).First()
                );
        }

        public static bool ValidateStringDateTime(string raw, int minYear = 1900, int? maxYear = null)
        {
            if (string.IsNullOrWhiteSpace(raw) || raw.Trim().ToLower() == "null") return true;

            if (!raw.All(char.IsDigit)) return false;

            int effectiveMaxYear = maxYear ?? DateTime.Now.Year;

            if (raw.Length == 4)
            {
                var yyyy = int.Parse(raw);
                return yyyy >= minYear && yyyy <= effectiveMaxYear;
            }

            if (raw.Length == 6)
            {
                var mm = int.Parse(raw.Substring(0, 2));
                var yyyy = int.Parse(raw.Substring(2, 4));

                return mm >= 1 && mm <= 12 && yyyy >= minYear && yyyy <= effectiveMaxYear;
            }

            if (raw.Length == 8)
            {
                var dd = int.Parse(raw.Substring(0, 2));
                var mm = int.Parse(raw.Substring(2, 2));
                var yyyy = int.Parse(raw.Substring(4, 4));

                if (yyyy < minYear || yyyy > effectiveMaxYear) return false;

                return DateTime.TryParse($"{yyyy}-{mm:D2}-{dd:D2}", out _);
            }
            return false;
        }

        public static string GetDisplayName<TModel, TProperty>(Expression<Func<TModel, TProperty>> expression)
        {
            var member = expression.Body as MemberExpression
                         ?? (expression.Body as UnaryExpression)?.Operand as MemberExpression;

            if (member == null) return "Trường";

            var displayNameAttr = member.Member.GetCustomAttribute<DisplayNameAttribute>();
            if (displayNameAttr != null)
                return displayNameAttr.DisplayName;

            return member.Member.Name ?? "Trường";
        }

        public static string CapitalizeFirstLetter(string input)
        {
            input = input.Trim().ToLower(); // loại bỏ khoảng trắng + thường hóa
            return char.ToUpper(input[0]) + input.Substring(1);
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
        private static readonly JsonSerializerOptions serializerOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        public static string FormatJson(string json)
        {
            try
            {
                var jsonDocument = JsonSerializer.Deserialize<JsonDocument>(json);
                return JsonSerializer.Serialize(jsonDocument, serializerOptions);
            }
            catch
            {
                return json;
            }
        }

        public static string CleanHTMLTag(string? input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }
            return Regex.Replace(input, "<[^>]+>", " ");
        }
    }
}
