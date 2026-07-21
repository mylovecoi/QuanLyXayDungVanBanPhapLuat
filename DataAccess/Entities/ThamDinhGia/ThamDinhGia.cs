using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities.ThamDinhGia
{
    public class ThamDinhGia : BaseEntity
    {
        public Guid DiaBanId { get; set; }
        public Guid DonViQuanLyId { get; set; }
        public Guid DonViChuQuanId { get; set; }
        public Guid DonViThamDinhId { get; set; }
        public Guid HoiDongId { get; set; }
        public Guid HangHoaId { get; set; }
        public string? DiaDiem { get; set; }
        public string? DvYeuCau { get; set; }
        public DateTime ThoiHan { get; set; }
        public string? SoTbKl { get; set; }
        public string? PhanLoai { get; set; }
        public string? SoQdPheDuyet { get; set; }
        public DateTime NgayQdPheDuyet { get; set; }
        public int SoNgayKq { get; set; }
        public string? TtTsTd { get; set; }
        public string? CongBo { get; set; }
        public string? GhiChu { get; set; }
        public DateTime Thoidiem { get; set; }
        public string? LyDo { get; set; }
        public string? ThongTin { get; set; }
        public string? TrangThai { get; set; }
        public string? ChiTietExcel { get; set; }

        //Trang thái kết nối CSDLQG
        public string? TrangThaiCSDLQG { get; set; }
        public DateTime NgayKetNoi { get; set; }
        public string? Ipf1 { get; set; }
        [NotMapped]
        public IFormFile? Ipf1Upload { get; set; }
        [NotMapped]
        public List<DataAccess.Entities.Manages.AttachedFile> AttachedFiles { get; set; } = [];
    }
}
