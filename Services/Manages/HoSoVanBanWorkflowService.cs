using DataAccess;
using DataAccess.Entities.Manages;
using DataAccess.Entities.QuanLyDanhMuc;
using Microsoft.EntityFrameworkCore;
using Services.Model;
using Services.Systems;

namespace Services.Manages
{
    public interface IHoSoVanBanWorkflowService
    {
        Task<CommonResponse> GetDanhSachAsync(string search, Guid? donViSoanThaoId = null, int pageSize = 5, int pageCurrent = 1);
        Task<CommonResponse> GetDanhSachDangKyAsync(string search, Guid? donViSoanThaoId = null, int pageSize = 5, int pageCurrent = 1);
        Task<CommonResponse> GetDanhSachTheoBuocAsync(string search, string maBuoc, Guid? donViSoanThaoId = null, int pageSize = 5, int pageCurrent = 1, bool chiLayDonViDangNhap = true, IEnumerable<string>? trangThaiNghiepVuFilters = null);
        Task<List<DonViOptionModel>> GetDonViOptionsAsync();
        Task<List<DataAccess.Entities.QuanLyDanhMuc.DanhMucVanBan>> GetDanhMucVanBanOptionsAsync();
        Task<List<DataAccess.Entities.QuanLyDanhMuc.DanhMucQuyTrinhSoanThao>> GetQuyTrinhOptionsAsync(Guid? danhMucVanBanId = null);
        Task<List<HoSoVanBanBuocThoiHanEditModel>> GetBuocThoiHanOptionsAsync(Guid quyTrinhSoanThaoId);
        Task<CommonResponse> CreateHoSoAsync(HoSoVanBanCreateModel request);
        Task<CommonResponse> GetHoSoEditModelAsync(Guid hoSoVanBanId);
        Task<CommonResponse> GetChuyenHoSoModelAsync(Guid hoSoVanBanId);
        Task<CommonResponse> UpdateHoSoAsync(HoSoVanBanCreateModel request);
        Task<CommonResponse> NhanHoSoAsync(Guid hoSoVanBanId, string actionType = "NHAN_HO_SO");
        Task<CommonResponse> TraLaiDangKyAsync(Guid hoSoVanBanId, string lyDoTraLai, string? ghiChu = null);
        Task<CommonResponse> HoanThanhXuLyAsync(HoSoVanBanXuLyStepModel request);
        Task<CommonResponse> HoanThanhLayYKienAsync(HoSoVanBanLayYKienStepModel request);
        Task<CommonResponse> HoanThanhDanhGiaAsync(HoSoVanBanDanhGiaStepModel request);
        Task<CommonResponse> PhanHoiDanhGiaAsync(HoSoVanBanPhanHoiDanhGiaModel request);
        Task<CommonResponse> GetChiTietAsync(Guid hoSoVanBanId);
    }

