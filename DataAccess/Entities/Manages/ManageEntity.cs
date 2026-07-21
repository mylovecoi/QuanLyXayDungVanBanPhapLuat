using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities.Manages
{
    public class ManageEntity : BaseEntity
    {
        public string? Status { get; set; } //CC, CD, DD, BTL
        public bool Public { get; set; } = false;
        [DisplayName("Đơn Vị Quản Lý")]
        public Guid DonVi { get; set; }
        public Guid DonViTiepNhan { get; set; }
        public string? DonVisDongChuyen { get; set; } //Guids ngăn cách ,
        public DateTime NgayChuyen { get; set; }
        public string? ThongTinChuyen { get; set; }
        public string? LyDoTraLai { get; set; }
        [DisplayName("Số Quyết Định Phê Duyệt")]
        public string? SoQDDuyet { get; set; }
        [DisplayName("Ngày Quyết Định Phê Duyệt")]
        public DateTime NgayDuyet { get; set; }
        public string? ThongTinDuyet { get; set; }
        
        [NotMapped]
        public string? TenDonVi { get; set; }
        [NotMapped]
        public string? TenDonViTiepNhan { get; set; }
    }
}
