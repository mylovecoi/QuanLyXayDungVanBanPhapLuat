using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.Manages.ThongTinHoSo.ExportData
{
    public class ExportDataRequestDto
    {
        public int NamKetXuat { get; set; }
        public Guid DonViId { get; set; }
        public List<Guid> HopDongIds { get; set; } = new();
        public bool LoaiNghiepVu { get; private set; }
        public void SetCongChung() => LoaiNghiepVu = true;
        public void SetChungThuc() => LoaiNghiepVu = false;
    }
}
