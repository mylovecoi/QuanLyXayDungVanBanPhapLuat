using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities.DinhGiaHHDV
{
    public class GiaThiTruongDanhMucCt : BaseEntity
    {
        public Guid ThongTuId { get; set; }
        [JsonProperty("MA_HHDV")]
        [DisplayName("Mã hàng hóa dịch vụ")]
        public string? MaHhDv { get; set; }
        [DisplayName("Tên hàng hóa dịch vụ")]
        public string? TenHhDv { get; set; }
        [JsonProperty("DAC_DIEM_KY_THUAT")]
        [DisplayName("Đặc điểm kỹ thuật")]
        public string? DacDiemKt { get; set; }
        [JsonProperty("DON_VI_TINH")]
        [DisplayName("Đơn vị tính")]
        public string? DonViTinh { get; set; }
        [DisplayName("Theo dõi")]
        public string TheoDoi { get; set; } = "TD";
        public string? STTSapXep { get; set; }
    }
}
