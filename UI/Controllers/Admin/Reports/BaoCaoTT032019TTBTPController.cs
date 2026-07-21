using DataAccess.Entities.Settings;
using DataAccess.Entities.Systems;
using DataAccess.Enums;
using Microsoft.AspNetCore.Mvc;
using Services.BaoCao12;
using Services.BaoCao17;
using Services.DTOs.BaoCao12;
using Services.DTOs.BaoCao17;
using Services.Settings;
using Services.Settings.DanhMucDungChung.DmHopDong;
using Services.Systems;
using UI.Helper;
using UI.Security;

namespace UI.Controllers.Admin.Reports
{
    [Route("Reports/BaoCaoTT032019TTBTP")]
    [SetViewDataFilter]
    public class BaoCaoTT032019TTBTPController(
        IBaoCao17Service baoCao17Service,
        IBaoCao12Service baoCao12Service,
        IUserService userService,
        IDanhMucDonViService danhMucDonViService,
        IDmHopDongService dmHopDongService) : BaseController
    {
        private static string ViewPath(string viewName) => $"../Admin/Reports/BaoCaoTT032019TTBTP/{viewName}";

        private string ViewPathMau17(string viewName) => $"../Admin/Reports/BaoCaoTT032019TTBTP/MauSo17/{viewName}";
        private string ViewPathMau12(string viewName) => $"../Admin/Reports/BaoCaoTT032019TTBTP/MauSo12/{viewName}";
        // [AuthorizeAction(nameof(Index))]
        public IActionResult Index()
        {
            ViewData["Title"] = "Báo cáo thông tư số 03/2019/TT-BTP";
            return View(ViewPath(nameof(Index)));
        }

        [HttpPost(nameof(FormFields))]
        // [AuthorizeAction("Create", "BaoCaoTT032019TTBTP", "Create")]
        public async Task<IActionResult> FormFields()
        {
            var model = new BaoCao17RequestDto();
            await InitDataForFormFields(model);
            return RenderValidationResult(true, "", model, ViewPathMau17("_FormFields"));
        }

        private async Task InitDataForFormFields(BaoCao17RequestDto model)
        {
            // Lấy thông tin user hiện tại và đơn vị mặc định từ session
            var currentUserId = FuntionGlobal.GetSsAdminId(HttpContext.Session);
            var currentDonViId = FuntionGlobal.GetSsAdminDonViId(HttpContext.Session);

            if (currentUserId == Guid.Empty || currentDonViId == Guid.Empty)
            {
                throw new Exception("Không thể xác định người dùng hiện tại hoặc đơn vị");
            }

            // Set dữ liệu mặc định cho model
            DateTime dateNow = DateTime.Now;
            model.NgayBaoCaoTu = new DateTime(dateNow.Year, 1, 1);
            model.NgayBaoCaoDen = dateNow;
            model.DonViId = currentDonViId;

            // Lấy thông tin chi tiết của user từ database thông qua UserService
            var userResponse = await userService.EditAsync(currentUserId);
            if (userResponse.Status == "success" && userResponse.Data is User currentUser)
            {
                model.TenDonViBaoCao = Services.Helper.CleanHTMLTag(currentUser.TenDonViBaoCao);
                model.DiaDanh = currentUser.DiaDanh;
            }

            // Load danh sách kỳ báo cáo
            ViewData["KyBaoCaoList"] = Services.Helper.GetEnumSelectList<KyBaoCao17>();
        }

        [HttpPost(nameof(BaoCao17a))]
        // [AuthorizeAction(nameof(Index), "BaoCaoTT032019TTBTP", nameof(Index))]
        public async Task<IActionResult> BaoCao17a(BaoCao17RequestDto request)
        {
            ViewData["Title"] = "Báo cáo 17a - Kết quả chứng thực tại UBND cấp xã";
            ViewData["BieuSo"] = "17a/BTP/HTQTCT/CT";
            ViewData["ThongTuBanHanh"] = "Thông tư số 03/2019/TT-BTP ngày 20/3/2019";

            var responseValidate = await baoCao17Service.ValidateRequestAsync(request);

            if (!responseValidate.IsValid)
            {
                ViewData["Message"] = "Dữ liệu không hợp lệ";
                responseValidate.ErrorMessages.ToList().ForEach(x => ModelState.AddModelError("", x));
                return View(ViewPathMau17(nameof(BaoCao17a)));
            }

            request.LoaiBaoCao = LoaiBaoCao17.BaoCao17a;
            var response = await baoCao17Service.GetBaoCao17aAsync(request);

            if (response.Status == "error")
            {
                ViewData["Message"] = response.Message;
                return View(ViewPathMau17(nameof(BaoCao17a)));
            }

            return View(ViewPathMau17(nameof(BaoCao17a)), response.Data);
        }

        [HttpPost(nameof(BaoCao17b))]
        // [AuthorizeAction(nameof(Index), "BaoCaoTT032019TTBTP", nameof(Index))]
        public async Task<IActionResult> BaoCao17b(BaoCao17RequestDto request)
        {
            ViewData["Title"] = "Báo cáo 17b - Kết quả chứng thực của Phòng Tư pháp và UBND cấp xã trên địa bàn huyện";
            ViewData["BieuSo"] = "17b/BTP/HTQTCT/CT";
            ViewData["ThongTuBanHanh"] = "Thông tư số 03/2019/TT-BTP ngày 20/3/2019";

            var responseValidate = await baoCao17Service.ValidateRequestAsync(request);

            if (!responseValidate.IsValid)
            {
                ViewData["Message"] = "Dữ liệu không hợp lệ";
                responseValidate.ErrorMessages.ToList().ForEach(x => ModelState.AddModelError("", x));
                return View(ViewPathMau17(nameof(BaoCao17b)));
            }

            request.LoaiBaoCao = LoaiBaoCao17.BaoCao17b;
            var response = await baoCao17Service.GetBaoCao17bAsync(request);

            if (response.Status == "error")
            {
                ViewData["Message"] = response.Message;
                return View(ViewPathMau17(nameof(BaoCao17b)));
            }

            return View(ViewPathMau17(nameof(BaoCao17b)), response.Data);
        }

        [HttpPost(nameof(BaoCao17c))]
        // [AuthorizeAction(nameof(Index), "BaoCaoTT032019TTBTP", nameof(Index))]
        public async Task<IActionResult> BaoCao17c(BaoCao17RequestDto request)
        {
            ViewData["Title"] = "Báo cáo 17c - Kết quả chứng thực của Phòng Tư pháp và UBND cấp xã trên địa bàn tỉnh";
            ViewData["BieuSo"] = "17c/BTP/HTQTCT/CT";
            ViewData["ThongTuBanHanh"] = "Thông tư số 03/2019/TT-BTP ngày 20/3/2019";

            var responseValidate = await baoCao17Service.ValidateRequestAsync(request);

            if (!responseValidate.IsValid)
            {
                ViewData["Message"] = "Dữ liệu không hợp lệ";
                responseValidate.ErrorMessages.ToList().ForEach(x => ModelState.AddModelError("", x));
                return View(ViewPathMau17(nameof(BaoCao17c)));
            }

            request.LoaiBaoCao = LoaiBaoCao17.BaoCao17c;
            var response = await baoCao17Service.GetBaoCao17cAsync(request);

            if (response.Status == "error")
            {
                ViewData["Message"] = response.Message;
                return View(ViewPathMau17(nameof(BaoCao17c)));
            }

            return View(ViewPathMau17(nameof(BaoCao17c)), response.Data);
        }

        [HttpPost(nameof(BaoCao17d))]
        // [AuthorizeAction(nameof(Index), "BaoCaoTT032019TTBTP", nameof(Index))]
        public async Task<IActionResult> BaoCao17d(BaoCao17RequestDto request)
        {
            ViewData["Title"] = "Báo cáo 17d - Kết quả chứng thực của các cơ quan đại diện Việt Nam ở nước ngoài";
            ViewData["BieuSo"] = "17d/BTP/HTQTCT/CT";
            ViewData["ThongTuBanHanh"] = "Thông tư số 03/2019/TT-BTP ngày 20/3/2019";

            var responseValidate = await baoCao17Service.ValidateRequestAsync(request);

            if (!responseValidate.IsValid)
            {
                ViewData["Message"] = "Dữ liệu không hợp lệ";
                responseValidate.ErrorMessages.ToList().ForEach(x => ModelState.AddModelError("", x));
                return View(ViewPathMau17(nameof(BaoCao17d)));
            }

            request.LoaiBaoCao = LoaiBaoCao17.BaoCao17d;
            var response = await baoCao17Service.GetBaoCao17dAsync(request);

            if (response.Status == "error")
            {
                ViewData["Message"] = response.Message;
                return View(ViewPathMau17(nameof(BaoCao17d)));
            }

            return View(ViewPathMau17(nameof(BaoCao17d)), response.Data);
        }

        #region BaoCao12 Methods

        private async Task InitDataForBaoCao12(BaoCao12RequestDto model)
        {
            // Lấy thông tin user hiện tại và đơn vị mặc định từ session
            var currentUserId = FuntionGlobal.GetSsAdminId(HttpContext.Session);
            var currentDonViId = FuntionGlobal.GetSsAdminDonViId(HttpContext.Session);

            if (currentUserId == Guid.Empty || currentDonViId == Guid.Empty)
            {
                throw new Exception("Không thể xác định người dùng hiện tại hoặc đơn vị");
            }

            // Set dữ liệu mặc định cho model
            DateTime dateNow = DateTime.Now;
            model.NgayBaoCaoTu = new DateTime(dateNow.Year, 1, 1);
            model.NgayBaoCaoDen = dateNow;
            model.DonViId = currentDonViId;

            // Lấy thông tin chi tiết của user từ database thông qua UserService
            var userResponse = await userService.EditAsync(currentUserId);
            if (userResponse.Status == "success" && userResponse.Data is User currentUser)
            {
                model.TenDonViBaoCao = Services.Helper.CleanHTMLTag(currentUser.TenDonViBaoCao);
            }

            // Load dữ liệu cho form
            ViewData["DanhMucDonVi"] = await danhMucDonViService.GetDanhMucDonViChuQuanBySession();
            ViewData["DanhMucHopDong"] = (await dmHopDongService.GetListByFilterAsync(new(1000, loaiNghiepVu: null))).Data ?? new List<DanhMucHopDong>();
            ViewData["KyBaoCaoList"] = Services.Helper.GetEnumSelectList<KyBaoCao12>();
        }

        [HttpPost(nameof(MauSo12FormFields))]
        //[AuthorizeAction("Create", "BaoCao12", "Create")]
        public async Task<IActionResult> MauSo12FormFields()
        {
            var model = new BaoCao12RequestDto();
            await InitDataForBaoCao12(model);
            return RenderValidationResult(true, "", model, ViewPathMau12("_MauSo12FormFields"));
        }

        [HttpPost(nameof(MauSo12a))]
        //[AuthorizeAction(nameof(Index), "BaoCao12", nameof(Index))]
        public async Task<IActionResult> MauSo12a(BaoCao12RequestDto request)
        {
            ViewData["Title"] = "TÌNH HÌNH TỔ CHỨC VÀ HOẠT ĐỘNG CÔNG CHỨNG";
            ViewData["BieuSo"] = "12a/BTP/BTTP/CC";
            ViewData["ThongTuBanHanh"] = "Thông tư số 03/2019/TT-BTP ngày 20/3/2019";

            var responseValidate = await baoCao12Service.ValidateRequestAsync(request);

            if (!responseValidate.IsValid)
            {
                ViewData["Message"] = "Dữ liệu không hợp lệ";
                responseValidate.ErrorMessages.ToList().ForEach(x => ModelState.AddModelError("", x));
                await InitDataForBaoCao12(request);
                return RenderValidationResult(false, "Dữ liệu không hợp lệ", request, ViewPathMau12("_MauSo12FormFields"));
            }

            request.LoaiBaoCao = LoaiBaoCao12.BaoCao12a;
            request.SetCongChung();
            var response = await baoCao12Service.GetBaoCao12aAsync(request);

            if (response.Status == "error")
            {
                ViewData["Message"] = response.Message;
                await InitDataForBaoCao12(request);
                return RenderValidationResult(false, response.Message, request, ViewPathMau12("_MauSo12FormFields"));
            }

            ViewData["ReportData"] = response.Data;
            return View(ViewPathMau12(nameof(MauSo12a)), response.Data);
        }

        [HttpPost(nameof(MauSo12b))]
        //[AuthorizeAction(nameof(Index), "BaoCao12", nameof(Index))]
        public async Task<IActionResult> MauSo12b(BaoCao12RequestDto request)
        {
            ViewData["Title"] = "TÌNH HÌNH TỔ CHỨC VÀ HOẠT ĐỘNG CÔNG CHỨNG TRÊN ĐỊA BÀN TỈNH";
            ViewData["BieuSo"] = "12b/BTP/BTTP/CC";
            ViewData["ThongTuBanHanh"] = "Thông tư số 03/2019/TT-BTP ngày 20/3/2019";

            var responseValidate = await baoCao12Service.ValidateRequestAsync(request);

            if (!responseValidate.IsValid)
            {
                ViewData["Message"] = "Dữ liệu không hợp lệ";
                responseValidate.ErrorMessages.ToList().ForEach(x => ModelState.AddModelError("", x));
                await InitDataForBaoCao12(request);
                return RenderValidationResult(false, "Dữ liệu không hợp lệ", request, ViewPathMau12("_MauSo12FormFields"));
            }

            request.LoaiBaoCao = LoaiBaoCao12.BaoCao12b;
            request.SetCongChung();
            var response = await baoCao12Service.GetBaoCao12bAsync(request);

            if (response.Status == "error")
            {
                ViewData["Message"] = response.Message;
                await InitDataForBaoCao12(request);
                return RenderValidationResult(false, response.Message, request, ViewPathMau12("_MauSo12FormFields"));
            }

            ViewData["ReportData"] = response.Data;
            return View(ViewPathMau12(nameof(MauSo12b)), response.Data);
        }

        #endregion
    }
}