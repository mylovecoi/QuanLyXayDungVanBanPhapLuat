using DataAccess.Enums;
using System.ComponentModel;

namespace Services.DTOs.BaoCao17
{
    /// <summary>
    /// DTO cho request báo cáo 17 theo Thông tư 03/2019/TT-BTP
    /// Form động tùy theo loại báo cáo
    /// </summary>
    public class BaoCao17RequestDto
    {
        [DisplayName("Đơn Vị")]
        public Guid DonViId { get; set; }

        [DisplayName("Loại Báo Cáo")]
        public LoaiBaoCao17 LoaiBaoCao { get; set; }

        [DisplayName("Thời Điểm Báo Cáo Từ")]
        public DateTime NgayBaoCaoTu { get; set; } = DateTime.Now;

        [DisplayName("Thời Điểm Báo Cáo Đến")]
        public DateTime NgayBaoCaoDen { get; set; } = DateTime.Now;

        [DisplayName("Ngày Báo Cáo")]
        public DateTime NgayBaoCao { get; set; } = DateTime.Now;

        [DisplayName("Kỳ Báo Cáo")]
        public KyBaoCao17 KyBaoCao { get; set; }

        // Trường chung - luôn có
        [DisplayName("Người Lập Biểu")]
        public string? NguoiLapBieu { get; set; }

        // Trường riêng tùy loại báo cáo - nullable
        [DisplayName("Đơn Vị Nhận Báo Cáo")]
        public string? TenDonViNhanBaoCao { get; set; }

        [DisplayName("Địa Danh")]
        public string? DiaDanh { get; set; }

        [DisplayName("Người Kiểm Tra")]
        public string? NguoiKiemTra { get; set; }

        [DisplayName("Chức Vụ Người Kiểm Tra")]
        public string? ChucVuNguoiKiemTra { get; set; }

        // Các chức danh lãnh đạo khác nhau tùy loại báo cáo
        [DisplayName("Chủ Tịch UBND")] // 17a
        public string? ChuTichUBND { get; set; }

        [DisplayName("Trưởng Phòng")] // 17b
        public string? TruongPhong { get; set; }

        [DisplayName("Giám Đốc")] // 17c
        public string? GiamDoc { get; set; }

        [DisplayName("Bộ Trưởng")] // 17d
        public string? BoTruong { get; set; }

        // Trường chung để lưu tên đơn vị từ user
        [DisplayName("Tên Đơn Vị Báo Cáo")]
        public string? TenDonViBaoCao { get; set; }
    }
}
