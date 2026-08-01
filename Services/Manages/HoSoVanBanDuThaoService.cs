using DataAccess;
using DataAccess.Entities.Manages;
using Microsoft.EntityFrameworkCore;
using Services.Model;

namespace Services.Manages
{
    public interface IHoSoVanBanDuThaoService
    {
        Task<CommonResponse> GetEditModelAsync(Guid hoSoVanBanId);
        Task<CommonResponse> SaveAsync(HoSoVanBanDuThaoEditModel request);
    }

    public class HoSoVanBanDuThaoService(ApplicationDbContext dbContext) : IHoSoVanBanDuThaoService
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        public async Task<CommonResponse> GetEditModelAsync(Guid hoSoVanBanId)
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
                    join duThao in _dbContext.HoSoVanBanDuThaos.AsNoTracking() on hoSo.Id equals duThao.HoSoVanBanId into duThaoJoin
                    from duThao in duThaoJoin.DefaultIfEmpty()
                    where hoSo.Id == hoSoVanBanId
                    select new HoSoVanBanDuThaoEditModel
                    {
                        Id = duThao != null ? duThao.Id : Guid.Empty,
                        HoSoVanBanId = hoSo.Id,
                        DonViSoanThaoId = hoSo.DonViSoanThaoId,
                        TenHoSo = hoSo.TenHoSo,
                        TenLoaiVanBan = vanBan.TenLoaiVanBan,
                        TenQuyTrinh = quyTrinh.TenQuyTrinh,
                        TenDonViSoanThao = donVi.TenDonVi,
                        TenBuocHienTai = buoc != null ? buoc.TenBuoc : "Đã hoàn thành",
                        NgayTaoHoSo = hoSo.NgayTaoHoSo,
                        HanXuLy = hoSo.HanXuLy,
                        TenDuThao = duThao != null ? duThao.TenDuThao : hoSo.TenHoSo,
                        SoLanDuThao = duThao != null ? duThao.SoLanDuThao : 1,
                        NgayCapNhatDuThao = duThao != null ? duThao.NgayCapNhatDuThao : null,
                        TrangThaiDuThao = duThao != null ? duThao.TrangThaiDuThao : "DA_HOAN_THANH_DU_THAO",
                        NoiDungTomTat = duThao != null ? duThao.NoiDungTomTat : null,
                        KetQuaThucHien = duThao != null ? duThao.KetQuaThucHien : "DA_HOAN_THANH_DU_THAO",
                        NgayBaoCaoKetQua = duThao != null ? duThao.NgayBaoCaoKetQua : null,
                        NoiDungBaoCao = duThao != null ? duThao.NoiDungBaoCao : null,
                        DaDuDieuKienChuyenBuoc = duThao != null && duThao.DaDuDieuKienChuyenBuoc,
                        GhiChu = duThao != null ? duThao.GhiChu : null,
                        MoTaHoSo = hoSo.MoTa,
                        GhiChuHoSo = hoSo.GhiChu
                    }).FirstOrDefaultAsync();

                if (model == null)
                {
                    return new CommonResponse("error", "Không tìm thấy thông tin hồ sơ.");
                }

                model.TrangThaiDuThaoOptions = BuildTrangThaiDuThaoOptions();
                model.KetQuaThucHienOptions = BuildKetQuaThucHienOptions();
                return new CommonResponse("success", "Thành công", model);
            }
            catch (Exception ex)
            {
                return new CommonResponse("error", $"Không thể tải dữ liệu dự thảo: {ex.Message}");
            }
        }

        public async Task<CommonResponse> SaveAsync(HoSoVanBanDuThaoEditModel request)
        {
            try
            {
                if (request.HoSoVanBanId == Guid.Empty)
                {
                    return new CommonResponse("error", "Hồ sơ văn bản không hợp lệ.");
                }

                var hoSo = await _dbContext.HoSoVanBans.FirstOrDefaultAsync(x => x.Id == request.HoSoVanBanId);
                if (hoSo == null)
                {
                    return new CommonResponse("error", "Không tìm thấy hồ sơ văn bản.");
                }

                var entity = await _dbContext.HoSoVanBanDuThaos.FirstOrDefaultAsync(x => x.HoSoVanBanId == request.HoSoVanBanId);
                if (entity == null)
                {
                    entity = new HoSoVanBanDuThao
                    {
                        HoSoVanBanId = request.HoSoVanBanId
                    };
                    _dbContext.HoSoVanBanDuThaos.Add(entity);
                }

                entity.TenDuThao = string.IsNullOrWhiteSpace(request.TenDuThao) ? hoSo.TenHoSo : request.TenDuThao.Trim();
                entity.SoLanDuThao = request.SoLanDuThao < 1 ? 1 : request.SoLanDuThao;
                entity.NgayCapNhatDuThao = request.NgayCapNhatDuThao;
                entity.TrangThaiDuThao = string.IsNullOrWhiteSpace(request.TrangThaiDuThao) ? "DA_HOAN_THANH_DU_THAO" : request.TrangThaiDuThao.Trim().ToUpperInvariant();
                entity.NoiDungTomTat = request.NoiDungTomTat?.Trim();
                entity.KetQuaThucHien = string.IsNullOrWhiteSpace(request.KetQuaThucHien) ? "DA_HOAN_THANH_DU_THAO" : request.KetQuaThucHien.Trim().ToUpperInvariant();
                entity.NgayBaoCaoKetQua = request.NgayBaoCaoKetQua;
                entity.NoiDungBaoCao = string.IsNullOrWhiteSpace(request.NoiDungBaoCao)
                    ? request.NoiDungTomTat?.Trim()
                    : request.NoiDungBaoCao.Trim();
                entity.DaDuDieuKienChuyenBuoc = request.DaDuDieuKienChuyenBuoc;
                entity.GhiChu = request.GhiChu?.Trim();

                await _dbContext.SaveChangesAsync();
                return new CommonResponse("success", "Cập nhật dự thảo thành công.", entity.Id);
            }
            catch (Exception ex)
            {
                var message = ex.InnerException == null ? ex.Message : $"{ex.Message} -> {ex.InnerException.Message}";
                return new CommonResponse("error", $"Không thể cập nhật dữ liệu dự thảo: {message}");
            }
        }

        private static List<SelectOptionModel> BuildTrangThaiDuThaoOptions()
        {
            return
            [
                new() { Value = "DA_HOAN_THANH_DU_THAO", Text = "Đã hoàn thành dự thảo" }
            ];
        }

        private static List<SelectOptionModel> BuildKetQuaThucHienOptions()
        {
            return
            [
                new() { Value = "DA_HOAN_THANH_DU_THAO", Text = "Hoàn thành" }
            ];
        }
    }
}
