using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Entities.Manages;
using DataAccess.Entities.Settings;

namespace Services.DTOs
{
    public class HomeResponseDto
    {
        public List<ThuTucHanhChinh> ThuTucHanhChinhs { get; set; } = new List<ThuTucHanhChinh>();
        public List<AttachedFile> VanBanPhapLuats { get; set; } = new List<AttachedFile>();
    }

    public class HoSoTheoThangResponseDto
    {
        public List<int> CongChung { get; set; } = new();
        public List<int> ChungThuc { get; set; } = new();
    }
}
