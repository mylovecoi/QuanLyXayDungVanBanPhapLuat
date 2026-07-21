using DataAccess.Entities.Settings;
using Microsoft.AspNetCore.Mvc;
using Services.DTOs.Manages.ThongTinHoSo;
using Services.Manages.ThongTinHoSo;
using Services.Settings;
using Services.Settings.DanhMucDungChung.DmHopDong;
using Services.Systems;
using UI.Helper;
using UI.Security;

namespace UI.Controllers.Admin.Manages.HoSoChungThuc
{
    [Route("Manages/HoSoChungThuc/TimKiemHoSo")]
    [SetViewDataFilter]
    public class TimKiemHoSoChungThucController(IAuthService authService,
       IHoSoCCCTService hoSoCCCTService,
       IDmHopDongService dmHopDongService,
       IDanhMucDonViService danhMucDonViService) : Controller
    {
        private readonly IAuthService _authService = authService;
        private readonly IHoSoCCCTService _hoSoCCCTService = hoSoCCCTService;
        private readonly IDmHopDongService _dmHopDongService = dmHopDongService;
        private readonly IDanhMucDonViService _danhMucDonViService = danhMucDonViService;
        private string ViewPath(string viewName) => $"../Admin/Manages/HoSoChungThuc/TimKiemHoSo/{viewName}";

        [AuthorizeAction(nameof(Index))]
        public async Task<IActionResult> Index()
        {
            var filter = new HoSoFilter(Request, loaiNghiepVu: false, _authService);
            filter.SetStatus("HT");
            var response = await _hoSoCCCTService.GetListByFilterAsync(filter);
            ViewData["DanhMucHopDong"] = (await _dmHopDongService.GetListByFilterAsync(new(1000, false))).Data ?? new List<DanhMucHopDong>();
            ViewData["LoaiHopDong"] = filter.LoaiHopDong;
            ViewData["DanhMucDonVi"] = await _danhMucDonViService.GetDanhMucDonViChuQuanBySession();
            ViewData["DonViId"] = filter.DonViId;
            ViewData["TargetYear"] = filter.TargetYear;
            var pageInfo = FuntionGlobal.GetPageInfo(response.TotalRecord, filter.Search, filter.PageSize, filter.PageCurrent, response.Data);
            return View(ViewPath(nameof(Index)), pageInfo);
        }
    }
}

