using DataAccess;
using DataAccess.Entities.KeKhaiDangKyGia;
using DataAccess.Entities.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.KeKhaiDangKyGia;
using UI.Helper;
using UI.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Controllers.Admin.KeKhaiDangKyGia
{
    [SetViewDataFilter]
    [AllowAnonymous]
    public class DoanhNghiepLvKdController(
        IDoanhNghiepService doanhNghiepService,
        ApplicationDbContext dbContext) : BaseController
    {
        private readonly IDoanhNghiepService _doanhNghiepService = doanhNghiepService;
        private readonly ApplicationDbContext _dbContext = dbContext;

        private string ViewPath(string viewName) => $"../Admin/KeKhaiDangKyGia/DoanhNghiep/LvKd/{viewName}";

        public IActionResult Index()
        {
            return View();
        }

        private async Task<(string tableHtml, string selectHtml)> RenderLvKdDataAsync(string maSoThue)
        {
            var dn = await _doanhNghiepService.GetOrCreateTempDoanhNghiepAsync(maSoThue);
            var lvkdList = await _doanhNghiepService.GetLvkdByDoanhNghiepIdAsync(dn.Id);

            // Set up ViewData for the partial view rendering
            var dsdonvi = await _dbContext.DanhMucDonVis.OrderBy(t => t.STTSapXep).ToListAsync();
            ViewData["DanhMucDonVi"] = dsdonvi;
            ViewData["DanhMucKinhDoanhNghe"] = await _dbContext.DanhMucKinhDoanhs.Where(t => (t.Level > 0 || t.PhanLoai == "Detail") && t.LoaiGia == "KKG").OrderBy(t => t.STTSapXep).ToListAsync();
            ViewData["DoanhNghiepLvKd"] = lvkdList;

            // Render Lvkd table html
            string tableHtml = StaticViewRenderHelper.RenderRazorViewToString(this, ViewPath("Index"), dn);

            var selectHtml = "";
            var uniqueDonViIds = lvkdList.Select(x => x.DonViQuanLyId).Distinct().ToList();
            if (uniqueDonViIds.Any())
            {
                var units = dsdonvi.Where(x => uniqueDonViIds.Contains(x.Id)).ToList();
                selectHtml = "<div class=\"col-xl-12\" id=\"listcoquanxetduyet_data\">" +
                             "<div class=\"form-group text-left\">" +
                             "<label class=\"font-size-h7 font-weight-bolder text-dark\">Cơ quan quản lý tài khoản<span class=\"text-danger\">*</span></label>" +
                             "<select class=\"form-control h-auto py-3 px-6 border-0 rounded-lg font-size-h7\" name=\"DonViQuanLyId\" id=\"DonViQuanLyId\" required>" +
                             "<option value=\"\">--Chọn cơ quan quản lý--</option>";
                foreach (var unit in units)
                {
                    var selected = dn.DonViQuanLyId == unit.Id ? "selected" : "";
                    selectHtml += $"<option value=\"{unit.Id}\" {selected}>&emsp;{unit.TenDonVi}</option>";
                }
                selectHtml += "</select></div></div>";
            }
            else
            {
                selectHtml = "<div id=\"listcoquanxetduyet_data\" style=\"display:none\"></div>";
            }

            return (tableHtml, selectHtml);
        }

        [HttpPost("DoanhNghiep/DangKy/Lvkd/CheckMaSoThue")]
        public async Task<IActionResult> CheckMaSoThue(string MaSoThue)
        {
            if (string.IsNullOrEmpty(MaSoThue))
            {
                return Json(new { status = "error", message = "Mã số thuế không được để trống!" });
            }

            var dn = await _dbContext.DoanhNghieps.FirstOrDefaultAsync(t => t.MaSoThue == MaSoThue);
            if (dn != null && dn.TrangThai != "CXD")
            {
                return Json(new { status = "error", message = "Mã số thuế này đã được đăng ký tài khoản trên hệ thống!" });
            }

            List<DoanhNghiepLvKd> lvkdList = new List<DoanhNghiepLvKd>();
            if (dn != null)
            {
                lvkdList = await _dbContext.DoanhNghiepLvKds.Where(t => t.DoanhNghiepQuanLyId == dn.Id).ToListAsync();
            }

            var dsdonvi = await _dbContext.DanhMucDonVis.OrderBy(t => t.STTSapXep).ToListAsync();
            ViewData["DanhMucDonVi"] = dsdonvi;
            ViewData["DanhMucKinhDoanhNghe"] = await _dbContext.DanhMucKinhDoanhs.Where(t => (t.Level > 0 || t.PhanLoai == "Detail") && t.LoaiGia == "KKG").OrderBy(t => t.STTSapXep).ToListAsync();
            ViewData["DoanhNghiepLvKd"] = lvkdList;

            string tableHtml = StaticViewRenderHelper.RenderRazorViewToString(this, ViewPath("Index"), dn ?? new DoanhNghiep { MaSoThue = MaSoThue });

            var selectHtml = "";
            var uniqueDonViIds = lvkdList.Select(x => x.DonViQuanLyId).Distinct().ToList();
            if (uniqueDonViIds.Any())
            {
                var units = dsdonvi.Where(x => uniqueDonViIds.Contains(x.Id)).ToList();
                selectHtml = "<div class=\"col-xl-12\" id=\"listcoquanxetduyet_data\">" +
                             "<div class=\"form-group text-left\">" +
                             "<label class=\"font-size-h7 font-weight-bolder text-dark\">Cơ quan quản lý xét duyệt đăng ký tài khoản<span class=\"text-danger\">*</span></label>" +
                             "<select class=\"form-control h-auto py-3 px-6 border-0 rounded-lg font-size-h7\" name=\"DonViQuanLyId\" id=\"DonViQuanLyId\" required>" +
                             "<option value=\"\">--Chọn cơ quan xét duyệt--</option>";
                foreach (var unit in units)
                {
                    var selected = (dn != null && dn.DonViQuanLyId == unit.Id) ? "selected" : "";
                    selectHtml += $"<option value=\"{unit.Id}\" {selected}>&emsp;{unit.TenDonVi}</option>";
                }
                selectHtml += "</select></div></div>";
            }
            else
            {
                selectHtml = "<div id=\"listcoquanxetduyet_data\" style=\"display:none\"></div>";
            }

            return Json(new { status = "success", message = tableHtml, message2 = selectHtml });
        }

        [HttpPost("DoanhNghiep/DangKy/Lvkd/Store")]
        public async Task<IActionResult> Store(string MaNghe, Guid DonViQuanLyId, string MaSoThue)
        {
            if (string.IsNullOrEmpty(MaSoThue))
            {
                return Json(new { status = "error", message = "Mã số thuế không được để trống!" });
            }

            if (DonViQuanLyId == Guid.Empty)
            {
                return Json(new { status = "error", message = "Vui lòng chọn đơn vị nhận hồ sơ!" });
            }

            var selectedNghe = await _dbContext.DanhMucKinhDoanhs.FirstOrDefaultAsync(x => x.MaNghe == MaNghe);
            if (selectedNghe == null)
            {
                return Json(new { status = "error", message = "Ngành nghề không hợp lệ!" });
            }

            // Check if this profession has any receiving units configured
            bool hasUnits = false;
            if (!string.IsNullOrEmpty(selectedNghe.DonViQuanLyId))
            {
                var guids = selectedNghe.DonViQuanLyId.Split(',').Select(x => Guid.TryParse(x, out var g) ? g : Guid.Empty).Where(g => g != Guid.Empty).ToList();
                if (guids.Any())
                {
                    hasUnits = await _dbContext.DanhMucDonVis.AnyAsync(x => guids.Contains(x.Id));
                }
            }

            if (!hasUnits)
            {
                return Json(new { status = "error", message = "Mã nghề này chưa cấu hình đơn vị nhận hồ sơ, không được thêm!" });
            }

            var result = await _doanhNghiepService.StoreLvKdAsync(MaSoThue, selectedNghe.MaNganh ?? "", MaNghe, DonViQuanLyId);
            if (result.Status == "success")
            {
                var (tableHtml, selectHtml) = await RenderLvKdDataAsync(MaSoThue);
                return Json(new { status = "success", message = tableHtml, message2 = selectHtml });
            }

            return Json(new { status = "error", message = result.Message });
        }

        [HttpPost("DoanhNghiep/DangKy/Lvkd/Edit")]
        public async Task<IActionResult> Edit(Guid Id)
        {
            var lvkd = await _doanhNghiepService.GetLvKdByIdAsync(Id);
            if (lvkd == null)
            {
                return Json(new { status = "error", message = "Không tìm thấy thông tin!" });
            }

            var listKinhDoanhNganh = await _dbContext.DanhMucKinhDoanhs.Where(t => (t.Level == 0 || t.PhanLoai == "Group") && t.LoaiGia == "KKG").OrderBy(t => t.STTSapXep).ToListAsync();
            var listKinhDoanhNghe = await _dbContext.DanhMucKinhDoanhs.Where(t => (t.Level > 0 || t.PhanLoai == "Detail") && t.LoaiGia == "KKG").OrderBy(t => t.STTSapXep).ToListAsync();
            
            var selectedNghe = listKinhDoanhNghe.FirstOrDefault(x => x.MaNghe == lvkd.MaNghe);
            var allowedUnits = new List<DanhMucDonVi>();
            if (selectedNghe != null && !string.IsNullOrEmpty(selectedNghe.DonViQuanLyId))
            {
                var guids = selectedNghe.DonViQuanLyId.Split(',').Select(x => Guid.TryParse(x, out var g) ? g : Guid.Empty).Where(g => g != Guid.Empty).ToList();
                allowedUnits = await _dbContext.DanhMucDonVis.Where(x => guids.Contains(x.Id)).ToListAsync();
            }

            var html = "<div class=\"row text-left\" id=\"edit_thongtin\">" +
                       $"<input type=\"hidden\" id=\"id_edit\" value=\"{lvkd.Id}\" />" +
                       "<div class=\"col-xl-12\"><div class=\"form-group\"><label style=\"font-weight:bold\">Danh mục kê khai đăng ký giá</label>" +
                       "<select class=\"form-control select2basic\" id=\"manghe_edit\" name=\"manghe_edit\" style=\"width: 100%\">";
            foreach (var nganh in listKinhDoanhNganh)
            {
                html += $"<optgroup label=\"{nganh.TenNghe}\">";
                var ngheList = listKinhDoanhNghe.Where(t => t.MaNganh == nganh.MaNghe).ToList();
                foreach (var item in ngheList)
                {
                    var selected = item.MaNghe == lvkd.MaNghe ? "selected" : "";
                    html += $"<option value=\"{item.MaNghe}\" {selected}>&emsp;{item.TenNghe}</option>";
                }
                html += "</optgroup>";
            }
            html += "</select></div></div>" +
                    "<div class=\"col-xl-12\"><div class=\"form-group\"><label style=\"font-weight:bold\">Đơn vị nhận hồ sơ</label>" +
                    "<select class=\"form-control select2basic\" id=\"madvhs_edit\" name=\"madvhs_edit\" style=\"width: 100%\">" +
                    "<option value=\"\">--Chọn đơn vị nhận hồ sơ--</option>";
            foreach (var unit in allowedUnits)
            {
                var selected = unit.Id == lvkd.DonViQuanLyId ? "selected" : "";
                html += $"<option value=\"{unit.Id}\" {selected}>&emsp;{unit.TenDonVi}</option>";
            }
            html += "</select></div></div></div>";

            return Json(new { status = "success", message = html });
        }

        [HttpPost("DoanhNghiep/DangKy/Lvkd/Update")]
        public async Task<IActionResult> Update(Guid Id, string MaNghe, Guid DonViQuanLyId, string MaSoThue)
        {
            if (DonViQuanLyId == Guid.Empty)
            {
                return Json(new { status = "error", message = "Vui lòng chọn đơn vị nhận hồ sơ!" });
            }

            var selectedNghe = await _dbContext.DanhMucKinhDoanhs.FirstOrDefaultAsync(x => x.MaNghe == MaNghe);
            if (selectedNghe == null)
            {
                return Json(new { status = "error", message = "Ngành nghề không hợp lệ!" });
            }

            // Check if this profession has any receiving units configured
            bool hasUnits = false;
            if (!string.IsNullOrEmpty(selectedNghe.DonViQuanLyId))
            {
                var guids = selectedNghe.DonViQuanLyId.Split(',').Select(x => Guid.TryParse(x, out var g) ? g : Guid.Empty).Where(g => g != Guid.Empty).ToList();
                if (guids.Any())
                {
                    hasUnits = await _dbContext.DanhMucDonVis.AnyAsync(x => guids.Contains(x.Id));
                }
            }

            if (!hasUnits)
            {
                return Json(new { status = "error", message = "Mã nghề này chưa cấu hình đơn vị nhận hồ sơ, không được cập nhật!" });
            }

            var result = await _doanhNghiepService.UpdateLvKdAsync(Id, selectedNghe.MaNganh ?? "", MaNghe, DonViQuanLyId);
            if (result.Status == "success")
            {
                var (tableHtml, selectHtml) = await RenderLvKdDataAsync(MaSoThue);
                return Json(new { status = "success", message = tableHtml, message2 = selectHtml });
            }

            return Json(new { status = "error", message = result.Message });
        }

        [HttpPost("DoanhNghiep/DangKy/Lvkd/Delete")]
        public async Task<IActionResult> Delete(Guid Id)
        {
            var lvkd = await _doanhNghiepService.GetLvKdByIdAsync(Id);
            if (lvkd == null)
            {
                return Json(new { status = "error", message = "Không tìm thấy thông tin!" });
            }

            var dn = await _dbContext.DoanhNghieps.FindAsync(lvkd.DoanhNghiepQuanLyId);
            var result = await _doanhNghiepService.DeleteLvKdAsync(Id);
            if (result.Status == "success" && dn != null)
            {
                var (tableHtml, selectHtml) = await RenderLvKdDataAsync(dn.MaSoThue ?? "");
                return Json(new { status = "success", message = tableHtml, message2 = selectHtml });
            }

            return Json(new { status = "error", message = result.Message });
        }

        [HttpPost("Ajax/GetDvNhanHs")]
        public async Task<IActionResult> GetDvNhanHs(string MaNghe, string KeySelect)
        {
            var selectedNghe = await _dbContext.DanhMucKinhDoanhs.FirstOrDefaultAsync(x => x.MaNghe == MaNghe);
            if (selectedNghe == null)
            {
                return Json(new { status = "error", message = "Không tìm thấy ngành nghề tương ứng!" });
            }

            var optionsHtml = $"<option value=\"\">--Chọn đơn vị nhận hồ sơ--</option>";
            bool hasUnits = false;

            if (!string.IsNullOrEmpty(selectedNghe.DonViQuanLyId))
            {
                var guids = selectedNghe.DonViQuanLyId.Split(',').Select(x => Guid.TryParse(x, out var g) ? g : Guid.Empty).Where(g => g != Guid.Empty).ToList();
                if (guids.Any())
                {
                    var units = await _dbContext.DanhMucDonVis.Where(x => guids.Contains(x.Id)).ToListAsync();
                    if (units.Any())
                    {
                        hasUnits = true;
                        foreach (var unit in units)
                        {
                            optionsHtml += $"<option value=\"{unit.Id}\">&emsp;{unit.TenDonVi}</option>";
                        }
                    }
                }
            }

            if (!hasUnits)
            {
                return Json(new { status = "error", message = "Mã nghề này chưa cấu hình đơn vị nhận hồ sơ, không được thêm!" });
            }

            return Json(new { status = "success", message = optionsHtml });
        }
    }
}
