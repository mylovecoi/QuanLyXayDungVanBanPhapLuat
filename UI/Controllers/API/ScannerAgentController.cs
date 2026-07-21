using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UI.Helper;
using System.Security.Claims;

namespace YourNamespace.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScannerAgentController : ControllerBase
    {
        private readonly ILogger<ScannerAgentController> _logger;
        private readonly IConfiguration _configuration;

        public ScannerAgentController(ILogger<ScannerAgentController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [HttpPost("UploadFile")]
        public async Task<IActionResult> UploadFile([FromForm] IFormFile file, [FromForm] string AgentId)
        {
            // 1. Lấy token từ header Authorization: Bearer {token}
            var authHeader = Request.Headers["Authorization"].ToString();
            if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
                return Unauthorized("Thiếu Authorization header.");

            var token = authHeader["Bearer ".Length..].Trim();

            // 2. Giải mã token để lấy thông tin AgentId từ claim
            var tokenHandler = new JwtSecurityTokenHandler();
            var secretKey = _configuration["JwtSettings:SecretKey"];
            if (string.IsNullOrWhiteSpace(secretKey))
            {
                _logger.LogError("⚠️ SecretKey không được cấu hình.");
                return Unauthorized("Lỗi cấu hình JWT.");
            }
            var key = Encoding.ASCII.GetBytes(secretKey);

            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["JwtSettings:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["JwtSettings:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var tokenAgentId = principal.FindFirstValue("AgentId"); // lấy claim AgentId

                if (!string.Equals(tokenAgentId, AgentId, StringComparison.OrdinalIgnoreCase))
                {
                    return Unauthorized("Token không khớp với AgentId.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Token validation failed.");
                return Unauthorized("Token không hợp lệ.");
            }

            // 3. Kiểm tra file
            if (file == null || file.Length == 0)
                return BadRequest("File không hợp lệ.");

            try
            {
                var rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "FileUpload", "Scan", AgentId);
                if (!Directory.Exists(rootPath))
                {
                    Directory.CreateDirectory(rootPath);
                }

                var fileName = Path.GetFileName(file.FileName);
                var filePath = Path.Combine(rootPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var relativeUrl = $"/FileUpload/Scan/{AgentId}/{fileName}";
                return Ok(new { success = true, fileName, url = relativeUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi upload file scan.");
                return StatusCode(500, "Lỗi server khi xử lý file.");
            }
        }
    }
}