    public class HoSoVanBanWorkflowService(
        ApplicationDbContext dbContext,
        IAuthService authService,
        INotificationService notificationService) : IHoSoVanBanWorkflowService
    {
        private readonly ApplicationDbContext _dbContext = dbContext;
        private readonly IAuthService _authService = authService;
        private readonly INotificationService _notificationService = notificationService;

        public async Task<CommonResponse> GetDanhSachAsync(string search, Guid? donViSoanThaoId = null, int pageSize = 5, int pageCurrent = 1)
        {
            return await GetDanhSachInternalAsync(search, pageSize, pageCurrent, false, null, false, donViSoanThaoId);
        }

        public async Task<CommonResponse> GetDanhSachDangKyAsync(string search, Guid? donViSoanThaoId = null, int pageSize = 5, int pageCurrent = 1)
        {
            return await GetDanhSachInternalAsync(search, pageSize, pageCurrent, false, null, false, donViSoanThaoId);
        }

        public async Task<CommonResponse> GetDanhSachTheoBuocAsync(string search, string maBuoc, Guid? donViSoanThaoId = null, int pageSize = 5, int pageCurrent = 1, bool chiLayDonViDangNhap = true, IEnumerable<string>? trangThaiNghiepVuFilters = null)
        {
            return await GetDanhSachInternalAsync(search, pageSize, pageCurrent, false, maBuoc, chiLayDonViDangNhap, donViSoanThaoId, true, trangThaiNghiepVuFilters);
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
            IEnumerable<string>? trangThaiNghiepVuFilters = null)
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
                    where (!chiLayBuocDangKy || (buoc != null && buoc.ThuTuSapXep == 1))
                          && (string.IsNullOrWhiteSpace(maBuoc) ||
                              (chiLayTheoLichSuNhanXuLy
                                  ? _dbContext.HoSoVanBanXuLys.AsNoTracking().Any(x =>
                                      x.HoSoVanBanId == hoSo.Id &&
                                      (isSSA || donViDangNhapId == Guid.Empty || x.DonViXuLyId == donViDangNhapId) &&
                                      _dbContext.DanhMucBuocQuyTrinhs.AsNoTracking().Any(bq => bq.Id == x.BuocQuyTrinhId && bq.MaBuoc == maBuoc))
                                  : (buoc != null && buoc.MaBuoc == maBuoc)))
                          && (!chiLayDonViDangNhap || chiLayTheoLichSuNhanXuLy || isSSA || donViDangNhapId == Guid.Empty || (xuLyCurrent != null && xuLyCurrent.DonViXuLyId == donViDangNhapId))
                          && (!donViSoanThaoId.HasValue || donViSoanThaoId.Value == Guid.Empty ||
                              (chiLayTheoLichSuNhanXuLy
                                  ? _dbContext.HoSoVanBanXuLys.AsNoTracking().Any(x =>
                                      x.HoSoVanBanId == hoSo.Id &&
                                      x.DonViXuLyId == donViSoanThaoId.Value &&
                                      _dbContext.DanhMucBuocQuyTrinhs.AsNoTracking().Any(bq =>
                                          bq.Id == x.BuocQuyTrinhId && bq.MaBuoc == maBuoc))
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
                        TenBuocHienTai = buoc != null ? buoc.TenBuoc : "Đã hoàn thành",
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
                    var trackingMap = await BuildTrackingMapAsync(data.Select(x => x.Id));
                    foreach (var item in data)
                    {
                        item.CanXuLyBuocHienTai = isSSA ||
                                                  donViDangNhapId == Guid.Empty ||
                                                  (item.DonViXuLyHienTaiId.HasValue && item.DonViXuLyHienTaiId.Value == donViDangNhapId);
                        item.DaNhanHoSo = item.NguoiXuLyHienTaiId.HasValue;
                        item.CanNhanHoSo = item.CanXuLyBuocHienTai && !item.NguoiXuLyHienTaiId.HasValue;
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

                        var currentStep = tracking.Steps.FirstOrDefault(x => x.IsCurrent);
                        var latestStep = currentStep ?? tracking.Steps.OrderByDescending(x => x.ThuTuSapXep).FirstOrDefault();
                        if (latestStep != null)
                        {
                            item.TrangThaiTienDo = latestStep.MaTrangThaiTheoDoi;
                            item.TenTrangThaiTienDo = latestStep.TenTrangThaiTheoDoi;
                            item.MaMauTienDo = latestStep.MaMauTrangThaiTheoDoi;
                            item.DangOQuaHan = latestStep.MaTrangThaiTheoDoi is "QUA_HAN" or "HOAN_THANH_QUA_HAN";
                        }

                        if (item.NgayHoanThanh.HasValue ||
                            (item.TongSoBuoc > 0 && item.SoBuocHoanThanh >= item.TongSoBuoc))
                        {
                            item.MaTrangThai = "HOAN_THANH";
                            item.TenTrangThai = "Đã hoàn thành";
                            item.MaMauTrangThai = "#28A745";
                            item.TenBuocHienTai = "Đã hoàn thành";
                            item.CanXuLyBuocHienTai = false;
                        }
                    }

                    foreach (var item in data.Where(x =>
                                 x.NgayHoanThanh.HasValue ||
                                 (x.TongSoBuoc > 0 && x.SoBuocHoanThanh >= x.TongSoBuoc)))
                    {
                        var hoanThanhQuaHan = item.SoBuocQuaHan > 0;
                        item.MaTrangThai = hoanThanhQuaHan ? "HOAN_THANH_QUA_HAN" : "HOAN_THANH_DUNG_HAN";
                        item.TenTrangThai = hoanThanhQuaHan ? "Hoàn thành quá hạn" : "Hoàn thành đúng hạn";
                        item.MaMauTrangThai = hoanThanhQuaHan ? "#DC3545" : "#28A745";
                        item.TenBuocHienTai = "Đã hoàn thành";
                        item.CanXuLyBuocHienTai = false;
                    }
                }

                foreach (var item in data.Where(x =>
                             x.NgayHoanThanh.HasValue ||
                             (x.TongSoBuoc > 0 && x.SoBuocHoanThanh >= x.TongSoBuoc)))
                {
                    var hoanThanhQuaHan = item.SoBuocQuaHan > 0;
                    item.TrangThaiTienDo = hoanThanhQuaHan ? "HOAN_THANH_QUA_HAN" : "HOAN_THANH_DUNG_HAN";
                    item.TenTrangThaiTienDo = hoanThanhQuaHan ? "Hoàn thành quá hạn" : "Hoàn thành đúng hạn";
                    item.MaMauTienDo = hoanThanhQuaHan ? "#DC3545" : "#28A745";
                    item.DangOQuaHan = hoanThanhQuaHan;
                }

                return new CommonResponse("success", "Thành công", data, totalRecord);
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

        public async Task<List<DataAccess.Entities.QuanLyDanhMuc.DanhMucQuyTrinhSoanThao>> GetQuyTrinhOptionsAsync(Guid? danhMucVanBanId = null)
        {
            var query = _dbContext.DanhMucQuyTrinhSoanThaos
                .AsNoTracking()
                .Where(x => x.TrangThai);

            if (danhMucVanBanId.HasValue && danhMucVanBanId.Value != Guid.Empty)
            {
                query = query.Where(x => x.DanhMucVanBanId == danhMucVanBanId.Value);
            }

            return await query.OrderBy(x => x.TenQuyTrinh).ToListAsync();
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
                return new CommonResponse("error", "Không xác định được tài khoản đang thao tác!");
            }

            if (!request.TuNgaySoanThao.HasValue || !request.DenNgaySoanThao.HasValue)
            {
                return new CommonResponse("error", "Thời gian soạn thảo bắt buộc phải nhập.");
            }

            if (request.DenNgaySoanThao.Value.Date < request.TuNgaySoanThao.Value.Date)
            {
                return new CommonResponse("error", "Đến ngày soạn thảo phải lớn hơn hoặc bằng từ ngày soạn thảo.");
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
                return new CommonResponse("error", "Mã hồ sơ đã tồn tại!");
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
                return new CommonResponse("error", "Không tìm thấy quy trình soạn thảo đang kích hoạt!");
            }

            var firstStep = await _dbContext.DanhMucBuocQuyTrinhs
                .AsNoTracking()
                .Where(x => x.QuyTrinhSoanThaoId == request.QuyTrinhSoanThaoId)
                .OrderBy(x => x.ThuTuSapXep)
                .ThenBy(x => x.MaBuoc)
                .FirstOrDefaultAsync();

            if (firstStep == null)
            {
                return new CommonResponse("error", "Quy trình chưa có bước nào để khởi tạo hồ sơ!");
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
                    NoiDungXuLy = "Khởi tạo hồ sơ vào quy trình.",
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
                    $"Hồ sơ '{hoSo.TenHoSo}' đã được khởi tạo và đang ở bước '{firstStep.TenBuoc}'.");

                return new CommonResponse("success", "Thành công", hoSo.Id);
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
                return new CommonResponse("error", "Không xác định được tài khoản đang thao tác!");
            }

            var hoSo = await _dbContext.HoSoVanBans
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == hoSoVanBanId);

