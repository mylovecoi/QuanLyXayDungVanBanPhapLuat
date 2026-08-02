using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DataAccess.Entities.Systems;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Services.Manages;
using Services.Model;
using Services.Systems;
using System.Globalization;
using UI.Helper;
using UI.Security;

namespace UI.Controllers.Admin.Manages
{
    [SetViewDataFilter]
    public class TraCuuDangKyVanBanController(
        IHoSoVanBanWorkflowService hoSoVanBanWorkflowService,
        IAuthService authService) : Controller
    {
        private readonly IHoSoVanBanWorkflowService _hoSoVanBanWorkflowService = hoSoVanBanWorkflowService;
        private readonly IAuthService _authService = authService;
        private const string DefaultFilterSessionPrefix = "TraCuuDangKyVanBan.DefaultFilters.";

        [HttpGet("Manages/TraCuuDangKyVanBan")]
        [AuthorizeAction("Index", "TraCuuDangKyVanBan", "Index")]
        public async Task<IActionResult> Index(
            string TimKiem = "",
            Guid? DonViId = null,
            Guid? DanhMucVanBanId = null,
            Guid? NguoiXuLyId = null,
            string? MaTrangThai = null,
            string? MaBuoc = null,
            DateTime? TuNgayTao = null,
            DateTime? DenNgayTao = null,
            DateTime? TuHanXuLy = null,
            DateTime? DenHanXuLy = null,
            DateTime? TuNgayHoanThanh = null,
            DateTime? DenNgayHoanThanh = null,
            int PageSize = 5,
            int PageCurrent = 1)
        {
            PageCurrent = PageCurrent < 1 ? 1 : PageCurrent;
            PageSize = PageSize < 5 ? 5 : PageSize > 100 ? 100 : PageSize;

            var requestFilter = new TraCuuDangKyFilterState
            {
                TimKiem = TimKiem,
                DonViId = DonViId,
                DanhMucVanBanId = DanhMucVanBanId,
                NguoiXuLyId = NguoiXuLyId,
                MaTrangThai = MaTrangThai,
                MaBuoc = MaBuoc,
                TuNgayTao = TuNgayTao,
                DenNgayTao = DenNgayTao,
                TuHanXuLy = TuHanXuLy,
                DenHanXuLy = DenHanXuLy,
                TuNgayHoanThanh = TuNgayHoanThanh,
                DenNgayHoanThanh = DenNgayHoanThanh
            };

            var currentUser = _authService.GetUserInfo();
            var savedFilter = GetDefaultFilter(currentUser?.Id);
            var effectiveFilter = ResolveEffectiveFilter(requestFilter, savedFilter);

            var selectedDonViId = await ApplyDonViFilterViewDataAsync(effectiveFilter.DonViId);
            effectiveFilter.DonViId = selectedDonViId;

            var model = await _hoSoVanBanWorkflowService.GetDanhSachDangKyAsync(
                effectiveFilter.TimKiem ?? string.Empty,
                selectedDonViId,
                PageSize,
                PageCurrent,
                effectiveFilter.DanhMucVanBanId,
                effectiveFilter.NguoiXuLyId,
                effectiveFilter.MaTrangThai,
                effectiveFilter.MaBuoc,
                effectiveFilter.TuNgayTao,
                effectiveFilter.DenNgayTao,
                effectiveFilter.TuHanXuLy,
                effectiveFilter.DenHanXuLy,
                effectiveFilter.TuNgayHoanThanh,
                effectiveFilter.DenNgayHoanThanh);

            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "Home";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            ViewData["Title"] = "Tra cứu đăng ký văn bản";
            ViewData["Role"] = "VanBanQPPL.DangKyXayDung.TraCuuDangKyVanBan";
            ViewData["RoutePrefix"] = "/Manages/TraCuuDangKyVanBan";
            ViewData["DanhMucVanBanOptions"] = await _hoSoVanBanWorkflowService.GetDanhMucVanBanOptionsAsync();
            ViewData["NguoiXuLyOptions"] = await _hoSoVanBanWorkflowService.GetNguoiXuLyOptionsAsync(IsCurrentUserSSA(currentUser) ? selectedDonViId : currentUser?.DanhMucDonViId);
            ViewData["TrangThaiOptions"] = BuildTrangThaiOptions();
            ViewData["BuocDangKyOptions"] = BuildBuocDangKyOptions();
            ViewData["SelectedDanhMucVanBanId"] = effectiveFilter.DanhMucVanBanId;
            ViewData["SelectedNguoiXuLyId"] = effectiveFilter.NguoiXuLyId;
            ViewData["SelectedMaTrangThai"] = effectiveFilter.MaTrangThai;
            ViewData["SelectedMaBuoc"] = effectiveFilter.MaBuoc;
            ViewData["TuNgayTao"] = effectiveFilter.TuNgayTao;
            ViewData["DenNgayTao"] = effectiveFilter.DenNgayTao;
            ViewData["TuHanXuLy"] = effectiveFilter.TuHanXuLy;
            ViewData["DenHanXuLy"] = effectiveFilter.DenHanXuLy;
            ViewData["TuNgayHoanThanh"] = effectiveFilter.TuNgayHoanThanh;
            ViewData["DenNgayHoanThanh"] = effectiveFilter.DenNgayHoanThanh;
            ViewData["HasSavedDefaultFilter"] = savedFilter != null;
            ViewData["PageInfo"] = FuntionGlobal.GetPageInfo(model.TotalRecord, effectiveFilter.TimKiem ?? string.Empty, PageSize, PageCurrent);
            return View("Views/Admin/Manages/TraCuuDangKyVanBan/Index.cshtml", model.Data);
        }

        [HttpPost("Manages/TraCuuDangKyVanBan/SaveDefaultFilter")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Index", "TraCuuDangKyVanBan", "Index")]
        public JsonResult SaveDefaultFilter([FromForm] TraCuuDangKyFilterState request)
        {
            var currentUser = _authService.GetUserInfo();
            if (currentUser == null)
            {
                return Json(new { status = "error", message = "Không xác định được tài khoản đang đăng nhập." });
            }

            request.DonViId = IsCurrentUserSSA(currentUser) ? request.DonViId : currentUser.DanhMucDonViId;
            SetDefaultFilter(currentUser.Id, request);
            return Json(new { status = "success", message = "Đã lưu bộ lọc mặc định." });
        }

        [HttpPost("Manages/TraCuuDangKyVanBan/ClearDefaultFilter")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Index", "TraCuuDangKyVanBan", "Index")]
        public JsonResult ClearDefaultFilter()
        {
            var currentUser = _authService.GetUserInfo();
            if (currentUser == null)
            {
                return Json(new { status = "error", message = "Không xác định được tài khoản đang đăng nhập." });
            }

            HttpContext.Session.Remove(BuildDefaultFilterSessionKey(currentUser.Id));
            return Json(new { status = "success", message = "Đã xóa bộ lọc mặc định." });
        }

        [HttpGet("Manages/TraCuuDangKyVanBan/ExportExcel")]
        [AuthorizeAction("Index", "TraCuuDangKyVanBan", "Index")]
        public async Task<IActionResult> ExportExcel(
            string TimKiem = "",
            Guid? DonViId = null,
            Guid? DanhMucVanBanId = null,
            Guid? NguoiXuLyId = null,
            string? MaTrangThai = null,
            string? MaBuoc = null,
            DateTime? TuNgayTao = null,
            DateTime? DenNgayTao = null,
            DateTime? TuHanXuLy = null,
            DateTime? DenHanXuLy = null,
            DateTime? TuNgayHoanThanh = null,
            DateTime? DenNgayHoanThanh = null)
        {
            var currentUser = _authService.GetUserInfo();
            var selectedDonViId = IsCurrentUserSSA(currentUser)
                ? DonViId
                : (currentUser?.DanhMucDonViId != Guid.Empty ? currentUser?.DanhMucDonViId : null);

            var result = await _hoSoVanBanWorkflowService.GetDanhSachDangKyAsync(
                TimKiem,
                selectedDonViId,
                100000,
                1,
                DanhMucVanBanId,
                NguoiXuLyId,
                MaTrangThai,
                MaBuoc,
                TuNgayTao,
                DenNgayTao,
                TuHanXuLy,
                DenHanXuLy,
                TuNgayHoanThanh,
                DenNgayHoanThanh);

            if (result.Status == "error" || result.Data is not IEnumerable<HoSoVanBanListItemModel> data)
            {
                ViewData["Messages"] = result.Message;
                ViewData["Controller"] = "TraCuuDangKyVanBan";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            using var stream = new MemoryStream();
            using (var document = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook, true))
            {
                var workbookPart = document.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                var sheetData = new SheetData();
                worksheetPart.Worksheet = new Worksheet(sheetData);

                AppendRow(sheetData,
                    "Mã hồ sơ",
                    "Tên hồ sơ",
                    "Loại văn bản",
                    "Quy trình",
                    "Bước hiện tại",
                    "Đơn vị soạn thảo",
                    "Đơn vị xử lý hiện tại",
                    "Người xử lý hiện tại",
                    "Trạng thái",
                    "Ngày tạo",
                    "Hạn xử lý",
                    "Ngày hoàn thành");

                foreach (var item in data)
                {
                    AppendRow(sheetData,
                        item.MaHoSo,
                        item.TenHoSo,
                        item.TenLoaiVanBan,
                        item.TenQuyTrinh,
                        item.TenBuocHienTai,
                        item.TenDonViSoanThao,
                        item.TenDonViXuLyHienTai,
                        item.NguoiXuLyHienTaiId?.ToString(),
                        item.TenTrangThai,
                        item.NgayTaoHoSo.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                        item.HanXuLy?.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                        item.NgayHoanThanh?.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture));
                }

                var sheets = workbookPart.Workbook.AppendChild(new Sheets());
                sheets.Append(new Sheet
                {
                    Id = workbookPart.GetIdOfPart(worksheetPart),
                    SheetId = 1,
                    Name = "TraCuuDangKy"
                });

                workbookPart.Workbook.Save();
            }

            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"TraCuuDangKyVanBan_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
        }

        [HttpPost("Manages/TraCuuDangKyVanBan/Show")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Index", "TraCuuDangKyVanBan", "Index")]
        public async Task<IActionResult> Show(Guid id)
        {
            ViewData["RoutePrefix"] = "/Manages/TraCuuDangKyVanBan";
            ViewData["HideWorkflowAction"] = "true";
            var model = await _hoSoVanBanWorkflowService.GetChiTietAsync(id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "TraCuuDangKyVanBan";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            return PartialView("Views/Admin/Manages/HoSoVanBan/Show.cshtml", model.Data);
        }

        [HttpPost("Manages/TraCuuDangKyVanBan/Timeline")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Index", "TraCuuDangKyVanBan", "Index")]
        public async Task<IActionResult> Timeline(Guid id)
        {
            var model = await _hoSoVanBanWorkflowService.GetChiTietAsync(id);
            if (model.Status == "error")
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "TraCuuDangKyVanBan";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            return PartialView("Views/Admin/Manages/HoSoVanBan/Timeline.cshtml", model.Data);
        }

        private async Task<Guid?> ApplyDonViFilterViewDataAsync(Guid? donViId)
        {
            var currentUser = _authService.GetUserInfo();
            var isSSA = IsCurrentUserSSA(currentUser);
            var sessionDonViId = currentUser?.DanhMucDonViId ?? Guid.Empty;
            var selectedDonViId = isSSA
                ? (donViId.HasValue && donViId.Value != Guid.Empty ? donViId : null)
                : (sessionDonViId != Guid.Empty ? sessionDonViId : null);

            var donViOptions = await _hoSoVanBanWorkflowService.GetDonViOptionsAsync();
            if (!isSSA)
            {
                donViOptions = donViOptions.Where(x => x.Id == sessionDonViId).ToList();
            }

            var selectedDonViName = selectedDonViId.HasValue && selectedDonViId.Value != Guid.Empty
                ? donViOptions.FirstOrDefault(x => x.Id == selectedDonViId.Value)?.TenDonVi
                : null;

            ViewData["DonViOptions"] = donViOptions;
            ViewData["SelectedDonViId"] = selectedDonViId;
            ViewData["SelectedDonViName"] = selectedDonViName;
            ViewData["IsSSA"] = isSSA;
            return selectedDonViId;
        }

        private static bool IsCurrentUserSSA(User? currentUser) => currentUser?.SSA ?? false;

        private static List<SelectOptionModel> BuildTrangThaiOptions()
        {
            return new List<SelectOptionModel>
            {
                new() { Value = "KHOI_TAO", Text = "Khởi tạo" },
                new() { Value = "CHO_DUYET", Text = "Chờ duyệt" },
                new() { Value = "DANG_XU_LY", Text = "Đang xử lý" },
                new() { Value = "HOAN_THANH", Text = "Hoàn thành" }
            };
        }

        private static List<SelectOptionModel> BuildBuocDangKyOptions()
        {
            return new List<SelectOptionModel>
            {
                new() { Value = "BUOC_01_DANG_KY", Text = "Bước đăng ký" },
                new() { Value = "BUOC_02_THONG_NHAT", Text = "Bước thống nhất" },
                new() { Value = "HOAN_THANH", Text = "Đã hoàn thành" }
            };
        }

        private TraCuuDangKyFilterState? GetDefaultFilter(Guid? userId)
        {
            if (!userId.HasValue || userId.Value == Guid.Empty)
            {
                return null;
            }

            var json = HttpContext.Session.GetString(BuildDefaultFilterSessionKey(userId.Value));
            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            try
            {
                return JsonConvert.DeserializeObject<TraCuuDangKyFilterState>(json);
            }
            catch
            {
                return null;
            }
        }

        private void SetDefaultFilter(Guid userId, TraCuuDangKyFilterState filter)
        {
            HttpContext.Session.SetString(BuildDefaultFilterSessionKey(userId), JsonConvert.SerializeObject(filter));
        }

        private static string BuildDefaultFilterSessionKey(Guid userId) => $"{DefaultFilterSessionPrefix}{userId}";

        private static TraCuuDangKyFilterState ResolveEffectiveFilter(TraCuuDangKyFilterState request, TraCuuDangKyFilterState? saved)
        {
            if (HasAnyFilterValue(request) || saved == null)
            {
                return request;
            }

            return saved;
        }

        private static bool HasAnyFilterValue(TraCuuDangKyFilterState request)
        {
            return !string.IsNullOrWhiteSpace(request.TimKiem)
                   || request.DonViId.HasValue
                   || request.DanhMucVanBanId.HasValue
                   || request.NguoiXuLyId.HasValue
                   || !string.IsNullOrWhiteSpace(request.MaTrangThai)
                   || !string.IsNullOrWhiteSpace(request.MaBuoc)
                   || request.TuNgayTao.HasValue
                   || request.DenNgayTao.HasValue
                   || request.TuHanXuLy.HasValue
                   || request.DenHanXuLy.HasValue
                   || request.TuNgayHoanThanh.HasValue
                   || request.DenNgayHoanThanh.HasValue;
        }

        private static void AppendRow(SheetData sheetData, params string?[] values)
        {
            var row = new Row();
            foreach (var value in values)
            {
                row.Append(new Cell
                {
                    DataType = CellValues.InlineString,
                    InlineString = new InlineString(new Text(value ?? string.Empty))
                });
            }
            sheetData.Append(row);
        }

        public class TraCuuDangKyFilterState
        {
            public string? TimKiem { get; set; }
            public Guid? DonViId { get; set; }
            public Guid? DanhMucVanBanId { get; set; }
            public Guid? NguoiXuLyId { get; set; }
            public string? MaTrangThai { get; set; }
            public string? MaBuoc { get; set; }
            public DateTime? TuNgayTao { get; set; }
            public DateTime? DenNgayTao { get; set; }
            public DateTime? TuHanXuLy { get; set; }
            public DateTime? DenHanXuLy { get; set; }
            public DateTime? TuNgayHoanThanh { get; set; }
            public DateTime? DenNgayHoanThanh { get; set; }
        }
    }
}
