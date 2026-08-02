using DataAccess;
using DataAccess.Entities.Manages;
using DataAccess.Entities.QuanLyDanhMuc;
using DataAccess.Entities.Settings;
using DataAccess.Entities.Systems;
using Microsoft.EntityFrameworkCore;
using Services.Model;

namespace Services.Manages
{
    public interface IThiHanhPhapLuatService
    {
        Task<CommonResponse> GetDanhSachKeHoachAsync(string search, Guid? donViId, int pageSize = 5, int pageCurrent = 1);
        Task<CommonResponse> GetKeHoachFormAsync(Guid? id);
        Task<CommonResponse> SaveKeHoachAsync(ThiHanhPhapLuatKeHoachFormModel request, User currentUser);
        Task<CommonResponse> GetChiTietKeHoachAsync(Guid id);
        Task<CommonResponse> GetNhiemVuFormAsync(Guid id);
        Task<CommonResponse> GetChiTietNhiemVuFormAsync(Guid id);
        Task<CommonResponse> SaveNhiemVuAsync(ThiHanhPhapLuatNhiemVuFormModel request, User currentUser);
        Task<CommonResponse> SaveChiTietNhiemVuAsync(ThiHanhPhapLuatChiTietNhiemVuFormModel request, User currentUser);
        Task<CommonResponse> DeleteNhiemVuAsync(Guid id);
        Task<CommonResponse> DeleteChiTietNhiemVuAsync(Guid id);
        Task<CommonResponse> GetDanhSachDanhGiaAsync(string search, Guid? donViId, string? canhBao, int pageSize = 5, int pageCurrent = 1);
        Task<CommonResponse> GetDanhSachTienDoAsync(string search, Guid? donViId, int pageSize = 5, int pageCurrent = 1);
        Task<CommonResponse> GetTienDoFormAsync(Guid chiTietNhiemVuId);
        Task<CommonResponse> SaveTienDoAsync(ThiHanhPhapLuatTienDoFormModel request, User currentUser);
        Task<CommonResponse> GetDanhGiaFormAsync(Guid keHoachId);
        Task<CommonResponse> SaveDanhGiaAsync(ThiHanhPhapLuatDanhGiaFormModel request, User currentUser);
        Task<CommonResponse> GetTongHopFormAsync(Guid keHoachId);
        Task<CommonResponse> SaveTongHopAsync(ThiHanhPhapLuatTongHopFormModel request, User currentUser);
        Task<List<DanhMucDonVi>> GetDonViOptionsAsync();
        Task<List<User>> GetNguoiDungOptionsAsync(Guid? donViId = null);
        Task<List<DanhMucVanBan>> GetDanhMucVanBanOptionsAsync();
    }

