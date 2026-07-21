using DataAccess.Entities.Settings;
using Microsoft.AspNetCore.Mvc;
using Services.DTOs.Settings.DanhMucDungChung;
using Services.Settings.DanhMucDungChung.DmHopDong;
using Services.Systems;
using UI.Helper;
using UI.Security;

namespace UI.Controllers.Admin.Settings.DanhMucDungChung
{
    [Route("Settings/DanhMucDungChung/DanhMucNghiepVu")]
    [SetViewDataFilter]
    public class DanhMucNghiepVuController(IDmHopDongService dmHopDongService, IOptionDataService optionDataService, IDmHopDongChiTietService dmHopDongChiTietService) : BaseController
    {
        private readonly IDmHopDongService _dmHopDongService = dmHopDongService;
        private readonly IOptionDataService _optionDataService = optionDataService;
        private readonly IDmHopDongChiTietService _dmHopDongChiTietService = dmHopDongChiTietService;
        private string ViewPath(string viewName) => $"../Admin/Settings/DanhMucDungChung/DanhMucNghiepVu/{viewName}";

        private async Task InitDataForCreateOrUpdate(DanhMucHopDong danhMuc)
        {
            danhMuc.DanhMucLoaiGiayTos = await _optionDataService.GetDataOptionsByCodeAsync("LoaiGiayTo");
        }

        [AuthorizeAction(nameof(Index))]
        public async Task<IActionResult> Index()
        {
            var filter = new DmHopDongFilter(Request);
            var response = await _dmHopDongService.GetListByFilterAsync(filter);
            var pageInfo = FuntionGlobal.GetPageInfo(response.TotalRecord, filter.Search, filter.PageSize, filter.PageCurrent, response.Data);
            return View(ViewPath(nameof(Index)), pageInfo);
        }

        [HttpGet(nameof(Create))]
        [AuthorizeAction(nameof(Create))]
        public async Task<IActionResult> Create(Guid parentId)
        {
            var response = await _dmHopDongService.GetSingleByIdWithParentAsync(Guid.Empty, parentId);
            await InitDataForCreateOrUpdate(response.Data);
            ViewData["Messages"] = response.Message;
            return PartialView(ViewPath("_FormFields"), response.Data);
        }

        [HttpPost(nameof(Store))]
        [AuthorizeAction(nameof(Store))]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Store(DanhMucHopDong request)
        {
            var validatResult = await _dmHopDongService.ValidateRequestAsync(request);
            string status = validatResult.Status, message = validatResult.Message;
            if (validatResult.Status == "error")
            {
                validatResult.ErrorMessages.ToList().ForEach(x => ModelState.AddModelError(x.Key, x.Value));
            }
            else
            {
                var response = await _dmHopDongService.StoreAsync(request);
                status = response.Status;
                message = response.Message;
            }
            if (status == "error")
                await this.InitDataForCreateOrUpdate(request);
            return RenderValidationResult(status == "success", message, request, ViewPath("_FormFields"));
        }

        [HttpGet(nameof(Edit))]
        [AuthorizeAction(nameof(Edit))]
        public async Task<IActionResult> Edit(Guid danhMucId)
        {
            var response = await _dmHopDongService.GetSingleByIdWithParentAsync(danhMucId, Guid.Empty);
            if (response.Status == "success")
                await this.InitDataForCreateOrUpdate(response.Data);
            ViewData["Messages"] = response.Message;
            return PartialView(ViewPath("_FormFields"), response.Data);
        }

        [HttpPost(nameof(Update))]
        [AuthorizeAction(nameof(Update))]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(DanhMucHopDong request)
        {
            var validatResult = await _dmHopDongService.ValidateRequestAsync(request);
            string status = validatResult.Status, message = validatResult.Message;
            if (validatResult.Status == "error")
            {
                validatResult.ErrorMessages.ToList().ForEach(x => ModelState.AddModelError(x.Key, x.Value));
            }
            else
            {
                var response = await _dmHopDongService.UpdateAsync(request);
                status = response.Status;
                message = response.Message;
            }
            if (status == "error")
                await this.InitDataForCreateOrUpdate(request);
            return RenderValidationResult(status == "success", message, request, ViewPath("_FormFields"));
        }

        [HttpPost(nameof(Delete))]
        [AuthorizeAction(nameof(Delete))]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid danhMucId)
        {
            var response = await _dmHopDongService.DeleteAsync(danhMucId);
            return RenderValidationResult(response.Status == "success", response.Message, new());
        }

        // NEW: Manage dynamic fields for a specific contract (child) danh mục
        [HttpGet(nameof(ManageFields))]
        //[AuthorizeAction(nameof(Edit))]
        public async Task<IActionResult> ManageFields(Guid danhMucId)
        {
            var response = await _dmHopDongChiTietService.GetListByDanhMucIdAsync(danhMucId);
            var danhMucHopDong = await _dmHopDongService.GetSingleByIdAsync(danhMucId);
            ViewData["DanhMucId"] = danhMucId;
            ViewData["DanhMucHopDong"] = danhMucHopDong.Data;
            return PartialView(ViewPath("_FieldsModal"), response.Data ?? new List<DanhMucHopDongChiTiet>());
        }

        [HttpGet("Fields/Create")]
        //[AuthorizeAction(nameof(Edit))]
        public IActionResult FieldCreate(Guid danhMucId)
        {
            // NEW: For create, keep Id = Guid.Empty so view posts to FieldStore
            var model = new DanhMucHopDongChiTiet { DanhMucHopDongId = danhMucId, ColSize = 12, Order = 1 };
            return PartialView(ViewPath("_FieldForm"), model);
        }

        [HttpGet("Fields/Edit")]
        //[AuthorizeAction(nameof(Edit))]
        public async Task<IActionResult> FieldEdit(Guid id)
        {
            var model = await _dmHopDongChiTietService.GetByIdAsync(id);
            return PartialView(ViewPath("_FieldForm"), model ?? new DanhMucHopDongChiTiet());
        }

        [HttpPost("Fields/Store")]
        //[AuthorizeAction(nameof(Edit))]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FieldStore(DanhMucHopDongChiTiet request)
        {
            var response = await _dmHopDongChiTietService.StoreAsync(request);
            return RenderValidationResult(response.Status == "success", response.Message, request, ViewPath("_FieldForm"));
        }

        [HttpPost("Fields/Update")]
        //[AuthorizeAction(nameof(Edit))]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FieldUpdate(DanhMucHopDongChiTiet request)
        {
            var response = await _dmHopDongChiTietService.UpdateAsync(request);
            return RenderValidationResult(response.Status == "success", response.Message, request, ViewPath("_FieldForm"));
        }

        [HttpPost("Fields/Delete")]
        //[AuthorizeAction(nameof(Delete))]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FieldDelete(Guid id)
        {
            var response = await _dmHopDongChiTietService.DeleteAsync(id);
            return RenderValidationResult(response.Status == "success", response.Message, new());
        }
    }
}
