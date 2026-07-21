using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DataAccess;
using DataAccess.Entities.KeKhaiDangKyGia;
using Services.Model;

namespace Services.KeKhaiDangKyGia
{
    public class KeKhaiDangKyGiaDanhMucService(ApplicationDbContext dbContext) : IKeKhaiDangKyGiaDanhMucService
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        #region Danh mục đối tượng (KeKhaiDangKyGiaDMDT)
        public async Task<CommonResponse> GetListDTAsync(string doanhNghiepQuanLyIdStr, string search, int pageSize, int pageCurrent)
        {
            var query = _dbContext.KeKhaiDangKyGiaDMDTs.AsQueryable();

            if (!string.IsNullOrEmpty(doanhNghiepQuanLyIdStr) && !doanhNghiepQuanLyIdStr.Equals("all", StringComparison.OrdinalIgnoreCase))
            {
                if (Guid.TryParse(doanhNghiepQuanLyIdStr, out var dnId))
                {
                    query = query.Where(x => x.DoanhNghiepQuanLyID == dnId);
                }
            }

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(x =>
                    (x.MaDT != null && x.MaDT.ToLower().Contains(search)) ||
                    (x.TenDT != null && x.TenDT.ToLower().Contains(search)) ||
                    (x.GhiChu != null && x.GhiChu.ToLower().Contains(search))
                );
            }

            query = query.OrderBy(x => x.MaDT);

            var totalRecord = await query.CountAsync();
            var dataView = await query.Skip((pageCurrent - 1) * pageSize).Take(pageSize).ToListAsync();