    public class ThiHanhPhapLuatService(ApplicationDbContext dbContext) : IThiHanhPhapLuatService
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        public async Task<CommonResponse> GetDanhSachKeHoachAsync(string search, Guid? donViId, int pageSize = 5, int pageCurrent = 1)
        {
            try
            {
                var keHoachQuery = _dbContext.ThiHanhPhapLuatKeHoachs.AsNoTracking().AsQueryable();

                if (!string.IsNullOrWhiteSpace(search))
                {
                    search = search.Trim();
                    keHoachQuery = keHoachQuery.Where(x =>
                        x.MaKeHoach.Contains(search) ||
                        x.TenKeHoach.Contains(search) ||
                        (x.SoKyHieuVanBanCanCu != null && x.SoKyHieuVanBanCanCu.Contains(search)) ||
                        (x.TrichYeuVanBanCanCu != null && x.TrichYeuVanBanCanCu.Contains(search)) ||
                        (x.CoQuanBanHanhVanBanCanCu != null && x.CoQuanBanHanhVanBanCanCu.Contains(search)));
                }

                if (donViId.HasValue && donViId.Value != Guid.Empty)
                {
                    keHoachQuery = keHoachQuery.Where(x => x.DonViChuTriId == donViId.Value);
                }

                var totalRecord = await keHoachQuery.CountAsync();

                var keHoachs = await keHoachQuery
                    .OrderByDescending(x => x.Nam)
                    .ThenByDescending(x => x.CreatedDate)
                    .Skip((pageCurrent - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var keHoachIds = keHoachs.Select(x => x.Id).ToList();
                var donViIds = keHoachs.Select(x => x.DonViChuTriId).Distinct().ToList();
                var vanBanIds = keHoachs.Where(x => x.DanhMucVanBanId.HasValue).Select(x => x.DanhMucVanBanId!.Value).Distinct().ToList();

                var taskStats = await _dbContext.ThiHanhPhapLuatNhiemVus.AsNoTracking()
                    .Where(x => keHoachIds.Contains(x.KeHoachId))
                    .GroupBy(x => x.KeHoachId)
                    .Select(g => new
                    {
                        KeHoachId = g.Key,
                        TongSoNhiemVu = g.Count(),
                        SoNhiemVuHoanThanh = g.Count(x => x.TrangThai == "HOAN_THANH")
                    })
                    .ToListAsync();

                var chiTietStats = await _dbContext.ThiHanhPhapLuatChiTietNhiemVus.AsNoTracking()
                    .Where(x => _dbContext.ThiHanhPhapLuatNhiemVus.Where(nv => keHoachIds.Contains(nv.KeHoachId)).Select(nv => nv.Id).Contains(x.NhiemVuId))
                    .Join(_dbContext.ThiHanhPhapLuatNhiemVus.AsNoTracking(),
                        ct => ct.NhiemVuId,
                        nv => nv.Id,
                        (ct, nv) => new { ct, nv.KeHoachId })
                    .GroupBy(x => x.KeHoachId)
                    .Select(g => new
                    {
                        KeHoachId = g.Key,
                        TongSoChiTiet = g.Count(),
                        SoChiTietHoanThanh = g.Count(x => x.ct.TrangThai == "HOAN_THANH")
                    })
                    .ToListAsync();

                var donViMap = await _dbContext.DanhMucDonVis.AsNoTracking()
                    .Where(x => donViIds.Contains(x.Id))
                    .ToDictionaryAsync(x => x.Id, x => x.TenDonVi);

                var vanBanMap = await _dbContext.DanhMucVanBans.AsNoTracking()
                    .Where(x => vanBanIds.Contains(x.Id))
                    .ToDictionaryAsync(x => x.Id, x => x.TenLoaiVanBan);

                var data = keHoachs.Select(x =>
                {
                    var taskStat = taskStats.FirstOrDefault(t => t.KeHoachId == x.Id);
                    var chiTietStat = chiTietStats.FirstOrDefault(t => t.KeHoachId == x.Id);
                    return new ThiHanhPhapLuatKeHoachListItemModel
                    {
                        Id = x.Id,
                        MaKeHoach = x.MaKeHoach,
                        TenKeHoach = x.TenKeHoach,
                        Nam = x.Nam,
                        TenLoaiVanBan = x.DanhMucVanBanId.HasValue && vanBanMap.TryGetValue(x.DanhMucVanBanId.Value, out var tenLoaiVanBan) ? tenLoaiVanBan : null,
                        TenDonViChuTri = donViMap.TryGetValue(x.DonViChuTriId, out var tenDonVi) ? tenDonVi : string.Empty,
                        NgayBatDau = x.NgayBatDau,
                        NgayKetThuc = x.NgayKetThuc,
                        NgayCongBo = x.NgayCongBo,
                        TrangThai = x.TrangThai,
                        TongSoNhiemVu = taskStat?.TongSoNhiemVu ?? 0,
                        SoNhiemVuHoanThanh = taskStat?.SoNhiemVuHoanThanh ?? 0,
                        TongSoChiTiet = chiTietStat?.TongSoChiTiet ?? 0,
                        SoChiTietHoanThanh = chiTietStat?.SoChiTietHoanThanh ?? 0
                    };
                }).ToList();

                return new CommonResponse("success", "ThÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â nh cÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â´ng", data, totalRecord);
            }
            catch
            {
                return new CommonResponse();
            }
        }

        public async Task<CommonResponse> GetKeHoachFormAsync(Guid? id)
        {
            try
            {
                if (!id.HasValue || id.Value == Guid.Empty)
                {
                    return new CommonResponse("success", "ThÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â nh cÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â´ng", new ThiHanhPhapLuatKeHoachFormModel
                    {
                        Nam = DateTime.Now.Year,
                        TrangThai = "NHAP"
                    });
                }

                var entity = await _dbContext.ThiHanhPhapLuatKeHoachs.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id.Value);
                if (entity == null)
                {
                    return new CommonResponse("error", "KhÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â´ng tÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â¬m thĂ„â€Ă‚Â¡Ä‚â€Ă‚ÂºÄ‚â€Ă‚Â¥y kĂ„â€Ă‚Â¡Ä‚â€Ă‚ÂºÄ‚â€Ă‚Â¿ hoĂ„â€Ă‚Â¡Ä‚â€Ă‚ÂºÄ‚â€Ă‚Â¡ch.");
                }

                return new CommonResponse("success", "ThÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â nh cÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â´ng", new ThiHanhPhapLuatKeHoachFormModel
                {
                    Id = entity.Id,
                    MaKeHoach = entity.MaKeHoach,
                    TenKeHoach = entity.TenKeHoach,
                    Nam = entity.Nam,
                    DanhMucVanBanId = entity.DanhMucVanBanId,
                    SoKyHieuVanBanCanCu = entity.SoKyHieuVanBanCanCu,
                    NgayBanHanhVanBanCanCu = entity.NgayBanHanhVanBanCanCu,
                    TrichYeuVanBanCanCu = entity.TrichYeuVanBanCanCu,
                    CoQuanBanHanhVanBanCanCu = entity.CoQuanBanHanhVanBanCanCu,
                    DonViChuTriId = entity.DonViChuTriId,
                    NgayBatDau = entity.NgayBatDau,
                    NgayKetThuc = entity.NgayKetThuc,
                    NgayCongBo = entity.NgayCongBo,
                    TrangThai = entity.TrangThai,
                    MoTa = entity.MoTa,
                    GhiChu = entity.GhiChu,
                    AttachedFileGroupId = entity.AttachedFileGroupId
                });
            }
            catch
            {
                return new CommonResponse();
            }
        }

        public async Task<CommonResponse> SaveKeHoachAsync(ThiHanhPhapLuatKeHoachFormModel request, User currentUser)
        {
            try
            {
                var validationMessage = ValidateKeHoach(request);
                if (!string.IsNullOrWhiteSpace(validationMessage))
                {
                    return new CommonResponse("error", validationMessage);
                }

                if (await _dbContext.ThiHanhPhapLuatKeHoachs.AnyAsync(x => x.MaKeHoach == request.MaKeHoach.Trim() && x.Id != request.Id))
                {
                    return new CommonResponse("error", "MÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â£ kĂ„â€Ă‚Â¡Ä‚â€Ă‚ÂºÄ‚â€Ă‚Â¿ hoĂ„â€Ă‚Â¡Ä‚â€Ă‚ÂºÄ‚â€Ă‚Â¡ch Ă„â€Ă¢â‚¬ÂÄ‚Â¢Ă¢â€Â¬Ă‹Å“Ä‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â£ tĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚Â¢Ă¢â€Â¬Ă…â€œn tĂ„â€Ă‚Â¡Ä‚â€Ă‚ÂºÄ‚â€Ă‚Â¡i.");
                }

                var now = DateTime.Now;
                if (request.Id == Guid.Empty)
                {
                    var entity = new ThiHanhPhapLuatKeHoach
                    {
                        MaKeHoach = request.MaKeHoach.Trim(),
                        TenKeHoach = request.TenKeHoach.Trim(),
                        Nam = request.Nam,
                        DanhMucVanBanId = request.DanhMucVanBanId,
                        SoKyHieuVanBanCanCu = NullIfWhiteSpace(request.SoKyHieuVanBanCanCu),
                        NgayBanHanhVanBanCanCu = request.NgayBanHanhVanBanCanCu,
                        TrichYeuVanBanCanCu = NullIfWhiteSpace(request.TrichYeuVanBanCanCu),
                        CoQuanBanHanhVanBanCanCu = NullIfWhiteSpace(request.CoQuanBanHanhVanBanCanCu),
                        DonViChuTriId = request.DonViChuTriId,
                        NgayBatDau = request.NgayBatDau,
                        NgayKetThuc = request.NgayKetThuc,
                        NgayCongBo = request.NgayCongBo,
                        TrangThai = request.TrangThai,
                        MoTa = NullIfWhiteSpace(request.MoTa),
                        GhiChu = NullIfWhiteSpace(request.GhiChu),
                        AttachedFileGroupId = request.AttachedFileGroupId,
                        CreatedBy = currentUser.Id,
                        CreatedDate = now,
                        UpdatedBy = currentUser.Id,
                        UpdatedDate = now
                    };

                    _dbContext.ThiHanhPhapLuatKeHoachs.Add(entity);
                }
                else
                {
                    var entity = await _dbContext.ThiHanhPhapLuatKeHoachs.FirstOrDefaultAsync(x => x.Id == request.Id);
                    if (entity == null)
                    {
                        return new CommonResponse("error", "KhÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â´ng tÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â¬m thĂ„â€Ă‚Â¡Ä‚â€Ă‚ÂºÄ‚â€Ă‚Â¥y kĂ„â€Ă‚Â¡Ä‚â€Ă‚ÂºÄ‚â€Ă‚Â¿ hoĂ„â€Ă‚Â¡Ä‚â€Ă‚ÂºÄ‚â€Ă‚Â¡ch.");
                    }

                    entity.MaKeHoach = request.MaKeHoach.Trim();
                    entity.TenKeHoach = request.TenKeHoach.Trim();
                    entity.Nam = request.Nam;
                    entity.DanhMucVanBanId = request.DanhMucVanBanId;
                    entity.SoKyHieuVanBanCanCu = NullIfWhiteSpace(request.SoKyHieuVanBanCanCu);
                    entity.NgayBanHanhVanBanCanCu = request.NgayBanHanhVanBanCanCu;
                    entity.TrichYeuVanBanCanCu = NullIfWhiteSpace(request.TrichYeuVanBanCanCu);
                    entity.CoQuanBanHanhVanBanCanCu = NullIfWhiteSpace(request.CoQuanBanHanhVanBanCanCu);
                    entity.DonViChuTriId = request.DonViChuTriId;
                    entity.NgayBatDau = request.NgayBatDau;
                    entity.NgayKetThuc = request.NgayKetThuc;
                    entity.NgayCongBo = request.NgayCongBo;
                    entity.TrangThai = request.TrangThai;
                    entity.MoTa = NullIfWhiteSpace(request.MoTa);
                    entity.GhiChu = NullIfWhiteSpace(request.GhiChu);
                    entity.AttachedFileGroupId = request.AttachedFileGroupId;
                    entity.UpdatedBy = currentUser.Id;
                    entity.UpdatedDate = now;
                }

                await _dbContext.SaveChangesAsync();
                return new CommonResponse("success", "LĂ„â€Ă¢â‚¬Â Ä‚â€Ă‚Â°u kĂ„â€Ă‚Â¡Ä‚â€Ă‚ÂºÄ‚â€Ă‚Â¿ hoĂ„â€Ă‚Â¡Ä‚â€Ă‚ÂºÄ‚â€Ă‚Â¡ch thÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â nh cÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â´ng.");
            }
            catch
            {
                return new CommonResponse();
            }
        }

        public async Task<CommonResponse> GetChiTietKeHoachAsync(Guid id)
        {
            try
            {
                var keHoach = await _dbContext.ThiHanhPhapLuatKeHoachs.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                if (keHoach == null)
                {
                    return new CommonResponse("error", "KhÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â´ng tÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â¬m thĂ„â€Ă‚Â¡Ä‚â€Ă‚ÂºÄ‚â€Ă‚Â¥y kĂ„â€Ă‚Â¡Ä‚â€Ă‚ÂºÄ‚â€Ă‚Â¿ hoĂ„â€Ă‚Â¡Ä‚â€Ă‚ÂºÄ‚â€Ă‚Â¡ch.");
                }

                var donViMap = await _dbContext.DanhMucDonVis.AsNoTracking().ToDictionaryAsync(x => x.Id, x => x.TenDonVi);
                var userMap = await _dbContext.Users.AsNoTracking().ToDictionaryAsync(x => x.Id, x => x.Name);
                var danhMucVanBanMap = await _dbContext.DanhMucVanBans.AsNoTracking().ToDictionaryAsync(x => x.Id, x => x.TenLoaiVanBan);

                var nhiemVus = await _dbContext.ThiHanhPhapLuatNhiemVus.AsNoTracking()
                    .Where(x => x.KeHoachId == id)
                    .OrderBy(x => x.ThuTuSapXep)
                    .ThenBy(x => x.CreatedDate)
                    .ToListAsync();

                var nhiemVuIds = nhiemVus.Select(x => x.Id).ToList();

                var chiTiets = await _dbContext.ThiHanhPhapLuatChiTietNhiemVus.AsNoTracking()
                    .Where(x => nhiemVuIds.Contains(x.NhiemVuId))
                    .OrderBy(x => x.ThuTuSapXep)
                    .ThenBy(x => x.CreatedDate)
                    .ToListAsync();

                var chiTietPhoiHops = await _dbContext.ThiHanhPhapLuatChiTietPhoiHops.AsNoTracking()
                    .Where(x => chiTiets.Select(ct => ct.Id).Contains(x.ChiTietNhiemVuId))
                    .ToListAsync();

                var data = new ThiHanhPhapLuatKeHoachDetailModel
                {
                    Id = keHoach.Id,
                    MaKeHoach = keHoach.MaKeHoach,
                    TenKeHoach = keHoach.TenKeHoach,
                    Nam = keHoach.Nam,
                    TenLoaiVanBan = keHoach.DanhMucVanBanId.HasValue && danhMucVanBanMap.TryGetValue(keHoach.DanhMucVanBanId.Value, out var tenLoaiVanBan) ? tenLoaiVanBan : null,
                    SoKyHieuVanBanCanCu = keHoach.SoKyHieuVanBanCanCu,
                    NgayBanHanhVanBanCanCu = keHoach.NgayBanHanhVanBanCanCu,
                    TrichYeuVanBanCanCu = keHoach.TrichYeuVanBanCanCu,
                    CoQuanBanHanhVanBanCanCu = keHoach.CoQuanBanHanhVanBanCanCu,
                    TenDonViChuTri = donViMap.TryGetValue(keHoach.DonViChuTriId, out var tenDonViChuTri) ? tenDonViChuTri : string.Empty,
                    NgayBatDau = keHoach.NgayBatDau,
                    NgayKetThuc = keHoach.NgayKetThuc,
                    NgayCongBo = keHoach.NgayCongBo,
                    TrangThai = keHoach.TrangThai,
                    MoTa = keHoach.MoTa,
                    GhiChu = keHoach.GhiChu,
                    AttachedFileGroupId = keHoach.AttachedFileGroupId,
                    NhiemVus = nhiemVus.Select(nv => new ThiHanhPhapLuatNhiemVuDetailModel
                    {
                        Id = nv.Id,
                        KeHoachId = nv.KeHoachId,
                        MaNhiemVu = nv.MaNhiemVu,
                        TenNhiemVu = nv.TenNhiemVu,
                        NoiDungNhiemVu = nv.NoiDungNhiemVu,
                        DonViChuTriId = nv.DonViChuTriId,
                        NguoiDieuPhoiId = nv.NguoiDieuPhoiId,
                        TenDonViChuTri = donViMap.TryGetValue(nv.DonViChuTriId, out var tenDonVi) ? tenDonVi : string.Empty,
                        TenNguoiDieuPhoi = nv.NguoiDieuPhoiId.HasValue && userMap.TryGetValue(nv.NguoiDieuPhoiId.Value, out var tenNguoiDieuPhoi) ? tenNguoiDieuPhoi : null,
                        NgayBatDau = nv.NgayBatDau,
                        HanHoanThanh = nv.HanHoanThanh,
                        MucDoUuTien = nv.MucDoUuTien,
                        TrangThai = nv.TrangThai,
                        ThuTuSapXep = nv.ThuTuSapXep,
                        YeuCauBaoCao = nv.YeuCauBaoCao,
                        GhiChu = nv.GhiChu,
                        ChiTiets = chiTiets.Where(ct => ct.NhiemVuId == nv.Id).Select(ct => new ThiHanhPhapLuatChiTietNhiemVuDetailModel
                        {
                            Id = ct.Id,
                            NhiemVuId = ct.NhiemVuId,
                            MaChiTiet = ct.MaChiTiet,
                            TenChiTiet = ct.TenChiTiet,
                            NoiDungChiTiet = ct.NoiDungChiTiet,
                            LoaiChiTiet = ct.LoaiChiTiet,
                            DonViThucHienId = ct.DonViThucHienId,
                            NguoiPhuTrachChinhId = ct.NguoiPhuTrachChinhId,
                            TenDonViThucHien = donViMap.TryGetValue(ct.DonViThucHienId, out var tenDonViThucHien) ? tenDonViThucHien : string.Empty,
                            TenNguoiPhuTrachChinh = ct.NguoiPhuTrachChinhId.HasValue && userMap.TryGetValue(ct.NguoiPhuTrachChinhId.Value, out var tenNguoiPhuTrach) ? tenNguoiPhuTrach : null,
                            NgayBatDau = ct.NgayBatDau,
                            HanHoanThanh = ct.HanHoanThanh,
                            TrangThai = ct.TrangThai,
                            TyLeHoanThanh = ct.TyLeHoanThanh,
                            KetQuaYeuCau = ct.KetQuaYeuCau,
                            GiaTriChiTieu = ct.GiaTriChiTieu,
                            DonViTinh = ct.DonViTinh,
                            ThuTuSapXep = ct.ThuTuSapXep,
                            GhiChu = ct.GhiChu,
                            NguoiDungPhoiHops = chiTietPhoiHops
                                .Where(p => p.ChiTietNhiemVuId == ct.Id)
                                .Select(p => userMap.TryGetValue(p.NguoiDungId, out var tenNguoiPhoiHop) ? tenNguoiPhoiHop : string.Empty)
                                .Where(x => !string.IsNullOrWhiteSpace(x))
                                .Distinct()
                                .ToList()
                        }).ToList()
                    }).ToList()
                };

                return new CommonResponse("success", "ThÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â nh cÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â´ng", data);
            }
            catch
            {
                return new CommonResponse();
            }
        }

        public async Task<CommonResponse> GetNhiemVuFormAsync(Guid id)
        {
            try
            {
                var entity = await _dbContext.ThiHanhPhapLuatNhiemVus.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                {
                    return new CommonResponse("error", "KhÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â´ng tÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â¬m thĂ„â€Ă‚Â¡Ä‚â€Ă‚ÂºÄ‚â€Ă‚Â¥y nhiĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚Â¢Ă¢â€Â¬Ă‚Â¡m vĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚â€Ă‚Â¥.");
                }

                return new CommonResponse("success", "ThÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â nh cÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â´ng", new ThiHanhPhapLuatNhiemVuFormModel
                {
                    Id = entity.Id,
                    KeHoachId = entity.KeHoachId,
                    MaNhiemVu = entity.MaNhiemVu,
                    TenNhiemVu = entity.TenNhiemVu,
                    NoiDungNhiemVu = entity.NoiDungNhiemVu,
                    DonViChuTriId = entity.DonViChuTriId,
                    NguoiDieuPhoiId = entity.NguoiDieuPhoiId,
                    NgayBatDau = entity.NgayBatDau,
                    HanHoanThanh = entity.HanHoanThanh,
                    MucDoUuTien = entity.MucDoUuTien,
                    TrangThai = entity.TrangThai,
                    ThuTuSapXep = entity.ThuTuSapXep,
                    YeuCauBaoCao = entity.YeuCauBaoCao,
                    GhiChu = entity.GhiChu
                });
            }
            catch
            {
                return new CommonResponse();
            }
        }

        public async Task<CommonResponse> GetChiTietNhiemVuFormAsync(Guid id)
        {
            try
            {
                var entity = await _dbContext.ThiHanhPhapLuatChiTietNhiemVus.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                {
                    return new CommonResponse("error", "KhÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â´ng tÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â¬m thĂ„â€Ă‚Â¡Ä‚â€Ă‚ÂºÄ‚â€Ă‚Â¥y nhiĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚Â¢Ă¢â€Â¬Ă‚Â¡m vĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚â€Ă‚Â¥ nhĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚â€Ă‚Â.");
                }

                var phoiHopIds = await _dbContext.ThiHanhPhapLuatChiTietPhoiHops.AsNoTracking()
                    .Where(x => x.ChiTietNhiemVuId == id)
                    .Select(x => x.NguoiDungId)
                    .ToListAsync();

                return new CommonResponse("success", "ThÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â nh cÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â´ng", new ThiHanhPhapLuatChiTietNhiemVuFormModel
                {
                    Id = entity.Id,
                    NhiemVuId = entity.NhiemVuId,
                    MaChiTiet = entity.MaChiTiet,
                    TenChiTiet = entity.TenChiTiet,
                    NoiDungChiTiet = entity.NoiDungChiTiet,
                    LoaiChiTiet = entity.LoaiChiTiet,
                    DonViThucHienId = entity.DonViThucHienId,
                    NguoiPhuTrachChinhId = entity.NguoiPhuTrachChinhId,
                    NgayBatDau = entity.NgayBatDau,
                    HanHoanThanh = entity.HanHoanThanh,
                    TrangThai = entity.TrangThai,
                    TyLeHoanThanh = entity.TyLeHoanThanh,
                    KetQuaYeuCau = entity.KetQuaYeuCau,
                    GiaTriChiTieu = entity.GiaTriChiTieu,
                    DonViTinh = entity.DonViTinh,
                    ThuTuSapXep = entity.ThuTuSapXep,
                    GhiChu = entity.GhiChu,
                    NguoiDungPhoiHopIds = phoiHopIds
                });
            }
            catch
            {
                return new CommonResponse();
            }
        }

        public async Task<CommonResponse> SaveNhiemVuAsync(ThiHanhPhapLuatNhiemVuFormModel request, User currentUser)
        {
            try
            {
                var validationMessage = ValidateNhiemVu(request);
                if (!string.IsNullOrWhiteSpace(validationMessage))
                {
                    return new CommonResponse("error", validationMessage);
                }

                if (!await _dbContext.ThiHanhPhapLuatKeHoachs.AnyAsync(x => x.Id == request.KeHoachId))
                {
                    return new CommonResponse("error", "KĂ„â€Ă‚Â¡Ä‚â€Ă‚ÂºÄ‚â€Ă‚Â¿ hoĂ„â€Ă‚Â¡Ä‚â€Ă‚ÂºÄ‚â€Ă‚Â¡ch khÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â´ng tĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚Â¢Ă¢â€Â¬Ă…â€œn tĂ„â€Ă‚Â¡Ä‚â€Ă‚ÂºÄ‚â€Ă‚Â¡i.");
                }

                if (await _dbContext.ThiHanhPhapLuatNhiemVus.AnyAsync(x => x.KeHoachId == request.KeHoachId && x.MaNhiemVu == request.MaNhiemVu.Trim() && x.Id != request.Id))
                {
                    return new CommonResponse("error", "MÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â£ nhiĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚Â¢Ă¢â€Â¬Ă‚Â¡m vĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚â€Ă‚Â¥ Ă„â€Ă¢â‚¬ÂÄ‚Â¢Ă¢â€Â¬Ă‹Å“Ä‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â£ tĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚Â¢Ă¢â€Â¬Ă…â€œn tĂ„â€Ă‚Â¡Ä‚â€Ă‚ÂºÄ‚â€Ă‚Â¡i trong kĂ„â€Ă‚Â¡Ä‚â€Ă‚ÂºÄ‚â€Ă‚Â¿ hoĂ„â€Ă‚Â¡Ä‚â€Ă‚ÂºÄ‚â€Ă‚Â¡ch.");
                }

                var now = DateTime.Now;
                if (request.Id == Guid.Empty)
                {
                    _dbContext.ThiHanhPhapLuatNhiemVus.Add(new ThiHanhPhapLuatNhiemVu
                    {
                        KeHoachId = request.KeHoachId,
                        MaNhiemVu = request.MaNhiemVu.Trim(),
                        TenNhiemVu = request.TenNhiemVu.Trim(),
                        NoiDungNhiemVu = NullIfWhiteSpace(request.NoiDungNhiemVu),
                        DonViChuTriId = request.DonViChuTriId,
                        NguoiDieuPhoiId = request.NguoiDieuPhoiId,
                        NgayBatDau = request.NgayBatDau,
                        HanHoanThanh = request.HanHoanThanh,
                        MucDoUuTien = request.MucDoUuTien,
                        TrangThai = request.TrangThai,
                        ThuTuSapXep = request.ThuTuSapXep,
                        YeuCauBaoCao = request.YeuCauBaoCao,
                        GhiChu = NullIfWhiteSpace(request.GhiChu),
                        CreatedBy = currentUser.Id,
                        CreatedDate = now,
                        UpdatedBy = currentUser.Id,
                        UpdatedDate = now
                    });
                }
                else
                {
                    var entity = await _dbContext.ThiHanhPhapLuatNhiemVus.FirstOrDefaultAsync(x => x.Id == request.Id);
                    if (entity == null)
                    {
                        return new CommonResponse("error", "KhÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â´ng tÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â¬m thĂ„â€Ă‚Â¡Ä‚â€Ă‚ÂºÄ‚â€Ă‚Â¥y nhiĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚Â¢Ă¢â€Â¬Ă‚Â¡m vĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚â€Ă‚Â¥.");
                    }

                    entity.MaNhiemVu = request.MaNhiemVu.Trim();
                    entity.TenNhiemVu = request.TenNhiemVu.Trim();
                    entity.NoiDungNhiemVu = NullIfWhiteSpace(request.NoiDungNhiemVu);
                    entity.DonViChuTriId = request.DonViChuTriId;
                    entity.NguoiDieuPhoiId = request.NguoiDieuPhoiId;
                    entity.NgayBatDau = request.NgayBatDau;
                    entity.HanHoanThanh = request.HanHoanThanh;
                    entity.MucDoUuTien = request.MucDoUuTien;
                    entity.TrangThai = request.TrangThai;
                    entity.ThuTuSapXep = request.ThuTuSapXep;
                    entity.YeuCauBaoCao = request.YeuCauBaoCao;
                    entity.GhiChu = NullIfWhiteSpace(request.GhiChu);
                    entity.UpdatedBy = currentUser.Id;
                    entity.UpdatedDate = now;
                }

                await _dbContext.SaveChangesAsync();
                return new CommonResponse("success", "LĂ„â€Ă¢â‚¬Â Ä‚â€Ă‚Â°u nhiĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚Â¢Ă¢â€Â¬Ă‚Â¡m vĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚â€Ă‚Â¥ thÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â nh cÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â´ng.");
            }
            catch
            {
                return new CommonResponse();
            }
        }

        public async Task<CommonResponse> SaveChiTietNhiemVuAsync(ThiHanhPhapLuatChiTietNhiemVuFormModel request, User currentUser)
        {
            try
            {
                var validationMessage = ValidateChiTietNhiemVu(request);
                if (!string.IsNullOrWhiteSpace(validationMessage))
                {
                    return new CommonResponse("error", validationMessage);
                }

                if (!await _dbContext.ThiHanhPhapLuatNhiemVus.AnyAsync(x => x.Id == request.NhiemVuId))
                {
                    return new CommonResponse("error", "NhiĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚Â¢Ă¢â€Â¬Ă‚Â¡m vĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚â€Ă‚Â¥ khÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â´ng tĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚Â¢Ă¢â€Â¬Ă…â€œn tĂ„â€Ă‚Â¡Ä‚â€Ă‚ÂºÄ‚â€Ă‚Â¡i.");
                }

                if (await _dbContext.ThiHanhPhapLuatChiTietNhiemVus.AnyAsync(x => x.NhiemVuId == request.NhiemVuId && x.MaChiTiet == request.MaChiTiet.Trim() && x.Id != request.Id))
                {
                    return new CommonResponse("error", "MÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â£ nhiĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚Â¢Ă¢â€Â¬Ă‚Â¡m vĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚â€Ă‚Â¥ nhĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚â€Ă‚Â Ă„â€Ă¢â‚¬ÂÄ‚Â¢Ă¢â€Â¬Ă‹Å“Ä‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â£ tĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚Â¢Ă¢â€Â¬Ă…â€œn tĂ„â€Ă‚Â¡Ä‚â€Ă‚ÂºÄ‚â€Ă‚Â¡i trong nhiĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚Â¢Ă¢â€Â¬Ă‚Â¡m vĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚â€Ă‚Â¥.");
                }

                var now = DateTime.Now;
                ThiHanhPhapLuatChiTietNhiemVu entity;
                if (request.Id == Guid.Empty)
                {
                    entity = new ThiHanhPhapLuatChiTietNhiemVu
                    {
                        NhiemVuId = request.NhiemVuId,
                        MaChiTiet = request.MaChiTiet.Trim(),
                        TenChiTiet = request.TenChiTiet.Trim(),
                        NoiDungChiTiet = NullIfWhiteSpace(request.NoiDungChiTiet),
                        LoaiChiTiet = request.LoaiChiTiet,
                        DonViThucHienId = request.DonViThucHienId,
                        NguoiPhuTrachChinhId = request.NguoiPhuTrachChinhId,
                        NgayBatDau = request.NgayBatDau,
                        HanHoanThanh = request.HanHoanThanh,
                        TrangThai = request.TrangThai,
                        TyLeHoanThanh = request.TyLeHoanThanh,
                        KetQuaYeuCau = NullIfWhiteSpace(request.KetQuaYeuCau),
                        GiaTriChiTieu = request.GiaTriChiTieu,
                        DonViTinh = NullIfWhiteSpace(request.DonViTinh),
                        ThuTuSapXep = request.ThuTuSapXep,
                        GhiChu = NullIfWhiteSpace(request.GhiChu),
                        CreatedBy = currentUser.Id,
                        CreatedDate = now,
                        UpdatedBy = currentUser.Id,
                        UpdatedDate = now
                    };
                    _dbContext.ThiHanhPhapLuatChiTietNhiemVus.Add(entity);
                }
                else
                {
                    entity = await _dbContext.ThiHanhPhapLuatChiTietNhiemVus.FirstOrDefaultAsync(x => x.Id == request.Id)
                        ?? throw new InvalidOperationException("KhÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â´ng tÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â¬m thĂ„â€Ă‚Â¡Ä‚â€Ă‚ÂºÄ‚â€Ă‚Â¥y nhiĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚Â¢Ă¢â€Â¬Ă‚Â¡m vĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚â€Ă‚Â¥ nhĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚â€Ă‚Â.");

                    entity.MaChiTiet = request.MaChiTiet.Trim();
                    entity.TenChiTiet = request.TenChiTiet.Trim();
                    entity.NoiDungChiTiet = NullIfWhiteSpace(request.NoiDungChiTiet);
                    entity.LoaiChiTiet = request.LoaiChiTiet;
                    entity.DonViThucHienId = request.DonViThucHienId;
                    entity.NguoiPhuTrachChinhId = request.NguoiPhuTrachChinhId;
                    entity.NgayBatDau = request.NgayBatDau;
                    entity.HanHoanThanh = request.HanHoanThanh;
                    entity.TrangThai = request.TrangThai;
                    entity.TyLeHoanThanh = request.TyLeHoanThanh;
                    entity.KetQuaYeuCau = NullIfWhiteSpace(request.KetQuaYeuCau);
                    entity.GiaTriChiTieu = request.GiaTriChiTieu;
                    entity.DonViTinh = NullIfWhiteSpace(request.DonViTinh);
                    entity.ThuTuSapXep = request.ThuTuSapXep;
                    entity.GhiChu = NullIfWhiteSpace(request.GhiChu);
                    entity.UpdatedBy = currentUser.Id;
                    entity.UpdatedDate = now;
                }

                await _dbContext.SaveChangesAsync();

                var oldPhoiHops = await _dbContext.ThiHanhPhapLuatChiTietPhoiHops.Where(x => x.ChiTietNhiemVuId == entity.Id).ToListAsync();
                if (oldPhoiHops.Count > 0)
                {
                    _dbContext.ThiHanhPhapLuatChiTietPhoiHops.RemoveRange(oldPhoiHops);
                }

                var nguoiDungPhoiHopIds = (request.NguoiDungPhoiHopIds ?? new List<Guid>()).Where(x => x != Guid.Empty).Distinct().ToList();
                if (nguoiDungPhoiHopIds.Count > 0)
                {
                    _dbContext.ThiHanhPhapLuatChiTietPhoiHops.AddRange(nguoiDungPhoiHopIds.Select(x => new ThiHanhPhapLuatChiTietPhoiHop
                    {
                        ChiTietNhiemVuId = entity.Id,
                        NguoiDungId = x,
                        CreatedBy = currentUser.Id,
                        CreatedDate = now,
                        UpdatedBy = currentUser.Id,
                        UpdatedDate = now
                    }));
                }

                await _dbContext.SaveChangesAsync();
                return new CommonResponse("success", "LĂ„â€Ă¢â‚¬Â Ä‚â€Ă‚Â°u nhiĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚Â¢Ă¢â€Â¬Ă‚Â¡m vĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚â€Ă‚Â¥ nhĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚â€Ă‚Â thÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â nh cÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â´ng.");
            }
            catch (InvalidOperationException ex)
            {
                return new CommonResponse("error", ex.Message);
            }
            catch
            {
                return new CommonResponse();
            }
        }

        public async Task<CommonResponse> DeleteNhiemVuAsync(Guid id)
        {
            try
            {
                var entity = await _dbContext.ThiHanhPhapLuatNhiemVus.FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                {
                    return new CommonResponse("error", "KhÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â´ng tÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â¬m thĂ„â€Ă‚Â¡Ä‚â€Ă‚ÂºÄ‚â€Ă‚Â¥y nhiĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚Â¢Ă¢â€Â¬Ă‚Â¡m vĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚â€Ă‚Â¥.");
                }

                _dbContext.ThiHanhPhapLuatNhiemVus.Remove(entity);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse("success", "XÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â³a nhiĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚Â¢Ă¢â€Â¬Ă‚Â¡m vĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚â€Ă‚Â¥ lĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚Â¢Ă¢â€Â¬Ă‚Âºn thÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â nh cÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â´ng.");
            }
            catch
            {
                return new CommonResponse();
            }
        }

        public async Task<CommonResponse> DeleteChiTietNhiemVuAsync(Guid id)
        {
            try
            {
                var entity = await _dbContext.ThiHanhPhapLuatChiTietNhiemVus.FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                {
                    return new CommonResponse("error", "KhÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â´ng tÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â¬m thĂ„â€Ă‚Â¡Ä‚â€Ă‚ÂºÄ‚â€Ă‚Â¥y nhiĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚Â¢Ă¢â€Â¬Ă‚Â¡m vĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚â€Ă‚Â¥ nhĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚â€Ă‚Â.");
                }

                _dbContext.ThiHanhPhapLuatChiTietNhiemVus.Remove(entity);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse("success", "XÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â³a nhiĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚Â¢Ă¢â€Â¬Ă‚Â¡m vĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚â€Ă‚Â¥ nhĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚â€Ă‚Â thÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â nh cÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â´ng.");
            }
            catch
            {
                return new CommonResponse();
            }
        }

        public async Task<CommonResponse> GetDanhSachDanhGiaAsync(string search, Guid? donViId, string? canhBao, int pageSize = 5, int pageCurrent = 1)
        {
            try
            {
                var keHoachQuery = _dbContext.ThiHanhPhapLuatKeHoachs.AsNoTracking().AsQueryable();
                if (!string.IsNullOrWhiteSpace(search))
                {
                    search = search.Trim();
                    keHoachQuery = keHoachQuery.Where(x => x.MaKeHoach.Contains(search) || x.TenKeHoach.Contains(search));
                }

                if (donViId.HasValue && donViId.Value != Guid.Empty)
                {
                    keHoachQuery = keHoachQuery.Where(x => x.DonViChuTriId == donViId.Value);
                }

                var keHoachs = await keHoachQuery
                    .OrderByDescending(x => x.Nam)
                    .ThenByDescending(x => x.CreatedDate)
                    .ToListAsync();

                var keHoachIds = keHoachs.Select(x => x.Id).ToList();
                var donViMap = await _dbContext.DanhMucDonVis.AsNoTracking().ToDictionaryAsync(x => x.Id, x => x.TenDonVi);

                var chiTietRows = await _dbContext.ThiHanhPhapLuatChiTietNhiemVus.AsNoTracking()
                    .Join(_dbContext.ThiHanhPhapLuatNhiemVus.AsNoTracking(),
                        ct => ct.NhiemVuId,
                        nv => nv.Id,
                        (ct, nv) => new { ChiTiet = ct, nv.KeHoachId })
                    .Where(x => keHoachIds.Contains(x.KeHoachId))
                    .ToListAsync();

                var chiTietIds = chiTietRows.Select(c => c.ChiTiet.Id).ToList();
                var tienDoCounts = await _dbContext.ThiHanhPhapLuatTienDos.AsNoTracking()
                    .Where(x => chiTietIds.Contains(x.ChiTietNhiemVuId))
                    .GroupBy(x => x.ChiTietNhiemVuId)
                    .Select(g => new { ChiTietNhiemVuId = g.Key, Count = g.Count() })
                    .ToListAsync();
                var tienDoMap = tienDoCounts.ToDictionary(x => x.ChiTietNhiemVuId, x => x.Count);

                var data = keHoachs.Select(keHoach =>
                {
                    var items = chiTietRows.Where(x => x.KeHoachId == keHoach.Id).Select(x => x.ChiTiet).ToList();
                    var total = items.Count;
                    var done = items.Count(x => x.TrangThai == "HOAN_THANH");
                    var noProgress = items.Count(x => !tienDoMap.TryGetValue(x.Id, out var count) || count == 0);
                    var overdue = items.Count(x => x.TrangThai != "HOAN_THANH" && x.HanHoanThanh.HasValue && x.HanHoanThanh.Value.Date < DateTime.Today);
                    var delayed = items.Count(x => x.TrangThai != "HOAN_THANH" && x.HanHoanThanh.HasValue && x.HanHoanThanh.Value.Date >= DateTime.Today && x.HanHoanThanh.Value.Date <= DateTime.Today.AddDays(3));
                    var notStarted = items.Count(x => x.TrangThai == "CHUA_THUC_HIEN");
                    var percent = total == 0 ? 0 : Math.Round(done * 100m / total, 2);
                    return new ThiHanhPhapLuatDanhGiaListItemModel
                    {
                        KeHoachId = keHoach.Id,
                        MaKeHoach = keHoach.MaKeHoach,
                        TenKeHoach = keHoach.TenKeHoach,
                        Nam = keHoach.Nam,
                        TenDonViChuTri = donViMap.TryGetValue(keHoach.DonViChuTriId, out var tenDonVi) ? tenDonVi : string.Empty,
                        TongSoChiTiet = total,
                        SoChiTietHoanThanh = done,
                        SoChiTietChuaThucHien = notStarted,
                        SoChiTietChuaNhapLieu = noProgress,
                        SoChiTietChamTienDo = delayed,
                        SoChiTietQuaHan = overdue,
                        TyLeHoanThanh = percent,
                        MucDoCanhBao = overdue > 0 ? "QUA_HAN" : delayed > 0 ? "CHAM_TIEN_DO" : noProgress > 0 ? "CHUA_NHAP_LIEU" : notStarted > 0 ? "CHUA_THUC_HIEN" : "BINH_THUONG"
                    };
                }).ToList();


                if (!string.IsNullOrWhiteSpace(canhBao))
                {
                    var normalizedCanhBao = canhBao.Trim().ToUpperInvariant();
                    data = data.Where(x => x.MucDoCanhBao == normalizedCanhBao).ToList();
                }

                var totalRecord = data.Count;
                data = data.Skip((pageCurrent - 1) * pageSize).Take(pageSize).ToList();

                return new CommonResponse("success", "ThÄ‚Â nh cÄ‚Â´ng", data, totalRecord);
            }
            catch
            {
                return new CommonResponse();
            }
        }

        public async Task<CommonResponse> GetDanhSachTienDoAsync(string search, Guid? donViId, int pageSize = 5, int pageCurrent = 1)
        {
            try
            {
                var query = _dbContext.ThiHanhPhapLuatChiTietNhiemVus.AsNoTracking()
                    .Join(_dbContext.ThiHanhPhapLuatNhiemVus.AsNoTracking(),
                        ct => ct.NhiemVuId,
                        nv => nv.Id,
                        (ct, nv) => new { ct, nv })
                    .Join(_dbContext.ThiHanhPhapLuatKeHoachs.AsNoTracking(),
                        x => x.nv.KeHoachId,
                        kh => kh.Id,
                        (x, kh) => new { x.ct, x.nv, kh })
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(search))
                {
                    search = search.Trim();
                    query = query.Where(x =>
                        x.kh.MaKeHoach.Contains(search) ||
                        x.kh.TenKeHoach.Contains(search) ||
                        x.nv.MaNhiemVu.Contains(search) ||
                        x.nv.TenNhiemVu.Contains(search) ||
                        x.ct.MaChiTiet.Contains(search) ||
                        x.ct.TenChiTiet.Contains(search));
                }

                if (donViId.HasValue && donViId.Value != Guid.Empty)
                {
                    query = query.Where(x => x.ct.DonViThucHienId == donViId.Value || x.kh.DonViChuTriId == donViId.Value);
                }

                var totalRecord = await query.CountAsync();
                var rows = await query
                    .OrderByDescending(x => x.kh.Nam)
                    .ThenBy(x => x.kh.MaKeHoach)
                    .ThenBy(x => x.nv.ThuTuSapXep)
                    .ThenBy(x => x.ct.ThuTuSapXep)
                    .Skip((pageCurrent - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var donViMap = await _dbContext.DanhMucDonVis.AsNoTracking().ToDictionaryAsync(x => x.Id, x => x.TenDonVi);
                var userMap = await _dbContext.Users.AsNoTracking().ToDictionaryAsync(x => x.Id, x => x.Name);
                var chiTietIds = rows.Select(x => x.ct.Id).ToList();
                var lastTienDos = await _dbContext.ThiHanhPhapLuatTienDos.AsNoTracking()
                    .Where(x => chiTietIds.Contains(x.ChiTietNhiemVuId))
                    .GroupBy(x => x.ChiTietNhiemVuId)
                    .Select(g => g.OrderByDescending(x => x.NgayCapNhat).First())
                    .ToListAsync();

                var data = rows.Select(x =>
                {
                    var last = lastTienDos.FirstOrDefault(t => t.ChiTietNhiemVuId == x.ct.Id);
                    return new ThiHanhPhapLuatTienDoListItemModel
                    {
                        ChiTietNhiemVuId = x.ct.Id,
                        KeHoachId = x.kh.Id,
                        MaKeHoach = x.kh.MaKeHoach,
                        TenKeHoach = x.kh.TenKeHoach,
                        MaNhiemVu = x.nv.MaNhiemVu,
                        TenNhiemVu = x.nv.TenNhiemVu,
                        MaChiTiet = x.ct.MaChiTiet,
                        TenChiTiet = x.ct.TenChiTiet,
                        TenDonViThucHien = donViMap.TryGetValue(x.ct.DonViThucHienId, out var tenDonVi) ? tenDonVi : string.Empty,
                        TenNguoiPhuTrachChinh = x.ct.NguoiPhuTrachChinhId.HasValue && userMap.TryGetValue(x.ct.NguoiPhuTrachChinhId.Value, out var tenNguoi) ? tenNguoi : null,
                        HanHoanThanh = x.ct.HanHoanThanh,
                        TrangThai = x.ct.TrangThai,
                        TyLeHoanThanh = x.ct.TyLeHoanThanh,
                        NgayCapNhatGanNhat = last?.NgayCapNhat,
                        TyLeCapNhatGanNhat = last?.TyLeHoanThanh,
                        TrangThaiBaoCao = last?.TrangThaiBaoCao
                    };
                }).ToList();

                return new CommonResponse("success", "ThÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â nh cÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â´ng", data, totalRecord);
            }
            catch
            {
                return new CommonResponse();
            }
        }

        public async Task<CommonResponse> GetTienDoFormAsync(Guid chiTietNhiemVuId)
        {
            try
            {
                var row = await _dbContext.ThiHanhPhapLuatChiTietNhiemVus.AsNoTracking()
                    .Join(_dbContext.ThiHanhPhapLuatNhiemVus.AsNoTracking(), ct => ct.NhiemVuId, nv => nv.Id, (ct, nv) => new { ct, nv })
                    .Join(_dbContext.ThiHanhPhapLuatKeHoachs.AsNoTracking(), x => x.nv.KeHoachId, kh => kh.Id, (x, kh) => new { x.ct, x.nv, kh })
                    .FirstOrDefaultAsync(x => x.ct.Id == chiTietNhiemVuId);
                if (row == null)
                {
                    return new CommonResponse("error", "KhÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â´ng tÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â¬m thĂ„â€Ă‚Â¡Ä‚â€Ă‚ÂºÄ‚â€Ă‚Â¥y nhiĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚Â¢Ă¢â€Â¬Ă‚Â¡m vĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚â€Ă‚Â¥ nhĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚â€Ă‚Â.");
                }

                var last = await _dbContext.ThiHanhPhapLuatTienDos.AsNoTracking()
                    .Where(x => x.ChiTietNhiemVuId == chiTietNhiemVuId)
                    .OrderByDescending(x => x.NgayCapNhat)
                    .FirstOrDefaultAsync();

                return new CommonResponse("success", "ThÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â nh cÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â´ng", new ThiHanhPhapLuatTienDoFormModel
                {
                    ChiTietNhiemVuId = row.ct.Id,
                    KeHoachId = row.kh.Id,
                    MaKeHoach = row.kh.MaKeHoach,
                    TenKeHoach = row.kh.TenKeHoach,
                    TenNhiemVu = row.nv.TenNhiemVu,
                    TenChiTiet = row.ct.TenChiTiet,
                    DonViCapNhatId = row.ct.DonViThucHienId,
                    NgayCapNhat = last?.NgayCapNhat ?? DateTime.Now,
                    TyLeHoanThanh = last?.TyLeHoanThanh ?? row.ct.TyLeHoanThanh,
                    KetQuaThucHien = last?.KetQuaThucHien,
                    NoiDungBaoCao = last?.NoiDungBaoCao,
                    KhoKhanVuongMac = last?.KhoKhanVuongMac,
                    DeXuatKienNghi = last?.DeXuatKienNghi,
                    TrangThaiBaoCao = last?.TrangThaiBaoCao ?? "NHAP",
                    AttachedFileGroupId = last?.AttachedFileGroupId ?? Guid.NewGuid(),
                    GhiChu = last?.GhiChu
                });
            }
            catch
            {
                return new CommonResponse();
            }
        }

        public async Task<CommonResponse> SaveTienDoAsync(ThiHanhPhapLuatTienDoFormModel request, User currentUser)
        {
            try
            {
                if (request.ChiTietNhiemVuId == Guid.Empty) return new CommonResponse("error", "ThiĂ„â€Ă‚Â¡Ä‚â€Ă‚ÂºÄ‚â€Ă‚Â¿u nhiĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚Â¢Ă¢â€Â¬Ă‚Â¡m vĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚â€Ă‚Â¥ nhĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚â€Ă‚Â.");
                if (request.DonViCapNhatId == Guid.Empty) return new CommonResponse("error", "ThiĂ„â€Ă‚Â¡Ä‚â€Ă‚ÂºÄ‚â€Ă‚Â¿u Ă„â€Ă¢â‚¬ÂÄ‚Â¢Ă¢â€Â¬Ă‹Å“Ă„â€Ă¢â‚¬Â Ä‚â€Ă‚Â¡n vĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚Â¢Ă¢â€Â¬Ă‚Â¹ cĂ„â€Ă‚Â¡Ä‚â€Ă‚ÂºÄ‚â€Ă‚Â­p nhĂ„â€Ă‚Â¡Ä‚â€Ă‚ÂºÄ‚â€Ă‚Â­t.");
                if (request.TyLeHoanThanh < 0 || request.TyLeHoanThanh > 100) return new CommonResponse("error", "TĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚â€Ă‚Â· lĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚Â¢Ă¢â€Â¬Ă‚Â¡ hoÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â n thÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â nh phĂ„â€Ă‚Â¡Ä‚â€Ă‚ÂºÄ‚â€Ă‚Â£i tĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚â€Ă‚Â« 0 Ă„â€Ă¢â‚¬ÂÄ‚Â¢Ă¢â€Â¬Ă‹Å“Ă„â€Ă‚Â¡Ä‚â€Ă‚ÂºÄ‚â€Ă‚Â¿n 100.");

                var chiTiet = await _dbContext.ThiHanhPhapLuatChiTietNhiemVus.FirstOrDefaultAsync(x => x.Id == request.ChiTietNhiemVuId);
                if (chiTiet == null) return new CommonResponse("error", "KhÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â´ng tÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â¬m thĂ„â€Ă‚Â¡Ä‚â€Ă‚ÂºÄ‚â€Ă‚Â¥y nhiĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚Â¢Ă¢â€Â¬Ă‚Â¡m vĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚â€Ă‚Â¥ nhĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚â€Ă‚Â.");

                var entity = new ThiHanhPhapLuatTienDo
                {
                    ChiTietNhiemVuId = request.ChiTietNhiemVuId,
                    DonViCapNhatId = request.DonViCapNhatId,
                    NguoiCapNhatId = currentUser.Id,
                    NgayCapNhat = request.NgayCapNhat,
                    TyLeHoanThanh = request.TyLeHoanThanh,
                    KetQuaThucHien = NullIfWhiteSpace(request.KetQuaThucHien),
                    NoiDungBaoCao = NullIfWhiteSpace(request.NoiDungBaoCao),
                    KhoKhanVuongMac = NullIfWhiteSpace(request.KhoKhanVuongMac),
                    DeXuatKienNghi = NullIfWhiteSpace(request.DeXuatKienNghi),
                    TrangThaiBaoCao = request.TrangThaiBaoCao,
                    AttachedFileGroupId = request.AttachedFileGroupId,
                    GhiChu = NullIfWhiteSpace(request.GhiChu)
                };
                _dbContext.ThiHanhPhapLuatTienDos.Add(entity);

                chiTiet.TyLeHoanThanh = request.TyLeHoanThanh;
                chiTiet.TrangThai = request.TyLeHoanThanh >= 100 ? "HOAN_THANH" : request.TyLeHoanThanh > 0 ? "DANG_THUC_HIEN" : "CHUA_THUC_HIEN";

                await _dbContext.SaveChangesAsync();
                return new CommonResponse("success", "CĂ„â€Ă‚Â¡Ä‚â€Ă‚ÂºÄ‚â€Ă‚Â­p nhĂ„â€Ă‚Â¡Ä‚â€Ă‚ÂºÄ‚â€Ă‚Â­t tiĂ„â€Ă‚Â¡Ä‚â€Ă‚ÂºÄ‚â€Ă‚Â¿n Ă„â€Ă¢â‚¬ÂÄ‚Â¢Ă¢â€Â¬Ă‹Å“Ă„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚Â¢Ă¢â‚¬ÂĂ‚Â¢ thÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â nh cÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â´ng.");
            }
            catch
            {
                return new CommonResponse();
            }
        }

        public async Task<CommonResponse> GetDanhGiaFormAsync(Guid keHoachId)
        {
            try
            {
                var keHoach = await _dbContext.ThiHanhPhapLuatKeHoachs.AsNoTracking().FirstOrDefaultAsync(x => x.Id == keHoachId);
                if (keHoach == null) return new CommonResponse("error", "KhÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â´ng tÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â¬m thĂ„â€Ă‚Â¡Ä‚â€Ă‚ÂºÄ‚â€Ă‚Â¥y kĂ„â€Ă‚Â¡Ä‚â€Ă‚ÂºÄ‚â€Ă‚Â¿ hoĂ„â€Ă‚Â¡Ä‚â€Ă‚ÂºÄ‚â€Ă‚Â¡ch.");

                var latest = await _dbContext.ThiHanhPhapLuatDanhGias.AsNoTracking()
                    .Where(x => x.KeHoachId == keHoachId && x.NhiemVuId == null && x.ChiTietNhiemVuId == null)
                    .OrderByDescending(x => x.NgayDanhGia)
                    .FirstOrDefaultAsync();

                return new CommonResponse("success", "ThÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â nh cÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â´ng", new ThiHanhPhapLuatDanhGiaFormModel
                {
                    KeHoachId = keHoachId,
                    TenKeHoach = keHoach.TenKeHoach,
                    DonViDuocDanhGiaId = keHoach.DonViChuTriId,
                    NgayDanhGia = latest?.NgayDanhGia ?? DateTime.Now,
                    KetQuaDanhGia = latest?.KetQuaDanhGia ?? "CHUA_THUC_HIEN",
                    MucDoCanhBao = latest?.MucDoCanhBao ?? "BINH_THUONG",
                    NoiDungDanhGia = latest?.NoiDungDanhGia,
                    KienNghiXuLy = latest?.KienNghiXuLy,
                    YeuCauBoSung = latest?.YeuCauBoSung,
                    TrangThai = latest?.TrangThai ?? "NHAP",
                    GhiChu = latest?.GhiChu
                });
            }
            catch
            {
                return new CommonResponse();
            }
        }

        public async Task<CommonResponse> SaveDanhGiaAsync(ThiHanhPhapLuatDanhGiaFormModel request, User currentUser)
        {
            try
            {
                if (request.KeHoachId == Guid.Empty) return new CommonResponse("error", "ThiĂ„â€Ă‚Â¡Ä‚â€Ă‚ÂºÄ‚â€Ă‚Â¿u kĂ„â€Ă‚Â¡Ä‚â€Ă‚ÂºÄ‚â€Ă‚Â¿ hoĂ„â€Ă‚Â¡Ä‚â€Ă‚ÂºÄ‚â€Ă‚Â¡ch.");
                if (request.DonViDuocDanhGiaId == Guid.Empty) return new CommonResponse("error", "ThiĂ„â€Ă‚Â¡Ä‚â€Ă‚ÂºÄ‚â€Ă‚Â¿u Ă„â€Ă¢â‚¬ÂÄ‚Â¢Ă¢â€Â¬Ă‹Å“Ă„â€Ă¢â‚¬Â Ä‚â€Ă‚Â¡n vĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚Â¢Ă¢â€Â¬Ă‚Â¹ Ă„â€Ă¢â‚¬ÂÄ‚Â¢Ă¢â€Â¬Ă‹Å“Ă„â€Ă¢â‚¬Â Ä‚â€Ă‚Â°Ă„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚â€Ă‚Â£c Ă„â€Ă¢â‚¬ÂÄ‚Â¢Ă¢â€Â¬Ă‹Å“Ä‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â¡nh giÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â¡.");

                _dbContext.ThiHanhPhapLuatDanhGias.Add(new ThiHanhPhapLuatDanhGia
                {
                    KeHoachId = request.KeHoachId,
                    DonViDuocDanhGiaId = request.DonViDuocDanhGiaId,
                    NguoiDanhGiaId = currentUser.Id,
                    NgayDanhGia = request.NgayDanhGia,
                    KetQuaDanhGia = request.KetQuaDanhGia,
                    MucDoCanhBao = request.MucDoCanhBao,
                    NoiDungDanhGia = NullIfWhiteSpace(request.NoiDungDanhGia),
                    KienNghiXuLy = NullIfWhiteSpace(request.KienNghiXuLy),
                    YeuCauBoSung = NullIfWhiteSpace(request.YeuCauBoSung),
                    TrangThai = request.TrangThai,
                    GhiChu = NullIfWhiteSpace(request.GhiChu)
                });
                await _dbContext.SaveChangesAsync();
                return new CommonResponse("success", "LĂ„â€Ă¢â‚¬Â Ä‚â€Ă‚Â°u Ă„â€Ă¢â‚¬ÂÄ‚Â¢Ă¢â€Â¬Ă‹Å“Ä‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â¡nh giÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â¡ thÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â nh cÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â´ng.");
            }
            catch
            {
                return new CommonResponse();
            }
        }

        public async Task<CommonResponse> GetTongHopFormAsync(Guid keHoachId)
        {
            try
            {
                var keHoach = await _dbContext.ThiHanhPhapLuatKeHoachs.AsNoTracking().FirstOrDefaultAsync(x => x.Id == keHoachId);
                if (keHoach == null) return new CommonResponse("error", "KhÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â´ng tÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â¬m thĂ„â€Ă‚Â¡Ä‚â€Ă‚ÂºÄ‚â€Ă‚Â¥y kĂ„â€Ă‚Â¡Ä‚â€Ă‚ÂºÄ‚â€Ă‚Â¿ hoĂ„â€Ă‚Â¡Ä‚â€Ă‚ÂºÄ‚â€Ă‚Â¡ch.");

                var stats = await GetDanhSachDanhGiaAsync(string.Empty, keHoach.DonViChuTriId, null, 100000, 1);
                var current = ((IEnumerable<ThiHanhPhapLuatDanhGiaListItemModel>)stats.Data).FirstOrDefault(x => x.KeHoachId == keHoachId);
                var latest = await _dbContext.ThiHanhPhapLuatTongHops.AsNoTracking()
                    .Where(x => x.KeHoachId == keHoachId)
                    .OrderByDescending(x => x.NgayTongHop)
                    .FirstOrDefaultAsync();

                return new CommonResponse("success", "ThÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â nh cÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â´ng", new ThiHanhPhapLuatTongHopFormModel
                {
                    KeHoachId = keHoachId,
                    TenKeHoach = keHoach.TenKeHoach,
                    NgayTongHop = latest?.NgayTongHop ?? DateTime.Now,
                    TongSoChiTietNhiemVu = latest?.TongSoChiTietNhiemVu ?? current?.TongSoChiTiet ?? 0,
                    SoChiTietDaHoanThanh = latest?.SoChiTietDaHoanThanh ?? current?.SoChiTietHoanThanh ?? 0,
                    SoChiTietChuaHoanThanh = latest?.SoChiTietChuaHoanThanh ?? ((current?.TongSoChiTiet ?? 0) - (current?.SoChiTietHoanThanh ?? 0)),
                    SoChiTietChamTienDo = latest?.SoChiTietChamTienDo ?? current?.SoChiTietChamTienDo ?? 0,
                    SoChiTietQuaHan = latest?.SoChiTietQuaHan ?? current?.SoChiTietQuaHan ?? 0,
                    SoChiTietChuaNhapLieu = latest?.SoChiTietChuaNhapLieu ?? current?.SoChiTietChuaNhapLieu ?? 0,
                    TyLeHoanThanh = latest?.TyLeHoanThanh ?? current?.TyLeHoanThanh ?? 0,
                    NhanXetTongHop = latest?.NhanXetTongHop,
                    KetLuan = latest?.KetLuan,
                    KienNghi = latest?.KienNghi,
                    TrangThai = latest?.TrangThai ?? "NHAP",
                    AttachedFileGroupId = latest?.AttachedFileGroupId ?? Guid.NewGuid(),
                    GhiChu = latest?.GhiChu
                });
            }
            catch
            {
                return new CommonResponse();
            }
        }

        public async Task<CommonResponse> SaveTongHopAsync(ThiHanhPhapLuatTongHopFormModel request, User currentUser)
        {
            try
            {
                if (request.KeHoachId == Guid.Empty) return new CommonResponse("error", "ThiĂ„â€Ă‚Â¡Ä‚â€Ă‚ÂºÄ‚â€Ă‚Â¿u kĂ„â€Ă‚Â¡Ä‚â€Ă‚ÂºÄ‚â€Ă‚Â¿ hoĂ„â€Ă‚Â¡Ä‚â€Ă‚ÂºÄ‚â€Ă‚Â¡ch.");
                _dbContext.ThiHanhPhapLuatTongHops.Add(new ThiHanhPhapLuatTongHop
                {
                    KeHoachId = request.KeHoachId,
                    NguoiTongHopId = currentUser.Id,
                    NgayTongHop = request.NgayTongHop,
                    TongSoChiTietNhiemVu = request.TongSoChiTietNhiemVu,
                    SoChiTietDaHoanThanh = request.SoChiTietDaHoanThanh,
                    SoChiTietChuaHoanThanh = request.SoChiTietChuaHoanThanh,
                    SoChiTietChamTienDo = request.SoChiTietChamTienDo,
                    SoChiTietQuaHan = request.SoChiTietQuaHan,
                    SoChiTietChuaNhapLieu = request.SoChiTietChuaNhapLieu,
                    TyLeHoanThanh = request.TyLeHoanThanh,
                    NhanXetTongHop = NullIfWhiteSpace(request.NhanXetTongHop),
                    KetLuan = NullIfWhiteSpace(request.KetLuan),
                    KienNghi = NullIfWhiteSpace(request.KienNghi),
                    TrangThai = request.TrangThai,
                    AttachedFileGroupId = request.AttachedFileGroupId,
                    GhiChu = NullIfWhiteSpace(request.GhiChu)
                });
                await _dbContext.SaveChangesAsync();
                return new CommonResponse("success", "LĂ„â€Ă¢â‚¬Â Ä‚â€Ă‚Â°u tĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚Â¢Ă¢â€Â¬Ă‚Â¢ng hĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚â€Ă‚Â£p thÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â nh cÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â´ng.");
            }
            catch
            {
                return new CommonResponse();
            }
        }

        public async Task<List<DanhMucDonVi>> GetDonViOptionsAsync()
        {
            return await _dbContext.DanhMucDonVis.AsNoTracking()
                .OrderBy(x => x.STTSapXep)
                .ThenBy(x => x.TenDonVi)
                .ToListAsync();
        }

        public async Task<List<User>> GetNguoiDungOptionsAsync(Guid? donViId = null)
        {
            var query = _dbContext.Users.AsNoTracking().Where(x => x.Status == "KÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â­ch hoĂ„â€Ă‚Â¡Ä‚â€Ă‚ÂºÄ‚â€Ă‚Â¡t");
            if (donViId.HasValue && donViId.Value != Guid.Empty)
            {
                query = query.Where(x => x.DanhMucDonViId == donViId.Value);
            }

            return await query.OrderBy(x => x.Name).ToListAsync();
        }

        public async Task<List<DanhMucVanBan>> GetDanhMucVanBanOptionsAsync()
        {
            return await _dbContext.DanhMucVanBans.AsNoTracking()
                .Where(x => x.TrangThai)
                .OrderBy(x => x.ThuTuSapXep)
                .ThenBy(x => x.TenLoaiVanBan)
                .ToListAsync();
        }

        private static string? ValidateKeHoach(ThiHanhPhapLuatKeHoachFormModel request)
        {
            if (string.IsNullOrWhiteSpace(request.MaKeHoach))
            {
                return "MÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â£ kĂ„â€Ă‚Â¡Ä‚â€Ă‚ÂºÄ‚â€Ă‚Â¿ hoĂ„â€Ă‚Â¡Ä‚â€Ă‚ÂºÄ‚â€Ă‚Â¡ch khÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â´ng Ă„â€Ă¢â‚¬ÂÄ‚Â¢Ă¢â€Â¬Ă‹Å“Ă„â€Ă¢â‚¬Â Ä‚â€Ă‚Â°Ă„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚â€Ă‚Â£c Ă„â€Ă¢â‚¬ÂÄ‚Â¢Ă¢â€Â¬Ă‹Å“Ă„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚â€ Ă¢â‚¬â„¢ trĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚Â¢Ă¢â€Â¬Ă‹Å“ng.";
            }

            if (string.IsNullOrWhiteSpace(request.TenKeHoach))
            {
                return "TÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Âªn kĂ„â€Ă‚Â¡Ä‚â€Ă‚ÂºÄ‚â€Ă‚Â¿ hoĂ„â€Ă‚Â¡Ä‚â€Ă‚ÂºÄ‚â€Ă‚Â¡ch khÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â´ng Ă„â€Ă¢â‚¬ÂÄ‚Â¢Ă¢â€Â¬Ă‹Å“Ă„â€Ă¢â‚¬Â Ä‚â€Ă‚Â°Ă„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚â€Ă‚Â£c Ă„â€Ă¢â‚¬ÂÄ‚Â¢Ă¢â€Â¬Ă‹Å“Ă„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚â€ Ă¢â‚¬â„¢ trĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚Â¢Ă¢â€Â¬Ă‹Å“ng.";
            }

            if (request.Nam < 2000 || request.Nam > 3000)
            {
                return "NĂ„â€Ă¢â‚¬ÂÄ‚â€ Ă¢â‚¬â„¢m kĂ„â€Ă‚Â¡Ä‚â€Ă‚ÂºÄ‚â€Ă‚Â¿ hoĂ„â€Ă‚Â¡Ä‚â€Ă‚ÂºÄ‚â€Ă‚Â¡ch khÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â´ng hĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚â€Ă‚Â£p lĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚Â¢Ă¢â€Â¬Ă‚Â¡.";
            }

            if (request.DonViChuTriId == Guid.Empty)
            {
                return "Ă„â€Ă¢â‚¬ÂÄ‚â€Ă‚ÂĂ„â€Ă¢â‚¬Â Ä‚â€Ă‚Â¡n vĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚Â¢Ă¢â€Â¬Ă‚Â¹ chĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚â€Ă‚Â§ trÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â¬ khÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â´ng Ă„â€Ă¢â‚¬ÂÄ‚Â¢Ă¢â€Â¬Ă‹Å“Ă„â€Ă¢â‚¬Â Ä‚â€Ă‚Â°Ă„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚â€Ă‚Â£c Ă„â€Ă¢â‚¬ÂÄ‚Â¢Ă¢â€Â¬Ă‹Å“Ă„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚â€ Ă¢â‚¬â„¢ trĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚Â¢Ă¢â€Â¬Ă‹Å“ng.";
            }

            if (request.NgayBatDau.HasValue && request.NgayKetThuc.HasValue && request.NgayBatDau > request.NgayKetThuc)
            {
                return "NgÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â y bĂ„â€Ă‚Â¡Ä‚â€Ă‚ÂºÄ‚â€Ă‚Â¯t Ă„â€Ă¢â‚¬ÂÄ‚Â¢Ă¢â€Â¬Ă‹Å“Ă„â€Ă‚Â¡Ä‚â€Ă‚ÂºÄ‚â€Ă‚Â§u khÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â´ng Ă„â€Ă¢â‚¬ÂÄ‚Â¢Ă¢â€Â¬Ă‹Å“Ă„â€Ă¢â‚¬Â Ä‚â€Ă‚Â°Ă„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚â€Ă‚Â£c lĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚Â¢Ă¢â€Â¬Ă‚Âºn hĂ„â€Ă¢â‚¬Â Ä‚â€Ă‚Â¡n ngÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â y kĂ„â€Ă‚Â¡Ä‚â€Ă‚ÂºÄ‚â€Ă‚Â¿t thÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Âºc.";
            }

            return null;
        }

        private static string? ValidateNhiemVu(ThiHanhPhapLuatNhiemVuFormModel request)
        {
            if (request.KeHoachId == Guid.Empty)
            {
                return "ThiĂ„â€Ă‚Â¡Ä‚â€Ă‚ÂºÄ‚â€Ă‚Â¿u thÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â´ng tin kĂ„â€Ă‚Â¡Ä‚â€Ă‚ÂºÄ‚â€Ă‚Â¿ hoĂ„â€Ă‚Â¡Ä‚â€Ă‚ÂºÄ‚â€Ă‚Â¡ch.";
            }

            if (string.IsNullOrWhiteSpace(request.MaNhiemVu))
            {
                return "MÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â£ nhiĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚Â¢Ă¢â€Â¬Ă‚Â¡m vĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚â€Ă‚Â¥ khÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â´ng Ă„â€Ă¢â‚¬ÂÄ‚Â¢Ă¢â€Â¬Ă‹Å“Ă„â€Ă¢â‚¬Â Ä‚â€Ă‚Â°Ă„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚â€Ă‚Â£c Ă„â€Ă¢â‚¬ÂÄ‚Â¢Ă¢â€Â¬Ă‹Å“Ă„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚â€ Ă¢â‚¬â„¢ trĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚Â¢Ă¢â€Â¬Ă‹Å“ng.";
            }

            if (string.IsNullOrWhiteSpace(request.TenNhiemVu))
            {
                return "TÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Âªn nhiĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚Â¢Ă¢â€Â¬Ă‚Â¡m vĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚â€Ă‚Â¥ khÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â´ng Ă„â€Ă¢â‚¬ÂÄ‚Â¢Ă¢â€Â¬Ă‹Å“Ă„â€Ă¢â‚¬Â Ä‚â€Ă‚Â°Ă„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚â€Ă‚Â£c Ă„â€Ă¢â‚¬ÂÄ‚Â¢Ă¢â€Â¬Ă‹Å“Ă„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚â€ Ă¢â‚¬â„¢ trĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚Â¢Ă¢â€Â¬Ă‹Å“ng.";
            }

            if (request.DonViChuTriId == Guid.Empty)
            {
                return "Ă„â€Ă¢â‚¬ÂÄ‚â€Ă‚ÂĂ„â€Ă¢â‚¬Â Ä‚â€Ă‚Â¡n vĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚Â¢Ă¢â€Â¬Ă‚Â¹ chĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚â€Ă‚Â§ trÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â¬ khÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â´ng Ă„â€Ă¢â‚¬ÂÄ‚Â¢Ă¢â€Â¬Ă‹Å“Ă„â€Ă¢â‚¬Â Ä‚â€Ă‚Â°Ă„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚â€Ă‚Â£c Ă„â€Ă¢â‚¬ÂÄ‚Â¢Ă¢â€Â¬Ă‹Å“Ă„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚â€ Ă¢â‚¬â„¢ trĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚Â¢Ă¢â€Â¬Ă‹Å“ng.";
            }

            if (request.NgayBatDau.HasValue && request.HanHoanThanh.HasValue && request.NgayBatDau > request.HanHoanThanh)
            {
                return "NgÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â y bĂ„â€Ă‚Â¡Ä‚â€Ă‚ÂºÄ‚â€Ă‚Â¯t Ă„â€Ă¢â‚¬ÂÄ‚Â¢Ă¢â€Â¬Ă‹Å“Ă„â€Ă‚Â¡Ä‚â€Ă‚ÂºÄ‚â€Ă‚Â§u khÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â´ng Ă„â€Ă¢â‚¬ÂÄ‚Â¢Ă¢â€Â¬Ă‹Å“Ă„â€Ă¢â‚¬Â Ä‚â€Ă‚Â°Ă„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚â€Ă‚Â£c lĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚Â¢Ă¢â€Â¬Ă‚Âºn hĂ„â€Ă¢â‚¬Â Ä‚â€Ă‚Â¡n hĂ„â€Ă‚Â¡Ä‚â€Ă‚ÂºÄ‚â€Ă‚Â¡n hoÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â n thÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â nh.";
            }

            return null;
        }

        private static string? ValidateChiTietNhiemVu(ThiHanhPhapLuatChiTietNhiemVuFormModel request)
        {
            if (request.NhiemVuId == Guid.Empty)
            {
                return "ThiĂ„â€Ă‚Â¡Ä‚â€Ă‚ÂºÄ‚â€Ă‚Â¿u thÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â´ng tin nhiĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚Â¢Ă¢â€Â¬Ă‚Â¡m vĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚â€Ă‚Â¥.";
            }

            if (string.IsNullOrWhiteSpace(request.MaChiTiet))
            {
                return "MÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â£ nhiĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚Â¢Ă¢â€Â¬Ă‚Â¡m vĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚â€Ă‚Â¥ nhĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚â€Ă‚Â khÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â´ng Ă„â€Ă¢â‚¬ÂÄ‚Â¢Ă¢â€Â¬Ă‹Å“Ă„â€Ă¢â‚¬Â Ä‚â€Ă‚Â°Ă„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚â€Ă‚Â£c Ă„â€Ă¢â‚¬ÂÄ‚Â¢Ă¢â€Â¬Ă‹Å“Ă„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚â€ Ă¢â‚¬â„¢ trĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚Â¢Ă¢â€Â¬Ă‹Å“ng.";
            }

            if (string.IsNullOrWhiteSpace(request.TenChiTiet))
            {
                return "TÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Âªn nhiĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚Â¢Ă¢â€Â¬Ă‚Â¡m vĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚â€Ă‚Â¥ nhĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚â€Ă‚Â khÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â´ng Ă„â€Ă¢â‚¬ÂÄ‚Â¢Ă¢â€Â¬Ă‹Å“Ă„â€Ă¢â‚¬Â Ä‚â€Ă‚Â°Ă„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚â€Ă‚Â£c Ă„â€Ă¢â‚¬ÂÄ‚Â¢Ă¢â€Â¬Ă‹Å“Ă„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚â€ Ă¢â‚¬â„¢ trĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚Â¢Ă¢â€Â¬Ă‹Å“ng.";
            }

            if (request.DonViThucHienId == Guid.Empty)
            {
                return "Ă„â€Ă¢â‚¬ÂÄ‚â€Ă‚ÂĂ„â€Ă¢â‚¬Â Ä‚â€Ă‚Â¡n vĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚Â¢Ă¢â€Â¬Ă‚Â¹ thĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚â€Ă‚Â±c hiĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚Â¢Ă¢â€Â¬Ă‚Â¡n khÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â´ng Ă„â€Ă¢â‚¬ÂÄ‚Â¢Ă¢â€Â¬Ă‹Å“Ă„â€Ă¢â‚¬Â Ä‚â€Ă‚Â°Ă„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚â€Ă‚Â£c Ă„â€Ă¢â‚¬ÂÄ‚Â¢Ă¢â€Â¬Ă‹Å“Ă„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚â€ Ă¢â‚¬â„¢ trĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚Â¢Ă¢â€Â¬Ă‹Å“ng.";
            }

            if (request.TyLeHoanThanh < 0 || request.TyLeHoanThanh > 100)
            {
                return "TĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚â€Ă‚Â· lĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚Â¢Ă¢â€Â¬Ă‚Â¡ hoÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â n thÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â nh phĂ„â€Ă‚Â¡Ä‚â€Ă‚ÂºÄ‚â€Ă‚Â£i tĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚â€Ă‚Â« 0 Ă„â€Ă¢â‚¬ÂÄ‚Â¢Ă¢â€Â¬Ă‹Å“Ă„â€Ă‚Â¡Ä‚â€Ă‚ÂºÄ‚â€Ă‚Â¿n 100.";
            }

            if (request.NgayBatDau.HasValue && request.HanHoanThanh.HasValue && request.NgayBatDau > request.HanHoanThanh)
            {
                return "NgÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â y bĂ„â€Ă‚Â¡Ä‚â€Ă‚ÂºÄ‚â€Ă‚Â¯t Ă„â€Ă¢â‚¬ÂÄ‚Â¢Ă¢â€Â¬Ă‹Å“Ă„â€Ă‚Â¡Ä‚â€Ă‚ÂºÄ‚â€Ă‚Â§u khÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â´ng Ă„â€Ă¢â‚¬ÂÄ‚Â¢Ă¢â€Â¬Ă‹Å“Ă„â€Ă¢â‚¬Â Ä‚â€Ă‚Â°Ă„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚â€Ă‚Â£c lĂ„â€Ă‚Â¡Ä‚â€Ă‚Â»Ä‚Â¢Ă¢â€Â¬Ă‚Âºn hĂ„â€Ă¢â‚¬Â Ä‚â€Ă‚Â¡n hĂ„â€Ă‚Â¡Ä‚â€Ă‚ÂºÄ‚â€Ă‚Â¡n hoÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â n thÄ‚â€Ă¢â‚¬ÂÄ‚â€Ă‚Â nh.";
            }

            return null;
        }

        private static string? NullIfWhiteSpace(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    public class ThiHanhPhapLuatKeHoachListItemModel
    {
        public Guid Id { get; set; }
        public string MaKeHoach { get; set; } = string.Empty;
        public string TenKeHoach { get; set; } = string.Empty;
        public int Nam { get; set; }
        public string? TenLoaiVanBan { get; set; }
        public string TenDonViChuTri { get; set; } = string.Empty;
        public DateTime? NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }
        public DateTime? NgayCongBo { get; set; }
        public string TrangThai { get; set; } = string.Empty;
        public int TongSoNhiemVu { get; set; }
        public int SoNhiemVuHoanThanh { get; set; }
        public int TongSoChiTiet { get; set; }
        public int SoChiTietHoanThanh { get; set; }
    }

    public class ThiHanhPhapLuatKeHoachFormModel
    {
        public Guid Id { get; set; }
        public string MaKeHoach { get; set; } = string.Empty;
        public string TenKeHoach { get; set; } = string.Empty;
        public int Nam { get; set; }
        public Guid? DanhMucVanBanId { get; set; }
        public string? SoKyHieuVanBanCanCu { get; set; }
        public DateTime? NgayBanHanhVanBanCanCu { get; set; }
        public string? TrichYeuVanBanCanCu { get; set; }
        public string? CoQuanBanHanhVanBanCanCu { get; set; }
        public Guid DonViChuTriId { get; set; }
        public DateTime? NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }
        public DateTime? NgayCongBo { get; set; }
        public string TrangThai { get; set; } = "NHAP";
        public string? MoTa { get; set; }
        public string? GhiChu { get; set; }
        public Guid? AttachedFileGroupId { get; set; }
    }

    public class ThiHanhPhapLuatKeHoachDetailModel
    {
        public Guid Id { get; set; }
        public string MaKeHoach { get; set; } = string.Empty;
        public string TenKeHoach { get; set; } = string.Empty;
        public int Nam { get; set; }
        public string? TenLoaiVanBan { get; set; }
        public string? SoKyHieuVanBanCanCu { get; set; }
        public DateTime? NgayBanHanhVanBanCanCu { get; set; }
        public string? TrichYeuVanBanCanCu { get; set; }
        public string? CoQuanBanHanhVanBanCanCu { get; set; }
        public string TenDonViChuTri { get; set; } = string.Empty;
        public DateTime? NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }
        public DateTime? NgayCongBo { get; set; }
        public string TrangThai { get; set; } = string.Empty;
        public string? MoTa { get; set; }
        public string? GhiChu { get; set; }
        public Guid? AttachedFileGroupId { get; set; }
        public List<ThiHanhPhapLuatNhiemVuDetailModel> NhiemVus { get; set; } = new();
    }

