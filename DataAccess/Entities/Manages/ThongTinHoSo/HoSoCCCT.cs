using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Entities.Settings;
using DataAccess.Entities.Systems;


namespace DataAccess.Entities.Manages.ThongTinHoSo
{
    public class HoSoCCCT : ManageEntity
    {
        public HoSoCCCT()
        {
            HoSoCCCTChiTiets = new HashSet<HoSoCCCTChiTiet>();
        }

        [DisplayName("Số Hợp Đồng")]
        public string MaSoHoSo { get; set; } = string.Empty;

        [DisplayName("Đơn Vị Quản Lý")]
        public Guid DonViQuanLyId { get; set; }

        [ForeignKey(nameof(DonViQuanLyId))]
        public DanhMucDonVi? DonViQuanLy { get; set; }

        [DisplayName("Tên Hợp Đồng")]
        public Guid LoaiHopDongId { get; set; }

        [ForeignKey(nameof(LoaiHopDongId))]
        public DanhMucHopDong? LoaiHopDong { get; set; }

        // Thông tin người nộp 
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

        [DisplayName("Thông Tin Bên A")]
        public string? ThongTinBenA { get; set; }
        [DisplayName("Thông Tin Bên B")]
        public string? ThongTinBenB { get; set; }
        [DisplayName("Nội Dung Hợp Đồng")]
        public string? NoiDungHoSo { get; set; }

        //Thông tin giấy tờ chứng thực
        [DisplayName("Thông tin giấy tờ chứng thực")]
        public string? ThongTinGiayToChungThuc { get; set; }

        // Thông tin tài sản
        [DisplayName("Loại Tài Sản")]
        public Guid? LoaiTaiSanId { get; set; }

        [ForeignKey(nameof(LoaiTaiSanId))]
        public OptionData? LoaiTaiSan { get; set; } // OptionData Code = "LoaiTaiSan"

        [DisplayName("Thông Tin Chi Tiết Tài Sản")]
        public string? ThongTinChiTietTaiSan { get; set; }

        [DisplayName("Địa Bàn")]
        public Guid? DiaBanId { get; set; }

        [ForeignKey(nameof(DiaBanId))]
        public DanhMucDiaDanh? DiaBan { get; set; }

        [DisplayName("Công Chứng Viên")]
        public Guid? CongChungVienId { get; set; }

        [ForeignKey(nameof(CongChungVienId))]
        public DanhMucCanBo? CongChungVien { get; set; }


        // Thông tin ngân hàng
        [DisplayName("Ngân Hàng")]
        public string? TenNganHang { get; set; }
        [DisplayName("Cán Bộ Tín Dụng")]
        public string? CanBoTinDung { get; set; }
        [DisplayName("Chiết Khấu")]
        public int ChietKhau { get; set; }

        // Thông tin lưu trữ
        [DisplayName("Số Trang")]
        public int SoTrang { get; set; }
        [DisplayName("Số Bản Công Chứng")]
        public int SoVanBan { get; set; }
        [DisplayName("Nơi Lưu Trữ")]
        public string? NoiLuuTru { get; set; }

        [DisplayName("Giá Trị Hợp Đồng")]
        public double? GiaTriHopDong { get; set; }

        public bool? DaThanhToan { get; set; } = false; // true: đã thanh toán, false: chưa thanh toán
        public DateTime? NgayThanhToan { get; set; } // Ngày thanh toán, nếu đã thanh toán

        [NotMapped]
        public string StrSoTienMienThue
        {
            get => Helper.ConvertDblToStr(GiaTriHopDong.HasValue ? GiaTriHopDong.Value : 0);
            set => GiaTriHopDong = (double)(Helper.ConvertStrToDecimal(value) ?? 0);
        }
        [DisplayName("Trạng Thái")]
        public bool TrangThai { get; set; } = true;                      // Đang hoạt động / Không hoạt động

        [DisplayName("Mô Tả")]
        public string? MoTa { get; set; }

        public List<HoSoCCCTChiPhi> HoSoCCCTChiPhis { get; set; } = [];

        public virtual ICollection<HoSoCCCTChiTiet> HoSoCCCTChiTiets { get; set; }

        [NotMapped]
        [DisplayName("Tệp Đính Kèm")]
        public List<AttachedFile> AttachedFiles { get; set; } = new();

        [NotMapped]
        public List<DanhMucDonVi> DanhMucDonVis { get; set; } = new();

        [NotMapped]
        public List<DanhMucHopDong> DanhMucHopDongs { get; set; } = new();

        [NotMapped]
        public List<OptionData> DanhMucLoaiTaiSans { get; set; } = new();
    }
}
