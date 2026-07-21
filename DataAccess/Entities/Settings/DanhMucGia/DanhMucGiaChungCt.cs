using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities.Settings.DanhMucGia
{
    public class DanhMucGiaChungCt : BaseEntity
    {
        public Guid DanhMucGiaChungId { get; set; }
        [ForeignKey(nameof(DanhMucGiaChungId))]
        public DanhMucGiaChung? DanhMucGiaChung { get; set; }
        public string? MaNghe { get; set; }
        [DisplayName("Mã chi tiết")]
        public string? MaChiTiet { get; set; }
        [DisplayName("Tên chi tiết")]
        public string? TenChiTiet { get; set; }
        public int STTSapXep { get; set; }
    }
}