    public class ThiHanhPhapLuatNhiemVuDetailModel
    {
        public Guid Id { get; set; }
        public Guid KeHoachId { get; set; }
        public string MaNhiemVu { get; set; } = string.Empty;
        public string TenNhiemVu { get; set; } = string.Empty;
        public string? NoiDungNhiemVu { get; set; }
        public Guid DonViChuTriId { get; set; }
        public Guid? NguoiDieuPhoiId { get; set; }
        public string TenDonViChuTri { get; set; } = string.Empty;
        public string? TenNguoiDieuPhoi { get; set; }
        public DateTime? NgayBatDau { get; set; }
        public DateTime? HanHoanThanh { get; set; }
        public string MucDoUuTien { get; set; } = string.Empty;
        public string TrangThai { get; set; } = string.Empty;
        public int ThuTuSapXep { get; set; }
        public bool YeuCauBaoCao { get; set; }
        public string? GhiChu { get; set; }
        public List<ThiHanhPhapLuatChiTietNhiemVuDetailModel> ChiTiets { get; set; } = new();
    }

    public class ThiHanhPhapLuatChiTietNhiemVuDetailModel
    {
        public Guid Id { get; set; }
        public Guid NhiemVuId { get; set; }
        public string MaChiTiet { get; set; } = string.Empty;
        public string TenChiTiet { get; set; } = string.Empty;
        public string? NoiDungChiTiet { get; set; }
        public string LoaiChiTiet { get; set; } = string.Empty;
        public Guid DonViThucHienId { get; set; }
        public Guid? NguoiPhuTrachChinhId { get; set; }
        public string TenDonViThucHien { get; set; } = string.Empty;
        public string? TenNguoiPhuTrachChinh { get; set; }
        public DateTime? NgayBatDau { get; set; }
        public DateTime? HanHoanThanh { get; set; }
        public string TrangThai { get; set; } = string.Empty;
        public int TyLeHoanThanh { get; set; }
        public string? KetQuaYeuCau { get; set; }
        public decimal? GiaTriChiTieu { get; set; }
        public string? DonViTinh { get; set; }
        public int ThuTuSapXep { get; set; }
        public string? GhiChu { get; set; }
        public List<string> NguoiDungPhoiHops { get; set; } = new();
    }

