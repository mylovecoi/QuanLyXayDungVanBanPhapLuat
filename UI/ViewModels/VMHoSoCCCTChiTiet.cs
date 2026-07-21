using DataAccess.Entities.Settings;
using DataAccess.Entities.Systems;

namespace UI.ViewModels
{
    public class VMHoSoCCCTChiTiet
    {
        public Guid ThongTinHoSoId { get; set; }
        public Guid DanhMucThongTinHopDongId { get; set; }
        public string Title { get; set; } = "";
        public FieldType Type { get; set; } // text, number, date, select, checkbox, radio, money, textArea
        public int ColSize { get; set; } = 12; // 1-12
        public string? Code { get; set; } // Dùng cho select value = code trong OptionData
        public string? Value { get; set; } // Giá trị của thông tin hồ sơ
        public List<OptionData> Options { get; set; } = new List<OptionData>();
    }
}
