using DataAccess.Entities.QuanLyDanhMuc;

namespace Services.QuanLyDanhMuc
{
    public class QuyTrinhSoanThaoListItemModel
    {
        public Guid Id { get; set; }
        public string MaQuyTrinh { get; set; } = string.Empty;
        public string TenQuyTrinh { get; set; } = string.Empty;
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
        public Guid? DanhMucVanBanId { get; set; }
        public List<Guid> DanhMucVanBanIds { get; set; } = new();
        public string? CapApDung { get; set; } = "Tỉnh";
        public List<string> CapApDungs { get; set; } = new() { "Tỉnh" };
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
                CapApDung = "Tỉnh",
                CapApDungs = new List<string> { "Tỉnh" },
                PhienBan = 1,
                TrangThai = true,
                BuocQuyTrinhs = new List<QuyTrinhSoanThaoBuocModel>
                {
                    new() { MaBuoc = "LAP_DE_NGHI", TenBuoc = "Lập đề nghị/Đăng ký danh mục", ThuTuSapXep = 1, LoaiBuoc = "KhoiTao", CachHoanThanh = "Tạo hồ sơ và trình đề nghị", SoNgayXuLyTieuChuan = 3, SoNgayCanhBaoSapHan = 1 },
                    new() { MaBuoc = "THONG_NHAT", TenBuoc = "Tiếp nhận/Xét duyệt đăng ký", ThuTuSapXep = 2, LoaiBuoc = "PheDuyet", CachHoanThanh = "Đồng ý hoặc không đồng ý cho xây dựng", SoNgayXuLyTieuChuan = 2, SoNgayCanhBaoSapHan = 1, DonViTiepNhanMacDinhId = QuyTrinhSoanThaoBuocModel.VanPhongUbndTinhDonViId },
                    new() { MaBuoc = "SOAN_THAO", TenBuoc = "Soạn thảo văn bản", ThuTuSapXep = 3, LoaiBuoc = "XuLy", CachHoanThanh = "Hoàn thành bản dự thảo", ChoPhepQuayLui = true, SoNgayXuLyTieuChuan = 5, SoNgayCanhBaoSapHan = 2 },
                    new() { MaBuoc = "LAY_Y_KIEN", TenBuoc = "Lấy ý kiến", ThuTuSapXep = 4, LoaiBuoc = "LayYKien", CachHoanThanh = "Nhận phản hồi và đính kèm file", SoLuongPhanHoiToiThieu = 1, YeuCauFileDinhKem = true, SoNgayXuLyTieuChuan = 7, SoNgayCanhBaoSapHan = 2 },
                    new() { MaBuoc = "DANH_GIA", TenBuoc = "Thẩm định/Đánh giá", ThuTuSapXep = 5, LoaiBuoc = "DanhGia", CachHoanThanh = "Đạt hoặc Không đạt", ChoPhepQuayLui = true, YeuCauFileDinhKem = true, SoLanTraLaiToiDa = 3, SoNgayXuLyTieuChuan = 5, SoNgayCanhBaoSapHan = 2 },
                    new() { MaBuoc = "TRINH_CO_QUAN", TenBuoc = "Trình cơ quan có thẩm quyền", ThuTuSapXep = 6, LoaiBuoc = "PheDuyet", CachHoanThanh = "Trình phê duyệt văn bản", SoNgayXuLyTieuChuan = 3, SoNgayCanhBaoSapHan = 1, DonViTiepNhanMacDinhId = QuyTrinhSoanThaoBuocModel.SoTuPhapDonViId },
                    new() { MaBuoc = "BAN_HANH", TenBuoc = "Ban hành", ThuTuSapXep = 7, LoaiBuoc = "BanHanh", CachHoanThanh = "Văn bản được ban hành", YeuCauFileDinhKem = true, SoNgayXuLyTieuChuan = 2, SoNgayCanhBaoSapHan = 1, DonViTiepNhanMacDinhId = QuyTrinhSoanThaoBuocModel.VanPhongUbndTinhDonViId }
                },
                ChuyenBuocs = new List<QuyTrinhSoanThaoChuyenBuocModel>
                {
                    new() { TuBuocMa = "LAP_DE_NGHI", DenBuocMa = "THONG_NHAT", DieuKienKetQua = "HOAN_THANH_DANG_KY", LaNhanhMacDinh = true },
                    new() { TuBuocMa = "THONG_NHAT", DenBuocMa = "SOAN_THAO", DieuKienKetQua = "DONG_Y", LaNhanhMacDinh = true },
                    new() { TuBuocMa = "THONG_NHAT", DenBuocMa = "LAP_DE_NGHI", DieuKienKetQua = "KHONG_DONG_Y", LaNhanhMacDinh = false },
                    new() { TuBuocMa = "SOAN_THAO", DenBuocMa = "LAY_Y_KIEN", DieuKienKetQua = "HOAN_THANH_DU_THAO", LaNhanhMacDinh = true },
                    new() { TuBuocMa = "LAY_Y_KIEN", DenBuocMa = "DANH_GIA", DieuKienKetQua = "DA_GAN_KET_QUA_Y_KIEN", LaNhanhMacDinh = true },
                    new() { TuBuocMa = "DANH_GIA", DenBuocMa = "TRINH_CO_QUAN", DieuKienKetQua = "DAT", LaNhanhMacDinh = true },
                    new() { TuBuocMa = "DANH_GIA", DenBuocMa = "SOAN_THAO", DieuKienKetQua = "KHONG_DAT", LaNhanhMacDinh = false, MoTa = "Trả lại đơn vị soạn thảo tối đa 3 lần" },
                    new() { TuBuocMa = "TRINH_CO_QUAN", DenBuocMa = "BAN_HANH", DieuKienKetQua = "TRINH_THANH_CONG", LaNhanhMacDinh = true }
                }
            };
        }
    }
}
