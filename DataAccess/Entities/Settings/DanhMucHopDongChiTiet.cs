using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataAccess.Entities.Systems;

namespace DataAccess.Entities.Settings
{
    public enum FieldType
    {
        Text, TextArea, Number, Date, Select, Checkbox, Radio, Money,
    }

    public class DanhMucHopDongChiTiet : BaseEntity
    {
        public Guid DanhMucHopDongId { get; set; }

        [ForeignKey(nameof(DanhMucHopDongId))]
        public DanhMucHopDong? DanhMucHopDong { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;
        public FieldType Type { get; set; } // text, number, date, select, checkbox, radio, money
        public int ColSize { get; set; } = 12; // 1-12
        public string? Code { get; set; } // Dùng cho select value = code trong OptionData
        public int Order { get; set; } // Thứ tự hiển thị
        public string? GhiChu { get; set; } // Ghi chú mô tả thêm   

        // NEW: Đánh dấu trường bắt buộc nhập
        public bool IsRequired { get; set; } = false;

        [NotMapped]
        public List<OptionData> Options { get; set; } = new List<OptionData>();

    }
}
