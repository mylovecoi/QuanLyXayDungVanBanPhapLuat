using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.DinhGiaHHDV.DinhGiaKhac;
using System.Threading.Tasks;
using UI.Helper;
using UI.Security;

namespace UI.Controllers.Admin.DinhGiaHHDV.DinhGiaKhac
{
    [Route("DinhGiaBaoCao")]
    [SetViewDataFilter]
    public class DinhGiaBaoCaoController(IDinhGiaService dinhGiaService) : BaseController
    {
        private readonly IDinhGiaService _dinhGiaService = dinhGiaService;

        [HttpGet("GetSoLuongDinhGiaTheoThang")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSoLuongDinhGiaTheoThang()
        {
            var response = await _dinhGiaService.GetSoLuongDinhGiaTheoThangAsync();
            return ReturnJson(response.Status == "success", response.Message, response.Data);
        }
    }
}
