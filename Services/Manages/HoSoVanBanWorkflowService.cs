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
        Task<CommonResponse> GetDanhSachDangKyAsync(string search, Guid? donViSoanThaoId = null, int pageSize = 5, int pageCurrent = 1);
        Task<CommonResponse> GetDanhSachTheoBuocAsync(string search, string maBuoc, Guid? donViSoanThaoId = null, int pageSize = 5, int pageCurrent = 1, bool chiLayDonViDangNhap = true, IEnumerable<string>? trangThaiNghiepVuFilters = null, string? loaiQuyTrinh = null);
        Task<CommonResponse> GetDanhSachLayYKienAsync(string search, Guid? donViId = null, int pageSize = 5, int pageCurrent = 1);
        Task<List<DonViOptionModel>> GetDonViOptionsAsync();
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

        public async Task<CommonResponse> GetDanhSachDangKyAsync(string search, Guid? donViSoanThaoId = null, int pageSize = 5, int pageCurrent = 1)
        {
            return await GetDanhSachInternalAsync(search, pageSize, pageCurrent, false, null, false, donViSoanThaoId, false, null, "DangKy");
        }

        public async Task<CommonResponse> GetDanhSachTheoBuocAsync(string search, string maBuoc, Guid? donViSoanThaoId = null, int pageSize = 5, int pageCurrent = 1, bool chiLayDonViDangNhap = true, IEnumerable<string>? trangThaiNghiepVuFilters = null, string? loaiQuyTrinh = null)
        {
            return await GetDanhSachInternalAsync(search, pageSize, pageCurrent, false, maBuoc, chiLayDonViDangNhap, donViSoanThaoId, true, trangThaiNghiepVuFilters, loaiQuyTrinh);
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

                return new CommonResponse("success", "ThÃ nh cÃ´ng", data, totalRecord);
            }
            catch
            {
                return new CommonResponse();
            }
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
            string? loaiQuyTrinh = null)
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
                        TenQuyTrinh = quyTrinh.TenQuyTrinh,
                        TenBuocHienTai = buoc != null ? buoc.TenBuoc : "ÄÃ£ hoÃ n thÃ nh",
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
                            item.TenBuocHienTai = "Đang lấy ý kiến góp ý";
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
                            item.TenTrangThai = "ÄÃ£ hoÃ n thÃ nh";
                            item.MaMauTrangThai = "#28A745";
                            item.TenBuocHienTai = "ÄÃ£ hoÃ n thÃ nh";
                            item.CanXuLyBuocHienTai = false;
                        }
                    }

                    foreach (var item in data.Where(x =>
                                 x.NgayHoanThanh.HasValue ||
                                 (x.TongSoBuoc > 0 && x.SoBuocHoanThanh >= x.TongSoBuoc)))
                    {
                        var hoanThanhQuaHan = item.SoBuocQuaHan > 0;
                        item.MaTrangThai = hoanThanhQuaHan ? "HOAN_THANH_QUA_HAN" : "HOAN_THANH_DUNG_HAN";
                        item.TenTrangThai = hoanThanhQuaHan ? "HoÃ n thÃ nh quÃ¡ háº¡n" : "HoÃ n thÃ nh Ä‘Ãºng háº¡n";
                        item.MaMauTrangThai = hoanThanhQuaHan ? "#DC3545" : "#28A745";
                        item.TenBuocHienTai = "ÄÃ£ hoÃ n thÃ nh";
                        item.CanXuLyBuocHienTai = false;
                    }
                }

                foreach (var item in data.Where(x =>
                             x.NgayHoanThanh.HasValue ||
                             (x.TongSoBuoc > 0 && x.SoBuocHoanThanh >= x.TongSoBuoc)))
                {
                    var hoanThanhQuaHan = item.SoBuocQuaHan > 0;
                    item.TrangThaiTienDo = hoanThanhQuaHan ? "HOAN_THANH_QUA_HAN" : "HOAN_THANH_DUNG_HAN";
                    item.TenTrangThaiTienDo = hoanThanhQuaHan ? "HoÃ n thÃ nh quÃ¡ háº¡n" : "HoÃ n thÃ nh Ä‘Ãºng háº¡n";
                    item.MaMauTienDo = hoanThanhQuaHan ? "#DC3545" : "#28A745";
                    item.DangOQuaHan = hoanThanhQuaHan;
                }

                return new CommonResponse("success", "ThÃ nh cÃ´ng", data, totalRecord);
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
                query = query.Where(x => x.DanhMucVanBanId == danhMucVanBanId.Value);
            }

            if (string.Equals(loaiQuyTrinh, "XayDung", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(x => _dbContext.DanhMucBuocQuyTrinhs
                    .AsNoTracking()
                    .Any(b => b.QuyTrinhSoanThaoId == x.Id &&
                              string.Equals(b.LoaiBuoc, "SoanThao")));
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
                return new CommonResponse("error", "KhÃ´ng xÃ¡c Ä‘á»‹nh Ä‘Æ°á»£c tÃ i khoáº£n Ä‘ang thao tÃ¡c!");
            }

            if (!request.TuNgaySoanThao.HasValue || !request.DenNgaySoanThao.HasValue)
            {
                return new CommonResponse("error", "Thá»i gian soáº¡n tháº£o báº¯t buá»™c pháº£i nháº­p.");
            }

            if (request.DenNgaySoanThao.Value.Date < request.TuNgaySoanThao.Value.Date)
            {
                return new CommonResponse("error", "Äáº¿n ngÃ y soáº¡n tháº£o pháº£i lá»›n hÆ¡n hoáº·c báº±ng tá»« ngÃ y soáº¡n tháº£o.");
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
                return new CommonResponse("error", "MÃ£ há»“ sÆ¡ Ä‘Ã£ tá»“n táº¡i!");
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
                return new CommonResponse("error", "KhÃ´ng tÃ¬m tháº¥y quy trÃ¬nh soáº¡n tháº£o Ä‘ang kÃ­ch hoáº¡t!");
            }

            var firstStep = await _dbContext.DanhMucBuocQuyTrinhs
                .AsNoTracking()
                .Where(x => x.QuyTrinhSoanThaoId == request.QuyTrinhSoanThaoId)
                .OrderBy(x => x.ThuTuSapXep)
                .ThenBy(x => x.MaBuoc)
                .FirstOrDefaultAsync();

            if (firstStep == null)
            {
                return new CommonResponse("error", "Quy trÃ¬nh chÆ°a cÃ³ bÆ°á»›c nÃ o Ä‘á»ƒ khá»Ÿi táº¡o há»“ sÆ¡!");
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
                    NoiDungXuLy = "Khá»Ÿi táº¡o há»“ sÆ¡ vÃ o quy trÃ¬nh.",
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
                    $"Há»“ sÆ¡ '{hoSo.TenHoSo}' Ä‘Ã£ Ä‘Æ°á»£c khá»Ÿi táº¡o vÃ  Ä‘ang á»Ÿ bÆ°á»›c '{firstStep.TenBuoc}'.");

                return new CommonResponse("success", "ThÃ nh cÃ´ng", hoSo.Id);
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
                return new CommonResponse("error", "KhÃ´ng xÃ¡c Ä‘á»‹nh Ä‘Æ°á»£c tÃ i khoáº£n Ä‘ang thao tÃ¡c!");
            }

            var hoSo = await _dbContext.HoSoVanBans
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == hoSoVanBanId);

            if (hoSo == null)
            {
                return new CommonResponse("error", "KhÃ´ng tÃ¬m tháº¥y há»“ sÆ¡ vÄƒn báº£n!");
            }

            var currentStep = await GetCurrentStepAsync(hoSo);
            var currentProcessing = await _dbContext.HoSoVanBanXuLys
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.HoSoVanBanId == hoSo.Id && x.IsCurrent);

            if (!CanEditDangKyHoSo(currentUser, hoSo, currentStep, currentProcessing))
            {
                return new CommonResponse("error", "Há»“ sÆ¡ nÃ y Ä‘Ã£ Ä‘Æ°á»£c gá»­i Ä‘i hoáº·c báº¡n khÃ´ng cÃ³ quyá»n cáº­p nháº­t.");
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

            return new CommonResponse("success", "ThÃ nh cÃ´ng", model);
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
                return new CommonResponse("error", "Không xác định được tài khoản đang thao tác!");
            }

            var hoSo = await GetHoSoWithCurrentStepAsync(hoSoVanBanId);
            if (hoSo == null)
            {
                return new CommonResponse("error", "Không tìm thấy hồ sơ cần chuyển xét duyệt.");
            }

            var currentStep = await GetCurrentStepAsync(hoSo);
            if (currentStep == null)
            {
                return new CommonResponse("error", "Không xác định được bước hiện tại của hồ sơ.");
            }

            var currentProcessing = await _dbContext.HoSoVanBanXuLys
                .AsNoTracking()
                .Where(x => x.HoSoVanBanId == hoSo.Id && x.IsCurrent)
                .OrderByDescending(x => x.NgayNhan)
                .FirstOrDefaultAsync();

            if (!CanCurrentUserXuLy(currentUser, currentProcessing))
            {
                return new CommonResponse("error", "Hồ sơ này không thuộc đơn vị đang đăng nhập để chuyển xét duyệt.");
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
                return new CommonResponse("error", "Workflow chưa cấu hình bước kế tiếp cho nghiệp vụ chuyển xét duyệt.");
            }

            var nextStepDeadline = await ResolveNextStepDeadlineAsync(hoSo.Id, nextStep);

            return new CommonResponse("success", "Thành công", new HoSoVanBanXuLyStepModel
            {
                HoSoVanBanId = hoSo.Id,
                TenHoSo = hoSo.TenHoSo,
                KetQuaXuLy = nextTransition?.DieuKienKetQua ?? "GUI_THAM_DINH",
                NgayXuLy = DateTime.Now,
                HanXuLy = nextStepDeadline,
                NoiDungXuLy = $"Chuyển hồ sơ {hoSo.TenHoSo} sang bước xét duyệt.",
                GhiChu = $"Chuyển hồ sơ {hoSo.TenHoSo} sang bước xử lý kế tiếp."
            });
        }

        public async Task<CommonResponse> GetChuyenPheDuyetModelAsync(Guid hoSoVanBanId)
        {
            var currentUser = _authService.GetUserInfo();
            if (currentUser == null)
            {
                return new CommonResponse("error", "Không xác định được tài khoản đang thao tác!");
            }

            var hoSo = await GetHoSoWithCurrentStepAsync(hoSoVanBanId);
            if (hoSo == null)
            {
                return new CommonResponse("error", "Không tìm thấy hồ sơ cần chuyển phê duyệt.");
            }

            var daCoBanGhiDanhGia = await _dbContext.HoSoVanBanDanhGias
                .AsNoTracking()
                .AnyAsync(x => x.HoSoVanBanId == hoSoVanBanId);

            if (!daCoBanGhiDanhGia)
            {
                return new CommonResponse("error", "Hồ sơ chưa có bản ghi xét duyệt nên chưa thể chuyển sang phê duyệt.");
            }

            var currentStep = await GetCurrentStepAsync(hoSo);
            if (currentStep == null)
            {
                return new CommonResponse("error", "Không xác định được bước hiện tại của hồ sơ.");
            }

            var currentProcessing = await _dbContext.HoSoVanBanXuLys
                .AsNoTracking()
                .Where(x => x.HoSoVanBanId == hoSo.Id && x.IsCurrent)
                .OrderByDescending(x => x.NgayNhan)
                .FirstOrDefaultAsync();

            if (!CanCurrentUserXuLy(currentUser, currentProcessing))
            {
                return new CommonResponse("error", "Hồ sơ này không thuộc đơn vị đang đăng nhập để chuyển phê duyệt.");
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
                return new CommonResponse("error", "Workflow chưa cấu hình bước kế tiếp cho nghiệp vụ chuyển phê duyệt.");
            }

            var nextStepDeadline = await ResolveNextStepDeadlineAsync(hoSo.Id, nextStep);

            return new CommonResponse("success", "Thành công", new HoSoVanBanXuLyStepModel
            {
                HoSoVanBanId = hoSo.Id,
                TenHoSo = hoSo.TenHoSo,
                KetQuaXuLy = nextTransition?.DieuKienKetQua ?? "THAM_DINH_XONG",
                NgayXuLy = DateTime.Now,
                HanXuLy = nextStepDeadline,
                NoiDungXuLy = $"Chuyển hồ sơ {hoSo.TenHoSo} sang bước phê duyệt văn bản.",
                GhiChu = $"Chuyển hồ sơ {hoSo.TenHoSo} sang bước phê duyệt."
            });
        }

        public async Task<CommonResponse> GetChuyenBanHanhModelAsync(Guid hoSoVanBanId)
        {
            var currentUser = _authService.GetUserInfo();
            if (currentUser == null)
            {
                return new CommonResponse("error", "Không xác định được tài khoản đang thao tác!");
            }

            var hoSo = await GetHoSoWithCurrentStepAsync(hoSoVanBanId);
            if (hoSo == null)
            {
                return new CommonResponse("error", "Không tìm thấy hồ sơ cần chuyển ban hành.");
            }

            var currentStep = await GetCurrentStepAsync(hoSo);
            if (currentStep == null)
            {
                return new CommonResponse("error", "Không xác định được bước hiện tại của hồ sơ.");
            }

            var currentProcessing = await _dbContext.HoSoVanBanXuLys
                .AsNoTracking()
                .Where(x => x.HoSoVanBanId == hoSo.Id && x.IsCurrent)
                .OrderByDescending(x => x.NgayNhan)
                .FirstOrDefaultAsync();

            if (!CanCurrentUserXuLy(currentUser, currentProcessing))
            {
                return new CommonResponse("error", "Hồ sơ này không thuộc đơn vị đang đăng nhập để chuyển ban hành.");
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
                return new CommonResponse("error", "Workflow chưa cấu hình bước kế tiếp cho nghiệp vụ chuyển ban hành.");
            }

            var nextStepDeadline = await ResolveNextStepDeadlineAsync(hoSo.Id, nextStep);

            return new CommonResponse("success", "Thành công", new HoSoVanBanXuLyStepModel
            {
                HoSoVanBanId = hoSo.Id,
                TenHoSo = hoSo.TenHoSo,
                KetQuaXuLy = nextTransition?.DieuKienKetQua ?? "TRINH_THANH_CONG",
                NgayXuLy = DateTime.Now,
                HanXuLy = nextStepDeadline,
                NoiDungXuLy = $"Chuyển hồ sơ {hoSo.TenHoSo} sang bước ban hành văn bản.",
                GhiChu = $"Chuyển hồ sơ {hoSo.TenHoSo} sang bước ban hành."
            });
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
                return new CommonResponse("error", "KhÃ´ng xÃ¡c Ä‘á»‹nh Ä‘Æ°á»£c tÃ i khoáº£n Ä‘ang thao tÃ¡c!");
            }

            if (!request.TuNgaySoanThao.HasValue || !request.DenNgaySoanThao.HasValue)
            {
                return new CommonResponse("error", "Thá»i gian soáº¡n tháº£o báº¯t buá»™c pháº£i nháº­p.");
            }

            if (request.DenNgaySoanThao.Value.Date < request.TuNgaySoanThao.Value.Date)
            {
                return new CommonResponse("error", "Äáº¿n ngÃ y soáº¡n tháº£o pháº£i lá»›n hÆ¡n hoáº·c báº±ng tá»« ngÃ y soáº¡n tháº£o.");
            }

            if (request.Id == Guid.Empty)
            {
                return new CommonResponse("error", "Thiáº¿u thÃ´ng tin há»“ sÆ¡ cáº§n cáº­p nháº­t!");
            }

            if (string.IsNullOrWhiteSpace(request.TenHoSo))
            {
                return new CommonResponse("error", "TÃªn há»“ sÆ¡ khÃ´ng Ä‘Æ°á»£c Ä‘á»ƒ trá»‘ng!");
            }

            var hoSo = await _dbContext.HoSoVanBans.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (hoSo == null)
            {
                return new CommonResponse("error", "KhÃ´ng tÃ¬m tháº¥y há»“ sÆ¡ vÄƒn báº£n!");
            }

            var currentStep = await GetCurrentStepAsync(hoSo);
            var currentProcessing = await _dbContext.HoSoVanBanXuLys
                .FirstOrDefaultAsync(x => x.HoSoVanBanId == hoSo.Id && x.IsCurrent);

            if (!CanEditDangKyHoSo(currentUser, hoSo, currentStep, currentProcessing))
            {
                return new CommonResponse("error", "Há»“ sÆ¡ nÃ y Ä‘Ã£ Ä‘Æ°á»£c gá»­i Ä‘i hoáº·c báº¡n khÃ´ng cÃ³ quyá»n cáº­p nháº­t.");
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
                return new CommonResponse("error", "KhÃ´ng tÃ¬m tháº¥y quy trÃ¬nh soáº¡n tháº£o Ä‘ang kÃ­ch hoáº¡t!");
            }

            var firstStep = await _dbContext.DanhMucBuocQuyTrinhs
                .AsNoTracking()
                .Where(x => x.QuyTrinhSoanThaoId == request.QuyTrinhSoanThaoId)
                .OrderBy(x => x.ThuTuSapXep)
                .ThenBy(x => x.MaBuoc)
                .FirstOrDefaultAsync();

            if (firstStep == null)
            {
                return new CommonResponse("error", "Quy trÃ¬nh chÆ°a cÃ³ bÆ°á»›c nÃ o Ä‘á»ƒ cáº­p nháº­t há»“ sÆ¡!");
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
                    currentProcessing.NoiDungXuLy = "Cáº­p nháº­t há»“ sÆ¡ Ä‘Äƒng kÃ½ trÆ°á»›c khi chuyá»ƒn bÆ°á»›c tiáº¿p theo.";
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
                return new CommonResponse("success", "Cáº­p nháº­t há»“ sÆ¡ thÃ nh cÃ´ng", hoSo.Id);
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
                return new CommonResponse("error", "KhÃ´ng xÃ¡c Ä‘á»‹nh Ä‘Æ°á»£c tÃ i khoáº£n Ä‘ang thao tÃ¡c!");
            }

            var hoSo = await GetHoSoWithCurrentStepAsync(request.HoSoVanBanId);
            if (hoSo == null)
            {
                return new CommonResponse("error", "KhÃ´ng tÃ¬m tháº¥y há»“ sÆ¡ vÄƒn báº£n!");
            }

            var currentStep = await GetCurrentStepAsync(hoSo);
            if (currentStep == null)
            {
                return new CommonResponse("error", "Ho so chua xac dinh duoc buoc hien tai!");
            }

            if (currentStep.LoaiBuoc == "LayYKien" || currentStep.LoaiBuoc == "DanhGia")
            {
                return new CommonResponse("error", "BÆ°á»›c hiá»‡n táº¡i lÃ  bÆ°á»›c Ä‘áº·c thÃ¹. HÃ£y dÃ¹ng nghiá»‡p vá»¥ Láº¥y Ã½ kiáº¿n hoáº·c ÄÃ¡nh giÃ¡.");
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
                    return new CommonResponse("error", "Há»“ sÆ¡ chÆ°a Ä‘Æ°á»£c nháº­n. Vui lÃ²ng báº¥m 'Nháº­n há»“ sÆ¡' trÆ°á»›c khi xá»­ lÃ½.");
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

                return new CommonResponse("success", "ThÃ nh cÃ´ng", hoSo.Id);
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
                return new CommonResponse("error", "KhÃ´ng xÃ¡c Ä‘á»‹nh Ä‘Æ°á»£c tÃ i khoáº£n Ä‘ang thao tÃ¡c!");
            }

            var hoSo = await GetHoSoWithCurrentStepAsync(hoSoVanBanId);
            if (hoSo == null)
            {
                return new CommonResponse("error", "KhÃ´ng tÃ¬m tháº¥y há»“ sÆ¡ vÄƒn báº£n!");
            }

            var currentProcessing = await _dbContext.HoSoVanBanXuLys
                .FirstOrDefaultAsync(x => x.HoSoVanBanId == hoSo.Id && x.IsCurrent);

            if (currentProcessing == null)
            {
                return new CommonResponse("error", "Há»“ sÆ¡ chÆ°a phÃ¡t sinh bÆ°á»›c xá»­ lÃ½ hiá»‡n táº¡i.");
            }

            if (!CanCurrentUserXuLy(currentUser, currentProcessing))
            {
                return new CommonResponse("error", "Há»“ sÆ¡ nÃ y khÃ´ng thuá»™c Ä‘Æ¡n vá»‹ Ä‘ang Ä‘Äƒng nháº­p Ä‘á»ƒ nháº­n.");
            }

            if (currentProcessing.NguoiXuLyId.HasValue && !currentUser.SSA)
            {
                return new CommonResponse("success", "Há»“ sÆ¡ Ä‘Ã£ Ä‘Æ°á»£c nháº­n trÆ°á»›c Ä‘Ã³.");
            }

            currentProcessing.NguoiXuLyId = currentUser.Id;
            currentProcessing.NgayNhan = DateTime.Now;
            await _dbContext.SaveChangesAsync();

            return new CommonResponse("success", "ÄÃ£ nháº­n há»“ sÆ¡ thÃ nh cÃ´ng.");
        }

        public async Task<CommonResponse> NhanHoSoAsync(Guid hoSoVanBanId, string actionType = "NHAN_HO_SO", string? noiDungXuLy = null, string? ghiChu = null, DateTime? ngayXuLy = null, DateTime? hanXuLy = null)
        {
            var currentUser = _authService.GetUserInfo();
            if (currentUser == null)
            {
                return new CommonResponse("error", "KhÃ´ng xÃ¡c Ä‘á»‹nh Ä‘Æ°á»£c tÃ i khoáº£n Ä‘ang thao tÃ¡c!");
            }

            var hoSo = await GetHoSoWithCurrentStepAsync(hoSoVanBanId);
            if (hoSo == null)
            {
                return new CommonResponse("error", "KhÃ´ng tÃ¬m tháº¥y há»“ sÆ¡ vÄƒn báº£n!");
            }

            var currentProcessing = await _dbContext.HoSoVanBanXuLys
                .FirstOrDefaultAsync(x => x.HoSoVanBanId == hoSo.Id && x.IsCurrent);

            if (currentProcessing == null)
            {
                return new CommonResponse("error", "Há»“ sÆ¡ chÆ°a phÃ¡t sinh bÆ°á»›c xá»­ lÃ½ hiá»‡n táº¡i.");
            }

            if (!CanCurrentUserXuLy(currentUser, currentProcessing))
            {
                return new CommonResponse("error", "Há»“ sÆ¡ nÃ y khÃ´ng thuá»™c Ä‘Æ¡n vá»‹ Ä‘ang Ä‘Äƒng nháº­p Ä‘á»ƒ nháº­n.");
            }

            var actionCode = NormalizeTiepNhanNghiepVuCode(actionType);
            if (string.IsNullOrWhiteSpace(actionCode))
            {
                return new CommonResponse("error", "Thao tÃ¡c nháº­n há»“ sÆ¡ khÃ´ng há»£p lá»‡.");
            }

            var laThaoTacNhanBanDau = actionCode is "NHAN_HO_SO" or "NHAN_VA_CHUYEN_PHE_DUYET" or "PHE_DUYET_HO_SO";
            if (!currentProcessing.NguoiXuLyId.HasValue)
            {
                currentProcessing.NguoiXuLyId = currentUser.Id;
                currentProcessing.NgayNhan = DateTime.Now;
            }
            else if (laThaoTacNhanBanDau && !currentUser.SSA)
            {
                return new CommonResponse("success", "Há»“ sÆ¡ Ä‘Ã£ Ä‘Æ°á»£c nháº­n trÆ°á»›c Ä‘Ã³.");
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
                    return new CommonResponse("error", "Không xác định được bước hiện tại của hồ sơ.");
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
                        return new CommonResponse("error", "Hồ sơ chưa có bản ghi xét duyệt nên chưa thể chuyển sang phê duyệt.");
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
                        "CHUYEN_PHE_DUYET" => "Workflow chưa cấu hình bước kế tiếp cho nghiệp vụ chuyển phê duyệt.",
                        "CHUYEN_BAN_HANH" => "Workflow chưa cấu hình bước kế tiếp cho nghiệp vụ chuyển ban hành.",
                        _ => "Workflow chưa cấu hình bước kế tiếp cho nghiệp vụ chuyển xét duyệt."
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
                return new CommonResponse("error", "Không xác định được tài khoản đang thao tác!");
            }

            if (string.IsNullOrWhiteSpace(lyDoTraLai))
            {
                return new CommonResponse("error", "Bạn cần nhập lý do trả lại hồ sơ.");
            }

            var hoSo = await GetHoSoWithCurrentStepAsync(hoSoVanBanId);
            if (hoSo == null)
            {
                return new CommonResponse("error", "Không tìm thấy hồ sơ văn bản!");
            }

            var currentStep = await GetCurrentStepAsync(hoSo);
            var laBuocDanhGia = currentStep != null &&
                                (string.Equals(currentStep.LoaiBuoc, "DanhGia", StringComparison.OrdinalIgnoreCase) ||
                                 string.Equals(currentStep.MaBuoc, "BUOC_03_THAM_DINH_VAN_BAN", StringComparison.OrdinalIgnoreCase));
            if (!laBuocDanhGia)
            {
                return new CommonResponse("error", "Hồ sơ hiện không ở bước thẩm định văn bản.");
            }

            var currentProcessing = await _dbContext.HoSoVanBanXuLys
                .Where(x => x.HoSoVanBanId == hoSo.Id && x.IsCurrent)
                .OrderByDescending(x => x.NgayNhan)
                .FirstOrDefaultAsync();

            if (!CanCurrentUserXuLy(currentUser, currentProcessing))
            {
                return new CommonResponse("error", "Hồ sơ này không thuộc đơn vị đang đăng nhập để trả lại.");
            }

            var soLanTraLaiToiDa = currentStep.SoLanTraLaiToiDa > 0 ? currentStep.SoLanTraLaiToiDa : 3;
            if (hoSo.SoLanTraLaiHienTai >= soLanTraLaiToiDa)
            {
                return new CommonResponse("error", $"Đã vượt quá số lần trả lại tối đa ({soLanTraLaiToiDa}) của bước này.");
            }

            var maBuocTraLai = await ResolveDraftReturnStepCodeAsync(hoSo.QuyTrinhSoanThaoId);
            if (string.IsNullOrWhiteSpace(maBuocTraLai))
            {
                return new CommonResponse("error", "Không xác định được bước trả lại cho hồ sơ này.");
            }

            var nextStep = await _dbContext.DanhMucBuocQuyTrinhs
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.QuyTrinhSoanThaoId == hoSo.QuyTrinhSoanThaoId && x.MaBuoc == maBuocTraLai);

            if (nextStep == null)
            {
                return new CommonResponse("error", "Không tìm thấy bước soạn thảo để trả lại hồ sơ.");
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
                    $"Trả lại hồ sơ từ bước đánh giá lần {hoSo.SoLanTraLaiHienTai}");

                await AdvanceWorkflowAsync(hoSo, currentUser, nextStep, null, "TRA_LAI_HO_SO", lyDoTraLai.Trim());
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return new CommonResponse("success", "Đã trả lại hồ sơ về bước soạn thảo.");
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
                return new CommonResponse("error", "KhÃ´ng xÃ¡c Ä‘á»‹nh Ä‘Æ°á»£c tÃ i khoáº£n Ä‘ang thao tÃ¡c!");
            }

            var hoSo = await GetHoSoWithCurrentStepAsync(request.HoSoVanBanId);
            if (hoSo == null)
            {
                return new CommonResponse("error", "KhÃ´ng tÃ¬m tháº¥y há»“ sÆ¡ vÄƒn báº£n!");
            }

            var currentStep = await GetCurrentStepAsync(hoSo);
            if (currentStep == null || !string.Equals(currentStep.LoaiBuoc, "SoanThao", StringComparison.OrdinalIgnoreCase))
            {
                return new CommonResponse("error", "Há»“ sÆ¡ hiá»‡n khÃ´ng á»Ÿ bÆ°á»›c soáº¡n tháº£o Ä‘á»ƒ chuyá»ƒn láº¥y gÃ³p Ã½.");
            }

            var currentProcessing = await _dbContext.HoSoVanBanXuLys
                .Where(x => x.HoSoVanBanId == hoSo.Id && x.IsCurrent)
                .OrderByDescending(x => x.NgayNhan)
                .FirstOrDefaultAsync();

            if (!CanCurrentUserXuLy(currentUser, currentProcessing))
            {
                return new CommonResponse("error", "Há»“ sÆ¡ nÃ y Ä‘Ã£ Ä‘Æ°á»£c chuyá»ƒn sang Ä‘Æ¡n vá»‹ khÃ¡c. Báº¡n khÃ´ng thá»ƒ cáº­p nháº­t ná»¯a!");
            }

            var actionMode = NormalizeLayYKienActionMode(request.ActionMode);
            if (actionMode == "GUI_DON_VI_GOP_Y" && request.DonViDuocLayYKienIds.Count == 0)
            {
                return new CommonResponse("error", "Báº¡n pháº£i chá»n Ã­t nháº¥t 1 Ä‘Æ¡n vá»‹ Ä‘á»ƒ gá»­i gÃ³p Ã½.");
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
                            GhiChu = "ÄÆ¡n vá»‹ soáº¡n tháº£o gá»­i Ä‘á» nghá»‹ gÃ³p Ã½."
                        });

                        await TaoThongBaoAsync(
                            hoSo,
                            currentStep,
                            currentUser.DanhMucDonViId,
                            donViId,
                            $"Há»“ sÆ¡ '{hoSo.TenHoSo}' Ä‘ang láº¥y gÃ³p Ã½ tá»« Ä‘Æ¡n vá»‹ cá»§a báº¡n.");
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
                        GhiChu = "ÄÆ¡n vá»‹ soáº¡n tháº£o tá»± cáº­p nháº­t káº¿t quáº£ gÃ³p Ã½."
                    });
                }

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return new CommonResponse("success", "ÄÃ£ chuyá»ƒn há»“ sÆ¡ sang bÆ°á»›c láº¥y gÃ³p Ã½.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new CommonResponse("error", $"KhÃ´ng thá»ƒ khá»Ÿi táº¡o bÆ°á»›c láº¥y gÃ³p Ã½: {ex.Message}");
            }
        }

        public async Task<CommonResponse> TraLaiDangKyAsync(Guid hoSoVanBanId, string lyDoTraLai, string? ghiChu = null)
        {
            var currentUser = _authService.GetUserInfo();
            if (currentUser == null)
            {
                return new CommonResponse("error", "KhÃ´ng xÃ¡c Ä‘á»‹nh Ä‘Æ°á»£c tÃ i khoáº£n Ä‘ang thao tÃ¡c!");
            }

            if (string.IsNullOrWhiteSpace(lyDoTraLai))
            {
                return new CommonResponse("error", "Báº¡n pháº£i nháº­p lÃ½ do tráº£ láº¡i há»“ sÆ¡.");
            }

            var hoSo = await GetHoSoWithCurrentStepAsync(hoSoVanBanId);
            if (hoSo == null)
            {
                return new CommonResponse("error", "KhÃ´ng tÃ¬m tháº¥y há»“ sÆ¡ vÄƒn báº£n!");
            }

            var currentStep = await GetCurrentStepAsync(hoSo);
            if (currentStep == null || currentStep.MaBuoc != "BUOC_02_THONG_NHAT")
            {
                return new CommonResponse("error", "Há»“ sÆ¡ hiá»‡n khÃ´ng á»Ÿ bÆ°á»›c xÃ©t duyá»‡t Ä‘Äƒng kÃ½ Ä‘á»ƒ tráº£ láº¡i.");
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
                    return new CommonResponse("error", "Há»“ sÆ¡ nÃ y khÃ´ng thuá»™c Ä‘Æ¡n vá»‹ Ä‘ang Ä‘Äƒng nháº­p Ä‘á»ƒ xá»­ lÃ½.");
                }

                if (currentProcessing != null && !currentUser.SSA && !currentProcessing.NguoiXuLyId.HasValue)
                {
                    return new CommonResponse("error", "Há»“ sÆ¡ chÆ°a Ä‘Æ°á»£c nháº­n. Vui lÃ²ng nháº­n há»“ sÆ¡ trÆ°á»›c khi tráº£ láº¡i.");
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
                    return new CommonResponse("error", "KhÃ´ng xÃ¡c Ä‘á»‹nh Ä‘Æ°á»£c bÆ°á»›c quay láº¡i khi tráº£ há»“ sÆ¡.");
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

                return new CommonResponse("success", "ÄÃ£ tráº£ láº¡i há»“ sÆ¡ vá» bÆ°á»›c Ä‘Äƒng kÃ½.");
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
                return new CommonResponse("error", "KhÃ´ng xÃ¡c Ä‘á»‹nh Ä‘Æ°á»£c tÃ i khoáº£n Ä‘ang thao tÃ¡c!");
            }

            var hoSo = await GetHoSoWithCurrentStepAsync(request.HoSoVanBanId);
            if (hoSo == null)
            {
                return new CommonResponse("error", "KhÃ´ng tÃ¬m tháº¥y há»“ sÆ¡ vÄƒn báº£n!");
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
                return new CommonResponse("error", "Há»“ sÆ¡ hiá»‡n khÃ´ng á»Ÿ nghiá»‡p vá»¥ láº¥y Ã½ kiáº¿n!");
            }

            if (currentStep.YeuCauFileDinhKem && !request.AttachedFileGroupId.HasValue)
            {
                return new CommonResponse("error", "BÆ°á»›c nÃ y yÃªu cáº§u cÃ³ file Ä‘Ã­nh kÃ¨m káº¿t quáº£ láº¥y Ã½ kiáº¿n!");
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
                        return new CommonResponse("error", "Thiáº¿u thÃ´ng tin Ä‘Æ¡n vá»‹ pháº£n há»“i gÃ³p Ã½.");
                    }

                    layYKien = existingRows
                        .Where(x => x.DonViDuocLayYKienId == request.DonViDuocLayYKienId.Value)
                        .OrderByDescending(x => x.CreatedDate)
                        .FirstOrDefault();

                    if (layYKien == null)
                    {
                        return new CommonResponse("error", "KhÃ´ng tÃ¬m tháº¥y yÃªu cáº§u gÃ³p Ã½ cá»§a Ä‘Æ¡n vá»‹ Ä‘Æ°á»£c chá»n.");
                    }

                    layYKien.NoiDungPhanHoi = request.NoiDungPhanHoi?.Trim();
                    layYKien.NgayPhanHoi = request.NgayPhanHoi ?? DateTime.Now;
                    layYKien.TrangThaiPhanHoi = string.IsNullOrWhiteSpace(request.TrangThaiPhanHoi) ? "DA_CO_Y_KIEN" : request.TrangThaiPhanHoi.Trim();
                    layYKien.AttachedFileGroupId = request.AttachedFileGroupId;
                    layYKien.GhiChu = request.GhiChu?.Trim();

                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new CommonResponse("success", "ÄÃ£ cáº­p nháº­t Ã½ kiáº¿n cá»§a Ä‘Æ¡n vá»‹ gÃ³p Ã½.", layYKien.Id);
                }

                if (actionMode == "TONG_HOP_Y_KIEN")
                {
                    request.CacLayYKien ??= new List<HoSoVanBanLayYKienItemModel>();

                    var invalidUnitRow = request.CacLayYKien.FirstOrDefault(x =>
                        x.DonViDuocLayYKienId == null || x.DonViDuocLayYKienId == Guid.Empty);
                    if (invalidUnitRow != null)
                    {
                        return new CommonResponse("error", "Vui lÃ²ng chá»n Ä‘Æ¡n vá»‹ gÃ³p Ã½ cho táº¥t cáº£ cÃ¡c dÃ²ng trÆ°á»›c khi lÆ°u.");
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

                return new CommonResponse("success", "ThÃ nh cÃ´ng", layYKien.Id);
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
                return new CommonResponse("error", "KhÃ´ng xÃ¡c Ä‘á»‹nh Ä‘Æ°á»£c tÃ i khoáº£n Ä‘ang thao tÃ¡c!");
            }

            var hoSo = await GetHoSoWithCurrentStepAsync(request.HoSoVanBanId);
            if (hoSo == null)
            {
                return new CommonResponse("error", "KhÃ´ng tÃ¬m tháº¥y há»“ sÆ¡ vÄƒn báº£n!");
            }

            var currentStep = await GetCurrentStepAsync(hoSo);
            var laBuocDanhGia = currentStep != null &&
                                (string.Equals(currentStep.LoaiBuoc, "DanhGia", StringComparison.OrdinalIgnoreCase) ||
                                 string.Equals(currentStep.MaBuoc, "BUOC_03_THAM_DINH_VAN_BAN", StringComparison.OrdinalIgnoreCase));
            if (!laBuocDanhGia)
            {
                return new CommonResponse("error", "Hồ sơ hiện không ở bước thẩm định văn bản.");
            }

            var ketQua = request.KetQuaDanhGia.Trim().ToUpperInvariant();
            var ketQuaDuocChapNhan = new[] { "DAT", "THAM_DINH_XONG" };
            var laTraLaiThamDinh = ketQua == "KHONG_DAT" || ketQua.StartsWith("KHONG_DAT_LAN_", StringComparison.OrdinalIgnoreCase);
            if (!ketQuaDuocChapNhan.Contains(ketQua) && !laTraLaiThamDinh)
            {
                return new CommonResponse("error", "Kết quả xét duyệt chỉ chấp nhận DAT, THAM_DINH_XONG hoặc các trạng thái KHONG_DAT_LAN_1..3!");
            }

            if (currentStep.YeuCauFileDinhKem && !request.AttachedFileGroupId.HasValue)
            {
                return new CommonResponse("error", "BÆ°á»›c Ä‘Ã¡nh giÃ¡ hiá»‡n táº¡i yÃªu cáº§u file Ä‘Ã­nh kÃ¨m!");
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
                        return new CommonResponse("error", "KhÃ´ng tÃ¬m tháº¥y bÆ°á»›c tráº£ láº¡i theo mÃ£ bÆ°á»›c Ä‘Ã£ nháº­p!");
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

                return new CommonResponse("success", "ThÃ nh cÃ´ng", danhGia.Id);
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
                return new CommonResponse("error", "KhÃ´ng xÃ¡c Ä‘á»‹nh Ä‘Æ°á»£c tÃ i khoáº£n Ä‘ang thao tÃ¡c!");
            }

            var hoSo = await _dbContext.HoSoVanBans.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.HoSoVanBanId);
            if (hoSo == null)
            {
                return new CommonResponse("error", "KhÃ´ng tÃ¬m tháº¥y há»“ sÆ¡ vÄƒn báº£n!");
            }

            var danhGia = await _dbContext.HoSoVanBanDanhGias
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.HoSoVanBanDanhGiaId && x.HoSoVanBanId == request.HoSoVanBanId);

            if (danhGia == null)
            {
                return new CommonResponse("error", "KhÃ´ng tÃ¬m tháº¥y láº§n Ä‘Ã¡nh giÃ¡ cáº§n pháº£n há»“i!");
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
                return new CommonResponse("success", "ThÃ nh cÃ´ng", phanHoi.Id);
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
                        model.TenBuocHienTai = "Đang lấy ý kiến góp ý";
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
                        model.TenTrangThai = "ÄÃ£ hoÃ n thÃ nh";
                        model.MaMauTrangThai = "#28A745";
                        model.TenBuocHienTai = "ÄÃ£ hoÃ n thÃ nh";
                        model.CanXuLyBuocHienTai = false;
                    }

                    if (model.NgayHoanThanh.HasValue ||
                        (model.TienDoSummary.TongSoBuoc > 0 && model.TienDoSummary.SoBuocHoanThanh >= model.TienDoSummary.TongSoBuoc))
                    {
                        var hoanThanhQuaHan = model.TienDoSummary.SoBuocQuaHan > 0;
                        model.TenTrangThai = hoanThanhQuaHan ? "HoÃ n thÃ nh quÃ¡ háº¡n" : "HoÃ n thÃ nh Ä‘Ãºng háº¡n";
                        model.MaMauTrangThai = hoanThanhQuaHan ? "#DC3545" : "#28A745";
                        model.TenBuocHienTai = "ÄÃ£ hoÃ n thÃ nh";
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
                }

                return model == null
                    ? new CommonResponse("error", "KhÃ´ng tÃ¬m tháº¥y há»“ sÆ¡ vÄƒn báº£n!")
                    : new CommonResponse("success", "ThÃ nh cÃ´ng", model);
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
                        FileName = x.FileName ?? "Tệp đính kèm",
                        MoTa = x.MoTa,
                        NguonHienThi = "Dự thảo hiện tại",
                        NhanHienThi = "Dự thảo hiện tại | " + (x.FileName ?? "Tệp đính kèm"),
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
                        FileName = file.FileName ?? "Tệp đính kèm",
                        MoTa = file.MoTa,
                        NguonHienThi = NormalizeDraftCompareSourceLabel(version.TenVersion),
                        NhanHienThi = NormalizeDraftCompareSourceLabel(version.TenVersion) + " | " + (file.FileName ?? "Tệp đính kèm"),
                        NgayTao = version.NgayTaoVersion,
                        FileExtension = file.FileName != null ? Path.GetExtension(file.FileName).ToLowerInvariant() : null,
                        LaDocx = file.FileName != null && string.Equals(Path.GetExtension(file.FileName), ".docx", StringComparison.OrdinalIgnoreCase)
                    })
                    .ToListAsync();

                var fileOptions = currentFiles
                    .Concat(versionFiles)
                    .Where(x => !string.IsNullOrWhiteSpace(x.FileExtension) && DraftFileExtensions.Contains(x.FileExtension!, StringComparer.OrdinalIgnoreCase))
                    .Where(x => !string.IsNullOrWhiteSpace(x.NguonHienThi) &&
                                x.NguonHienThi.StartsWith("Dự thảo lần ", StringComparison.OrdinalIgnoreCase))
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
                    model.CanhBao = "Cần ít nhất 2 file dự thảo .doc hoặc .docx để thực hiện so sánh.";
                    return new CommonResponse("success", "Thành công", model);
                }

                model.SourceFileId = sourceFileId ?? fileOptions[0].FileId;
                model.TargetFileId = targetFileId ?? fileOptions.FirstOrDefault(x => x.FileId != model.SourceFileId)?.FileId;
                model.SourceFile = fileOptions.FirstOrDefault(x => x.FileId == model.SourceFileId);
                model.TargetFile = fileOptions.FirstOrDefault(x => x.FileId == model.TargetFileId);

                if (model.SourceFile == null || model.TargetFile == null)
                {
                    model.CanhBao = "Không xác định được đủ 2 file để so sánh.";
                    return new CommonResponse("success", "Thành công", model);
                }

                if (!model.SourceFile.LaDocx || !model.TargetFile.LaDocx)
                {
                    model.CanhBao = "Chức năng so sánh nội dung hiện hỗ trợ trực tiếp cho file .docx. Với file .doc, bạn vẫn có thể mở/tải file để đối chiếu thủ công.";
                    return new CommonResponse("success", "Thành công", model);
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
                    model.CanhBao = "Không đọc được nội dung 1 trong 2 file đã chọn.";
                    return new CommonResponse("success", "Thành công", model);
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

                return new CommonResponse("success", "Thành công", model);
            }
            catch (Exception ex)
            {
                return new CommonResponse("error", $"Không thể so sánh dự thảo: {ex.Message}");
            }
        }

        private static string NormalizeDraftCompareSourceLabel(string? sourceLabel)
        {
            if (string.IsNullOrWhiteSpace(sourceLabel))
            {
                return "Dự thảo";
            }

            var normalized = sourceLabel.Trim();
            if (normalized.StartsWith("Phiên bản dự thảo lần ", StringComparison.OrdinalIgnoreCase))
            {
                return "Dự thảo lần " + normalized["Phiên bản dự thảo lần ".Length..];
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
                        return new CommonResponse("error", "ÄÆ¡n vá»‹ hiá»‡n táº¡i khÃ´ng cÃ³ yÃªu cáº§u gÃ³p Ã½ cho há»“ sÆ¡ nÃ y.");
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

                return new CommonResponse("success", "ThÃ nh cÃ´ng", form);
            }
            catch (Exception ex)
            {
                return new CommonResponse("error", $"KhÃ´ng thá»ƒ táº£i form láº¥y gÃ³p Ã½: {ex.Message}");
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
                $"Há»“ sÆ¡ '{hoSo.TenHoSo}' Ä‘Ã£ chuyá»ƒn sang bÆ°á»›c '{nextStep.TenBuoc}'.");
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
            return $"Dự thảo lần {draftVersionNumber}";
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
                return "<span class=\"compare-inline-empty\">(trống)</span>";
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
                return "<span class=\"compare-inline-empty\">(trống)</span>";
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
                return new CommonResponse("error", "Phải đính kèm ít nhất 1 file dự thảo trước khi chuyển thẩm định.");
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
                return new CommonResponse("error", $"Phải có ít nhất 1 file \"{requiredDraftVersionLabel}\" trước khi chuyển xét duyệt dự thảo.");
            }

            var hasWordFile = requiredDraftFiles.Any(file =>
                !string.IsNullOrWhiteSpace(file.FileName) &&
                DraftFileExtensions.Contains(Path.GetExtension(file.FileName).ToLowerInvariant()));

            return hasWordFile
                ? new CommonResponse("success", "Thành công")
                : new CommonResponse("error", $"Phải có ít nhất 1 file \"{requiredDraftVersionLabel}\" định dạng .doc hoặc .docx trước khi chuyển xét duyệt dự thảo.");
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
                TenVersion = $"Phiên bản dự thảo lần {nextVersion + 1}",
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
                    Text = "Đơn vị soạn thảo tự cập nhật kết quả góp ý"
                },
                new()
                {
                    Value = "GUI_DON_VI_GOP_Y",
                    Text = "Gửi lấy góp ý đến từng đơn vị rồi tổng hợp lại"
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
                "BUOC_02_THONG_NHAT" => "Pháº£n há»“i VP UBND vá» Ä‘Äƒng kÃ½ xÃ¢y dá»±ng",
                "BUOC_06_TRINH_THAM_QUYEN" => "Pháº£n há»“i káº¿t quáº£ trÃ¬nh cÆ¡ quan cÃ³ tháº©m quyá»n",
                _ => $"Cáº­p nháº­t bÆ°á»›c {(string.IsNullOrWhiteSpace(tenBuocHienTai) ? "hiá»‡n táº¡i" : tenBuocHienTai)}"
            };
        }

        private static string ResolveStepActionButton(string? maBuocHienTai, string? tenBuocHienTai)
        {
            return maBuocHienTai switch
            {
                "BUOC_01_DANG_KY" => "Chuyá»ƒn Ä‘áº¿n bÆ°á»›c 2",
                "BUOC_02_THONG_NHAT" => "Gá»­i káº¿t quáº£ phÃª duyá»‡t Ä‘Äƒng kÃ½",
                "BUOC_06_TRINH_THAM_QUYEN" => "Gá»­i káº¿t quáº£ phÃª duyá»‡t vÄƒn báº£n",
                _ => $"HoÃ n thÃ nh {(string.IsNullOrWhiteSpace(tenBuocHienTai) ? "bÆ°á»›c hiá»‡n táº¡i" : tenBuocHienTai)}"
            };
        }

        private static string? BuildXuLyGhiChu(string? ghiChu, Guid? attachedFileGroupId)
        {
            var normalizedNote = ghiChu?.Trim();
            if (!attachedFileGroupId.HasValue || attachedFileGroupId.Value == Guid.Empty)
            {
                return string.IsNullOrWhiteSpace(normalizedNote) ? null : normalizedNote;
            }

            var attachedFileNote = $"TÃ i liá»‡u Ä‘Ã­nh kÃ¨m group: {attachedFileGroupId.Value}";
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
                "NHAN_HO_SO" => "ÄÃ£ nháº­n há»“ sÆ¡",
                "NHAN_VA_CHUYEN_PHE_DUYET" => "ÄÃ£ nháº­n vÃ  chuyá»ƒn phÃª duyá»‡t",
                "PHE_DUYET_HO_SO" => "ÄÃ£ phÃª duyá»‡t há»“ sÆ¡",
                "TRA_LAI_HO_SO" => "ÄÃ£ tráº£ láº¡i há»“ sÆ¡",
                "CHUYEN_PHE_DUYET" => "ÄÃ£ chuyá»ƒn phÃª duyá»‡t",
                "CHUYEN_BAN_HANH" => "ÄÃ£ chuyá»ƒn ban hÃ nh",
                "CHUYEN_XET_DUYET_DANH_GIA" => "ÄÃ£ chuyá»ƒn xá»­ lÃ½ Ä‘Ã¡nh giÃ¡",
                "DANG_LAY_GOP_Y" => "Äang láº¥y gÃ³p Ã½",
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
                "NHAN_HO_SO" => "ÄÃ£ nháº­n há»“ sÆ¡ thÃ nh cÃ´ng.",
                "NHAN_VA_CHUYEN_PHE_DUYET" => "ÄÃ£ nháº­n há»“ sÆ¡ vÃ  ghi nháº­n chuyá»ƒn phÃª duyá»‡t.",
                "PHE_DUYET_HO_SO" => "ÄÃ£ ghi nháº­n phÃª duyá»‡t há»“ sÆ¡.",
                "TRA_LAI_HO_SO" => "ÄÃ£ ghi nháº­n tráº£ láº¡i há»“ sÆ¡.",
                "CHUYEN_PHE_DUYET" => "ÄÃ£ ghi nháº­n chuyá»ƒn phÃª duyá»‡t.",
                "CHUYEN_BAN_HANH" => "ÄÃ£ ghi nháº­n chuyá»ƒn ban hÃ nh.",
                "CHUYEN_XET_DUYET_DANH_GIA" => "ÄÃ£ chuyá»ƒn há»“ sÆ¡ sang mÃ n hÃ¬nh xá»­ lÃ½ Ä‘Ã¡nh giÃ¡.",
                "DANG_LAY_GOP_Y" => "ÄÃ£ chuyá»ƒn sang tráng thÃ¡i láº¥y gÃ³p Ã½.",
                _ => "Cáº­p nháº­t tráº¡ng thÃ¡i nghiá»‡p vá»¥ thÃ nh cÃ´ng."
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
                model.GhiChuTheoDoi = "BÆ°á»›c nÃ y chÆ°a phÃ¡t sinh xá»­ lÃ½.";
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
                    model.TenTrangThaiTheoDoi = "Äang xá»­ lÃ½ quÃ¡ háº¡n";
                    model.MaMauTrangThaiTheoDoi = mauQuaHan;
                    model.SoNgayTre = (int)Math.Ceiling((now - processing.HanXuLy.Value).TotalDays);
                    model.GhiChuTheoDoi = $"BÆ°á»›c Ä‘ang xá»­ lÃ½ vÃ  Ä‘Ã£ quÃ¡ háº¡n {Math.Max(model.SoNgayTre ?? 0, 0)} ngÃ y.";
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
                    model.GhiChuTheoDoi = "BÆ°á»›c Ä‘ang xá»­ lÃ½ vÃ  Ä‘Ã£ Ä‘áº¿n ngÆ°á»¡ng cáº£nh bÃ¡o sáº¯p háº¡n.";
                    return model;
                }

                model.MaTrangThaiTheoDoi = "DANG_XU_LY";
                model.TenTrangThaiTheoDoi = "Äang xá»­ lÃ½";
                model.MaMauTrangThaiTheoDoi = mauDangXuLy;
                model.GhiChuTheoDoi = "BÆ°á»›c Ä‘ang Ä‘Æ°á»£c thá»±c hiá»‡n.";
                return model;
            }

            if (processing.NgayXuLy.HasValue)
            {
                if (processing.HanXuLy.HasValue && processing.NgayXuLy.Value > processing.HanXuLy.Value)
                {
                    model.MaTrangThaiTheoDoi = "HOAN_THANH_QUA_HAN";
                    model.TenTrangThaiTheoDoi = "HoÃ n thÃ nh quÃ¡ háº¡n";
                    model.MaMauTrangThaiTheoDoi = mauQuaHan;
                    model.SoNgayTre = (int)Math.Ceiling((processing.NgayXuLy.Value - processing.HanXuLy.Value).TotalDays);
                    model.GhiChuTheoDoi = $"BÆ°á»›c Ä‘Ã£ hoÃ n thÃ nh nhÆ°ng trá»… {Math.Max(model.SoNgayTre ?? 0, 0)} ngÃ y.";
                    return model;
                }

                model.MaTrangThaiTheoDoi = "HOAN_THANH_DUNG_HAN";
                model.TenTrangThaiTheoDoi = "HoÃ n thÃ nh Ä‘Ãºng háº¡n";
                model.MaMauTrangThaiTheoDoi = mauDangXuLy;
                model.GhiChuTheoDoi = "BÆ°á»›c Ä‘Ã£ hoÃ n thÃ nh trong háº¡n.";
                return model;
            }

            model.MaTrangThaiTheoDoi = "CHUA_THUC_HIEN";
            model.TenTrangThaiTheoDoi = "ChÆ°a thá»±c hiá»‡n";
            model.MaMauTrangThaiTheoDoi = "#CED4DA";
            model.GhiChuTheoDoi = "BÆ°á»›c chÆ°a cÃ³ káº¿t quáº£ xá»­ lÃ½.";
            return model;
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
    }
}
