namespace Services.Manages
{
    public class HoSoVanBanDraftCompareModel
    {
        public Guid HoSoVanBanId { get; set; }
        public string TenHoSo { get; set; } = string.Empty;
        public string? TenLoaiVanBan { get; set; }
        public string? TenQuyTrinh { get; set; }
        public int TongSoDong { get; set; }
        public int SoDongGiongNhau { get; set; }
        public int SoDongThem { get; set; }
        public int SoDongXoa { get; set; }
        public int SoDongSua { get; set; }
        public List<HoSoVanBanDraftCompareFileOptionModel> FileOptions { get; set; } = new();
        public Guid? SourceFileId { get; set; }
        public Guid? TargetFileId { get; set; }
        public HoSoVanBanDraftCompareFileOptionModel? SourceFile { get; set; }
        public HoSoVanBanDraftCompareFileOptionModel? TargetFile { get; set; }
        public bool CoTheSoSanh { get; set; }
        public string? CanhBao { get; set; }
        public List<HoSoVanBanDraftCompareRowModel> DiffRows { get; set; } = new();
    }

    public class HoSoVanBanDraftCompareFileOptionModel
    {
        public Guid FileId { get; set; }
        public Guid GroupId { get; set; }
        public string TableName { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string? PhanLoaiDuThao { get; set; }
        public string? MoTa { get; set; }
        public string NguonHienThi { get; set; } = string.Empty;
        public string NhanHienThi { get; set; } = string.Empty;
        public DateTime? NgayTao { get; set; }
        public string? FileExtension { get; set; }
        public bool LaDocx { get; set; }
    }

    public class HoSoVanBanDraftCompareRowModel
    {
        public int Index { get; set; }
        public string LeftText { get; set; } = string.Empty;
        public string RightText { get; set; } = string.Empty;
        public string LeftHtml { get; set; } = string.Empty;
        public string RightHtml { get; set; } = string.Empty;
        public string Status { get; set; } = "same";
    }

    public class HoSoVanBanCreateModel
    {
        public Guid Id { get; set; }
        public Guid? HoSoDangKyId { get; set; }
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

    public class HoSoVanBanTaoSoanThaoTuDangKyModel
    {
        public Guid HoSoDangKyId { get; set; }
        public string TenHoSoDangKy { get; set; } = string.Empty;
        public Guid DanhMucVanBanId { get; set; }
        public string? TenLoaiVanBan { get; set; }
        public Guid? DonViSoanThaoId { get; set; }
        public Guid QuyTrinhSoanThaoId { get; set; }
        public DateTime TuNgaySoanThao { get; set; } = DateTime.Today;
        public DateTime? DenNgaySoanThao { get; set; }
        public string? GhiChu { get; set; }
        public List<HoSoVanBanBuocThoiHanEditModel> BuocThoiHans { get; set; } = new();
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
        public Guid? DefaultDonViTiepNhanId { get; set; }
        public Guid? DanhMucTrangThaiId { get; set; }
        public string? GhiChu { get; set; }
        public int? DraftVersionNumber { get; set; }
        public string? DraftVersionLabel { get; set; }
    }

    public class DonViOptionModel
    {
        public Guid Id { get; set; }
        public string TenDonVi { get; set; } = string.Empty;
    }

    public class HoSoDangKyOptionModel
    {
        public Guid Id { get; set; }
        public string TenHoSo { get; set; } = string.Empty;
        public Guid DanhMucVanBanId { get; set; }
        public string? TenLoaiVanBan { get; set; }
        public Guid? DonViSoanThaoId { get; set; }
        public string? TenDonViSoanThao { get; set; }
        public string NhanHienThi { get; set; } = string.Empty;
    }

    public class HoSoVanBanLayYKienStepModel
    {
        public Guid HoSoVanBanId { get; set; }
        public string ActionMode { get; set; } = "CAP_NHAT_KET_QUA";
        public Guid? NguoiDuocLayYKienId { get; set; }
        public Guid? DonViDuocLayYKienId { get; set; }
        public List<Guid> DonViDuocLayYKienIds { get; set; } = new();
        public List<HoSoVanBanLayYKienItemModel> CacLayYKien { get; set; } = new();
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
        public Guid? HoSoDangKyNguonId { get; set; }
        public string? TenHoSoDangKyNguon { get; set; }
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
        public List<SelectOptionModel> CheDoLayYKienOptions { get; set; } = new();
        public List<DonViOptionModel> DonViLayYKienOptions { get; set; } = new();
        public string? CheDoLayYKienHienTai { get; set; }
        public bool CoThePhanHoiLayYKien { get; set; }
        public bool CoTheTongHopLayYKien { get; set; }
        public List<HoSoVanBanLayYKienItemModel> CacLayYKien { get; set; } = new();
        public List<HoSoVanBanDuThaoVersionItemModel> CacVersionDuThao { get; set; } = new();
    }

