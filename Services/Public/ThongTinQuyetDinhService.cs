using Services.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Public
{
    public interface IThongTinQuyetDinhService
    {
        Task<CommonResponse> GetThongTinQuyetDinhAsync(string Search, int PageSize, int PageCurrent);
    }
    public class ThongTinQuyetDinhService : IThongTinQuyetDinhService
    {
        public ThongTinQuyetDinhService()
        {

        }

        public Task<CommonResponse> GetThongTinQuyetDinhAsync(string Search, int PageSize, int PageCurrent)
        {
            throw new NotImplementedException();
        }
    }
}
