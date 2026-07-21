using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities.Manages.ThongTinHoSo
{
    public class HoSoCCCTHistory : BaseEntity
    {
        public Guid HoSoId { get; set; }

        [ForeignKey(nameof(HoSoId))]
        public virtual HoSoCCCT? HoSoCCCT { get; set; }

        public string? ThongTinThayDoi { get; set; } // Lưu json {field: value, field: value}

        public string HanhDong { get; set; } = string.Empty; // "Tao", "Sua", "Xoa", "ThayDoiTrangThai"

        public string? TruongBiThayDoi { get; set; }         // VD: "TrangThaiHoSo"
        public string? GiaTriCu { get; set; }
        public string? GiaTriMoi { get; set; }

        [DisplayName("Mô Tả")]
        public string? MoTa { get; set; }
    }
}
