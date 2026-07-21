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
    public class DanhMucNuocSachCtService(ApplicationDbContext dbContext) : IDanhMucNuocSachCtService
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        public async Task<CommonResponse> GetListDanhMucCtAsync(Guid danhMucNuocSachId, string search, int pageSize, int pageCurrent)
        {
            var query = _dbContext.DanhMucNuocSachCts
                .Where(x => x.DanhMucNuocSachId == danhMucNuocSachId)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(x =>
                    (x.DoiTuongSuDung != null && x.DoiTuongSuDung.ToLower().Contains(search)) ||
                    (x.MaDoiTuong != null && x.MaDoiTuong.ToLower().Contains(search))
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

        public async Task<CommonResponse> StoreAsync(DanhMucNuocSachCt request)
        {
            try
            {
                request.Id = Guid.NewGuid();

                if (request.STTSapXep == 0)
                {
                    var currentItems = await _dbContext.DanhMucNuocSachCts
                        .Where(x => x.DanhMucNuocSachId == request.DanhMucNuocSachId)
                        .ToListAsync();

                    int maxStt = currentItems.Any() ? currentItems.Max(x => x.STTSapXep) : 0;
                    request.STTSapXep = maxStt + 1;
                }

                _dbContext.DanhMucNuocSachCts.Add(request);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success", Message = "Thêm chi tiết danh mục nước sạch thành công!" };
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
                var data = await _dbContext.DanhMucNuocSachCts.FindAsync(id);
                if (data == null) return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin chi tiết danh mục!" };
                return new CommonResponse { Status = "success", Data = data };
            }
            catch (Exception ex)
            {
                return new CommonResponse { Status = "error", Message = ex.Message };
            }
        }

        public async Task<CommonResponse> UpdateAsync(DanhMucNuocSachCt request)
        {
            try
            {
                var data = await _dbContext.DanhMucNuocSachCts.FindAsync(request.Id);
                if (data == null) return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin cần cập nhật!" };

                data.MaDoiTuong = request.MaDoiTuong;
                data.DoiTuongSuDung = request.DoiTuongSuDung;
                data.STTSapXep = request.STTSapXep;

                _dbContext.DanhMucNuocSachCts.Update(data);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success", Message = "Cập nhật chi tiết danh mục nước sạch thành công!" };
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
                var data = await _dbContext.DanhMucNuocSachCts.FindAsync(id);
                if (data == null) return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin chi tiết danh mục!" };

                _dbContext.DanhMucNuocSachCts.Remove(data);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success", Message = "Xóa chi tiết danh mục nước sạch thành công!" };
            }
            catch (Exception ex)
            {
                return new CommonResponse { Status = "error", Message = "Không thể xóa chi tiết danh mục: " + ex.Message };
            }
        }

        public async Task<CommonResponse> DeleteAllAsync(Guid danhMucNuocSachId)
        {
            try
            {
                var list = await _dbContext.DanhMucNuocSachCts.Where(x => x.DanhMucNuocSachId == danhMucNuocSachId).ToListAsync();
                if (list.Any())
                {
                    _dbContext.DanhMucNuocSachCts.RemoveRange(list);
                    await _dbContext.SaveChangesAsync();
                }
                return new CommonResponse { Status = "success", Message = "Xóa toàn bộ chi tiết danh mục nước sạch thành công!" };
            }
            catch (Exception ex)
            {
                return new CommonResponse { Status = "error", Message = "Không thể xóa toàn bộ chi tiết danh mục: " + ex.Message };
            }
        }

        public async Task<CommonResponse> StoreRangeAsync(List<DanhMucNuocSachCt> requests)
        {
            try
            {
                var now = DateTime.Now;
                for (int i = 0; i < requests.Count; i++)
                {
                    var request = requests[i];
                    request.Id = Guid.NewGuid();
                    request.CreatedDate = now.AddSeconds(-i);
                    _dbContext.DanhMucNuocSachCts.Add(request);
                }
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success", Message = $"Nhận thành công {requests.Count} chi tiết nước sạch từ file Excel!" };
            }
            catch (Exception ex)
            {
                return new CommonResponse { Status = "error", Message = "Không thể lưu dữ liệu từ Excel: " + ex.Message };
            }
        }
    }
}
