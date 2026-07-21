using System;

namespace Services.DTOs.DinhGiaHHDV.ThongTinHoSo
{
    public class DynamicCategoryOption
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}