            return new CommonResponse
            {
                Status = "success",
                Data = dataView,
                TotalRecord = totalRecord
            };
        }

        public async Task<CommonResponse> StoreDTAsync(KeKhaiDangKyGiaDMDT request)
        {
            try
            {
                request.Id = Guid.NewGuid();
                _dbContext.KeKhaiDangKyGiaDMDTs.Add(request);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success", Message = "Thêm đối tượng thành công!" };
            }
            catch (Exception ex)
            {
                return new CommonResponse { Status = "error", Message = "Không thể thêm đối tượng: " + ex.Message };
            }
        }

        public async Task<CommonResponse> EditDTAsync(Guid id)
        {
            try
            {
                var data = await _dbContext.KeKhaiDangKyGiaDMDTs.FindAsync(id);
                if (data == null) return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin đối tượng!" };
                return new CommonResponse { Status = "success", Data = data };
            }
            catch (Exception ex)
            {
                return new CommonResponse { Status = "error", Message = ex.Message };
            }
        }

        public async Task<CommonResponse> UpdateDTAsync(KeKhaiDangKyGiaDMDT request)
        {
            try
            {
                var data = await _dbContext.KeKhaiDangKyGiaDMDTs.FindAsync(request.Id);
                if (data == null) return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin cần cập nhật!" };

                data.MaDT = request.MaDT;
                data.TenDT = request.TenDT;
                data.GhiChu = request.GhiChu;

                _dbContext.KeKhaiDangKyGiaDMDTs.Update(data);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success", Message = "Cập nhật đối tượng thành công!" };
            }
            catch (Exception ex)
            {
                return new CommonResponse { Status = "error", Message = "Không thể cập nhật đối tượng: " + ex.Message };
            }
        }

        public async Task<CommonResponse> DeleteDTAsync(Guid id)
        {
            try
            {
                var data = await _dbContext.KeKhaiDangKyGiaDMDTs.FindAsync(id);
                if (data == null) return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin đối tượng!" };

                _dbContext.KeKhaiDangKyGiaDMDTs.Remove(data);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success", Message = "Xóa đối tượng thành công!" };
            }
            catch (Exception ex)
            {
                return new CommonResponse { Status = "error", Message = "Không thể xóa đối tượng: " + ex.Message };
            }
        }
        #endregion

        #region Danh mục hàng hóa (KeKhaiDangKyGiaDMHH)
        public async Task<CommonResponse> GetListHHAsync(string doanhNghiepQuanLyIdStr, string search, int pageSize, int pageCurrent)
        {
            var query = _dbContext.KeKhaiDangKyGiaDMHHs.AsQueryable();

            if (!string.IsNullOrEmpty(doanhNghiepQuanLyIdStr) && !doanhNghiepQuanLyIdStr.Equals("all", StringComparison.OrdinalIgnoreCase))
            {
                if (Guid.TryParse(doanhNghiepQuanLyIdStr, out var dnId))
                {
                    query = query.Where(x => x.DoanhNghiepQuanLyID == dnId);
                }
            }

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(x =>
                    (x.MaDVCU != null && x.MaDVCU.ToLower().Contains(search)) ||
                    (x.TenDvCungUng != null && x.TenDvCungUng.ToLower().Contains(search)) ||
                    (x.QuyCachChatLuong != null && x.QuyCachChatLuong.ToLower().Contains(search)) ||
                    (x.DonViTinh != null && x.DonViTinh.ToLower().Contains(search)) ||
                    (x.MaHH_BTC != null && x.MaHH_BTC.ToLower().Contains(search)) ||
                    (x.GhiChu != null && x.GhiChu.ToLower().Contains(search))
                );
            }

            query = query.OrderBy(x => x.MaDVCU);

            var totalRecord = await query.CountAsync();
            var dataView = await query.Skip((pageCurrent - 1) * pageSize).Take(pageSize).ToListAsync();

            return new CommonResponse
            {
                Status = "success",
                Data = dataView,
                TotalRecord = totalRecord
            };
        }

        public async Task<CommonResponse> StoreHHAsync(KeKhaiDangKyGiaDMHH request)
        {
            try
            {
                request.Id = Guid.NewGuid();
                _dbContext.KeKhaiDangKyGiaDMHHs.Add(request);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success", Message = "Thêm hàng hóa thành công!" };
            }
            catch (Exception ex)
            {
                return new CommonResponse { Status = "error", Message = "Không thể thêm hàng hóa: " + ex.Message };
            }
        }

        public async Task<CommonResponse> EditHHAsync(Guid id)
        {
            try
            {
                var data = await _dbContext.KeKhaiDangKyGiaDMHHs.FindAsync(id);
                if (data == null) return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin hàng hóa!" };
                return new CommonResponse { Status = "success", Data = data };
            }
            catch (Exception ex)
            {
                return new CommonResponse { Status = "error", Message = ex.Message };
            }
        }

        public async Task<CommonResponse> UpdateHHAsync(KeKhaiDangKyGiaDMHH request)
        {
            try
            {
                var data = await _dbContext.KeKhaiDangKyGiaDMHHs.FindAsync(request.Id);
                if (data == null) return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin cần cập nhật!" };

                data.MaNghe = request.MaNghe;
                data.MaDVCU = request.MaDVCU;
                data.TenDvCungUng = request.TenDvCungUng;
                data.QuyCachChatLuong = request.QuyCachChatLuong;
                data.DonViTinh = request.DonViTinh;
                data.GhiChu = request.GhiChu;
                data.MaHH_BTC = request.MaHH_BTC;

                _dbContext.KeKhaiDangKyGiaDMHHs.Update(data);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success", Message = "Cập nhật hàng hóa thành công!" };
            }
            catch (Exception ex)
            {
                return new CommonResponse { Status = "error", Message = "Không thể cập nhật hàng hóa: " + ex.Message };
            }
        }

        public async Task<CommonResponse> DeleteHHAsync(Guid id)
        {
            try
            {
                var data = await _dbContext.KeKhaiDangKyGiaDMHHs.FindAsync(id);
                if (data == null) return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin hàng hóa!" };

                _dbContext.KeKhaiDangKyGiaDMHHs.Remove(data);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success", Message = "Xóa hàng hóa thành công!" };
            }
            catch (Exception ex)
            {
                return new CommonResponse { Status = "error", Message = "Không thể xóa hàng hóa: " + ex.Message };
            }
        }
        #endregion

        #region Danh mục kho hàng (KeKhaiDangKyGiaDMKH)
        public async Task<CommonResponse> GetListKHAsync(string doanhNghiepQuanLyIdStr, string search, int pageSize, int pageCurrent)
        {
            var query = _dbContext.KeKhaiDangKyGiaDMKHs.AsQueryable();

            if (!string.IsNullOrEmpty(doanhNghiepQuanLyIdStr) && !doanhNghiepQuanLyIdStr.Equals("all", StringComparison.OrdinalIgnoreCase))
            {
                if (Guid.TryParse(doanhNghiepQuanLyIdStr, out var dnId))
                {
                    query = query.Where(x => x.DoanhNghiepQuanLyID == dnId);
                }
            }

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(x =>
                    (x.MaKH != null && x.MaKH.ToLower().Contains(search)) ||
                    (x.TenKH != null && x.TenKH.ToLower().Contains(search)) ||
                    (x.DiaChi != null && x.DiaChi.ToLower().Contains(search)) ||
                    (x.GhiChu != null && x.GhiChu.ToLower().Contains(search))
                );
            }

            query = query.OrderBy(x => x.MaKH);

            var totalRecord = await query.CountAsync();
            var dataView = await query.Skip((pageCurrent - 1) * pageSize).Take(pageSize).ToListAsync();

            return new CommonResponse
            {
                Status = "success",
                Data = dataView,
                TotalRecord = totalRecord
            };
        }

        public async Task<CommonResponse> StoreKHAsync(KeKhaiDangKyGiaDMKH request)
        {
            try
            {
                request.Id = Guid.NewGuid();
                _dbContext.KeKhaiDangKyGiaDMKHs.Add(request);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success", Message = "Thêm kho hàng thành công!" };
            }
            catch (Exception ex)
            {
                return new CommonResponse { Status = "error", Message = "Không thể thêm kho hàng: " + ex.Message };
            }
        }

        public async Task<CommonResponse> EditKHAsync(Guid id)
        {
            try
            {
                var data = await _dbContext.KeKhaiDangKyGiaDMKHs.FindAsync(id);
                if (data == null) return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin kho hàng!" };
                return new CommonResponse { Status = "success", Data = data };
            }
            catch (Exception ex)
            {
                return new CommonResponse { Status = "error", Message = ex.Message };
            }
        }

        public async Task<CommonResponse> UpdateKHAsync(KeKhaiDangKyGiaDMKH request)
        {
            try
            {
                var data = await _dbContext.KeKhaiDangKyGiaDMKHs.FindAsync(request.Id);
                if (data == null) return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin cần cập nhật!" };

                data.MaKH = request.MaKH;
                data.TenKH = request.TenKH;
                data.DiaChi = request.DiaChi;
                data.GhiChu = request.GhiChu;

                _dbContext.KeKhaiDangKyGiaDMKHs.Update(data);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success", Message = "Cập nhật kho hàng thành công!" };
            }
            catch (Exception ex)
            {
                return new CommonResponse { Status = "error", Message = "Không thể cập nhật kho hàng: " + ex.Message };
            }
        }

        public async Task<CommonResponse> DeleteKHAsync(Guid id)
        {
            try
            {
                var data = await _dbContext.KeKhaiDangKyGiaDMKHs.FindAsync(id);
                if (data == null) return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin kho hàng!" };

                _dbContext.KeKhaiDangKyGiaDMKHs.Remove(data);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success", Message = "Xóa kho hàng thành công!" };
            }
            catch (Exception ex)
            {
                return new CommonResponse { Status = "error", Message = "Không thể xóa kho hàng: " + ex.Message };
            }
        }
        #endregion
    }
}
