using DataAccess.Entities.Systems;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Entities.Settings
{
    public class DanhMucPhiLePhi : BaseEntity
    {
        public string MoTa { get; set; } = string.Empty;

        public double PhiCoDinh { get; set; }

        public double TyLeVuotMuc { get; set; }

        public Guid? LoaiHopDongId { get; set; }

        public double PhiToiDa { get; set; }

        public double NguongVuotMuc { get; set; }                   // gia tri bat dau tinh theo ty le vuot muc

        public string DonViTinh { get; set; } = string.Empty;

        public int SoLuongToiDa { get; set; }

        public Guid? PhanLoaiId { get; set; }

        [ForeignKey(nameof(LoaiHopDongId))]
        public DanhMucHopDong? LoaiHopDong { get; set; }

        [ForeignKey(nameof(PhanLoaiId))]
        public OptionData? PhanLoai { get; set; }

        [NotMapped]
        public string? strPhiCoDinh { get; set; }

        [NotMapped]
        public string? strPhiToiDa { get; set; }

        [NotMapped]
        public string? strNguongVuotMuc { get; set; }

        [NotMapped]
        public string? strTyLeVuotMuc { get; set; }
    }
}
