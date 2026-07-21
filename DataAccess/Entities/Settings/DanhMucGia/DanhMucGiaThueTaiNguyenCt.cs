using DataAccess.Entities.DinhGiaHHDV.ChiTiet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities.Settings.DanhMucGia
{
    public class DanhMucGiaThueTaiNguyenCt : BaseEntity
    {
        public Guid DanhMucGiaThueTaiNguyenId { get; set; }
        [ForeignKey(nameof(DanhMucGiaThueTaiNguyenId))]
        public DanhMucGiaThueTaiNguyen? DanhMucGiaThueTaiNguyen { get; set; }
        [DisplayName("Cấp I")]
        public string? Cap1 { get; set; }
        [DisplayName("Cấp II")]
        public string? Cap2 { get; set; }
        [DisplayName("Cấp III")]
        public string? Cap3 { get; set; }
        [DisplayName("Cấp IV")]
        public string? Cap4 { get; set; }
        [DisplayName("Cấp V")]
        public string? Cap5 { get; set; }
        [DisplayName("Cấp VI")]
        public string? Cap6 { get; set; }
        [DisplayName("Tên nhóm, loại tài nguyên")]
        public string? Ten { get; set; }
        [DisplayName("Đơn vị tính")]
        public string? DonViTinh { get; set; }
        public int STTSapXep { get; set; }
    }
}
