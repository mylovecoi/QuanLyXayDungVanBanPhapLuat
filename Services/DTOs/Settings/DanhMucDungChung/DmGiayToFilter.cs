using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Services.DTOs.Settings.DanhMucDungChung
{
    public class DmGiayToFilter : BaseFilterDTO
    {
        public Guid HopDongId { get; private set; }
        public DmGiayToFilter()
        {
        }

        public DmGiayToFilter(HttpRequest request) : base(request)
        {
            HopDongId = Guid.TryParse(request.Query["HopDongId"], out var hopDongId) ? hopDongId : Guid.Empty;
        }

        public DmGiayToFilter(int pageSize) : base(pageSize)
        {
            SetDefaultPaging(1, pageSize);
        }
    }
}