    public class ThiHanhPhapLuatNhiemVuFormModel
    {
        public Guid Id { get; set; }
        public Guid KeHoachId { get; set; }
        public string MaNhiemVu { get; set; } = string.Empty;
        public string TenNhiemVu { get; set; } = string.Empty;
        public string? NoiDungNhiemVu { get; set; }
        public Guid DonViChuTriId { get; set; }
        public Guid? NguoiDieuPhoiId { get; set; }
        public DateTime? NgayBatDau { get; set; }
        public DateTime? HanHoanThanh { get; set; }
        public string MucDoUuTien { get; set; } = "TRUNG_BINH";
        public string TrangThai { get; set; } = "CHUA_THUC_HIEN";
        public int ThuTuSapXep { get; set; }
        public bool YeuCauBaoCao { get; set; } = true;
        public string? GhiChu { get; set; }
    }

    public class ThiHanhPhapLuatChiTietNhiemVuFormModel
    {
        public Guid Id { get; set; }
        public Guid NhiemVuId { get; set; }
        public Guid KeHoachId { get; set; }
        public string MaChiTiet { get; set; } = string.Empty;
        public string TenChiTiet { get; set; } = string.Empty;
        public string? NoiDungChiTiet { get; set; }
        public string LoaiChiTiet { get; set; } = "NHIEM_VU_CON";
        public Guid DonViThucHienId { get; set; }
        public Guid? NguoiPhuTrachChinhId { get; set; }
        public DateTime? NgayBatDau { get; set; }
        public DateTime? HanHoanThanh { get; set; }
        public string TrangThai { get; set; } = "CHUA_THUC_HIEN";
        public int TyLeHoanThanh { get; set; }
        public string? KetQuaYeuCau { get; set; }
        public decimal? GiaTriChiTieu { get; set; }
        public string? DonViTinh { get; set; }
        public int ThuTuSapXep { get; set; }
        public string? GhiChu { get; set; }
        public List<Guid> NguoiDungPhoiHopIds { get; set; } = new();
    }

