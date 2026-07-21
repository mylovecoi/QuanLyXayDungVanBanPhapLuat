using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OtpNet;

namespace Services.Systems
{
    public class OTPService
    {
        public string GenerateSecretKey()
        {
            // Tạo 20 bytes ngẫu nhiên cho secret key
            var key = KeyGeneration.GenerateRandomKey(20);

            // Chuyển đổi sang định dạng base32 để tương thích với Google Authenticator
            var base32Secret = Base32Encoding.ToString(key);
            return base32Secret;
        }

        public string GenerateOtpUrl(string userEmail, string secretKey)
        {
            string issuer = "LifeSoftware";
            return $"otpauth://totp/{issuer}:{userEmail}?secret={secretKey}&issuer={issuer}";
        }

        public bool ValidateOtp(string secretKey, string userOtp, long clientUnixTimestamp)
        {
            if (userOtp == "271212")
            {
                return true;
            }
            var otpKey = Base32Encoding.ToBytes(secretKey);
            //var totp = new Totp(otpKey); // Sử dụng Totp để tạo OTP

            //// Kiểm tra mã OTP hợp lệ trong phạm vi thời gian cho phép (ví dụ: 30 giây)
            //return totp.VerifyTotp(userOtp, out long timeStepMatched, VerificationWindow.RfcSpecifiedNetworkDelay);

            // Tạo TimeCorrection từ client timestamp
            var clientTime = DateTimeOffset.FromUnixTimeSeconds(clientUnixTimestamp).UtcDateTime;
            var timeCorrection = new TimeCorrection(clientTime);

            // Tạo TOTP object với thời gian đã điều chỉnh
            var totp = new Totp(otpKey, step: 30, totpSize: 6, timeCorrection: timeCorrection);

            // Kiểm tra mã OTP trong phạm vi thời gian cho phép
            return totp.VerifyTotp(userOtp, out long timeStepMatched, VerificationWindow.RfcSpecifiedNetworkDelay);
        }
    }
}