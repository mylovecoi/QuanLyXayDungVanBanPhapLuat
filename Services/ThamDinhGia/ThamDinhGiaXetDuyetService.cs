using DataAccess;
using DataAccess.Entities.ThamDinhGia;
using Microsoft.EntityFrameworkCore;
using Services.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.ThamDinhGia
{
    public class ThamDinhGiaXetDuyetService(ApplicationDbContext dbContext) : IThamDinhGiaXetDuyetService
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        public async Task<CommonResponse> GetListXetDuyetByFilterAsync(int year, Guid donViId, bool isSSA, string search, int pageSize, int pageCurrent)
        {
            try
            {
                var query = _dbContext.ThamDinhGias.AsQueryable();

                // Trạng thái hồ sơ xem được: CD, DD, CB, HCB
                var allowedStatuses = new List<string> { "CD", "DD", "CB", "HCB" };
                query = query.Where(x => allowedStatuses.Contains(x.TrangThai ?? ""));

                // Lọc theo năm
                if (year > 0)
                {
                    query = query.Where(x => x.Thoidiem.Year == year);
                }

                // Lọc theo đơn vị (nếu không phải SSA)
                if (!isSSA)
                {
                    query = query.Where(x => x.DonViChuQuanId == donViId);
                }

                // Tìm kiếm
                if (!string.IsNullOrEmpty(search))
                {
                    search = search.ToLower();
                    query = query.Where(x =>
                        (x.SoTbKl != null && x.SoTbKl.ToLower().Contains(search)) ||
                        (x.GhiChu != null && x.GhiChu.ToLower().Contains(search))
                    );
                }

                query = query.OrderByDescending(x => x.Thoidiem);

                var totalRecord = await query.CountAsync();
                var dataView = await query.Skip((pageCurrent - 1) * pageSize).Take(pageSize).ToListAsync();

                return new CommonResponse
                {
                    Status = "success",
                    Message = "Lấy dữ liệu thành công",
                    Data = dataView,
                    TotalRecord = totalRecord
                };
            }
            catch (Exception ex)
            {
                return new CommonResponse("error", "Lỗi lấy danh sách xét duyệt: " + ex.Message);
            }
        }

        public async Task<CommonResponse> DuyetAsync(Guid id)
        {
            try
            {
                var model = await _dbContext.ThamDinhGias.FirstOrDefaultAsync(t => t.Id == id);
                if (model == null) return new CommonResponse("error", "Không tìm thấy hồ sơ");

                model.TrangThai = "DD"; // Đã duyệt
                model.UpdatedDate = DateTime.Now;
                _dbContext.ThamDinhGias.Update(model);
                await _dbContext.SaveChangesAsync();

                return new CommonResponse("success", "Duyệt hồ sơ thành công");
            }
            catch (Exception ex)
            {
                return new CommonResponse("error", "Lỗi duyệt hồ sơ: " + ex.Message);
            }
        }

        public async Task<CommonResponse> HuyDuyetAsync(Guid id)
        {
            try
            {
                var model = await _dbContext.ThamDinhGias.FirstOrDefaultAsync(t => t.Id == id);
                if (model == null) return new CommonResponse("error", "Không tìm thấy hồ sơ");

                model.TrangThai = "CD"; // Chờ duyệt (hoặc Chờ chuyển)
                model.UpdatedDate = DateTime.Now;
                _dbContext.ThamDinhGias.Update(model);
                await _dbContext.SaveChangesAsync();

                return new CommonResponse("success", "Hủy duyệt hồ sơ thành công");
            }
            catch (Exception ex)
            {
                return new CommonResponse("error", "Lỗi hủy duyệt hồ sơ: " + ex.Message);
            }
        }

        public async Task<CommonResponse> TraLaiAsync(Guid id, string lyDo)
        {
            try
            {
                var model = await _dbContext.ThamDinhGias.FirstOrDefaultAsync(t => t.Id == id);
                if (model == null) return new CommonResponse("error", "Không tìm thấy hồ sơ");

                model.TrangThai = "BTL"; // Bị trả lại
                model.LyDo = lyDo;
                model.UpdatedDate = DateTime.Now;
                _dbContext.ThamDinhGias.Update(model);
                await _dbContext.SaveChangesAsync();

                return new CommonResponse("success", "Trả lại hồ sơ thành công");
            }
            catch (Exception ex)
            {
                return new CommonResponse("error", "Lỗi trả lại hồ sơ: " + ex.Message);
            }
        }

        public async Task<CommonResponse> CongBoAsync(Guid id)
        {
            try
            {
                var model = await _dbContext.ThamDinhGias.FirstOrDefaultAsync(t => t.Id == id);
                if (model == null) return new CommonResponse("error", "Không tìm thấy hồ sơ");

                model.TrangThai = "CB"; // Công bố
                model.UpdatedDate = DateTime.Now;
                _dbContext.ThamDinhGias.Update(model);
                await _dbContext.SaveChangesAsync();

                return new CommonResponse("success", "Công bố hồ sơ thành công");
            }
            catch (Exception ex)
            {
                return new CommonResponse("error", "Lỗi công bố hồ sơ: " + ex.Message);
            }
        }

        public async Task<CommonResponse> HuyCongBoAsync(Guid id)
        {
            try
            {
                var model = await _dbContext.ThamDinhGias.FirstOrDefaultAsync(t => t.Id == id);
                if (model == null) return new CommonResponse("error", "Không tìm thấy hồ sơ");

                model.TrangThai = "HCB"; // Hủy công bố (hoặc quay lại Đã duyệt)
                model.UpdatedDate = DateTime.Now;
                _dbContext.ThamDinhGias.Update(model);
                await _dbContext.SaveChangesAsync();

                return new CommonResponse("success", "Hủy công bố hồ sơ thành công");
            }
            catch (Exception ex)
            {
                return new CommonResponse("error", "Lỗi hủy công bố hồ sơ: " + ex.Message);
            }
        }
    }
}
