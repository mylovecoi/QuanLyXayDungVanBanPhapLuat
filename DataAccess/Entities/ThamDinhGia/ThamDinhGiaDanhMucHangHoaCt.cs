using DataAccess.Entities.KeKhaiDangKyGia;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities.ThamDinhGia
{
    public class ThamDinhGiaDanhMucHangHoaCt : BaseEntity
    {
        public Guid HangHoaId { get; set; }
        [ForeignKey(nameof(HangHoaId))]
        public ThamDinhGiaDanhMucHangHoa? HangHoa { get; set; }
        public string? MaHangHoa { get; set; }
        public string? TenHangHoa { get; set; }
        public string? QuyCachChatLuong { get; set; }
        public string? ThongSoKt { get; set; }
        public string? XuatXu { get; set; }
        public string? DonViTinh { get; set; }
        public string? TrangThai { get; set; }
    }
}
