using DataAccess.Entities.ThamDinhGia;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities.Settings.DanhMucGia
{
    public class DanhMucNuocSachCt : BaseEntity
    {
        public Guid DanhMucNuocSachId { get; set; }
        [ForeignKey(nameof(DanhMucNuocSachId))]
        public DanhMucNuocSach? DanhMucNuocSach { get; set; }
        [DisplayName("Mã đối tượng")]
        public string? MaDoiTuong { get; set; }
        [DisplayName("Đối tượng sử dụng")]
        public string? DoiTuongSuDung { get; set; }
        public int STTSapXep { get; set; }
    }
}
