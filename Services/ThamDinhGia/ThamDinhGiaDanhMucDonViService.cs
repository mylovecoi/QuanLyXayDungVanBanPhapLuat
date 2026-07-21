using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess;
using DataAccess.Entities.ThamDinhGia;
using Microsoft.EntityFrameworkCore;
using Services.Model;

namespace Services.ThamDinhGia
{
    public class ThamDinhGiaDanhMucDonViService(ApplicationDbContext context) : IThamDinhGiaDanhMucDonViService
    {
        public async Task<CommonResponse> DeleteAsync(Guid id)
        {
            try
            {
                var entity = await context.ThamDinhGiaDanhMucDonVis.FindAsync(id);
                if (entity is null)
                    return new CommonResponse
                    {
                        Status = "error",
                        Message = "Không tìm thấy thông tin dữ liệu cần xóa"
                    };

                context.ThamDinhGiaDanhMucDonVis.Remove(entity);
                await context.SaveChangesAsync();
                return new CommonResponse { Status = "success" };
            }
            catch (Exception ex)
            {
                return new CommonResponse
                {
                    Status = "error",
                    Message = "Đã xảy ra lỗi khi xóa dữ liệu: " + ex.Message
                };
            }
        }

        public async Task<CommonResponse> EditAsync(Guid id)
        {
            try
            {
                var entity = await context.ThamDinhGiaDanhMucDonVis.FindAsync(id);
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
            catch (Exception ex)
            {
                return new CommonResponse
                {
                    Status = "error",
                    Message = "Đã xảy ra lỗi khi lấy dữ liệu: " + ex.Message
                };
            }
        }

        public async Task<CommonResponse> GetDanhMucDonViAsync(string search, int pageSize, int pageCurrent)
        {
            try
            {
                var query = context.ThamDinhGiaDanhMucDonVis.AsQueryable();

                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(t =>
                        EF.Functions.Like(t.TenDv, $"%{search}%") ||
                        EF.Functions.Like(t.MaGCN, $"%{search}%") ||
                        EF.Functions.Like(t.SoQd, $"%{search}%") ||
                        EF.Functions.Like(t.DiaChi, $"%{search}%"));
                }

                var total = await query.CountAsync();
                query = query.OrderBy(t => t.STTSapXep != null ? t.STTSapXep.Length : 0).ThenBy(t => t.STTSapXep)
                    .Skip((pageCurrent - 1) * pageSize).Take(pageSize);

                var data = await query.ToListAsync();

                return new CommonResponse
                {
                    Status = "success",
                    Data = data,
                    TotalRecord = total
                };
            }
            catch (Exception ex)
            {
                return new CommonResponse
                {
                    Status = "error",
                    Message = "Đã xảy ra lỗi khi lấy dữ liệu: " + ex.Message
                };
            }
        }

        public async Task<CommonResponse> StoreAsync(ThamDinhGiaDanhMucDonVi request)
        {
            try
            {
                context.ThamDinhGiaDanhMucDonVis.Add(request);
                await context.SaveChangesAsync();
                return new CommonResponse { Status = "success" };
            }
            catch (Exception ex)
            {
                return new CommonResponse
                {
                    Status = "error",
                    Message = "Đã xảy ra lỗi khi lưu dữ liệu: " + ex.Message
                };
            }
        }

        public async Task<CommonResponse> StoreRangeAsync(List<ThamDinhGiaDanhMucDonVi> requests)
        {
            try
            {
                foreach (var item in requests)
                {
                    item.Id = Guid.NewGuid();
                    context.ThamDinhGiaDanhMucDonVis.Add(item);
                }
                await context.SaveChangesAsync();
                return new CommonResponse { Status = "success", Message = "Nhập dữ liệu thành công!" };
            }
            catch (Exception ex)
            {
                return new CommonResponse
                {
                    Status = "error",
                    Message = "Đã xảy ra lỗi khi lưu dữ liệu hàng loạt: " + ex.Message
                };
            }
        }

        public async Task<CommonResponse> UpdateAsync(ThamDinhGiaDanhMucDonVi request)
        {
            try
            {
                var entity = await context.ThamDinhGiaDanhMucDonVis.FindAsync(request.Id);
                if (entity is null)
                    return new CommonResponse
                    {
                        Status = "error",
                        Message = "Không tìm thấy thông tin dữ liệu cần cập nhật"
                    };

                entity.MaGCN = request.MaGCN;
                entity.TenDv = request.TenDv;
                entity.DiaChi = request.DiaChi;
                entity.NguoiDaiDien = request.NguoiDaiDien;
                entity.ChucVu = request.ChucVu;
                entity.SoThe = request.SoThe;
                entity.NgayCap = request.NgayCap;
                entity.SoQd = request.SoQd;
                entity.NgayQd = request.NgayQd;
                entity.TrangThai = request.TrangThai;
                entity.STTSapXep = request.STTSapXep;

                context.ThamDinhGiaDanhMucDonVis.Update(entity);
                await context.SaveChangesAsync();
                return new CommonResponse { Status = "success" };
            }
            catch (Exception ex)
            {
                return new CommonResponse
                {
                    Status = "error",
                    Message = "Đã xảy ra lỗi khi cập nhật dữ liệu: " + ex.Message
                };
            }
        }
    }
}
