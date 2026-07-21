using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Entities.Manages.ThongTinHoSo;
using DataAccess.Entities.Systems;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Entities.Settings
{
    public class DanhMucHopDong : BaseEntity
    {
        public DanhMucHopDong()
        {
            Children = new HashSet<DanhMucHopDong>();
            HoSoCCCTs = new HashSet<HoSoCCCT>();
            HopDongChiTiet = new HashSet<DanhMucHopDongChiTiet>();
        }
        private string _tenHopDong = string.Empty;

        [DisplayName("Tên Hợp Đồng")]
        public string TenHopDong
        {
            get => _tenHopDong;
            set
            {
                _tenHopDong = value;
                NameAscii = Helper.ConvertStrToSlug(value);
            }
        }

        public string? NameAscii { get; set; }

        [DisplayName("Mã Hợp Đồng")]
        public string MaHopDong { get; set; } = string.Empty;        // Mã định danh, ví dụ: HD_CNQSDĐ

        [DisplayName("Phân Loại Nghiệp Vụ")]
        public bool IsCC { get; set; } = true;                       // Công chứng (true) / Chứng thực (false)
        //public bool LoaiNghiepVu { get; set; } // t/f "CongChung" / "ChungThuc"

        [DisplayName("Trạng Thái")]
        public bool TrangThai { get; set; } = true;                  // Đang hoạt động / Không hoạt động

        [DisplayName("Số Thứ Tự Sắp Xếp")]
        public int STTSapXep { get; set; } = 0;                      // Sắp xếp thứ tự trong form

        [DisplayName("Mô Tả")]
        public string? MoTa { get; set; }                            // Ghi chú thêm

        public virtual Guid? ParentId { get; set; }                  // Nghiệp vụ ParentId == null, hợp đồng ParentId != null
        public int Level { get; set; }

        [ForeignKey(nameof(ParentId))]
        public virtual DanhMucHopDong? Parent { get; set; }

        public virtual ICollection<DanhMucHopDong> Children { get; set; }

        public virtual ICollection<HoSoCCCT> HoSoCCCTs { get; set; }

        public virtual ICollection<DanhMucHopDongChiTiet> HopDongChiTiet { get; set; }

        [NotMapped]
        public List<DanhMucHopDong> DanhMucParents { get; set; } = new();

        public string LoaiGiayTo { get; set; } = string.Empty;       // OptionData "LoaiGiayTo", VD: "code1;code2;code3"

        [NotMapped]
        [DisplayName("Các Loại Giấy Tờ Đi Kèm")]
        public List<string> DanhSachOption
        {
            get => string.IsNullOrWhiteSpace(LoaiGiayTo) ? new List<string>() : LoaiGiayTo.Split(';').Select(s => s.Trim()).ToList();
            set => LoaiGiayTo = string.Join(";", value.Where(s => !string.IsNullOrWhiteSpace(s)));
        }

        [NotMapped]
        public List<OptionData> DanhMucLoaiGiayTos { get; set; } = new();
    }
}
