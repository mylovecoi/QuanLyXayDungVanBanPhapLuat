using DataAccess;
using DataAccess.Entities.KeKhaiDangKyGia;
using DataAccess.Entities.Settings;
using DataAccess.Entities.Settings.DanhMucGia;
using Microsoft.EntityFrameworkCore;
using Services.DTOs.KeKhaiDangKyGia;
using Services.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.KeKhaiDangKyGia
{
    public class KeKhaiDangKyGiaXetDuyetService(ApplicationDbContext dbContext) : IKeKhaiDangKyGiaXetDuyetService
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        public async Task<CommonResponse> GetListXetDuyetByFilterAsync(KeKhaiDangKyGiaFilter filter)
        {
            try
            {
                IQueryable<DataAccess.Entities.KeKhaiDangKyGia.KeKhaiDangKyGia> query = _dbContext.KeKhaiDangKyGias
                    .Include(x => x.DoanhNghiepQuanLy)
                    .AsNoTracking();

                if (filter.TargetYear > 0)
                {
                    query = query.Where(x => x.NgayChuyen.Year == filter.TargetYear);
                }

                if (filter.DonViQuanLyId != Guid.Empty)
                {
                    string donViIdStr = filter.DonViQuanLyId.ToString();
                    if (filter.TrangThai == "CD")
                    {
                        var dmKinhDoanh = await GetDanhMucKinhDoanhByFilterAsync(filter);
                        var listMaNghe = dmKinhDoanh.Select(x => x.MaNghe).ToList();

                        query = query.Where(x => x.DonViQuanLyId == filter.DonViQuanLyId && listMaNghe.Contains(x.MaNghe));
                    }
                    else
                    {
                        query = query.Where(x => x.DonViQuanLyId == filter.DonViQuanLyId || (x.DonViDongChuyenId != null && x.DonViDongChuyenId.Contains(donViIdStr)));
                    }
                }

                if (!string.IsNullOrEmpty(filter.TrangThai))
                {
                    query = query.Where(x => x.TrangThai == filter.TrangThai);
                }
                else
                {
                    query = query.Where(x => x.TrangThai != "CXD" && x.TrangThai != "CC");
                }

                if (!string.IsNullOrEmpty(filter.MaNghe) && filter.MaNghe != "all")
                {
                    query = query.Where(x => x.MaNghe == filter.MaNghe);
                }

                if (!string.IsNullOrEmpty(filter.Search))
                {
                    var searchLower = filter.Search.ToLower().Trim();
                    query = query.Where(x =>
                        (x.SoQd != null && x.SoQd.ToLower().Contains(searchLower)) ||
                        (x.GhiChu != null && x.GhiChu.ToLower().Contains(searchLower)) ||
                        (x.DonViTinh != null && x.DonViTinh.ToLower().Contains(searchLower))
                    );
                }

                int totalRecord = await query.CountAsync();
                filter.AdjustPageIfInvalid(totalRecord);

                var dataList = await query
                    .OrderByDescending(x => x.NgayChuyen)
                    .Skip((filter.PageCurrent - 1) * filter.PageSize)
                    .Take(filter.PageSize)
                    .ToListAsync();

                return new CommonResponse("success", "Lấy dữ liệu thành công", dataList, totalRecord);
            }
            catch (Exception ex)
            {
                return new CommonResponse("error", "Lỗi lấy dữ liệu: " + ex.Message);
            }
        }

        public async Task<List<DanhMucKinhDoanh>> GetDanhMucKinhDoanhByFilterAsync(KeKhaiDangKyGiaFilter filter)
        {
            var query = _dbContext.DanhMucKinhDoanhs.AsNoTracking()
                .Where(x => x.PhanLoai == "Detail" && x.LoaiGia == "KKG");

            if (filter.DonViQuanLyId != Guid.Empty)
            {
                string donViIdStr = filter.DonViQuanLyId.ToString();
                if (filter.TrangThai == "CD")
                {
                    query = query.Where(x => x.DonViQuanLyId != null && x.DonViQuanLyId.Contains(donViIdStr));
                }
                else
                {
                    query = query.Where(x => 
                        (x.DonViQuanLyId != null && x.DonViQuanLyId.Contains(donViIdStr)) || 
                        (x.DonViDongChuyenId != null && x.DonViDongChuyenId.Contains(donViIdStr))
                    );
                }
            }

            return await query.OrderBy(x => x.STTSapXep).ToListAsync();
        }

        public async Task<CommonResponse> GetSingleByIdAsync(Guid id)
        {
            try
            {
                var model = await _dbContext.KeKhaiDangKyGias.FirstOrDefaultAsync(x => x.Id == id);
                if (model == null) return new CommonResponse("error", "Không tìm thấy hồ sơ");
                return new CommonResponse("success", "Lấy dữ liệu thành công", model);
            }
            catch (Exception ex)
            {
                return new CommonResponse("error", "Lỗi lấy dữ liệu: " + ex.Message);
            }
        }

        public async Task<CommonResponse> DuyetAsync(Guid id, string soHsDuyet)
        {
            try
            {
                var model = await _dbContext.KeKhaiDangKyGias.FirstOrDefaultAsync(t => t.Id == id);
                if (model == null) return new CommonResponse("error", "Không tìm thấy hồ sơ");

                model.TrangThai = "DD"; // Đã duyệt
                model.SoHsDuyet = soHsDuyet;
                model.NgayDuyet = DateTime.Now;

                _dbContext.KeKhaiDangKyGias.Update(model);
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
                var model = await _dbContext.KeKhaiDangKyGias.FirstOrDefaultAsync(t => t.Id == id);
                if (model == null) return new CommonResponse("error", "Không tìm thấy hồ sơ");

                model.TrangThai = "CD"; // Trở về Chờ duyệt
                model.SoHsDuyet = null;
                model.NgayDuyet = DateTime.MinValue;

                _dbContext.KeKhaiDangKyGias.Update(model);
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
                var model = await _dbContext.KeKhaiDangKyGias.FirstOrDefaultAsync(t => t.Id == id);
                if (model == null) return new CommonResponse("error", "Không tìm thấy hồ sơ");

                model.TrangThai = "BTL"; // Bị trả lại
                model.LyDo = lyDo;
                model.NgayTraHoSo = DateTime.Now;

                _dbContext.KeKhaiDangKyGias.Update(model);
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
                var model = await _dbContext.KeKhaiDangKyGias.FirstOrDefaultAsync(t => t.Id == id);
                if (model == null) return new CommonResponse("error", "Không tìm thấy hồ sơ");

                model.TrangThai = "CB"; // Công bố

                _dbContext.KeKhaiDangKyGias.Update(model);
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
                var model = await _dbContext.KeKhaiDangKyGias.FirstOrDefaultAsync(t => t.Id == id);
                if (model == null) return new CommonResponse("error", "Không tìm thấy hồ sơ");

                model.TrangThai = "DD"; // Hủy công bố chuyển về Đã duyệt

                _dbContext.KeKhaiDangKyGias.Update(model);
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
