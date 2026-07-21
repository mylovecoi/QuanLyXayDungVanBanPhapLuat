using Microsoft.AspNetCore.Mvc;
using Services.Settings;
using System.Net;
using System.Text;

namespace UI.Controllers.Admin
{

    public class GlobalController(IDanhMucDonViService danhMucDonViService, IDanhMucDiaDanhService danhMucDiaDanhService) : Controller
    {
        private readonly IDanhMucDonViService _danhMucDonViService = danhMucDonViService;
        private readonly IDanhMucDiaDanhService _danhMucDiaDanhService = danhMucDiaDanhService;
        private ISession? _session => HttpContext?.Session;

        [HttpPost("Global/GetDonViTiepNhan")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(Guid Id)
        {
            if (_session == null || string.IsNullOrEmpty(_session.GetString("SsAdmin")))
            {
                return Json(new { status = "error", message = "Bạn đã kết thúc phiên làm việc! Vui lòng đăng nhập lại" });
            }
            var data = await _danhMucDonViService.GetDanhMucDonViChuQuanByIdAsync(Id);
            if (data == null)
            {
                return Json(new { status = "error", message = "Không tìm thấy thông tin đơn vị chủ quản" });
            }
            else
            {
                if (data.Count() == 0)
                    return Json(new { status = "error", message = "Không tìm thấy thông tin đơn vị chủ quản" });
            }
            StringBuilder result = new StringBuilder();

            foreach (var item in data)
            {
                result.AppendFormat("<option value='{0}'>{1}</option>",
                                       item.Id,
                                       WebUtility.HtmlEncode(item.TenDonVi) // Mã hóa HTML để tránh lỗi hiển thị
                                   );
            }
            return Json(new { status = "success", message = result.ToString() });
        }

        [HttpGet("Global/GetDanhMucDiaDanh")]
        public async Task<IActionResult> GetDanhMucDiaDanh(Guid danhMucId)
        {
            if (_session == null || string.IsNullOrEmpty(_session.GetString("SsAdmin")))
            {
                return Json(new { isValid = false, message = "Bạn đã kết thúc phiên làm việc! Vui lòng đăng nhập lại" });
            }
            var response = await _danhMucDiaDanhService.GetListByParentAsync(danhMucId);
            return Json(new { isValid = true, data = response, message = "Thành công" });
        }
    }
}
