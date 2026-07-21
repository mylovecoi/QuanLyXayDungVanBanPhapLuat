using DataAccess.Entities.KeKhaiDangKyGia;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities.ThamDinhGia
{
    public class ThamDinhGiaHoiDongCt : BaseEntity
    {
        public Guid HoiDongId { get; set; }
        [ForeignKey(nameof(HoiDongId))]
        public ThamDinhGiaHoiDong? HoiDong { get; set; }
        public string? STTSapXep { get; set; }
        public string? HoTen { get; set; }
        public string? ChucVu { get; set; }
        public string? VaiTro { get; set; }
    }
}
