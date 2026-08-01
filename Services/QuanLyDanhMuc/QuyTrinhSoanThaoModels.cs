using DataAccess.Entities.QuanLyDanhMuc;

namespace Services.QuanLyDanhMuc
{
    public class QuyTrinhSoanThaoListItemModel
    {
        public Guid Id { get; set; }
        public string MaQuyTrinh { get; set; } = string.Empty;
        public string TenQuyTrinh { get; set; } = string.Empty;
        public string LoaiQuyTrinh { get; set; } = "XayDung";
        public string? TenLoaiQuyTrinh { get; set; }
        public string? TenLoaiVanBan { get; set; }
        public string? CapApDung { get; set; }
        public int PhienBan { get; set; }
        public bool TrangThai { get; set; }
        public int SoBuoc { get; set; }
        public int SoNhanhChuyen { get; set; }
    }

    public class QuyTrinhSoanThaoEditModel
    {
        public Guid Id { get; set; }
        public string MaQuyTrinh { get; set; } = string.Empty;
        public string TenQuyTrinh { get; set; } = string.Empty;
        public string LoaiQuyTrinh { get; set; } = "XayDung";
        public Guid? DanhMucVanBanId { get; set; }
        public List<Guid> DanhMucVanBanIds { get; set; } = new();
        public string? CapApDung { get; set; } = "Tinh";
        public List<string> CapApDungs { get; set; } = new() { "Tinh" };
        public int PhienBan { get; set; } = 1;
        public bool TrangThai { get; set; } = true;
        public string? MoTa { get; set; }
        public string? GhiChu { get; set; }
        public List<QuyTrinhSoanThaoBuocModel> BuocQuyTrinhs { get; set; } = new();
        public List<QuyTrinhSoanThaoChuyenBuocModel> ChuyenBuocs { get; set; } = new();
    }

    public class QuyTrinhSoanThaoBuocModel
    {
        public static readonly Guid SoTuPhapDonViId = Guid.Parse("40000000-0000-0000-0000-000000000002");
        public static readonly Guid VanPhongUbndTinhDonViId = Guid.Parse("40000000-0000-0000-0000-000000000013");

        public Guid Id { get; set; }
        public string MaBuoc { get; set; } = string.Empty;
        public string TenBuoc { get; set; } = string.Empty;
        public int ThuTuSapXep { get; set; } = 1;
        public string LoaiBuoc { get; set; } = "XuLy";
        public bool BatBuoc { get; set; } = true;
        public bool ChoPhepBoQua { get; set; }
        public bool ChoPhepQuayLui { get; set; }
        public string? CachHoanThanh { get; set; }
        public int? SoLuongPhanHoiToiThieu { get; set; }
        public bool YeuCauFileDinhKem { get; set; }
        public int SoLanTraLaiToiDa { get; set; }
        public int? SoNgayXuLyTieuChuan { get; set; }
        public int? SoNgayCanhBaoSapHan { get; set; }
        public Guid? DonViTiepNhanMacDinhId { get; set; }
        public string? MoTa { get; set; }
        public string? GhiChu { get; set; }
    }

    public class QuyTrinhSoanThaoChuyenBuocModel
    {
        public Guid Id { get; set; }
        public string TuBuocMa { get; set; } = string.Empty;
        public string DenBuocMa { get; set; } = string.Empty;
        public string DieuKienKetQua { get; set; } = "HOAN_THANH";
        public bool LaNhanhMacDinh { get; set; } = true;
        public string? MoTa { get; set; }
        public string? GhiChu { get; set; }
    }

