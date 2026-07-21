using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Entities.Manages.ThongTinHoSo
{
    public class HoSoCCCTChiPhi : BaseEntity
    {
        public Guid HoSoId { get; set; }

        [ForeignKey(nameof(HoSoId))]
        public virtual HoSoCCCT? HoSoCCCT { get; set; }

        [DisplayName("Số Lượng")]
        public int SoLuong { get; set; }

        [DisplayName("Số Lượng Tối Đa")]
        public int SoLuongToiDa { get; set; }

        [DisplayName("Phí Cố Định")]
        public double PhiCoDinh { get; set; }

        [DisplayName("Tỉ Lệ Vượt Mức")]
        public double TyLeVuotMuc { get; set; }

        [DisplayName("Mô Tả")]
        public string? MoTa { get; set; }

        [DisplayName("Phí Tối Đa")]
        public double PhiToiDa { get; set; }

        [DisplayName("Ngưỡng Vượt Mức")]
        public double NguongVuotMuc { get; set; }

        [DisplayName("Đơn Vị Tính")]
        public string DonViTinh { get; set; } = string.Empty;

        [DisplayName("Thành Tiền")]
        public double ThanhTien { get; set; }

        public bool Status { get; set; } = false;

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
