using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Services
{  
    public class SmartCAService
    {
        private readonly HttpClient _http;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public class SmartCAGetTranInfoResponse
        {
            public int code { get; set; }
            public string? message { get; set; }
            public SmartCAGetTranInfoContent? content { get; set; }
        }

        public class SmartCAGetTranInfoContent
        {
            public string? tranStatusDesc { get; set; }
            public List<SmartCADocument>? documents { get; set; }
        }

        public class SmartCADocument
        {
            public string? name { get; set; }
            public string? dataSigned { get; set; }
        }

        public SmartCAService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _http = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        public async Task<string?> GetAccessTokenAsync(string username, string password)
        {
            var form = new Dictionary<string, string>
            {
                ["grant_type"] = "password",
                ["client_id"] = _configuration["SmartCASettings:CLIENT_ID"]!,
                ["client_secret"] = _configuration["SmartCASettings:CLIENT_SECRET"]!,
                ["username"] = username,
                ["password"] = password
            };

            var response = await _http.PostAsync(_configuration["SmartCASettings:AUTH_URL"], new FormUrlEncodedContent(form));
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            dynamic? result = JsonConvert.DeserializeObject(json);
          
            return result?.access_token;
        }

        public async Task<string?> GetCredentialIdAsync(string token)
        {
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _http.PostAsync(_configuration["SmartCASettings:CREDENTIALS_URL"], new StringContent("{}", Encoding.UTF8, "application/json"));
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            dynamic? result = JsonConvert.DeserializeObject(json);
            return result?.content[0];
        }

        public async Task<(string? TranId, string? Message)> SignFileAsync(IFormFile file, string token, string credentialId)
        {
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            var hash = SHA256.Create().ComputeHash(ms.ToArray());
            var base64Hash = Convert.ToBase64String(hash);

            var request = _httpContextAccessor.HttpContext?.Request;
            if (request == null)
                return (null, "Không lấy được HttpContext");

            var host = $"{request.Scheme}://{request.Host}";
            var urlCallBack = $"{host}/smartca/callback";

            var payload = new
            {
                credentialId,
                refTranId = Guid.NewGuid().ToString(),
                notifyUrl = urlCallBack, // optional
                description = "Ký file PDF",
                datas = new[]
                {
                new { name = file.FileName, hash = base64Hash }
            }
            };

            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

            var response = await _http.PostAsync(_configuration["SmartCASettings:SIGNATURE_URL"], content);
            var json = await response.Content.ReadAsStringAsync();
            dynamic? result = JsonConvert.DeserializeObject(json);

            return (result?.content?.tranId, result?.message);
        }

        public async Task<SmartCAGetTranInfoContent?> CheckTransactionAsync(string tranId, string token)
        {
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var payload = JsonConvert.SerializeObject(new { tranId });
            var response = await _http.PostAsync(_configuration["SmartCASettings:TRANSACTION_URL"], new StringContent(payload, Encoding.UTF8, "application/json"));
            var json = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<SmartCAGetTranInfoResponse>(json);
            return result?.content;
        }

        public async Task<(bool Success, string? Message)> ConfirmOtpAsync(string tranId, string otp, string accessToken)
        {
            if (string.IsNullOrWhiteSpace(accessToken))
                return (false, "AccessToken không được để trống");

            try
            {
                var payload = new
                {
                    tranId,
                    otp
                };

                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

                var url = _configuration["SmartCASettings:AUTHORIZE_URL"];
                var response = await _http.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                    return (false, $"Lỗi khi gửi yêu cầu xác thực OTP: {response.StatusCode}");

                var json = await response.Content.ReadAsStringAsync();
                dynamic? result = JsonConvert.DeserializeObject(json);

                bool success =
                    result?.code == "00" ||
                    result?.message?.ToString().Contains("success", StringComparison.OrdinalIgnoreCase) == true ||
                    result?.content?.status == "APPROVED";

                string message = result?.message?.ToString() ?? "Không rõ phản hồi từ SmartCA";

                return (success, message);
            }
            catch (Exception ex)
            {
                return (false, $"Exception khi xác thực OTP: {ex.Message}");
            }
        }
    }
}
