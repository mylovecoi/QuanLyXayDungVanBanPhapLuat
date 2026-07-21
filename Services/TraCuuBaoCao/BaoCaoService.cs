using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess;
using DataAccess.Entities.DinhGiaHHDV;
using DataAccess.Entities.ThamDinhGia;
using Microsoft.EntityFrameworkCore;

namespace Services.TraCuuBaoCao
{
    public class BaoCaoService : IBaoCaoService
    {
        private readonly ApplicationDbContext _dbContext;

        public BaoCaoService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<DinhGia>> SearchDinhGiaReportAsync(string maNghe, DateTime? tuNgay, DateTime? denNgay)
        {
            var query = _dbContext.DinhGias.Include(x => x.DonViQuanLy).Where(x => x.MaNghe == maNghe);

            if (tuNgay.HasValue)
            {
                query = query.Where(x => x.ThoiDiem >= tuNgay.Value);
            }

            if (denNgay.HasValue)
            {
                query = query.Where(x => x.ThoiDiem <= denNgay.Value);
            }

            return await query.OrderByDescending(x => x.ThoiDiem).ToListAsync();
        }

        public async Task<List<DataAccess.Entities.KeKhaiDangKyGia.KeKhaiDangKyGia>> SearchKeKhaiDangKyGiaReportAsync(string maNghe, DateTime? tuNgay, DateTime? denNgay)
        {
            var query = _dbContext.KeKhaiDangKyGias.Include(x => x.DoanhNghiepQuanLy).AsQueryable();

            if (!string.IsNullOrEmpty(maNghe))
            {
                query = query.Where(x => x.MaNghe == maNghe);
            }

            if (tuNgay.HasValue)
            {
                query = query.Where(x => x.ThoiDiem >= tuNgay.Value);
            }

            if (denNgay.HasValue)
            {
                query = query.Where(x => x.ThoiDiem <= denNgay.Value);
            }

            return await query.OrderByDescending(x => x.ThoiDiem).ToListAsync();
        }

        public async Task<List<GiaThiTruong>> SearchGiaThiTruongReportAsync(Guid thongTuId, DateTime? tuNgay, DateTime? denNgay)
        {
            var query = _dbContext.GiaThiTruongs.AsQueryable();

            if (thongTuId != Guid.Empty)
            {
                query = query.Where(x => x.ThongTuId == thongTuId);
            }

            if (tuNgay.HasValue)
            {
                query = query.Where(x => x.Thoidiem >= tuNgay.Value);
            }

            if (denNgay.HasValue)
            {
                query = query.Where(x => x.Thoidiem <= denNgay.Value);
            }

            return await query.OrderByDescending(x => x.Thoidiem).ToListAsync();
        }

        public async Task<List<DataAccess.Entities.ThamDinhGia.ThamDinhGia>> SearchThamDinhGiaReportAsync(DateTime? tuNgay, DateTime? denNgay)
        {
            var query = _dbContext.ThamDinhGias.AsQueryable();

            if (tuNgay.HasValue)
            {
                query = query.Where(x => x.Thoidiem >= tuNgay.Value);
            }

            if (denNgay.HasValue)
            {
                query = query.Where(x => x.Thoidiem <= denNgay.Value);
            }

            return await query.OrderByDescending(x => x.Thoidiem).ToListAsync();
        }
    }
}
