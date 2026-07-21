using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities.Settings
{
    public class DanhMucDonViTinh
    {
        public Guid Id { get; set; }
        public string? MaDonViTinh { get; set; }
        public string? TenDonViTinh { get; set; }
    }
}
