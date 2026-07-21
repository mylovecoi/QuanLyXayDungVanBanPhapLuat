using DataAccess.Entities.Settings;
using DataAccess.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Entities.Manages
{
    public class ThongTinNganChan : BaseEntity
    {
        [ForeignKey(nameof(DanhMucDonVi))]
        public Guid DonViBanHanhId { get; set; }

        [Required(ErrorMessage = "Cơ quan ban hành không được để trống")]
        public string CoQuanBanHanh { get; set; } = string.Empty;

        public DanhMucDonVi? DonViBanHanh { get; set; }

        [Required(ErrorMessage = "Số quyết định không được để trống")]
        public string SoQuyetDinh { get; set; } = string.Empty;

        public DateTime NgayQuyetDinh { get; set; }
        public DateTime NgayApDung { get; set; }
        public TrangThaiNganChan TrangThai { get; set; }

        [Required(ErrorMessage = "Thông tin tài sản không được để trống")]
        public string ThongTinTaiSan { get; set; } = string.Empty;

        [NotMapped]
        public List<AttachedFile> DSHopDongDinhKem { get; set; } = [];

        public string? SoQuyetDinhDung { get; set; }
        public DateTime? NgayQuyetDinhDung { get; set; }
        public DateTime? NgayApDungDung { get; set; }
        public string? CoQuanDung { get; set; }
    }
}
