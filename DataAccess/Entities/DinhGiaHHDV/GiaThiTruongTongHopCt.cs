using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities.DinhGiaHHDV
{
    public class GiaThiTruongTongHopCt : BaseEntity
    {
        public string? MaHoSo { get; set; }
        public Guid ThongTuId { get; set; }
        [DisplayName("Mã hàng hóa dịch vụ")]
        public string? MaHhDv { get; set; }
        [DisplayName("Tên hàng hóa dịch vụ")]
        public string? TenHhDv { get; set; }
        [DisplayName("Đặc điểm kỹ thuật")]
        public string? DacDiemKt { get; set; }
        [DisplayName("Đơn vị tính")]
        public string? DonViTinh { get; set; }
        [DisplayName("Giá phổ biến kỳ báo cáo")]
        public double GiaBaoCao { get; set; }
        [DisplayName("Giá bình quân kỳ trước")]
        public double GiaKyTruoc { get; set; }
        [DisplayName("Giá bình quân kỳ này")]
        public double GiaKyNay { get; set; }
        [DisplayName("Mức tăng giảm")]
        public string? MucTangGiam { get; set; }
        [DisplayName("Tỷ lệ tăng giảm")]
        public string? TyLeTangGiam { get; set; }
        [DisplayName("Loại giá")]
        public string LoaiGia { get; set; } = "Giá bán lẻ";
        [DisplayName("Nguồn thông tin")]
        public string? NguonThongTin { get; set; }
        [DisplayName("Ghi chú")]
        public string? GhiChu { get; set; }
        [DisplayName("Trạng thái")]
        public string TrangThai { get; set; } = "CXD";
        public string? STTSapXep { get; set; }
    }
}
