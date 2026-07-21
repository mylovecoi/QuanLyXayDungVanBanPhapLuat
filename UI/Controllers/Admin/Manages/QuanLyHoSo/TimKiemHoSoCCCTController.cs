using Microsoft.AspNetCore.Mvc;
using Services.Settings.DanhMucDungChung.DmHopDong;
using Services.Settings;
using Services.Systems;
using UI.Security;
using Services.Manages.ThongTinHoSo;
using DataAccess.Entities.Settings;
using Services.DTOs.Manages.ThongTinHoSo;
using UI.Helper;
using UI.ViewModels;

namespace UI.Controllers.Admin.Manages.QuanLyHoSo
{
    [Route("Manages/QuanLyHoSo/TimKiemHoSoCCCT")]
    [SetViewDataFilter]
    public class TimKiemHoSoCCCTController(IAuthService authService,
        IHoSoCCCTService hoSoCCCTService,
        IDmHopDongService dmHopDongService,
        IDanhMucDonViService danhMucDonViService) : BaseController
    {
        private readonly IAuthService _authService = authService;
        private readonly IHoSoCCCTService _hoSoCCCTService = hoSoCCCTService;
        private readonly IDmHopDongService _dmHopDongService = dmHopDongService;
        private readonly IDanhMucDonViService _danhMucDonViService = danhMucDonViService;
        private string ViewPath(string viewName) => $"../Admin/Manages/QuanLyHoSo/TimKiemHoSoCCCT/{viewName}";

        [AuthorizeAction(nameof(Index))]
        public async Task<IActionResult> Index()
        {
            var filter = new HoSoFilter(Request, loaiNghiepVu: null, _authService);
            filter.SetStatus("HT");
            var response = await _hoSoCCCTService.AdvancedSearchHoSoAsync(filter);
            ViewData["DanhMucHopDong"] = (await _dmHopDongService.GetListByFilterAsync(new(1000, true))).Data ?? new List<DanhMucHopDong>();
            ViewData["DanhMucDonVi"] = await _danhMucDonViService.GetDanhMucDonViChuQuanBySession();
            ViewData["Filter"] = filter;
            var pageInfo = FuntionGlobal.GetPageInfo(response.TotalRecord, filter.Search, filter.PageSize, filter.PageCurrent, response.Data);
            return View(ViewPath(nameof(Index)), pageInfo);
        }
    }
}
