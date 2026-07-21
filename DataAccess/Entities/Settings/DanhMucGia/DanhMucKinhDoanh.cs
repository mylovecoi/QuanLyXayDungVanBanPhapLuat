using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Entities.Settings.DanhMucGia
{
    public class DanhMucKinhDoanh : BaseEntity
    {
        public string? MaNganh { get; set; }
        public string? MaNghe { get; set; }
        public string? TenNghe { get; set; }
        public string? DonViQuanLyId { get; set; } // MaDv
        public string? DonViDongChuyenId { get; set; } // Madvdongchuyen
        public string? TheoDoi { get; set; }
        public string? PhanLoai { get; set; }
        public string? LoaiGia { get; set; }
        public string? Report { get; set; }
        public string? MaHH_BTC { get; set; } //Mã kê khai theo BTC
        public int Level { get; set; }
        public int STTSapXep { get; set; }
        public string? STTHienThi { get; set; }
        public string? Role { get; set; }
        public string? RoleGoc { get; set; }
    }
}
