using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities.KeKhaiDangKyGia
{
    public class KeKhaiDangKyGiaDMHH
    {
        public Guid Id { get; set; }
        public Guid DoanhNghiepQuanLyID { get; set; }
        public Guid DonViQuanLyId { get; set; } = Guid.Empty;
        public string? MaNghe { get; set; }

        public string? MaDVCU { get; set; }
        public string? TenDvCungUng { get; set; }
        public string? QuyCachChatLuong { get; set; }
        public string? DonViTinh { get; set; }
        public string? GhiChu { get; set; }
        public string? MaHH_BTC { get; set; } //Mã kê khai theo BTC

        //Trang thái kết nối CSDLQG
        public string? TrangThaiCSDLQG { get; set; }
        public DateTime NgayKetNoi { get; set; }
    }
}
