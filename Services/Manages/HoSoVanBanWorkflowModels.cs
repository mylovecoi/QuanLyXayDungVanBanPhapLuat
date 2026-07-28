namespace Services.Manages
{
    public class HoSoVanBanCreateModel
    {
        public Guid Id { get; set; }
        public Guid? DonViDeNghiId { get; set; }
        public string TenHoSo { get; set; } = string.Empty;
        public Guid DanhMucVanBanId { get; set; }
        public Guid QuyTrinhSoanThaoId { get; set; }
        public DateTime? HanXuLy { get; set; }
        public DateTime? TuNgaySoanThao { get; set; }
        public DateTime? DenNgaySoanThao { get; set; }
        public Guid? AttachedFileGroupId { get; set; }
        public string? MoTa { get; set; }
        public string? GhiChu { get; set; }
        public List<HoSoVanBanBuocThoiHanEditModel> BuocThoiHans { get; set; } = new();
    }

    public class HoSoVanBanBuocThoiHanEditModel
    {
        public Guid BuocQuyTrinhId { get; set; }
        public string MaBuoc { get; set; } = string.Empty;
        public string TenBuoc { get; set; } = string.Empty;
        public int ThuTuSapXep { get; set; }
        public int? SoNgayXuLy { get; set; }
        public int? SoNgayCanhBaoSapHan { get; set; }
        public string? GhiChu { get; set; }
    }

    public class HoSoVanBanXuLyStepModel
    {
        public Guid HoSoVanBanId { get; set; }
        public string? MaHoSo { get; set; }
        public string? TenHoSo { get; set; }
        public string KetQuaXuLy { get; set; } = "HOAN_THANH";
        public string? NoiDungXuLy { get; set; }
        public DateTime? NgayXuLy { get; set; }
        public DateTime? HanXuLy { get; set; }
        public Guid? AttachedFileGroupId { get; set; }
        public Guid? DonViTiepNhanId { get; set; }
        public Guid? DanhMucTrangThaiId { get; set; }
        public string? GhiChu { get; set; }
    }

    public class DonViOptionModel
    {
        public Guid Id { get; set; }
        public string TenDonVi { get; set; } = string.Empty;
    }

    public class HoSoVanBanLayYKienStepModel
    {
        public Guid HoSoVanBanId { get; set; }
        public Guid? NguoiDuocLayYKienId { get; set; }
        public Guid? DonViDuocLayYKienId { get; set; }
        public string? NoiDungYeuCau { get; set; }
        public string? NoiDungPhanHoi { get; set; }
        public DateTime? HanPhanHoi { get; set; }
        public DateTime? NgayPhanHoi { get; set; }
        public string TrangThaiPhanHoi { get; set; } = "DA_CO_Y_KIEN";
        public Guid? AttachedFileGroupId { get; set; }
        public string? GhiChu { get; set; }
    }

    public class HoSoVanBanDanhGiaStepModel
    {
        public Guid HoSoVanBanId { get; set; }
        public Guid? NguoiDanhGiaId { get; set; }
        public string KetQuaDanhGia { get; set; } = "DAT";
        public string? NoiDungDanhGia { get; set; }
        public string? YeuCauChinhSua { get; set; }
        public Guid? AttachedFileGroupId { get; set; }
        public string? TraLaiBuocMa { get; set; }
        public string? GhiChu { get; set; }
    }

    public class HoSoVanBanPhanHoiDanhGiaModel
    {
        public Guid HoSoVanBanId { get; set; }
        public Guid HoSoVanBanDanhGiaId { get; set; }
        public string? NoiDungGiaiTrinh { get; set; }
        public Guid? AttachedFileGroupId { get; set; }
        public string? GhiChu { get; set; }
    }

    public class HoSoVanBanWorkflowDetailModel
    {
        public Guid Id { get; set; }
        public string MaHoSo { get; set; } = string.Empty;
        public string TenHoSo { get; set; } = string.Empty;
        public string? TenLoaiVanBan { get; set; }
        public string? TenQuyTrinh { get; set; }
        public string? TenBuocHienTai { get; set; }
        public string? MaBuocHienTai { get; set; }
        public Guid? BuocHienTaiId { get; set; }
        public string? LoaiBuocHienTai { get; set; }
        public string? TenTrangThai { get; set; }
        public string? MaMauTrangThai { get; set; }
        public string? TenDonViSoanThao { get; set; }
        public Guid? DonViXuLyHienTaiId { get; set; }
        public Guid? NguoiXuLyHienTaiId { get; set; }
        public DateTime? NgayNhanHienTai { get; set; }
        public string? TrangThaiNghiepVuTiepNhan { get; set; }
        public string? TenTrangThaiNghiepVuTiepNhan { get; set; }
        public string? NoiDungXuLyHienTai { get; set; }
        public int SoLanTraLaiHienTai { get; set; }
        public DateTime NgayTaoHoSo { get; set; }
        public DateTime? HanXuLy { get; set; }
        public DateTime? NgayHoanThanh { get; set; }
        public string? MoTa { get; set; }
        public string? GhiChu { get; set; }
        public bool CanXuLyBuocHienTai { get; set; }
        public bool CanNhanHoSo { get; set; }
        public bool DaNhanHoSo { get; set; }
        public HoSoVanBanTienDoSummaryModel TienDoSummary { get; set; } = new();
        public List<HoSoVanBanBuocTheoDoiModel> CacBuocTheoDoi { get; set; } = new();
        public List<string> BuocTraLaiOptions { get; set; } = new();
        public List<string> KetQuaXuLyOptions { get; set; } = new();
        public string? KetQuaXuLyMacDinh { get; set; }
        public string? TieuDeXuLyBuoc { get; set; }
        public string? NhanNutXuLyBuoc { get; set; }
    }

    public class HoSoVanBanListItemModel
    {
        public Guid Id { get; set; }
        public string MaHoSo { get; set; } = string.Empty;
        public string TenHoSo { get; set; } = string.Empty;
        public string? MaBuocHienTai { get; set; }
        public string? TenLoaiVanBan { get; set; }
        public string? TenQuyTrinh { get; set; }
        public string? TenBuocHienTai { get; set; }
        public string? MaTrangThai { get; set; }
        public string? TenTrangThai { get; set; }
        public string? MaMauTrangThai { get; set; }
        public string? TenDonViSoanThao { get; set; }
        public Guid? DonViXuLyHienTaiId { get; set; }
        public string? TenDonViXuLyHienTai { get; set; }
        public Guid? NguoiXuLyHienTaiId { get; set; }
        public DateTime? NgayNhanHienTai { get; set; }
        public string? TrangThaiNghiepVuTiepNhan { get; set; }
        public string? TenTrangThaiNghiepVuTiepNhan { get; set; }
        public string? NoiDungXuLyHienTai { get; set; }
        public DateTime NgayTaoHoSo { get; set; }
        public DateTime? HanXuLy { get; set; }
        public DateTime? NgayHoanThanh { get; set; }
        public int SoLanTraLaiHienTai { get; set; }
        public bool CanXuLyBuocHienTai { get; set; }
        public bool CanNhanHoSo { get; set; }
        public bool DaNhanHoSo { get; set; }
        public int TongSoBuoc { get; set; }
        public int SoBuocHoanThanh { get; set; }
        public int SoBuocDungHan { get; set; }
        public int SoBuocQuaHan { get; set; }
        public int SoBuocChuaThucHien { get; set; }
        public decimal TyLeHoanThanh { get; set; }
        public string TrangThaiTienDo { get; set; } = "CHUA_THUC_HIEN";
        public string TenTrangThaiTienDo { get; set; } = "Chưa thực hiện";
        public string MaMauTienDo { get; set; } = "#CED4DA";
        public bool DangOQuaHan { get; set; }
    }

    public class HoSoVanBanTienDoSummaryModel
    {
        public int TongSoBuoc { get; set; }
        public int SoBuocHoanThanh { get; set; }
        public int SoBuocDungHan { get; set; }
        public int SoBuocQuaHan { get; set; }
        public int SoBuocChuaThucHien { get; set; }
        public int SoBuocDangXuLy { get; set; }
        public decimal TyLeHoanThanh { get; set; }
    }

    public class HoSoVanBanBuocTheoDoiModel
    {
        public Guid BuocId { get; set; }
        public string MaBuoc { get; set; } = string.Empty;
        public string TenBuoc { get; set; } = string.Empty;
        public int ThuTuSapXep { get; set; }
        public string LoaiBuoc { get; set; } = string.Empty;
        public int? SoNgayXuLyTieuChuan { get; set; }
        public int? SoNgayCanhBaoSapHan { get; set; }
        public int LanXuLy { get; set; }
        public DateTime? NgayNhan { get; set; }
        public DateTime? HanXuLy { get; set; }
        public DateTime? NgayXuLy { get; set; }
        public string? KetQuaXuLy { get; set; }
        public string? NoiDungXuLy { get; set; }
        public bool IsCurrent { get; set; }
        public string MaTrangThaiTheoDoi { get; set; } = "CHUA_THUC_HIEN";
        public string TenTrangThaiTheoDoi { get; set; } = "Chưa thực hiện";
        public string MaMauTrangThaiTheoDoi { get; set; } = "#CED4DA";
        public string? GhiChuTheoDoi { get; set; }
        public int? SoNgayTre { get; set; }
    }
}