    public class ThiHanhPhapLuatDanhGiaListItemModel
    {
        public Guid KeHoachId { get; set; }
        public string MaKeHoach { get; set; } = string.Empty;
        public string TenKeHoach { get; set; } = string.Empty;
        public int Nam { get; set; }
        public string TenDonViChuTri { get; set; } = string.Empty;
        public int TongSoChiTiet { get; set; }
        public int SoChiTietHoanThanh { get; set; }
        public int SoChiTietChuaThucHien { get; set; }
        public int SoChiTietChuaNhapLieu { get; set; }
        public int SoChiTietChamTienDo { get; set; }
        public int SoChiTietQuaHan { get; set; }
        public decimal TyLeHoanThanh { get; set; }
        public string MucDoCanhBao { get; set; } = "BINH_THUONG";
    }

    public class ThiHanhPhapLuatTienDoListItemModel
    {
        public Guid ChiTietNhiemVuId { get; set; }
        public Guid KeHoachId { get; set; }
        public string MaKeHoach { get; set; } = string.Empty;
        public string TenKeHoach { get; set; } = string.Empty;
        public string MaNhiemVu { get; set; } = string.Empty;
        public string TenNhiemVu { get; set; } = string.Empty;
        public string MaChiTiet { get; set; } = string.Empty;
        public string TenChiTiet { get; set; } = string.Empty;
        public string TenDonViThucHien { get; set; } = string.Empty;
        public string? TenNguoiPhuTrachChinh { get; set; }
        public DateTime? HanHoanThanh { get; set; }
        public string TrangThai { get; set; } = string.Empty;
        public int TyLeHoanThanh { get; set; }
        public DateTime? NgayCapNhatGanNhat { get; set; }
        public int? TyLeCapNhatGanNhat { get; set; }
        public string? TrangThaiBaoCao { get; set; }
    }

