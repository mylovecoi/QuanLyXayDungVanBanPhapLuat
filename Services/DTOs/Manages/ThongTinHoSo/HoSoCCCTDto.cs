using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Entities.Manages.ThongTinHoSo;
using DataAccess.Entities.Manages;
using DataAccess.Entities.Settings;
using DataAccess.Entities.Systems;
using Services.Helpers;

namespace Services.DTOs.Manages.ThongTinHoSo
{
    public class HoSoCCCTDto
    {
        public Guid Id { get; set; }
        [DisplayName("Số Hợp Đồng")]
        public string MaSoHoSo { get; set; } = string.Empty;

        [DisplayName("Đơn Vị Quản Lý")]
        public Guid DonViQuanLyId { get; set; }
        public DanhMucDonVi? DonViQuanLy { get; set; }

        [DisplayName("Tên Hợp Đồng")]
        public Guid LoaiHopDongId { get; set; }
        public DanhMucHopDong? LoaiHopDong { get; set; }

        [DisplayName("Họ Tên")]
        public string? HoTenNguoiNop { get; set; }
        [DisplayName("Số Thẻ CCCD")]
        public string? SoCCCDNguoiNop { get; set; }
        [DisplayName("SĐT")]
        public string? SDTNguoiNop { get; set; }
        [DisplayName("Thông Tin Đơn Vị")]
        public string? ThongTinDonVi { get; set; }

        [DisplayName("Ngày Yêu Cầu")]

        public DateTime NgayThuLy { get; set; }

        [DisplayName("Phương Thức Công Chứng")]
        public bool PhuongThucCongChung { get; set; } // true: Bản Giấy, false: Bản Điện Tử

        //Thông tin giấy tờ chứng thực
        public string? ThongTinGiayToChungThuc { get; set; }

        public Guid? CongChungVienId { get; set; }

        [ForeignKey(nameof(CongChungVienId))]
        public DanhMucCanBo? CongChungVien { get; set; }

        [DisplayName("Giá Trị Hợp Đồng")]
        public double? GiaTriHopDong { get; set; }

        public bool? DaThanhToan { get; set; } = false; // true: đã thanh toán, false: chưa thanh toán
        public DateTime? NgayThanhToan { get; set; } // Ngày thanh toán, nếu đã thanh toán

        [NotMapped]
        public string StrSoTienMienThue
        {
            get => FunctionHelper.ConvertDblToStr(GiaTriHopDong.HasValue ? GiaTriHopDong.Value : 0);
            set => GiaTriHopDong = (double)(FunctionHelper.ConvertStrToDecimal(value) ?? 0);
        }

        public string Status { get; set; } = "CXD";
        public DateTime? NgayDuyet { get; set; }

        public Guid CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

        public Guid UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }


        public List<HoSoCCCTChiPhi> HoSoCCCTChiPhis { get; set; } = [];
        public List<AttachedFile> AttachedFiles { get; set; } = [];
        public List<HoSoCCCTChiTietDto> HoSoCCCTChiTietDtos { get; set; } = [];
    }
}
