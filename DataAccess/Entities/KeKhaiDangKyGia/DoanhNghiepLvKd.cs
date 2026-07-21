using DataAccess.Entities.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities.KeKhaiDangKyGia
{
    public class DoanhNghiepLvKd : BaseEntity
    {
        public string? MaHoSo { get; set; }
        public Guid DoanhNghiepQuanLyId { get; set; }
        [ForeignKey(nameof(DoanhNghiepQuanLyId))]
        public DoanhNghiep? DoanhNghiepQuanLy { get; set; }
        public string? MaNganh { get; set; }
        public string? MaNghe { get; set; }
        public Guid DonViQuanLyId { get; set; }
        public string? TrangThai { get; set; }
    }
}
