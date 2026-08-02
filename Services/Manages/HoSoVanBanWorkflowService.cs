using DataAccess;
using DataAccess.Entities.Manages;
using DataAccess.Entities.QuanLyDanhMuc;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore;
using Services.Model;
using Services.Systems;
using System.Text;

namespace Services.Manages
{
    public interface IHoSoVanBanWorkflowService
    {
        Task<CommonResponse> GetDanhSachAsync(string search, Guid? donViSoanThaoId = null, int pageSize = 5, int pageCurrent = 1);
        Task<CommonResponse> GetDanhSachDangKyAsync(
            string search,
            Guid? donViSoanThaoId = null,
            int pageSize = 5,
            int pageCurrent = 1,
            Guid? danhMucVanBanId = null,
            Guid? nguoiXuLyId = null,
            string? maTrangThai = null,
            string? maBuoc = null,
            DateTime? tuNgayTao = null,
            DateTime? denNgayTao = null,
            DateTime? tuHanXuLy = null,
            DateTime? denHanXuLy = null,
            DateTime? tuNgayHoanThanh = null,
            DateTime? denNgayHoanThanh = null);
        Task<CommonResponse> GetDanhSachTheoBuocAsync(string search, string maBuoc, Guid? donViSoanThaoId = null, int pageSize = 5, int pageCurrent = 1, bool chiLayDonViDangNhap = true, IEnumerable<string>? trangThaiNghiepVuFilters = null, string? loaiQuyTrinh = null);
        Task<CommonResponse> GetDanhSachBanHanhAsync(string search, Guid? donViSoanThaoId = null, int pageSize = 5, int pageCurrent = 1);
        Task<CommonResponse> GetDanhSachChamDiemXayDungAsync(string search, Guid? donViSoanThaoId = null, int pageSize = 5, int pageCurrent = 1);
        Task<CommonResponse> GetDanhSachTheoDoiTienDoXayDungAsync(string search, Guid? donViSoanThaoId = null, string? maBuoc = null, string? mucCanhBao = null, int pageSize = 5, int pageCurrent = 1);
        Task<CommonResponse> GetDanhSachGiaHanXayDungAsync(string search, Guid? donViSoanThaoId = null, int pageSize = 5, int pageCurrent = 1);
        Task<CommonResponse> GetDanhSachLayYKienAsync(string search, Guid? donViId = null, int pageSize = 5, int pageCurrent = 1);
        Task<CommonResponse> GetDonDocTienDoFormAsync(Guid hoSoVanBanId);
        Task<CommonResponse> GuiDonDocTienDoAsync(HoSoVanBanDonDocFormModel request);
        Task<CommonResponse> GetDonDocTienDoHangLoatFormAsync(List<Guid> hoSoVanBanIds);
        Task<CommonResponse> GuiDonDocTienDoHangLoatAsync(HoSoVanBanDonDocHangLoatFormModel request);
        Task<CommonResponse> GetKetQuaLayYKienFormAsync(Guid hoSoVanBanId, string coQuan = "UBND");
        Task<CommonResponse> SaveKetQuaLayYKienAsync(HoSoVanBanKetQuaLayYKienFormModel request);
        Task<CommonResponse> GetBanHanhFormAsync(Guid hoSoVanBanId);
        Task<CommonResponse> SaveBanHanhAsync(HoSoVanBanBanHanhFormModel request, bool xacNhanBanHanh);
        Task<CommonResponse> GetGiaHanXayDungFormAsync(Guid hoSoVanBanId);
        Task<CommonResponse> SaveGiaHanXayDungAsync(HoSoVanBanGiaHanFormModel request);
        Task<CommonResponse> GetChamDiemXayDungFormAsync(Guid hoSoVanBanId);
        Task<CommonResponse> SaveChamDiemXayDungAsync(HoSoVanBanChamDiemFormModel request);
        Task<List<DonViOptionModel>> GetDonViOptionsAsync();
        Task<List<SelectOptionModel>> GetBuocTheoDoiTienDoOptionsAsync();
        Task<List<SelectOptionModel>> GetNguoiXuLyOptionsAsync(Guid? donViId = null);
        Task<List<HoSoDangKyOptionModel>> GetHoSoDangKyOptionsAsync(Guid? donViId = null, bool isSSA = false);
        Task<List<DataAccess.Entities.QuanLyDanhMuc.DanhMucVanBan>> GetDanhMucVanBanOptionsAsync();
        Task<List<DataAccess.Entities.QuanLyDanhMuc.DanhMucQuyTrinhSoanThao>> GetQuyTrinhOptionsAsync(Guid? danhMucVanBanId = null, string? loaiQuyTrinh = null);
        Task<List<DataAccess.Entities.QuanLyDanhMuc.DanhMucQuyTrinhSoanThao>> GetDraftQuyTrinhOptionsAsync(Guid danhMucVanBanId);
        Task<List<HoSoVanBanBuocThoiHanEditModel>> GetBuocThoiHanOptionsAsync(Guid quyTrinhSoanThaoId);
        Task<CommonResponse> CreateHoSoAsync(HoSoVanBanCreateModel request);
        Task<CommonResponse> GetHoSoEditModelAsync(Guid hoSoVanBanId);
        Task<CommonResponse> GetChuyenHoSoModelAsync(Guid hoSoVanBanId);
        Task<CommonResponse> GetChuyenXetDuyetDuThaoModelAsync(Guid hoSoVanBanId);
        Task<CommonResponse> GetChuyenDanhGiaModelAsync(Guid hoSoVanBanId);
        Task<CommonResponse> GetChuyenPheDuyetModelAsync(Guid hoSoVanBanId);
        Task<CommonResponse> GetChuyenBanHanhModelAsync(Guid hoSoVanBanId);
        Task<CommonResponse> GetTaoHoSoSoanThaoTuDangKyModelAsync(Guid hoSoDangKyId);
        Task<CommonResponse> UpdateHoSoAsync(HoSoVanBanCreateModel request);
        Task<CommonResponse> NhanHoSoAsync(Guid hoSoVanBanId, string actionType = "NHAN_HO_SO", string? noiDungXuLy = null, string? ghiChu = null, DateTime? ngayXuLy = null, DateTime? hanXuLy = null);
        Task<CommonResponse> TraLaiDanhGiaAsync(Guid hoSoVanBanId, string lyDoTraLai, string? ghiChu = null);
        Task<CommonResponse> TraLaiDangKyAsync(Guid hoSoVanBanId, string lyDoTraLai, string? ghiChu = null);
        Task<CommonResponse> HuyXetDuyetDangKyAsync(Guid hoSoVanBanId, string lyDoHuy, DateTime? ngayHuy = null, string? ghiChu = null);
        Task<CommonResponse> TaoHoSoSoanThaoTuDangKyAsync(HoSoVanBanTaoSoanThaoTuDangKyModel request);
        Task<CommonResponse> HoanThanhXuLyAsync(HoSoVanBanXuLyStepModel request);
        Task<CommonResponse> KhoiTaoLayYKienAsync(HoSoVanBanLayYKienStepModel request);
        Task<CommonResponse> HoanThanhLayYKienAsync(HoSoVanBanLayYKienStepModel request);
        Task<CommonResponse> GetLayYKienFormAsync(Guid hoSoVanBanId, string actionMode);
        Task<CommonResponse> HoanThanhDanhGiaAsync(HoSoVanBanDanhGiaStepModel request);
        Task<CommonResponse> PhanHoiDanhGiaAsync(HoSoVanBanPhanHoiDanhGiaModel request);
        Task<CommonResponse> GetChiTietAsync(Guid hoSoVanBanId);
        Task<CommonResponse> GetSoSanhDuThaoAsync(Guid hoSoVanBanId, Guid? sourceFileId = null, Guid? targetFileId = null);
    }

