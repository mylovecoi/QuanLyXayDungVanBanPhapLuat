 using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities.Manages
{
    public class AttachedFile : BaseEntity
    {
        public Guid GroupId { get; set; }
        //Lưu thêm tên bảng để tránh trường hợp Guid có thể trùng giữa các bảng
        public string? TableName { get; set; }
        // URL nếu có (ví dụ: link tham chiếu đến hệ thống quản lý file khác hoặc thông tin bổ sung)
        public string? SoVanBan { get; set; } = "";
        public DateTime NgayBanHanh { get; set; } = DateTime.Now;
        public DateTime NgayApDung { get; set; } = DateTime.Now;
        [StringLength(250)]
        public string? Url { get; set; }

        // Nội dung file đính kèm, lưu trực tiếp vào SQL Server dưới dạng byte array
        public byte[]? FileContent { get; set; }

        // Tên file đính kèm
        [StringLength(100)]
        public string? FileName { get; set; }

        // Loại file (ContentType), ví dụ: "image/jpeg", "image/png"
        [StringLength(50)]
        public string? ContentType { get; set; }

        // Mô tả file đính kèm
        [StringLength(500)]
        public string? MoTa { get; set; }

        [StringLength(50)]
        public string? PhanLoaiDuThao { get; set; }

        public string? Status { get; set; } = "CXD";

        public bool Public { get; set; } = false;

        [NotMapped]
        public Guid? DonViId { get; set; }

        [NotMapped]         
        public IFormFile? FileUpLoad { get; set; }   
        
        [NotMapped]
        public string? ScannedFilePath { get; set; }
        [NotMapped]
        public string? ScannedFileName { get; set; }
        [NotMapped]
        public string? stringBase64 { get; set; }
    }
}
