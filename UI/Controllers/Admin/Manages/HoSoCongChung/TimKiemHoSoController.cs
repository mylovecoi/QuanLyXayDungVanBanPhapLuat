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

namespace UI.Controllers.Admin.Manages.HoSoCongChung
{
    [Route("Manages/HoSoCongChung/TimKiemHoSo")]
    [SetViewDataFilter]
    public class TimKiemHoSoController(IAuthService authService,
        IHoSoCCCTService hoSoCCCTService,
        IDmHopDongService dmHopDongService,
        IDanhMucDonViService danhMucDonViService) : BaseController
    {
        private readonly IAuthService _authService = authService;
        private readonly IHoSoCCCTService _hoSoCCCTService = hoSoCCCTService;
        private readonly IDmHopDongService _dmHopDongService = dmHopDongService;
        private readonly IDanhMucDonViService _danhMucDonViService = danhMucDonViService;
        private string ViewPath(string viewName) => $"../Admin/Manages/HoSoCongChung/TimKiemHoSo/{viewName}";

        [AuthorizeAction(nameof(Index))]
        public async Task<IActionResult> Index()
        {
            var filter = new HoSoFilter(Request, loaiNghiepVu: true, _authService);
            filter.SetStatus("HT");
            var response = await _hoSoCCCTService.GetListByFilterAsync(filter);
            ViewData["DanhMucHopDong"] = (await _dmHopDongService.GetListByFilterAsync(new(1000, true))).Data ?? new List<DanhMucHopDong>();
            ViewData["LoaiHopDong"] = filter.LoaiHopDong;
            ViewData["DanhMucDonVi"] = await _danhMucDonViService.GetDanhMucDonViChuQuanBySession();
            ViewData["DonViId"] = filter.DonViId;
            ViewData["TargetYear"] = filter.TargetYear;
            var pageInfo = FuntionGlobal.GetPageInfo(response.TotalRecord, filter.Search, filter.PageSize, filter.PageCurrent, response.Data);
            return View(ViewPath(nameof(Index)), pageInfo);
        }
    }
}
