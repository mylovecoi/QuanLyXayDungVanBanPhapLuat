using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Entities.Settings;

namespace Services.DTOs.Manages.ThongTinHoSo
{
    public class ReportRequestDto
    {
        [DisplayName("Đơn Vị")]
        public Guid DonViId { get; set; }
        [DisplayName("Danh Mục Hợp Đồng")]
        public List<Guid> DanhMucHopDongIds { get; set; } = new();
        [DisplayName("Thời Điểm Báo Cáo Từ")]
        public DateTime NgayBaoCaoTu { get; set; } = DateTime.Now;
        [DisplayName("Thời Điểm Báo Cáo Đến")]
        public DateTime NgayBaoCaoDen { get; set; } = DateTime.Now;
        [DisplayName("Ngày Báo Cáo")]
        public DateTime NgayBaoCao { get; set; } = DateTime.Now;

        public bool LoaiNghiepVu { get; private set; }

        public bool IsHoSoDienTu { get; private set; }

        public void SetCongChung() => LoaiNghiepVu = true;
        public void SetChungThuc() => LoaiNghiepVu = false;
        public void SetHoSoGiay() => IsHoSoDienTu = true;
        public void SetHoSoDienTu() => IsHoSoDienTu = false;
    }
}
