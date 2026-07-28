using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities.Manages
{
    public class Notification : BaseEntity
    {
        public Guid DonViGui { get; set; }
        public Guid DonViTiepNhan { get; set; }
        public string? DonViDongChuyen { get; set; }
        public string? NoiDung { get; set; }
        public string? ControllerNameDanhSach { get; set; }
        public string? ActionNameDanhSach { get; set; }
        public string? ParameterDanhSach { get; set; }
        public string? ControllerNameXetDuyet { get; set; }
        public string? ActionNameXetDuyet { get; set; }
        public string? ParameterXetDuyet { get; set; }
        public List<Guid> DonViView { get; set; } = [];
        [NotMapped]
        public string? RoleDanhSach { get; set; }
        [NotMapped]
        public string? RoleXetDuyet { get; set; }
        [NotMapped]
        public string? TenDonViGuiThongBao { get; set; }
        [NotMapped]
        public bool DaXem { get; set; } = false;
        [NotMapped]
        public string? UrlDanhSach { get; set; }
        [NotMapped]
        public string? UrlXetDuyet { get; set; }
    }
}
