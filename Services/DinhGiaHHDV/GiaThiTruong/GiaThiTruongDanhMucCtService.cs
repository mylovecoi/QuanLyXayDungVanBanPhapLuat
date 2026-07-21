using DataAccess;
using DataAccess.Entities.DinhGiaHHDV;
using Microsoft.EntityFrameworkCore;
using Services.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.DinhGiaHHDV.GiaThiTruong
{
    public class GiaThiTruongDanhMucCtService(ApplicationDbContext dbContext) : IGiaThiTruongDanhMucCtService
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        public async Task<CommonResponse> GetListDanhMucCtAsync(Guid thongTuId, string search, int pageSize, int pageCurrent)
        {
            var query = _dbContext.GiaThiTruongDanhMucCts
                .Where(x => x.ThongTuId == thongTuId)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(x =>
                    (x.TenHhDv != null && x.TenHhDv.ToLower().Contains(search)) ||
                    (x.MaHhDv != null && x.MaHhDv.ToLower().Contains(search))
                );
            }

            query = query.OrderBy(x => x.STTSapXep != null ? x.STTSapXep.Length : 0)
                         .ThenBy(x => x.STTSapXep);

            var totalRecord = await query.CountAsync();
            var dataView = await query.Skip((pageCurrent - 1) * pageSize).Take(pageSize).ToListAsync();

            return new CommonResponse
            {
                Status = "success",
                Data = dataView,
                TotalRecord = totalRecord
            };
        }

        public async Task<CommonResponse> StoreAsync(GiaThiTruongDanhMucCt request)
        {
            try
            {
                request.Id = Guid.NewGuid();
                request.TheoDoi = string.IsNullOrEmpty(request.TheoDoi) ? "TD" : request.TheoDoi;
                
                if (string.IsNullOrEmpty(request.STTSapXep))
                {
                    var currentItems = await _dbContext.GiaThiTruongDanhMucCts
                        .Where(x => x.ThongTuId == request.ThongTuId)
                        .ToListAsync();
                    
                    int maxStt = 0;
                    foreach (var item in currentItems)
                    {
                        if (int.TryParse(item.STTSapXep, out int sttVal))
                        {
                            if (sttVal > maxStt)
                            {
                                maxStt = sttVal;
                            }
                        }
                    }
                    request.STTSapXep = (maxStt + 1).ToString();
                }

                _dbContext.GiaThiTruongDanhMucCts.Add(request);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success", Message = "Thêm chi tiết danh mục giá thị trường thành công!" };
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
                var data = await _dbContext.GiaThiTruongDanhMucCts.FindAsync(id);
                if (data == null) return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin chi tiết danh mục!" };
                return new CommonResponse { Status = "success", Data = data };
            }
            catch (Exception ex)
            {
                return new CommonResponse { Status = "error", Message = ex.Message };
            }
        }

        public async Task<CommonResponse> UpdateAsync(GiaThiTruongDanhMucCt request)
        {
            try
            {
                var data = await _dbContext.GiaThiTruongDanhMucCts.FindAsync(request.Id);
                if (data == null) return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin cần cập nhật!" };

                data.MaHhDv = request.MaHhDv;
                data.TenHhDv = request.TenHhDv;
                data.DacDiemKt = request.DacDiemKt;
                data.DonViTinh = request.DonViTinh;
                data.TheoDoi = string.IsNullOrEmpty(request.TheoDoi) ? "TD" : request.TheoDoi;
                data.STTSapXep = request.STTSapXep;

                _dbContext.GiaThiTruongDanhMucCts.Update(data);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success", Message = "Cập nhật chi tiết danh mục giá thị trường thành công!" };
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
                var data = await _dbContext.GiaThiTruongDanhMucCts.FindAsync(id);
                if (data == null) return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin chi tiết danh mục!" };

                _dbContext.GiaThiTruongDanhMucCts.Remove(data);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success", Message = "Xóa chi tiết danh mục giá thị trường thành công!" };
            }
            catch (Exception ex)
            {
                return new CommonResponse { Status = "error", Message = "Không thể xóa chi tiết danh mục: " + ex.Message };
            }
        }

        public async Task<CommonResponse> DeleteAllAsync(Guid thongTuId)
        {
            try
            {
                var list = await _dbContext.GiaThiTruongDanhMucCts.Where(x => x.ThongTuId == thongTuId).ToListAsync();
                if (list.Any())
                {
                    _dbContext.GiaThiTruongDanhMucCts.RemoveRange(list);
                    await _dbContext.SaveChangesAsync();
                }
                return new CommonResponse { Status = "success", Message = "Xóa toàn bộ chi tiết danh mục giá thị trường thành công!" };
            }
            catch (Exception ex)
            {
                return new CommonResponse { Status = "error", Message = "Không thể xóa toàn bộ chi tiết danh mục: " + ex.Message };
            }
        }

        public async Task<CommonResponse> StoreRangeAsync(List<GiaThiTruongDanhMucCt> requests)
        {
            try
            {
                var now = DateTime.Now;
                for (int i = 0; i < requests.Count; i++)
                {
                    var request = requests[i];
                    request.Id = Guid.NewGuid();
                    request.TheoDoi = string.IsNullOrEmpty(request.TheoDoi) ? "TD" : request.TheoDoi;
                    request.CreatedDate = now.AddSeconds(-i);
                    _dbContext.GiaThiTruongDanhMucCts.Add(request);
                }
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success", Message = $"Nhận thành công {requests.Count} chi tiết hàng hóa dịch vụ từ file Excel!" };
            }
            catch (Exception ex)
            {
                return new CommonResponse { Status = "error", Message = "Không thể lưu dữ liệu từ Excel: " + ex.Message };
            }
        }
    }
}
