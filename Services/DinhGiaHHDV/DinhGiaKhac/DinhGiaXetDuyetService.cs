using DataAccess;
using DataAccess.Entities.DinhGiaHHDV;
using Microsoft.EntityFrameworkCore;
using Services.DTOs.DinhGiaHHDV.ThongTinHoSo;
using Services.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.DinhGiaHHDV.DinhGiaKhac
{
    public class DinhGiaXetDuyetService(ApplicationDbContext dbContext) : IDinhGiaXetDuyetService
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        public async Task<CommonResponse> GetListXetDuyetByFilterAsync(DinhGiaFilter filter, string MaNghe)
        {
            try
            {
                IQueryable<DinhGia> queryable = _dbContext.DinhGias
                    .Include(x => x.DonViQuanLy)
                    .AsNoTracking()
                    .Where(x => x.TrangThai != "CXD" && x.TrangThai != "CC" && x.TrangThai != "BTL");

                if (!string.IsNullOrEmpty(MaNghe) && MaNghe != "all")
                {
                    queryable = queryable.Where(x => x.MaNghe == MaNghe);
                }

                if (filter.TargetYear > 0) queryable = queryable.Where(x => x.ThoiDiem.Year == filter.TargetYear);

                if (!string.IsNullOrEmpty(filter.Search))
                    queryable = queryable.Where(x =>
                        (EF.Functions.Like(x.MaHoSo, $"%{filter.Search}%") ||
                        EF.Functions.Like(x.ThoiDiem.Year.ToString(), $"%{filter.Search}%")));

                int totalRecord = await queryable.CountAsync();
                filter.AdjustPageIfInvalid(totalRecord);

                queryable = queryable.OrderByDescending(x => x.ThoiDiem).ThenBy(x => x.MaHoSo);

                var dataView = queryable.Skip((filter.PageCurrent - 1) * filter.PageSize).Take(filter.PageSize).ToList();

                return new("success", "Lấy thông tin danh sách xét duyệt thành công", dataView, totalRecord);
            }
            catch (Exception)
            {
                return new("error", "Có lỗi trong quá trình lấy dữ liệu. Hãy kiểm tra lại!");
            }
        }

        public async Task<CommonResponse> GetSingleByIdAsync(Guid hoSoId)
        {
            try
            {
                var model = await _dbContext.DinhGias.FirstOrDefaultAsync(t => t.Id == hoSoId);
                if (model == null) return new("error", "Hồ sơ không tồn tại");
                return new("success", "Thành công", model);
            }
            catch (Exception)
            {
                return new("error", "Có lỗi xảy ra khi lấy thông tin hồ sơ");
            }
        }

        public async Task<CommonResponse> DuyetAsync(Guid id)
        {
            try
            {
                var model = await _dbContext.DinhGias.FirstOrDefaultAsync(t => t.Id == id);
                if (model == null) return new("error", "Không tìm thấy hồ sơ");

                model.TrangThai = "DD"; // Đã duyệt
                model.UpdatedDate = DateTime.Now;
                _dbContext.DinhGias.Update(model);
                await _dbContext.SaveChangesAsync();

                return new("success", "Duyệt hồ sơ thành công");
            }
            catch (Exception ex)
            {
                return new("error", "Lỗi duyệt hồ sơ: " + ex.Message);
            }
        }

        public async Task<CommonResponse> HuyDuyetAsync(Guid id)
        {
            try
            {
                var model = await _dbContext.DinhGias.FirstOrDefaultAsync(t => t.Id == id);
                if (model == null) return new("error", "Không tìm thấy hồ sơ");

                model.TrangThai = "CD"; // Chờ duyệt
                model.UpdatedDate = DateTime.Now;
                _dbContext.DinhGias.Update(model);
                await _dbContext.SaveChangesAsync();

                return new("success", "Hủy duyệt hồ sơ thành công");
            }
            catch (Exception ex)
            {
                return new("error", "Lỗi hủy duyệt hồ sơ: " + ex.Message);
            }
        }

        public async Task<CommonResponse> TraLaiAsync(Guid id, string lyDo)
        {
            try
            {
                var model = await _dbContext.DinhGias.FirstOrDefaultAsync(t => t.Id == id);
                if (model == null) return new("error", "Không tìm thấy hồ sơ");

                model.TrangThai = "BTL"; // Bị trả lại
                model.LyDo = lyDo;
                model.UpdatedDate = DateTime.Now;
                _dbContext.DinhGias.Update(model);
                await _dbContext.SaveChangesAsync();

                return new("success", "Trả lại hồ sơ thành công");
            }
            catch (Exception ex)
            {
                return new("error", "Lỗi trả lại hồ sơ: " + ex.Message);
            }
        }

        public async Task<CommonResponse> CongBoAsync(Guid id)
        {
            try
            {
                var model = await _dbContext.DinhGias.FirstOrDefaultAsync(t => t.Id == id);
                if (model == null) return new("error", "Không tìm thấy hồ sơ");

                model.TrangThai = "CB"; // Công bố
                model.UpdatedDate = DateTime.Now;
                _dbContext.DinhGias.Update(model);
                await _dbContext.SaveChangesAsync();

                return new("success", "Công bố hồ sơ thành công");
            }
            catch (Exception ex)
            {
                return new("error", "Lỗi công bố hồ sơ: " + ex.Message);
            }
        }

        public async Task<CommonResponse> HuyCongBoAsync(Guid id)
        {
            try
            {
                var model = await _dbContext.DinhGias.FirstOrDefaultAsync(t => t.Id == id);
                if (model == null) return new("error", "Không tìm thấy hồ sơ");

                model.TrangThai = "DD"; // Hủy công bố chuyển về Đã duyệt
                model.UpdatedDate = DateTime.Now;
                _dbContext.DinhGias.Update(model);
                await _dbContext.SaveChangesAsync();

                return new("success", "Hủy công bố hồ sơ thành công");
            }
            catch (Exception ex)
            {
                return new("error", "Lỗi hủy công bố hồ sơ: " + ex.Message);
            }
        }
    }
}
