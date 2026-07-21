using DataAccess;
using DataAccess.Entities.Manages;
using DataAccess.Enums;
using Microsoft.EntityFrameworkCore;
using Services.Model;

namespace Services.Manages
{

    public interface IThongTinNganChanService
    {
        Task<CommonResponse> GetThongTinNganChansAsync(string search, int pageSize, int currentPage);
        Task<CommonResponse> StoreAsync(ThongTinNganChan request);
        Task<CommonResponse> EditAsync(Guid id);
        Task<CommonResponse> UpdateAsync(ThongTinNganChan request);
        Task<CommonResponse> DeleteAsync(Guid id);
        Task<CommonResponse> SearchThongTinNganChansAsync(
            string soQuyetDinh,
            string coQuanBanHanh,
            Guid? donViBanHanhId,
            int? namQuyetDinh,
            int? namApDung,
            TrangThaiNganChan? trangThai,
            string thongTinTaiSan,
            string soQuyetDinhDung,
            string coQuanDung,
            int? namQuyetDinhDung,
            int? namApDungDung,
            string timkiem,
            int pageSize,
            int pageCurrent);
    }

    public class ThongTinNganChanService(
        ApplicationDbContext context,
        IAttachedFileService attachedFileService) : IThongTinNganChanService
    {
        public async Task<CommonResponse> DeleteAsync(Guid id)
        {
            try
            {
                var entity = await context.ThongTinNganChans.FindAsync(id);
                if (entity is null)
                    return new CommonResponse
                    {
                        Status = "error",
                        Message = "Không tìm thấy thông tin dữ liệu cần xóa"
                    };

                context.ThongTinNganChans.Remove(entity);
                await attachedFileService.RemoveRangeByGroupId(id);
                await context.SaveChangesAsync();
                return new CommonResponse { Status = "success" };
            }
            catch
            {
                return new CommonResponse
                {
                    Status = "error",
                    Message = "Đã xảy ra lỗi khi xóa dữ liệu. Vui lòng thử lại sau!"
                };
            }
        }

        public async Task<CommonResponse> EditAsync(Guid id)
        {
            try
            {
                var entity = await context.ThongTinNganChans.FindAsync(id);
                if (entity is null)
                    return new CommonResponse
                    {
                        Status = "error",
                        Message = "Không tìm thấy thông tin dữ liệu cần cập nhật"
                    };

                return new CommonResponse
                {
                    Status = "success",
                    Data = entity
                };
            }
            catch
            {
                return new CommonResponse
                {
                    Status = "error",
                    Message = "Đã xảy ra lỗi khi lấy dữ liệu. Vui lòng thử lại sau!"
                };
            }
        }

        public async Task<CommonResponse> GetThongTinNganChansAsync(string search, int pageSize, int pageCurrent)
        {
            try
            {
                var query = context.ThongTinNganChans.Where(t =>
                    EF.Functions.Like(t.ThongTinTaiSan, $"%{search}%") ||
                    EF.Functions.Like(t.SoQuyetDinh, $"%{search}%") ||
                    EF.Functions.Like(t.SoQuyetDinhDung, $"%{search}%"));

                var total = await query.CountAsync();
                query = query.OrderByDescending(t => t.UpdatedDate)
                    .Skip((pageCurrent - 1) * pageSize).Take(pageSize);

                var data = await query.Include(t => t.DonViBanHanh).ToListAsync();

                foreach (var item in data)
                {
                    item.DSHopDongDinhKem = await attachedFileService.GetAllAttachedFilesAsync(item.Id, "ThongTinNganChan");
                }

                return new CommonResponse
                {
                    Status = "success",
                    Data = data,
                    TotalRecord = total
                };
            }
            catch
            {
                return new CommonResponse
                {
                    Status = "error",
                    Message = "Đã xảy ra lỗi khi lấy dữ liệu. Vui lòng thử lại sau!"
                };
            }
        }

        public async Task<CommonResponse> StoreAsync(ThongTinNganChan request)
        {
            try
            {
                context.ThongTinNganChans.Add(request);
                await context.SaveChangesAsync();
                return new CommonResponse { Status = "success" };
            }
            catch
            {
                return new CommonResponse
                {
                    Status = "error",
                    Message = "Đã xảy ra lỗi khi lưu dữ liệu. Vui lòng thử lại sau!"
                };
            }
        }

        public async Task<CommonResponse> UpdateAsync(ThongTinNganChan request)
        {
            try
            {
                var entity = await context.ThongTinNganChans.FindAsync(request.Id);
                if (entity is null)
                    return new CommonResponse
                    {
                        Status = "error",
                        Message = "Không tìm thấy thông tin dữ liệu cần cập nhật"
                    };

                entity.DonViBanHanhId = request.DonViBanHanhId;
                entity.CoQuanBanHanh = request.CoQuanBanHanh;
                entity.NgayQuyetDinh = request.NgayQuyetDinh;
                entity.TrangThai = request.TrangThai;
                entity.ThongTinTaiSan = request.ThongTinTaiSan;
                entity.SoQuyetDinh = request.SoQuyetDinh;
                entity.NgayQuyetDinhDung = request.NgayQuyetDinhDung;
                entity.NgayApDungDung = request.NgayApDungDung;
                entity.SoQuyetDinhDung = request.SoQuyetDinhDung;
                entity.NgayApDung = request.NgayApDung;
                entity.CoQuanDung = request.CoQuanDung;

                context.ThongTinNganChans.Update(entity);
                await context.SaveChangesAsync();
                return new CommonResponse { Status = "success" };
            }
            catch
            {
                return new CommonResponse
                {
                    Status = "error",
                    Message = "Đã xảy ra lỗi khi cập nhật dữ liệu. Vui lòng thử lại sau!"
                };
            }
        }

        public async Task<CommonResponse> SearchThongTinNganChansAsync(
            string soQuyetDinh,
            string coQuanBanHanh,
            Guid? donViBanHanhId,
            int? namQuyetDinh,
            int? namApDung,
            TrangThaiNganChan? trangThai,
            string thongTinTaiSan,
            string soQuyetDinhDung,
            string coQuanDung,
            int? namQuyetDinhDung,
            int? namApDungDung,
            string? timkiem,
            int pageSize,
            int pageCurrent)
        {
            try
            {
                var query = context.ThongTinNganChans.AsQueryable();

                // Áp dụng các điều kiện tìm kiếm
                if (!string.IsNullOrEmpty(soQuyetDinh))
                    query = query.Where(t => EF.Functions.Like(t.SoQuyetDinh, $"%{soQuyetDinh}%"));

                if (!string.IsNullOrEmpty(coQuanBanHanh))
                    query = query.Where(t => EF.Functions.Like(t.CoQuanBanHanh, $"%{coQuanBanHanh}%"));

                if (donViBanHanhId.HasValue)
                    query = query.Where(t => t.DonViBanHanhId == donViBanHanhId.Value);

                if (namQuyetDinh.HasValue)
                    query = query.Where(t => t.NgayQuyetDinh.Year == namQuyetDinh.Value);

                if (namApDung.HasValue)
                    query = query.Where(t => t.NgayApDung.Year == namApDung.Value);

                if (trangThai.HasValue)
                    query = query.Where(t => t.TrangThai == trangThai.Value);

                if (!string.IsNullOrEmpty(thongTinTaiSan))
                    query = query.Where(t => EF.Functions.Like(t.ThongTinTaiSan, $"%{thongTinTaiSan}%"));

                // Thêm điều kiện tìm kiếm cho thông tin dừng
                if (!string.IsNullOrEmpty(soQuyetDinhDung))
                    query = query.Where(t => t.SoQuyetDinhDung != null && EF.Functions.Like(t.SoQuyetDinhDung, $"%{soQuyetDinhDung}%"));

                if (!string.IsNullOrEmpty(coQuanDung))
                    query = query.Where(t => t.CoQuanDung != null && EF.Functions.Like(t.CoQuanDung, $"%{coQuanDung}%"));

                if (namQuyetDinhDung.HasValue)
                    query = query.Where(t => t.NgayQuyetDinhDung != null && t.NgayQuyetDinhDung.Value.Year == namQuyetDinhDung.Value);

                if (namApDungDung.HasValue)
                    query = query.Where(t => t.NgayApDungDung != null && t.NgayApDungDung.Value.Year == namApDungDung.Value);

                if (!string.IsNullOrEmpty(timkiem))
                {
                    var keyword = timkiem.ToLower();
                    query = query.Where(t =>
                        (t.SoQuyetDinh != null && t.SoQuyetDinh.ToLower().Contains(keyword)) ||
                        (t.CoQuanBanHanh != null && t.CoQuanBanHanh.ToLower().Contains(keyword)) ||
                        (t.ThongTinTaiSan != null && t.ThongTinTaiSan.ToLower().Contains(keyword)) ||
                        (t.SoQuyetDinhDung != null && t.SoQuyetDinhDung.ToLower().Contains(keyword)) ||
                        (t.CoQuanDung != null && t.CoQuanDung.ToLower().Contains(keyword))
                    );
                }

                var total = await query.CountAsync();
                query = query.OrderByDescending(t => t.UpdatedDate)
                    .Skip((pageCurrent - 1) * pageSize).Take(pageSize);

                var data = await query.Include(t => t.DonViBanHanh).ToListAsync();

                foreach (var item in data)
                {
                    item.DSHopDongDinhKem = await attachedFileService.GetAllAttachedFilesAsync(item.Id, "ThongTinNganChan");
                }

                return new CommonResponse
                {
                    Status = "success",
                    Data = data,
                    TotalRecord = total
                };
            }
            catch
            {
                return new CommonResponse
                {
                    Status = "error",
                    Message = "Đã xảy ra lỗi khi lấy dữ liệu. Vui lòng thử lại sau!"
                };
            }
        }
    }
}
