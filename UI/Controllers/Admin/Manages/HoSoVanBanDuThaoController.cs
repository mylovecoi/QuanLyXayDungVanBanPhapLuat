using Microsoft.AspNetCore.Mvc;
using Services.Manages;
using Services.Systems;
using UI.Security;

namespace UI.Controllers.Admin.Manages
{
    [SetViewDataFilter]
    public class HoSoVanBanDuThaoController(
        IHoSoVanBanDuThaoService hoSoVanBanDuThaoService,
        IAuthService authService) : Controller
    {
        private readonly IHoSoVanBanDuThaoService _hoSoVanBanDuThaoService = hoSoVanBanDuThaoService;
        private readonly IAuthService _authService = authService;

        [HttpGet("Manages/HoSoVanBanDuThao/EditPage")]
        [AuthorizeAction("Edit", "HoSoVanBan", "Index")]
        public async Task<IActionResult> EditPage(Guid id)
        {
            var model = await _hoSoVanBanDuThaoService.GetEditModelAsync(id);
            if (model.Status == "error" || model.Data is not HoSoVanBanDuThaoEditModel data)
            {
                ViewData["Messages"] = model.Message;
                ViewData["Controller"] = "DuThaoVanBan";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            if (!HasDraftAccess(data.DonViSoanThaoId))
            {
                ViewData["Messages"] = "Bạn không có quyền cập nhật hồ sơ dự thảo này.";
                ViewData["Controller"] = "DuThaoVanBan";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            return BuildEditView(data);
        }

        [HttpPost("Manages/HoSoVanBanDuThao/Save")]
        [ValidateAntiForgeryToken]
        [AuthorizeAction("Edit", "HoSoVanBan", "Index")]
        public async Task<IActionResult> Save(HoSoVanBanDuThaoEditModel request)
        {
            var currentModel = await _hoSoVanBanDuThaoService.GetEditModelAsync(request.HoSoVanBanId);
            if (currentModel.Status == "error" || currentModel.Data is not HoSoVanBanDuThaoEditModel currentData)
            {
                ViewData["Messages"] = currentModel.Message;
                ViewData["Controller"] = "DuThaoVanBan";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            if (!HasDraftAccess(currentData.DonViSoanThaoId))
            {
                ViewData["Messages"] = "Bạn không có quyền cập nhật hồ sơ dự thảo này.";
                ViewData["Controller"] = "DuThaoVanBan";
                ViewData["Action"] = "Index";
                return View("Views/Shared/Error.cshtml");
            }

            NormalizeActionRequest(request);

            var saveDraft = await _hoSoVanBanDuThaoService.SaveAsync(request);
            if (saveDraft.Status == "error")
            {
                return await ReloadEditViewWithErrorAsync(request, saveDraft.Message);
            }

            TempData["SuccessMessage"] = saveDraft.Message;
            return RedirectToAction("Index", "DuThaoVanBan");
        }

        private bool HasDraftAccess(Guid donViSoanThaoId)
        {
            var currentUser = _authService.GetUserInfo();
            if (currentUser == null)
            {
                return false;
            }

            if (currentUser.SSA)
            {
                return true;
            }

            return currentUser.DanhMucDonViId != Guid.Empty && currentUser.DanhMucDonViId == donViSoanThaoId;
        }

        private static void NormalizeActionRequest(HoSoVanBanDuThaoEditModel request)
        {
            request.ActionMode = "SAVE";
            request.TenDuThao = string.IsNullOrWhiteSpace(request.TenDuThao) ? request.TenHoSo : request.TenDuThao;
            request.SoLanDuThao = request.SoLanDuThao < 1 ? 1 : request.SoLanDuThao;
            request.TrangThaiDuThao = "DA_HOAN_THANH_DU_THAO";
            request.KetQuaThucHien = "DA_HOAN_THANH_DU_THAO";
            request.DaDuDieuKienChuyenBuoc = true;
            request.NgayCapNhatDuThao ??= DateTime.Today;
            request.NgayBaoCaoKetQua ??= request.NgayCapNhatDuThao ?? DateTime.Today;
            request.NoiDungTomTat = request.NoiDungTomTat?.Trim();
            request.NoiDungBaoCao = string.IsNullOrWhiteSpace(request.NoiDungBaoCao)
                ? request.NoiDungTomTat
                : request.NoiDungBaoCao.Trim();
        }

        private async Task<IActionResult> ReloadEditViewWithErrorAsync(HoSoVanBanDuThaoEditModel request, string message)
        {
            var reload = await _hoSoVanBanDuThaoService.GetEditModelAsync(request.HoSoVanBanId);
            if (reload.Data is HoSoVanBanDuThaoEditModel data)
            {
                data.Id = request.Id;
                data.ActionMode = "SAVE";
                data.TenDuThao = request.TenDuThao;
                data.SoLanDuThao = request.SoLanDuThao;
                data.NgayCapNhatDuThao = request.NgayCapNhatDuThao;
                data.TrangThaiDuThao = request.TrangThaiDuThao;
                data.NoiDungTomTat = request.NoiDungTomTat;
                data.KetQuaThucHien = request.KetQuaThucHien;
                data.NgayBaoCaoKetQua = request.NgayBaoCaoKetQua;
                data.NoiDungBaoCao = request.NoiDungBaoCao;
                data.DaDuDieuKienChuyenBuoc = request.DaDuDieuKienChuyenBuoc;
                data.GhiChu = request.GhiChu;

                ViewData["Messages"] = message;
                return BuildEditView(data);
            }

            ViewData["Messages"] = message;
            ViewData["Controller"] = "DuThaoVanBan";
            ViewData["Action"] = "Index";
            return View("Views/Shared/Error.cshtml");
        }

        private ViewResult BuildEditView(HoSoVanBanDuThaoEditModel data)
        {
            ViewData["Title"] = "Dự thảo văn bản";
            ViewData["PageTitle"] = "Cập nhật kết quả dự thảo";
            ViewData["PageSubtitle"] = "Chỉ đơn vị soạn thảo cập nhật kết quả và file dự thảo trên màn hình này.";
            return View("Views/Admin/Manages/HoSoVanBanDuThao/Edit.cshtml", data);
        }
    }
}