    public class ThiHanhPhapLuatTienDoFormModel
    {
        public Guid ChiTietNhiemVuId { get; set; }
        public Guid KeHoachId { get; set; }
        public string MaKeHoach { get; set; } = string.Empty;
        public string TenKeHoach { get; set; } = string.Empty;
        public string TenNhiemVu { get; set; } = string.Empty;
        public string TenChiTiet { get; set; } = string.Empty;
        public Guid DonViCapNhatId { get; set; }
        public DateTime NgayCapNhat { get; set; }
        public int TyLeHoanThanh { get; set; }
        public string? KetQuaThucHien { get; set; }
        public string? NoiDungBaoCao { get; set; }
        public string? KhoKhanVuongMac { get; set; }
        public string? DeXuatKienNghi { get; set; }
        public string TrangThaiBaoCao { get; set; } = "NHAP";
        public Guid? AttachedFileGroupId { get; set; }
        public string? GhiChu { get; set; }
    }

    public class ThiHanhPhapLuatDanhGiaFormModel
    {
        public Guid KeHoachId { get; set; }
        public string TenKeHoach { get; set; } = string.Empty;
        public Guid DonViDuocDanhGiaId { get; set; }
        public DateTime NgayDanhGia { get; set; }
        public string KetQuaDanhGia { get; set; } = "CHUA_THUC_HIEN";
        public string MucDoCanhBao { get; set; } = "BINH_THUONG";
        public string? NoiDungDanhGia { get; set; }
        public string? KienNghiXuLy { get; set; }
        public string? YeuCauBoSung { get; set; }
        public string TrangThai { get; set; } = "NHAP";
        public string? GhiChu { get; set; }
    }

    public class ThiHanhPhapLuatTongHopFormModel
    {
        public Guid KeHoachId { get; set; }
        public string TenKeHoach { get; set; } = string.Empty;
        public DateTime NgayTongHop { get; set; }
        public int TongSoChiTietNhiemVu { get; set; }
        public int SoChiTietDaHoanThanh { get; set; }
        public int SoChiTietChuaHoanThanh { get; set; }
        public int SoChiTietChamTienDo { get; set; }
        public int SoChiTietQuaHan { get; set; }
        public int SoChiTietChuaNhapLieu { get; set; }
        public decimal TyLeHoanThanh { get; set; }
        public string? NhanXetTongHop { get; set; }
        public string? KetLuan { get; set; }
        public string? KienNghi { get; set; }
        public string TrangThai { get; set; } = "NHAP";
        public Guid? AttachedFileGroupId { get; set; }
        public string? GhiChu { get; set; }
    }
}
