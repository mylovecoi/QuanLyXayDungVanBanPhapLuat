using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities.DinhGiaHHDV
{
    public class GiaThiTruongDanhMuc : BaseEntity
    {
        [DisplayName("Tên thông tư")]
        public string? TenTT { get; set; }
        [DisplayName("Thời điểm ban hành thông tư")]
        public DateTime ThoiDiemBanHanhTT { get; set; }
        [DisplayName("Theo dõi")]
        public string TheoDoi { get; set; } = "TD";

        //Trang thái kết nối CSDLQG
        [DisplayName("Kết nối CSDLQG")]
        public string? TrangThaiCSDLQG { get; set; }
        public DateTime NgayKetNoi { get; set; }
    }
}
