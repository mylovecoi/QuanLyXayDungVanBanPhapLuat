using DataAccess;
using DataAccess.Entities.Settings;
using DataAccess.Enums;
using Microsoft.EntityFrameworkCore;
using Services.Model;

namespace Services.Settings
{
    public interface IDanhMucPhongBanService
    {
        Task<CommonResponse> GetDanhMucPhongBanAsync(string search, int pageSize, int currentPage, Guid? donViId = null, LoaiPhongBan? loaiPhongBan = null);
        Task<CommonResponse> StoreAsync(DanhMucPhongBan request);
        Task<CommonResponse> EditAsync(Guid id);
        Task<CommonResponse> UpdateAsync(DanhMucPhongBan request);
        Task<CommonResponse> DeleteAsync(Guid id);
    }

    public class DanhMucPhongBanService(ApplicationDbContext context) : IDanhMucPhongBanService
    {
        public async Task<CommonResponse> DeleteAsync(Guid id)
        {
            try
            {
                var entity = await context.DanhMucPhongBans.FindAsync(id);
                if (entity is null)
                    return new CommonResponse
                    {
                        Status = "error",
                        Message = "Không tìm thấy thông tin dữ liệu cần xóa"
                    };

                context.DanhMucPhongBans.Remove(entity);
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
                var entity = await context.DanhMucPhongBans
                    .Include(d => d.DanhMucDonVi)
                    .FirstOrDefaultAsync(d => d.Id == id);

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

        public async Task<CommonResponse> GetDanhMucPhongBanAsync(string search, int pageSize, int pageCurrent, Guid? donViId = null, LoaiPhongBan? loaiPhongBan = null)
        {
            try
            {
                var query = context.DanhMucPhongBans.Where(t =>
                    EF.Functions.Like(t.TenPhongBan, $"%{search}%") ||
                    EF.Functions.Like(t.MaPhongBan, $"%{search}%"));

                // Lọc theo đơn vị nếu có
                if (donViId.HasValue && donViId != Guid.Empty)
                {
                    query = query.Where(t => t.DanhMucDonViId == donViId);
                }

                // Lọc theo loại phòng ban nếu có
                if (loaiPhongBan.HasValue)
                {
                    query = query.Where(t => t.LoaiPhongBan == loaiPhongBan);
                }

                var total = await query.CountAsync();
                query = query.OrderByDescending(t => t.UpdatedDate)
                    .Skip((pageCurrent - 1) * pageSize).Take(pageSize);

                var data = await query.Include(d => d.DanhMucDonVi).ToListAsync();

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

        public async Task<CommonResponse> StoreAsync(DanhMucPhongBan request)
        {
            try
            {
                context.DanhMucPhongBans.Add(request);
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

        public async Task<CommonResponse> UpdateAsync(DanhMucPhongBan request)
        {
            try
            {
                var entity = await context.DanhMucPhongBans.FindAsync(request.Id);
                if (entity is null)
                    return new CommonResponse
                    {
                        Status = "error",
                        Message = "Không tìm thấy thông tin dữ liệu cần cập nhật"
                    };

                entity.TenPhongBan = request.TenPhongBan;
                entity.MaPhongBan = request.MaPhongBan;
                entity.LoaiPhongBan = request.LoaiPhongBan;
                entity.DanhMucDonViId = request.DanhMucDonViId;

                context.DanhMucPhongBans.Update(entity);
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
    }
}