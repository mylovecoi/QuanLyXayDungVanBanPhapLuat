using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities.ThamDinhGia
{
    public class ThamDinhGiaDanhMucHangHoa : BaseEntity
    {
        public string? TenDanhMucHangHoa { get; set; }
        public string? TrangThai { get; set; }
    }
}