    public class HoSoVanBanWorkflowService(
        ApplicationDbContext dbContext,
        IAuthService authService,
        INotificationService notificationService) : IHoSoVanBanWorkflowService
    {
        private readonly ApplicationDbContext _dbContext = dbContext;
        private readonly IAuthService _authService = authService;
        private readonly INotificationService _notificationService = notificationService;
        private const string DraftSourceNotePrefix = "[NguonDangKy:";
        private static readonly Guid SoTuPhapDonViId = Guid.Parse("40000000-0000-0000-0000-000000000002");
        private static readonly string[] DraftFileExtensions = [".doc", ".docx"];

        public async Task<CommonResponse> GetDanhSachAsync(string search, Guid? donViSoanThaoId = null, int pageSize = 5, int pageCurrent = 1)
        {
            return await GetDanhSachInternalAsync(search, pageSize, pageCurrent, false, null, false, donViSoanThaoId);
        }

        public async Task<CommonResponse> GetDanhSachDangKyAsync(
            string search,
            Guid? donViSoanThaoId = null,
            int pageSize = 5,
            int pageCurrent = 1,
            Guid? danhMucVanBanId = null,
            Guid? nguoiXuLyId = null,
            string? maTrangThai = null,
            string? maBuoc = null,
            DateTime? tuNgayTao = null,
            DateTime? denNgayTao = null,
            DateTime? tuHanXuLy = null,
            DateTime? denHanXuLy = null,
            DateTime? tuNgayHoanThanh = null,
            DateTime? denNgayHoanThanh = null)
        {
            return await GetDanhSachInternalAsync(
                search,
                pageSize,
                pageCurrent,
                false,
                null,
                false,
                donViSoanThaoId,
                false,
                null,
                "DangKy",
                danhMucVanBanId,
                nguoiXuLyId,
                maTrangThai,
                maBuoc,
                tuNgayTao,
                denNgayTao,
                tuHanXuLy,
                denHanXuLy,
                tuNgayHoanThanh,
                denNgayHoanThanh);
        }

        public async Task<CommonResponse> GetDanhSachTheoBuocAsync(string search, string maBuoc, Guid? donViSoanThaoId = null, int pageSize = 5, int pageCurrent = 1, bool chiLayDonViDangNhap = true, IEnumerable<string>? trangThaiNghiepVuFilters = null, string? loaiQuyTrinh = null)
        {
            return await GetDanhSachInternalAsync(search, pageSize, pageCurrent, false, maBuoc, chiLayDonViDangNhap, donViSoanThaoId, true, trangThaiNghiepVuFilters, loaiQuyTrinh);
        }

        public async Task<CommonResponse> GetDanhSachBanHanhAsync(string search, Guid? donViSoanThaoId = null, int pageSize = 5, int pageCurrent = 1)
        {
            try
            {
                var currentUser = _authService.GetUserInfo();
                var donViDangNhapId = currentUser?.DanhMucDonViId ?? Guid.Empty;
                var isSSA = currentUser?.SSA ?? false;

                var query =
                    from hoSo in _dbContext.HoSoVanBans.AsNoTracking()
                    join vanBan in _dbContext.DanhMucVanBans.AsNoTracking() on hoSo.DanhMucVanBanId equals vanBan.Id
                    join quyTrinh in _dbContext.DanhMucQuyTrinhSoanThaos.AsNoTracking() on hoSo.QuyTrinhSoanThaoId equals quyTrinh.Id
                    join donVi in _dbContext.DanhMucDonVis.AsNoTracking() on hoSo.DonViSoanThaoId equals donVi.Id
                    join buoc in _dbContext.DanhMucBuocQuyTrinhs.AsNoTracking() on hoSo.BuocHienTaiId equals buoc.Id
                    join xuLyCurrent in _dbContext.HoSoVanBanXuLys.AsNoTracking().Where(x => x.IsCurrent) on hoSo.Id equals xuLyCurrent.HoSoVanBanId into xuLyCurrentJoin
                    from xuLyCurrent in xuLyCurrentJoin.DefaultIfEmpty()
                    join donViXuLy in _dbContext.DanhMucDonVis.AsNoTracking() on xuLyCurrent.DonViXuLyId equals donViXuLy.Id into donViXuLyJoin
                    from donViXuLy in donViXuLyJoin.DefaultIfEmpty()
                    join trangThai in _dbContext.DanhMucTrangThais.AsNoTracking() on hoSo.DanhMucTrangThaiId equals trangThai.Id into trangThaiJoin
                    from trangThai in trangThaiJoin.DefaultIfEmpty()
                    where quyTrinh.LoaiQuyTrinh == NormalizeWorkflowType("XayDung")
                          && (buoc.MaBuoc == "BUOC_06_THONG_QUA_BAN_HANH" || buoc.MaBuoc == "BUOC_07_THONG_QUA_BAN_HANH")
                          && (isSSA || donViDangNhapId == Guid.Empty || (xuLyCurrent != null && xuLyCurrent.DonViXuLyId == donViDangNhapId))
                          && (!donViSoanThaoId.HasValue || donViSoanThaoId.Value == Guid.Empty || hoSo.DonViSoanThaoId == donViSoanThaoId.Value)
                    select new HoSoVanBanListItemModel
                    {
                        Id = hoSo.Id,
                        MaHoSo = hoSo.MaHoSo,
                        TenHoSo = hoSo.TenHoSo,
                        MaBuocHienTai = buoc.MaBuoc,
                        TenLoaiVanBan = vanBan.TenLoaiVanBan,
                        ChuTheBanHanh = vanBan.ChuTheBanHanh,
                        TenQuyTrinh = quyTrinh.TenQuyTrinh,
                        TenBuocHienTai = buoc.TenBuoc,
                        MaTrangThai = trangThai != null ? trangThai.MaTrangThai : null,
                        TenTrangThai = trangThai != null ? trangThai.TenTrangThai : null,
                        MaMauTrangThai = trangThai != null ? trangThai.MaMauHex : null,
                        TenDonViSoanThao = donVi.TenDonVi,
                        DonViXuLyHienTaiId = xuLyCurrent != null ? xuLyCurrent.DonViXuLyId : null,
                        TenDonViXuLyHienTai = donViXuLy != null ? donViXuLy.TenDonVi : null,
                        NguoiXuLyHienTaiId = xuLyCurrent != null ? xuLyCurrent.NguoiXuLyId : null,
                        NgayNhanHienTai = xuLyCurrent != null ? xuLyCurrent.NgayNhan : null,
                        TrangThaiNghiepVuTiepNhan = xuLyCurrent != null ? xuLyCurrent.KetQuaXuLy : null,
                        TenTrangThaiNghiepVuTiepNhan = xuLyCurrent != null ? xuLyCurrent.KetQuaXuLy : null,
                        NoiDungXuLyHienTai = xuLyCurrent != null ? xuLyCurrent.NoiDungXuLy : null,
                        NgayTaoHoSo = hoSo.NgayTaoHoSo,
                        HanXuLy = hoSo.HanXuLy,
                        NgayHoanThanh = hoSo.NgayHoanThanh,
                        SoLanTraLaiHienTai = hoSo.SoLanTraLaiHienTai,
                        CanXuLyBuocHienTai = false,
                        CanNhanHoSo = false,
                        DaNhanHoSo = false,
                        DaCoDuThao = false
                    };

                if (!string.IsNullOrWhiteSpace(search))
                {
                    var keyword = search.Trim().ToLower();
                    query = query.Where(x =>
                        x.MaHoSo.ToLower().Contains(keyword) ||
                        x.TenHoSo.ToLower().Contains(keyword) ||
                        (x.TenLoaiVanBan != null && x.TenLoaiVanBan.ToLower().Contains(keyword)) ||
                        (x.TenDonViSoanThao != null && x.TenDonViSoanThao.ToLower().Contains(keyword)));
                }

                var totalRecord = await query.CountAsync();
                var data = await query
                    .OrderByDescending(x => x.NgayNhanHienTai ?? x.NgayTaoHoSo)
                    .Skip((pageCurrent - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var trackingMap = await BuildTrackingMapAsync(data.Select(x => x.Id));
                foreach (var item in data)
                {
                    item.CanXuLyBuocHienTai = isSSA ||
                                              donViDangNhapId == Guid.Empty ||
                                              (item.DonViXuLyHienTaiId.HasValue && item.DonViXuLyHienTaiId.Value == donViDangNhapId);
                    item.DaNhanHoSo = item.NguoiXuLyHienTaiId.HasValue;
                    item.CanNhanHoSo = item.CanXuLyBuocHienTai && !item.DaNhanHoSo;
                    if (trackingMap.TryGetValue(item.Id, out var tracking))
                    {
                        item.TongSoBuoc = tracking.Summary.TongSoBuoc;
                        item.SoBuocHoanThanh = tracking.Summary.SoBuocHoanThanh;
                        item.SoBuocDungHan = tracking.Summary.SoBuocDungHan;
                        item.SoBuocQuaHan = tracking.Summary.SoBuocQuaHan;
                        item.SoBuocChuaThucHien = tracking.Summary.SoBuocChuaThucHien;
                        item.TyLeHoanThanh = tracking.Summary.TyLeHoanThanh;
                    }
                    item.TenTrangThaiNghiepVuTiepNhan = ResolveTiepNhanNghiepVuLabel(item.TrangThaiNghiepVuTiepNhan);
                }

                return new CommonResponse("success", "Thành công", data, totalRecord);
            }
            catch
            {
                return new CommonResponse();
            }
        }

        public async Task<CommonResponse> GetDanhSachChamDiemXayDungAsync(string search, Guid? donViSoanThaoId = null, int pageSize = 5, int pageCurrent = 1)
        {
            try
            {
                var currentUser = _authService.GetUserInfo();
                var donViDangNhapId = currentUser?.DanhMucDonViId ?? Guid.Empty;
                var isSSA = currentUser?.SSA ?? false;

                var query =
                    from hoSo in _dbContext.HoSoVanBans.AsNoTracking()
                    join vanBan in _dbContext.DanhMucVanBans.AsNoTracking() on hoSo.DanhMucVanBanId equals vanBan.Id
                    join quyTrinh in _dbContext.DanhMucQuyTrinhSoanThaos.AsNoTracking() on hoSo.QuyTrinhSoanThaoId equals quyTrinh.Id
                    join donVi in _dbContext.DanhMucDonVis.AsNoTracking() on hoSo.DonViSoanThaoId equals donVi.Id
                    join chamDiem in _dbContext.HoSoVanBanChamDiems.AsNoTracking() on hoSo.Id equals chamDiem.HoSoVanBanId into chamDiemJoin
                    from chamDiem in chamDiemJoin.DefaultIfEmpty()
                    where quyTrinh.LoaiQuyTrinh == NormalizeWorkflowType("XayDung")
                          && hoSo.TrangThaiBanHanh == "DA_BAN_HANH"
                          && (isSSA || donViDangNhapId == Guid.Empty || hoSo.DonViSoanThaoId == donViDangNhapId)
                          && (!donViSoanThaoId.HasValue || donViSoanThaoId.Value == Guid.Empty || hoSo.DonViSoanThaoId == donViSoanThaoId.Value)
                    select new HoSoVanBanListItemModel
                    {
                        Id = hoSo.Id,
                        MaHoSo = hoSo.MaHoSo,
                        TenHoSo = hoSo.TenHoSo,
                        TenLoaiVanBan = vanBan.TenLoaiVanBan,
                        ChuTheBanHanh = vanBan.ChuTheBanHanh,
                        TenQuyTrinh = quyTrinh.TenQuyTrinh,
                        TenDonViSoanThao = donVi.TenDonVi,
                        NgayTaoHoSo = hoSo.NgayTaoHoSo,
                        HanXuLy = hoSo.HanXuLy,
                        NgayHoanThanh = hoSo.NgayBanHanh ?? hoSo.NgayHoanThanh,
                        SoLanTraLaiHienTai = hoSo.SoLanTraLaiHienTai,
                        DiemTienDoXayDung = hoSo.DiemTienDoXayDung,
                        DiemChatLuongVanBan = hoSo.DiemChatLuongVanBan,
                        TongDiemDanhGia = hoSo.TongDiemDanhGia,
                        XepLoaiDanhGia = hoSo.XepLoaiDanhGia,
                        DaCoBanGhiChamDiem = chamDiem != null
                    };

                if (!string.IsNullOrWhiteSpace(search))
                {
                    var keyword = search.Trim().ToLower();
                    query = query.Where(x =>
                        x.MaHoSo.ToLower().Contains(keyword) ||
                        x.TenHoSo.ToLower().Contains(keyword) ||
                        (x.TenLoaiVanBan != null && x.TenLoaiVanBan.ToLower().Contains(keyword)) ||
                        (x.TenDonViSoanThao != null && x.TenDonViSoanThao.ToLower().Contains(keyword)));
                }

                var totalRecord = await query.CountAsync();
                var data = await query
                    .OrderByDescending(x => x.NgayHoanThanh ?? x.NgayTaoHoSo)
                    .Skip((pageCurrent - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return new CommonResponse("success", "Thành công", data, totalRecord);
            }
            catch
            {
                return new CommonResponse();
            }
        }

        public async Task<CommonResponse> GetDanhSachTheoDoiTienDoXayDungAsync(string search, Guid? donViSoanThaoId = null, string? maBuoc = null, string? mucCanhBao = null, int pageSize = 5, int pageCurrent = 1)
        {
            try
            {
                var currentUser = _authService.GetUserInfo();
                var donViDangNhapId = currentUser?.DanhMucDonViId ?? Guid.Empty;
                var isSSA = currentUser?.SSA ?? false;
                var canViewAll = isSSA || donViDangNhapId == SoTuPhapDonViId;
                var normalizedMucCanhBao = string.IsNullOrWhiteSpace(mucCanhBao) ? null : mucCanhBao.Trim().ToUpperInvariant();
                var normalizedMaBuoc = string.IsNullOrWhiteSpace(maBuoc) ? null : maBuoc.Trim();

                var query =
                    from hoSo in _dbContext.HoSoVanBans.AsNoTracking()
                    join vanBan in _dbContext.DanhMucVanBans.AsNoTracking() on hoSo.DanhMucVanBanId equals vanBan.Id
                    join quyTrinh in _dbContext.DanhMucQuyTrinhSoanThaos.AsNoTracking() on hoSo.QuyTrinhSoanThaoId equals quyTrinh.Id
                    join donVi in _dbContext.DanhMucDonVis.AsNoTracking() on hoSo.DonViSoanThaoId equals donVi.Id
                    join buoc in _dbContext.DanhMucBuocQuyTrinhs.AsNoTracking() on hoSo.BuocHienTaiId equals buoc.Id into buocJoin
                    from buoc in buocJoin.DefaultIfEmpty()
                    join xuLyCurrent in _dbContext.HoSoVanBanXuLys.AsNoTracking().Where(x => x.IsCurrent) on hoSo.Id equals xuLyCurrent.HoSoVanBanId into xuLyCurrentJoin
                    from xuLyCurrent in xuLyCurrentJoin.DefaultIfEmpty()
                    join donViXuLy in _dbContext.DanhMucDonVis.AsNoTracking() on xuLyCurrent.DonViXuLyId equals donViXuLy.Id into donViXuLyJoin
                    from donViXuLy in donViXuLyJoin.DefaultIfEmpty()
                    join trangThai in _dbContext.DanhMucTrangThais.AsNoTracking() on hoSo.DanhMucTrangThaiId equals trangThai.Id into trangThaiJoin
                    from trangThai in trangThaiJoin.DefaultIfEmpty()
                    where quyTrinh.LoaiQuyTrinh == NormalizeWorkflowType("XayDung")
                          && hoSo.TrangThaiBanHanh != "DA_BAN_HANH"
                          && !hoSo.NgayBanHanh.HasValue
                          && !hoSo.NgayHoanThanh.HasValue
                          && (string.IsNullOrWhiteSpace(normalizedMaBuoc) || (buoc != null && buoc.MaBuoc == normalizedMaBuoc))
                          && (!donViSoanThaoId.HasValue || donViSoanThaoId.Value == Guid.Empty || hoSo.DonViSoanThaoId == donViSoanThaoId.Value)
                          && (canViewAll || donViDangNhapId == Guid.Empty || hoSo.DonViSoanThaoId == donViDangNhapId || (xuLyCurrent != null && xuLyCurrent.DonViXuLyId == donViDangNhapId))
                    select new HoSoVanBanListItemModel
                    {
                        Id = hoSo.Id,
                        MaHoSo = hoSo.MaHoSo,
                        TenHoSo = hoSo.TenHoSo,
                        MaBuocHienTai = buoc != null ? buoc.MaBuoc : null,
                        TenLoaiVanBan = vanBan.TenLoaiVanBan,
                        ChuTheBanHanh = vanBan.ChuTheBanHanh,
                        TenQuyTrinh = quyTrinh.TenQuyTrinh,
                        TenBuocHienTai = buoc != null ? buoc.TenBuoc : null,
                        MaTrangThai = trangThai != null ? trangThai.MaTrangThai : null,
                        TenTrangThai = trangThai != null ? trangThai.TenTrangThai : null,
                        MaMauTrangThai = trangThai != null ? trangThai.MaMauHex : null,
                        TenDonViSoanThao = donVi.TenDonVi,
                        DonViXuLyHienTaiId = xuLyCurrent != null ? xuLyCurrent.DonViXuLyId : null,
                        TenDonViXuLyHienTai = donViXuLy != null ? donViXuLy.TenDonVi : null,
                        NguoiXuLyHienTaiId = xuLyCurrent != null ? xuLyCurrent.NguoiXuLyId : null,
                        NgayNhanHienTai = xuLyCurrent != null ? xuLyCurrent.NgayNhan : null,
                        TrangThaiNghiepVuTiepNhan = xuLyCurrent != null ? xuLyCurrent.KetQuaXuLy : null,
                        TenTrangThaiNghiepVuTiepNhan = xuLyCurrent != null ? xuLyCurrent.KetQuaXuLy : null,
                        NoiDungXuLyHienTai = xuLyCurrent != null ? xuLyCurrent.NoiDungXuLy : null,
                        NgayTaoHoSo = hoSo.NgayTaoHoSo,
                        HanXuLy = xuLyCurrent != null ? xuLyCurrent.HanXuLy : hoSo.HanXuLy,
                        SoLanTraLaiHienTai = hoSo.SoLanTraLaiHienTai
                    };

                if (!string.IsNullOrWhiteSpace(search))
                {
                    var keyword = search.Trim().ToLowerInvariant();
                    query = query.Where(x =>
                        x.MaHoSo.ToLower().Contains(keyword) ||
                        x.TenHoSo.ToLower().Contains(keyword) ||
                        (x.TenLoaiVanBan != null && x.TenLoaiVanBan.ToLower().Contains(keyword)) ||
                        (x.TenDonViSoanThao != null && x.TenDonViSoanThao.ToLower().Contains(keyword)) ||
                        (x.TenBuocHienTai != null && x.TenBuocHienTai.ToLower().Contains(keyword)));
                }

                var rawData = await query
                    .OrderByDescending(x => x.NgayNhanHienTai ?? x.NgayTaoHoSo)
                    .ThenBy(x => x.MaHoSo)
                    .ToListAsync();

                if (rawData.Count > 0)
                {
                    var trackingMap = await BuildTrackingMapAsync(rawData.Select(x => x.Id));
                    foreach (var item in rawData)
                    {
                        item.CanXuLyBuocHienTai = canViewAll ||
                                                  donViDangNhapId == Guid.Empty ||
                                                  (item.DonViXuLyHienTaiId.HasValue && item.DonViXuLyHienTaiId.Value == donViDangNhapId);
                        item.DaNhanHoSo = item.NguoiXuLyHienTaiId.HasValue;
                        item.CanNhanHoSo = item.CanXuLyBuocHienTai && !item.DaNhanHoSo;
                        item.TenTrangThaiNghiepVuTiepNhan = ResolveTiepNhanNghiepVuLabel(item.TrangThaiNghiepVuTiepNhan);

                        if (!trackingMap.TryGetValue(item.Id, out var tracking))
                        {
                            continue;
                        }

                        item.TongSoBuoc = tracking.Summary.TongSoBuoc;
                        item.SoBuocHoanThanh = tracking.Summary.SoBuocHoanThanh;
                        item.SoBuocDungHan = tracking.Summary.SoBuocDungHan;
                        item.SoBuocQuaHan = tracking.Summary.SoBuocQuaHan;
                        item.SoBuocChuaThucHien = tracking.Summary.SoBuocChuaThucHien;
                        item.TyLeHoanThanh = tracking.Summary.TyLeHoanThanh;

                        var currentStep = tracking.Steps.FirstOrDefault(x => x.IsCurrent)
                                          ?? tracking.Steps.FirstOrDefault(x => x.MaBuoc == item.MaBuocHienTai)
                                          ?? tracking.Steps.OrderByDescending(x => x.ThuTuSapXep).FirstOrDefault();
                        if (currentStep == null)
                        {
                            continue;
                        }

                        item.TrangThaiTienDo = currentStep.MaTrangThaiTheoDoi;
                        item.TenTrangThaiTienDo = currentStep.TenTrangThaiTheoDoi;
                        item.MaMauTienDo = currentStep.MaMauTrangThaiTheoDoi;
                        item.DangOQuaHan = currentStep.MaTrangThaiTheoDoi is "QUA_HAN" or "HOAN_THANH_QUA_HAN";

                        ResolveAlertInfo(item, currentStep);
                    }
                }

                if (!string.IsNullOrWhiteSpace(normalizedMucCanhBao))
                {
                    rawData = rawData
                        .Where(x => string.Equals(x.MucCanhBao, normalizedMucCanhBao, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                var totalRecord = rawData.Count;
                var data = rawData
                    .Skip((pageCurrent - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return new CommonResponse("success", "Thành công", data, totalRecord);
            }
            catch
            {
                return new CommonResponse();
            }
        }

        public async Task<CommonResponse> GetDanhSachGiaHanXayDungAsync(string search, Guid? donViSoanThaoId = null, int pageSize = 5, int pageCurrent = 1)
        {
            try
            {
                var currentUser = _authService.GetUserInfo();
                var donViDangNhapId = currentUser?.DanhMucDonViId ?? Guid.Empty;
                var isSSA = currentUser?.SSA ?? false;
                var canViewAll = isSSA || donViDangNhapId == SoTuPhapDonViId;

                var query =
                    from hoSo in _dbContext.HoSoVanBans.AsNoTracking()
                    join vanBan in _dbContext.DanhMucVanBans.AsNoTracking() on hoSo.DanhMucVanBanId equals vanBan.Id
                    join quyTrinh in _dbContext.DanhMucQuyTrinhSoanThaos.AsNoTracking() on hoSo.QuyTrinhSoanThaoId equals quyTrinh.Id
                    join donVi in _dbContext.DanhMucDonVis.AsNoTracking() on hoSo.DonViSoanThaoId equals donVi.Id
                    join buoc in _dbContext.DanhMucBuocQuyTrinhs.AsNoTracking() on hoSo.BuocHienTaiId equals buoc.Id into buocJoin
                    from buoc in buocJoin.DefaultIfEmpty()
                    join xuLyCurrent in _dbContext.HoSoVanBanXuLys.AsNoTracking().Where(x => x.IsCurrent) on hoSo.Id equals xuLyCurrent.HoSoVanBanId into xuLyCurrentJoin
                    from xuLyCurrent in xuLyCurrentJoin.DefaultIfEmpty()
                    join donViXuLy in _dbContext.DanhMucDonVis.AsNoTracking() on xuLyCurrent.DonViXuLyId equals donViXuLy.Id into donViXuLyJoin
                    from donViXuLy in donViXuLyJoin.DefaultIfEmpty()
                    where quyTrinh.LoaiQuyTrinh == NormalizeWorkflowType("XayDung")
                          && hoSo.TrangThaiBanHanh != "DA_BAN_HANH"
                          && !hoSo.NgayBanHanh.HasValue
                          && !hoSo.NgayHoanThanh.HasValue
                          && (!donViSoanThaoId.HasValue || donViSoanThaoId.Value == Guid.Empty || hoSo.DonViSoanThaoId == donViSoanThaoId.Value)
                          && (canViewAll || donViDangNhapId == Guid.Empty || hoSo.DonViSoanThaoId == donViDangNhapId || (xuLyCurrent != null && xuLyCurrent.DonViXuLyId == donViDangNhapId))
                    select new HoSoVanBanListItemModel
                    {
                        Id = hoSo.Id,
                        MaHoSo = hoSo.MaHoSo,
                        TenHoSo = hoSo.TenHoSo,
                        MaBuocHienTai = buoc != null ? buoc.MaBuoc : null,
                        TenLoaiVanBan = vanBan.TenLoaiVanBan,
                        TenBuocHienTai = buoc != null ? buoc.TenBuoc : null,
                        TenDonViSoanThao = donVi.TenDonVi,
                        TenDonViXuLyHienTai = donViXuLy != null ? donViXuLy.TenDonVi : null,
                        DonViXuLyHienTaiId = xuLyCurrent != null ? xuLyCurrent.DonViXuLyId : null,
                        NgayTaoHoSo = hoSo.NgayTaoHoSo,
                        HanXuLy = xuLyCurrent != null ? xuLyCurrent.HanXuLy : hoSo.HanXuLy,
                        SoLanTraLaiHienTai = hoSo.SoLanTraLaiHienTai,
                        SoLanGiaHan = _dbContext.HoSoVanBanGiaHans.Count(x => x.HoSoVanBanId == hoSo.Id)
                    };

                if (!string.IsNullOrWhiteSpace(search))
                {
                    var keyword = search.Trim().ToLowerInvariant();
                    query = query.Where(x =>
                        x.MaHoSo.ToLower().Contains(keyword) ||
                        x.TenHoSo.ToLower().Contains(keyword) ||
                        (x.TenLoaiVanBan != null && x.TenLoaiVanBan.ToLower().Contains(keyword)) ||
                        (x.TenDonViSoanThao != null && x.TenDonViSoanThao.ToLower().Contains(keyword)) ||
                        (x.TenBuocHienTai != null && x.TenBuocHienTai.ToLower().Contains(keyword)));
                }

                var totalRecord = await query.CountAsync();
                var data = await query
                    .OrderBy(x => x.HanXuLy ?? DateTime.MaxValue)
                    .ThenByDescending(x => x.SoLanGiaHan)
                    .ThenBy(x => x.MaHoSo)
                    .Skip((pageCurrent - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return new CommonResponse("success", "Thành công", data, totalRecord);
            }
            catch
            {
                return new CommonResponse();
            }
        }

        public async Task<CommonResponse> GetDanhSachLayYKienAsync(string search, Guid? donViId = null, int pageSize = 5, int pageCurrent = 1)
        {
            try
            {
                var currentUser = _authService.GetUserInfo();
                var donViDangNhapId = currentUser?.DanhMucDonViId ?? Guid.Empty;
                var isSSA = currentUser?.SSA ?? false;
                var effectiveDonViId = isSSA
                    ? donViId
                    : (donViDangNhapId != Guid.Empty ? donViDangNhapId : null);

                var query =
                    from hoSo in _dbContext.HoSoVanBans.AsNoTracking()
                    join vanBan in _dbContext.DanhMucVanBans.AsNoTracking() on hoSo.DanhMucVanBanId equals vanBan.Id
                    join quyTrinh in _dbContext.DanhMucQuyTrinhSoanThaos.AsNoTracking() on hoSo.QuyTrinhSoanThaoId equals quyTrinh.Id
                    join donVi in _dbContext.DanhMucDonVis.AsNoTracking() on hoSo.DonViSoanThaoId equals donVi.Id
                    join buoc in _dbContext.DanhMucBuocQuyTrinhs.AsNoTracking() on hoSo.BuocHienTaiId equals buoc.Id
                    join xuLyCurrent in _dbContext.HoSoVanBanXuLys.AsNoTracking().Where(x => x.IsCurrent) on hoSo.Id equals xuLyCurrent.HoSoVanBanId into xuLyCurrentJoin
                    from xuLyCurrent in xuLyCurrentJoin.DefaultIfEmpty()
                    join donViXuLy in _dbContext.DanhMucDonVis.AsNoTracking() on xuLyCurrent.DonViXuLyId equals donViXuLy.Id into donViXuLyJoin
                    from donViXuLy in donViXuLyJoin.DefaultIfEmpty()
                    join trangThai in _dbContext.DanhMucTrangThais.AsNoTracking() on hoSo.DanhMucTrangThaiId equals trangThai.Id into trangThaiJoin
                    from trangThai in trangThaiJoin.DefaultIfEmpty()
                    where (((buoc.MaBuoc == "BUOC_04_LAY_Y_KIEN"
                             && (xuLyCurrent == null || xuLyCurrent.KetQuaXuLy != "DA_TONG_HOP_Y_KIEN"))
                            || (xuLyCurrent != null && xuLyCurrent.KetQuaXuLy == "DANG_LAY_GOP_Y")))
                          && (!effectiveDonViId.HasValue || effectiveDonViId == Guid.Empty
                              || hoSo.DonViSoanThaoId == effectiveDonViId.Value
                              || _dbContext.HoSoVanBanLayYKiens.AsNoTracking().Any(x =>
                                  x.HoSoVanBanId == hoSo.Id &&
                                  x.DonViDuocLayYKienId == effectiveDonViId.Value))
                    select new HoSoVanBanListItemModel
                    {
                        Id = hoSo.Id,
                        MaHoSo = hoSo.MaHoSo,
                        TenHoSo = hoSo.TenHoSo,
                        MaBuocHienTai = buoc.MaBuoc,
                        TenLoaiVanBan = vanBan.TenLoaiVanBan,
                        ChuTheBanHanh = vanBan.ChuTheBanHanh,
                        TenQuyTrinh = quyTrinh.TenQuyTrinh,
                        TenBuocHienTai = buoc.TenBuoc,
                        MaTrangThai = trangThai != null ? trangThai.MaTrangThai : null,
                        TenTrangThai = trangThai != null ? trangThai.TenTrangThai : null,
                        MaMauTrangThai = trangThai != null ? trangThai.MaMauHex : null,
                        TenDonViSoanThao = donVi.TenDonVi,
                        DonViXuLyHienTaiId = xuLyCurrent != null ? xuLyCurrent.DonViXuLyId : null,
                        TenDonViXuLyHienTai = donViXuLy != null ? donViXuLy.TenDonVi : null,
                        NguoiXuLyHienTaiId = xuLyCurrent != null ? xuLyCurrent.NguoiXuLyId : null,
                        NgayNhanHienTai = xuLyCurrent != null ? xuLyCurrent.NgayNhan : null,
                        TrangThaiNghiepVuTiepNhan = xuLyCurrent != null ? xuLyCurrent.KetQuaXuLy : null,
                        TenTrangThaiNghiepVuTiepNhan = xuLyCurrent != null ? xuLyCurrent.KetQuaXuLy : null,
                        NoiDungXuLyHienTai = xuLyCurrent != null ? xuLyCurrent.NoiDungXuLy : null,
                        NgayTaoHoSo = hoSo.NgayTaoHoSo,
                        HanXuLy = hoSo.HanXuLy,
                        NgayHoanThanh = hoSo.NgayHoanThanh,
                        SoLanTraLaiHienTai = hoSo.SoLanTraLaiHienTai
                    };

                if (!string.IsNullOrWhiteSpace(search))
                {
                    query = query.Where(x =>
                        x.MaHoSo.Contains(search) ||
                        x.TenHoSo.Contains(search) ||
                        (x.TenLoaiVanBan != null && x.TenLoaiVanBan.Contains(search)) ||
                        (x.TenQuyTrinh != null && x.TenQuyTrinh.Contains(search)) ||
                        (x.TenDonViSoanThao != null && x.TenDonViSoanThao.Contains(search)));
                }

                query = query.OrderByDescending(x => x.NgayTaoHoSo).ThenBy(x => x.MaHoSo);
                var totalRecord = await query.CountAsync();
                var data = await query.Skip((pageCurrent - 1) * pageSize).Take(pageSize).ToListAsync();

                if (data.Count > 0)
                {
                    var hoSoIds = data.Select(x => x.Id).ToList();
                    var trackingMap = await BuildTrackingMapAsync(hoSoIds);
                    var layYKienRows = await (
                        from row in _dbContext.HoSoVanBanLayYKiens.AsNoTracking()
                        join donVi in _dbContext.DanhMucDonVis.AsNoTracking() on row.DonViDuocLayYKienId equals donVi.Id into donViJoin
                        from donVi in donViJoin.DefaultIfEmpty()
                        where hoSoIds.Contains(row.HoSoVanBanId)
                        select new HoSoVanBanLayYKienItemModel
                        {
                            Id = row.Id,
                            HoSoVanBanId = row.HoSoVanBanId,
                            DonViDuocLayYKienId = row.DonViDuocLayYKienId,
                            TenDonViDuocLayYKien = donVi != null ? donVi.TenDonVi : null,
                            NguoiDuocLayYKienId = row.NguoiDuocLayYKienId,
                            NoiDungYeuCau = row.NoiDungYeuCau,
                            NoiDungPhanHoi = row.NoiDungPhanHoi,
                            NgayGui = row.NgayGui,
                            HanPhanHoi = row.HanPhanHoi,
                            NgayPhanHoi = row.NgayPhanHoi,
                            TrangThaiPhanHoi = row.TrangThaiPhanHoi,
                            AttachedFileGroupId = row.AttachedFileGroupId,
                            GhiChu = row.GhiChu
                        }).ToListAsync();

                    var rowMap = layYKienRows.GroupBy(x => x.HoSoVanBanId).ToDictionary(x => x.Key, x => x.ToList());

                    foreach (var item in data)
                    {
                        item.CanXuLyBuocHienTai = isSSA ||
                                                  donViDangNhapId == Guid.Empty ||
                                                  (item.DonViXuLyHienTaiId.HasValue && item.DonViXuLyHienTaiId.Value == donViDangNhapId);
                        item.DaNhanHoSo = item.NguoiXuLyHienTaiId.HasValue;
                        item.CanNhanHoSo = false;
                        item.TenTrangThaiNghiepVuTiepNhan = ResolveTiepNhanNghiepVuLabel(item.TrangThaiNghiepVuTiepNhan);

                        rowMap.TryGetValue(item.Id, out var itemRows);
                        itemRows ??= new List<HoSoVanBanLayYKienItemModel>();
                        item.CheDoLayYKienHienTai = itemRows.Any(x => x.DonViDuocLayYKienId.HasValue)
                            ? "GUI_DON_VI_GOP_Y"
                            : "CAP_NHAT_KET_QUA";

                        item.CoTheTongHopLayYKien = item.CanXuLyBuocHienTai;

                        item.CoThePhanHoiLayYKien = itemRows.Any(x =>
                            x.DonViDuocLayYKienId.HasValue &&
                            x.DonViDuocLayYKienId.Value == donViDangNhapId &&
                            !string.Equals(x.TrangThaiPhanHoi, "DA_CO_Y_KIEN", StringComparison.OrdinalIgnoreCase));

                        if (trackingMap.TryGetValue(item.Id, out var tracking))
                        {
                            item.TongSoBuoc = tracking.Summary.TongSoBuoc;
                            item.SoBuocHoanThanh = tracking.Summary.SoBuocHoanThanh;
                            item.SoBuocDungHan = tracking.Summary.SoBuocDungHan;
                            item.SoBuocQuaHan = tracking.Summary.SoBuocQuaHan;
                            item.SoBuocChuaThucHien = tracking.Summary.SoBuocChuaThucHien;
                            item.TyLeHoanThanh = tracking.Summary.TyLeHoanThanh;

                            var currentStep = tracking.Steps.FirstOrDefault(x => x.IsCurrent);
                            var latestStep = currentStep ?? tracking.Steps.OrderByDescending(x => x.ThuTuSapXep).FirstOrDefault();
                            if (latestStep != null)
                            {
                                item.TrangThaiTienDo = latestStep.MaTrangThaiTheoDoi;
                                item.TenTrangThaiTienDo = latestStep.TenTrangThaiTheoDoi;
                                item.MaMauTienDo = latestStep.MaMauTrangThaiTheoDoi;
                                item.DangOQuaHan = latestStep.MaTrangThaiTheoDoi is "QUA_HAN" or "HOAN_THANH_QUA_HAN";
                            }
                        }
                    }
                }

                return new CommonResponse("success", "ThĂ nh cĂ´ng", data, totalRecord);
            }
            catch
            {
                return new CommonResponse();
            }
        }

        public async Task<CommonResponse> GetKetQuaLayYKienFormAsync(Guid hoSoVanBanId, string coQuan = "UBND")
        {
            var normalizedCoQuan = (coQuan ?? "UBND").Trim().ToUpperInvariant();
            var data = await (
                from hoSo in _dbContext.HoSoVanBans.AsNoTracking()
                join vanBan in _dbContext.DanhMucVanBans.AsNoTracking() on hoSo.DanhMucVanBanId equals vanBan.Id
                join donVi in _dbContext.DanhMucDonVis.AsNoTracking() on hoSo.DonViSoanThaoId equals donVi.Id
                join buoc in _dbContext.DanhMucBuocQuyTrinhs.AsNoTracking() on hoSo.BuocHienTaiId equals buoc.Id
                where hoSo.Id == hoSoVanBanId
                select new HoSoVanBanKetQuaLayYKienFormModel
                {
                    HoSoVanBanId = hoSo.Id,
                    TenHoSo = hoSo.TenHoSo,
                    TenLoaiVanBan = vanBan.TenLoaiVanBan,
                    TenDonViSoanThao = donVi.TenDonVi,
                    TenBuocHienTai = buoc.TenBuoc,
                    CoQuanLayYKien = normalizedCoQuan,
                    AttachedFileGroupId = Guid.NewGuid(),
                    NgayGuiLayYKien = DateTime.Today,
                    HanPhanHoi = hoSo.HanXuLy
                }).FirstOrDefaultAsync();

            if (data == null)
            {
                return new CommonResponse("error", "KhĂ´ng tĂ¬m tháº¥y há»“ sÆ¡ láº¥y Ă½ kiáº¿n.");
            }

            var dot = await _dbContext.HoSoVanBanDotLayYKiens.AsNoTracking()
                .Where(x => x.HoSoVanBanId == hoSoVanBanId && x.CoQuanLayYKien == normalizedCoQuan)
                .OrderByDescending(x => x.LanLayYKien)
                .FirstOrDefaultAsync();

            if (dot == null)
            {
                data.LanLayYKien = await _dbContext.HoSoVanBanDotLayYKiens
                    .CountAsync(x => x.HoSoVanBanId == hoSoVanBanId && x.CoQuanLayYKien == normalizedCoQuan) + 1;
                return new CommonResponse("success", "ThĂ nh cĂ´ng", data);
            }

            data.Id = dot.Id;
            data.LanLayYKien = dot.LanLayYKien;
            data.CheDoNhapYKien = dot.CheDoNhapYKien;
            data.HinhThucLayYKien = dot.HinhThucLayYKien;
            data.SoVanBanLayYKien = dot.SoVanBanLayYKien;
            data.NgayGuiLayYKien = dot.NgayGuiLayYKien;
            data.HanPhanHoi = dot.HanPhanHoi;
            data.NgayCoKetQua = dot.NgayCoKetQua;
            data.NoiDungYeuCau = dot.NoiDungYeuCau;
            data.TongSoThanhVien = dot.TongSoThanhVien;
            data.SoDongY = dot.SoDongY;
            data.SoDongYCoYKien = dot.SoDongYCoYKien;
            data.SoKhongDongY = dot.SoKhongDongY;
            data.SoKhongPhanHoi = dot.SoKhongPhanHoi;
            data.TyLeDongY = dot.TyLeDongY;
            data.KetQuaChung = dot.KetQuaChung;
            data.NoiDungTongHop = dot.NoiDungTongHop;
            data.NoiDungTiepThu = dot.NoiDungTiepThu;
            data.TrangThai = dot.TrangThai;
            data.AttachedFileGroupId = dot.AttachedFileGroupId ?? data.AttachedFileGroupId;
            data.GhiChu = dot.GhiChu;
            data.ThanhViens = await _dbContext.HoSoVanBanYKienThanhViens.AsNoTracking()
                .Where(x => x.DotLayYKienId == dot.Id)
                .OrderBy(x => x.ThuTuHienThi)
                .Select(x => new HoSoVanBanYKienThanhVienModel
                {
                    Id = x.Id, ThanhVienId = x.ThanhVienId, HoTenThanhVien = x.HoTenThanhVien,
                    ChucVu = x.ChucVu, DonViId = x.DonViId, TenDonVi = x.TenDonVi,
                    ThuTuHienThi = x.ThuTuHienThi, CoQuyenBieuQuyet = x.CoQuyenBieuQuyet,
                    KetQuaYKien = x.KetQuaYKien, NoiDungYKien = x.NoiDungYKien,
                    NoiDungTiepThu = x.NoiDungTiepThu, NgayPhanHoi = x.NgayPhanHoi,
                    AttachedFileGroupId = x.AttachedFileGroupId, GhiChu = x.GhiChu
                }).ToListAsync();

            return new CommonResponse("success", "ThĂ nh cĂ´ng", data);
        }

        public async Task<CommonResponse> SaveKetQuaLayYKienAsync(HoSoVanBanKetQuaLayYKienFormModel request)
        {
            var currentUser = _authService.GetUserInfo();
            if (currentUser == null) return new CommonResponse("error", "KhĂ´ng xĂ¡c Ä‘á»‹nh Ä‘Æ°á»£c tĂ i khoáº£n Ä‘ang thao tĂ¡c.");

            var normalizedCoQuan = string.IsNullOrWhiteSpace(request.CoQuanLayYKien)
                ? "UBND"
                : request.CoQuanLayYKien.Trim().ToUpperInvariant();
            var expectedStepCode = normalizedCoQuan == "HDND"
                ? "BUOC_06_TRINH_HDND_HOP"
                : "BUOC_05_LAY_Y_KIEN_THANH_VIEN_UBND";
            var successTransition = normalizedCoQuan == "HDND"
                ? "TRINH_HDND_XONG"
                : "LAY_Y_KIEN_UBND_XONG";
            var coQuanLabel = normalizedCoQuan == "HDND" ? "HÄND" : "UBND";

            var hoSo = await GetHoSoWithCurrentStepAsync(request.HoSoVanBanId);
            var currentStep = hoSo == null ? null : await GetCurrentStepAsync(hoSo);
            if (hoSo == null || currentStep == null || currentStep.MaBuoc != expectedStepCode)
            {
                return new CommonResponse("error", $"Há»“ sÆ¡ hiá»‡n khĂ´ng á»Ÿ bÆ°á»›c láº¥y Ă½ kiáº¿n {coQuanLabel}.");
            }

            var currentProcessing = await _dbContext.HoSoVanBanXuLys
                .Where(x => x.HoSoVanBanId == hoSo.Id && x.IsCurrent)
                .OrderByDescending(x => x.NgayNhan).FirstOrDefaultAsync();
            if (!CanCurrentUserXuLy(currentUser, currentProcessing))
                return new CommonResponse("error", "Báº¡n khĂ´ng cĂ³ quyá»n cáº­p nháº­t há»“ sÆ¡ nĂ y.");

            var mode = (request.CheDoNhapYKien ?? "TONG_HOP").Trim().ToUpperInvariant();
            if (mode != "TONG_HOP" && mode != "CHI_TIET")
                return new CommonResponse("error", "Cháº¿ Ä‘á»™ nháº­p Ă½ kiáº¿n khĂ´ng há»£p lá»‡.");

            var members = request.ThanhViens.Where(x => !string.IsNullOrWhiteSpace(x.HoTenThanhVien)).ToList();
            if (mode == "CHI_TIET")
            {
                if (members.Count == 0) return new CommonResponse("error", $"Pháº£i nháº­p Ă­t nháº¥t má»™t thĂ nh viĂªn {coQuanLabel}.");
                var voting = members.Where(x => x.CoQuyenBieuQuyet).ToList();
                request.TongSoThanhVien = voting.Count;
                request.SoDongY = voting.Count(x => x.KetQuaYKien == "DONG_Y");
                request.SoDongYCoYKien = voting.Count(x => x.KetQuaYKien == "DONG_Y_CO_Y_KIEN");
                request.SoKhongDongY = voting.Count(x => x.KetQuaYKien == "KHONG_DONG_Y");
                request.SoKhongPhanHoi = voting.Count(x => string.IsNullOrWhiteSpace(x.KetQuaYKien) || x.KetQuaYKien == "CHUA_PHAN_HOI" || x.KetQuaYKien == "KHONG_THAM_GIA");
            }

            var total = request.TongSoThanhVien ?? 0;
            var sum = (request.SoDongY ?? 0) + (request.SoDongYCoYKien ?? 0) + (request.SoKhongDongY ?? 0) + (request.SoKhongPhanHoi ?? 0);
            if (total < 0 || sum != total) return new CommonResponse("error", "Tá»•ng sá»‘ káº¿t quáº£ pháº£i báº±ng tá»•ng sá»‘ thĂ nh viĂªn.");
            if (request.TrangThai == "DA_XAC_NHAN" && (total == 0 || string.IsNullOrWhiteSpace(request.KetQuaChung) || !request.NgayCoKetQua.HasValue))
                return new CommonResponse("error", "Khi xĂ¡c nháº­n pháº£i cĂ³ thĂ nh viĂªn, ngĂ y cĂ³ káº¿t quáº£ vĂ  káº¿t quáº£ chung.");
            request.TyLeDongY = total == 0 ? 0 : Math.Round(((request.SoDongY ?? 0) + (request.SoDongYCoYKien ?? 0)) * 100m / total, 2);

            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var dot = request.Id == Guid.Empty ? null : await _dbContext.HoSoVanBanDotLayYKiens.FirstOrDefaultAsync(x => x.Id == request.Id);
                dot ??= new HoSoVanBanDotLayYKien { HoSoVanBanId = hoSo.Id, BuocQuyTrinhId = currentStep.Id, LanLayYKien = request.LanLayYKien, CoQuanLayYKien = normalizedCoQuan };
                if (dot.Id == Guid.Empty) _dbContext.HoSoVanBanDotLayYKiens.Add(dot);
                dot.BuocQuyTrinhId = currentStep.Id;
                dot.CoQuanLayYKien = normalizedCoQuan;
                dot.CheDoNhapYKien = mode; dot.HinhThucLayYKien = request.HinhThucLayYKien; dot.SoVanBanLayYKien = request.SoVanBanLayYKien;
                dot.NgayGuiLayYKien = request.NgayGuiLayYKien; dot.HanPhanHoi = request.HanPhanHoi; dot.NgayCoKetQua = request.NgayCoKetQua;
                dot.NoiDungYeuCau = request.NoiDungYeuCau; dot.TongSoThanhVien = request.TongSoThanhVien; dot.SoDongY = request.SoDongY;
                dot.SoDongYCoYKien = request.SoDongYCoYKien; dot.SoKhongDongY = request.SoKhongDongY; dot.SoKhongPhanHoi = request.SoKhongPhanHoi;
                dot.TyLeDongY = request.TyLeDongY; dot.KetQuaChung = request.KetQuaChung; dot.NoiDungTongHop = request.NoiDungTongHop;
                dot.NoiDungTiepThu = request.NoiDungTiepThu; dot.NguoiTongHopId = currentUser.Id; dot.NgayTongHop = DateTime.Now;
                dot.TrangThai = request.TrangThai == "DA_XAC_NHAN" ? "DA_XAC_NHAN" : "NHAP"; dot.AttachedFileGroupId = request.AttachedFileGroupId; dot.GhiChu = request.GhiChu;
                await _dbContext.SaveChangesAsync();

                var oldMembers = await _dbContext.HoSoVanBanYKienThanhViens.Where(x => x.DotLayYKienId == dot.Id).ToListAsync();
                _dbContext.HoSoVanBanYKienThanhViens.RemoveRange(oldMembers);
                if (mode == "CHI_TIET") _dbContext.HoSoVanBanYKienThanhViens.AddRange(members.Select((x, i) => new HoSoVanBanYKienThanhVien
                {
                    DotLayYKienId = dot.Id, ThanhVienId = x.ThanhVienId, HoTenThanhVien = x.HoTenThanhVien.Trim(), ChucVu = x.ChucVu,
                    DonViId = x.DonViId, TenDonVi = x.TenDonVi, ThuTuHienThi = i + 1, CoQuyenBieuQuyet = x.CoQuyenBieuQuyet,
                    KetQuaYKien = string.IsNullOrWhiteSpace(x.KetQuaYKien) ? "CHUA_PHAN_HOI" : x.KetQuaYKien,
                    NoiDungYKien = x.NoiDungYKien, NoiDungTiepThu = x.NoiDungTiepThu, NgayPhanHoi = x.NgayPhanHoi,
                    TrangThaiPhanHoi = string.IsNullOrWhiteSpace(x.KetQuaYKien) || x.KetQuaYKien == "CHUA_PHAN_HOI" ? "CHUA_PHAN_HOI" : "DA_PHAN_HOI",
                    AttachedFileGroupId = x.AttachedFileGroupId, GhiChu = x.GhiChu
                }));

                if (dot.TrangThai == "DA_XAC_NHAN")
                {
                    currentProcessing!.IsCurrent = false; currentProcessing.NgayXuLy = DateTime.Now;
                    currentProcessing.KetQuaXuLy = successTransition; currentProcessing.NoiDungXuLy = request.NoiDungTongHop;
                    var transition = await GetTransitionAsync(hoSo.QuyTrinhSoanThaoId, currentStep.Id, successTransition);
                    var nextStep = transition == null ? null : await _dbContext.DanhMucBuocQuyTrinhs.AsNoTracking().FirstOrDefaultAsync(x => x.Id == transition.DenBuocId);
                    await AdvanceWorkflowAsync(hoSo, currentUser, nextStep, null, successTransition, request.NoiDungTongHop);
                }

                await _dbContext.SaveChangesAsync(); await transaction.CommitAsync();
                if (normalizedCoQuan == "HDND")
                {
                    return new CommonResponse("success", dot.TrangThai == "DA_XAC_NHAN" ? "ÄĂ£ xĂ¡c nháº­n káº¿t quáº£ láº¥y Ă½ kiáº¿n HÄND." : "ÄĂ£ lÆ°u nhĂ¡p káº¿t quáº£ láº¥y Ă½ kiáº¿n HÄND.", dot.Id);
                }
                return new CommonResponse("success", dot.TrangThai == "DA_XAC_NHAN" ? "ÄĂ£ xĂ¡c nháº­n káº¿t quáº£ láº¥y Ă½ kiáº¿n UBND." : "ÄĂ£ lÆ°u nhĂ¡p káº¿t quáº£ láº¥y Ă½ kiáº¿n UBND.", dot.Id);
            }
            catch { await transaction.RollbackAsync(); return new CommonResponse("error", "KhĂ´ng thá»ƒ lÆ°u káº¿t quáº£ láº¥y Ă½ kiáº¿n UBND."); }
        }

        private async Task<CommonResponse> GetDanhSachInternalAsync(
            string search,
            int pageSize,
            int pageCurrent,
            bool chiLayBuocDangKy,
            string? maBuoc = null,
            bool chiLayDonViDangNhap = false,
            Guid? donViSoanThaoId = null,
            bool chiLayTheoLichSuNhanXuLy = false,
            IEnumerable<string>? trangThaiNghiepVuFilters = null,
            string? loaiQuyTrinh = null,
            Guid? danhMucVanBanId = null,
            Guid? nguoiXuLyId = null,
            string? maTrangThai = null,
            string? maBuocLoc = null,
            DateTime? tuNgayTao = null,
            DateTime? denNgayTao = null,
            DateTime? tuHanXuLy = null,
            DateTime? denHanXuLy = null,
            DateTime? tuNgayHoanThanh = null,
            DateTime? denNgayHoanThanh = null)
        {
            try
            {
                var currentUser = _authService.GetUserInfo();
                var donViDangNhapId = currentUser?.DanhMucDonViId ?? Guid.Empty;
                var isSSA = currentUser?.SSA ?? false;
                var trangThaiNghiepVuFilterList = trangThaiNghiepVuFilters?
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => x.Trim().ToUpper())
                    .Distinct()
                    .ToList();
                var normalizedLoaiQuyTrinh = string.IsNullOrWhiteSpace(loaiQuyTrinh) ? null : NormalizeWorkflowType(loaiQuyTrinh);

                var query =
                    from hoSo in _dbContext.HoSoVanBans.AsNoTracking()
                    join vanBan in _dbContext.DanhMucVanBans.AsNoTracking() on hoSo.DanhMucVanBanId equals vanBan.Id
                    join quyTrinh in _dbContext.DanhMucQuyTrinhSoanThaos.AsNoTracking() on hoSo.QuyTrinhSoanThaoId equals quyTrinh.Id
                    join donVi in _dbContext.DanhMucDonVis.AsNoTracking() on hoSo.DonViSoanThaoId equals donVi.Id
                    join buoc in _dbContext.DanhMucBuocQuyTrinhs.AsNoTracking() on hoSo.BuocHienTaiId equals buoc.Id into buocJoin
                    from buoc in buocJoin.DefaultIfEmpty()
                    join xuLyCurrent in _dbContext.HoSoVanBanXuLys.AsNoTracking().Where(x => x.IsCurrent) on hoSo.Id equals xuLyCurrent.HoSoVanBanId into xuLyCurrentJoin
                    from xuLyCurrent in xuLyCurrentJoin.DefaultIfEmpty()
                    join donViXuLy in _dbContext.DanhMucDonVis.AsNoTracking() on xuLyCurrent.DonViXuLyId equals donViXuLy.Id into donViXuLyJoin
                    from donViXuLy in donViXuLyJoin.DefaultIfEmpty()
                    join trangThai in _dbContext.DanhMucTrangThais.AsNoTracking() on hoSo.DanhMucTrangThaiId equals trangThai.Id into trangThaiJoin
                    from trangThai in trangThaiJoin.DefaultIfEmpty()
                    where (normalizedLoaiQuyTrinh == null || quyTrinh.LoaiQuyTrinh == normalizedLoaiQuyTrinh)
                          && (!chiLayBuocDangKy || (buoc != null && buoc.ThuTuSapXep == 1))
                          && (string.IsNullOrWhiteSpace(maBuoc) ||
                              ((normalizedLoaiQuyTrinh == NormalizeWorkflowType("XayDung") && maBuoc == "SOAN_THAO")
                                  ? (chiLayTheoLichSuNhanXuLy
                                      ? _dbContext.HoSoVanBanXuLys.AsNoTracking().Any(x =>
                                          x.HoSoVanBanId == hoSo.Id &&
                                          (isSSA || donViDangNhapId == Guid.Empty || x.DonViXuLyId == donViDangNhapId) &&
                                          _dbContext.DanhMucBuocQuyTrinhs.AsNoTracking().Any(bq =>
                                              bq.Id == x.BuocQuyTrinhId &&
                                              (string.Equals(bq.LoaiBuoc, "SoanThao") || bq.ThuTuSapXep == 1)))
                                      : (buoc != null && (string.Equals(buoc.LoaiBuoc, "SoanThao") || buoc.ThuTuSapXep == 1)))
                                  : (chiLayTheoLichSuNhanXuLy
                                      ? _dbContext.HoSoVanBanXuLys.AsNoTracking().Any(x =>
                                          x.HoSoVanBanId == hoSo.Id &&
                                          (isSSA || donViDangNhapId == Guid.Empty || x.DonViXuLyId == donViDangNhapId) &&
                                          _dbContext.DanhMucBuocQuyTrinhs.AsNoTracking().Any(bq => bq.Id == x.BuocQuyTrinhId && bq.MaBuoc == maBuoc))
                                      : (buoc != null && buoc.MaBuoc == maBuoc))))
                          && (!chiLayDonViDangNhap || chiLayTheoLichSuNhanXuLy || isSSA || donViDangNhapId == Guid.Empty || (xuLyCurrent != null && xuLyCurrent.DonViXuLyId == donViDangNhapId))
                          && (!donViSoanThaoId.HasValue || donViSoanThaoId.Value == Guid.Empty ||
                              (chiLayTheoLichSuNhanXuLy
                                  ? (hoSo.DonViSoanThaoId == donViSoanThaoId.Value ||
                                     _dbContext.HoSoVanBanXuLys.AsNoTracking().Any(x =>
                                         x.HoSoVanBanId == hoSo.Id &&
                                         x.DonViXuLyId == donViSoanThaoId.Value &&
                                         _dbContext.DanhMucBuocQuyTrinhs.AsNoTracking().Any(bq =>
                                             bq.Id == x.BuocQuyTrinhId &&
                                             ((normalizedLoaiQuyTrinh == NormalizeWorkflowType("XayDung") && maBuoc == "SOAN_THAO")
                                                 ? (string.Equals(bq.LoaiBuoc, "SoanThao") || bq.ThuTuSapXep == 1)
                                                 : bq.MaBuoc == maBuoc))))
                                  : hoSo.DonViSoanThaoId == donViSoanThaoId.Value))
                          && (!danhMucVanBanId.HasValue || danhMucVanBanId.Value == Guid.Empty || hoSo.DanhMucVanBanId == danhMucVanBanId.Value)
                          && (!nguoiXuLyId.HasValue || nguoiXuLyId.Value == Guid.Empty || (xuLyCurrent != null && xuLyCurrent.NguoiXuLyId == nguoiXuLyId.Value))
                          && (trangThaiNghiepVuFilterList == null || trangThaiNghiepVuFilterList.Count == 0 ||
                              (xuLyCurrent != null &&
                               !string.IsNullOrWhiteSpace(xuLyCurrent.KetQuaXuLy) &&
                               trangThaiNghiepVuFilterList.Contains(xuLyCurrent.KetQuaXuLy.ToUpper())))
                    select new HoSoVanBanListItemModel
                    {
                        Id = hoSo.Id,
                        MaHoSo = hoSo.MaHoSo,
                        TenHoSo = hoSo.TenHoSo,
                        MaBuocHienTai = buoc != null ? buoc.MaBuoc : null,
                        TenLoaiVanBan = vanBan.TenLoaiVanBan,
                        ChuTheBanHanh = vanBan.ChuTheBanHanh,
                        TenQuyTrinh = quyTrinh.TenQuyTrinh,
                        TenBuocHienTai = buoc != null ? buoc.TenBuoc : "ÄĂ£ hoĂ n thĂ nh",
                        MaTrangThai = trangThai != null ? trangThai.MaTrangThai : null,
                        TenTrangThai = trangThai != null ? trangThai.TenTrangThai : null,
                        MaMauTrangThai = trangThai != null ? trangThai.MaMauHex : null,
                        TenDonViSoanThao = donVi.TenDonVi,
                        DonViXuLyHienTaiId = xuLyCurrent != null ? xuLyCurrent.DonViXuLyId : null,
                        TenDonViXuLyHienTai = donViXuLy != null ? donViXuLy.TenDonVi : null,
                        NguoiXuLyHienTaiId = xuLyCurrent != null ? xuLyCurrent.NguoiXuLyId : null,
                        NgayNhanHienTai = xuLyCurrent != null ? xuLyCurrent.NgayNhan : null,
                        TrangThaiNghiepVuTiepNhan = xuLyCurrent != null ? xuLyCurrent.KetQuaXuLy : null,
                        TenTrangThaiNghiepVuTiepNhan = xuLyCurrent != null ? xuLyCurrent.KetQuaXuLy : null,
                        NoiDungXuLyHienTai = xuLyCurrent != null ? xuLyCurrent.NoiDungXuLy : null,
                        NgayTaoHoSo = hoSo.NgayTaoHoSo,
                        HanXuLy = hoSo.HanXuLy,
                        NgayHoanThanh = hoSo.NgayHoanThanh,
                        SoLanTraLaiHienTai = hoSo.SoLanTraLaiHienTai
                    };

                if (!string.IsNullOrWhiteSpace(search))
                {
                    query = query.Where(x =>
                        x.MaHoSo.Contains(search) ||
                        x.TenHoSo.Contains(search) ||
                        (x.TenLoaiVanBan != null && x.TenLoaiVanBan.Contains(search)) ||
                        (x.TenQuyTrinh != null && x.TenQuyTrinh.Contains(search)) ||
                        (x.TenDonViSoanThao != null && x.TenDonViSoanThao.Contains(search)));
                }

                if (!string.IsNullOrWhiteSpace(maTrangThai))
                {
                    var normalizedMaTrangThai = maTrangThai.Trim().ToUpperInvariant();
                    query = query.Where(x => x.MaTrangThai != null && x.MaTrangThai.ToUpper() == normalizedMaTrangThai);
                }

                if (!string.IsNullOrWhiteSpace(maBuocLoc))
                {
                    var normalizedMaBuoc = maBuocLoc.Trim().ToUpperInvariant();
                    query = normalizedMaBuoc == "HOAN_THANH"
                        ? query.Where(x => x.NgayHoanThanh.HasValue || x.MaBuocHienTai == null)
                        : query.Where(x => x.MaBuocHienTai != null && x.MaBuocHienTai.ToUpper() == normalizedMaBuoc);
                }

                if (tuNgayTao.HasValue)
                {
                    var fromDate = tuNgayTao.Value.Date;
                    query = query.Where(x => x.NgayTaoHoSo.Date >= fromDate);
                }

                if (denNgayTao.HasValue)
                {
                    var toDate = denNgayTao.Value.Date;
                    query = query.Where(x => x.NgayTaoHoSo.Date <= toDate);
                }

                if (tuHanXuLy.HasValue)
                {
                    var fromDeadline = tuHanXuLy.Value.Date;
                    query = query.Where(x => x.HanXuLy.HasValue && x.HanXuLy.Value.Date >= fromDeadline);
                }

                if (denHanXuLy.HasValue)
                {
                    var toDeadline = denHanXuLy.Value.Date;
                    query = query.Where(x => x.HanXuLy.HasValue && x.HanXuLy.Value.Date <= toDeadline);
                }

                if (tuNgayHoanThanh.HasValue)
                {
                    var fromCompleted = tuNgayHoanThanh.Value.Date;
                    query = query.Where(x => x.NgayHoanThanh.HasValue && x.NgayHoanThanh.Value.Date >= fromCompleted);
                }

                if (denNgayHoanThanh.HasValue)
                {
                    var toCompleted = denNgayHoanThanh.Value.Date;
                    query = query.Where(x => x.NgayHoanThanh.HasValue && x.NgayHoanThanh.Value.Date <= toCompleted);
                }

                if (chiLayTheoLichSuNhanXuLy && maBuoc == "BUOC_02_THONG_NHAT")
                {
                    query = query.Where(x => x.MaBuocHienTai != "BUOC_01_DANG_KY" || x.TrangThaiNghiepVuTiepNhan != "TRA_LAI_HO_SO");
                }

                query = query.OrderByDescending(x => x.NgayTaoHoSo).ThenBy(x => x.MaHoSo);
                var totalRecord = await query.CountAsync();
                var data = await query.Skip((pageCurrent - 1) * pageSize).Take(pageSize).ToListAsync();

                if (data.Count > 0)
                {
                    var hoSoIds = data.Select(x => x.Id).ToList();
                    var duThaoHoSoIds = await _dbContext.HoSoVanBanDuThaos
                        .AsNoTracking()
                        .Where(x => hoSoIds.Contains(x.HoSoVanBanId))
                        .Select(x => x.HoSoVanBanId)
                        .Distinct()
                        .ToListAsync();
                    var danhGiaHoSoIds = await _dbContext.HoSoVanBanDanhGias
                        .AsNoTracking()
                        .Where(x => hoSoIds.Contains(x.HoSoVanBanId))
                        .Select(x => x.HoSoVanBanId)
                        .Distinct()
                        .ToListAsync();

                    var trackingMap = await BuildTrackingMapAsync(data.Select(x => x.Id));
                    foreach (var item in data)
                    {
                        item.CanXuLyBuocHienTai = isSSA ||
                                                  donViDangNhapId == Guid.Empty ||
                                                  (item.DonViXuLyHienTaiId.HasValue && item.DonViXuLyHienTaiId.Value == donViDangNhapId);
                        item.DaNhanHoSo = item.NguoiXuLyHienTaiId.HasValue;
                        item.CanNhanHoSo = item.CanXuLyBuocHienTai && !item.NguoiXuLyHienTaiId.HasValue;
                        item.DaCoDuThao = duThaoHoSoIds.Contains(item.Id);
                        item.DaCoBanGhiDanhGia = danhGiaHoSoIds.Contains(item.Id);
                        item.TenTrangThaiNghiepVuTiepNhan = ResolveTiepNhanNghiepVuLabel(item.TrangThaiNghiepVuTiepNhan);

                        if (string.Equals(maBuoc, "BUOC_02_THONG_NHAT", StringComparison.OrdinalIgnoreCase))
                        {
                            item.CanNhanHoSo = false;
                        }

                        if (!trackingMap.TryGetValue(item.Id, out var tracking))
                        {
                            continue;
                        }

                        item.TongSoBuoc = tracking.Summary.TongSoBuoc;
                        item.SoBuocHoanThanh = tracking.Summary.SoBuocHoanThanh;
                        item.SoBuocDungHan = tracking.Summary.SoBuocDungHan;
                        item.SoBuocQuaHan = tracking.Summary.SoBuocQuaHan;
                        item.SoBuocChuaThucHien = tracking.Summary.SoBuocChuaThucHien;
                        item.TyLeHoanThanh = tracking.Summary.TyLeHoanThanh;

                        var currentStep = tracking.Steps.FirstOrDefault(x => x.IsCurrent);
                        var latestStep = currentStep ?? tracking.Steps.OrderByDescending(x => x.ThuTuSapXep).FirstOrDefault();
                        if (latestStep != null)
                        {
                            item.TrangThaiTienDo = latestStep.MaTrangThaiTheoDoi;
                            item.TenTrangThaiTienDo = latestStep.TenTrangThaiTheoDoi;
                            item.MaMauTienDo = latestStep.MaMauTrangThaiTheoDoi;
                            item.DangOQuaHan = latestStep.MaTrangThaiTheoDoi is "QUA_HAN" or "HOAN_THANH_QUA_HAN";
                        }

                        if (normalizedLoaiQuyTrinh == NormalizeWorkflowType("XayDung") &&
                            currentStep != null &&
                            (string.Equals(currentStep.LoaiBuoc, "SoanThao", StringComparison.OrdinalIgnoreCase) ||
                             currentStep.ThuTuSapXep == 1))
                        {
                            item.CanNhanHoSo = false;
                            item.TenBuocHienTai = "\u0110ang so\u1ea1n th\u1ea3o";
                        }

                        if (normalizedLoaiQuyTrinh == NormalizeWorkflowType("XayDung") &&
                            string.Equals(item.TrangThaiNghiepVuTiepNhan, "DANG_LAY_GOP_Y", StringComparison.OrdinalIgnoreCase))
                        {
                            item.CanNhanHoSo = false;
                            item.CanXuLyBuocHienTai = false;
                            item.TenBuocHienTai = "ï¿½ang l?y ï¿½ ki?n gï¿½p ï¿½";
                        }

                        if (normalizedLoaiQuyTrinh == NormalizeWorkflowType("XayDung") &&
                            string.Equals(maBuoc, "SOAN_THAO", StringComparison.OrdinalIgnoreCase) &&
                            currentStep != null &&
                            !string.Equals(currentStep.LoaiBuoc, "SoanThao", StringComparison.OrdinalIgnoreCase) &&
                            currentStep.ThuTuSapXep != 1)
                        {
                            item.CanNhanHoSo = false;
                            item.CanXuLyBuocHienTai = false;
                        }

                        if (item.Id == Guid.Empty && item.Id != Guid.Empty)
                        {
                            item.CanNhanHoSo = false;
                            item.TenBuocHienTai = "Äang soáº¡n tháº£o";
                        }

                        if (item.NgayHoanThanh.HasValue ||
                            (item.TongSoBuoc > 0 && item.SoBuocHoanThanh >= item.TongSoBuoc))
                        {
                            item.MaTrangThai = "HOAN_THANH";
                            item.TenTrangThai = "ÄĂ£ hoĂ n thĂ nh";
                            item.MaMauTrangThai = "#28A745";
                            item.TenBuocHienTai = "ÄĂ£ hoĂ n thĂ nh";
                            item.CanXuLyBuocHienTai = false;
                        }
                    }

                    foreach (var item in data.Where(x =>
                                 x.NgayHoanThanh.HasValue ||
                                 (x.TongSoBuoc > 0 && x.SoBuocHoanThanh >= x.TongSoBuoc)))
                    {
                        var hoanThanhQuaHan = item.SoBuocQuaHan > 0;
                        item.MaTrangThai = hoanThanhQuaHan ? "HOAN_THANH_QUA_HAN" : "HOAN_THANH_DUNG_HAN";
                        item.TenTrangThai = hoanThanhQuaHan ? "HoĂ n thĂ nh quĂ¡ háº¡n" : "HoĂ n thĂ nh Ä‘Ăºng háº¡n";
                        item.MaMauTrangThai = hoanThanhQuaHan ? "#DC3545" : "#28A745";
                        item.TenBuocHienTai = "ÄĂ£ hoĂ n thĂ nh";
                        item.CanXuLyBuocHienTai = false;
                    }
                }

                foreach (var item in data.Where(x =>
                             x.NgayHoanThanh.HasValue ||
                             (x.TongSoBuoc > 0 && x.SoBuocHoanThanh >= x.TongSoBuoc)))
                {
                    var hoanThanhQuaHan = item.SoBuocQuaHan > 0;
                    item.TrangThaiTienDo = hoanThanhQuaHan ? "HOAN_THANH_QUA_HAN" : "HOAN_THANH_DUNG_HAN";
                    item.TenTrangThaiTienDo = hoanThanhQuaHan ? "HoĂ n thĂ nh quĂ¡ háº¡n" : "HoĂ n thĂ nh Ä‘Ăºng háº¡n";
                    item.MaMauTienDo = hoanThanhQuaHan ? "#DC3545" : "#28A745";
                    item.DangOQuaHan = hoanThanhQuaHan;
                }

                return new CommonResponse("success", "ThĂ nh cĂ´ng", data, totalRecord);
            }
            catch
            {
                return new CommonResponse();
            }
        }

        public async Task<CommonResponse> GetDonDocTienDoFormAsync(Guid hoSoVanBanId)
        {
            try
            {
                var hoSo = await (
                    from h in _dbContext.HoSoVanBans.AsNoTracking()
                    join vanBan in _dbContext.DanhMucVanBans.AsNoTracking() on h.DanhMucVanBanId equals vanBan.Id
                    join donVi in _dbContext.DanhMucDonVis.AsNoTracking() on h.DonViSoanThaoId equals donVi.Id
                    join buoc in _dbContext.DanhMucBuocQuyTrinhs.AsNoTracking() on h.BuocHienTaiId equals buoc.Id into buocJoin
                    from buoc in buocJoin.DefaultIfEmpty()
                    join xuLyCurrent in _dbContext.HoSoVanBanXuLys.AsNoTracking().Where(x => x.IsCurrent) on h.Id equals xuLyCurrent.HoSoVanBanId into xuLyCurrentJoin
                    from xuLyCurrent in xuLyCurrentJoin.DefaultIfEmpty()
                    join donViXuLy in _dbContext.DanhMucDonVis.AsNoTracking() on xuLyCurrent.DonViXuLyId equals donViXuLy.Id into donViXuLyJoin
                    from donViXuLy in donViXuLyJoin.DefaultIfEmpty()
                    where h.Id == hoSoVanBanId
                    select new HoSoVanBanDonDocFormModel
                    {
                        HoSoVanBanId = h.Id,
                        MaHoSo = h.MaHoSo,
                        TenHoSo = h.TenHoSo,
                        TenLoaiVanBan = vanBan.TenLoaiVanBan,
                        TenBuocHienTai = buoc != null ? buoc.TenBuoc : null,
                        TenDonViSoanThao = donVi.TenDonVi,
                        TenDonViXuLyHienTai = donViXuLy != null ? donViXuLy.TenDonVi : null,
                        HanXuLy = xuLyCurrent != null ? xuLyCurrent.HanXuLy : h.HanXuLy
                    }).FirstOrDefaultAsync();

                if (hoSo == null)
                {
                    return new CommonResponse("error", "Không tìm thấy hồ sơ cần đôn đốc.");
                }

                var trackingMap = await BuildTrackingMapAsync(new[] { hoSoVanBanId });
                var currentStep = trackingMap.TryGetValue(hoSoVanBanId, out var tracking)
                    ? tracking.Steps.FirstOrDefault(x => x.IsCurrent) ?? tracking.Steps.OrderByDescending(x => x.ThuTuSapXep).FirstOrDefault()
                    : null;

                var fakeItem = new HoSoVanBanListItemModel { SoLanTraLaiHienTai = 0 };
                if (currentStep != null)
                {
                    ResolveAlertInfo(fakeItem, currentStep);
                    hoSo.MucCanhBao = fakeItem.MucCanhBao;
                    hoSo.GhiChuCanhBao = fakeItem.GhiChuCanhBao;
                }

                hoSo.NoiDungDonDoc =
                    $"Đề nghị đơn vị khẩn trương rà soát và xử lý hồ sơ '{hoSo.TenHoSo}'" +
                    $"{(string.IsNullOrWhiteSpace(hoSo.TenBuocHienTai) ? string.Empty : $" tại bước '{hoSo.TenBuocHienTai}'")}" +
                    $"{(hoSo.HanXuLy.HasValue ? $", hạn xử lý {hoSo.HanXuLy.Value:dd/MM/yyyy}" : string.Empty)}.";

                return new CommonResponse("success", "Thành công", hoSo);
            }
            catch
            {
                return new CommonResponse();
            }
        }

        public async Task<CommonResponse> GuiDonDocTienDoAsync(HoSoVanBanDonDocFormModel request)
        {
            try
            {
                var currentUser = _authService.GetUserInfo();
                if (currentUser == null)
                {
                    return new CommonResponse("error", "Không xác định được tài khoản đang thao tác.");
                }

                if (string.IsNullOrWhiteSpace(request.NoiDungDonDoc))
                {
                    return new CommonResponse("error", "Nội dung đôn đốc không được để trống.");
                }

                var hoSo = await _dbContext.HoSoVanBans.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.HoSoVanBanId);
                if (hoSo == null)
                {
                    return new CommonResponse("error", "Không tìm thấy hồ sơ cần đôn đốc.");
                }

                var currentProcessing = await _dbContext.HoSoVanBanXuLys
                    .AsNoTracking()
                    .Where(x => x.HoSoVanBanId == request.HoSoVanBanId && x.IsCurrent)
                    .OrderByDescending(x => x.NgayNhan)
                    .FirstOrDefaultAsync();

                var donViNhan = currentProcessing?.DonViXuLyId != Guid.Empty
                    ? currentProcessing!.DonViXuLyId
                    : hoSo.DonViSoanThaoId;

                if (donViNhan == Guid.Empty || donViNhan == currentUser.DanhMucDonViId)
                {
                    return new CommonResponse("error", "Không xác định được đơn vị nhận đôn đốc phù hợp.");
                }

                var currentStep = await GetCurrentStepAsync(hoSo);
                var config = BuildNotificationNavigation(currentStep?.MaBuoc, hoSo.MaHoSo);
                var noiDung = request.NoiDungDonDoc.Trim();
                if (!noiDung.Contains("Đôn đốc", StringComparison.OrdinalIgnoreCase))
                {
                    noiDung = $"[Đôn đốc tiến độ] {noiDung}";
                }

                var thongBao = new Notification
                {
                    DonViGui = currentUser.DanhMucDonViId,
                    DonViTiepNhan = donViNhan,
                    DonViDongChuyen = string.Empty,
                    NoiDung = noiDung,
                    ControllerNameDanhSach = config.ControllerNameDanhSach,
                    ActionNameDanhSach = config.ActionNameDanhSach,
                    ParameterDanhSach = config.ParameterDanhSach,
                    ControllerNameXetDuyet = config.ControllerNameXetDuyet,
                    ActionNameXetDuyet = config.ActionNameXetDuyet,
                    ParameterXetDuyet = config.ParameterXetDuyet,
                    DonViView = new List<Guid>()
                };

                var result = await _notificationService.StoreAsync(thongBao);
                return result.Status == "success"
                    ? new CommonResponse("success", "Đã gửi thông báo đôn đốc thành công.")
                    : result;
            }
            catch
            {
                return new CommonResponse();
            }
        }

        public async Task<CommonResponse> GetDonDocTienDoHangLoatFormAsync(List<Guid> hoSoVanBanIds)
        {
            try
            {
                var ids = hoSoVanBanIds.Where(x => x != Guid.Empty).Distinct().ToList();
                if (ids.Count == 0)
                {
                    return new CommonResponse("error", "Bạn chưa chọn hồ sơ để đôn đốc.");
                }

                var hoSos = await (
                    from h in _dbContext.HoSoVanBans.AsNoTracking()
                    join buoc in _dbContext.DanhMucBuocQuyTrinhs.AsNoTracking() on h.BuocHienTaiId equals buoc.Id into buocJoin
                    from buoc in buocJoin.DefaultIfEmpty()
                    join xuLyCurrent in _dbContext.HoSoVanBanXuLys.AsNoTracking().Where(x => x.IsCurrent) on h.Id equals xuLyCurrent.HoSoVanBanId into xuLyCurrentJoin
                    from xuLyCurrent in xuLyCurrentJoin.DefaultIfEmpty()
                    join donViXuLy in _dbContext.DanhMucDonVis.AsNoTracking() on xuLyCurrent.DonViXuLyId equals donViXuLy.Id into donViXuLyJoin
                    from donViXuLy in donViXuLyJoin.DefaultIfEmpty()
                    where ids.Contains(h.Id)
                    select new HoSoVanBanDonDocHangLoatItemModel
                    {
                        HoSoVanBanId = h.Id,
                        MaHoSo = h.MaHoSo,
                        TenHoSo = h.TenHoSo,
                        TenBuocHienTai = buoc != null ? buoc.TenBuoc : null,
                        TenDonViXuLyHienTai = donViXuLy != null ? donViXuLy.TenDonVi : null,
                        HanXuLy = xuLyCurrent != null ? xuLyCurrent.HanXuLy : h.HanXuLy
                    })
                    .ToListAsync();

                if (hoSos.Count == 0)
                {
                    return new CommonResponse("error", "Không tìm thấy hồ sơ phù hợp để đôn đốc.");
                }

                var trackingMap = await BuildTrackingMapAsync(ids);
                foreach (var item in hoSos)
                {
                    if (!trackingMap.TryGetValue(item.HoSoVanBanId, out var tracking))
                    {
                        item.TenMucCanhBao = "Bình thường";
                        continue;
                    }

                    var currentStep = tracking.Steps.FirstOrDefault(x => x.IsCurrent)
                                      ?? tracking.Steps.OrderByDescending(x => x.ThuTuSapXep).FirstOrDefault();
                    var fakeItem = new HoSoVanBanListItemModel();
                    if (currentStep != null)
                    {
                        ResolveAlertInfo(fakeItem, currentStep);
                        item.TenMucCanhBao = fakeItem.TenMucCanhBao;
                    }
                }

                var model = new HoSoVanBanDonDocHangLoatFormModel
                {
                    HoSoVanBanIds = hoSos.Select(x => x.HoSoVanBanId).ToList(),
                    TongSoHoSo = hoSos.Count,
                    NoiDungDonDoc = $"Đề nghị các đơn vị khẩn trương rà soát, xử lý và báo cáo tiến độ các hồ sơ được đôn đốc tính đến ngày {DateTime.Today:dd/MM/yyyy}.",
                    HoSos = hoSos.OrderBy(x => x.MaHoSo).ToList()
                };

                return new CommonResponse("success", "Thành công", model);
            }
            catch
            {
                return new CommonResponse();
            }
        }

        public async Task<CommonResponse> GuiDonDocTienDoHangLoatAsync(HoSoVanBanDonDocHangLoatFormModel request)
        {
            try
            {
                var ids = request.HoSoVanBanIds.Where(x => x != Guid.Empty).Distinct().ToList();
                if (ids.Count == 0)
                {
                    return new CommonResponse("error", "Bạn chưa chọn hồ sơ để đôn đốc.");
                }

                if (string.IsNullOrWhiteSpace(request.NoiDungDonDoc))
                {
                    return new CommonResponse("error", "Nội dung đôn đốc không được để trống.");
                }

                var successCount = 0;
                foreach (var id in ids)
                {
                    var singleRequest = new HoSoVanBanDonDocFormModel
                    {
                        HoSoVanBanId = id,
                        NoiDungDonDoc = request.NoiDungDonDoc.Trim()
                    };

                    var result = await GuiDonDocTienDoAsync(singleRequest);
                    if (result.Status == "success")
                    {
                        successCount++;
                    }
                }

                if (successCount == 0)
                {
                    return new CommonResponse("error", "Không gửi được đôn đốc cho hồ sơ nào.");
                }

                return new CommonResponse("success", $"Đã gửi đôn đốc cho {successCount}/{ids.Count} hồ sơ.");
            }
            catch
            {
                return new CommonResponse();
            }
        }

        public async Task<List<DataAccess.Entities.QuanLyDanhMuc.DanhMucVanBan>> GetDanhMucVanBanOptionsAsync()
        {
            return await _dbContext.DanhMucVanBans
                .AsNoTracking()
                .Where(x => x.TrangThai)
                .OrderBy(x => x.ThuTuSapXep)
                .ThenBy(x => x.TenLoaiVanBan)
                .ToListAsync();
        }

        public async Task<List<DonViOptionModel>> GetDonViOptionsAsync()
        {
            return await _dbContext.DanhMucDonVis
                .AsNoTracking()
                .OrderBy(x => x.Id == Guid.Parse("40000000-0000-0000-0000-000000000013") ? 0 :
                              x.Id == Guid.Parse("40000000-0000-0000-0000-000000000002") ? 1 :
                              x.Id == Guid.Parse("40000000-0000-0000-0000-000000000003") ? 2 : 9)
                .ThenBy(x => x.TenDonVi)
                .Select(x => new DonViOptionModel
                {
                    Id = x.Id,
                    TenDonVi = x.TenDonVi
                })
                .ToListAsync();
        }

        public async Task<List<SelectOptionModel>> GetNguoiXuLyOptionsAsync(Guid? donViId = null)
        {
            return await (
                from user in _dbContext.Users.AsNoTracking()
                join donVi in _dbContext.DanhMucDonVis.AsNoTracking() on user.DanhMucDonViId equals donVi.Id into donViJoin
                from donVi in donViJoin.DefaultIfEmpty()
                where user.Status == "Kích hoạt"
                      && (!donViId.HasValue || donViId.Value == Guid.Empty || user.DanhMucDonViId == donViId.Value)
                orderby user.Name
                select new SelectOptionModel
                {
                    Value = user.Id.ToString(),
                    Text = donVi != null ? $"{user.Name} - {donVi.TenDonVi}" : user.Name
                }).ToListAsync();
        }

        public async Task<List<SelectOptionModel>> GetBuocTheoDoiTienDoOptionsAsync()
        {
            var items = await _dbContext.DanhMucBuocQuyTrinhs
                .AsNoTracking()
                .Where(x => _dbContext.DanhMucQuyTrinhSoanThaos.Any(q => q.Id == x.QuyTrinhSoanThaoId && q.LoaiQuyTrinh == NormalizeWorkflowType("XayDung")))
                .OrderBy(x => x.ThuTuSapXep)
                .ThenBy(x => x.TenBuoc)
                .Select(x => new SelectOptionModel
                {
                    Value = x.MaBuoc,
                    Text = x.TenBuoc
                })
                .ToListAsync();

            return items
                .GroupBy(x => x.Value)
                .Select(x => x.First())
                .ToList();
        }

        public async Task<List<HoSoDangKyOptionModel>> GetHoSoDangKyOptionsAsync(Guid? donViId = null, bool isSSA = false)
        {
            var query = _dbContext.HoSoVanBans
                .AsNoTracking()
                .Where(x => _dbContext.DanhMucQuyTrinhSoanThaos.Any(q => q.Id == x.QuyTrinhSoanThaoId && q.LoaiQuyTrinh == NormalizeWorkflowType("DangKy")))
                .Select(x => new
                {
                    x.Id,
                    x.TenHoSo,
                    x.DanhMucVanBanId,
                    x.DonViSoanThaoId,
                    TenLoaiVanBan = _dbContext.DanhMucVanBans
                        .Where(v => v.Id == x.DanhMucVanBanId)
                        .Select(v => v.TenLoaiVanBan)
                        .FirstOrDefault(),
                    TenDonViSoanThao = _dbContext.DanhMucDonVis
                        .Where(d => d.Id == x.DonViSoanThaoId)
                        .Select(d => d.TenDonVi)
                        .FirstOrDefault(),
                    x.NgayTaoHoSo
                });

            if (!isSSA && donViId.HasValue && donViId.Value != Guid.Empty)
            {
                query = query.Where(x => x.DonViSoanThaoId == donViId.Value);
            }

            return await query
                .OrderByDescending(x => x.NgayTaoHoSo)
                .ThenBy(x => x.TenHoSo)
                .Select(x => new HoSoDangKyOptionModel
                {
                    Id = x.Id,
                    TenHoSo = x.TenHoSo,
                    DanhMucVanBanId = x.DanhMucVanBanId,
                    TenLoaiVanBan = x.TenLoaiVanBan,
                    DonViSoanThaoId = x.DonViSoanThaoId,
                    TenDonViSoanThao = x.TenDonViSoanThao,
                    NhanHienThi = string.IsNullOrWhiteSpace(x.TenLoaiVanBan)
                        ? x.TenHoSo
                        : $"{x.TenHoSo} ({x.TenLoaiVanBan})"
                })
                .ToListAsync();
        }

        public async Task<List<DataAccess.Entities.QuanLyDanhMuc.DanhMucQuyTrinhSoanThao>> GetQuyTrinhOptionsAsync(Guid? danhMucVanBanId = null, string? loaiQuyTrinh = null)
        {
            var query = _dbContext.DanhMucQuyTrinhSoanThaos
                .AsNoTracking()
                .Where(x => x.TrangThai);

            if (!string.IsNullOrWhiteSpace(loaiQuyTrinh))
            {
                var normalizedLoaiQuyTrinh = NormalizeWorkflowType(loaiQuyTrinh);
                query = query.Where(x => x.LoaiQuyTrinh == normalizedLoaiQuyTrinh);
            }

            if (danhMucVanBanId.HasValue && danhMucVanBanId.Value != Guid.Empty)
            {
                var vanBanIdToken = danhMucVanBanId.Value.ToString();
                query = query.Where(x =>
                    x.DanhMucVanBanId == danhMucVanBanId.Value ||
                    (!string.IsNullOrWhiteSpace(x.DanhMucVanBanIds) && x.DanhMucVanBanIds.Contains(vanBanIdToken)) ||
                    (!x.DanhMucVanBanId.HasValue && string.IsNullOrWhiteSpace(x.DanhMucVanBanIds)));
            }

            return await query.OrderBy(x => x.TenQuyTrinh).ToListAsync();
        }

        public async Task<List<DataAccess.Entities.QuanLyDanhMuc.DanhMucQuyTrinhSoanThao>> GetDraftQuyTrinhOptionsAsync(Guid danhMucVanBanId)
        {
            var vanBanIdToken = danhMucVanBanId.ToString();
            var loaiQuyTrinh = NormalizeWorkflowType("XayDung");

            return await _dbContext.DanhMucQuyTrinhSoanThaos
                .AsNoTracking()
                .Where(x => x.TrangThai && x.LoaiQuyTrinh == loaiQuyTrinh)
                .Where(x => _dbContext.DanhMucBuocQuyTrinhs
                    .AsNoTracking()
                    .Any(b => b.QuyTrinhSoanThaoId == x.Id &&
                              string.Equals(b.LoaiBuoc, "SoanThao")))
                .Where(x => x.DanhMucVanBanId == danhMucVanBanId ||
                            (!string.IsNullOrWhiteSpace(x.DanhMucVanBanIds) && x.DanhMucVanBanIds.Contains(vanBanIdToken)) ||
                            (!x.DanhMucVanBanId.HasValue && string.IsNullOrWhiteSpace(x.DanhMucVanBanIds)))
                .OrderBy(x => x.TenQuyTrinh)
                .ToListAsync();
        }

        public async Task<List<HoSoVanBanBuocThoiHanEditModel>> GetBuocThoiHanOptionsAsync(Guid quyTrinhSoanThaoId)
        {
            return await _dbContext.DanhMucBuocQuyTrinhs
                .AsNoTracking()
                .Where(x => x.QuyTrinhSoanThaoId == quyTrinhSoanThaoId)
                .OrderBy(x => x.ThuTuSapXep)
                .ThenBy(x => x.MaBuoc)
                .Select(x => new HoSoVanBanBuocThoiHanEditModel
                {
                    BuocQuyTrinhId = x.Id,
                    MaBuoc = x.MaBuoc,
                    TenBuoc = x.TenBuoc,
                    ThuTuSapXep = x.ThuTuSapXep,
                    SoNgayXuLy = x.SoNgayXuLyTieuChuan,
                    SoNgayCanhBaoSapHan = x.SoNgayCanhBaoSapHan
                })
                .ToListAsync();
        }

        public async Task<CommonResponse> CreateHoSoAsync(HoSoVanBanCreateModel request)
        {
            var currentUser = _authService.GetUserInfo();
            if (currentUser == null)
            {
                return new CommonResponse("error", "KhĂ´ng xĂ¡c Ä‘á»‹nh Ä‘Æ°á»£c tĂ i khoáº£n Ä‘ang thao tĂ¡c!");
            }

            if (!request.TuNgaySoanThao.HasValue || !request.DenNgaySoanThao.HasValue)
            {
                return new CommonResponse("error", "Thá»i gian soáº¡n tháº£o báº¯t buá»™c pháº£i nháº­p.");
            }

            if (request.DenNgaySoanThao.Value.Date < request.TuNgaySoanThao.Value.Date)
            {
                return new CommonResponse("error", "Äáº¿n ngĂ y soáº¡n tháº£o pháº£i lá»›n hÆ¡n hoáº·c báº±ng tá»« ngĂ y soáº¡n tháº£o.");
            }

            if (request.Id == Guid.Empty)
            {
                request.Id = Guid.NewGuid();
            }

            request.AttachedFileGroupId = request.Id;
            request.HanXuLy = request.DenNgaySoanThao.Value.Date;

            if (string.IsNullOrWhiteSpace(request.TenHoSo))
            {
                return new CommonResponse("error", "Ma ho so va ten ho so khong duoc de trong!");
            }

            if (await _dbContext.HoSoVanBans.AnyAsync(x => x.Id == request.Id))
            {
                return new CommonResponse("error", "MĂ£ há»“ sÆ¡ Ä‘Ă£ tá»“n táº¡i!");
            }

            request.BuocThoiHans = request.BuocThoiHans
                .Where(x => x.BuocQuyTrinhId != Guid.Empty)
                .OrderBy(x => x.ThuTuSapXep)
                .ToList();

            var quyTrinh = await _dbContext.DanhMucQuyTrinhSoanThaos
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.QuyTrinhSoanThaoId && x.TrangThai);

            if (quyTrinh == null)
            {
                return new CommonResponse("error", "KhĂ´ng tĂ¬m tháº¥y quy trĂ¬nh soáº¡n tháº£o Ä‘ang kĂ­ch hoáº¡t!");
            }

            DanhMucBuocQuyTrinh? firstStep;
            if (quyTrinh.LoaiQuyTrinh == NormalizeWorkflowType("XayDung"))
            {
                firstStep = await ResolveDraftStartStepAsync(request.QuyTrinhSoanThaoId)
                    ?? await _dbContext.DanhMucBuocQuyTrinhs
                        .AsNoTracking()
                        .Where(x => x.QuyTrinhSoanThaoId == request.QuyTrinhSoanThaoId)
                        .OrderBy(x => x.ThuTuSapXep)
                        .ThenBy(x => x.MaBuoc)
                        .FirstOrDefaultAsync();
            }
            else
            {
                firstStep = await _dbContext.DanhMucBuocQuyTrinhs
                    .AsNoTracking()
                    .Where(x => x.QuyTrinhSoanThaoId == request.QuyTrinhSoanThaoId)
                    .OrderBy(x => x.ThuTuSapXep)
                    .ThenBy(x => x.MaBuoc)
                    .FirstOrDefaultAsync();
            }

            if (firstStep == null)
            {
                return new CommonResponse("error", "Quy trĂ¬nh chÆ°a cĂ³ bÆ°á»›c nĂ o Ä‘á»ƒ khá»Ÿi táº¡o há»“ sÆ¡!");
            }

            var stepDeadlinePlans = await BuildRequestedStepDeadlinePlansAsync(request.QuyTrinhSoanThaoId, request.BuocThoiHans);

            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var dangXuLyStatusId = await GetTrangThaiIdByCodeAsync("DANG_XU_LY");
                var now = DateTime.Now;
                var firstStepPlan = stepDeadlinePlans.FirstOrDefault(x => x.BuocQuyTrinhId == firstStep.Id);
                var hanXuLyBuocDau = request.HanXuLy ?? CalculateStepDeadline(firstStepPlan?.SoNgayXuLy ?? firstStep.SoNgayXuLyTieuChuan, now);
                var donViTiepNhanBuocDauId = await ResolveAssignedDonViXuLyIdAsync(
                    new HoSoVanBan
                    {
                        DonViSoanThaoId = currentUser.DanhMucDonViId != Guid.Empty
                            ? currentUser.DanhMucDonViId
                            : Guid.Empty
                    },
                    firstStep,
                    currentUser.DanhMucDonViId);

                var hoSo = new HoSoVanBan
                {
                    Id = request.Id,
                    MaHoSo = request.Id.ToString(),
                    TenHoSo = request.TenHoSo.Trim(),
                    DanhMucVanBanId = request.DanhMucVanBanId,
                    QuyTrinhSoanThaoId = request.QuyTrinhSoanThaoId,
                    BuocHienTaiId = firstStep.Id,
                    DanhMucTrangThaiId = dangXuLyStatusId,
                    DonViSoanThaoId = request.DonViDeNghiId.HasValue && request.DonViDeNghiId.Value != Guid.Empty
                        ? request.DonViDeNghiId.Value
                        : currentUser.DanhMucDonViId,
                    NguoiTaoId = currentUser.Id,
                    NgayTaoHoSo = now,
                    HanXuLy = hanXuLyBuocDau,
                    AttachedFileGroupId = request.Id,
                    MoTa = request.MoTa?.Trim(),
                    GhiChu = request.GhiChu?.Trim(),
                    SoLanTraLaiHienTai = 0
                };

                _dbContext.HoSoVanBans.Add(hoSo);
                await _dbContext.SaveChangesAsync();

                if (stepDeadlinePlans.Count > 0)
                {
                    var planRows = stepDeadlinePlans.Select(x => new HoSoVanBanBuocThoiHan
                    {
                        HoSoVanBanId = hoSo.Id,
                        BuocQuyTrinhId = x.BuocQuyTrinhId,
                        ThuTuSapXep = x.ThuTuSapXep,
                        SoNgayXuLy = x.SoNgayXuLy,
                        SoNgayCanhBaoSapHan = x.SoNgayCanhBaoSapHan,
                        GhiChu = x.GhiChu
                    }).ToList();

                    _dbContext.HoSoVanBanBuocThoiHans.AddRange(planRows);
                    await _dbContext.SaveChangesAsync();
                }

                var xuLyDauTien = new HoSoVanBanXuLy
                {
                    HoSoVanBanId = hoSo.Id,
                    BuocQuyTrinhId = firstStep.Id,
                    LanXuLy = 1,
                    DonViXuLyId = donViTiepNhanBuocDauId == Guid.Empty ? hoSo.DonViSoanThaoId : donViTiepNhanBuocDauId,
                    NguoiXuLyId = donViTiepNhanBuocDauId == currentUser.DanhMucDonViId ? currentUser.Id : null,
                    NgayNhan = now,
                    HanXuLy = hanXuLyBuocDau,
                    DanhMucTrangThaiId = dangXuLyStatusId,
                    IsCurrent = true,
                    KetQuaXuLy = null,
                    NoiDungXuLy = "Khá»Ÿi táº¡o há»“ sÆ¡ vĂ o quy trĂ¬nh.",
                    GhiChu = request.GhiChu
                };

                _dbContext.HoSoVanBanXuLys.Add(xuLyDauTien);
                await _dbContext.SaveChangesAsync();

                await transaction.CommitAsync();

                await TaoThongBaoAsync(
                    hoSo,
                    firstStep,
                    currentUser.DanhMucDonViId,
                    xuLyDauTien.DonViXuLyId,
                    $"Há»“ sÆ¡ '{hoSo.TenHoSo}' Ä‘Ă£ Ä‘Æ°á»£c khá»Ÿi táº¡o vĂ  Ä‘ang á»Ÿ bÆ°á»›c '{firstStep.TenBuoc}'.");

                return new CommonResponse("success", "ThĂ nh cĂ´ng", hoSo.Id);
            }
            catch
            {
                await transaction.RollbackAsync();
                return new CommonResponse();
            }
        }

        public async Task<CommonResponse> GetHoSoEditModelAsync(Guid hoSoVanBanId)
        {
            var currentUser = _authService.GetUserInfo();
            if (currentUser == null)
            {
                return new CommonResponse("error", "KhĂ´ng xĂ¡c Ä‘á»‹nh Ä‘Æ°á»£c tĂ i khoáº£n Ä‘ang thao tĂ¡c!");
            }

            var hoSo = await _dbContext.HoSoVanBans
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == hoSoVanBanId);

            if (hoSo == null)
            {
                return new CommonResponse("error", "KhĂ´ng tĂ¬m tháº¥y há»“ sÆ¡ vÄƒn báº£n!");
            }

            var currentStep = await GetCurrentStepAsync(hoSo);
            var currentProcessing = await _dbContext.HoSoVanBanXuLys
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.HoSoVanBanId == hoSo.Id && x.IsCurrent);

            if (!CanEditDangKyHoSo(currentUser, hoSo, currentStep, currentProcessing))
            {
                return new CommonResponse("error", "Há»“ sÆ¡ nĂ y Ä‘Ă£ Ä‘Æ°á»£c gá»­i Ä‘i hoáº·c báº¡n khĂ´ng cĂ³ quyá»n cáº­p nháº­t.");
            }

            var buocThoiHans = await _dbContext.HoSoVanBanBuocThoiHans
                .AsNoTracking()
                .Where(x => x.HoSoVanBanId == hoSo.Id)
                .OrderBy(x => x.ThuTuSapXep)
                .Select(x => new HoSoVanBanBuocThoiHanEditModel
                {
                    BuocQuyTrinhId = x.BuocQuyTrinhId,
                    ThuTuSapXep = x.ThuTuSapXep,
                    SoNgayXuLy = x.SoNgayXuLy,
                    SoNgayCanhBaoSapHan = x.SoNgayCanhBaoSapHan,
                    GhiChu = x.GhiChu
                })
                .ToListAsync();

            if (buocThoiHans.Count > 0)
            {
                var stepInfo = await _dbContext.DanhMucBuocQuyTrinhs
                    .AsNoTracking()
                    .Where(x => x.QuyTrinhSoanThaoId == hoSo.QuyTrinhSoanThaoId)
                    .Select(x => new { x.Id, x.MaBuoc, x.TenBuoc, x.ThuTuSapXep })
                    .ToListAsync();

                var stepMap = stepInfo.ToDictionary(x => x.Id);
                foreach (var item in buocThoiHans)
                {
                    if (stepMap.TryGetValue(item.BuocQuyTrinhId, out var step))
                    {
                        item.MaBuoc = step.MaBuoc;
                        item.TenBuoc = step.TenBuoc;
                        item.ThuTuSapXep = step.ThuTuSapXep;
                    }
                }
            }
            else
            {
                buocThoiHans = await GetBuocThoiHanOptionsAsync(hoSo.QuyTrinhSoanThaoId);
            }

            var model = new HoSoVanBanCreateModel
            {
                Id = hoSo.Id,
                DonViDeNghiId = hoSo.DonViSoanThaoId,
                TenHoSo = hoSo.TenHoSo,
                DanhMucVanBanId = hoSo.DanhMucVanBanId,
                QuyTrinhSoanThaoId = hoSo.QuyTrinhSoanThaoId,
                HanXuLy = hoSo.HanXuLy?.Date,
                TuNgaySoanThao = hoSo.NgayTaoHoSo.Date,
                DenNgaySoanThao = hoSo.HanXuLy?.Date,
                AttachedFileGroupId = hoSo.AttachedFileGroupId,
                MoTa = hoSo.MoTa,
                GhiChu = hoSo.GhiChu,
                BuocThoiHans = buocThoiHans
            };

            return new CommonResponse("success", "ThĂ nh cĂ´ng", model);
        }

        public async Task<CommonResponse> GetChuyenHoSoModelAsync(Guid hoSoVanBanId)
        {
            var currentUser = _authService.GetUserInfo();
            if (currentUser == null)
            {
                return new CommonResponse("error", "Khong xac dinh duoc tai khoan dang thao tac!");
            }

            var hoSo = await GetHoSoWithCurrentStepAsync(hoSoVanBanId);
            if (hoSo == null)
            {
                return new CommonResponse("error", "Khong tim thay ho so van ban!");
            }

            var currentStep = await GetCurrentStepAsync(hoSo);
            if (currentStep == null || currentStep.MaBuoc != "BUOC_01_DANG_KY")
            {
                return new CommonResponse("error", "Ho so khong con o buoc dang ky de chuyen.");
            }

            var currentProcessing = await _dbContext.HoSoVanBanXuLys
                .AsNoTracking()
                .Where(x => x.HoSoVanBanId == hoSo.Id && x.IsCurrent)
                .OrderByDescending(x => x.NgayNhan)
                .FirstOrDefaultAsync();

            if (!CanCurrentUserXuLy(currentUser, currentProcessing))
            {
                return new CommonResponse("error", "Ho so nay da duoc chuyen sang don vi khac. Ban khong the cap nhat nua!");
            }

            var nextTransition = await GetTransitionAsync(hoSo.QuyTrinhSoanThaoId, currentStep.Id, "HOAN_THANH_DANG_KY");
            var nextStep = nextTransition == null
                ? null
                : await _dbContext.DanhMucBuocQuyTrinhs.AsNoTracking().FirstOrDefaultAsync(x => x.Id == nextTransition.DenBuocId);

            var nextStepDeadline = await ResolveNextStepDeadlineAsync(hoSo.Id, nextStep);

            return new CommonResponse("success", "Thanh cong", new HoSoVanBanXuLyStepModel
            {
                HoSoVanBanId = hoSo.Id,
                MaHoSo = hoSo.MaHoSo,
                TenHoSo = hoSo.TenHoSo,
                KetQuaXuLy = "HOAN_THANH_DANG_KY",
                NgayXuLy = DateTime.Today,
                HanXuLy = nextStepDeadline,
                NoiDungXuLy = $"Chuyen ho so {hoSo.TenHoSo} len don vi tiep nhan.",
                GhiChu = $"Gui ho so dang ky xay dung van ban {hoSo.TenHoSo}."
            });
        }

        public async Task<CommonResponse> GetChuyenXetDuyetDuThaoModelAsync(Guid hoSoVanBanId)
        {
            var currentUser = _authService.GetUserInfo();
            if (currentUser == null)
            {
                return new CommonResponse("error", "Khong xac dinh duoc tai khoan dang thao tac!");
            }

            var hoSo = await GetHoSoWithCurrentStepAsync(hoSoVanBanId);
            if (hoSo == null)
            {
                return new CommonResponse("error", "Khong tim thay ho so du thao!");
            }

            var currentStep = await GetCurrentStepAsync(hoSo);
            if (currentStep == null || !string.Equals(currentStep.LoaiBuoc, "SoanThao", StringComparison.OrdinalIgnoreCase))
            {
                return new CommonResponse("error", "Ho so hien khong o buoc du thao van ban de chuyen xet duyet.");
            }

            var currentProcessing = await _dbContext.HoSoVanBanXuLys
                .AsNoTracking()
                .Where(x => x.HoSoVanBanId == hoSo.Id && x.IsCurrent)
                .OrderByDescending(x => x.NgayNhan)
                .FirstOrDefaultAsync();

            if (!CanCurrentUserXuLy(currentUser, currentProcessing))
            {
                return new CommonResponse("error", "Ho so nay da duoc chuyen sang don vi khac. Ban khong the cap nhat nua!");
            }

            if (!string.Equals(currentProcessing?.KetQuaXuLy, "DA_TONG_HOP_Y_KIEN", StringComparison.OrdinalIgnoreCase))
            {
                return new CommonResponse("error", "Ho so chua hoan thanh buoc lay y kien de chuyen xet duyet du thao.");
            }

            var nextTransition = await ResolveTransitionByResultsAsync(
                hoSo.QuyTrinhSoanThaoId,
                currentStep.Id,
                "GUI_LAY_Y_KIEN",
                "GUI_THAM_DINH",
                "HOAN_THANH_DU_THAO");
            var nextStep = nextTransition == null
                ? null
                : await _dbContext.DanhMucBuocQuyTrinhs.AsNoTracking().FirstOrDefaultAsync(x => x.Id == nextTransition.DenBuocId);

            if (nextStep == null)
            {
                return new CommonResponse("error", "Workflow chua cau hinh buoc tiep theo cho nghiep vu chuyen xet duyet du thao.");
            }

            var nextStepDeadline = await ResolveNextStepDeadlineAsync(hoSo.Id, nextStep);
            var defaultDonViTiepNhanId = await ResolveExistingDonViIdAsync(SoTuPhapDonViId);
            if (defaultDonViTiepNhanId == Guid.Empty)
            {
                defaultDonViTiepNhanId = nextStep.DonViTiepNhanMacDinhId ?? Guid.Empty;
            }

            var draftVersionNumber = ResolveDraftVersionNumber(hoSo.SoLanTraLaiHienTai);

            return new CommonResponse("success", "Thanh cong", new HoSoVanBanXuLyStepModel
            {
                HoSoVanBanId = hoSo.Id,
                MaHoSo = hoSo.MaHoSo,
                TenHoSo = hoSo.TenHoSo,
                KetQuaXuLy = nextTransition!.DieuKienKetQua,
                NgayXuLy = DateTime.Today,
                HanXuLy = nextStepDeadline,
                DonViTiepNhanId = defaultDonViTiepNhanId == Guid.Empty ? null : defaultDonViTiepNhanId,
                DefaultDonViTiepNhanId = defaultDonViTiepNhanId == Guid.Empty ? null : defaultDonViTiepNhanId,
                DraftVersionNumber = draftVersionNumber,
                DraftVersionLabel = ResolveDraftVersionLabel(draftVersionNumber),
                NoiDungXuLy = $"Chuyen ho so du thao {hoSo.TenHoSo} den don vi xet duyet.",
                GhiChu = $"Gui ho so du thao van ban {hoSo.TenHoSo} sang buoc xet duyet."
            });
        }

        public async Task<CommonResponse> GetChuyenDanhGiaModelAsync(Guid hoSoVanBanId)
        {
            var currentUser = _authService.GetUserInfo();
            if (currentUser == null)
            {
                return new CommonResponse("error", "Khï¿½ng xï¿½c d?nh du?c tï¿½i kho?n dang thao tï¿½c!");
            }

            var hoSo = await GetHoSoWithCurrentStepAsync(hoSoVanBanId);
            if (hoSo == null)
            {
                return new CommonResponse("error", "Khï¿½ng tï¿½m th?y h? so c?n chuy?n xï¿½t duy?t.");
            }

            var currentStep = await GetCurrentStepAsync(hoSo);
            if (currentStep == null)
            {
                return new CommonResponse("error", "Khï¿½ng xï¿½c d?nh du?c bu?c hi?n t?i c?a h? so.");
            }

            var currentProcessing = await _dbContext.HoSoVanBanXuLys
                .AsNoTracking()
                .Where(x => x.HoSoVanBanId == hoSo.Id && x.IsCurrent)
                .OrderByDescending(x => x.NgayNhan)
                .FirstOrDefaultAsync();

            if (!CanCurrentUserXuLy(currentUser, currentProcessing))
            {
                return new CommonResponse("error", "H? so nï¿½y khï¿½ng thu?c don v? dang dang nh?p d? chuy?n xï¿½t duy?t.");
            }

            var nextTransition = await ResolveTransitionByResultsAsync(
                hoSo.QuyTrinhSoanThaoId,
                currentStep.Id,
                "GUI_THAM_DINH",
                "CHUYEN_XET_DUYET_DANH_GIA");

            var nextStep = nextTransition == null
                ? await _dbContext.DanhMucBuocQuyTrinhs
                    .AsNoTracking()
                    .Where(x => x.QuyTrinhSoanThaoId == hoSo.QuyTrinhSoanThaoId && x.ThuTuSapXep > currentStep.ThuTuSapXep)
                    .OrderBy(x => x.ThuTuSapXep)
                    .FirstOrDefaultAsync()
                : await _dbContext.DanhMucBuocQuyTrinhs
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == nextTransition.DenBuocId);

            if (nextStep == null)
            {
                return new CommonResponse("error", "Workflow chua c?u hï¿½nh bu?c k? ti?p cho nghi?p v? chuy?n xï¿½t duy?t.");
            }

            var nextStepDeadline = await ResolveNextStepDeadlineAsync(hoSo.Id, nextStep);

            return new CommonResponse("success", "Thï¿½nh cï¿½ng", new HoSoVanBanXuLyStepModel
            {
                HoSoVanBanId = hoSo.Id,
                TenHoSo = hoSo.TenHoSo,
                KetQuaXuLy = nextTransition?.DieuKienKetQua ?? "GUI_THAM_DINH",
                NgayXuLy = DateTime.Now,
                HanXuLy = nextStepDeadline,
                NoiDungXuLy = $"Chuy?n h? so {hoSo.TenHoSo} sang bu?c xï¿½t duy?t.",
                GhiChu = $"Chuy?n h? so {hoSo.TenHoSo} sang bu?c x? lï¿½ k? ti?p."
            });
        }

        public async Task<CommonResponse> GetChuyenPheDuyetModelAsync(Guid hoSoVanBanId)
        {
            var currentUser = _authService.GetUserInfo();
            if (currentUser == null)
            {
                return new CommonResponse("error", "Khï¿½ng xï¿½c d?nh du?c tï¿½i kho?n dang thao tï¿½c!");
            }

            var hoSo = await GetHoSoWithCurrentStepAsync(hoSoVanBanId);
            if (hoSo == null)
            {
                return new CommonResponse("error", "Khï¿½ng tï¿½m th?y h? so c?n chuy?n phï¿½ duy?t.");
            }

            var daCoBanGhiDanhGia = await _dbContext.HoSoVanBanDanhGias
                .AsNoTracking()
                .AnyAsync(x => x.HoSoVanBanId == hoSoVanBanId);

            if (!daCoBanGhiDanhGia)
            {
                return new CommonResponse("error", "H? so chua cï¿½ b?n ghi xï¿½t duy?t nï¿½n chua th? chuy?n sang phï¿½ duy?t.");
            }

            var currentStep = await GetCurrentStepAsync(hoSo);
            if (currentStep == null)
            {
                return new CommonResponse("error", "Khï¿½ng xï¿½c d?nh du?c bu?c hi?n t?i c?a h? so.");
            }

            var currentProcessing = await _dbContext.HoSoVanBanXuLys
                .AsNoTracking()
                .Where(x => x.HoSoVanBanId == hoSo.Id && x.IsCurrent)
                .OrderByDescending(x => x.NgayNhan)
                .FirstOrDefaultAsync();

            if (!CanCurrentUserXuLy(currentUser, currentProcessing))
            {
                return new CommonResponse("error", "H? so nï¿½y khï¿½ng thu?c don v? dang dang nh?p d? chuy?n phï¿½ duy?t.");
            }

            var nextTransition = await ResolveTransitionByResultsAsync(
                hoSo.QuyTrinhSoanThaoId,
                currentStep.Id,
                "THAM_DINH_XONG",
                "CHUYEN_PHE_DUYET");

            var nextStep = nextTransition == null
                ? await _dbContext.DanhMucBuocQuyTrinhs
                    .AsNoTracking()
                    .Where(x => x.QuyTrinhSoanThaoId == hoSo.QuyTrinhSoanThaoId && x.ThuTuSapXep > currentStep.ThuTuSapXep)
                    .OrderBy(x => x.ThuTuSapXep)
                    .FirstOrDefaultAsync()
                : await _dbContext.DanhMucBuocQuyTrinhs
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == nextTransition.DenBuocId);

            if (nextStep == null)
            {
                return new CommonResponse("error", "Workflow chua c?u hï¿½nh bu?c k? ti?p cho nghi?p v? chuy?n phï¿½ duy?t.");
            }

            var nextStepDeadline = await ResolveNextStepDeadlineAsync(hoSo.Id, nextStep);

            return new CommonResponse("success", "Thï¿½nh cï¿½ng", new HoSoVanBanXuLyStepModel
            {
                HoSoVanBanId = hoSo.Id,
                TenHoSo = hoSo.TenHoSo,
                KetQuaXuLy = nextTransition?.DieuKienKetQua ?? "THAM_DINH_XONG",
                NgayXuLy = DateTime.Now,
                HanXuLy = nextStepDeadline,
                NoiDungXuLy = $"Chuy?n h? so {hoSo.TenHoSo} sang bu?c phï¿½ duy?t van b?n.",
                GhiChu = $"Chuy?n h? so {hoSo.TenHoSo} sang bu?c phï¿½ duy?t."
            });
        }

        public async Task<CommonResponse> GetChuyenBanHanhModelAsync(Guid hoSoVanBanId)
        {
            var currentUser = _authService.GetUserInfo();
            if (currentUser == null)
            {
                return new CommonResponse("error", "Khï¿½ng xï¿½c d?nh du?c tï¿½i kho?n dang thao tï¿½c!");
            }

            var hoSo = await GetHoSoWithCurrentStepAsync(hoSoVanBanId);
            if (hoSo == null)
            {
                return new CommonResponse("error", "Khï¿½ng tï¿½m th?y h? so c?n chuy?n ban hï¿½nh.");
            }

            var currentStep = await GetCurrentStepAsync(hoSo);
            if (currentStep == null)
            {
                return new CommonResponse("error", "Khï¿½ng xï¿½c d?nh du?c bu?c hi?n t?i c?a h? so.");
            }

            var currentProcessing = await _dbContext.HoSoVanBanXuLys
                .AsNoTracking()
                .Where(x => x.HoSoVanBanId == hoSo.Id && x.IsCurrent)
                .OrderByDescending(x => x.NgayNhan)
                .FirstOrDefaultAsync();

            if (!CanCurrentUserXuLy(currentUser, currentProcessing))
            {
                return new CommonResponse("error", "H? so nï¿½y khï¿½ng thu?c don v? dang dang nh?p d? chuy?n ban hï¿½nh.");
            }

            var nextTransition = await ResolveTransitionByResultsAsync(
                hoSo.QuyTrinhSoanThaoId,
                currentStep.Id,
                "TRINH_THANH_CONG",
                "CHUYEN_BAN_HANH");

            var nextStep = nextTransition == null
                ? await _dbContext.DanhMucBuocQuyTrinhs
                    .AsNoTracking()
                    .Where(x => x.QuyTrinhSoanThaoId == hoSo.QuyTrinhSoanThaoId && x.ThuTuSapXep > currentStep.ThuTuSapXep)
                    .OrderBy(x => x.ThuTuSapXep)
                    .FirstOrDefaultAsync()
                : await _dbContext.DanhMucBuocQuyTrinhs
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == nextTransition.DenBuocId);

            if (nextStep == null)
            {
                return new CommonResponse("error", "Workflow chua c?u hï¿½nh bu?c k? ti?p cho nghi?p v? chuy?n ban hï¿½nh.");
            }

            var nextStepDeadline = await ResolveNextStepDeadlineAsync(hoSo.Id, nextStep);

            return new CommonResponse("success", "Thï¿½nh cï¿½ng", new HoSoVanBanXuLyStepModel
            {
                HoSoVanBanId = hoSo.Id,
                TenHoSo = hoSo.TenHoSo,
                KetQuaXuLy = nextTransition?.DieuKienKetQua ?? "TRINH_THANH_CONG",
                NgayXuLy = DateTime.Now,
                HanXuLy = nextStepDeadline,
                NoiDungXuLy = $"Chuy?n h? so {hoSo.TenHoSo} sang bu?c ban hï¿½nh van b?n.",
                GhiChu = $"Chuy?n h? so {hoSo.TenHoSo} sang bu?c ban hï¿½nh."
            });
        }

        public async Task<CommonResponse> GetBanHanhFormAsync(Guid hoSoVanBanId)
        {
            var data = await (
                from hoSo in _dbContext.HoSoVanBans.AsNoTracking()
                join vanBan in _dbContext.DanhMucVanBans.AsNoTracking() on hoSo.DanhMucVanBanId equals vanBan.Id
                join donVi in _dbContext.DanhMucDonVis.AsNoTracking() on hoSo.DonViSoanThaoId equals donVi.Id
                join buoc in _dbContext.DanhMucBuocQuyTrinhs.AsNoTracking() on hoSo.BuocHienTaiId equals buoc.Id
                where hoSo.Id == hoSoVanBanId
                select new HoSoVanBanBanHanhFormModel
                {
                    HoSoVanBanId = hoSo.Id,
                    TenHoSo = hoSo.TenHoSo,
                    TenLoaiVanBan = vanBan.TenLoaiVanBan,
                    TenDonViSoanThao = donVi.TenDonVi,
                    TenBuocHienTai = buoc.TenBuoc,
                    AttachedFileGroupId = hoSo.AttachedFileGroupId ?? hoSo.Id,
                    LoaiVanBanBanHanh = hoSo.LoaiVanBanBanHanh,
                    SoKyHieuBanHanh = hoSo.SoKyHieuBanHanh,
                    TrichYeuBanHanh = hoSo.TrichYeuBanHanh,
                    CoQuanBanHanhId = hoSo.CoQuanBanHanhId,
                    NguoiKyId = hoSo.NguoiKyId,
                    HoTenNguoiKy = hoSo.HoTenNguoiKy,
                    ChucVuNguoiKy = hoSo.ChucVuNguoiKy,
                    NgayKy = hoSo.NgayKy,
                    NgayBanHanh = hoSo.NgayBanHanh,
                    NgayCoHieuLuc = hoSo.NgayCoHieuLuc,
                    NgayHetHieuLuc = hoSo.NgayHetHieuLuc,
                    TrangThaiBanHanh = hoSo.TrangThaiBanHanh,
                    VanBanPhapLuatId = hoSo.VanBanPhapLuatId,
                    NgayCongKhai = hoSo.NgayCongKhai,
                    DuongDanCongKhai = hoSo.DuongDanCongKhai,
                    TongThoiGianXayDungNgay = hoSo.TongThoiGianXayDungNgay,
                    TongThoiGianQuyDinhNgay = hoSo.TongThoiGianQuyDinhNgay,
                    TyLeThoiGianXayDung = hoSo.TyLeThoiGianXayDung,
                    DiemTienDoXayDung = hoSo.DiemTienDoXayDung,
                    DiemChatLuongVanBan = hoSo.DiemChatLuongVanBan,
                    TongDiemDanhGia = hoSo.TongDiemDanhGia,
                    XepLoaiDanhGia = hoSo.XepLoaiDanhGia,
                    NgayChamDiem = hoSo.NgayChamDiem,
                    GhiChu = hoSo.GhiChu
                }).FirstOrDefaultAsync();

            if (data == null)
            {
                return new CommonResponse("error", "Không tìm thấy hồ sơ ban hành.");
            }

            data.CoQuanBanHanhOptions = await GetDonViOptionsAsync();
            data.NguoiKyOptions = await _dbContext.Users.AsNoTracking()
                .Where(x => !data.CoQuanBanHanhId.HasValue || data.CoQuanBanHanhId == Guid.Empty || x.DanhMucDonViId == data.CoQuanBanHanhId.Value)
                .OrderBy(x => x.Name)
                .Select(x => new GuidTextOptionModel
                {
                    Id = x.Id,
                    Text = string.IsNullOrWhiteSpace(x.HoTenNguoiKy)
                        ? x.Name + (string.IsNullOrWhiteSpace(x.ChucDanhKy) ? string.Empty : $" - {x.ChucDanhKy}")
                        : x.HoTenNguoiKy + (string.IsNullOrWhiteSpace(x.ChucDanhKy) ? string.Empty : $" - {x.ChucDanhKy}")
                }).ToListAsync();

            data.QuyetDinhFileOptions = await _dbContext.AttachedFiles.AsNoTracking()
                .Where(x => x.GroupId == data.AttachedFileGroupId && x.TableName == "HoSoVanBan")
                .OrderByDescending(x => x.CreatedDate)
                .Select(x => new GuidTextOptionModel
                {
                    Id = x.Id,
                    Text = string.IsNullOrWhiteSpace(x.MoTa) ? (x.FileName ?? x.Id.ToString()) : $"{x.MoTa} - {x.FileName}"
                }).ToListAsync();

            var publicFile = data.VanBanPhapLuatId.HasValue
                ? await _dbContext.AttachedFiles.AsNoTracking().FirstOrDefaultAsync(x => x.Id == data.VanBanPhapLuatId.Value)
                : null;
            if (publicFile != null)
            {
                data.QuyetDinhBanHanhFileId = await _dbContext.AttachedFiles.AsNoTracking()
                    .Where(x => x.GroupId == data.AttachedFileGroupId && x.TableName == "HoSoVanBan" && x.FileName == publicFile.FileName)
                    .Select(x => (Guid?)x.Id)
                    .FirstOrDefaultAsync();
            }

            return new CommonResponse("success", "Thành công", data);
        }

        public async Task<CommonResponse> GetGiaHanXayDungFormAsync(Guid hoSoVanBanId)
        {
            var data = await (
                from hoSo in _dbContext.HoSoVanBans.AsNoTracking()
                join vanBan in _dbContext.DanhMucVanBans.AsNoTracking() on hoSo.DanhMucVanBanId equals vanBan.Id
                join donVi in _dbContext.DanhMucDonVis.AsNoTracking() on hoSo.DonViSoanThaoId equals donVi.Id
                join buoc in _dbContext.DanhMucBuocQuyTrinhs.AsNoTracking() on hoSo.BuocHienTaiId equals buoc.Id into buocJoin
                from buoc in buocJoin.DefaultIfEmpty()
                join xuLyCurrent in _dbContext.HoSoVanBanXuLys.AsNoTracking().Where(x => x.IsCurrent) on hoSo.Id equals xuLyCurrent.HoSoVanBanId into xuLyCurrentJoin
                from xuLyCurrent in xuLyCurrentJoin.DefaultIfEmpty()
                join donViXuLy in _dbContext.DanhMucDonVis.AsNoTracking() on xuLyCurrent.DonViXuLyId equals donViXuLy.Id into donViXuLyJoin
                from donViXuLy in donViXuLyJoin.DefaultIfEmpty()
                where hoSo.Id == hoSoVanBanId
                select new HoSoVanBanGiaHanFormModel
                {
                    HoSoVanBanId = hoSo.Id,
                    MaHoSo = hoSo.MaHoSo,
                    TenHoSo = hoSo.TenHoSo,
                    TenLoaiVanBan = vanBan.TenLoaiVanBan,
                    TenBuocHienTai = buoc != null ? buoc.TenBuoc : null,
                    TenDonViSoanThao = donVi.TenDonVi,
                    TenDonViXuLyHienTai = donViXuLy != null ? donViXuLy.TenDonVi : null,
                    HanXuLyHienTai = xuLyCurrent != null ? xuLyCurrent.HanXuLy : hoSo.HanXuLy
                }).FirstOrDefaultAsync();

            if (data == null)
            {
                return new CommonResponse("error", "Không tìm thấy hồ sơ cần gia hạn.");
            }

            if (!data.HanXuLyHienTai.HasValue)
            {
                return new CommonResponse("error", "Hồ sơ hiện chưa có hạn xử lý để gia hạn.");
            }

            data.HanXuLyMoi = data.HanXuLyHienTai.Value.Date.AddDays(1);
            data.SoNgayGiaHan = 1;
            data.LichSus = await (
                from row in _dbContext.HoSoVanBanGiaHans.AsNoTracking()
                join user in _dbContext.Users.AsNoTracking() on row.NguoiGiaHanId equals user.Id into userJoin
                from user in userJoin.DefaultIfEmpty()
                where row.HoSoVanBanId == hoSoVanBanId
                orderby row.CreatedDate descending
                select new HoSoVanBanGiaHanHistoryItemModel
                {
                    Id = row.Id,
                    HanXuLyCu = row.HanXuLyCu,
                    HanXuLyMoi = row.HanXuLyMoi,
                    SoNgayGiaHan = row.SoNgayGiaHan,
                    LyDoGiaHan = row.LyDoGiaHan,
                    TenNguoiGiaHan = user != null ? user.Name : null,
                    CreatedDate = row.CreatedDate,
                    AttachedFileGroupId = row.AttachedFileGroupId,
                    GhiChu = row.GhiChu
                }).ToListAsync();

            return new CommonResponse("success", "Thành công", data);
        }

        public async Task<CommonResponse> SaveGiaHanXayDungAsync(HoSoVanBanGiaHanFormModel request)
        {
            var currentUser = _authService.GetUserInfo();
            if (currentUser == null)
            {
                return new CommonResponse("error", "Không xác định được tài khoản đang thao tác.");
            }

            if (!request.HanXuLyHienTai.HasValue)
            {
                return new CommonResponse("error", "Không xác định được hạn xử lý hiện tại.");
            }

            if (request.HanXuLyMoi.Date <= request.HanXuLyHienTai.Value.Date)
            {
                return new CommonResponse("error", "Hạn xử lý mới phải lớn hơn hạn xử lý hiện tại.");
            }

            if (string.IsNullOrWhiteSpace(request.LyDoGiaHan))
            {
                return new CommonResponse("error", "Bạn phải nhập lý do gia hạn.");
            }

            var hoSo = await _dbContext.HoSoVanBans.FirstOrDefaultAsync(x => x.Id == request.HoSoVanBanId);
            if (hoSo == null)
            {
                return new CommonResponse("error", "Không tìm thấy hồ sơ cần gia hạn.");
            }

            var currentProcessing = await _dbContext.HoSoVanBanXuLys
                .FirstOrDefaultAsync(x => x.HoSoVanBanId == request.HoSoVanBanId && x.IsCurrent);
            var currentStep = await GetCurrentStepAsync(hoSo);

            var soNgayGiaHan = (request.HanXuLyMoi.Date - request.HanXuLyHienTai.Value.Date).Days;
            if (soNgayGiaHan <= 0)
            {
                return new CommonResponse("error", "Số ngày gia hạn phải lớn hơn 0.");
            }

            var currentStepPlan = hoSo.BuocHienTaiId.HasValue
                ? await _dbContext.HoSoVanBanBuocThoiHans.FirstOrDefaultAsync(x => x.HoSoVanBanId == request.HoSoVanBanId && x.BuocQuyTrinhId == hoSo.BuocHienTaiId.Value)
                : null;

            var entity = new HoSoVanBanGiaHan
            {
                HoSoVanBanId = request.HoSoVanBanId,
                BuocQuyTrinhId = hoSo.BuocHienTaiId,
                NguoiGiaHanId = currentUser.Id,
                HanXuLyCu = request.HanXuLyHienTai.Value.Date,
                HanXuLyMoi = request.HanXuLyMoi.Date,
                SoNgayGiaHan = soNgayGiaHan,
                LyDoGiaHan = request.LyDoGiaHan?.Trim(),
                AttachedFileGroupId = request.AttachedFileGroupId,
                GhiChu = request.GhiChu?.Trim()
            };

            hoSo.HanXuLy = request.HanXuLyMoi.Date;
            if (currentProcessing != null)
            {
                currentProcessing.HanXuLy = request.HanXuLyMoi.Date;
            }

            if (currentStepPlan != null)
            {
                currentStepPlan.SoNgayXuLy = (currentStepPlan.SoNgayXuLy ?? 0) + soNgayGiaHan;
            }

            _dbContext.HoSoVanBanGiaHans.Add(entity);
            await _dbContext.SaveChangesAsync();

            if (currentUser.DanhMucDonViId != Guid.Empty &&
                currentUser.DanhMucDonViId != SoTuPhapDonViId &&
                currentStep != null)
            {
                var noiDungThongBao = new StringBuilder()
                    .Append($"[Gia hạn tiến độ] Hồ sơ {hoSo.MaHoSo} - {hoSo.TenHoSo} đã được gia hạn tại bước {currentStep.TenBuoc}. ")
                    .Append($"Hạn cũ: {request.HanXuLyHienTai.Value:dd/MM/yyyy}; hạn mới: {request.HanXuLyMoi:dd/MM/yyyy}; thêm {soNgayGiaHan} ngày.")
                    .Append(string.IsNullOrWhiteSpace(request.LyDoGiaHan) ? string.Empty : $" Lý do: {request.LyDoGiaHan.Trim()}.")
                    .ToString();

                await TaoThongBaoAsync(
                    hoSo,
                    currentStep,
                    currentUser.DanhMucDonViId,
                    SoTuPhapDonViId,
                    noiDungThongBao);
            }

            return new CommonResponse("success", "Đã gia hạn thời gian xây dựng văn bản thành công.");
        }

        public async Task<CommonResponse> GetChamDiemXayDungFormAsync(Guid hoSoVanBanId)
        {
            var hoSo = await (
                from x in _dbContext.HoSoVanBans.AsNoTracking()
                join vanBan in _dbContext.DanhMucVanBans.AsNoTracking() on x.DanhMucVanBanId equals vanBan.Id
                join donVi in _dbContext.DanhMucDonVis.AsNoTracking() on x.DonViSoanThaoId equals donVi.Id
                where x.Id == hoSoVanBanId
                select new HoSoVanBanChamDiemFormModel
                {
                    HoSoVanBanId = x.Id,
                    MaHoSo = x.MaHoSo,
                    TenHoSo = x.TenHoSo,
                    TenLoaiVanBan = vanBan.TenLoaiVanBan,
                    TenDonViSoanThao = donVi.TenDonVi,
                    NgayTaoHoSo = x.NgayTaoHoSo,
                    NgayBanHanh = x.NgayBanHanh,
                    SoLanTraLaiHienTai = x.SoLanTraLaiHienTai,
                    TongThoiGianXayDungNgay = x.TongThoiGianXayDungNgay,
                    TongThoiGianQuyDinhNgay = x.TongThoiGianQuyDinhNgay,
                    TyLeThoiGianXayDung = x.TyLeThoiGianXayDung
                }).FirstOrDefaultAsync();

            if (hoSo == null)
            {
                return new CommonResponse("error", "Không tìm thấy hồ sơ chấm điểm.");
            }

            if (!hoSo.NgayBanHanh.HasValue)
            {
                return new CommonResponse("error", "Chỉ chấm điểm cho hồ sơ đã ban hành.");
            }

            var existing = await _dbContext.HoSoVanBanChamDiems.AsNoTracking()
                .FirstOrDefaultAsync(x => x.HoSoVanBanId == hoSoVanBanId);

            if (existing != null)
            {
                hoSo.Id = existing.Id;
                hoSo.NgayChamDiem = existing.NgayChamDiem;
                hoSo.TrangThai = existing.TrangThai;
                hoSo.TongDiem = existing.TongDiem;
                hoSo.XepLoai = existing.XepLoai;
                hoSo.GhiChu = existing.GhiChu;
                hoSo.DaCoBanGhiChamDiem = true;
                hoSo.ChiTiets = await _dbContext.HoSoVanBanChamDiemChiTiets.AsNoTracking()
                    .Where(x => x.HoSoVanBanChamDiemId == existing.Id)
                    .OrderBy(x => x.CreatedDate)
                    .Select(x => new HoSoVanBanChamDiemChiTietFormModel
                    {
                        Id = x.Id,
                        DanhMucTieuChiDiemId = x.DanhMucTieuChiDiemId,
                        MaTieuChi = x.MaTieuChi,
                        TenTieuChi = x.TenTieuChi,
                        LoaiTieuChi = x.LoaiTieuChi,
                        DiemToiDa = x.DiemToiDa,
                        GiaTriTinhDiem = x.GiaTriTinhDiem,
                        DienGiaiGiaTri = x.DienGiaiGiaTri,
                        DiemDeXuat = x.DiemDeXuat,
                        DiemChinhThuc = x.DiemChinhThuc,
                        GhiChu = x.GhiChu
                    }).ToListAsync();

                return new CommonResponse("success", "Thành công", hoSo);
            }

            var proposal = await BuildChamDiemProposalAsync(hoSoVanBanId);
            if (proposal == null)
            {
                return new CommonResponse("error", "Chưa cấu hình danh mục tiêu chí chấm điểm.");
            }

            hoSo.TongThoiGianXayDungNgay = proposal.TongThoiGianXayDungNgay;
            hoSo.TongThoiGianQuyDinhNgay = proposal.TongThoiGianQuyDinhNgay;
            hoSo.TyLeThoiGianXayDung = proposal.TyLeThoiGianXayDung;
            hoSo.NgayChamDiem = DateTime.Today;
            hoSo.TrangThai = "NHAP";
            hoSo.TongDiem = proposal.TongDiem;
            hoSo.XepLoai = ResolveXepLoaiTongDiem(proposal.TongDiem);
            hoSo.ChiTiets = proposal.ChiTiets.Select(x => new HoSoVanBanChamDiemChiTietFormModel
            {
                DanhMucTieuChiDiemId = x.DanhMucTieuChiDiemId,
                MaTieuChi = x.MaTieuChi,
                TenTieuChi = x.TenTieuChi,
                LoaiTieuChi = x.LoaiTieuChi,
                DiemToiDa = x.DiemToiDa,
                GiaTriTinhDiem = x.GiaTriTinhDiem,
                DienGiaiGiaTri = x.DienGiaiGiaTri,
                DiemDeXuat = x.DiemDeXuat,
                DiemChinhThuc = x.DiemDeXuat ?? 0
            }).ToList();

            return new CommonResponse("success", "Thành công", hoSo);
        }

        public async Task<CommonResponse> SaveBanHanhAsync(HoSoVanBanBanHanhFormModel request, bool xacNhanBanHanh)
        {
            var currentUser = _authService.GetUserInfo();
            if (currentUser == null)
            {
                return new CommonResponse("error", "Không xác định được tài khoản đang thao tác.");
            }

            var hoSo = await GetHoSoWithCurrentStepAsync(request.HoSoVanBanId);
            var currentStep = hoSo == null ? null : await GetCurrentStepAsync(hoSo);
            if (hoSo == null || currentStep == null || (currentStep.MaBuoc != "BUOC_06_THONG_QUA_BAN_HANH" && currentStep.MaBuoc != "BUOC_07_THONG_QUA_BAN_HANH"))
            {
                return new CommonResponse("error", "Hồ sơ hiện không ở bước ban hành.");
            }

            var currentProcessing = await _dbContext.HoSoVanBanXuLys
                .Where(x => x.HoSoVanBanId == hoSo.Id && x.IsCurrent)
                .OrderByDescending(x => x.NgayNhan)
                .FirstOrDefaultAsync();
            if (!CanCurrentUserXuLy(currentUser, currentProcessing))
            {
                return new CommonResponse("error", "Bạn không có quyền cập nhật hồ sơ này.");
            }

            if (xacNhanBanHanh)
            {
                if (string.IsNullOrWhiteSpace(request.SoKyHieuBanHanh) || !request.NgayBanHanh.HasValue || !request.NgayKy.HasValue || !request.QuyetDinhBanHanhFileId.HasValue)
                {
                    return new CommonResponse("error", "Khi xác nhận ban hành phải có số ký hiệu, ngày ký, ngày ban hành và file quyết định.");
                }
            }

            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                hoSo.AttachedFileGroupId ??= request.AttachedFileGroupId != Guid.Empty ? request.AttachedFileGroupId : hoSo.Id;
                hoSo.LoaiVanBanBanHanh = request.LoaiVanBanBanHanh?.Trim();
                hoSo.SoKyHieuBanHanh = request.SoKyHieuBanHanh?.Trim();
                hoSo.TrichYeuBanHanh = request.TrichYeuBanHanh?.Trim();
                hoSo.CoQuanBanHanhId = request.CoQuanBanHanhId;
                hoSo.NguoiKyId = request.NguoiKyId;
                hoSo.HoTenNguoiKy = request.HoTenNguoiKy?.Trim();
                hoSo.ChucVuNguoiKy = request.ChucVuNguoiKy?.Trim();
                hoSo.NgayKy = request.NgayKy;
                hoSo.NgayBanHanh = request.NgayBanHanh;
                hoSo.NgayCoHieuLuc = request.NgayCoHieuLuc;
                hoSo.NgayHetHieuLuc = request.NgayHetHieuLuc;
                hoSo.NgayCongKhai = request.NgayCongKhai;
                hoSo.DuongDanCongKhai = request.DuongDanCongKhai?.Trim();
                hoSo.GhiChu = request.GhiChu?.Trim();
                hoSo.TrangThaiBanHanh = xacNhanBanHanh ? "DA_BAN_HANH" : "CHUA_BAN_HANH";

                var signer = request.NguoiKyId.HasValue
                    ? await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.NguoiKyId.Value)
                    : null;
                if (signer != null)
                {
                    hoSo.HoTenNguoiKy = string.IsNullOrWhiteSpace(request.HoTenNguoiKy) ? (signer.HoTenNguoiKy ?? signer.Name) : request.HoTenNguoiKy.Trim();
                    hoSo.ChucVuNguoiKy = string.IsNullOrWhiteSpace(request.ChucVuNguoiKy) ? signer.ChucDanhKy : request.ChucVuNguoiKy.Trim();
                }

                if (xacNhanBanHanh)
                {
                    var hoSoFiles = await _dbContext.AttachedFiles
                        .Where(x => x.GroupId == hoSo.AttachedFileGroupId && x.TableName == "HoSoVanBan")
                        .ToListAsync();
                    var qdFile = hoSoFiles.FirstOrDefault(x => x.Id == request.QuyetDinhBanHanhFileId.Value);
                    if (qdFile == null)
                    {
                        return new CommonResponse("error", "Không tìm thấy file quyết định đã chọn.");
                    }

                    foreach (var file in hoSoFiles)
                    {
                        file.Public = false;
                        file.PhamViCongKhai = file.Id == qdFile.Id ? "CONG_KHAI" : "NOI_BO";
                        file.LoaiTaiLieu = file.Id == qdFile.Id ? "QUYET_DINH_BAN_HANH" : (file.LoaiTaiLieu == "QUYET_DINH_BAN_HANH" ? "TAI_LIEU_HO_SO" : file.LoaiTaiLieu);
                    }

                    AttachedFile publicFile;
                    if (hoSo.VanBanPhapLuatId.HasValue)
                    {
                        publicFile = await _dbContext.AttachedFiles.FirstOrDefaultAsync(x => x.Id == hoSo.VanBanPhapLuatId.Value)
                            ?? new AttachedFile { TableName = "VanBanPhapLuat" };
                    }
                    else
                    {
                        publicFile = new AttachedFile { TableName = "VanBanPhapLuat" };
                    }

                    publicFile.SoVanBan = hoSo.SoKyHieuBanHanh ?? request.SoKyHieuBanHanh;
                    publicFile.NgayBanHanh = hoSo.NgayBanHanh ?? request.NgayBanHanh ?? DateTime.Today;
                    publicFile.NgayApDung = hoSo.NgayCoHieuLuc ?? hoSo.NgayBanHanh ?? request.NgayBanHanh ?? DateTime.Today;
                    publicFile.MoTa = hoSo.TrichYeuBanHanh ?? request.TrichYeuBanHanh ?? hoSo.TenHoSo;
                    publicFile.Url = hoSo.DuongDanCongKhai ?? request.DuongDanCongKhai;
                    publicFile.FileName = qdFile.FileName;
                    publicFile.ContentType = qdFile.ContentType;
                    publicFile.FileContent = qdFile.FileContent;
                    publicFile.Public = true;
                    publicFile.PhamViCongKhai = "CONG_KHAI";
                    publicFile.LoaiTaiLieu = "QUYET_DINH_BAN_HANH";
                    publicFile.Status = "XD";
                    publicFile.DonViId = hoSo.CoQuanBanHanhId ?? currentUser.DanhMucDonViId;

                    if (hoSo.VanBanPhapLuatId.HasValue && publicFile.Id != Guid.Empty)
                    {
                        _dbContext.AttachedFiles.Update(publicFile);
                    }
                    else
                    {
                        _dbContext.AttachedFiles.Add(publicFile);
                    }

                    await _dbContext.SaveChangesAsync();
                    hoSo.VanBanPhapLuatId = publicFile.Id;

                    currentProcessing!.IsCurrent = false;
                    currentProcessing.NgayXuLy = DateTime.Now;
                    currentProcessing.KetQuaXuLy = "BAN_HANH_XONG";
                    currentProcessing.NoiDungXuLy = hoSo.TrichYeuBanHanh ?? request.TrichYeuBanHanh;
                    await AdvanceWorkflowAsync(hoSo, currentUser, null, null, "BAN_HANH_XONG", currentProcessing.NoiDungXuLy);
                    await ApplyTieuChiDiemAsync(hoSo);
                }

                _dbContext.HoSoVanBans.Update(hoSo);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return new CommonResponse("success", xacNhanBanHanh ? "Đã cập nhật và xác nhận ban hành văn bản." : "Đã lưu nháp thông tin ban hành.");
            }
            catch
            {
                await transaction.RollbackAsync();
                return new CommonResponse("error", "Không thể cập nhật thông tin ban hành.");
            }
        }

