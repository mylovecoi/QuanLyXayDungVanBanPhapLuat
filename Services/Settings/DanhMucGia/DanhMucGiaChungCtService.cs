using DataAccess;
using DataAccess.Entities.Settings;
using DataAccess.Entities.Settings.DanhMucGia;
using Microsoft.EntityFrameworkCore;
using Services.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Settings.DanhMucGia
{
    public class DanhMucGiaChungCtService(ApplicationDbContext dbContext) : IDanhMucGiaChungCtService
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        public async Task<CommonResponse> GetListDanhMucCtAsync(Guid danhMucGiaChungId, string search, int pageSize, int pageCurrent)
        {
            var query = _dbContext.DanhMucGiaChungCts
                .Where(x => x.DanhMucGiaChungId == danhMucGiaChungId)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(x =>
                    (x.TenChiTiet != null && x.TenChiTiet.ToLower().Contains(search)) ||
                    (x.MaChiTiet != null && x.MaChiTiet.ToLower().Contains(search))
                );
            }

            query = query.OrderBy(x => x.STTSapXep);

            var totalRecord = await query.CountAsync();
            var dataView = await query.Skip((pageCurrent - 1) * pageSize).Take(pageSize).ToListAsync();

            return new CommonResponse
            {
                Status = "success",
                Data = dataView,
                TotalRecord = totalRecord
            };
        }

        public async Task<CommonResponse> StoreAsync(DanhMucGiaChungCt request)
        {
            try
            {
                request.Id = Guid.NewGuid();

                // Đẩy manghe ở bảng cha vào
                var parent = await _dbContext.DanhMucGiaChungs.FindAsync(request.DanhMucGiaChungId);
                if (parent != null)
                {
                    request.MaNghe = parent.MaNghe;
                }

                if (request.STTSapXep == 0)
                {
                    var currentItems = await _dbContext.DanhMucGiaChungCts
                        .Where(x => x.DanhMucGiaChungId == request.DanhMucGiaChungId)
                        .ToListAsync();

                    int maxStt = currentItems.Any() ? currentItems.Max(x => x.STTSapXep) : 0;
                    request.STTSapXep = maxStt + 1;
                }

                _dbContext.DanhMucGiaChungCts.Add(request);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success", Message = "Thêm chi tiết danh mục giá chung thành công!" };
            }
            catch (Exception ex)
            {
                return new CommonResponse { Status = "error", Message = "Không thể thêm chi tiết danh mục: " + ex.Message };
            }
        }

        public async Task<CommonResponse> EditAsync(Guid id)
        {
            try
            {
                var data = await _dbContext.DanhMucGiaChungCts.FindAsync(id);
                if (data == null) return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin chi tiết danh mục!" };
                return new CommonResponse { Status = "success", Data = data };
            }
            catch (Exception ex)
            {
                return new CommonResponse { Status = "error", Message = ex.Message };
            }
        }

        public async Task<CommonResponse> UpdateAsync(DanhMucGiaChungCt request)
        {
            try
            {
                var data = await _dbContext.DanhMucGiaChungCts.FindAsync(request.Id);
                if (data == null) return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin cần cập nhật!" };

                data.MaChiTiet = request.MaChiTiet;
                data.TenChiTiet = request.TenChiTiet;
                data.STTSapXep = request.STTSapXep;

                // Đồng bộ cả MaNghe từ cha sang chi tiết nếu thay đổi
                var parent = await _dbContext.DanhMucGiaChungs.FindAsync(data.DanhMucGiaChungId);
                if (parent != null)
                {
                    data.MaNghe = parent.MaNghe;
                }

                _dbContext.DanhMucGiaChungCts.Update(data);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success", Message = "Cập nhật chi tiết danh mục giá chung thành công!" };
            }
            catch (Exception ex)
            {
                return new CommonResponse { Status = "error", Message = "Không thể cập nhật chi tiết danh mục: " + ex.Message };
            }
        }

        public async Task<CommonResponse> DeleteAsync(Guid id)
        {
            try
            {
                var data = await _dbContext.DanhMucGiaChungCts.FindAsync(id);
                if (data == null) return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin chi tiết danh mục!" };

                _dbContext.DanhMucGiaChungCts.Remove(data);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success", Message = "Xóa chi tiết danh mục giá chung thành công!" };
            }
            catch (Exception ex)
            {
                return new CommonResponse { Status = "error", Message = "Không thể xóa chi tiết danh mục: " + ex.Message };
            }
        }

        public async Task<CommonResponse> DeleteAllAsync(Guid danhMucGiaChungId)
        {
            try
            {
                var list = await _dbContext.DanhMucGiaChungCts.Where(x => x.DanhMucGiaChungId == danhMucGiaChungId).ToListAsync();
                if (list.Any())
                {
                    _dbContext.DanhMucGiaChungCts.RemoveRange(list);
                    await _dbContext.SaveChangesAsync();
                }
                return new CommonResponse { Status = "success", Message = "Xóa toàn bộ chi tiết danh mục giá chung thành công!" };
            }
            catch (Exception ex)
            {
                return new CommonResponse { Status = "error", Message = "Không thể xóa toàn bộ chi tiết danh mục: " + ex.Message };
            }
        }

        public async Task<CommonResponse> StoreRangeAsync(List<DanhMucGiaChungCt> requests)
        {
            try
            {
                if (requests.Count == 0)
                {
                    return new CommonResponse { Status = "error", Message = "Không có dữ liệu để lưu!" };
                }

                Guid parentId = requests[0].DanhMucGiaChungId;
                var parent = await _dbContext.DanhMucGiaChungs.FindAsync(parentId);
                string? maNghe = parent?.MaNghe;

                var now = DateTime.Now;
                for (int i = 0; i < requests.Count; i++)
                {
                    var request = requests[i];
                    request.Id = Guid.NewGuid();
                    request.CreatedDate = now.AddSeconds(-i);
                    request.MaNghe = maNghe; // Đẩy manghe ở bảng cha vào
                    _dbContext.DanhMucGiaChungCts.Add(request);
                }
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success", Message = $"Nhận thành công {requests.Count} chi tiết giá chung từ file Excel!" };
            }
            catch (Exception ex)
            {
                return new CommonResponse { Status = "error", Message = "Không thể lưu dữ liệu từ Excel: " + ex.Message };
            }
        }
    }
}
