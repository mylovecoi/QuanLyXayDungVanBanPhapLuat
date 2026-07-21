using DataAccess.Enums;
using System.ComponentModel;

namespace Services.DTOs.BaoCao12
{
    /// <summary>
    /// DTO cho request báo cáo 12 - Tình hình tổ chức và hoạt động công chứng
    /// </summary>
    public class BaoCao12RequestDto
    {
        [DisplayName("Đơn Vị")]
        public Guid DonViId { get; set; }

        [DisplayName("Loại Báo Cáo")]
        public LoaiBaoCao12 LoaiBaoCao { get; set; }

        [DisplayName("Thời Điểm Báo Cáo Từ")]
        public DateTime NgayBaoCaoTu { get; set; } = DateTime.Now;

        [DisplayName("Thời Điểm Báo Cáo Đến")]
        public DateTime NgayBaoCaoDen { get; set; } = DateTime.Now;

        [DisplayName("Ngày Báo Cáo")]
        public DateTime NgayBaoCao { get; set; } = DateTime.Now;

        [DisplayName("Kỳ Báo Cáo")]
        public KyBaoCao12 KyBaoCao { get; set; }

        [DisplayName("Danh Mục Hợp Đồng")]
        public List<Guid> DanhMucHopDongIds { get; set; } = new();

        // Trường chung - luôn có
        [DisplayName("Người Lập Biểu")]
        public string? NguoiLapBieu { get; set; }

        // Trường riêng tùy loại báo cáo - nullable
        [DisplayName("Đơn Vị Nhận Báo Cáo")]
        public string? TenDonViNhanBaoCao { get; set; }

        [DisplayName("Người Kiểm Tra")]
        public string? NguoiKiemTra { get; set; }

        [DisplayName("Chức Vụ Người Kiểm Tra")]
        public string? ChucVuNguoiKiemTra { get; set; }

        // Các chức danh lãnh đạo khác nhau tùy loại báo cáo
        [DisplayName("Chủ Tịch UBND")] // 12a
        public string? ChuTichUBND { get; set; }

        [DisplayName("Trưởng Phòng")] // 12a
        public string? TruongPhong { get; set; }

        [DisplayName("Giám Đốc")] // 12a
        public string? GiamDoc { get; set; }

        // Trường chung để lưu tên đơn vị từ user
        [DisplayName("Tên Đơn Vị Báo Cáo")]
        public string? TenDonViBaoCao { get; set; }

        // Properties riêng cho BaoCao12
        public bool LoaiNghiepVu { get; private set; }
        public bool IsHoSoDienTu { get; private set; }

        public void SetCongChung() => LoaiNghiepVu = true;
        public void SetChungThuc() => LoaiNghiepVu = false;
        public void SetHoSoGiay() => IsHoSoDienTu = true;
        public void SetHoSoDienTu() => IsHoSoDienTu = false;
    }
}