            if (hoSo == null)
            {
                return new CommonResponse("error", "Không tìm thấy hồ sơ văn bản!");
            }

            var currentStep = await GetCurrentStepAsync(hoSo);
            var currentProcessing = await _dbContext.HoSoVanBanXuLys
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.HoSoVanBanId == hoSo.Id && x.IsCurrent);

            if (!CanEditDangKyHoSo(currentUser, hoSo, currentStep, currentProcessing))
            {
                return new CommonResponse("error", "Hồ sơ này đã được gửi đi hoặc bạn không có quyền cập nhật.");
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

            return new CommonResponse("success", "Thành công", model);
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

        public async Task<CommonResponse> UpdateHoSoAsync(HoSoVanBanCreateModel request)
        {
            var currentUser = _authService.GetUserInfo();
            if (currentUser == null)
            {
                return new CommonResponse("error", "Không xác định được tài khoản đang thao tác!");
            }

            if (!request.TuNgaySoanThao.HasValue || !request.DenNgaySoanThao.HasValue)
            {
                return new CommonResponse("error", "Thời gian soạn thảo bắt buộc phải nhập.");
            }

            if (request.DenNgaySoanThao.Value.Date < request.TuNgaySoanThao.Value.Date)
            {
                return new CommonResponse("error", "Đến ngày soạn thảo phải lớn hơn hoặc bằng từ ngày soạn thảo.");
            }

            if (request.Id == Guid.Empty)
            {
                return new CommonResponse("error", "Thiếu thông tin hồ sơ cần cập nhật!");
            }

            if (string.IsNullOrWhiteSpace(request.TenHoSo))
            {
                return new CommonResponse("error", "Tên hồ sơ không được để trống!");
            }

            var hoSo = await _dbContext.HoSoVanBans.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (hoSo == null)
            {
                return new CommonResponse("error", "Không tìm thấy hồ sơ văn bản!");
            }

            var currentStep = await GetCurrentStepAsync(hoSo);
            var currentProcessing = await _dbContext.HoSoVanBanXuLys
                .FirstOrDefaultAsync(x => x.HoSoVanBanId == hoSo.Id && x.IsCurrent);

            if (!CanEditDangKyHoSo(currentUser, hoSo, currentStep, currentProcessing))
            {
                return new CommonResponse("error", "Hồ sơ này đã được gửi đi hoặc bạn không có quyền cập nhật.");
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
                return new CommonResponse("error", "Không tìm thấy quy trình soạn thảo đang kích hoạt!");
            }

            var firstStep = await _dbContext.DanhMucBuocQuyTrinhs
                .AsNoTracking()
                .Where(x => x.QuyTrinhSoanThaoId == request.QuyTrinhSoanThaoId)
                .OrderBy(x => x.ThuTuSapXep)
                .ThenBy(x => x.MaBuoc)
                .FirstOrDefaultAsync();

            if (firstStep == null)
            {
                return new CommonResponse("error", "Quy trình chưa có bước nào để cập nhật hồ sơ!");
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
                    currentProcessing.NoiDungXuLy = "Cập nhật hồ sơ đăng ký trước khi chuyển bước tiếp theo.";
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
                return new CommonResponse("success", "Cập nhật hồ sơ thành công", hoSo.Id);
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
                return new CommonResponse("error", "Không xác định được tài khoản đang thao tác!");
            }

            var hoSo = await GetHoSoWithCurrentStepAsync(request.HoSoVanBanId);
            if (hoSo == null)
            {
                return new CommonResponse("error", "Không tìm thấy hồ sơ văn bản!");
            }

            var currentStep = await GetCurrentStepAsync(hoSo);
            if (currentStep == null)
            {
                return new CommonResponse("error", "Ho so chua xac dinh duoc buoc hien tai!");
            }

            if (currentStep.LoaiBuoc == "LayYKien" || currentStep.LoaiBuoc == "DanhGia")
            {
                return new CommonResponse("error", "Bước hiện tại là bước đặc thù. Hãy dùng nghiệp vụ Lấy ý kiến hoặc Đánh giá.");
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

                if (currentProcessing != null && !currentUser.SSA && !currentProcessing.NguoiXuLyId.HasValue)
                {
                    return new CommonResponse("error", "Hồ sơ chưa được nhận. Vui lòng bấm 'Nhận hồ sơ' trước khi xử lý.");
                }

                if (currentProcessing != null)
                {
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

                await AdvanceWorkflowAsync(hoSo, currentUser, nextStep, request.HanXuLy, request.KetQuaXuLy.Trim(), request.NoiDungXuLy, request.DonViTiepNhanId);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return new CommonResponse("success", "Thành công", hoSo.Id);
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
                return new CommonResponse("error", "Hồ sơ chưa phát sinh bước xử lý hiện tại.");
            }

            if (!CanCurrentUserXuLy(currentUser, currentProcessing))
            {
                return new CommonResponse("error", "Hồ sơ này không thuộc đơn vị đang đăng nhập để nhận.");
            }

            if (currentProcessing.NguoiXuLyId.HasValue && !currentUser.SSA)
            {
                return new CommonResponse("success", "Hồ sơ đã được nhận trước đó.");
            }

            currentProcessing.NguoiXuLyId = currentUser.Id;
            currentProcessing.NgayNhan = DateTime.Now;
            await _dbContext.SaveChangesAsync();

            return new CommonResponse("success", "Đã nhận hồ sơ thành công.");
        }

        public async Task<CommonResponse> NhanHoSoAsync(Guid hoSoVanBanId, string actionType = "NHAN_HO_SO")
        {
            var currentUser = _authService.GetUserInfo();
            if (currentUser == null)
            {
                return new CommonResponse("error", "Không xác định được tài khoản đang thao tác!");
            }

            var hoSo = await GetHoSoWithCurrentStepAsync(hoSoVanBanId);
            if (hoSo == null)
            {
                return new CommonResponse("error", "Không tìm thấy hồ sơ văn bản!");
            }

            var currentProcessing = await _dbContext.HoSoVanBanXuLys
                .FirstOrDefaultAsync(x => x.HoSoVanBanId == hoSo.Id && x.IsCurrent);

            if (currentProcessing == null)
            {
                return new CommonResponse("error", "Hồ sơ chưa phát sinh bước xử lý hiện tại.");
            }

            if (!CanCurrentUserXuLy(currentUser, currentProcessing))
            {
                return new CommonResponse("error", "Hồ sơ này không thuộc đơn vị đang đăng nhập để nhận.");
            }

            var actionCode = NormalizeTiepNhanNghiepVuCode(actionType);
            if (string.IsNullOrWhiteSpace(actionCode))
            {
                return new CommonResponse("error", "Thao tác nhận hồ sơ không hợp lệ.");
            }

            var laThaoTacNhanBanDau = actionCode is "NHAN_HO_SO" or "NHAN_VA_CHUYEN_PHE_DUYET" or "PHE_DUYET_HO_SO";
            if (!currentProcessing.NguoiXuLyId.HasValue)
            {
                currentProcessing.NguoiXuLyId = currentUser.Id;
                currentProcessing.NgayNhan = DateTime.Now;
            }
            else if (laThaoTacNhanBanDau && !currentUser.SSA)
            {
                return new CommonResponse("success", "Hồ sơ đã được nhận trước đó.");
            }

            currentProcessing.KetQuaXuLy = actionCode;
            currentProcessing.NoiDungXuLy = BuildTiepNhanNghiepVuNote(actionCode);
            await _dbContext.SaveChangesAsync();

            return new CommonResponse("success", ResolveTiepNhanNghiepVuSuccessMessage(actionCode));
        }

        public async Task<CommonResponse> TraLaiDangKyAsync(Guid hoSoVanBanId, string lyDoTraLai, string? ghiChu = null)
        {
            var currentUser = _authService.GetUserInfo();
            if (currentUser == null)
            {
                return new CommonResponse("error", "Không xác định được tài khoản đang thao tác!");
            }

            if (string.IsNullOrWhiteSpace(lyDoTraLai))
            {
                return new CommonResponse("error", "Bạn phải nhập lý do trả lại hồ sơ.");
            }

            var hoSo = await GetHoSoWithCurrentStepAsync(hoSoVanBanId);
            if (hoSo == null)
            {
                return new CommonResponse("error", "Không tìm thấy hồ sơ văn bản!");
            }

            var currentStep = await GetCurrentStepAsync(hoSo);
            if (currentStep == null || currentStep.MaBuoc != "BUOC_02_THONG_NHAT")
            {
                return new CommonResponse("error", "Hồ sơ hiện không ở bước xét duyệt đăng ký để trả lại.");
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
                    return new CommonResponse("error", "Hồ sơ này không thuộc đơn vị đang đăng nhập để xử lý.");
                }

                if (currentProcessing != null && !currentUser.SSA && !currentProcessing.NguoiXuLyId.HasValue)
                {
                    return new CommonResponse("error", "Hồ sơ chưa được nhận. Vui lòng nhận hồ sơ trước khi trả lại.");
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
                    return new CommonResponse("error", "Không xác định được bước quay lại khi trả hồ sơ.");
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

                return new CommonResponse("success", "Đã trả lại hồ sơ về bước đăng ký.");
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
                return new CommonResponse("error", "Không xác định được tài khoản đang thao tác!");
            }

            var hoSo = await GetHoSoWithCurrentStepAsync(request.HoSoVanBanId);
            if (hoSo == null)
            {
                return new CommonResponse("error", "Không tìm thấy hồ sơ văn bản!");
            }

            var currentStep = await GetCurrentStepAsync(hoSo);
            if (currentStep == null || currentStep.LoaiBuoc != "LayYKien")
            {
                return new CommonResponse("error", "Hồ sơ hiện không ở bước Lấy ý kiến!");
            }

            if (currentStep.YeuCauFileDinhKem && !request.AttachedFileGroupId.HasValue)
            {
                return new CommonResponse("error", "Bước này yêu cầu có file đính kèm kết quả lấy ý kiến!");
            }

            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var layYKien = new HoSoVanBanLayYKien
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

                var currentProcessing = await _dbContext.HoSoVanBanXuLys
                    .Where(x => x.HoSoVanBanId == hoSo.Id && x.IsCurrent)
                    .OrderByDescending(x => x.NgayNhan)
                    .FirstOrDefaultAsync();

                if (!CanCurrentUserXuLy(currentUser, currentProcessing))
                {
                    return new CommonResponse("error", "Ho so nay da duoc chuyen sang don vi khac. Ban khong the cap nhat nua!");
                }

                if (currentProcessing != null)
                {
                    currentProcessing.IsCurrent = false;
                    currentProcessing.NgayXuLy = layYKien.NgayPhanHoi;
                    currentProcessing.KetQuaXuLy = layYKien.TrangThaiPhanHoi;
                    currentProcessing.NoiDungXuLy = layYKien.NoiDungPhanHoi;
                    currentProcessing.GhiChu = layYKien.GhiChu;
                }

                var nextTransition = await GetTransitionAsync(hoSo.QuyTrinhSoanThaoId, currentStep.Id, layYKien.TrangThaiPhanHoi);
                var nextStep = nextTransition == null
                    ? null
                    : await _dbContext.DanhMucBuocQuyTrinhs.AsNoTracking().FirstOrDefaultAsync(x => x.Id == nextTransition.DenBuocId);

                await AdvanceWorkflowAsync(hoSo, currentUser, nextStep, null, layYKien.TrangThaiPhanHoi, layYKien.NoiDungPhanHoi);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return new CommonResponse("success", "Thành công", layYKien.Id);
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
                return new CommonResponse("error", "Không xác định được tài khoản đang thao tác!");
            }

            var hoSo = await GetHoSoWithCurrentStepAsync(request.HoSoVanBanId);
            if (hoSo == null)
            {
                return new CommonResponse("error", "Không tìm thấy hồ sơ văn bản!");
            }

            var currentStep = await GetCurrentStepAsync(hoSo);
            if (currentStep == null || currentStep.LoaiBuoc != "DanhGia")
            {
                return new CommonResponse("error", "Hồ sơ hiện không ở bước Đánh giá/Thẩm định!");
            }

            var ketQua = request.KetQuaDanhGia.Trim().ToUpperInvariant();
            if (ketQua != "DAT" && ketQua != "KHONG_DAT")
            {
                return new CommonResponse("error", "Ket qua danh gia chi chap nhan DAT hoac KHONG_DAT!");
            }

            if (currentStep.YeuCauFileDinhKem && !request.AttachedFileGroupId.HasValue)
            {
                return new CommonResponse("error", "Bước đánh giá hiện tại yêu cầu file đính kèm!");
            }

            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                Guid? traLaiBuocId = null;
                DanhMucBuocQuyTrinh? nextStep = null;
                if (ketQua == "KHONG_DAT")
                {
                    if (hoSo.SoLanTraLaiHienTai >= currentStep.SoLanTraLaiToiDa)
                    {
                        return new CommonResponse("error", $"Da vuot qua so lan tra lai toi da ({currentStep.SoLanTraLaiToiDa}) cua buoc nay!");
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
                        return new CommonResponse("error", "Không tìm thấy bước trả lại theo mã bước đã nhập!");
                    }

                    traLaiBuocId = nextStep.Id;
                }

                var lanDanhGia = await _dbContext.HoSoVanBanDanhGias.CountAsync(x => x.HoSoVanBanId == hoSo.Id) + 1;
                var danhGia = new HoSoVanBanDanhGia
                {
                    HoSoVanBanId = hoSo.Id,
                    BuocQuyTrinhId = currentStep.Id,
                    LanDanhGia = lanDanhGia,
                    DonViDanhGiaId = currentUser.DanhMucDonViId,
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

                var currentProcessing = await _dbContext.HoSoVanBanXuLys
                    .Where(x => x.HoSoVanBanId == hoSo.Id && x.IsCurrent)
                    .OrderByDescending(x => x.NgayNhan)
                    .FirstOrDefaultAsync();

                if (!CanCurrentUserXuLy(currentUser, currentProcessing))
                {
                    return new CommonResponse("error", "Ho so nay da duoc chuyen sang don vi khac. Ban khong the cap nhat nua!");
                }

                if (currentProcessing != null)
                {
                    currentProcessing.IsCurrent = false;
                    currentProcessing.NgayXuLy = DateTime.Now;
                    currentProcessing.KetQuaXuLy = ketQua;
                    currentProcessing.NoiDungXuLy = request.NoiDungDanhGia?.Trim();
                    currentProcessing.GhiChu = request.GhiChu?.Trim();
                }

                if (ketQua == "DAT")
                {
                    var nextTransition = await GetTransitionAsync(hoSo.QuyTrinhSoanThaoId, currentStep.Id, ketQua);
                    nextStep = nextTransition == null
                        ? null
                        : await _dbContext.DanhMucBuocQuyTrinhs.AsNoTracking().FirstOrDefaultAsync(x => x.Id == nextTransition.DenBuocId);
                }
                else
                {
                    hoSo.SoLanTraLaiHienTai += 1;
                }

                await AdvanceWorkflowAsync(hoSo, currentUser, nextStep, null, ketQua, request.NoiDungDanhGia);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return new CommonResponse("success", "Thành công", danhGia.Id);
            }
            catch
            {
                await transaction.RollbackAsync();
                return new CommonResponse();
            }
        }

        public async Task<CommonResponse> PhanHoiDanhGiaAsync(HoSoVanBanPhanHoiDanhGiaModel request)
        {
            var currentUser = _authService.GetUserInfo();
            if (currentUser == null)
            {
                return new CommonResponse("error", "Không xác định được tài khoản đang thao tác!");
            }

            var hoSo = await _dbContext.HoSoVanBans.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.HoSoVanBanId);
            if (hoSo == null)
            {
                return new CommonResponse("error", "Không tìm thấy hồ sơ văn bản!");
            }

            var danhGia = await _dbContext.HoSoVanBanDanhGias
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.HoSoVanBanDanhGiaId && x.HoSoVanBanId == request.HoSoVanBanId);

            if (danhGia == null)
            {
                return new CommonResponse("error", "Không tìm thấy lần đánh giá cần phản hồi!");
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
                return new CommonResponse("success", "Thành công", phanHoi.Id);
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

                    var trackingMap = await BuildTrackingMapAsync(new[] { hoSoVanBanId });
                    if (trackingMap.TryGetValue(hoSoVanBanId, out var tracking))
                    {
                        model.TienDoSummary = tracking.Summary;
                        model.CacBuocTheoDoi = tracking.Steps;
                    }

                    if (model.NgayHoanThanh.HasValue ||
                        (model.TienDoSummary.TongSoBuoc > 0 && model.TienDoSummary.SoBuocHoanThanh >= model.TienDoSummary.TongSoBuoc))
                    {
                        model.TenTrangThai = "Đã hoàn thành";
                        model.MaMauTrangThai = "#28A745";
                        model.TenBuocHienTai = "Đã hoàn thành";
                        model.CanXuLyBuocHienTai = false;
                    }

                    if (model.NgayHoanThanh.HasValue ||
                        (model.TienDoSummary.TongSoBuoc > 0 && model.TienDoSummary.SoBuocHoanThanh >= model.TienDoSummary.TongSoBuoc))
                    {
                        var hoanThanhQuaHan = model.TienDoSummary.SoBuocQuaHan > 0;
                        model.TenTrangThai = hoanThanhQuaHan ? "Hoàn thành quá hạn" : "Hoàn thành đúng hạn";
                        model.MaMauTrangThai = hoanThanhQuaHan ? "#DC3545" : "#28A745";
                        model.TenBuocHienTai = "Đã hoàn thành";
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
                }

                return model == null
                    ? new CommonResponse("error", "Không tìm thấy hồ sơ văn bản!")
                    : new CommonResponse("success", "Thành công", model);
            }
            catch
            {
                return new CommonResponse();
            }
        }

        private async Task<HoSoVanBan?> GetHoSoWithCurrentStepAsync(Guid hoSoVanBanId)
        {
            return await _dbContext.HoSoVanBans.FirstOrDefaultAsync(x => x.Id == hoSoVanBanId);
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
                    .FirstOrDefaultAsync(x => x.QuyTrinhSoanThaoId == quyTrinhId && x.MaBuoc == "BUOC_03_SOAN_THAO");
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
                NoiDungXuLy = $"Chuyển từ bước trước với kết quả '{ketQua}'. {(string.IsNullOrWhiteSpace(noiDung) ? string.Empty : noiDung)}".Trim(),
                GhiChu = null
            };

            _dbContext.HoSoVanBanXuLys.Add(xuLyMoi);

            await TaoThongBaoAsync(
                hoSo,
                nextStep,
                currentUser.DanhMucDonViId,
                donViXuLyId,
                $"Hồ sơ '{hoSo.TenHoSo}' đã chuyển sang bước '{nextStep.TenBuoc}'.");
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

            if (step.MaBuoc is "BUOC_01_DANG_KY" or "BUOC_03_SOAN_THAO")
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

        private static string ResolveDefaultStepResult(string? maBuocHienTai)
        {
            return maBuocHienTai switch
            {
                "BUOC_01_DANG_KY" => "HOAN_THANH_DANG_KY",
                "BUOC_02_THONG_NHAT" => "DONG_Y",
                "BUOC_03_SOAN_THAO" => "HOAN_THANH_DU_THAO",
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
                "BUOC_01_DANG_KY" => "Chuyển văn bản đến đơn vị tiếp nhận",
                "BUOC_02_THONG_NHAT" => "Phản hồi VP UBND về đăng ký xây dựng",
                "BUOC_06_TRINH_THAM_QUYEN" => "Phản hồi kết quả trình cơ quan có thẩm quyền",
                _ => $"Cập nhật bước {(string.IsNullOrWhiteSpace(tenBuocHienTai) ? "hiện tại" : tenBuocHienTai)}"
            };
        }

        private static string ResolveStepActionButton(string? maBuocHienTai, string? tenBuocHienTai)
        {
            return maBuocHienTai switch
            {
                "BUOC_01_DANG_KY" => "Chuyển đến bước 2",
                "BUOC_02_THONG_NHAT" => "Gửi kết quả phê duyệt đăng ký",
                "BUOC_06_TRINH_THAM_QUYEN" => "Gửi kết quả phê duyệt văn bản",
                _ => $"Hoàn thành {(string.IsNullOrWhiteSpace(tenBuocHienTai) ? "bước hiện tại" : tenBuocHienTai)}"
            };
        }

        private static string? BuildXuLyGhiChu(string? ghiChu, Guid? attachedFileGroupId)
        {
            var normalizedNote = ghiChu?.Trim();
            if (!attachedFileGroupId.HasValue || attachedFileGroupId.Value == Guid.Empty)
            {
                return string.IsNullOrWhiteSpace(normalizedNote) ? null : normalizedNote;
            }

            var attachedFileNote = $"Tài liệu đính kèm group: {attachedFileGroupId.Value}";
            if (string.IsNullOrWhiteSpace(normalizedNote))
            {
                return attachedFileNote;
            }

            return $"{normalizedNote} | {attachedFileNote}";
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
                _ => string.Empty
            };
        }

        private static string ResolveTiepNhanNghiepVuLabel(string? actionType)
        {
            return NormalizeTiepNhanNghiepVuCode(actionType) switch
            {
                "NHAN_HO_SO" => "Đã nhận hồ sơ",
                "NHAN_VA_CHUYEN_PHE_DUYET" => "Đã nhận và chuyển phê duyệt",
                "PHE_DUYET_HO_SO" => "Đã phê duyệt hồ sơ",
                "TRA_LAI_HO_SO" => "Đã trả lại hồ sơ",
                "CHUYEN_PHE_DUYET" => "Đã chuyển phê duyệt",
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
                "NHAN_HO_SO" => "Đã nhận hồ sơ thành công.",
                "NHAN_VA_CHUYEN_PHE_DUYET" => "Đã nhận hồ sơ và ghi nhận chuyển phê duyệt.",
                "PHE_DUYET_HO_SO" => "Đã ghi nhận phê duyệt hồ sơ.",
                "TRA_LAI_HO_SO" => "Đã ghi nhận trả lại hồ sơ.",
                "CHUYEN_PHE_DUYET" => "Đã ghi nhận chuyển phê duyệt.",
                _ => "Cập nhật trạng thái nghiệp vụ thành công."
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
                model.TenTrangThaiTheoDoi = "Chưa thực hiện";
                model.MaMauTrangThaiTheoDoi = "#CED4DA";
                model.GhiChuTheoDoi = "Bước này chưa phát sinh xử lý.";
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
                    model.TenTrangThaiTheoDoi = "Đang xử lý quá hạn";
                    model.MaMauTrangThaiTheoDoi = mauQuaHan;
                    model.SoNgayTre = (int)Math.Ceiling((now - processing.HanXuLy.Value).TotalDays);
                    model.GhiChuTheoDoi = $"Bước đang xử lý và đã quá hạn {Math.Max(model.SoNgayTre ?? 0, 0)} ngày.";
                    return model;
                }

                var soNgayCanhBao = model.SoNgayCanhBaoSapHan.GetValueOrDefault(0);
                if (processing.HanXuLy.HasValue &&
                    soNgayCanhBao > 0 &&
                    now >= processing.HanXuLy.Value.AddDays(-soNgayCanhBao))
                {
                    model.MaTrangThaiTheoDoi = "SAP_DEN_HAN";
                    model.TenTrangThaiTheoDoi = "Sắp đến hạn";
                    model.MaMauTrangThaiTheoDoi = mauSapHan;
                    model.GhiChuTheoDoi = "Bước đang xử lý và đã đến ngưỡng cảnh báo sắp hạn.";
                    return model;
                }

                model.MaTrangThaiTheoDoi = "DANG_XU_LY";
                model.TenTrangThaiTheoDoi = "Đang xử lý";
                model.MaMauTrangThaiTheoDoi = mauDangXuLy;
                model.GhiChuTheoDoi = "Bước đang được thực hiện.";
                return model;
            }

            if (processing.NgayXuLy.HasValue)
            {
                if (processing.HanXuLy.HasValue && processing.NgayXuLy.Value > processing.HanXuLy.Value)
                {
                    model.MaTrangThaiTheoDoi = "HOAN_THANH_QUA_HAN";
                    model.TenTrangThaiTheoDoi = "Hoàn thành quá hạn";
                    model.MaMauTrangThaiTheoDoi = mauQuaHan;
                    model.SoNgayTre = (int)Math.Ceiling((processing.NgayXuLy.Value - processing.HanXuLy.Value).TotalDays);
                    model.GhiChuTheoDoi = $"Bước đã hoàn thành nhưng trễ {Math.Max(model.SoNgayTre ?? 0, 0)} ngày.";
                    return model;
                }

                model.MaTrangThaiTheoDoi = "HOAN_THANH_DUNG_HAN";
                model.TenTrangThaiTheoDoi = "Hoàn thành đúng hạn";
                model.MaMauTrangThaiTheoDoi = mauDangXuLy;
                model.GhiChuTheoDoi = "Bước đã hoàn thành trong hạn.";
                return model;
            }

            model.MaTrangThaiTheoDoi = "CHUA_THUC_HIEN";
            model.TenTrangThaiTheoDoi = "Chưa thực hiện";
            model.MaMauTrangThaiTheoDoi = "#CED4DA";
            model.GhiChuTheoDoi = "Bước chưa có kết quả xử lý.";
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
