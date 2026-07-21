using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Services.DTOs.BaoCaoKhac
{
    public class BaoCaoSuDungLaoDongRequest
    {
        [Required(ErrorMessage = "Vui lòng chọn đơn vị")]
        [DisplayName("Đơn vị")]
        public Guid DonViId { get; set; }

        [DisplayName("Năm báo cáo")]
        public int Nam { get; private set; } = DateTime.Now.Year;

        [Required(ErrorMessage = "Vui lòng chọn từ ngày")]
        [DisplayName("Từ ngày")]
        public DateTime TuNgay { get; set; } = new DateTime(DateTime.Now.Year, 1, 1);

        [Required(ErrorMessage = "Vui lòng chọn đến ngày")]
        [DisplayName("Đến ngày")]
        public DateTime DenNgay { get; set; } = new DateTime(DateTime.Now.Year, 12, 31);

        public string? TenToChuc { get; set; }

        public string? TinhThanhPho { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập quyển số")]
        [DisplayName("Quyển số")]
        public string QuyenSo { get; set; } = string.Empty;
    }
}
