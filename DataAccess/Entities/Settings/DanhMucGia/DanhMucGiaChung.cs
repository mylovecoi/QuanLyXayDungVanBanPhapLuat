using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities.Settings.DanhMucGia
{
    public class DanhMucGiaChung : BaseEntity
    {
        public string? MaNghe { get; set; }
        public string? MaDanhMuc { get; set; }
        public string? TenDanhMuc { get; set; }
        public string? TrangThai { get; set; }
    }
}
