using DataAccess.Entities.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities.DinhGiaHHDV.ChiTiet
{
    public class ChiTietBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid DonViQuanLyId { get; set; } // MaDv
        [ForeignKey(nameof(DonViQuanLyId))]
        public DanhMucDonVi? DonViQuanLy { get; set; }
        [DisplayName("Mã hồ sơ")]
        public string? MaHoSo { get; set; }
        [DisplayName("Đơn vị tính")]
        public string? DonViTinh { get; set; } = null;
        public string? TrangThai { get; set; } = "CXD";
        public int STTSapXep { get; set; }
    }
}