    public class HoSoVanBanDuThaoVersionItemModel
    {
        public Guid Id { get; set; }
        public int LanVersion { get; set; }
        public int SoLanTraLai { get; set; }
        public string TenVersion { get; set; } = string.Empty;
        public Guid AttachedFileGroupId { get; set; }
        public Guid? DonViTaoId { get; set; }
        public string? TenDonViTao { get; set; }
        public Guid? NguoiTaoId { get; set; }
        public DateTime NgayTaoVersion { get; set; }
        public string LoaiVersion { get; set; } = string.Empty;
        public string? GhiChu { get; set; }
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
        public bool DaCoDuThao { get; set; }
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
        public bool CoThePhanHoiLayYKien { get; set; }
        public bool CoTheTongHopLayYKien { get; set; }
        public string? CheDoLayYKienHienTai { get; set; }
        public bool DaCoBanGhiDanhGia { get; set; }
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

    public class HoSoVanBanLayYKienItemModel
    {
        public Guid Id { get; set; }
        public Guid HoSoVanBanId { get; set; }
        public Guid? DonViDuocLayYKienId { get; set; }
        public string? TenDonViDuocLayYKien { get; set; }
        public Guid? NguoiDuocLayYKienId { get; set; }
        public string? NoiDungYeuCau { get; set; }
        public string? NoiDungPhanHoi { get; set; }
        public DateTime NgayGui { get; set; }
        public DateTime? HanPhanHoi { get; set; }
        public DateTime? NgayPhanHoi { get; set; }
        public string? TrangThaiPhanHoi { get; set; }
        public Guid? AttachedFileGroupId { get; set; }
        public string? GhiChu { get; set; }
        public bool CoTheNhapTongHop { get; set; }
    }

    public class HoSoVanBanLayYKienFormModel
    {
        public Guid HoSoVanBanId { get; set; }
        public Guid AttachedFileGroupId { get; set; }
        public Guid? DonViDuocLayYKienId { get; set; }
        public string ActionMode { get; set; } = "PHAN_HOI_DON_VI";
        public string TenHoSo { get; set; } = string.Empty;
        public string? TenLoaiVanBan { get; set; }
        public string? TenDonViSoanThao { get; set; }
        public string? TenBuocHienTai { get; set; }
        public DateTime? HanPhanHoi { get; set; }
        public DateTime? NgayPhanHoi { get; set; }
        public string? NoiDungYeuCau { get; set; }
        public string? NoiDungPhanHoi { get; set; }
        public string TrangThaiPhanHoi { get; set; } = "DA_CO_Y_KIEN";
        public string? GhiChu { get; set; }
        public bool YeuCauFileDinhKem { get; set; }
        public List<DonViOptionModel> DonViLayYKienOptions { get; set; } = new();
        public List<HoSoVanBanLayYKienItemModel> CacLayYKien { get; set; } = new();
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

    public class HoSoVanBanDuThaoEditModel
    {
        public Guid Id { get; set; }
        public Guid HoSoVanBanId { get; set; }
        public Guid DonViSoanThaoId { get; set; }
        public string ActionMode { get; set; } = "SAVE";
        public string TenHoSo { get; set; } = string.Empty;
        public string? TenLoaiVanBan { get; set; }
        public string? TenQuyTrinh { get; set; }
        public string? TenDonViSoanThao { get; set; }
        public string? TenBuocHienTai { get; set; }
        public DateTime NgayTaoHoSo { get; set; }
        public DateTime? HanXuLy { get; set; }
        public string TenDuThao { get; set; } = string.Empty;
        public int SoLanDuThao { get; set; } = 1;
        public DateTime? NgayCapNhatDuThao { get; set; }
        public string TrangThaiDuThao { get; set; } = "CHUA_CAP_NHAT";
        public string? NoiDungTomTat { get; set; }
        public string KetQuaThucHien { get; set; } = "CHUA_HOAN_THANH";
        public DateTime? NgayBaoCaoKetQua { get; set; }
        public string? NoiDungBaoCao { get; set; }
        public bool DaDuDieuKienChuyenBuoc { get; set; }
        public string? GhiChu { get; set; }
        public string DraftFileTableName { get; set; } = "HoSoVanBanDuThao";
        public string? MoTaHoSo { get; set; }
        public string? GhiChuHoSo { get; set; }
        public List<SelectOptionModel> TrangThaiDuThaoOptions { get; set; } = new();
        public List<SelectOptionModel> KetQuaThucHienOptions { get; set; } = new();
    }

    public class SelectOptionModel
    {
        public string Value { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
    }
}
