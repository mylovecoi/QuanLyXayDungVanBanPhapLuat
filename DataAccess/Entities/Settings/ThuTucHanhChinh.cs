using DataAccess.Entities.Manages;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Entities.Settings
{
    public class ThuTucHanhChinh : BaseEntity
    {
        [Required(ErrorMessage = "Mã thủ tục hành chính không được để trống")]
        public string MaThuTuc { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tên thủ tục hành chính không được để trống")]
        public string TenThuTuc { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tên quyết định không được để trống")]
        public string TenQuyetDinh { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ngày quyết định không được để trống")]
        public DateTime NgayQuyetDinh { get; set; }

        [Required(ErrorMessage = "Cơ quan thực hiện không được để trống")]
        public string CoQuanThucHien { get; set; } = string.Empty;

        [Required(ErrorMessage = "Cách thức thực hiện không được để trống")]
        public string CachThucThucHien { get; set; } = string.Empty;

        [Required(ErrorMessage = "Đối tượng thực hiện không được để trống")]
        public string DoiTuongThucHien { get; set; } = string.Empty;

        [Required(ErrorMessage = "Trình tự thực hiện không được để trống")]
        public string TrinhTuThucHien { get; set; } = string.Empty;

        [Required(ErrorMessage = "Thời hạn giải quyết không được để trống")]
        public string ThoiHanGiaiQuyet { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phí không được để trống")]
        [Range(0, double.MaxValue, ErrorMessage = "Phí không được nhỏ hơn 0")]
        public double Phi { get; set; }

        [Required(ErrorMessage = "Lệ phí không được để trống")]
        [Range(0, double.MaxValue, ErrorMessage = "Lệ phí không được nhỏ hơn 0")]
        public double LePhi { get; set; }

        [Required(ErrorMessage = "Thành phần hồ sơ không được để trống")]
        public string ThanhPhanHoSo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Yêu cầu điều kiện không được để trống")]
        public string YeuCauDieuKien { get; set; } = string.Empty;

        [Required(ErrorMessage = "Căn cứ pháp lý không được để trống")]
        public string CanCuPhapLy { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kết quả thực hiện không được để trống")]
        public string KetQuaThucHien { get; set; } = string.Empty;

        [NotMapped]
        public List<AttachedFile> DSFileDinhKem { get; set; } = [];
    }
}