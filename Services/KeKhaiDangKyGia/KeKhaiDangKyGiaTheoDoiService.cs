using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DataAccess;
using DataAccess.Entities.KeKhaiDangKyGia;
using Services.Model;
using Services.DTOs.KeKhaiDangKyGia;

namespace Services.KeKhaiDangKyGia
{
    public class KeKhaiDangKyGiaTheoDoiService : IKeKhaiDangKyGiaTheoDoiService
    {
        private readonly ApplicationDbContext _dbContext;

        public KeKhaiDangKyGiaTheoDoiService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<CommonResponse> GetListTheoDoiByFilterAsync(KeKhaiDangKyGiaFilter filter)
        {
            try
            {
                var allowedStatuses = new List<string> { "CD", "DD", "CB", "HCB" };
                IQueryable<DataAccess.Entities.KeKhaiDangKyGia.KeKhaiDangKyGia> query = _dbContext.KeKhaiDangKyGias
                    .Include(x => x.DoanhNghiepQuanLy)
                    .AsNoTracking();

                // Filter by year based on NgayChuyen (ngày gửi hồ sơ)
                if (filter.TargetYear > 0)
                {
                    query = query.Where(x => x.NgayChuyen.Year == filter.TargetYear);
                }

                // Filter by DonViQuanLyId
                if (filter.DonViQuanLyId != Guid.Empty)
                {
                    query = query.Where(x => x.DonViQuanLyId == filter.DonViQuanLyId);
                }

                // Filter by MaNghe
                if (!string.IsNullOrEmpty(filter.MaNghe) && filter.MaNghe != "all")
                {
                    query = query.Where(x => x.MaNghe == filter.MaNghe);
                }

                // Filter by TrangThai (must be within CD, DD, CB, HCB)
                if (!string.IsNullOrEmpty(filter.TrangThai) && filter.TrangThai != "all" && allowedStatuses.Contains(filter.TrangThai))
                {
                    query = query.Where(x => x.TrangThai == filter.TrangThai);
                }
                else
                {
                    query = query.Where(x => allowedStatuses.Contains(x.TrangThai ?? ""));
                }

                // Filter by Search text (SoQd, LyDo, GhiChu)
                if (!string.IsNullOrEmpty(filter.Search))
                {
                    string searchLower = filter.Search.ToLower().Trim();
                    query = query.Where(x => 
                        (x.SoQd != null && x.SoQd.ToLower().Contains(searchLower)) ||
                        (x.LyDo != null && x.LyDo.ToLower().Contains(searchLower)) ||
                        (x.GhiChu != null && x.GhiChu.ToLower().Contains(searchLower))
                    );
                }

                int totalRecord = await query.CountAsync();
                
                // Adjust paging
                filter.AdjustPageIfInvalid(totalRecord);

                var dataList = await query
                    .OrderByDescending(x => x.NgayChuyen)
                    .Skip((filter.PageCurrent - 1) * filter.PageSize)
                    .Take(filter.PageSize)
                    .ToListAsync();

                return new CommonResponse("success", "Lấy danh sách thành công", dataList, totalRecord);
            }
            catch (Exception ex)
            {
                return new CommonResponse("error", "Lỗi lấy danh sách hồ sơ theo dõi: " + ex.Message);
            }
        }
    }
}