    public static class QuyTrinhSoanThaoDefaultFactory
    {
        public static QuyTrinhSoanThaoEditModel CreateDefault()
        {
            return new QuyTrinhSoanThaoEditModel
            {
                LoaiQuyTrinh = "XayDung",
                CapApDung = "Tinh",
                CapApDungs = new List<string> { "Tinh" },
                PhienBan = 1,
                TrangThai = true,
                BuocQuyTrinhs = new List<QuyTrinhSoanThaoBuocModel>
                {
                    new() { MaBuoc = "LAP_DE_NGHI", TenBuoc = "Lap de nghi/Dang ky danh muc", ThuTuSapXep = 1, LoaiBuoc = "KhoiTao", CachHoanThanh = "Tao ho so va trinh de nghi", SoNgayXuLyTieuChuan = 3, SoNgayCanhBaoSapHan = 1 },
                    new() { MaBuoc = "THONG_NHAT", TenBuoc = "Tiep nhan/Xet duyet dang ky", ThuTuSapXep = 2, LoaiBuoc = "PheDuyet", CachHoanThanh = "Dong y hoac khong dong y cho xay dung", SoNgayXuLyTieuChuan = 2, SoNgayCanhBaoSapHan = 1, DonViTiepNhanMacDinhId = QuyTrinhSoanThaoBuocModel.VanPhongUbndTinhDonViId },
                    new() { MaBuoc = "SOAN_THAO", TenBuoc = "Soan thao van ban", ThuTuSapXep = 3, LoaiBuoc = "XuLy", CachHoanThanh = "Hoan thanh ban du thao", ChoPhepQuayLui = true, SoNgayXuLyTieuChuan = 5, SoNgayCanhBaoSapHan = 2 },
                    new() { MaBuoc = "LAY_Y_KIEN", TenBuoc = "Lay y kien", ThuTuSapXep = 4, LoaiBuoc = "LayYKien", CachHoanThanh = "Nhan phan hoi va dinh kem file", SoLuongPhanHoiToiThieu = 1, YeuCauFileDinhKem = true, SoNgayXuLyTieuChuan = 7, SoNgayCanhBaoSapHan = 2 },
                    new() { MaBuoc = "DANH_GIA", TenBuoc = "Tham dinh/Danh gia", ThuTuSapXep = 5, LoaiBuoc = "DanhGia", CachHoanThanh = "Dat hoac Khong dat", ChoPhepQuayLui = true, YeuCauFileDinhKem = true, SoLanTraLaiToiDa = 3, SoNgayXuLyTieuChuan = 5, SoNgayCanhBaoSapHan = 2 },
                    new() { MaBuoc = "TRINH_CO_QUAN", TenBuoc = "Trinh co quan co tham quyen", ThuTuSapXep = 6, LoaiBuoc = "PheDuyet", CachHoanThanh = "Trinh phe duyet van ban", SoNgayXuLyTieuChuan = 3, SoNgayCanhBaoSapHan = 1, DonViTiepNhanMacDinhId = QuyTrinhSoanThaoBuocModel.SoTuPhapDonViId },
                    new() { MaBuoc = "BAN_HANH", TenBuoc = "Ban hanh", ThuTuSapXep = 7, LoaiBuoc = "BanHanh", CachHoanThanh = "Van ban duoc ban hanh", YeuCauFileDinhKem = true, SoNgayXuLyTieuChuan = 2, SoNgayCanhBaoSapHan = 1, DonViTiepNhanMacDinhId = QuyTrinhSoanThaoBuocModel.VanPhongUbndTinhDonViId }
                },
                ChuyenBuocs = new List<QuyTrinhSoanThaoChuyenBuocModel>
                {
                    new() { TuBuocMa = "LAP_DE_NGHI", DenBuocMa = "THONG_NHAT", DieuKienKetQua = "HOAN_THANH_DANG_KY", LaNhanhMacDinh = true },
                    new() { TuBuocMa = "THONG_NHAT", DenBuocMa = "SOAN_THAO", DieuKienKetQua = "DONG_Y", LaNhanhMacDinh = true },
                    new() { TuBuocMa = "THONG_NHAT", DenBuocMa = "LAP_DE_NGHI", DieuKienKetQua = "KHONG_DONG_Y", LaNhanhMacDinh = false },
                    new() { TuBuocMa = "SOAN_THAO", DenBuocMa = "LAY_Y_KIEN", DieuKienKetQua = "HOAN_THANH_DU_THAO", LaNhanhMacDinh = true },
                    new() { TuBuocMa = "LAY_Y_KIEN", DenBuocMa = "DANH_GIA", DieuKienKetQua = "DA_GAN_KET_QUA_Y_KIEN", LaNhanhMacDinh = true },
                    new() { TuBuocMa = "DANH_GIA", DenBuocMa = "TRINH_CO_QUAN", DieuKienKetQua = "DAT", LaNhanhMacDinh = true },
                    new() { TuBuocMa = "DANH_GIA", DenBuocMa = "SOAN_THAO", DieuKienKetQua = "KHONG_DAT", LaNhanhMacDinh = false, MoTa = "Tra lai don vi soan thao toi da 3 lan" },
                    new() { TuBuocMa = "TRINH_CO_QUAN", DenBuocMa = "BAN_HANH", DieuKienKetQua = "TRINH_THANH_CONG", LaNhanhMacDinh = true }
                }
            };
        }
    }
}
