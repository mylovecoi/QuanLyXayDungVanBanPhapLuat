using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities.ThamDinhGia
{
    public class ThamDinhGiaCt : BaseEntity
    {
        [DisplayName("Mã hồ sơ")]
        public Guid MaHoSo { get; set; }
        [DisplayName("Danh mục hàng hóa")]
        public Guid HangHoaId { get; set; }
        [DisplayName("Mã hàng hóa")]
        public string? MaHangHoa { get; set; }
        [DisplayName("Tên hàng hóa")]
        public string? TenHangHoa { get; set; }
        [DisplayName("Quy cách chất lượng")]
        public string? QuyCachChatLuong { get; set; }
        [DisplayName("Thông số kỹ thuật")]
        public string? ThongSoKt { get; set; }
        [DisplayName("Xuất xứ")]
        public string? XuatXu { get; set; }
        [DisplayName("Đơn vị tính")]
        public string? DonViTinh { get; set; }
        [DisplayName("Số lượng")]
        public double SoLuong { get; set; }
        [DisplayName("Đơn giá thẩm định")]
        public double DonGiaThamDinh { get; set; }
        [DisplayName("Giá trị tài sản thẩm định")]
        public double GiaTriTsThamDinh { get; set; }
        [DisplayName("Ghi chú")]
        public string? GhiChu { get; set; }
        [DisplayName("Trạng thái")]
        public string? TrangThai { get; set; }
    }
}
