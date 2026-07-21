using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Services.Systems;

namespace Services.DTOs.Settings.DanhMucDungChung
{
    public class DmHopDongFilter : BaseFilterDTO
    {
        public bool? LoaiNghiepVu { get; private set; }
        public DmHopDongFilter()
        {
        }

        public DmHopDongFilter(HttpRequest request) : base(request)
        {
        }

        public DmHopDongFilter(int pageSize, bool? loaiNghiepVu) : base(pageSize)
        {
            LoaiNghiepVu = loaiNghiepVu;
            SetDefaultPaging(1, pageSize);
        }

        public DmHopDongFilter(int pageSize) : base(pageSize)
        {
            SetDefaultPaging(1, pageSize);
        }
    }
}
