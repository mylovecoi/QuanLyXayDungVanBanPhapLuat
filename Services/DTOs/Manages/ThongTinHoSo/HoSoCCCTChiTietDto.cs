using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Entities.Settings;
using DataAccess.Entities.Systems;

namespace Services.DTOs.Manages.ThongTinHoSo
{
    public class HoSoCCCTChiTietDto
    {
        public Guid Id { get; set; }
        public Guid HoSoId { get; set; }
        public Guid HopDongChiTietId { get; set; }
        public string Title { get; set; } = "";
        public FieldType Type { get; set; } // text, number, date, select, checkbox, radio
        public int ColSize { get; set; } = 12; // 1-12
        public string? Code { get; set; } // Dùng cho select value = code trong OptionData
        public int Order { get; set; } = 0;
        public string? Value { get; set; } // Giá trị của thông tin hồ sơ
        public string? ValueRaw { get; set; } // Custom các giá trị cần hiển thị thêm
        public bool IsRequired { get; set; } = false;
        public List<OptionData> Options { get; set; } = new List<OptionData>();
    }
}
