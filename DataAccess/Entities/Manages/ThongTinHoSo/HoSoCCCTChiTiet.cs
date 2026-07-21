using DataAccess.Entities.Settings;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Entities.Manages.ThongTinHoSo
{
    public class HoSoCCCTChiTiet : BaseEntity
    {
        public Guid HoSoId { get; set; }
        [ForeignKey(nameof(HoSoId))]
        public HoSoCCCT? HoSo { get; set; }

        public Guid DanhMucHopDongChiTietId { get; set; }
        [ForeignKey(nameof(DanhMucHopDongChiTietId))]
        public DanhMucHopDongChiTiet? Field { get; set; }

        public string? Value { get; set; }
    }
}
