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
        public int? TongThoiGianXayDungNgay { get; set; }
        public int? TongThoiGianQuyDinhNgay { get; set; }
        public decimal? TyLeThoiGianXayDung { get; set; }
        public decimal? DiemTienDoXayDung { get; set; }
        public decimal? DiemChatLuongVanBan { get; set; }
        public decimal? TongDiemDanhGia { get; set; }
        public string? XepLoaiDanhGia { get; set; }
        public DateTime? NgayChamDiem { get; set; }
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
        public List<HoSoVanBanDonDocHistoryItemModel> LichSuDonDocs { get; set; } = new();
        public List<HoSoVanBanGiaHanHistoryItemModel> LichSuGiaHans { get; set; } = new();
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
        public string? ChuTheBanHanh { get; set; }
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
        public decimal? DiemTienDoXayDung { get; set; }
        public decimal? DiemChatLuongVanBan { get; set; }
        public decimal? TongDiemDanhGia { get; set; }
        public string? XepLoaiDanhGia { get; set; }
        public bool DaCoBanGhiChamDiem { get; set; }
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
        public string MucCanhBao { get; set; } = "BINH_THUONG";
        public string TenMucCanhBao { get; set; } = "Bình thường";
        public string MaMauCanhBao { get; set; } = "#28A745";
        public int? SoNgayConLai { get; set; }
        public int? SoNgayQuaHanHienTai { get; set; }
        public string? GhiChuCanhBao { get; set; }
        public int SoLanGiaHan { get; set; }
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

    public class HoSoVanBanKetQuaLayYKienFormModel
    {
        public Guid Id { get; set; }
        public Guid HoSoVanBanId { get; set; }
        public string TenHoSo { get; set; } = string.Empty;
        public string? TenLoaiVanBan { get; set; }
        public string? TenDonViSoanThao { get; set; }
        public string? TenBuocHienTai { get; set; }
        public int LanLayYKien { get; set; } = 1;
        public string CoQuanLayYKien { get; set; } = "UBND";
        public string CheDoNhapYKien { get; set; } = "TONG_HOP";
        public string? HinhThucLayYKien { get; set; }
        public string? SoVanBanLayYKien { get; set; }
        public DateTime? NgayGuiLayYKien { get; set; }
        public DateTime? HanPhanHoi { get; set; }
        public DateTime? NgayCoKetQua { get; set; }
        public string? NoiDungYeuCau { get; set; }
        public int? TongSoThanhVien { get; set; }
        public int? SoDongY { get; set; }
        public int? SoDongYCoYKien { get; set; }
        public int? SoKhongDongY { get; set; }
        public int? SoKhongPhanHoi { get; set; }
        public decimal? TyLeDongY { get; set; }
        public string? KetQuaChung { get; set; }
        public string? NoiDungTongHop { get; set; }
        public string? NoiDungTiepThu { get; set; }
        public string TrangThai { get; set; } = "NHAP";
        public Guid? AttachedFileGroupId { get; set; }
        public string? GhiChu { get; set; }
        public List<HoSoVanBanYKienThanhVienModel> ThanhViens { get; set; } = new();
    }

    public class HoSoVanBanYKienThanhVienModel
    {
        public Guid Id { get; set; }
        public Guid? ThanhVienId { get; set; }
        public string HoTenThanhVien { get; set; } = string.Empty;
        public string? ChucVu { get; set; }
        public Guid? DonViId { get; set; }
        public string? TenDonVi { get; set; }
        public int ThuTuHienThi { get; set; }
        public bool CoQuyenBieuQuyet { get; set; } = true;
        public string? KetQuaYKien { get; set; }
        public string? NoiDungYKien { get; set; }
        public string? NoiDungTiepThu { get; set; }
        public DateTime? NgayPhanHoi { get; set; }
        public Guid? AttachedFileGroupId { get; set; }
        public string? GhiChu { get; set; }
    }

    public class GuidTextOptionModel
    {
        public Guid Id { get; set; }
        public string Text { get; set; } = string.Empty;
    }

    public class HoSoVanBanBanHanhFormModel
    {
        public Guid HoSoVanBanId { get; set; }
        public string TenHoSo { get; set; } = string.Empty;
        public string? TenLoaiVanBan { get; set; }
        public string? TenDonViSoanThao { get; set; }
        public string? TenBuocHienTai { get; set; }
        public Guid AttachedFileGroupId { get; set; }
        public string? LoaiVanBanBanHanh { get; set; }
        public string? SoKyHieuBanHanh { get; set; }
        public string? TrichYeuBanHanh { get; set; }
        public Guid? CoQuanBanHanhId { get; set; }
        public Guid? NguoiKyId { get; set; }
        public string? HoTenNguoiKy { get; set; }
        public string? ChucVuNguoiKy { get; set; }
        public DateTime? NgayKy { get; set; }
        public DateTime? NgayBanHanh { get; set; }
        public DateTime? NgayCoHieuLuc { get; set; }
        public DateTime? NgayHetHieuLuc { get; set; }
        public string TrangThaiBanHanh { get; set; } = "CHUA_BAN_HANH";
        public Guid? VanBanPhapLuatId { get; set; }
        public DateTime? NgayCongKhai { get; set; }
        public string? DuongDanCongKhai { get; set; }
        public Guid? QuyetDinhBanHanhFileId { get; set; }
        public int? TongThoiGianXayDungNgay { get; set; }
        public int? TongThoiGianQuyDinhNgay { get; set; }
        public decimal? TyLeThoiGianXayDung { get; set; }
        public decimal? DiemTienDoXayDung { get; set; }
        public decimal? DiemChatLuongVanBan { get; set; }
        public decimal? TongDiemDanhGia { get; set; }
        public string? XepLoaiDanhGia { get; set; }
        public DateTime? NgayChamDiem { get; set; }
        public string? GhiChu { get; set; }
        public List<DonViOptionModel> CoQuanBanHanhOptions { get; set; } = new();
        public List<GuidTextOptionModel> NguoiKyOptions { get; set; } = new();
        public List<GuidTextOptionModel> QuyetDinhFileOptions { get; set; } = new();
    }

    public class HoSoVanBanChamDiemFormModel
    {
        public Guid? Id { get; set; }
        public Guid HoSoVanBanId { get; set; }
        public string TenHoSo { get; set; } = string.Empty;
        public string? MaHoSo { get; set; }
        public string? TenLoaiVanBan { get; set; }
        public string? TenDonViSoanThao { get; set; }
        public DateTime NgayTaoHoSo { get; set; }
        public DateTime? NgayBanHanh { get; set; }
        public int SoLanTraLaiHienTai { get; set; }
        public int? TongThoiGianXayDungNgay { get; set; }
        public int? TongThoiGianQuyDinhNgay { get; set; }
        public decimal? TyLeThoiGianXayDung { get; set; }
        public DateTime NgayChamDiem { get; set; } = DateTime.Today;
        public string TrangThai { get; set; } = "NHAP";
        public decimal TongDiem { get; set; }
        public string? XepLoai { get; set; }
        public string? GhiChu { get; set; }
        public bool DaCoBanGhiChamDiem { get; set; }
        public List<HoSoVanBanChamDiemChiTietFormModel> ChiTiets { get; set; } = new();
    }

    public class HoSoVanBanChamDiemChiTietFormModel
    {
        public Guid? Id { get; set; }
        public Guid DanhMucTieuChiDiemId { get; set; }
        public string MaTieuChi { get; set; } = string.Empty;
        public string TenTieuChi { get; set; } = string.Empty;
        public string LoaiTieuChi { get; set; } = string.Empty;
        public decimal DiemToiDa { get; set; }
        public decimal? GiaTriTinhDiem { get; set; }
        public string? DienGiaiGiaTri { get; set; }
        public decimal? DiemDeXuat { get; set; }
        public decimal DiemChinhThuc { get; set; }
        public string? GhiChu { get; set; }
    }

    public class HoSoVanBanDonDocFormModel
    {
        public Guid HoSoVanBanId { get; set; }
        public string TenHoSo { get; set; } = string.Empty;
        public string? MaHoSo { get; set; }
        public string? TenLoaiVanBan { get; set; }
        public string? TenBuocHienTai { get; set; }
        public string? TenDonViSoanThao { get; set; }
        public string? TenDonViXuLyHienTai { get; set; }
        public DateTime? HanXuLy { get; set; }
        public string MucCanhBao { get; set; } = "BINH_THUONG";
        public string? GhiChuCanhBao { get; set; }
        public string NoiDungDonDoc { get; set; } = string.Empty;
    }

    public class HoSoVanBanDonDocHangLoatFormModel
    {
        public List<Guid> HoSoVanBanIds { get; set; } = new();
        public int TongSoHoSo { get; set; }
        public string NoiDungDonDoc { get; set; } = string.Empty;
        public List<HoSoVanBanDonDocHangLoatItemModel> HoSos { get; set; } = new();
    }

    public class HoSoVanBanDonDocHangLoatItemModel
    {
        public Guid HoSoVanBanId { get; set; }
        public string MaHoSo { get; set; } = string.Empty;
        public string TenHoSo { get; set; } = string.Empty;
        public string? TenBuocHienTai { get; set; }
        public string? TenDonViXuLyHienTai { get; set; }
        public string? TenMucCanhBao { get; set; }
        public DateTime? HanXuLy { get; set; }
    }

    public class HoSoVanBanDonDocHistoryItemModel
    {
        public Guid Id { get; set; }
        public Guid DonViGuiId { get; set; }
        public string? TenDonViGui { get; set; }
        public Guid DonViNhanId { get; set; }
        public string? TenDonViNhan { get; set; }
        public string NoiDung { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public bool DaXem { get; set; }
    }

    public class HoSoVanBanGiaHanFormModel
    {
        public Guid? Id { get; set; }
        public Guid HoSoVanBanId { get; set; }
        public string TenHoSo { get; set; } = string.Empty;
        public string? MaHoSo { get; set; }
        public string? TenLoaiVanBan { get; set; }
        public string? TenBuocHienTai { get; set; }
        public string? TenDonViSoanThao { get; set; }
        public string? TenDonViXuLyHienTai { get; set; }
        public DateTime? HanXuLyHienTai { get; set; }
        public DateTime HanXuLyMoi { get; set; } = DateTime.Today.AddDays(7);
        public int SoNgayGiaHan { get; set; }
        public string? LyDoGiaHan { get; set; }
        public Guid AttachedFileGroupId { get; set; } = Guid.NewGuid();
        public string? GhiChu { get; set; }
        public List<HoSoVanBanGiaHanHistoryItemModel> LichSus { get; set; } = new();
    }

    public class HoSoVanBanGiaHanHistoryItemModel
    {
        public Guid Id { get; set; }
        public DateTime HanXuLyCu { get; set; }
        public DateTime HanXuLyMoi { get; set; }
        public int SoNgayGiaHan { get; set; }
        public string? LyDoGiaHan { get; set; }
        public string? TenNguoiGiaHan { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? AttachedFileGroupId { get; set; }
        public string? GhiChu { get; set; }
    }
}