        public async Task<CommonResponse> SaveChamDiemXayDungAsync(HoSoVanBanChamDiemFormModel request)
        {
            var currentUser = _authService.GetUserInfo();
            if (currentUser == null)
            {
                return new CommonResponse("error", "Không xác định được tài khoản đang thao tác.");
            }

            var hoSo = await _dbContext.HoSoVanBans.FirstOrDefaultAsync(x => x.Id == request.HoSoVanBanId);
            if (hoSo == null || hoSo.TrangThaiBanHanh != "DA_BAN_HANH")
            {
                return new CommonResponse("error", "Chỉ chấm điểm cho hồ sơ đã ban hành.");
            }

            var chiTiets = request.ChiTiets ?? new List<HoSoVanBanChamDiemChiTietFormModel>();
            if (chiTiets.Count == 0)
            {
                return new CommonResponse("error", "Bản ghi chấm điểm phải có ít nhất một tiêu chí.");
            }

            foreach (var item in chiTiets)
            {
                if (item.DiemChinhThuc < 0)
                {
                    return new CommonResponse("error", $"Điểm của tiêu chí {item.TenTieuChi} không được âm.");
                }
                if (item.DiemChinhThuc > item.DiemToiDa)
                {
                    return new CommonResponse("error", $"Điểm của tiêu chí {item.TenTieuChi} không được vượt quá điểm tối đa.");
                }
            }

            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var entity = await _dbContext.HoSoVanBanChamDiems
                    .FirstOrDefaultAsync(x => x.HoSoVanBanId == request.HoSoVanBanId);

                if (entity == null)
                {
                    entity = new HoSoVanBanChamDiem
                    {
                        HoSoVanBanId = request.HoSoVanBanId
                    };
                    _dbContext.HoSoVanBanChamDiems.Add(entity);
                    await _dbContext.SaveChangesAsync();
                }
                else
                {
                    var oldDetails = await _dbContext.HoSoVanBanChamDiemChiTiets
                        .Where(x => x.HoSoVanBanChamDiemId == entity.Id)
                        .ToListAsync();
                    if (oldDetails.Count > 0)
                    {
                        _dbContext.HoSoVanBanChamDiemChiTiets.RemoveRange(oldDetails);
                    }
                }

                entity.NguoiChamDiemId = currentUser.Id;
                entity.NgayChamDiem = request.NgayChamDiem;
                entity.TrangThai = string.IsNullOrWhiteSpace(request.TrangThai) ? "NHAP" : request.TrangThai.Trim().ToUpperInvariant();
                entity.GhiChu = string.IsNullOrWhiteSpace(request.GhiChu) ? null : request.GhiChu.Trim();
                entity.TongDiem = chiTiets.Sum(x => x.DiemChinhThuc);
                entity.XepLoai = ResolveXepLoaiTongDiem(entity.TongDiem);

                var detailEntities = chiTiets.Select(x => new HoSoVanBanChamDiemChiTiet
                {
                    HoSoVanBanChamDiemId = entity.Id,
                    DanhMucTieuChiDiemId = x.DanhMucTieuChiDiemId,
                    MaTieuChi = x.MaTieuChi,
                    TenTieuChi = x.TenTieuChi,
                    LoaiTieuChi = x.LoaiTieuChi,
                    GiaTriTinhDiem = x.GiaTriTinhDiem,
                    DiemDeXuat = x.DiemDeXuat,
                    DiemChinhThuc = x.DiemChinhThuc,
                    DiemToiDa = x.DiemToiDa,
                    DienGiaiGiaTri = x.DienGiaiGiaTri,
                    GhiChu = string.IsNullOrWhiteSpace(x.GhiChu) ? null : x.GhiChu.Trim()
                }).ToList();

                _dbContext.HoSoVanBanChamDiemChiTiets.AddRange(detailEntities);

                hoSo.TongThoiGianXayDungNgay = request.TongThoiGianXayDungNgay;
                hoSo.TongThoiGianQuyDinhNgay = request.TongThoiGianQuyDinhNgay;
                hoSo.TyLeThoiGianXayDung = request.TyLeThoiGianXayDung;
                hoSo.DiemTienDoXayDung = chiTiets.FirstOrDefault(x => x.LoaiTieuChi == "THOI_GIAN")?.DiemChinhThuc;
                hoSo.DiemChatLuongVanBan = chiTiets.FirstOrDefault(x => x.LoaiTieuChi == "CHAT_LUONG")?.DiemChinhThuc;
                hoSo.TongDiemDanhGia = entity.TongDiem;
                hoSo.XepLoaiDanhGia = entity.XepLoai;
                hoSo.NgayChamDiem = entity.NgayChamDiem;

                _dbContext.HoSoVanBans.Update(hoSo);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return new CommonResponse("success", "Đã lưu bản ghi chấm điểm.");
            }
            catch
            {
                await transaction.RollbackAsync();
                return new CommonResponse("error", "Không thể lưu bản ghi chấm điểm.");
            }
        }

        public async Task<CommonResponse> GetTaoHoSoSoanThaoTuDangKyModelAsync(Guid hoSoDangKyId)
        {
            var currentUser = _authService.GetUserInfo();
            if (currentUser == null)
            {
                return new CommonResponse("error", "Khong xac dinh duoc tai khoan dang thao tac!");
            }

            var hoSoDangKy = await GetHoSoWithCurrentStepAsync(hoSoDangKyId);
            if (hoSoDangKy == null)
            {
                return new CommonResponse("error", "Khong tim thay ho so dang ky!");
            }

            var latestReviewProcessing = await _dbContext.HoSoVanBanXuLys
                .Where(x => x.HoSoVanBanId == hoSoDangKy.Id)
                .Join(
                    _dbContext.DanhMucBuocQuyTrinhs.AsNoTracking().Where(x => x.MaBuoc == "BUOC_02_THONG_NHAT"),
                    xuLy => xuLy.BuocQuyTrinhId,
                    buoc => buoc.Id,
                    (xuLy, _) => xuLy)
                .OrderByDescending(x => x.NgayXuLy ?? x.NgayNhan)
                .ThenByDescending(x => x.CreatedDate)
                .FirstOrDefaultAsync();

            if (latestReviewProcessing == null || !string.Equals(latestReviewProcessing.KetQuaXuLy, "DONG_Y", StringComparison.OrdinalIgnoreCase))
            {
                return new CommonResponse("error", "Ho so nay chua duoc xet duyet dong y de tao ho so soan thao.");
            }

            if (!CanDangKyReviewUserAccess(currentUser, hoSoDangKy, latestReviewProcessing))
            {
                return new CommonResponse("error", "Ban khong co quyen tao ho so soan thao tu ho so dang ky nay.");
            }

            var existingDraftHoSo = await FindExistingDraftHoSoByDangKyIdAsync(hoSoDangKy.Id);
            if (existingDraftHoSo != null)
            {
                return new CommonResponse("error", "Ho so soan thao da duoc tao truoc do.");
            }

            var vanBan = await _dbContext.DanhMucVanBans
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == hoSoDangKy.DanhMucVanBanId);

            var defaultWorkflow = await ResolveDraftWorkflowAsync(hoSoDangKy.DanhMucVanBanId);

            if (defaultWorkflow == null)
            {
                return new CommonResponse("error", "Khong tim thay workflow ho so soan thao rieng cho chuc nang nay.");
            }

            var model = new HoSoVanBanTaoSoanThaoTuDangKyModel
            {
                HoSoDangKyId = hoSoDangKy.Id,
                TenHoSoDangKy = hoSoDangKy.TenHoSo,
                DanhMucVanBanId = hoSoDangKy.DanhMucVanBanId,
                TenLoaiVanBan = vanBan?.TenLoaiVanBan,
                DonViSoanThaoId = hoSoDangKy.DonViSoanThaoId,
                QuyTrinhSoanThaoId = defaultWorkflow.Id,
                TuNgaySoanThao = DateTime.Today,
                DenNgaySoanThao = DateTime.Today.AddDays(7),
                GhiChu = hoSoDangKy.GhiChu,
                BuocThoiHans = await GetBuocThoiHanOptionsAsync(defaultWorkflow.Id)
            };

            return new CommonResponse("success", "Thanh cong", model);
        }

        public async Task<CommonResponse> UpdateHoSoAsync(HoSoVanBanCreateModel request)
        {
            var currentUser = _authService.GetUserInfo();
            if (currentUser == null)
            {
                return new CommonResponse("error", "KhĂ´ng xĂ¡c Ä‘á»‹nh Ä‘Æ°á»£c tĂ i khoáº£n Ä‘ang thao tĂ¡c!");
            }

            if (!request.TuNgaySoanThao.HasValue || !request.DenNgaySoanThao.HasValue)
            {
                return new CommonResponse("error", "Thá»i gian soáº¡n tháº£o báº¯t buá»™c pháº£i nháº­p.");
            }

            if (request.DenNgaySoanThao.Value.Date < request.TuNgaySoanThao.Value.Date)
            {
                return new CommonResponse("error", "Äáº¿n ngĂ y soáº¡n tháº£o pháº£i lá»›n hÆ¡n hoáº·c báº±ng tá»« ngĂ y soáº¡n tháº£o.");
            }

            if (request.Id == Guid.Empty)
            {
                return new CommonResponse("error", "Thiáº¿u thĂ´ng tin há»“ sÆ¡ cáº§n cáº­p nháº­t!");
            }

            if (string.IsNullOrWhiteSpace(request.TenHoSo))
            {
                return new CommonResponse("error", "TĂªn há»“ sÆ¡ khĂ´ng Ä‘Æ°á»£c Ä‘á»ƒ trá»‘ng!");
            }

            var hoSo = await _dbContext.HoSoVanBans.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (hoSo == null)
            {
                return new CommonResponse("error", "KhĂ´ng tĂ¬m tháº¥y há»“ sÆ¡ vÄƒn báº£n!");
            }

            var currentStep = await GetCurrentStepAsync(hoSo);
            var currentProcessing = await _dbContext.HoSoVanBanXuLys
                .FirstOrDefaultAsync(x => x.HoSoVanBanId == hoSo.Id && x.IsCurrent);

            if (!CanEditDangKyHoSo(currentUser, hoSo, currentStep, currentProcessing))
            {
                return new CommonResponse("error", "Há»“ sÆ¡ nĂ y Ä‘Ă£ Ä‘Æ°á»£c gá»­i Ä‘i hoáº·c báº¡n khĂ´ng cĂ³ quyá»n cáº­p nháº­t.");
            }

            request.BuocThoiHans = request.BuocThoiHans
                .Where(x => x.BuocQuyTrinhId != Guid.Empty)
                .OrderBy(x => x.ThuTuSapXep)
                .ToList();

            var quyTrinh = await _dbContext.DanhMucQuyTrinhSoanThaos
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.QuyTrinhSoanThaoId && x.TrangThai);

            if (quyTrinh == null)
            {
                return new CommonResponse("error", "KhĂ´ng tĂ¬m tháº¥y quy trĂ¬nh soáº¡n tháº£o Ä‘ang kĂ­ch hoáº¡t!");
            }

            var firstStep = await _dbContext.DanhMucBuocQuyTrinhs
                .AsNoTracking()
                .Where(x => x.QuyTrinhSoanThaoId == request.QuyTrinhSoanThaoId)
                .OrderBy(x => x.ThuTuSapXep)
                .ThenBy(x => x.MaBuoc)
                .FirstOrDefaultAsync();

            if (firstStep == null)
            {
                return new CommonResponse("error", "Quy trĂ¬nh chÆ°a cĂ³ bÆ°á»›c nĂ o Ä‘á»ƒ cáº­p nháº­t há»“ sÆ¡!");
            }

            var stepDeadlinePlans = await BuildRequestedStepDeadlinePlansAsync(request.QuyTrinhSoanThaoId, request.BuocThoiHans);
            var firstStepPlan = stepDeadlinePlans.FirstOrDefault(x => x.BuocQuyTrinhId == firstStep.Id);
            var effectiveDonViSoanThaoId = request.DonViDeNghiId.HasValue && request.DonViDeNghiId.Value != Guid.Empty
                ? request.DonViDeNghiId.Value
                : hoSo.DonViSoanThaoId;
            request.HanXuLy = request.DenNgaySoanThao.Value.Date;
            var hanXuLyBuocDau = request.HanXuLy ?? CalculateStepDeadline(firstStepPlan?.SoNgayXuLy ?? firstStep.SoNgayXuLyTieuChuan, DateTime.Now);

            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                hoSo.TenHoSo = request.TenHoSo.Trim();
                hoSo.DanhMucVanBanId = request.DanhMucVanBanId;
                hoSo.QuyTrinhSoanThaoId = request.QuyTrinhSoanThaoId;
                hoSo.DonViSoanThaoId = effectiveDonViSoanThaoId;
                hoSo.BuocHienTaiId = firstStep.Id;
                hoSo.HanXuLy = hanXuLyBuocDau;
                hoSo.AttachedFileGroupId = request.Id;
                hoSo.MoTa = request.MoTa?.Trim();
                hoSo.GhiChu = request.GhiChu?.Trim();

                if (currentProcessing != null)
                {
                    currentProcessing.BuocQuyTrinhId = firstStep.Id;
                    currentProcessing.DonViXuLyId = effectiveDonViSoanThaoId;
                    currentProcessing.NguoiXuLyId = currentUser.SSA || currentUser.DanhMucDonViId == effectiveDonViSoanThaoId
                        ? currentUser.Id
                        : null;
                    currentProcessing.HanXuLy = hanXuLyBuocDau;
                    currentProcessing.GhiChu = request.GhiChu?.Trim();
                    currentProcessing.NoiDungXuLy = "Cáº­p nháº­t há»“ sÆ¡ Ä‘Äƒng kĂ½ trÆ°á»›c khi chuyá»ƒn bÆ°á»›c tiáº¿p theo.";
                }

                var oldStepPlans = await _dbContext.HoSoVanBanBuocThoiHans
                    .Where(x => x.HoSoVanBanId == hoSo.Id)
                    .ToListAsync();

                if (oldStepPlans.Count > 0)
                {
                    _dbContext.HoSoVanBanBuocThoiHans.RemoveRange(oldStepPlans);
                }

                var newStepPlans = stepDeadlinePlans.Select(x => new HoSoVanBanBuocThoiHan
                {
                    HoSoVanBanId = hoSo.Id,
                    BuocQuyTrinhId = x.BuocQuyTrinhId,
                    ThuTuSapXep = x.ThuTuSapXep,
                    SoNgayXuLy = x.SoNgayXuLy,
                    SoNgayCanhBaoSapHan = x.SoNgayCanhBaoSapHan,
                    GhiChu = x.GhiChu
                }).ToList();

                if (newStepPlans.Count > 0)
                {
                    _dbContext.HoSoVanBanBuocThoiHans.AddRange(newStepPlans);
                }

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return new CommonResponse("success", "Cáº­p nháº­t há»“ sÆ¡ thĂ nh cĂ´ng", hoSo.Id);
            }
            catch
            {
                await transaction.RollbackAsync();
                return new CommonResponse();
            }
        }

        public async Task<CommonResponse> HoanThanhXuLyAsync(HoSoVanBanXuLyStepModel request)
        {
            var currentUser = _authService.GetUserInfo();
            if (currentUser == null)
            {
                return new CommonResponse("error", "KhĂ´ng xĂ¡c Ä‘á»‹nh Ä‘Æ°á»£c tĂ i khoáº£n Ä‘ang thao tĂ¡c!");
            }

            var hoSo = await GetHoSoWithCurrentStepAsync(request.HoSoVanBanId);
            if (hoSo == null)
            {
                return new CommonResponse("error", "KhĂ´ng tĂ¬m tháº¥y há»“ sÆ¡ vÄƒn báº£n!");
            }

            var currentStep = await GetCurrentStepAsync(hoSo);
            if (currentStep == null)
            {
                return new CommonResponse("error", "Ho so chua xac dinh duoc buoc hien tai!");
            }

            if (currentStep.LoaiBuoc == "LayYKien" || currentStep.LoaiBuoc == "DanhGia")
            {
                return new CommonResponse("error", "BÆ°á»›c hiá»‡n táº¡i lĂ  bÆ°á»›c Ä‘áº·c thĂ¹. HĂ£y dĂ¹ng nghiá»‡p vá»¥ Láº¥y Ă½ kiáº¿n hoáº·c ÄĂ¡nh giĂ¡.");
            }

            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var currentProcessing = await _dbContext.HoSoVanBanXuLys
                    .Where(x => x.HoSoVanBanId == hoSo.Id && x.IsCurrent)
                    .OrderByDescending(x => x.NgayNhan)
                    .FirstOrDefaultAsync();

                if (!CanCurrentUserXuLy(currentUser, currentProcessing))
                {
                    return new CommonResponse("error", "Ho so nay da duoc chuyen sang don vi khac. Ban khong the cap nhat nua!");
                }

                var isSoanThaoStep = string.Equals(currentStep.LoaiBuoc, "SoanThao", StringComparison.OrdinalIgnoreCase);
                if (currentProcessing != null && !currentUser.SSA && !currentProcessing.NguoiXuLyId.HasValue && !isSoanThaoStep)
                {
                    return new CommonResponse("error", "Há»“ sÆ¡ chÆ°a Ä‘Æ°á»£c nháº­n. Vui lĂ²ng báº¥m 'Nháº­n há»“ sÆ¡' trÆ°á»›c khi xá»­ lĂ½.");
                }

                if (currentProcessing != null)
                {
                    if (!currentProcessing.NguoiXuLyId.HasValue && isSoanThaoStep)
                    {
                        currentProcessing.NguoiXuLyId = currentUser.Id;
                        currentProcessing.NgayNhan = DateTime.Now;
                    }

                    currentProcessing.IsCurrent = false;
                    currentProcessing.NgayXuLy = request.NgayXuLy ?? DateTime.Now;
                    currentProcessing.KetQuaXuLy = request.KetQuaXuLy.Trim();
                    currentProcessing.NoiDungXuLy = request.NoiDungXuLy?.Trim();
                    currentProcessing.GhiChu = BuildXuLyGhiChu(request.GhiChu, request.AttachedFileGroupId);
                    if (request.DanhMucTrangThaiId.HasValue)
                    {
                        currentProcessing.DanhMucTrangThaiId = request.DanhMucTrangThaiId;
                    }
                }

                var nextTransition = await GetTransitionAsync(hoSo.QuyTrinhSoanThaoId, currentStep.Id, request.KetQuaXuLy.Trim());
                var nextStep = nextTransition == null
                    ? null
                    : await _dbContext.DanhMucBuocQuyTrinhs.AsNoTracking().FirstOrDefaultAsync(x => x.Id == nextTransition.DenBuocId);

                nextStep ??= await ResolveFallbackStepAsync(hoSo.QuyTrinhSoanThaoId, currentStep.MaBuoc, request.KetQuaXuLy.Trim());

                if (string.Equals(currentStep.LoaiBuoc, "SoanThao", StringComparison.OrdinalIgnoreCase) &&
                    IsDraftTransferResult(request.KetQuaXuLy))
                {
                    await PromoteDraftAttachedFilesAsync(hoSo.Id);

                    var requiredDraftVersionNumber = ResolveDraftVersionNumber(hoSo.SoLanTraLaiHienTai);
                    var draftValidation = await ValidateDraftSubmissionAsync(hoSo.Id, requiredDraftVersionNumber);
                    if (draftValidation.Status == "error")
                    {
                        await transaction.RollbackAsync();
                        return draftValidation;
                    }

                    await SaveDraftVersionSnapshotAsync(
                        hoSo,
                        currentUser.DanhMucDonViId,
                        currentUser.Id,
                        "GUI_THAM_DINH",
                        $"Gui tham dinh du thao lan {hoSo.SoLanTraLaiHienTai + 1}");
                }

                await AdvanceWorkflowAsync(hoSo, currentUser, nextStep, request.HanXuLy, request.KetQuaXuLy.Trim(), request.NoiDungXuLy, request.DonViTiepNhanId);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return new CommonResponse("success", "ThĂ nh cĂ´ng", hoSo.Id);
            }
            catch
            {
                await transaction.RollbackAsync();
                return new CommonResponse();
            }
        }

        public async Task<CommonResponse> NhanHoSoAsync(Guid hoSoVanBanId)
        {
            var currentUser = _authService.GetUserInfo();
            if (currentUser == null)
            {
                return new CommonResponse("error", "KhĂ´ng xĂ¡c Ä‘á»‹nh Ä‘Æ°á»£c tĂ i khoáº£n Ä‘ang thao tĂ¡c!");
            }

            var hoSo = await GetHoSoWithCurrentStepAsync(hoSoVanBanId);
            if (hoSo == null)
            {
                return new CommonResponse("error", "KhĂ´ng tĂ¬m tháº¥y há»“ sÆ¡ vÄƒn báº£n!");
            }

            var currentProcessing = await _dbContext.HoSoVanBanXuLys
                .FirstOrDefaultAsync(x => x.HoSoVanBanId == hoSo.Id && x.IsCurrent);

            if (currentProcessing == null)
            {
                return new CommonResponse("error", "Há»“ sÆ¡ chÆ°a phĂ¡t sinh bÆ°á»›c xá»­ lĂ½ hiá»‡n táº¡i.");
            }

            if (!CanCurrentUserXuLy(currentUser, currentProcessing))
            {
                return new CommonResponse("error", "Há»“ sÆ¡ nĂ y khĂ´ng thuá»™c Ä‘Æ¡n vá»‹ Ä‘ang Ä‘Äƒng nháº­p Ä‘á»ƒ nháº­n.");
            }

            if (currentProcessing.NguoiXuLyId.HasValue && !currentUser.SSA)
            {
                return new CommonResponse("success", "Há»“ sÆ¡ Ä‘Ă£ Ä‘Æ°á»£c nháº­n trÆ°á»›c Ä‘Ă³.");
            }

            currentProcessing.NguoiXuLyId = currentUser.Id;
            currentProcessing.NgayNhan = DateTime.Now;
            await _dbContext.SaveChangesAsync();

            return new CommonResponse("success", "ÄĂ£ nháº­n há»“ sÆ¡ thĂ nh cĂ´ng.");
        }

        public async Task<CommonResponse> NhanHoSoAsync(Guid hoSoVanBanId, string actionType = "NHAN_HO_SO", string? noiDungXuLy = null, string? ghiChu = null, DateTime? ngayXuLy = null, DateTime? hanXuLy = null)
        {
            var currentUser = _authService.GetUserInfo();
            if (currentUser == null)
            {
                return new CommonResponse("error", "KhĂ´ng xĂ¡c Ä‘á»‹nh Ä‘Æ°á»£c tĂ i khoáº£n Ä‘ang thao tĂ¡c!");
            }

            var hoSo = await GetHoSoWithCurrentStepAsync(hoSoVanBanId);
            if (hoSo == null)
            {
                return new CommonResponse("error", "KhĂ´ng tĂ¬m tháº¥y há»“ sÆ¡ vÄƒn báº£n!");
            }

            var currentProcessing = await _dbContext.HoSoVanBanXuLys
                .FirstOrDefaultAsync(x => x.HoSoVanBanId == hoSo.Id && x.IsCurrent);

            if (currentProcessing == null)
            {
                return new CommonResponse("error", "Há»“ sÆ¡ chÆ°a phĂ¡t sinh bÆ°á»›c xá»­ lĂ½ hiá»‡n táº¡i.");
            }

            if (!CanCurrentUserXuLy(currentUser, currentProcessing))
            {
                return new CommonResponse("error", "Há»“ sÆ¡ nĂ y khĂ´ng thuá»™c Ä‘Æ¡n vá»‹ Ä‘ang Ä‘Äƒng nháº­p Ä‘á»ƒ nháº­n.");
            }

            var actionCode = NormalizeTiepNhanNghiepVuCode(actionType);
            if (string.IsNullOrWhiteSpace(actionCode))
            {
                return new CommonResponse("error", "Thao tĂ¡c nháº­n há»“ sÆ¡ khĂ´ng há»£p lá»‡.");
            }

            var laThaoTacNhanBanDau = actionCode is "NHAN_HO_SO" or "NHAN_VA_CHUYEN_PHE_DUYET" or "PHE_DUYET_HO_SO";
            if (!currentProcessing.NguoiXuLyId.HasValue)
            {
                currentProcessing.NguoiXuLyId = currentUser.Id;
                currentProcessing.NgayNhan = DateTime.Now;
            }
            else if (laThaoTacNhanBanDau && !currentUser.SSA)
            {
                return new CommonResponse("success", "Há»“ sÆ¡ Ä‘Ă£ Ä‘Æ°á»£c nháº­n trÆ°á»›c Ä‘Ă³.");
            }

            currentProcessing.KetQuaXuLy = actionCode;
            currentProcessing.NoiDungXuLy = string.IsNullOrWhiteSpace(noiDungXuLy)
                ? BuildTiepNhanNghiepVuNote(actionCode)
                : noiDungXuLy.Trim();

            if (actionCode == "CHUYEN_XET_DUYET_DANH_GIA" || actionCode == "CHUYEN_PHE_DUYET" || actionCode == "CHUYEN_BAN_HANH")
            {
                var transferTime = ngayXuLy ?? DateTime.Now;
                currentProcessing.NgayXuLy = transferTime;
                var currentStep = await GetCurrentStepAsync(hoSo);
                if (currentStep == null)
                {
                    return new CommonResponse("error", "Khï¿½ng xï¿½c d?nh du?c bu?c hi?n t?i c?a h? so.");
                }

                var ketQuaChuyenBuoc = actionCode switch
                {
                    "CHUYEN_PHE_DUYET" => "THAM_DINH_XONG",
                    "CHUYEN_BAN_HANH" => "TRINH_THANH_CONG",
                    _ => "GUI_THAM_DINH"
                };

                if (actionCode == "CHUYEN_PHE_DUYET")
                {
                    var daCoBanGhiDanhGia = await _dbContext.HoSoVanBanDanhGias
                        .AsNoTracking()
                        .AnyAsync(x => x.HoSoVanBanId == hoSoVanBanId);

                    if (!daCoBanGhiDanhGia)
                    {
                        return new CommonResponse("error", "H? so chua cï¿½ b?n ghi xï¿½t duy?t nï¿½n chua th? chuy?n sang phï¿½ duy?t.");
                    }
                }

                var nextTransition = await ResolveTransitionByResultsAsync(
                    hoSo.QuyTrinhSoanThaoId,
                    currentStep.Id,
                    ketQuaChuyenBuoc,
                    actionCode);

                var nextStep = nextTransition == null
                    ? await _dbContext.DanhMucBuocQuyTrinhs
                        .AsNoTracking()
                        .Where(x => x.QuyTrinhSoanThaoId == hoSo.QuyTrinhSoanThaoId && x.ThuTuSapXep > currentStep.ThuTuSapXep)
                        .OrderBy(x => x.ThuTuSapXep)
                        .FirstOrDefaultAsync()
                    : await _dbContext.DanhMucBuocQuyTrinhs
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.Id == nextTransition.DenBuocId);

                if (nextStep != null && nextStep.Id == currentStep.Id)
                {
                    nextStep = await _dbContext.DanhMucBuocQuyTrinhs
                        .AsNoTracking()
                        .Where(x => x.QuyTrinhSoanThaoId == hoSo.QuyTrinhSoanThaoId && x.ThuTuSapXep > currentStep.ThuTuSapXep)
                        .OrderBy(x => x.ThuTuSapXep)
                        .FirstOrDefaultAsync();
                }

                if (nextStep == null)
                {
                    return new CommonResponse("error", actionCode switch
                    {
                        "CHUYEN_PHE_DUYET" => "Workflow chua c?u hï¿½nh bu?c k? ti?p cho nghi?p v? chuy?n phï¿½ duy?t.",
                        "CHUYEN_BAN_HANH" => "Workflow chua c?u hï¿½nh bu?c k? ti?p cho nghi?p v? chuy?n ban hï¿½nh.",
                        _ => "Workflow chua c?u hï¿½nh bu?c k? ti?p cho nghi?p v? chuy?n xï¿½t duy?t."
                    });
                }

                var workflowResult = nextTransition?.DieuKienKetQua ?? ketQuaChuyenBuoc;
                currentProcessing.IsCurrent = false;
                currentProcessing.KetQuaXuLy = workflowResult;
                currentProcessing.GhiChu = string.IsNullOrWhiteSpace(ghiChu) ? currentProcessing.GhiChu : ghiChu.Trim();

                await AdvanceWorkflowAsync(hoSo, currentUser, nextStep, hanXuLy, workflowResult, currentProcessing.NoiDungXuLy);
                await _dbContext.SaveChangesAsync();

                return new CommonResponse("success", ResolveTiepNhanNghiepVuSuccessMessage(actionCode));
            }

            if (!string.IsNullOrWhiteSpace(ghiChu))
            {
                currentProcessing.GhiChu = ghiChu.Trim();
            }
            await _dbContext.SaveChangesAsync();

            return new CommonResponse("success", ResolveTiepNhanNghiepVuSuccessMessage(actionCode));
        }

        public async Task<CommonResponse> TraLaiDanhGiaAsync(Guid hoSoVanBanId, string lyDoTraLai, string? ghiChu = null)
        {
            var currentUser = _authService.GetUserInfo();
            if (currentUser == null)
            {
                return new CommonResponse("error", "Khï¿½ng xï¿½c d?nh du?c tï¿½i kho?n dang thao tï¿½c!");
            }

            if (string.IsNullOrWhiteSpace(lyDoTraLai))
            {
                return new CommonResponse("error", "B?n c?n nh?p lï¿½ do tr? l?i h? so.");
            }

            var hoSo = await GetHoSoWithCurrentStepAsync(hoSoVanBanId);
            if (hoSo == null)
            {
                return new CommonResponse("error", "Khï¿½ng tï¿½m th?y h? so van b?n!");
            }

            var currentStep = await GetCurrentStepAsync(hoSo);
            var laBuocDanhGia = currentStep != null &&
                                (string.Equals(currentStep.LoaiBuoc, "DanhGia", StringComparison.OrdinalIgnoreCase) ||
                                 string.Equals(currentStep.MaBuoc, "BUOC_03_THAM_DINH_VAN_BAN", StringComparison.OrdinalIgnoreCase));
            if (!laBuocDanhGia)
            {
                return new CommonResponse("error", "H? so hi?n khï¿½ng ? bu?c th?m d?nh van b?n.");
            }

            var currentProcessing = await _dbContext.HoSoVanBanXuLys
                .Where(x => x.HoSoVanBanId == hoSo.Id && x.IsCurrent)
                .OrderByDescending(x => x.NgayNhan)
                .FirstOrDefaultAsync();

            if (!CanCurrentUserXuLy(currentUser, currentProcessing))
            {
                return new CommonResponse("error", "H? so nï¿½y khï¿½ng thu?c don v? dang dang nh?p d? tr? l?i.");
            }

            var soLanTraLaiToiDa = currentStep.SoLanTraLaiToiDa > 0 ? currentStep.SoLanTraLaiToiDa : 3;
            if (hoSo.SoLanTraLaiHienTai >= soLanTraLaiToiDa)
            {
                return new CommonResponse("error", $"ï¿½ï¿½ vu?t quï¿½ s? l?n tr? l?i t?i da ({soLanTraLaiToiDa}) c?a bu?c nï¿½y.");
            }

            var maBuocTraLai = await ResolveDraftReturnStepCodeAsync(hoSo.QuyTrinhSoanThaoId);
            if (string.IsNullOrWhiteSpace(maBuocTraLai))
            {
                return new CommonResponse("error", "Khï¿½ng xï¿½c d?nh du?c bu?c tr? l?i cho h? so nï¿½y.");
            }

            var nextStep = await _dbContext.DanhMucBuocQuyTrinhs
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.QuyTrinhSoanThaoId == hoSo.QuyTrinhSoanThaoId && x.MaBuoc == maBuocTraLai);

            if (nextStep == null)
            {
                return new CommonResponse("error", "Khï¿½ng tï¿½m th?y bu?c so?n th?o d? tr? l?i h? so.");
            }

            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                if (currentProcessing != null)
                {
                    if (!currentProcessing.NguoiXuLyId.HasValue)
                    {
                        currentProcessing.NguoiXuLyId = currentUser.Id;
                        currentProcessing.NgayNhan = DateTime.Now;
                    }

                    currentProcessing.IsCurrent = false;
                    currentProcessing.NgayXuLy = DateTime.Now;
                    currentProcessing.KetQuaXuLy = "TRA_LAI_HO_SO";
                    currentProcessing.NoiDungXuLy = lyDoTraLai.Trim();
                    currentProcessing.GhiChu = string.IsNullOrWhiteSpace(ghiChu) ? ghiChu : ghiChu.Trim();
                }

                hoSo.SoLanTraLaiHienTai += 1;
                await SaveDraftVersionSnapshotAsync(
                    hoSo,
                    currentUser.DanhMucDonViId,
                    currentUser.Id,
                    "TRA_LAI_HO_SO_DANH_GIA",
                    $"Tr? l?i h? so t? bu?c dï¿½nh giï¿½ l?n {hoSo.SoLanTraLaiHienTai}");

                await AdvanceWorkflowAsync(hoSo, currentUser, nextStep, null, "TRA_LAI_HO_SO", lyDoTraLai.Trim());
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return new CommonResponse("success", "ï¿½ï¿½ tr? l?i h? so v? bu?c so?n th?o.");
            }
            catch
            {
                await transaction.RollbackAsync();
                return new CommonResponse();
            }
        }

        public async Task<CommonResponse> KhoiTaoLayYKienAsync(HoSoVanBanLayYKienStepModel request)
        {
            var currentUser = _authService.GetUserInfo();
            if (currentUser == null)
            {
                return new CommonResponse("error", "KhĂ´ng xĂ¡c Ä‘á»‹nh Ä‘Æ°á»£c tĂ i khoáº£n Ä‘ang thao tĂ¡c!");
            }

            var hoSo = await GetHoSoWithCurrentStepAsync(request.HoSoVanBanId);
            if (hoSo == null)
            {
                return new CommonResponse("error", "KhĂ´ng tĂ¬m tháº¥y há»“ sÆ¡ vÄƒn báº£n!");
            }

            var currentStep = await GetCurrentStepAsync(hoSo);
            if (currentStep == null || !string.Equals(currentStep.LoaiBuoc, "SoanThao", StringComparison.OrdinalIgnoreCase))
            {
                return new CommonResponse("error", "Há»“ sÆ¡ hiá»‡n khĂ´ng á»Ÿ bÆ°á»›c soáº¡n tháº£o Ä‘á»ƒ chuyá»ƒn láº¥y gĂ³p Ă½.");
            }

            var currentProcessing = await _dbContext.HoSoVanBanXuLys
                .Where(x => x.HoSoVanBanId == hoSo.Id && x.IsCurrent)
                .OrderByDescending(x => x.NgayNhan)
                .FirstOrDefaultAsync();

            if (!CanCurrentUserXuLy(currentUser, currentProcessing))
            {
                return new CommonResponse("error", "Há»“ sÆ¡ nĂ y Ä‘Ă£ Ä‘Æ°á»£c chuyá»ƒn sang Ä‘Æ¡n vá»‹ khĂ¡c. Báº¡n khĂ´ng thá»ƒ cáº­p nháº­t ná»¯a!");
            }

            var actionMode = NormalizeLayYKienActionMode(request.ActionMode);
            if (actionMode == "GUI_DON_VI_GOP_Y" && request.DonViDuocLayYKienIds.Count == 0)
            {
                return new CommonResponse("error", "Báº¡n pháº£i chá»n Ă­t nháº¥t 1 Ä‘Æ¡n vá»‹ Ä‘á»ƒ gá»­i gĂ³p Ă½.");
            }

            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var now = request.NgayPhanHoi ?? DateTime.Now;
                if (currentProcessing != null)
                {
                    if (!currentProcessing.NguoiXuLyId.HasValue)
                    {
                        currentProcessing.NguoiXuLyId = currentUser.Id;
                        currentProcessing.NgayNhan = DateTime.Now;
                    }

                    currentProcessing.KetQuaXuLy = "DANG_LAY_GOP_Y";
                    currentProcessing.NoiDungXuLy = request.NoiDungYeuCau?.Trim();
                    currentProcessing.GhiChu = request.GhiChu?.Trim();
                    currentProcessing.HanXuLy = request.HanPhanHoi ?? currentProcessing.HanXuLy;
                }

                hoSo.HanXuLy = request.HanPhanHoi ?? hoSo.HanXuLy;

                await _dbContext.SaveChangesAsync();

                var normalizedTargetIds = request.DonViDuocLayYKienIds
                    .Where(x => x != Guid.Empty)
                    .Distinct()
                    .ToList();

                if (actionMode == "GUI_DON_VI_GOP_Y")
                {
                    foreach (var donViId in normalizedTargetIds)
                    {
                        _dbContext.HoSoVanBanLayYKiens.Add(new HoSoVanBanLayYKien
                        {
                            HoSoVanBanId = hoSo.Id,
                            BuocQuyTrinhId = currentStep.Id,
                            DonViDuocLayYKienId = donViId,
                            NoiDungYeuCau = request.NoiDungYeuCau?.Trim(),
                            NgayGui = now,
                            HanPhanHoi = request.HanPhanHoi,
                            TrangThaiPhanHoi = "CHO_GOP_Y",
                            GhiChu = "ÄÆ¡n vá»‹ soáº¡n tháº£o gá»­i Ä‘á» nghá»‹ gĂ³p Ă½."
                        });

                        await TaoThongBaoAsync(
                            hoSo,
                            currentStep,
                            currentUser.DanhMucDonViId,
                            donViId,
                            $"Há»“ sÆ¡ '{hoSo.TenHoSo}' Ä‘ang láº¥y gĂ³p Ă½ tá»« Ä‘Æ¡n vá»‹ cá»§a báº¡n.");
                    }
                }
                else
                {
                    _dbContext.HoSoVanBanLayYKiens.Add(new HoSoVanBanLayYKien
                    {
                        HoSoVanBanId = hoSo.Id,
                        BuocQuyTrinhId = currentStep.Id,
                        NoiDungYeuCau = request.NoiDungYeuCau?.Trim(),
                        NgayGui = now,
                        HanPhanHoi = request.HanPhanHoi,
                        TrangThaiPhanHoi = "CHO_CAP_NHAT_KET_QUA",
                        GhiChu = "ÄÆ¡n vá»‹ soáº¡n tháº£o tá»± cáº­p nháº­t káº¿t quáº£ gĂ³p Ă½."
                    });
                }

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return new CommonResponse("success", "ÄĂ£ chuyá»ƒn há»“ sÆ¡ sang bÆ°á»›c láº¥y gĂ³p Ă½.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new CommonResponse("error", $"KhĂ´ng thá»ƒ khá»Ÿi táº¡o bÆ°á»›c láº¥y gĂ³p Ă½: {ex.Message}");
            }
        }

        public async Task<CommonResponse> TraLaiDangKyAsync(Guid hoSoVanBanId, string lyDoTraLai, string? ghiChu = null)
        {
            var currentUser = _authService.GetUserInfo();
            if (currentUser == null)
            {
                return new CommonResponse("error", "KhĂ´ng xĂ¡c Ä‘á»‹nh Ä‘Æ°á»£c tĂ i khoáº£n Ä‘ang thao tĂ¡c!");
            }

            if (string.IsNullOrWhiteSpace(lyDoTraLai))
            {
                return new CommonResponse("error", "Báº¡n pháº£i nháº­p lĂ½ do tráº£ láº¡i há»“ sÆ¡.");
            }

            var hoSo = await GetHoSoWithCurrentStepAsync(hoSoVanBanId);
            if (hoSo == null)
            {
                return new CommonResponse("error", "KhĂ´ng tĂ¬m tháº¥y há»“ sÆ¡ vÄƒn báº£n!");
            }

            var currentStep = await GetCurrentStepAsync(hoSo);
            if (currentStep == null || currentStep.MaBuoc != "BUOC_02_THONG_NHAT")
            {
                return new CommonResponse("error", "Há»“ sÆ¡ hiá»‡n khĂ´ng á»Ÿ bÆ°á»›c xĂ©t duyá»‡t Ä‘Äƒng kĂ½ Ä‘á»ƒ tráº£ láº¡i.");
            }

            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var currentProcessing = await _dbContext.HoSoVanBanXuLys
                    .Where(x => x.HoSoVanBanId == hoSo.Id && x.IsCurrent)
                    .OrderByDescending(x => x.NgayNhan)
                    .FirstOrDefaultAsync();

                if (!CanCurrentUserXuLy(currentUser, currentProcessing))
                {
                    return new CommonResponse("error", "Há»“ sÆ¡ nĂ y khĂ´ng thuá»™c Ä‘Æ¡n vá»‹ Ä‘ang Ä‘Äƒng nháº­p Ä‘á»ƒ xá»­ lĂ½.");
                }

                if (currentProcessing != null && !currentUser.SSA && !currentProcessing.NguoiXuLyId.HasValue)
                {
                    return new CommonResponse("error", "Há»“ sÆ¡ chÆ°a Ä‘Æ°á»£c nháº­n. Vui lĂ²ng nháº­n há»“ sÆ¡ trÆ°á»›c khi tráº£ láº¡i.");
                }

                if (currentProcessing != null)
                {
                    currentProcessing.IsCurrent = false;
                    currentProcessing.NgayXuLy = DateTime.Now;
                    currentProcessing.KetQuaXuLy = "KHONG_DONG_Y";
                    currentProcessing.NoiDungXuLy = lyDoTraLai.Trim();
                    currentProcessing.GhiChu = ghiChu?.Trim();
                }

                var nextStep = await ResolveFallbackStepAsync(hoSo.QuyTrinhSoanThaoId, currentStep.MaBuoc, "KHONG_DONG_Y");
                if (nextStep == null)
                {
                    return new CommonResponse("error", "KhĂ´ng xĂ¡c Ä‘á»‹nh Ä‘Æ°á»£c bÆ°á»›c quay láº¡i khi tráº£ há»“ sÆ¡.");
                }

                hoSo.SoLanTraLaiHienTai += 1;
                await AdvanceWorkflowAsync(hoSo, currentUser, nextStep, null, "KHONG_DONG_Y", lyDoTraLai.Trim());

                var processingBackStep = _dbContext.HoSoVanBanXuLys.Local
                    .LastOrDefault(x => x.HoSoVanBanId == hoSo.Id && x.IsCurrent);

                if (processingBackStep != null)
                {
                    processingBackStep.KetQuaXuLy = "TRA_LAI_HO_SO";
                    processingBackStep.NoiDungXuLy = lyDoTraLai.Trim();
                    processingBackStep.GhiChu = ghiChu?.Trim();
                }

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return new CommonResponse("success", "ÄĂ£ tráº£ láº¡i há»“ sÆ¡ vá» bÆ°á»›c Ä‘Äƒng kĂ½.");
            }
            catch
            {
                await transaction.RollbackAsync();
                return new CommonResponse();
            }
        }

        public async Task<CommonResponse> HuyXetDuyetDangKyAsync(Guid hoSoVanBanId, string lyDoHuy, DateTime? ngayHuy = null, string? ghiChu = null)
        {
            var currentUser = _authService.GetUserInfo();
            if (currentUser == null)
            {
                return new CommonResponse("error", "Khong xac dinh duoc tai khoan dang thao tac!");
            }

            if (string.IsNullOrWhiteSpace(lyDoHuy))
            {
                return new CommonResponse("error", "Ban phai nhap ly do huy xet duyet.");
            }

            var hoSo = await GetHoSoWithCurrentStepAsync(hoSoVanBanId);
            if (hoSo == null)
            {
                return new CommonResponse("error", "Khong tim thay ho so van ban!");
            }

            var currentStep = await GetCurrentStepAsync(hoSo);
            var latestReviewProcessing = await _dbContext.HoSoVanBanXuLys
                .Where(x => x.HoSoVanBanId == hoSo.Id)
                .Join(
                    _dbContext.DanhMucBuocQuyTrinhs.AsNoTracking().Where(x => x.MaBuoc == "BUOC_02_THONG_NHAT"),
                    xuLy => xuLy.BuocQuyTrinhId,
                    buoc => buoc.Id,
                    (xuLy, _) => xuLy)
                .OrderByDescending(x => x.NgayXuLy ?? x.NgayNhan)
                .ThenByDescending(x => x.CreatedDate)
                .FirstOrDefaultAsync();

            if (latestReviewProcessing == null)
            {
                return new CommonResponse("error", "Ho so nay chua phat sinh buoc xet duyet de huy.");
            }

            if (!CanDangKyReviewUserAccess(currentUser, hoSo, latestReviewProcessing))
            {
                return new CommonResponse("error", "Ban khong co quyen huy ket qua xet duyet cua ho so nay.");
            }

            var stepDangKy = await _dbContext.DanhMucBuocQuyTrinhs
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.QuyTrinhSoanThaoId == hoSo.QuyTrinhSoanThaoId && x.MaBuoc == "BUOC_01_DANG_KY");

            if (stepDangKy == null)
            {
                return new CommonResponse("error", "Khong tim thay buoc dang ky de khoi phuc ho so.");
            }

            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var thoiGianHuy = ngayHuy ?? DateTime.Now;
                var currentProcessing = await _dbContext.HoSoVanBanXuLys
                    .Where(x => x.HoSoVanBanId == hoSo.Id && x.IsCurrent)
                    .OrderByDescending(x => x.NgayNhan)
                    .FirstOrDefaultAsync();

                if (currentProcessing != null)
                {
                    if (!CanDangKyReviewUserAccess(currentUser, hoSo, currentProcessing))
                    {
                        return new CommonResponse("error", "Ban khong co quyen cap nhat ho so nay.");
                    }

                    currentProcessing.IsCurrent = false;
                    currentProcessing.NgayXuLy = thoiGianHuy;
                    currentProcessing.KetQuaXuLy = "HUY_XET_DUYET";
                    currentProcessing.NoiDungXuLy = lyDoHuy.Trim();
                    currentProcessing.GhiChu = ghiChu?.Trim();
                }

                hoSo.NgayHoanThanh = null;
                hoSo.BuocHienTaiId = null;

                await AdvanceWorkflowAsync(
                    hoSo,
                    currentUser,
                    stepDangKy,
                    null,
                    "HUY_XET_DUYET",
                    lyDoHuy.Trim());

                var processingBackStep = _dbContext.HoSoVanBanXuLys.Local
                    .LastOrDefault(x => x.HoSoVanBanId == hoSo.Id && x.IsCurrent);

                if (processingBackStep != null)
                {
                    processingBackStep.KetQuaXuLy = "HUY_XET_DUYET";
                    processingBackStep.NgayNhan = thoiGianHuy;
                    processingBackStep.NoiDungXuLy = lyDoHuy.Trim();
                    processingBackStep.GhiChu = ghiChu?.Trim();
                }

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return new CommonResponse("success", "Da huy xet duyet va dua ho so ve buoc dang ky.");
            }
            catch
            {
                await transaction.RollbackAsync();
                return new CommonResponse();
            }
        }

        public async Task<CommonResponse> TaoHoSoSoanThaoTuDangKyAsync(HoSoVanBanTaoSoanThaoTuDangKyModel request)
        {
            var currentUser = _authService.GetUserInfo();
            if (currentUser == null)
            {
                return new CommonResponse("error", "Khong xac dinh duoc tai khoan dang thao tac!");
            }

            if (request.HoSoDangKyId == Guid.Empty)
            {
                return new CommonResponse("error", "Ho so dang ky khong hop le.");
            }

            if (!request.DonViSoanThaoId.HasValue || request.DonViSoanThaoId == Guid.Empty)
            {
                return new CommonResponse("error", "Ban phai chon don vi soan thao.");
            }

            if (request.QuyTrinhSoanThaoId == Guid.Empty)
            {
                return new CommonResponse("error", "Ban phai chon quy trinh soan thao.");
            }

            if (!request.DenNgaySoanThao.HasValue)
            {
                return new CommonResponse("error", "Ban phai nhap thoi han hoan thanh soan thao.");
            }

            if (request.DenNgaySoanThao.Value.Date < request.TuNgaySoanThao.Date)
            {
                return new CommonResponse("error", "Thoi han hoan thanh phai lon hon hoac bang tu ngay soan thao.");
            }

            var hoSoDangKy = await GetHoSoWithCurrentStepAsync(request.HoSoDangKyId);
            if (hoSoDangKy == null)
            {
                return new CommonResponse("error", "Khong tim thay ho so dang ky!");
            }

            var latestReviewProcessing = await _dbContext.HoSoVanBanXuLys
                .Where(x => x.HoSoVanBanId == hoSoDangKy.Id)
                .Join(
                    _dbContext.DanhMucBuocQuyTrinhs.AsNoTracking().Where(x => x.MaBuoc == "BUOC_02_THONG_NHAT"),
                    xuLy => xuLy.BuocQuyTrinhId,
                    buoc => buoc.Id,
                    (xuLy, _) => xuLy)
                .OrderByDescending(x => x.NgayXuLy ?? x.NgayNhan)
                .ThenByDescending(x => x.CreatedDate)
                .FirstOrDefaultAsync();

            if (latestReviewProcessing == null || !string.Equals(latestReviewProcessing.KetQuaXuLy, "DONG_Y", StringComparison.OrdinalIgnoreCase))
            {
                return new CommonResponse("error", "Ho so nay chua duoc xet duyet dong y de tao ho so soan thao.");
            }

            if (!CanDangKyReviewUserAccess(currentUser, hoSoDangKy, latestReviewProcessing))
            {
                return new CommonResponse("error", "Ban khong co quyen tao ho so soan thao tu ho so dang ky nay.");
            }

            var existingDraftHoSo = await FindExistingDraftHoSoByDangKyIdAsync(hoSoDangKy.Id);
            if (existingDraftHoSo != null)
            {
                return new CommonResponse("success", "Ho so soan thao da duoc tao truoc do.", existingDraftHoSo.Id);
            }

            var targetWorkflow = await _dbContext.DanhMucQuyTrinhSoanThaos
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.QuyTrinhSoanThaoId &&
                                          x.TrangThai &&
                                          x.LoaiQuyTrinh == NormalizeWorkflowType("XayDung"));

            if (targetWorkflow == null)
            {
                return new CommonResponse("error", "Khong tim thay workflow ho so soan thao duoc cau hinh.");
            }

            if (targetWorkflow.LoaiQuyTrinh != NormalizeWorkflowType("XayDung"))
            {
                return new CommonResponse("error", "Workflow duoc chon khong phai workflow ho so soan thao rieng.");
            }

            var stepSoanThao = await ResolveDraftStartStepAsync(targetWorkflow.Id);

            if (stepSoanThao == null)
            {
                return new CommonResponse("error", "Workflow duoc chon chua cau hinh buoc soan thao.");
            }

            request.BuocThoiHans = request.BuocThoiHans
                .Where(x => x.BuocQuyTrinhId != Guid.Empty)
                .OrderBy(x => x.ThuTuSapXep)
                .ToList();

            var stepPlans = await BuildRequestedStepDeadlinePlansAsync(targetWorkflow.Id, request.BuocThoiHans);
            var now = DateTime.Now;
            var newHoSoId = Guid.NewGuid();
            var hanXuLySoanThao = request.DenNgaySoanThao.Value.Date;
            var dangXuLyStatusId = await GetTrangThaiIdByCodeAsync("DANG_XU_LY");

            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var hoSoSoanThao = new HoSoVanBan
                {
                    Id = newHoSoId,
                    MaHoSo = newHoSoId.ToString(),
                    TenHoSo = hoSoDangKy.TenHoSo,
                    DanhMucVanBanId = hoSoDangKy.DanhMucVanBanId,
                    QuyTrinhSoanThaoId = targetWorkflow.Id,
                    BuocHienTaiId = stepSoanThao.Id,
                    DanhMucTrangThaiId = dangXuLyStatusId,
                    DonViSoanThaoId = request.DonViSoanThaoId.Value,
                    NguoiTaoId = currentUser.Id,
                    NgayTaoHoSo = now,
                    HanXuLy = hanXuLySoanThao,
                    AttachedFileGroupId = newHoSoId,
                    MoTa = hoSoDangKy.MoTa,
                    GhiChu = BuildDraftSourceNote(hoSoDangKy.Id, request.GhiChu),
                    SoLanTraLaiHienTai = 0
                };

                _dbContext.HoSoVanBans.Add(hoSoSoanThao);
                await _dbContext.SaveChangesAsync();

                if (stepPlans.Count > 0)
                {
                    var planRows = stepPlans.Select(x => new HoSoVanBanBuocThoiHan
                    {
                        HoSoVanBanId = hoSoSoanThao.Id,
                        BuocQuyTrinhId = x.BuocQuyTrinhId,
                        ThuTuSapXep = x.ThuTuSapXep,
                        SoNgayXuLy = x.SoNgayXuLy,
                        SoNgayCanhBaoSapHan = x.SoNgayCanhBaoSapHan,
                        GhiChu = x.GhiChu
                    }).ToList();

                    _dbContext.HoSoVanBanBuocThoiHans.AddRange(planRows);
                    await _dbContext.SaveChangesAsync();
                }

                var xuLyDauTien = new HoSoVanBanXuLy
                {
                    HoSoVanBanId = hoSoSoanThao.Id,
                    BuocQuyTrinhId = stepSoanThao.Id,
                    LanXuLy = 1,
                    DonViXuLyId = hoSoSoanThao.DonViSoanThaoId,
                    NguoiXuLyId = currentUser.SSA || currentUser.DanhMucDonViId == hoSoSoanThao.DonViSoanThaoId
                        ? currentUser.Id
                        : null,
                    NgayNhan = now,
                    HanXuLy = hanXuLySoanThao,
                    DanhMucTrangThaiId = dangXuLyStatusId,
                    IsCurrent = true,
                    KetQuaXuLy = null,
                    NoiDungXuLy = $"Khoi tao ho so soan thao tu ho so dang ky '{hoSoDangKy.TenHoSo}'.",
                    GhiChu = BuildDraftSourceNote(hoSoDangKy.Id, null)
                };

                _dbContext.HoSoVanBanXuLys.Add(xuLyDauTien);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                await TaoThongBaoAsync(
                    hoSoSoanThao,
                    stepSoanThao,
                    currentUser.DanhMucDonViId,
                    xuLyDauTien.DonViXuLyId,
                    $"Ho so soan thao '{hoSoSoanThao.TenHoSo}' da duoc tao tu ho so dang ky.");

                return new CommonResponse("success", "Da tao ho so soan thao tu ho so dang ky.", hoSoSoanThao.Id);
            }
            catch
            {
                await transaction.RollbackAsync();
                return new CommonResponse();
            }
        }

        public async Task<CommonResponse> HoanThanhLayYKienAsync(HoSoVanBanLayYKienStepModel request)
        {
            var currentUser = _authService.GetUserInfo();
            if (currentUser == null)
            {
                return new CommonResponse("error", "KhĂ´ng xĂ¡c Ä‘á»‹nh Ä‘Æ°á»£c tĂ i khoáº£n Ä‘ang thao tĂ¡c!");
            }

            var hoSo = await GetHoSoWithCurrentStepAsync(request.HoSoVanBanId);
            if (hoSo == null)
            {
                return new CommonResponse("error", "KhĂ´ng tĂ¬m tháº¥y há»“ sÆ¡ vÄƒn báº£n!");
            }

            var currentStep = await GetCurrentStepAsync(hoSo);
            var currentProcessing = await _dbContext.HoSoVanBanXuLys
                .AsNoTracking()
                .Where(x => x.HoSoVanBanId == hoSo.Id && x.IsCurrent)
                .OrderByDescending(x => x.NgayNhan)
                .FirstOrDefaultAsync();

            var dangLayGopY = string.Equals(currentProcessing?.KetQuaXuLy, "DANG_LAY_GOP_Y", StringComparison.OrdinalIgnoreCase);
            if (currentStep == null || (currentStep.LoaiBuoc != "LayYKien" && !dangLayGopY))
            {
                return new CommonResponse("error", "Há»“ sÆ¡ hiá»‡n khĂ´ng á»Ÿ nghiá»‡p vá»¥ láº¥y Ă½ kiáº¿n!");
            }

            if (currentStep.YeuCauFileDinhKem && !request.AttachedFileGroupId.HasValue)
            {
                return new CommonResponse("error", "BÆ°á»›c nĂ y yĂªu cáº§u cĂ³ file Ä‘Ă­nh kĂ¨m káº¿t quáº£ láº¥y Ă½ kiáº¿n!");
            }

            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                currentProcessing = await _dbContext.HoSoVanBanXuLys
                    .Where(x => x.HoSoVanBanId == hoSo.Id && x.IsCurrent)
                    .OrderByDescending(x => x.NgayNhan)
                    .FirstOrDefaultAsync();

                if (!CanCurrentUserXuLy(currentUser, currentProcessing))
                {
                    return new CommonResponse("error", "Ho so nay da duoc chuyen sang don vi khac. Ban khong the cap nhat nua!");
                }

                var actionMode = NormalizeLayYKienActionMode(request.ActionMode);
                var existingRows = await _dbContext.HoSoVanBanLayYKiens
                    .Where(x => x.HoSoVanBanId == hoSo.Id && x.BuocQuyTrinhId == currentStep.Id)
                    .ToListAsync();

                HoSoVanBanLayYKien? layYKien = null;

                if (actionMode == "PHAN_HOI_DON_VI")
                {
                    if (!request.DonViDuocLayYKienId.HasValue || request.DonViDuocLayYKienId == Guid.Empty)
                    {
                        return new CommonResponse("error", "Thiáº¿u thĂ´ng tin Ä‘Æ¡n vá»‹ pháº£n há»“i gĂ³p Ă½.");
                    }

                    layYKien = existingRows
                        .Where(x => x.DonViDuocLayYKienId == request.DonViDuocLayYKienId.Value)
                        .OrderByDescending(x => x.CreatedDate)
                        .FirstOrDefault();

                    if (layYKien == null)
                    {
                        return new CommonResponse("error", "KhĂ´ng tĂ¬m tháº¥y yĂªu cáº§u gĂ³p Ă½ cá»§a Ä‘Æ¡n vá»‹ Ä‘Æ°á»£c chá»n.");
                    }

                    layYKien.NoiDungPhanHoi = request.NoiDungPhanHoi?.Trim();
                    layYKien.NgayPhanHoi = request.NgayPhanHoi ?? DateTime.Now;
                    layYKien.TrangThaiPhanHoi = string.IsNullOrWhiteSpace(request.TrangThaiPhanHoi) ? "DA_CO_Y_KIEN" : request.TrangThaiPhanHoi.Trim();
                    layYKien.AttachedFileGroupId = request.AttachedFileGroupId;
                    layYKien.GhiChu = request.GhiChu?.Trim();

                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new CommonResponse("success", "ÄĂ£ cáº­p nháº­t Ă½ kiáº¿n cá»§a Ä‘Æ¡n vá»‹ gĂ³p Ă½.", layYKien.Id);
                }

                if (actionMode == "TONG_HOP_Y_KIEN")
                {
                    request.CacLayYKien ??= new List<HoSoVanBanLayYKienItemModel>();

                    var invalidUnitRow = request.CacLayYKien.FirstOrDefault(x =>
                        x.DonViDuocLayYKienId == null || x.DonViDuocLayYKienId == Guid.Empty);
                    if (invalidUnitRow != null)
                    {
                        return new CommonResponse("error", "Vui lĂ²ng chá»n Ä‘Æ¡n vá»‹ gĂ³p Ă½ cho táº¥t cáº£ cĂ¡c dĂ²ng trÆ°á»›c khi lÆ°u.");
                    }

                    var updateRows = request.CacLayYKien
                        .Where(x => x.Id != Guid.Empty)
                        .ToDictionary(x => x.Id, x => x);

                    if (updateRows.Count > 0)
                    {
                        var existingOpinionRows = await _dbContext.HoSoVanBanLayYKiens
                            .Where(x => x.HoSoVanBanId == hoSo.Id &&
                                        x.BuocQuyTrinhId == currentStep.Id &&
                                        updateRows.Keys.Contains(x.Id))
                            .ToListAsync();

                        foreach (var row in existingOpinionRows)
                        {
                            if (!updateRows.TryGetValue(row.Id, out var input))
                            {
                                continue;
                            }

                            row.NoiDungPhanHoi = input.NoiDungPhanHoi?.Trim();
                            row.GhiChu = input.GhiChu?.Trim();
                            row.NgayPhanHoi ??= request.NgayPhanHoi ?? DateTime.Now;

                            if (!string.IsNullOrWhiteSpace(row.NoiDungPhanHoi))
                            {
                                row.TrangThaiPhanHoi = "DA_CO_Y_KIEN";
                            }
                        }
                    }

                    var existingUnitIds = existingRows
                        .Where(x => x.DonViDuocLayYKienId.HasValue)
                        .Select(x => x.DonViDuocLayYKienId!.Value)
                        .ToHashSet();

                    var newRows = request.CacLayYKien
                        .Where(x => x.Id == Guid.Empty
                            && x.DonViDuocLayYKienId.HasValue
                            && x.DonViDuocLayYKienId.Value != Guid.Empty
                            && !existingUnitIds.Contains(x.DonViDuocLayYKienId.Value)
                            && (!string.IsNullOrWhiteSpace(x.NoiDungPhanHoi) || !string.IsNullOrWhiteSpace(x.GhiChu)))
                        .ToList();

                    foreach (var input in newRows)
                    {
                        var newOpinion = new HoSoVanBanLayYKien
                        {
                            HoSoVanBanId = hoSo.Id,
                            BuocQuyTrinhId = currentStep.Id,
                            DonViDuocLayYKienId = input.DonViDuocLayYKienId,
                            NoiDungYeuCau = input.NoiDungYeuCau?.Trim() ?? request.NoiDungYeuCau?.Trim(),
                            NoiDungPhanHoi = input.NoiDungPhanHoi?.Trim(),
                            NgayGui = request.NgayPhanHoi ?? DateTime.Now,
                            HanPhanHoi = input.HanPhanHoi ?? request.HanPhanHoi,
                            NgayPhanHoi = request.NgayPhanHoi ?? DateTime.Now,
                            TrangThaiPhanHoi = string.IsNullOrWhiteSpace(input.NoiDungPhanHoi)
                                ? "CHO_CAP_NHAT_KET_QUA"
                                : "DA_CO_Y_KIEN",
                            AttachedFileGroupId = input.AttachedFileGroupId,
                            GhiChu = input.GhiChu?.Trim()
                        };

                        _dbContext.HoSoVanBanLayYKiens.Add(newOpinion);
                        existingUnitIds.Add(input.DonViDuocLayYKienId.Value);
                    }

                    layYKien = new HoSoVanBanLayYKien
                    {
                        HoSoVanBanId = hoSo.Id,
                        BuocQuyTrinhId = currentStep.Id,
                        NoiDungYeuCau = request.NoiDungYeuCau?.Trim(),
                        NoiDungPhanHoi = request.NoiDungPhanHoi?.Trim(),
                        NgayGui = DateTime.Now,
                        HanPhanHoi = request.HanPhanHoi,
                        NgayPhanHoi = request.NgayPhanHoi ?? DateTime.Now,
                        TrangThaiPhanHoi = "DA_GAN_KET_QUA_Y_KIEN",
                        AttachedFileGroupId = request.AttachedFileGroupId,
                        GhiChu = request.GhiChu?.Trim()
                    };
                    _dbContext.HoSoVanBanLayYKiens.Add(layYKien);
                }
                else
                {
                    layYKien = new HoSoVanBanLayYKien
                    {
                        HoSoVanBanId = hoSo.Id,
                        BuocQuyTrinhId = currentStep.Id,
                        NguoiDuocLayYKienId = request.NguoiDuocLayYKienId,
                        DonViDuocLayYKienId = request.DonViDuocLayYKienId,
                        NoiDungYeuCau = request.NoiDungYeuCau?.Trim(),
                        NoiDungPhanHoi = request.NoiDungPhanHoi?.Trim(),
                        NgayGui = DateTime.Now,
                        HanPhanHoi = request.HanPhanHoi,
                        NgayPhanHoi = request.NgayPhanHoi ?? DateTime.Now,
                        TrangThaiPhanHoi = string.IsNullOrWhiteSpace(request.TrangThaiPhanHoi) ? "DA_CO_Y_KIEN" : request.TrangThaiPhanHoi.Trim(),
                        AttachedFileGroupId = request.AttachedFileGroupId,
                        GhiChu = request.GhiChu?.Trim()
                    };

                    _dbContext.HoSoVanBanLayYKiens.Add(layYKien);
                }

                if (currentProcessing != null)
                {
                    currentProcessing.NgayXuLy = layYKien.NgayPhanHoi;
                    currentProcessing.NoiDungXuLy = string.IsNullOrWhiteSpace(layYKien.NoiDungPhanHoi)
                        ? currentProcessing.NoiDungXuLy
                        : layYKien.NoiDungPhanHoi;
                    currentProcessing.GhiChu = string.IsNullOrWhiteSpace(layYKien.GhiChu)
                        ? currentProcessing.GhiChu
                        : layYKien.GhiChu;

                    if (dangLayGopY)
                    {
                        currentProcessing.KetQuaXuLy = actionMode switch
                        {
                            "TONG_HOP_Y_KIEN" => "DA_TONG_HOP_Y_KIEN",
                            "PHAN_HOI_DON_VI" => "DANG_LAY_GOP_Y",
                            _ => currentProcessing.KetQuaXuLy
                        };
                    }
                    else
                    {
                        currentProcessing.IsCurrent = false;
                        currentProcessing.KetQuaXuLy = layYKien.TrangThaiPhanHoi;
                    }
                }

                if (actionMode == "TONG_HOP_Y_KIEN")
                {
                    await DongBoHoSoDuThaoSauLayYKienAsync(hoSo, request);
                }

                if (!dangLayGopY)
                {
                    var nextTransition = await GetTransitionAsync(hoSo.QuyTrinhSoanThaoId, currentStep.Id, layYKien.TrangThaiPhanHoi);
                    var nextStep = nextTransition == null
                        ? null
                        : await _dbContext.DanhMucBuocQuyTrinhs.AsNoTracking().FirstOrDefaultAsync(x => x.Id == nextTransition.DenBuocId);

                    await AdvanceWorkflowAsync(hoSo, currentUser, nextStep, null, layYKien.TrangThaiPhanHoi, layYKien.NoiDungPhanHoi);
                }

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return new CommonResponse("success", "ThĂ nh cĂ´ng", layYKien.Id);
            }
            catch
            {
                await transaction.RollbackAsync();
                return new CommonResponse();
            }
        }

        public async Task<CommonResponse> HoanThanhDanhGiaAsync(HoSoVanBanDanhGiaStepModel request)
        {
            var currentUser = _authService.GetUserInfo();
            if (currentUser == null)
            {
                return new CommonResponse("error", "KhĂ´ng xĂ¡c Ä‘á»‹nh Ä‘Æ°á»£c tĂ i khoáº£n Ä‘ang thao tĂ¡c!");
            }

            var hoSo = await GetHoSoWithCurrentStepAsync(request.HoSoVanBanId);
            if (hoSo == null)
            {
                return new CommonResponse("error", "KhĂ´ng tĂ¬m tháº¥y há»“ sÆ¡ vÄƒn báº£n!");
            }

            var currentStep = await GetCurrentStepAsync(hoSo);
            var laBuocDanhGia = currentStep != null &&
                                (string.Equals(currentStep.LoaiBuoc, "DanhGia", StringComparison.OrdinalIgnoreCase) ||
                                 string.Equals(currentStep.MaBuoc, "BUOC_03_THAM_DINH_VAN_BAN", StringComparison.OrdinalIgnoreCase));
            if (!laBuocDanhGia)
            {
                return new CommonResponse("error", "H? so hi?n khï¿½ng ? bu?c th?m d?nh van b?n.");
            }

            var ketQua = request.KetQuaDanhGia.Trim().ToUpperInvariant();
            var ketQuaDuocChapNhan = new[] { "DAT", "THAM_DINH_XONG" };
            var laTraLaiThamDinh = ketQua == "KHONG_DAT" || ketQua.StartsWith("KHONG_DAT_LAN_", StringComparison.OrdinalIgnoreCase);
            if (!ketQuaDuocChapNhan.Contains(ketQua) && !laTraLaiThamDinh)
            {
                return new CommonResponse("error", "K?t qu? xï¿½t duy?t ch? ch?p nh?n DAT, THAM_DINH_XONG ho?c cï¿½c tr?ng thï¿½i KHONG_DAT_LAN_1..3!");
            }

            if (currentStep.YeuCauFileDinhKem && !request.AttachedFileGroupId.HasValue)
            {
                return new CommonResponse("error", "BÆ°á»›c Ä‘Ă¡nh giĂ¡ hiá»‡n táº¡i yĂªu cáº§u file Ä‘Ă­nh kĂ¨m!");
            }

            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var currentProcessing = await _dbContext.HoSoVanBanXuLys
                    .Where(x => x.HoSoVanBanId == hoSo.Id && x.IsCurrent)
                    .OrderByDescending(x => x.NgayNhan)
                    .FirstOrDefaultAsync();

                if (!CanCurrentUserXuLy(currentUser, currentProcessing))
                {
                    return new CommonResponse("error", "Ho so nay da duoc chuyen sang don vi khac. Ban khong the cap nhat nua!");
                }

                if (currentProcessing == null)
                {
                    return new CommonResponse("error", "Khong tim thay du lieu tiep nhan hien tai cua ho so.");
                }

                var donViDanhGiaId = currentProcessing.DonViXuLyId != Guid.Empty
                    ? currentProcessing.DonViXuLyId
                    : currentUser.DanhMucDonViId;

                if (donViDanhGiaId == Guid.Empty)
                {
                    return new CommonResponse("error", "Khong xac dinh duoc don vi tham dinh de luu ket qua.");
                }

                Guid? traLaiBuocId = null;
                DanhMucBuocQuyTrinh? nextStep = null;
                if (laTraLaiThamDinh)
                {
                    var soLanTraLaiToiDa = currentStep.SoLanTraLaiToiDa > 0
                        ? currentStep.SoLanTraLaiToiDa
                        : 3;

                    if (hoSo.SoLanTraLaiHienTai >= soLanTraLaiToiDa)
                    {
                        return new CommonResponse("error", $"Da vuot qua so lan tra lai toi da ({soLanTraLaiToiDa}) cua buoc nay!");
                    }

                    if (string.IsNullOrWhiteSpace(request.TraLaiBuocMa))
                    {
                        request.TraLaiBuocMa = await ResolveDraftReturnStepCodeAsync(hoSo.QuyTrinhSoanThaoId);
                    }

                    if (string.IsNullOrWhiteSpace(request.TraLaiBuocMa))
                    {
                        return new CommonResponse("error", "Phai chi dinh ma buoc tra lai khi ket qua la KHONG_DAT!");
                    }

                    nextStep = await _dbContext.DanhMucBuocQuyTrinhs
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.QuyTrinhSoanThaoId == hoSo.QuyTrinhSoanThaoId && x.MaBuoc == request.TraLaiBuocMa.Trim());

                    if (nextStep == null)
                    {
                        return new CommonResponse("error", "KhĂ´ng tĂ¬m tháº¥y bÆ°á»›c tráº£ láº¡i theo mĂ£ bÆ°á»›c Ä‘Ă£ nháº­p!");
                    }

                    traLaiBuocId = nextStep.Id;
                }

                var lanDanhGia = await _dbContext.HoSoVanBanDanhGias.CountAsync(x => x.HoSoVanBanId == hoSo.Id) + 1;
                var danhGia = new HoSoVanBanDanhGia
                {
                    HoSoVanBanId = hoSo.Id,
                    BuocQuyTrinhId = currentStep.Id,
                    LanDanhGia = lanDanhGia,
                    DonViDanhGiaId = donViDanhGiaId,
                    NguoiDanhGiaId = request.NguoiDanhGiaId ?? currentUser.Id,
                    NgayDanhGia = DateTime.Now,
                    KetQuaDanhGia = ketQua,
                    NoiDungDanhGia = request.NoiDungDanhGia?.Trim(),
                    YeuCauChinhSua = request.YeuCauChinhSua?.Trim(),
                    AttachedFileGroupId = request.AttachedFileGroupId,
                    TraLaiBuocId = traLaiBuocId,
                    GhiChu = request.GhiChu?.Trim()
                };

                _dbContext.HoSoVanBanDanhGias.Add(danhGia);

                if (currentProcessing != null)
                {
                    currentProcessing.IsCurrent = false;
                    currentProcessing.NgayXuLy = DateTime.Now;
                    currentProcessing.KetQuaXuLy = ketQua;
                    currentProcessing.NoiDungXuLy = request.NoiDungDanhGia?.Trim();
                    currentProcessing.GhiChu = request.GhiChu?.Trim();
                }

                if (ketQuaDuocChapNhan.Contains(ketQua))
                {
                    var nextTransition = await GetTransitionAsync(hoSo.QuyTrinhSoanThaoId, currentStep.Id, ketQua);
                    nextStep = nextTransition == null
                        ? await _dbContext.DanhMucBuocQuyTrinhs
                            .AsNoTracking()
                            .Where(x => x.QuyTrinhSoanThaoId == hoSo.QuyTrinhSoanThaoId && x.ThuTuSapXep > currentStep.ThuTuSapXep)
                            .OrderBy(x => x.ThuTuSapXep)
                            .FirstOrDefaultAsync()
                        : await _dbContext.DanhMucBuocQuyTrinhs.AsNoTracking().FirstOrDefaultAsync(x => x.Id == nextTransition.DenBuocId);
                }
                else
                {
                    hoSo.SoLanTraLaiHienTai += 1;
                    await SaveDraftVersionSnapshotAsync(
                        hoSo,
                        currentUser.DanhMucDonViId,
                        currentUser.Id,
                        "TRA_LAI_THAM_DINH",
                        $"Tra lai tham dinh lan {hoSo.SoLanTraLaiHienTai}");
                }

                await AdvanceWorkflowAsync(hoSo, currentUser, nextStep, null, ketQua, request.NoiDungDanhGia);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return new CommonResponse("success", "ThĂ nh cĂ´ng", danhGia.Id);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new CommonResponse("error", ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<CommonResponse> PhanHoiDanhGiaAsync(HoSoVanBanPhanHoiDanhGiaModel request)
        {
            var currentUser = _authService.GetUserInfo();
            if (currentUser == null)
            {
                return new CommonResponse("error", "KhĂ´ng xĂ¡c Ä‘á»‹nh Ä‘Æ°á»£c tĂ i khoáº£n Ä‘ang thao tĂ¡c!");
            }

            var hoSo = await _dbContext.HoSoVanBans.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.HoSoVanBanId);
            if (hoSo == null)
            {
                return new CommonResponse("error", "KhĂ´ng tĂ¬m tháº¥y há»“ sÆ¡ vÄƒn báº£n!");
            }

            var danhGia = await _dbContext.HoSoVanBanDanhGias
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.HoSoVanBanDanhGiaId && x.HoSoVanBanId == request.HoSoVanBanId);

            if (danhGia == null)
            {
                return new CommonResponse("error", "KhĂ´ng tĂ¬m tháº¥y láº§n Ä‘Ă¡nh giĂ¡ cáº§n pháº£n há»“i!");
            }

            try
            {
                var phanHoi = new HoSoVanBanPhanHoiDanhGia
                {
                    HoSoVanBanDanhGiaId = danhGia.Id,
                    HoSoVanBanId = hoSo.Id,
                    LanDanhGia = danhGia.LanDanhGia,
                    DonViSoanThaoId = currentUser.DanhMucDonViId,
                    NguoiPhanHoiId = currentUser.Id,
                    NgayPhanHoi = DateTime.Now,
                    NoiDungGiaiTrinh = request.NoiDungGiaiTrinh?.Trim(),
                    AttachedFileGroupId = request.AttachedFileGroupId,
                    GhiChu = request.GhiChu?.Trim()
                };

                _dbContext.HoSoVanBanPhanHoiDanhGias.Add(phanHoi);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse("success", "ThĂ nh cĂ´ng", phanHoi.Id);
            }
            catch
            {
                return new CommonResponse();
            }
        }

        public async Task<CommonResponse> GetChiTietAsync(Guid hoSoVanBanId)
        {
            try
            {
                var model = await (
                    from hoSo in _dbContext.HoSoVanBans.AsNoTracking()
                    join vanBan in _dbContext.DanhMucVanBans.AsNoTracking() on hoSo.DanhMucVanBanId equals vanBan.Id
                    join quyTrinh in _dbContext.DanhMucQuyTrinhSoanThaos.AsNoTracking() on hoSo.QuyTrinhSoanThaoId equals quyTrinh.Id
                    join donVi in _dbContext.DanhMucDonVis.AsNoTracking() on hoSo.DonViSoanThaoId equals donVi.Id
                    join buoc in _dbContext.DanhMucBuocQuyTrinhs.AsNoTracking() on hoSo.BuocHienTaiId equals buoc.Id into buocJoin
                    from buoc in buocJoin.DefaultIfEmpty()
                    join xuLyCurrent in _dbContext.HoSoVanBanXuLys.AsNoTracking().Where(x => x.IsCurrent) on hoSo.Id equals xuLyCurrent.HoSoVanBanId into xuLyCurrentJoin
                    from xuLyCurrent in xuLyCurrentJoin.DefaultIfEmpty()
                    join trangThai in _dbContext.DanhMucTrangThais.AsNoTracking() on hoSo.DanhMucTrangThaiId equals trangThai.Id into trangThaiJoin
                    from trangThai in trangThaiJoin.DefaultIfEmpty()
                    where hoSo.Id == hoSoVanBanId
                    select new HoSoVanBanWorkflowDetailModel
                    {
                        Id = hoSo.Id,
                        MaHoSo = hoSo.MaHoSo,
                        TenHoSo = hoSo.TenHoSo,
                        TenLoaiVanBan = vanBan.TenLoaiVanBan,
                        TenQuyTrinh = quyTrinh.TenQuyTrinh,
                        TenBuocHienTai = buoc != null ? buoc.TenBuoc : null,
                        MaBuocHienTai = buoc != null ? buoc.MaBuoc : null,
                        BuocHienTaiId = hoSo.BuocHienTaiId,
                        LoaiBuocHienTai = buoc != null ? buoc.LoaiBuoc : null,
                        TenTrangThai = trangThai != null ? trangThai.TenTrangThai : null,
                        MaMauTrangThai = trangThai != null ? trangThai.MaMauHex : null,
                        TenDonViSoanThao = donVi.TenDonVi,
                        DonViXuLyHienTaiId = xuLyCurrent != null ? xuLyCurrent.DonViXuLyId : null,
                        NguoiXuLyHienTaiId = xuLyCurrent != null ? xuLyCurrent.NguoiXuLyId : null,
                        NgayNhanHienTai = xuLyCurrent != null ? xuLyCurrent.NgayNhan : null,
                        TrangThaiNghiepVuTiepNhan = xuLyCurrent != null ? xuLyCurrent.KetQuaXuLy : null,
                        TenTrangThaiNghiepVuTiepNhan = xuLyCurrent != null ? xuLyCurrent.KetQuaXuLy : null,
                        NoiDungXuLyHienTai = xuLyCurrent != null ? xuLyCurrent.NoiDungXuLy : null,
                        SoLanTraLaiHienTai = hoSo.SoLanTraLaiHienTai,
                        NgayTaoHoSo = hoSo.NgayTaoHoSo,
                        HanXuLy = hoSo.HanXuLy,
                        NgayHoanThanh = hoSo.NgayHoanThanh,
                        MoTa = hoSo.MoTa,
                        GhiChu = hoSo.GhiChu
                    }).FirstOrDefaultAsync();

                if (model != null)
                {
                    var currentUser = _authService.GetUserInfo();
                    var donViDangNhapId = currentUser?.DanhMucDonViId ?? Guid.Empty;
                    var isSSA = currentUser?.SSA ?? false;
                    model.CanXuLyBuocHienTai = model.BuocHienTaiId.HasValue &&
                                               (isSSA ||
                                                donViDangNhapId == Guid.Empty ||
                                                (model.DonViXuLyHienTaiId.HasValue && model.DonViXuLyHienTaiId.Value == donViDangNhapId));
                    model.DaNhanHoSo = model.NguoiXuLyHienTaiId.HasValue;
                    model.CanNhanHoSo = model.CanXuLyBuocHienTai && !model.NguoiXuLyHienTaiId.HasValue;
                    model.TenTrangThaiNghiepVuTiepNhan = ResolveTiepNhanNghiepVuLabel(model.TrangThaiNghiepVuTiepNhan);

                    if (string.Equals(model.LoaiBuocHienTai, "SoanThao", StringComparison.OrdinalIgnoreCase))
                    {
                        model.TenBuocHienTai = "Äang soáº¡n tháº£o";
                    }

                    if (string.Equals(model.TrangThaiNghiepVuTiepNhan, "DANG_LAY_GOP_Y", StringComparison.OrdinalIgnoreCase))
                    {
                        model.CanNhanHoSo = false;
                        model.CanXuLyBuocHienTai = false;
                        model.TenBuocHienTai = "ï¿½ang l?y ï¿½ ki?n gï¿½p ï¿½";
                    }

                    var trackingMap = await BuildTrackingMapAsync(new[] { hoSoVanBanId });
                    if (trackingMap.TryGetValue(hoSoVanBanId, out var tracking))
                    {
                        model.TienDoSummary = tracking.Summary;
                        model.CacBuocTheoDoi = tracking.Steps;
                    }

                    if (model.NgayHoanThanh.HasValue ||
                        (model.TienDoSummary.TongSoBuoc > 0 && model.TienDoSummary.SoBuocHoanThanh >= model.TienDoSummary.TongSoBuoc))
                    {
                        model.TenTrangThai = "ÄĂ£ hoĂ n thĂ nh";
                        model.MaMauTrangThai = "#28A745";
                        model.TenBuocHienTai = "ÄĂ£ hoĂ n thĂ nh";
                        model.CanXuLyBuocHienTai = false;
                    }

                    if (model.NgayHoanThanh.HasValue ||
                        (model.TienDoSummary.TongSoBuoc > 0 && model.TienDoSummary.SoBuocHoanThanh >= model.TienDoSummary.TongSoBuoc))
                    {
                        var hoanThanhQuaHan = model.TienDoSummary.SoBuocQuaHan > 0;
                        model.TenTrangThai = hoanThanhQuaHan ? "HoĂ n thĂ nh quĂ¡ háº¡n" : "HoĂ n thĂ nh Ä‘Ăºng háº¡n";
                        model.MaMauTrangThai = hoanThanhQuaHan ? "#DC3545" : "#28A745";
                        model.TenBuocHienTai = "ÄĂ£ hoĂ n thĂ nh";
                        model.CanXuLyBuocHienTai = false;
                    }

                    if (model.BuocHienTaiId.HasValue && model.LoaiBuocHienTai == "DanhGia")
                    {
                        model.BuocTraLaiOptions = await _dbContext.DanhMucChuyenBuocQuyTrinhs
                            .AsNoTracking()
                            .Where(x => x.QuyTrinhSoanThaoId == _dbContext.HoSoVanBans
                                .Where(h => h.Id == hoSoVanBanId)
                                .Select(h => h.QuyTrinhSoanThaoId)
                                .FirstOrDefault() &&
                                x.TuBuocId == model.BuocHienTaiId.Value &&
                                x.DieuKienKetQua.ToUpper().StartsWith("KHONG_DAT"))
                            .Join(
                                _dbContext.DanhMucBuocQuyTrinhs.AsNoTracking(),
                                chuyen => chuyen.DenBuocId,
                                buocQt => buocQt.Id,
                                (chuyen, buocQt) => buocQt.MaBuoc)
                            .Distinct()
                            .ToListAsync();
                    }

                    if (model.BuocHienTaiId.HasValue && model.LoaiBuocHienTai != "LayYKien" && model.LoaiBuocHienTai != "DanhGia")
                    {
                        model.KetQuaXuLyOptions = await _dbContext.DanhMucChuyenBuocQuyTrinhs
                            .AsNoTracking()
                            .Where(x => x.TuBuocId == model.BuocHienTaiId.Value)
                            .OrderByDescending(x => x.LaNhanhMacDinh)
                            .ThenBy(x => x.DieuKienKetQua)
                            .Select(x => x.DieuKienKetQua)
                            .Distinct()
                            .ToListAsync();

                        if (model.MaBuocHienTai == "BUOC_06_TRINH_THAM_QUYEN" &&
                            !model.KetQuaXuLyOptions.Contains("KHONG_DONG_Y"))
                        {
                            model.KetQuaXuLyOptions.Add("KHONG_DONG_Y");
                        }
                    }

                    model.KetQuaXuLyMacDinh = ResolveDefaultStepResult(model.MaBuocHienTai);
                    model.TieuDeXuLyBuoc = ResolveStepActionTitle(model.MaBuocHienTai, model.TenBuocHienTai);
                    model.NhanNutXuLyBuoc = ResolveStepActionButton(model.MaBuocHienTai, model.TenBuocHienTai);
                    model.CheDoLayYKienOptions = BuildCheDoLayYKienOptions();
                    model.DonViLayYKienOptions = await GetDonViOptionsAsync();
                    model.CacLayYKien = await GetLayYKienItemsAsync(hoSoVanBanId);
                    model.CacVersionDuThao = await (
                        from version in _dbContext.HoSoVanBanDuThaoVersions.AsNoTracking()
                        join donViVersion in _dbContext.DanhMucDonVis.AsNoTracking() on version.DonViTaoId equals donViVersion.Id into donViVersionJoin
                        from donViVersion in donViVersionJoin.DefaultIfEmpty()
                        where version.HoSoVanBanId == hoSoVanBanId
                        orderby version.LanVersion descending, version.NgayTaoVersion descending
                        select new HoSoVanBanDuThaoVersionItemModel
                        {
                            Id = version.Id,
                            LanVersion = version.LanVersion,
                            SoLanTraLai = version.SoLanTraLai,
                            TenVersion = version.TenVersion,
                            AttachedFileGroupId = version.AttachedFileGroupId,
                            DonViTaoId = version.DonViTaoId,
                            TenDonViTao = donViVersion != null ? donViVersion.TenDonVi : null,
                            NguoiTaoId = version.NguoiTaoId,
                            NgayTaoVersion = version.NgayTaoVersion,
                            LoaiVersion = version.LoaiVersion,
                            GhiChu = version.GhiChu
                        }).ToListAsync();
                    model.CheDoLayYKienHienTai = model.CacLayYKien.Any(x => x.DonViDuocLayYKienId.HasValue)
                        ? "GUI_DON_VI_GOP_Y"
                        : "CAP_NHAT_KET_QUA";

                    if ((model.LoaiBuocHienTai == "LayYKien" || model.TrangThaiNghiepVuTiepNhan == "DANG_LAY_GOP_Y") && currentUser != null)
                    {
                        model.CoTheTongHopLayYKien = model.CanXuLyBuocHienTai;
                        model.CoThePhanHoiLayYKien = currentUser.SSA || model.CacLayYKien.Any(x =>
                            x.DonViDuocLayYKienId.HasValue &&
                            x.DonViDuocLayYKienId.Value == currentUser.DanhMucDonViId &&
                            !string.Equals(x.TrangThaiPhanHoi, "DA_CO_Y_KIEN", StringComparison.OrdinalIgnoreCase) &&
                            !string.Equals(x.TrangThaiPhanHoi, "DA_TONG_HOP_Y_KIEN", StringComparison.OrdinalIgnoreCase));
                    }

                    model.LichSuDonDocs = await GetLichSuDonDocAsync(model.MaHoSo);
                    model.LichSuGiaHans = await GetLichSuGiaHanAsync(hoSoVanBanId);
                }

                return model == null
                    ? new CommonResponse("error", "KhĂ´ng tĂ¬m tháº¥y há»“ sÆ¡ vÄƒn báº£n!")
                    : new CommonResponse("success", "ThĂ nh cĂ´ng", model);
            }
            catch
            {
                return new CommonResponse();
            }
        }

        public async Task<CommonResponse> GetSoSanhDuThaoAsync(Guid hoSoVanBanId, Guid? sourceFileId = null, Guid? targetFileId = null)
        {
            try
            {
                var detailResponse = await GetChiTietAsync(hoSoVanBanId);
                if (detailResponse.Status == "error" || detailResponse.Data is not HoSoVanBanWorkflowDetailModel detail)
                {
                    return new CommonResponse("error", detailResponse.Message);
                }

                var currentFiles = await _dbContext.AttachedFiles
                    .AsNoTracking()
                    .Where(x => x.GroupId == hoSoVanBanId &&
                                x.TableName == "HoSoVanBanDuThao" &&
                                x.Status == "XD")
                    .Select(x => new HoSoVanBanDraftCompareFileOptionModel
                    {
                        FileId = x.Id,
                        GroupId = x.GroupId,
                        TableName = x.TableName ?? "HoSoVanBanDuThao",
                        FileName = x.FileName ?? "T?p dï¿½nh kï¿½m",
                        MoTa = x.MoTa,
                        NguonHienThi = "D? th?o hi?n t?i",
                        NhanHienThi = "D? th?o hi?n t?i | " + (x.FileName ?? "T?p dï¿½nh kï¿½m"),
                        NgayTao = x.CreatedDate,
                        FileExtension = x.FileName != null ? Path.GetExtension(x.FileName).ToLowerInvariant() : null,
                        LaDocx = x.FileName != null && string.Equals(Path.GetExtension(x.FileName), ".docx", StringComparison.OrdinalIgnoreCase)
                    })
                    .ToListAsync();

                var versionFiles = await (
                    from version in _dbContext.HoSoVanBanDuThaoVersions.AsNoTracking()
                    join file in _dbContext.AttachedFiles.AsNoTracking()
                        on version.AttachedFileGroupId equals file.GroupId
                    where version.HoSoVanBanId == hoSoVanBanId &&
                          file.TableName == "HoSoVanBanDuThaoVersion" &&
                          file.Status == "XD"
                    select new HoSoVanBanDraftCompareFileOptionModel
                    {
                        FileId = file.Id,
                        GroupId = file.GroupId,
                        TableName = file.TableName ?? "HoSoVanBanDuThaoVersion",
                        FileName = file.FileName ?? "T?p dï¿½nh kï¿½m",
                        MoTa = file.MoTa,
                        NguonHienThi = NormalizeDraftCompareSourceLabel(version.TenVersion),
                        NhanHienThi = NormalizeDraftCompareSourceLabel(version.TenVersion) + " | " + (file.FileName ?? "T?p dï¿½nh kï¿½m"),
                        NgayTao = version.NgayTaoVersion,
                        FileExtension = file.FileName != null ? Path.GetExtension(file.FileName).ToLowerInvariant() : null,
                        LaDocx = file.FileName != null && string.Equals(Path.GetExtension(file.FileName), ".docx", StringComparison.OrdinalIgnoreCase)
                    })
                    .ToListAsync();

                var fileOptions = currentFiles
                    .Concat(versionFiles)
                    .Where(x => !string.IsNullOrWhiteSpace(x.FileExtension) && DraftFileExtensions.Contains(x.FileExtension!, StringComparer.OrdinalIgnoreCase))
                    .Where(x => !string.IsNullOrWhiteSpace(x.NguonHienThi) &&
                                x.NguonHienThi.StartsWith("D? th?o l?n ", StringComparison.OrdinalIgnoreCase))
                    .OrderByDescending(x => x.NgayTao ?? DateTime.MinValue)
                    .ThenBy(x => x.NhanHienThi)
                    .ToList();

                var model = new HoSoVanBanDraftCompareModel
                {
                    HoSoVanBanId = hoSoVanBanId,
                    TenHoSo = detail.TenHoSo,
                    TenLoaiVanBan = detail.TenLoaiVanBan,
                    TenQuyTrinh = detail.TenQuyTrinh,
                    FileOptions = fileOptions
                };

                if (fileOptions.Count < 2)
                {
                    model.CanhBao = "C?n ï¿½t nh?t 2 file d? th?o .doc ho?c .docx d? th?c hi?n so sï¿½nh.";
                    return new CommonResponse("success", "Thï¿½nh cï¿½ng", model);
                }

                model.SourceFileId = sourceFileId ?? fileOptions[0].FileId;
                model.TargetFileId = targetFileId ?? fileOptions.FirstOrDefault(x => x.FileId != model.SourceFileId)?.FileId;
                model.SourceFile = fileOptions.FirstOrDefault(x => x.FileId == model.SourceFileId);
                model.TargetFile = fileOptions.FirstOrDefault(x => x.FileId == model.TargetFileId);

                if (model.SourceFile == null || model.TargetFile == null)
                {
                    model.CanhBao = "Khï¿½ng xï¿½c d?nh du?c d? 2 file d? so sï¿½nh.";
                    return new CommonResponse("success", "Thï¿½nh cï¿½ng", model);
                }

                if (!model.SourceFile.LaDocx || !model.TargetFile.LaDocx)
                {
                    model.CanhBao = "Ch?c nang so sï¿½nh n?i dung hi?n h? tr? tr?c ti?p cho file .docx. V?i file .doc, b?n v?n cï¿½ th? m?/t?i file d? d?i chi?u th? cï¿½ng.";
                    return new CommonResponse("success", "Thï¿½nh cï¿½ng", model);
                }

                var sourceEntity = await _dbContext.AttachedFiles
                    .AsNoTracking()
                    .Where(x => x.Id == model.SourceFile.FileId)
                    .Select(x => new { x.FileContent })
                    .FirstOrDefaultAsync();
                var targetEntity = await _dbContext.AttachedFiles
                    .AsNoTracking()
                    .Where(x => x.Id == model.TargetFile.FileId)
                    .Select(x => new { x.FileContent })
                    .FirstOrDefaultAsync();
                if (sourceEntity?.FileContent == null || targetEntity?.FileContent == null)
                {
                    model.CanhBao = "Khï¿½ng d?c du?c n?i dung 1 trong 2 file dï¿½ ch?n.";
                    return new CommonResponse("success", "Thï¿½nh cï¿½ng", model);
                }

                var leftLines = ExtractDocxLines(sourceEntity.FileContent);
                var rightLines = ExtractDocxLines(targetEntity.FileContent);
                model.DiffRows = BuildDraftDiffRows(leftLines, rightLines);
                model.CoTheSoSanh = model.DiffRows.Count > 0;
                model.TongSoDong = model.DiffRows.Count;
                model.SoDongGiongNhau = model.DiffRows.Count(x => x.Status == "same");
                model.SoDongThem = model.DiffRows.Count(x => x.Status == "added");
                model.SoDongXoa = model.DiffRows.Count(x => x.Status == "removed");
                model.SoDongSua = model.DiffRows.Count(x => x.Status == "changed");

                return new CommonResponse("success", "Thï¿½nh cï¿½ng", model);
            }
            catch (Exception ex)
            {
                return new CommonResponse("error", $"Khï¿½ng th? so sï¿½nh d? th?o: {ex.Message}");
            }
        }

        private static string NormalizeDraftCompareSourceLabel(string? sourceLabel)
        {
            if (string.IsNullOrWhiteSpace(sourceLabel))
            {
                return "D? th?o";
            }

            var normalized = sourceLabel.Trim();
            if (normalized.StartsWith("Phiï¿½n b?n d? th?o l?n ", StringComparison.OrdinalIgnoreCase))
            {
                return "D? th?o l?n " + normalized["Phiï¿½n b?n d? th?o l?n ".Length..];
            }

            return normalized;
        }

        public async Task<CommonResponse> GetLayYKienFormAsync(Guid hoSoVanBanId, string actionMode)
        {
            try
            {
                var detailResponse = await GetChiTietAsync(hoSoVanBanId);
                if (detailResponse.Status == "error" || detailResponse.Data is not HoSoVanBanWorkflowDetailModel detail)
                {
                    return new CommonResponse("error", detailResponse.Message);
                }

                var normalizedMode = NormalizeLayYKienActionMode(actionMode);
                var form = new HoSoVanBanLayYKienFormModel
                {
                    HoSoVanBanId = detail.Id,
                    AttachedFileGroupId = Guid.NewGuid(),
                    ActionMode = normalizedMode,
                    TenHoSo = detail.TenHoSo,
                    TenLoaiVanBan = detail.TenLoaiVanBan,
                    TenDonViSoanThao = detail.TenDonViSoanThao,
                    TenBuocHienTai = detail.TenBuocHienTai,
                    HanPhanHoi = detail.HanXuLy,
                    NgayPhanHoi = DateTime.Now,
                    NoiDungYeuCau = detail.CacLayYKien.OrderByDescending(x => x.NgayGui).FirstOrDefault()?.NoiDungYeuCau,
                    YeuCauFileDinhKem = await _dbContext.DanhMucBuocQuyTrinhs
                        .AsNoTracking()
                        .Where(x => x.Id == detail.BuocHienTaiId)
                        .Select(x => x.YeuCauFileDinhKem)
                        .FirstOrDefaultAsync(),
                    DonViLayYKienOptions = await GetDonViOptionsAsync(),
                    CacLayYKien = detail.CacLayYKien
                };

                var currentUser = _authService.GetUserInfo();
                if (normalizedMode == "PHAN_HOI_DON_VI" && currentUser != null && !currentUser.SSA)
                {
                    var row = detail.CacLayYKien
                        .Where(x => x.DonViDuocLayYKienId == currentUser.DanhMucDonViId)
                        .OrderByDescending(x => x.NgayGui)
                        .FirstOrDefault();

                    if (row == null)
                    {
                        return new CommonResponse("error", "ÄÆ¡n vá»‹ hiá»‡n táº¡i khĂ´ng cĂ³ yĂªu cáº§u gĂ³p Ă½ cho há»“ sÆ¡ nĂ y.");
                    }

                    form.DonViDuocLayYKienId = row.DonViDuocLayYKienId;
                    form.HanPhanHoi = row.HanPhanHoi;
                    form.NoiDungYeuCau = row.NoiDungYeuCau;
                    form.NoiDungPhanHoi = row.NoiDungPhanHoi;
                    form.TrangThaiPhanHoi = string.IsNullOrWhiteSpace(row.TrangThaiPhanHoi) ? "DA_CO_Y_KIEN" : row.TrangThaiPhanHoi;
                    form.GhiChu = row.GhiChu;
                    form.AttachedFileGroupId = row.AttachedFileGroupId ?? Guid.NewGuid();
                }

                if (normalizedMode == "TONG_HOP_Y_KIEN")
                {
                    form.TrangThaiPhanHoi = "DA_GAN_KET_QUA_Y_KIEN";
                }

                return new CommonResponse("success", "ThĂ nh cĂ´ng", form);
            }
            catch (Exception ex)
            {
                return new CommonResponse("error", $"KhĂ´ng thá»ƒ táº£i form láº¥y gĂ³p Ă½: {ex.Message}");
            }
        }

        private async Task<HoSoVanBan?> GetHoSoWithCurrentStepAsync(Guid hoSoVanBanId)
        {
            return await _dbContext.HoSoVanBans.FirstOrDefaultAsync(x => x.Id == hoSoVanBanId);
        }

        private async Task<List<HoSoVanBanLayYKienItemModel>> GetLayYKienItemsAsync(Guid hoSoVanBanId)
        {
            return await (
                from row in _dbContext.HoSoVanBanLayYKiens.AsNoTracking()
                join donVi in _dbContext.DanhMucDonVis.AsNoTracking() on row.DonViDuocLayYKienId equals donVi.Id into donViJoin
                from donVi in donViJoin.DefaultIfEmpty()
                where row.HoSoVanBanId == hoSoVanBanId
                orderby row.NgayGui descending, row.CreatedDate descending
                select new HoSoVanBanLayYKienItemModel
                {
                    Id = row.Id,
                    HoSoVanBanId = row.HoSoVanBanId,
                    DonViDuocLayYKienId = row.DonViDuocLayYKienId,
                    TenDonViDuocLayYKien = donVi != null ? donVi.TenDonVi : null,
                    NguoiDuocLayYKienId = row.NguoiDuocLayYKienId,
                    NoiDungYeuCau = row.NoiDungYeuCau,
                    NoiDungPhanHoi = row.NoiDungPhanHoi,
                    NgayGui = row.NgayGui,
                    HanPhanHoi = row.HanPhanHoi,
                    NgayPhanHoi = row.NgayPhanHoi,
                    TrangThaiPhanHoi = row.TrangThaiPhanHoi,
                    AttachedFileGroupId = row.AttachedFileGroupId,
                    GhiChu = row.GhiChu
                }).ToListAsync();
        }

        private async Task<List<HoSoVanBanDonDocHistoryItemModel>> GetLichSuDonDocAsync(string maHoSo)
        {
            if (string.IsNullOrWhiteSpace(maHoSo))
            {
                return new List<HoSoVanBanDonDocHistoryItemModel>();
            }

            var keyword = $"TimKiem={maHoSo}";
            var currentDonViId = _authService.GetUserInfo()?.DanhMucDonViId ?? Guid.Empty;

            var items = await (
                from notification in _dbContext.Notifications.AsNoTracking()
                join donViGui in _dbContext.DanhMucDonVis.AsNoTracking() on notification.DonViGui equals donViGui.Id into donViGuiJoin
                from donViGui in donViGuiJoin.DefaultIfEmpty()
                join donViNhan in _dbContext.DanhMucDonVis.AsNoTracking() on notification.DonViTiepNhan equals donViNhan.Id into donViNhanJoin
                from donViNhan in donViNhanJoin.DefaultIfEmpty()
                where notification.ParameterDanhSach != null
                      && notification.ParameterDanhSach.Contains(keyword)
                      && notification.NoiDung != null
                      && notification.NoiDung.Contains("[Đôn đốc tiến độ]")
                orderby notification.CreatedDate descending
                select new HoSoVanBanDonDocHistoryItemModel
                {
                    Id = notification.Id,
                    DonViGuiId = notification.DonViGui,
                    TenDonViGui = donViGui != null ? donViGui.TenDonVi : null,
                    DonViNhanId = notification.DonViTiepNhan,
                    TenDonViNhan = donViNhan != null ? donViNhan.TenDonVi : null,
                    NoiDung = notification.NoiDung ?? string.Empty,
                    CreatedDate = notification.CreatedDate,
                    DaXem = currentDonViId != Guid.Empty && notification.DonViView.Contains(currentDonViId)
                })
                .ToListAsync();

            return items;
        }

        private async Task<List<HoSoVanBanGiaHanHistoryItemModel>> GetLichSuGiaHanAsync(Guid hoSoVanBanId)
        {
            return await (
                from row in _dbContext.HoSoVanBanGiaHans.AsNoTracking()
                join user in _dbContext.Users.AsNoTracking() on row.NguoiGiaHanId equals user.Id into userJoin
                from user in userJoin.DefaultIfEmpty()
                where row.HoSoVanBanId == hoSoVanBanId
                orderby row.CreatedDate descending
                select new HoSoVanBanGiaHanHistoryItemModel
                {
                    Id = row.Id,
                    HanXuLyCu = row.HanXuLyCu,
                    HanXuLyMoi = row.HanXuLyMoi,
                    SoNgayGiaHan = row.SoNgayGiaHan,
                    LyDoGiaHan = row.LyDoGiaHan,
                    TenNguoiGiaHan = user != null ? user.Name : null,
                    CreatedDate = row.CreatedDate,
                    AttachedFileGroupId = row.AttachedFileGroupId,
                    GhiChu = row.GhiChu
                }).ToListAsync();
        }

        private async Task<DanhMucBuocQuyTrinh?> GetCurrentStepAsync(HoSoVanBan hoSo)
        {
            if (!hoSo.BuocHienTaiId.HasValue)
            {
                return null;
            }

            return await _dbContext.DanhMucBuocQuyTrinhs
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == hoSo.BuocHienTaiId.Value);
        }

        private async Task<DanhMucChuyenBuocQuyTrinh?> GetTransitionAsync(Guid quyTrinhId, Guid fromStepId, string ketQua)
        {
            var normalized = ketQua.Trim().ToUpperInvariant();

            return await _dbContext.DanhMucChuyenBuocQuyTrinhs
                .AsNoTracking()
                .Where(x => x.QuyTrinhSoanThaoId == quyTrinhId &&
                            x.TuBuocId == fromStepId &&
                            x.DieuKienKetQua.ToUpper() == normalized)
                .OrderByDescending(x => x.LaNhanhMacDinh)
                .FirstOrDefaultAsync();
        }

        private async Task<DanhMucBuocQuyTrinh?> ResolveFallbackStepAsync(Guid quyTrinhId, string? currentStepCode, string ketQua)
        {
            var normalized = ketQua.Trim().ToUpperInvariant();
            if (currentStepCode == "BUOC_02_THONG_NHAT" && normalized == "KHONG_DONG_Y")
            {
                return await _dbContext.DanhMucBuocQuyTrinhs
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.QuyTrinhSoanThaoId == quyTrinhId && x.MaBuoc == "BUOC_01_DANG_KY");
            }

            if (currentStepCode == "BUOC_06_TRINH_THAM_QUYEN" && normalized == "KHONG_DONG_Y")
            {
                return await _dbContext.DanhMucBuocQuyTrinhs
                    .AsNoTracking()
                    .Where(x => x.QuyTrinhSoanThaoId == quyTrinhId && string.Equals(x.LoaiBuoc, "SoanThao"))
                    .OrderBy(x => x.ThuTuSapXep)
                    .ThenBy(x => x.MaBuoc)
                    .FirstOrDefaultAsync();
            }

            return null;
        }

        private async Task<Guid?> GetTrangThaiIdByCodeAsync(string code)
        {
            return await _dbContext.DanhMucTrangThais
                .AsNoTracking()
                .Where(x => x.MaTrangThai == code)
                .Select(x => (Guid?)x.Id)
                .FirstOrDefaultAsync();
        }

        private async Task AdvanceWorkflowAsync(
            HoSoVanBan hoSo,
            DataAccess.Entities.Systems.User currentUser,
            DanhMucBuocQuyTrinh? nextStep,
            DateTime? hanXuLy,
            string ketQua,
            string? noiDung,
            Guid? donViTiepNhanOverrideId = null)
        {
            var dangXuLyStatusId = await GetTrangThaiIdByCodeAsync("DANG_XU_LY");
            if (nextStep == null)
            {
                hoSo.BuocHienTaiId = null;
                hoSo.NgayHoanThanh = DateTime.Now;
                hoSo.HanXuLy = null;
                hoSo.DanhMucTrangThaiId = dangXuLyStatusId;
                return;
            }

            var now = DateTime.Now;
            var stepPlan = await _dbContext.HoSoVanBanBuocThoiHans
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.HoSoVanBanId == hoSo.Id && x.BuocQuyTrinhId == nextStep.Id);

            var stepDeadline = hanXuLy ?? CalculateStepDeadline(stepPlan?.SoNgayXuLy ?? nextStep.SoNgayXuLyTieuChuan, now);

            hoSo.BuocHienTaiId = nextStep.Id;
            hoSo.HanXuLy = stepDeadline;
            hoSo.DanhMucTrangThaiId = dangXuLyStatusId;

            var donViXuLyId = await ResolveAssignedDonViXuLyIdAsync(hoSo, nextStep, currentUser.DanhMucDonViId, donViTiepNhanOverrideId);
            var lanXuLy = await _dbContext.HoSoVanBanXuLys.CountAsync(x => x.HoSoVanBanId == hoSo.Id && x.BuocQuyTrinhId == nextStep.Id) + 1;
            var xuLyMoi = new HoSoVanBanXuLy
            {
                HoSoVanBanId = hoSo.Id,
                BuocQuyTrinhId = nextStep.Id,
                LanXuLy = lanXuLy,
                DonViXuLyId = donViXuLyId,
                NguoiXuLyId = donViXuLyId == currentUser.DanhMucDonViId ? currentUser.Id : null,
                NgayNhan = now,
                HanXuLy = stepDeadline,
                DanhMucTrangThaiId = dangXuLyStatusId,
                IsCurrent = true,
                KetQuaXuLy = null,
                NoiDungXuLy = $"Chuyá»ƒn tá»« bÆ°á»›c trÆ°á»›c vá»›i káº¿t quáº£ '{ketQua}'. {(string.IsNullOrWhiteSpace(noiDung) ? string.Empty : noiDung)}".Trim(),
                GhiChu = null
            };

            _dbContext.HoSoVanBanXuLys.Add(xuLyMoi);

            await TaoThongBaoAsync(
                hoSo,
                nextStep,
                currentUser.DanhMucDonViId,
                donViXuLyId,
                $"Há»“ sÆ¡ '{hoSo.TenHoSo}' Ä‘Ă£ chuyá»ƒn sang bÆ°á»›c '{nextStep.TenBuoc}'.");
        }

        private async Task<Guid> ResolveAssignedDonViXuLyIdAsync(
            HoSoVanBan hoSo,
            DanhMucBuocQuyTrinh step,
            Guid currentDonViId,
            Guid? donViTiepNhanOverrideId = null)
        {
            if (donViTiepNhanOverrideId.HasValue && donViTiepNhanOverrideId.Value != Guid.Empty)
            {
                var overrideDonViId = await ResolveExistingDonViIdAsync(donViTiepNhanOverrideId.Value);
                if (overrideDonViId != Guid.Empty)
                {
                    return overrideDonViId;
                }
            }

            if (step.DonViTiepNhanMacDinhId.HasValue && step.DonViTiepNhanMacDinhId.Value != Guid.Empty)
            {
                var configuredDonViId = await ResolveExistingDonViIdAsync(step.DonViTiepNhanMacDinhId.Value);
                if (configuredDonViId != Guid.Empty)
                {
                    return configuredDonViId;
                }
            }

            if (step.MaBuoc == "BUOC_01_DANG_KY" ||
                string.Equals(step.LoaiBuoc, "SoanThao", StringComparison.OrdinalIgnoreCase))
            {
                return hoSo.DonViSoanThaoId;
            }

            if (currentDonViId != Guid.Empty)
            {
                return currentDonViId;
            }

            return hoSo.DonViSoanThaoId;
        }

        private async Task<Guid> ResolveExistingDonViIdAsync(Guid preferredId)
        {
            var exists = await _dbContext.DanhMucDonVis
                .AsNoTracking()
                .AnyAsync(x => x.Id == preferredId);

            return exists ? preferredId : Guid.Empty;
        }

        private async Task<DanhMucChuyenBuocQuyTrinh?> ResolveTransitionByResultsAsync(
            Guid quyTrinhSoanThaoId,
            Guid tuBuocId,
            params string[] dieuKienKetQuas)
        {
            foreach (var dieuKien in dieuKienKetQuas.Where(x => !string.IsNullOrWhiteSpace(x)))
            {
                var transition = await GetTransitionAsync(quyTrinhSoanThaoId, tuBuocId, dieuKien.Trim());
                if (transition != null)
                {
                    return transition;
                }
            }

            return null;
        }

        private static bool IsDraftTransferResult(string? ketQuaXuLy)
        {
            var normalized = ketQuaXuLy?.Trim().ToUpperInvariant();
            return normalized == "GUI_LAY_Y_KIEN" ||
                   normalized == "GUI_THAM_DINH" ||
                   normalized == "HOAN_THANH_DU_THAO";
        }

        private static int ResolveDraftVersionNumber(int soLanTraLaiHienTai)
        {
            return Math.Clamp(soLanTraLaiHienTai + 1, 1, 3);
        }

        private static string ResolveDraftVersionLabel(int draftVersionNumber)
        {
            return $"D? th?o l?n {draftVersionNumber}";
        }

        private static List<string> ExtractDocxLines(byte[] fileContent)
        {
            using var stream = new MemoryStream(fileContent);
            using var document = WordprocessingDocument.Open(stream, false);
            var body = document.MainDocumentPart?.Document?.Body;
            if (body == null)
            {
                return new List<string>();
            }

            var lines = body
                .Descendants<Paragraph>()
                .Select(p => string.Concat(p.Descendants<Text>().Select(t => t.Text)).Trim())
                .Where(text => !string.IsNullOrWhiteSpace(text))
                .ToList();

            if (lines.Count > 0)
            {
                return lines;
            }

            var fallback = body.InnerText?.Trim();
            return string.IsNullOrWhiteSpace(fallback)
                ? new List<string>()
                : fallback.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
        }

        private static List<HoSoVanBanDraftCompareRowModel> BuildDraftDiffRows(List<string> leftLines, List<string> rightLines)
        {
            var rows = new List<HoSoVanBanDraftCompareRowModel>();
            var max = Math.Max(leftLines.Count, rightLines.Count);
            for (var i = 0; i < max; i++)
            {
                var left = i < leftLines.Count ? leftLines[i] : string.Empty;
                var right = i < rightLines.Count ? rightLines[i] : string.Empty;
                var status = string.Equals(left, right, StringComparison.Ordinal)
                    ? "same"
                    : string.IsNullOrWhiteSpace(left)
                        ? "added"
                        : string.IsNullOrWhiteSpace(right)
                            ? "removed"
                            : "changed";

                rows.Add(new HoSoVanBanDraftCompareRowModel
                {
                    Index = i + 1,
                    LeftText = left,
                    RightText = right,
                    Status = status
                });

                rows[^1].LeftHtml = BuildCompareHtml(left, right, true, status);
                rows[^1].RightHtml = BuildCompareHtml(left, right, false, status);
            }

            return rows;
        }

        private static string BuildCompareHtml(string leftText, string rightText, bool isLeftSide, string status)
        {
            var current = isLeftSide ? leftText ?? string.Empty : rightText ?? string.Empty;
            var other = isLeftSide ? rightText ?? string.Empty : leftText ?? string.Empty;

            if (string.Equals(current, other, StringComparison.Ordinal))
            {
                return HtmlEncodeKeepBreaks(current);
            }

            if (string.IsNullOrEmpty(current))
            {
                return "<span class=\"compare-inline-empty\">(tr?ng)</span>";
            }

            if (status == "added")
            {
                return $"<span class=\"compare-inline-added\">{HtmlEncodeKeepBreaks(current)}</span>";
            }

            if (status == "removed")
            {
                return $"<span class=\"compare-inline-removed\">{HtmlEncodeKeepBreaks(current)}</span>";
            }

            if (status != "changed")
            {
                return HtmlEncodeKeepBreaks(current);
            }

            return BuildTokenDiffHtml(current, other);
        }

        private static string BuildTokenDiffHtml(string current, string other)
        {
            var currentTokens = TokenizeCompareText(current);
            var otherTokens = TokenizeCompareText(other);
            if (currentTokens.Count == 0)
            {
                return "<span class=\"compare-inline-empty\">(tr?ng)</span>";
            }

            var matrix = BuildLcsMatrix(currentTokens, otherTokens);
            var matched = new bool[currentTokens.Count];
            MarkMatchedTokens(currentTokens, otherTokens, matrix, 0, 0, matched);

            var builder = new StringBuilder();
            var inHighlight = false;
            for (var i = 0; i < currentTokens.Count; i++)
            {
                var token = currentTokens[i];
                var tokenHtml = HtmlEncodeKeepBreaks(token);
                if (matched[i])
                {
                    if (inHighlight)
                    {
                        builder.Append("</span>");
                        inHighlight = false;
                    }

                    builder.Append(tokenHtml);
                }
                else
                {
                    if (!inHighlight)
                    {
                        builder.Append("<span class=\"compare-inline-changed\">");
                        inHighlight = true;
                    }

                    builder.Append(tokenHtml);
                }
            }

            if (inHighlight)
            {
                builder.Append("</span>");
            }

            return builder.ToString();
        }

        private static List<string> TokenizeCompareText(string text)
        {
            var tokens = new List<string>();
            if (string.IsNullOrEmpty(text))
            {
                return tokens;
            }

            var current = new StringBuilder();
            CompareTokenType? currentType = null;
            foreach (var ch in text)
            {
                var tokenType = ResolveCompareTokenType(ch);
                if (currentType == null || currentType == tokenType)
                {
                    current.Append(ch);
                    currentType = tokenType;
                    continue;
                }

                tokens.Add(current.ToString());
                current.Clear();
                current.Append(ch);
                currentType = tokenType;
            }

            if (current.Length > 0)
            {
                tokens.Add(current.ToString());
            }

            return tokens;
        }

        private static int[,] BuildLcsMatrix(List<string> leftTokens, List<string> rightTokens)
        {
            var matrix = new int[leftTokens.Count + 1, rightTokens.Count + 1];
            for (var i = leftTokens.Count - 1; i >= 0; i--)
            {
                for (var j = rightTokens.Count - 1; j >= 0; j--)
                {
                    matrix[i, j] = string.Equals(leftTokens[i], rightTokens[j], StringComparison.Ordinal)
                        ? matrix[i + 1, j + 1] + 1
                        : Math.Max(matrix[i + 1, j], matrix[i, j + 1]);
                }
            }

            return matrix;
        }

        private static void MarkMatchedTokens(List<string> leftTokens, List<string> rightTokens, int[,] matrix, int i, int j, bool[] matched)
        {
            while (i < leftTokens.Count && j < rightTokens.Count)
            {
                if (string.Equals(leftTokens[i], rightTokens[j], StringComparison.Ordinal))
                {
                    matched[i] = true;
                    i++;
                    j++;
                }
                else if (matrix[i + 1, j] >= matrix[i, j + 1])
                {
                    i++;
                }
                else
                {
                    j++;
                }
            }
        }

        private static string HtmlEncodeKeepBreaks(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            return System.Net.WebUtility.HtmlEncode(text)
                .Replace("\r\n", "<br />")
                .Replace("\n", "<br />");
        }

        private enum CompareTokenType
        {
            Word,
            WhiteSpace,
            Punctuation
        }

        private static CompareTokenType ResolveCompareTokenType(char ch)
        {
            if (char.IsWhiteSpace(ch))
            {
                return CompareTokenType.WhiteSpace;
            }

            if (char.IsLetterOrDigit(ch))
            {
                return CompareTokenType.Word;
            }

            return CompareTokenType.Punctuation;
        }

        private async Task PromoteDraftAttachedFilesAsync(Guid hoSoVanBanId)
        {
            await _dbContext.AttachedFiles
                .Where(x => x.GroupId == hoSoVanBanId &&
                            x.TableName == "HoSoVanBanDuThao" &&
                            x.Status != "XD")
                .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.Status, "XD"));
        }

        private async Task<CommonResponse> ValidateDraftSubmissionAsync(Guid hoSoVanBanId, int requiredDraftVersionNumber)
        {
            var draftFiles = await _dbContext.AttachedFiles
                .AsNoTracking()
                .Where(x => x.GroupId == hoSoVanBanId && x.TableName == "HoSoVanBanDuThao" && x.Status == "XD")
                .Select(x => new
                {
                    x.FileName,
                    x.PhanLoaiDuThao
                })
                .ToListAsync();

            if (draftFiles.Count == 0)
            {
                return new CommonResponse("error", "Ph?i dï¿½nh kï¿½m ï¿½t nh?t 1 file d? th?o tru?c khi chuy?n th?m d?nh.");
            }

            var requiredDraftVersionLabel = ResolveDraftVersionLabel(requiredDraftVersionNumber);
            var requiredDraftFiles = draftFiles
                .Where(x => string.Equals(
                    (x.PhanLoaiDuThao ?? string.Empty).Trim(),
                    requiredDraftVersionLabel,
                    StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (requiredDraftFiles.Count == 0)
            {
                return new CommonResponse("error", $"Ph?i cï¿½ ï¿½t nh?t 1 file \"{requiredDraftVersionLabel}\" tru?c khi chuy?n xï¿½t duy?t d? th?o.");
            }

            var hasWordFile = requiredDraftFiles.Any(file =>
                !string.IsNullOrWhiteSpace(file.FileName) &&
                DraftFileExtensions.Contains(Path.GetExtension(file.FileName).ToLowerInvariant()));

            return hasWordFile
                ? new CommonResponse("success", "Thï¿½nh cï¿½ng")
                : new CommonResponse("error", $"Ph?i cï¿½ ï¿½t nh?t 1 file \"{requiredDraftVersionLabel}\" d?nh d?ng .doc ho?c .docx tru?c khi chuy?n xï¿½t duy?t d? th?o.");
        }

        private async Task<string?> ResolveDraftReturnStepCodeAsync(Guid quyTrinhSoanThaoId)
        {
            return await _dbContext.DanhMucBuocQuyTrinhs
                .AsNoTracking()
                .Where(x => x.QuyTrinhSoanThaoId == quyTrinhSoanThaoId && x.LoaiBuoc == "SoanThao")
                .OrderBy(x => x.ThuTuSapXep)
                .Select(x => x.MaBuoc)
                .FirstOrDefaultAsync();
        }

        private async Task SaveDraftVersionSnapshotAsync(
            HoSoVanBan hoSo,
            Guid? donViTaoId,
            Guid? nguoiTaoId,
            string loaiVersion,
            string? ghiChu)
        {
            var draftFiles = await _dbContext.AttachedFiles
                .AsNoTracking()
                .Where(x => x.GroupId == hoSo.Id && x.TableName == "HoSoVanBanDuThao" && x.Status == "XD")
                .ToListAsync();

            if (draftFiles.Count == 0)
            {
                return;
            }

            var nextVersion = await _dbContext.HoSoVanBanDuThaoVersions
                .Where(x => x.HoSoVanBanId == hoSo.Id)
                .MaxAsync(x => (int?)x.LanVersion) ?? 0;

            var version = new HoSoVanBanDuThaoVersion
            {
                Id = Guid.NewGuid(),
                HoSoVanBanId = hoSo.Id,
                LanVersion = nextVersion + 1,
                SoLanTraLai = hoSo.SoLanTraLaiHienTai,
                TenVersion = $"Phiï¿½n b?n d? th?o l?n {nextVersion + 1}",
                AttachedFileGroupId = Guid.NewGuid(),
                DonViTaoId = donViTaoId,
                NguoiTaoId = nguoiTaoId,
                NgayTaoVersion = DateTime.Now,
                LoaiVersion = loaiVersion,
                GhiChu = ghiChu
            };

            _dbContext.HoSoVanBanDuThaoVersions.Add(version);

            foreach (var file in draftFiles)
            {
                _dbContext.AttachedFiles.Add(new AttachedFile
                {
                    Id = Guid.NewGuid(),
                    GroupId = version.AttachedFileGroupId,
                    TableName = "HoSoVanBanDuThaoVersion",
                    SoVanBan = file.SoVanBan,
                    NgayBanHanh = file.NgayBanHanh,
                    NgayApDung = file.NgayApDung,
                    Url = file.Url,
                    FileContent = file.FileContent,
                    FileName = file.FileName,
                    ContentType = file.ContentType,
                    MoTa = file.MoTa,
                    PhanLoaiDuThao = file.PhanLoaiDuThao,
                    Status = "XD",
                    Public = file.Public
                });
            }
        }

        private async Task<DanhMucQuyTrinhSoanThao?> ResolveDraftWorkflowAsync(Guid danhMucVanBanId)
        {
            var vanBanIdToken = danhMucVanBanId.ToString();
            var loaiQuyTrinh = NormalizeWorkflowType("XayDung");
            return await _dbContext.DanhMucQuyTrinhSoanThaos
                .AsNoTracking()
                .Where(x => x.TrangThai && x.LoaiQuyTrinh == loaiQuyTrinh)
                .Where(x => _dbContext.DanhMucBuocQuyTrinhs
                    .AsNoTracking()
                    .Any(b => b.QuyTrinhSoanThaoId == x.Id &&
                              string.Equals(b.LoaiBuoc, "SoanThao")))
                .Where(x => x.DanhMucVanBanId == danhMucVanBanId ||
                            (!string.IsNullOrWhiteSpace(x.DanhMucVanBanIds) && x.DanhMucVanBanIds.Contains(vanBanIdToken)) ||
                            (!x.DanhMucVanBanId.HasValue && string.IsNullOrWhiteSpace(x.DanhMucVanBanIds)))
                .OrderBy(x => x.TenQuyTrinh)
                .FirstOrDefaultAsync();
        }

        private async Task<DanhMucBuocQuyTrinh?> ResolveDraftStartStepAsync(Guid quyTrinhSoanThaoId)
        {
            return await _dbContext.DanhMucBuocQuyTrinhs
                .AsNoTracking()
                .Where(x => x.QuyTrinhSoanThaoId == quyTrinhSoanThaoId)
                .OrderBy(x => x.ThuTuSapXep)
                .ThenBy(x => x.MaBuoc)
                .FirstOrDefaultAsync();
        }

        private async Task<HoSoVanBan?> FindExistingDraftHoSoByDangKyIdAsync(Guid hoSoDangKyId)
        {
            var sourceNote = $"{DraftSourceNotePrefix}{hoSoDangKyId}]";
            return await _dbContext.HoSoVanBans
                .AsNoTracking()
                .Where(x => x.GhiChu != null &&
                            x.GhiChu.Contains(sourceNote) &&
                            _dbContext.DanhMucQuyTrinhSoanThaos.Any(q => q.Id == x.QuyTrinhSoanThaoId && q.LoaiQuyTrinh == NormalizeWorkflowType("XayDung")))
                .OrderByDescending(x => x.CreatedDate)
                .FirstOrDefaultAsync();
        }

        private static string NormalizeWorkflowType(string? value)
        {
            var normalized = (value ?? string.Empty).Trim();
            return normalized.ToUpperInvariant() switch
            {
                "DANGKY" => "DangKy",
                "DANG_KY" => "DangKy",
                "XAYDUNG" => "XayDung",
                "XAY_DUNG" => "XayDung",
                _ => string.IsNullOrWhiteSpace(normalized) ? "XayDung" : normalized
            };
        }

        private static string BuildDraftSourceNote(Guid hoSoDangKyId, string? existingNote)
        {
            var sourceNote = $"{DraftSourceNotePrefix}{hoSoDangKyId}]";
            if (string.IsNullOrWhiteSpace(existingNote))
            {
                return sourceNote;
            }

            if (existingNote.Contains(sourceNote, StringComparison.OrdinalIgnoreCase))
            {
                return existingNote.Trim();
            }

            return $"{existingNote.Trim()} {sourceNote}";
        }

        private static bool CanDangKyReviewUserAccess(
            DataAccess.Entities.Systems.User currentUser,
            HoSoVanBan hoSo,
            HoSoVanBanXuLy processing)
        {
            if (currentUser.SSA)
            {
                return true;
            }

            if (currentUser.DanhMucDonViId == Guid.Empty)
            {
                return true;
            }

            return currentUser.DanhMucDonViId == hoSo.DonViSoanThaoId ||
                   currentUser.DanhMucDonViId == processing.DonViXuLyId;
        }

        private static bool CanCurrentUserXuLy(DataAccess.Entities.Systems.User currentUser, HoSoVanBanXuLy? currentProcessing)
        {
            if (currentUser.SSA)
            {
                return true;
            }

            if (currentProcessing == null)
            {
                return false;
            }

            return currentUser.DanhMucDonViId == Guid.Empty || currentProcessing.DonViXuLyId == currentUser.DanhMucDonViId;
        }

        private static bool CanEditDangKyHoSo(
            DataAccess.Entities.Systems.User currentUser,
            HoSoVanBan hoSo,
            DanhMucBuocQuyTrinh? currentStep,
            HoSoVanBanXuLy? currentProcessing)
        {
            if (currentStep == null || currentStep.MaBuoc != "BUOC_01_DANG_KY")
            {
                return false;
            }

            if (!CanCurrentUserXuLy(currentUser, currentProcessing))
            {
                return false;
            }

            return currentUser.SSA ||
                   currentUser.DanhMucDonViId == Guid.Empty ||
                   currentUser.DanhMucDonViId == hoSo.DonViSoanThaoId;
        }

        private async Task<DateTime> ResolveNextStepDeadlineAsync(Guid hoSoVanBanId, DanhMucBuocQuyTrinh? nextStep)
        {
            var startDate = DateTime.Today;
            if (nextStep == null)
            {
                return startDate.AddDays(7);
            }

            var stepPlan = await _dbContext.HoSoVanBanBuocThoiHans
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.HoSoVanBanId == hoSoVanBanId && x.BuocQuyTrinhId == nextStep.Id);

            var soNgayXuLy = stepPlan?.SoNgayXuLy ?? nextStep.SoNgayXuLyTieuChuan;
            return soNgayXuLy.HasValue && soNgayXuLy.Value > 0
                ? startDate.AddDays(soNgayXuLy.Value)
                : startDate.AddDays(7);
        }

        private static DateTime? CalculateStepDeadline(int? soNgayXuLy, DateTime startDate)
        {
            if (!soNgayXuLy.HasValue || soNgayXuLy.Value <= 0)
            {
                return null;
            }

            return startDate.AddDays(soNgayXuLy.Value);
        }

        private static string NormalizeLayYKienActionMode(string? actionMode)
        {
            return actionMode?.Trim().ToUpperInvariant() switch
            {
                "GUI_DON_VI_GOP_Y" => "GUI_DON_VI_GOP_Y",
                "PHAN_HOI_DON_VI" => "PHAN_HOI_DON_VI",
                "TONG_HOP_Y_KIEN" => "TONG_HOP_Y_KIEN",
                _ => "CAP_NHAT_KET_QUA"
            };
        }

        private static List<SelectOptionModel> BuildCheDoLayYKienOptions()
        {
            return new List<SelectOptionModel>
            {
                new()
                {
                    Value = "CAP_NHAT_KET_QUA",
                    Text = "ÄÆ¡n vá»‹ soáº¡n tháº£o tá»± cáº­p nháº­t káº¿t quáº£ gĂ³p Ă½"
                },
                new()
                {
                    Value = "GUI_DON_VI_GOP_Y",
                    Text = "Gá»­i láº¥y gĂ³p Ă½ Ä‘áº¿n tá»«ng Ä‘Æ¡n vá»‹ rá»“i tá»•ng há»£p láº¡i"
                }
            };
        }

        private static string ResolveDefaultStepResult(string? maBuocHienTai)
        {
            return maBuocHienTai switch
            {
                "BUOC_01_DANG_KY" => "HOAN_THANH_DANG_KY",
                "BUOC_02_THONG_NHAT" => "DONG_Y",
                "SOAN_THAO" => "HOAN_THANH_DU_THAO",
                "BUOC_04_LAY_Y_KIEN" => "DA_GAN_KET_QUA_Y_KIEN",
                "BUOC_06_TRINH_THAM_QUYEN" => "TRINH_THANH_CONG",
                "BUOC_07_BAN_HANH" => "BAN_HANH",
                _ => "HOAN_THANH"
            };
        }

        private static string ResolveStepActionTitle(string? maBuocHienTai, string? tenBuocHienTai)
        {
            return maBuocHienTai switch
            {
                "BUOC_01_DANG_KY" => "Chuyá»ƒn vÄƒn báº£n Ä‘áº¿n Ä‘Æ¡n vá»‹ tiáº¿p nháº­n",
                "BUOC_02_THONG_NHAT" => "Pháº£n há»“i VP UBND vá» Ä‘Äƒng kĂ½ xĂ¢y dá»±ng",
                "BUOC_06_TRINH_THAM_QUYEN" => "Pháº£n há»“i káº¿t quáº£ trĂ¬nh cÆ¡ quan cĂ³ tháº©m quyá»n",
                _ => $"Cáº­p nháº­t bÆ°á»›c {(string.IsNullOrWhiteSpace(tenBuocHienTai) ? "hiá»‡n táº¡i" : tenBuocHienTai)}"
            };
        }

        private static string ResolveStepActionButton(string? maBuocHienTai, string? tenBuocHienTai)
        {
            return maBuocHienTai switch
            {
                "BUOC_01_DANG_KY" => "Chuyá»ƒn Ä‘áº¿n bÆ°á»›c 2",
                "BUOC_02_THONG_NHAT" => "Gá»­i káº¿t quáº£ phĂª duyá»‡t Ä‘Äƒng kĂ½",
                "BUOC_06_TRINH_THAM_QUYEN" => "Gá»­i káº¿t quáº£ phĂª duyá»‡t vÄƒn báº£n",
                _ => $"HoĂ n thĂ nh {(string.IsNullOrWhiteSpace(tenBuocHienTai) ? "bÆ°á»›c hiá»‡n táº¡i" : tenBuocHienTai)}"
            };
        }

        private static string? BuildXuLyGhiChu(string? ghiChu, Guid? attachedFileGroupId)
        {
            var normalizedNote = ghiChu?.Trim();
            if (!attachedFileGroupId.HasValue || attachedFileGroupId.Value == Guid.Empty)
            {
                return string.IsNullOrWhiteSpace(normalizedNote) ? null : normalizedNote;
            }

            var attachedFileNote = $"TĂ i liá»‡u Ä‘Ă­nh kĂ¨m group: {attachedFileGroupId.Value}";
            if (string.IsNullOrWhiteSpace(normalizedNote))
            {
                return attachedFileNote;
            }

            return $"{normalizedNote} | {attachedFileNote}";
        }

        private async Task DongBoHoSoDuThaoSauLayYKienAsync(HoSoVanBan hoSo, HoSoVanBanLayYKienStepModel request)
        {
            var duThao = await _dbContext.HoSoVanBanDuThaos.FirstOrDefaultAsync(x => x.HoSoVanBanId == hoSo.Id);
            if (duThao == null)
            {
                duThao = new HoSoVanBanDuThao
                {
                    HoSoVanBanId = hoSo.Id,
                    TenDuThao = hoSo.TenHoSo,
                    SoLanDuThao = 1,
                    TrangThaiDuThao = "DA_HOAN_THANH_DU_THAO",
                    KetQuaThucHien = "DA_HOAN_THANH_DU_THAO",
                    DaDuDieuKienChuyenBuoc = true
                };
                _dbContext.HoSoVanBanDuThaos.Add(duThao);
            }

            duThao.TenDuThao = string.IsNullOrWhiteSpace(duThao.TenDuThao) ? hoSo.TenHoSo : duThao.TenDuThao;
            duThao.NgayCapNhatDuThao ??= request.NgayPhanHoi ?? DateTime.Now;
            duThao.NgayBaoCaoKetQua ??= request.NgayPhanHoi ?? DateTime.Now;
            duThao.TrangThaiDuThao = "DA_HOAN_THANH_DU_THAO";
            duThao.KetQuaThucHien = "DA_HOAN_THANH_DU_THAO";
            duThao.DaDuDieuKienChuyenBuoc = true;
            duThao.NoiDungTomTat ??= request.NoiDungPhanHoi?.Trim();
            duThao.NoiDungBaoCao ??= request.NoiDungPhanHoi?.Trim();
            duThao.GhiChu = string.IsNullOrWhiteSpace(duThao.GhiChu) ? request.GhiChu?.Trim() : duThao.GhiChu;

            await SaoChepFileGopYSangDuThaoAsync(hoSo.Id, request);
        }

        private async Task SaoChepFileGopYSangDuThaoAsync(Guid hoSoVanBanId, HoSoVanBanLayYKienStepModel request)
        {
            const string draftTableName = "HoSoVanBanDuThao";

            var sourceGroupIds = await _dbContext.HoSoVanBanLayYKiens
                .AsNoTracking()
                .Where(x => x.HoSoVanBanId == hoSoVanBanId && x.AttachedFileGroupId.HasValue && x.AttachedFileGroupId.Value != Guid.Empty)
                .Select(x => x.AttachedFileGroupId!.Value)
                .Distinct()
                .ToListAsync();

            sourceGroupIds.AddRange(request.CacLayYKien
                .Where(x => x.AttachedFileGroupId.HasValue && x.AttachedFileGroupId.Value != Guid.Empty)
                .Select(x => x.AttachedFileGroupId!.Value));

            if (request.AttachedFileGroupId.HasValue && request.AttachedFileGroupId.Value != Guid.Empty)
            {
                sourceGroupIds.Add(request.AttachedFileGroupId.Value);
            }

            sourceGroupIds = sourceGroupIds
                .Where(x => x != Guid.Empty && x != hoSoVanBanId)
                .Distinct()
                .ToList();

            if (sourceGroupIds.Count == 0)
            {
                return;
            }

            var sourceFiles = await _dbContext.AttachedFiles
                .AsNoTracking()
                .Where(x => sourceGroupIds.Contains(x.GroupId))
                .ToListAsync();

            if (sourceFiles.Count == 0)
            {
                return;
            }

            var existingDraftFiles = await _dbContext.AttachedFiles
                .AsNoTracking()
                .Where(x => x.GroupId == hoSoVanBanId && x.TableName == draftTableName)
                .Select(x => new { x.FileName, x.Url, x.ContentType, x.MoTa })
                .ToListAsync();

            var existingKeys = existingDraftFiles
                .Select(x => $"{x.FileName}|{x.Url}|{x.ContentType}|{x.MoTa}")
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            foreach (var file in sourceFiles)
            {
                var key = $"{file.FileName}|{file.Url}|{file.ContentType}|{file.MoTa}";
                if (existingKeys.Contains(key))
                {
                    continue;
                }

                _dbContext.AttachedFiles.Add(new AttachedFile
                {
                    GroupId = hoSoVanBanId,
                    TableName = draftTableName,
                    SoVanBan = file.SoVanBan,
                    NgayBanHanh = file.NgayBanHanh,
                    NgayApDung = file.NgayApDung,
                    Url = file.Url,
                    FileContent = file.FileContent == null ? null : file.FileContent.ToArray(),
                    FileName = file.FileName,
                    ContentType = file.ContentType,
                    MoTa = file.MoTa,
                    PhanLoaiDuThao = file.PhanLoaiDuThao,
                    Status = file.Status,
                    Public = file.Public
                });

                existingKeys.Add(key);
            }
        }

        private static string NormalizeTiepNhanNghiepVuCode(string? actionType)
        {
            return actionType?.Trim().ToUpperInvariant() switch
            {
                "NHAN_HO_SO" => "NHAN_HO_SO",
                "NHAN_VA_CHUYEN_PHE_DUYET" => "NHAN_VA_CHUYEN_PHE_DUYET",
                "PHE_DUYET_HO_SO" => "PHE_DUYET_HO_SO",
                "TRA_LAI_HO_SO" => "TRA_LAI_HO_SO",
                "CHUYEN_PHE_DUYET" => "CHUYEN_PHE_DUYET",
                "CHUYEN_BAN_HANH" => "CHUYEN_BAN_HANH",
                "CHUYEN_XET_DUYET_DANH_GIA" => "CHUYEN_XET_DUYET_DANH_GIA",
                "DANG_LAY_GOP_Y" => "DANG_LAY_GOP_Y",
                _ => string.Empty
            };
        }

        private static string ResolveTiepNhanNghiepVuLabel(string? actionType)
        {
            return NormalizeTiepNhanNghiepVuCode(actionType) switch
            {
                "NHAN_HO_SO" => "ÄĂ£ nháº­n há»“ sÆ¡",
                "NHAN_VA_CHUYEN_PHE_DUYET" => "ÄĂ£ nháº­n vĂ  chuyá»ƒn phĂª duyá»‡t",
                "PHE_DUYET_HO_SO" => "ÄĂ£ phĂª duyá»‡t há»“ sÆ¡",
                "TRA_LAI_HO_SO" => "ÄĂ£ tráº£ láº¡i há»“ sÆ¡",
                "CHUYEN_PHE_DUYET" => "ÄĂ£ chuyá»ƒn phĂª duyá»‡t",
                "CHUYEN_BAN_HANH" => "ÄĂ£ chuyá»ƒn ban hĂ nh",
                "CHUYEN_XET_DUYET_DANH_GIA" => "ÄĂ£ chuyá»ƒn xá»­ lĂ½ Ä‘Ă¡nh giĂ¡",
                "DANG_LAY_GOP_Y" => "Äang láº¥y gĂ³p Ă½",
                _ => string.Empty
            };
        }

        private static string BuildTiepNhanNghiepVuNote(string actionType)
        {
            return ResolveTiepNhanNghiepVuLabel(actionType);
        }

        private static string ResolveTiepNhanNghiepVuSuccessMessage(string actionType)
        {
            return NormalizeTiepNhanNghiepVuCode(actionType) switch
            {
                "NHAN_HO_SO" => "ÄĂ£ nháº­n há»“ sÆ¡ thĂ nh cĂ´ng.",
                "NHAN_VA_CHUYEN_PHE_DUYET" => "ÄĂ£ nháº­n há»“ sÆ¡ vĂ  ghi nháº­n chuyá»ƒn phĂª duyá»‡t.",
                "PHE_DUYET_HO_SO" => "ÄĂ£ ghi nháº­n phĂª duyá»‡t há»“ sÆ¡.",
                "TRA_LAI_HO_SO" => "ÄĂ£ ghi nháº­n tráº£ láº¡i há»“ sÆ¡.",
                "CHUYEN_PHE_DUYET" => "ÄĂ£ ghi nháº­n chuyá»ƒn phĂª duyá»‡t.",
                "CHUYEN_BAN_HANH" => "ÄĂ£ ghi nháº­n chuyá»ƒn ban hĂ nh.",
                "CHUYEN_XET_DUYET_DANH_GIA" => "ÄĂ£ chuyá»ƒn há»“ sÆ¡ sang mĂ n hĂ¬nh xá»­ lĂ½ Ä‘Ă¡nh giĂ¡.",
                "DANG_LAY_GOP_Y" => "ÄĂ£ chuyá»ƒn sang trï¿½ng thĂ¡i láº¥y gĂ³p Ă½.",
                _ => "Cáº­p nháº­t tráº¡ng thĂ¡i nghiá»‡p vá»¥ thĂ nh cĂ´ng."
            };
        }

        private async Task<List<HoSoVanBanBuocThoiHanEditModel>> BuildRequestedStepDeadlinePlansAsync(
            Guid quyTrinhSoanThaoId,
            List<HoSoVanBanBuocThoiHanEditModel> requestedPlans)
        {
            var workflowSteps = await _dbContext.DanhMucBuocQuyTrinhs
                .AsNoTracking()
                .Where(x => x.QuyTrinhSoanThaoId == quyTrinhSoanThaoId)
                .OrderBy(x => x.ThuTuSapXep)
                .ThenBy(x => x.MaBuoc)
                .ToListAsync();

            var requestMap = requestedPlans.ToDictionary(x => x.BuocQuyTrinhId, x => x);
            return workflowSteps.Select(step =>
            {
                requestMap.TryGetValue(step.Id, out var requested);
                return new HoSoVanBanBuocThoiHanEditModel
                {
                    BuocQuyTrinhId = step.Id,
                    MaBuoc = step.MaBuoc,
                    TenBuoc = step.TenBuoc,
                    ThuTuSapXep = step.ThuTuSapXep,
                    SoNgayXuLy = requested?.SoNgayXuLy ?? step.SoNgayXuLyTieuChuan,
                    SoNgayCanhBaoSapHan = requested?.SoNgayCanhBaoSapHan ?? step.SoNgayCanhBaoSapHan,
                    GhiChu = requested?.GhiChu
                };
            }).ToList();
        }

        private async Task<Dictionary<Guid, HoSoVanBanTrackingAggregate>> BuildTrackingMapAsync(IEnumerable<Guid> hoSoIds)
        {
            var ids = hoSoIds.Distinct().ToList();
            if (ids.Count == 0)
            {
                return new Dictionary<Guid, HoSoVanBanTrackingAggregate>();
            }

            var hoSos = await _dbContext.HoSoVanBans
                .AsNoTracking()
                .Where(x => ids.Contains(x.Id))
                .Select(x => new { x.Id, x.QuyTrinhSoanThaoId })
                .ToListAsync();

            var quyTrinhIds = hoSos.Select(x => x.QuyTrinhSoanThaoId).Distinct().ToList();
            var steps = await _dbContext.DanhMucBuocQuyTrinhs
                .AsNoTracking()
                .Where(x => quyTrinhIds.Contains(x.QuyTrinhSoanThaoId))
                .OrderBy(x => x.ThuTuSapXep)
                .ThenBy(x => x.MaBuoc)
                .ToListAsync();

            var processings = await _dbContext.HoSoVanBanXuLys
                .AsNoTracking()
                .Where(x => ids.Contains(x.HoSoVanBanId))
                .OrderBy(x => x.LanXuLy)
                .ThenBy(x => x.NgayNhan)
                .ToListAsync();

            var deadlinePlans = await _dbContext.HoSoVanBanBuocThoiHans
                .AsNoTracking()
                .Where(x => ids.Contains(x.HoSoVanBanId))
                .ToListAsync();

            var statusColors = await GetStatusColorMapAsync();
            var result = new Dictionary<Guid, HoSoVanBanTrackingAggregate>();
            var now = DateTime.Now;

            foreach (var hoSo in hoSos)
            {
                var workflowSteps = steps
                    .Where(x => x.QuyTrinhSoanThaoId == hoSo.QuyTrinhSoanThaoId)
                    .OrderBy(x => x.ThuTuSapXep)
                    .ThenBy(x => x.MaBuoc)
                    .ToList();

                var processingMap = processings
                    .Where(x => x.HoSoVanBanId == hoSo.Id)
                    .GroupBy(x => x.BuocQuyTrinhId)
                    .ToDictionary(
                        x => x.Key,
                        x => x.OrderByDescending(y => y.LanXuLy)
                              .ThenByDescending(y => y.NgayNhan)
                              .First());

                var deadlinePlanMap = deadlinePlans
                    .Where(x => x.HoSoVanBanId == hoSo.Id)
                    .ToDictionary(x => x.BuocQuyTrinhId, x => x);

                var trackingSteps = new List<HoSoVanBanBuocTheoDoiModel>();

                foreach (var step in workflowSteps)
                {
                    processingMap.TryGetValue(step.Id, out var latestProcessing);
                    deadlinePlanMap.TryGetValue(step.Id, out var deadlinePlan);
                    trackingSteps.Add(BuildTrackingStep(step, deadlinePlan, latestProcessing, statusColors, now));
                }

                var summary = new HoSoVanBanTienDoSummaryModel
                {
                    TongSoBuoc = trackingSteps.Count,
                    SoBuocHoanThanh = trackingSteps.Count(x => x.MaTrangThaiTheoDoi is "HOAN_THANH_DUNG_HAN" or "HOAN_THANH_QUA_HAN"),
                    SoBuocDungHan = trackingSteps.Count(x => x.MaTrangThaiTheoDoi == "HOAN_THANH_DUNG_HAN"),
                    SoBuocQuaHan = trackingSteps.Count(x => x.MaTrangThaiTheoDoi is "QUA_HAN" or "HOAN_THANH_QUA_HAN"),
                    SoBuocChuaThucHien = trackingSteps.Count(x => x.MaTrangThaiTheoDoi == "CHUA_THUC_HIEN"),
                    SoBuocDangXuLy = trackingSteps.Count(x => x.MaTrangThaiTheoDoi is "DANG_XU_LY" or "SAP_DEN_HAN" or "QUA_HAN")
                };
                summary.TyLeHoanThanh = summary.TongSoBuoc == 0
                    ? 0
                    : Math.Round((decimal)summary.SoBuocHoanThanh * 100 / summary.TongSoBuoc, 0, MidpointRounding.AwayFromZero);

                result[hoSo.Id] = new HoSoVanBanTrackingAggregate
                {
                    Summary = summary,
                    Steps = trackingSteps
                };
            }

            return result;
        }

        private static HoSoVanBanBuocTheoDoiModel BuildTrackingStep(
            DanhMucBuocQuyTrinh step,
            HoSoVanBanBuocThoiHan? deadlinePlan,
            HoSoVanBanXuLy? processing,
            Dictionary<string, string> statusColors,
            DateTime now)
        {
            var model = new HoSoVanBanBuocTheoDoiModel
            {
                BuocId = step.Id,
                MaBuoc = step.MaBuoc,
                TenBuoc = step.TenBuoc,
                ThuTuSapXep = step.ThuTuSapXep,
                LoaiBuoc = step.LoaiBuoc,
                SoNgayXuLyTieuChuan = deadlinePlan?.SoNgayXuLy ?? step.SoNgayXuLyTieuChuan,
                SoNgayCanhBaoSapHan = deadlinePlan?.SoNgayCanhBaoSapHan ?? step.SoNgayCanhBaoSapHan
            };

            if (processing == null)
            {
                model.MaTrangThaiTheoDoi = "CHUA_THUC_HIEN";
                model.TenTrangThaiTheoDoi = "ChÆ°a thá»±c hiá»‡n";
                model.MaMauTrangThaiTheoDoi = "#CED4DA";
                model.GhiChuTheoDoi = "BÆ°á»›c nĂ y chÆ°a phĂ¡t sinh xá»­ lĂ½.";
                return model;
            }

            model.LanXuLy = processing.LanXuLy;
            model.NgayNhan = processing.NgayNhan;
            model.HanXuLy = processing.HanXuLy;
            model.NgayXuLy = processing.NgayXuLy;
            model.KetQuaXuLy = processing.KetQuaXuLy;
            model.NoiDungXuLy = processing.NoiDungXuLy;
            model.IsCurrent = processing.IsCurrent;

            var mauDangXuLy = GetColor(statusColors, "DANG_XU_LY", "#28A745");
            var mauQuaHan = GetColor(statusColors, "QUA_HAN", "#DC3545");
            var mauSapHan = GetColor(statusColors, "SAP_DEN_HAN", "#FFC107");

            if (processing.IsCurrent)
            {
                if (processing.HanXuLy.HasValue && now > processing.HanXuLy.Value)
                {
                    model.MaTrangThaiTheoDoi = "QUA_HAN";
                    model.TenTrangThaiTheoDoi = "Äang xá»­ lĂ½ quĂ¡ háº¡n";
                    model.MaMauTrangThaiTheoDoi = mauQuaHan;
                    model.SoNgayTre = (int)Math.Ceiling((now - processing.HanXuLy.Value).TotalDays);
                    model.GhiChuTheoDoi = $"BÆ°á»›c Ä‘ang xá»­ lĂ½ vĂ  Ä‘Ă£ quĂ¡ háº¡n {Math.Max(model.SoNgayTre ?? 0, 0)} ngĂ y.";
                    return model;
                }

                var soNgayCanhBao = model.SoNgayCanhBaoSapHan.GetValueOrDefault(0);
                if (processing.HanXuLy.HasValue &&
                    soNgayCanhBao > 0 &&
                    now >= processing.HanXuLy.Value.AddDays(-soNgayCanhBao))
                {
                    model.MaTrangThaiTheoDoi = "SAP_DEN_HAN";
                    model.TenTrangThaiTheoDoi = "Sáº¯p Ä‘áº¿n háº¡n";
                    model.MaMauTrangThaiTheoDoi = mauSapHan;
                    model.GhiChuTheoDoi = "BÆ°á»›c Ä‘ang xá»­ lĂ½ vĂ  Ä‘Ă£ Ä‘áº¿n ngÆ°á»¡ng cáº£nh bĂ¡o sáº¯p háº¡n.";
                    return model;
                }

                model.MaTrangThaiTheoDoi = "DANG_XU_LY";
                model.TenTrangThaiTheoDoi = "Äang xá»­ lĂ½";
                model.MaMauTrangThaiTheoDoi = mauDangXuLy;
                model.GhiChuTheoDoi = "BÆ°á»›c Ä‘ang Ä‘Æ°á»£c thá»±c hiá»‡n.";
                return model;
            }

            if (processing.NgayXuLy.HasValue)
            {
                if (processing.HanXuLy.HasValue && processing.NgayXuLy.Value > processing.HanXuLy.Value)
                {
                    model.MaTrangThaiTheoDoi = "HOAN_THANH_QUA_HAN";
                    model.TenTrangThaiTheoDoi = "HoĂ n thĂ nh quĂ¡ háº¡n";
                    model.MaMauTrangThaiTheoDoi = mauQuaHan;
                    model.SoNgayTre = (int)Math.Ceiling((processing.NgayXuLy.Value - processing.HanXuLy.Value).TotalDays);
                    model.GhiChuTheoDoi = $"BÆ°á»›c Ä‘Ă£ hoĂ n thĂ nh nhÆ°ng trá»… {Math.Max(model.SoNgayTre ?? 0, 0)} ngĂ y.";
                    return model;
                }

                model.MaTrangThaiTheoDoi = "HOAN_THANH_DUNG_HAN";
                model.TenTrangThaiTheoDoi = "HoĂ n thĂ nh Ä‘Ăºng háº¡n";
                model.MaMauTrangThaiTheoDoi = mauDangXuLy;
                model.GhiChuTheoDoi = "BÆ°á»›c Ä‘Ă£ hoĂ n thĂ nh trong háº¡n.";
                return model;
            }

            model.MaTrangThaiTheoDoi = "CHUA_THUC_HIEN";
            model.TenTrangThaiTheoDoi = "ChÆ°a thá»±c hiá»‡n";
            model.MaMauTrangThaiTheoDoi = "#CED4DA";
            model.GhiChuTheoDoi = "BÆ°á»›c chÆ°a cĂ³ káº¿t quáº£ xá»­ lĂ½.";
            return model;
        }

        private static void ResolveAlertInfo(HoSoVanBanListItemModel item, HoSoVanBanBuocTheoDoiModel currentStep)
        {
            item.GhiChuCanhBao = currentStep.GhiChuTheoDoi;

            if (item.SoLanTraLaiHienTai >= 2)
            {
                item.MucCanhBao = "TRA_LAI_NHIEU";
                item.TenMucCanhBao = "Trả lại nhiều lần";
                item.MaMauCanhBao = "#6F42C1";
                return;
            }

            if (currentStep.MaTrangThaiTheoDoi == "QUA_HAN")
            {
                item.MucCanhBao = "QUA_HAN";
                item.TenMucCanhBao = "Quá hạn";
                item.MaMauCanhBao = "#DC3545";
                item.SoNgayQuaHanHienTai = Math.Max(currentStep.SoNgayTre ?? 0, 0);
                return;
            }

            if (currentStep.MaTrangThaiTheoDoi == "SAP_DEN_HAN")
            {
                item.MucCanhBao = "SAP_DEN_HAN";
                item.TenMucCanhBao = "Sắp đến hạn";
                item.MaMauCanhBao = "#FFC107";
                if (currentStep.HanXuLy.HasValue)
                {
                    item.SoNgayConLai = Math.Max((int)Math.Ceiling((currentStep.HanXuLy.Value.Date - DateTime.Today).TotalDays), 0);
                }
                return;
            }

            item.MucCanhBao = "BINH_THUONG";
            item.TenMucCanhBao = "Bình thường";
            item.MaMauCanhBao = "#28A745";
            if (currentStep.HanXuLy.HasValue && currentStep.MaTrangThaiTheoDoi == "DANG_XU_LY")
            {
                item.SoNgayConLai = Math.Max((int)Math.Ceiling((currentStep.HanXuLy.Value.Date - DateTime.Today).TotalDays), 0);
            }
        }

        private async Task<Dictionary<string, string>> GetStatusColorMapAsync()
        {
            return await _dbContext.DanhMucTrangThais
                .AsNoTracking()
                .ToDictionaryAsync(x => x.MaTrangThai, x => x.MaMauHex);
        }

        private static string GetColor(Dictionary<string, string> colors, string code, string defaultColor)
        {
            return colors.TryGetValue(code, out var color) && !string.IsNullOrWhiteSpace(color)
                ? color
                : defaultColor;
        }

        private async Task TaoThongBaoAsync(HoSoVanBan hoSo, DanhMucBuocQuyTrinh step, Guid donViGui, Guid donViTiepNhan, string noiDung)
        {
            if (donViGui == Guid.Empty || donViTiepNhan == Guid.Empty || donViGui == donViTiepNhan)
            {
                return;
            }

            var config = BuildNotificationNavigation(step.MaBuoc, hoSo.MaHoSo);
            var thongBao = new Notification
            {
                DonViGui = donViGui,
                DonViTiepNhan = donViTiepNhan,
                DonViDongChuyen = string.Empty,
                NoiDung = noiDung,
                ControllerNameDanhSach = config.ControllerNameDanhSach,
                ActionNameDanhSach = config.ActionNameDanhSach,
                ParameterDanhSach = config.ParameterDanhSach,
                ControllerNameXetDuyet = config.ControllerNameXetDuyet,
                ActionNameXetDuyet = config.ActionNameXetDuyet,
                ParameterXetDuyet = config.ParameterXetDuyet,
                DonViView = new List<Guid>()
            };

            await _notificationService.StoreAsync(thongBao);
        }

        private static NotificationNavigationConfig BuildNotificationNavigation(string? maBuoc, string maHoSo)
        {
            var filter = $"TimKiem={maHoSo}";

            return maBuoc switch
            {
                "BUOC_02_THONG_NHAT" => new NotificationNavigationConfig
                {
                    ControllerNameDanhSach = "DangKyVanBan",
                    ActionNameDanhSach = "Index",
                    ParameterDanhSach = filter,
                    ControllerNameXetDuyet = "XetDuyetDangKy",
                    ActionNameXetDuyet = "Index",
                    ParameterXetDuyet = filter
                },
                "BUOC_04_LAY_Y_KIEN" => new NotificationNavigationConfig
                {
                    ControllerNameDanhSach = "DuThaoVanBan",
                    ActionNameDanhSach = "Index",
                    ParameterDanhSach = filter,
                    ControllerNameXetDuyet = "XetDuyetDuThao",
                    ActionNameXetDuyet = "Index",
                    ParameterXetDuyet = filter
                },
                "BUOC_06_TRINH_THAM_QUYEN" => new NotificationNavigationConfig
                {
                    ControllerNameDanhSach = "HoSoVanBan",
                    ActionNameDanhSach = "Index",
                    ParameterDanhSach = filter,
                    ControllerNameXetDuyet = "XetDuyetVanBan",
                    ActionNameXetDuyet = "Index",
                    ParameterXetDuyet = filter
                },
                _ => new NotificationNavigationConfig
                {
                    ControllerNameDanhSach = "HoSoVanBan",
                    ActionNameDanhSach = "Index",
                    ParameterDanhSach = filter,
                    ControllerNameXetDuyet = "HoSoVanBan",
                    ActionNameXetDuyet = "Index",
                    ParameterXetDuyet = filter
                }
            };
        }

        private async Task ApplyTieuChiDiemAsync(HoSoVanBan hoSo)
        {
            var proposal = await BuildChamDiemProposalAsync(hoSo.Id);
            if (proposal == null)
            {
                ClearScore(hoSo);
                return;
            }
            hoSo.TongThoiGianXayDungNgay = proposal.TongThoiGianXayDungNgay;
            hoSo.TongThoiGianQuyDinhNgay = proposal.TongThoiGianQuyDinhNgay;
            hoSo.TyLeThoiGianXayDung = proposal.TyLeThoiGianXayDung;
            hoSo.DiemTienDoXayDung = proposal.ChiTiets.FirstOrDefault(x => x.LoaiTieuChi == "THOI_GIAN")?.DiemDeXuat;
            hoSo.DiemChatLuongVanBan = proposal.ChiTiets.FirstOrDefault(x => x.LoaiTieuChi == "CHAT_LUONG")?.DiemDeXuat;
            hoSo.TongDiemDanhGia = proposal.TongDiem;
            hoSo.XepLoaiDanhGia = ResolveXepLoaiTongDiem(proposal.TongDiem);
            hoSo.NgayChamDiem = DateTime.Now;
        }

        private async Task<ChamDiemProposal?> BuildChamDiemProposalAsync(Guid hoSoVanBanId)
        {
            var hoSo = await _dbContext.HoSoVanBans.AsNoTracking().FirstOrDefaultAsync(x => x.Id == hoSoVanBanId);
            if (hoSo == null)
            {
                return null;
            }

            var tieuChis = await _dbContext.DanhMucTieuChiDiems
                .AsNoTracking()
                .Where(x => x.TrangThai)
                .OrderBy(x => x.ThuTuSapXep)
                .ToListAsync();
            if (tieuChis.Count == 0)
            {
                return null;
            }

            var mucs = await _dbContext.DanhMucTieuChiDiemMucs
                .AsNoTracking()
                .Where(x => x.TrangThai)
                .OrderBy(x => x.ThuTuSapXep)
                .ThenBy(x => x.TuGiaTri)
                .ToListAsync();

            var ngayKetThuc = hoSo.NgayBanHanh ?? hoSo.NgayHoanThanh ?? DateTime.Today;
            var tongNgayThucTe = Math.Max(1, (ngayKetThuc.Date - hoSo.NgayTaoHoSo.Date).Days + 1);

            var tongNgayQuyDinh = await _dbContext.HoSoVanBanBuocThoiHans
                .AsNoTracking()
                .Where(x => x.HoSoVanBanId == hoSo.Id && x.SoNgayXuLy.HasValue && x.SoNgayXuLy.Value > 0)
                .SumAsync(x => (int?)x.SoNgayXuLy) ?? 0;

            if (tongNgayQuyDinh <= 0 && hoSo.HanXuLy.HasValue)
            {
                tongNgayQuyDinh = Math.Max(1, (hoSo.HanXuLy.Value.Date - hoSo.NgayTaoHoSo.Date).Days + 1);
            }

            decimal? tyLeThoiGian = null;
            if (tongNgayQuyDinh > 0)
            {
                tyLeThoiGian = Math.Round(tongNgayThucTe * 100m / tongNgayQuyDinh, 2, MidpointRounding.AwayFromZero);
            }

            var result = new ChamDiemProposal
            {
                TongThoiGianXayDungNgay = tongNgayThucTe,
                TongThoiGianQuyDinhNgay = tongNgayQuyDinh > 0 ? tongNgayQuyDinh : null,
                TyLeThoiGianXayDung = tyLeThoiGian
            };

            foreach (var tieuChi in tieuChis)
            {
                decimal? giaTri = tieuChi.LoaiTieuChi switch
                {
                    "THOI_GIAN" => tyLeThoiGian,
                    "CHAT_LUONG" => hoSo.SoLanTraLaiHienTai,
                    _ => null
                };

                var muc = giaTri.HasValue
                    ? mucs.Where(x => x.DanhMucTieuChiDiemId == tieuChi.Id).FirstOrDefault(x => MatchMucDiem(x, giaTri.Value))
                    : null;

                var diem = muc?.Diem;
                if (diem.HasValue)
                {
                    result.TongDiem += diem.Value;
                }

                result.ChiTiets.Add(new ChamDiemProposalChiTiet
                {
                    DanhMucTieuChiDiemId = tieuChi.Id,
                    MaTieuChi = tieuChi.MaTieuChi,
                    TenTieuChi = tieuChi.TenTieuChi,
                    LoaiTieuChi = tieuChi.LoaiTieuChi,
                    DiemToiDa = tieuChi.DiemToiDa,
                    GiaTriTinhDiem = giaTri,
                    DienGiaiGiaTri = muc?.NhanHienThi,
                    DiemDeXuat = diem
                });
            }

            return result;
        }

        private static bool MatchMucDiem(DanhMucTieuChiDiemMuc muc, decimal giaTri)
        {
            var hopLeTu = !muc.TuGiaTri.HasValue
                || (muc.BaoGomTuGiaTri ? giaTri >= muc.TuGiaTri.Value : giaTri > muc.TuGiaTri.Value);
            var hopLeDen = !muc.DenGiaTri.HasValue
                || (muc.BaoGomDenGiaTri ? giaTri <= muc.DenGiaTri.Value : giaTri < muc.DenGiaTri.Value);

            return hopLeTu && hopLeDen;
        }

        private static string ResolveXepLoaiTongDiem(decimal tongDiem)
        {
            if (tongDiem >= 90) return "XUAT_SAC";
            if (tongDiem >= 80) return "TOT";
            if (tongDiem >= 65) return "KHA";
            if (tongDiem >= 50) return "TRUNG_BINH";
            return "CAN_CAI_THIEN";
        }

        private static void ClearScore(HoSoVanBan hoSo)
        {
            hoSo.TongThoiGianXayDungNgay = null;
            hoSo.TongThoiGianQuyDinhNgay = null;
            hoSo.TyLeThoiGianXayDung = null;
            hoSo.DiemTienDoXayDung = null;
            hoSo.DiemChatLuongVanBan = null;
            hoSo.TongDiemDanhGia = null;
            hoSo.XepLoaiDanhGia = null;
            hoSo.NgayChamDiem = null;
        }

        private sealed class NotificationNavigationConfig
        {
            public string ControllerNameDanhSach { get; set; } = "HoSoVanBan";
            public string ActionNameDanhSach { get; set; } = "Index";
            public string ParameterDanhSach { get; set; } = string.Empty;
            public string ControllerNameXetDuyet { get; set; } = "HoSoVanBan";
            public string ActionNameXetDuyet { get; set; } = "Index";
            public string ParameterXetDuyet { get; set; } = string.Empty;
        }

        private sealed class HoSoVanBanTrackingAggregate
        {
            public HoSoVanBanTienDoSummaryModel Summary { get; set; } = new();
            public List<HoSoVanBanBuocTheoDoiModel> Steps { get; set; } = new();
        }

        private sealed class ChamDiemProposal
        {
            public int? TongThoiGianXayDungNgay { get; set; }
            public int? TongThoiGianQuyDinhNgay { get; set; }
            public decimal? TyLeThoiGianXayDung { get; set; }
            public decimal TongDiem { get; set; }
            public List<ChamDiemProposalChiTiet> ChiTiets { get; set; } = new();
        }

        private sealed class ChamDiemProposalChiTiet
        {
            public Guid DanhMucTieuChiDiemId { get; set; }
            public string MaTieuChi { get; set; } = string.Empty;
            public string TenTieuChi { get; set; } = string.Empty;
            public string LoaiTieuChi { get; set; } = string.Empty;
            public decimal DiemToiDa { get; set; }
            public decimal? GiaTriTinhDiem { get; set; }
            public string? DienGiaiGiaTri { get; set; }
            public decimal? DiemDeXuat { get; set; }
        }
    }
}
