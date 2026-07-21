using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DataAccess;
using DataAccess.Entities.Settings;
using DataAccess.Entities.Settings.DanhMucGia;
using DataAccess.Entities.DinhGiaHHDV;
using DataAccess.Entities.KeKhaiDangKyGia;
using DataAccess.Entities.ThamDinhGia;

namespace Services.TraCuuBaoCao.TraCuu
{
    public class TraCuuService : ITraCuuService
    {
        private readonly ApplicationDbContext _dbContext;

        public TraCuuService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<DanhMucKinhDoanh>> GetDanhMucKinhDoanhNganhAsync(string loaiGia)
        {
            return await _dbContext.DanhMucKinhDoanhs
                .Where(t => (t.Level == 0 || t.PhanLoai == "Group") && t.LoaiGia == loaiGia && t.TheoDoi == "TD")
                .OrderBy(t => t.STTSapXep)
                .ToListAsync();
        }

        public async Task<List<DanhMucKinhDoanh>> GetDanhMucKinhDoanhNgheAsync(string loaiGia)
        {
            return await _dbContext.DanhMucKinhDoanhs
                .Where(t => (t.Level > 0 || t.PhanLoai == "Detail") && t.LoaiGia == loaiGia && t.TheoDoi == "TD")
                .OrderBy(t => t.STTSapXep)
                .ToListAsync();
        }

        public async Task<List<GiaThiTruongDanhMuc>> GetGiaThiTruongDanhMucAsync()
        {
            return await _dbContext.GiaThiTruongDanhMucs
                .Where(t => t.TheoDoi == "TD")
                .ToListAsync();
        }

        private Type? GetDetailType(string maNghe)
        {
            var type = Type.GetType($"DataAccess.Entities.DinhGiaHHDV.ChiTiet.ChiTiet{maNghe}, DataAccess")
                       ?? Type.GetType($"DataAccess.Entities.DinhGiaHHDV.ChiTiet{maNghe}, DataAccess");
            if (type == null)
            {
                return typeof(DataAccess.Entities.DinhGiaHHDV.ChiTiet.ChiTietGiaChung);
            }
            return type;
        }

        private IQueryable GetDbSet(Type entityType)
        {
            var method = typeof(DbContext).GetMethods()
                .First(m => m.Name == "Set" && m.GetParameters().Length == 0)
                .MakeGenericMethod(entityType);
            return (IQueryable)method.Invoke(_dbContext, null)!;
        }

        public async Task<Tuple<List<object>, Dictionary<string, DinhGia>>> SearchDinhGiaCtAsync(string maNghe, DateTime? tuNgay, DateTime? denNgay, string soQd, string moTa, string maHoSo = null)
        {
            var query = _dbContext.DinhGias.Include(x => x.DonViQuanLy).Where(x => x.MaNghe == maNghe);

            if (!string.IsNullOrEmpty(maHoSo))
            {
                query = query.Where(x => x.MaHoSo == maHoSo);
            }

            if (tuNgay.HasValue)
            {
                query = query.Where(x => x.ThoiDiem >= tuNgay.Value);
            }

            if (denNgay.HasValue)
            {
                query = query.Where(x => x.ThoiDiem <= denNgay.Value);
            }

            if (!string.IsNullOrEmpty(soQd))
            {
                query = query.Where(x => x.SoQd != null && x.SoQd.Contains(soQd));
            }

            if (!string.IsNullOrEmpty(moTa))
            {
                query = query.Where(x => x.MoTa != null && x.MoTa.Contains(moTa));
            }

            var parents = await query.ToListAsync();
            var parentMap = parents.GroupBy(x => x.MaHoSo).ToDictionary(g => g.Key!, g => g.First());
            var maHoSos = parents.Select(x => x.MaHoSo).ToList();

            var listDetails = new List<object>();
            if (maHoSos.Any())
            {
                var detailType = GetDetailType(maNghe);
                if (detailType != null)
                {
                    var dbSet = GetDbSet(detailType);
                    var parameter = Expression.Parameter(detailType, "t");
                    var property = Expression.Property(parameter, "MaHoSo");
                    var containsMethod = typeof(List<string>).GetMethod("Contains", new[] { typeof(string) });
                    var containsExpression = Expression.Call(Expression.Constant(maHoSos), containsMethod!, property);
                    var lambda = Expression.Lambda(containsExpression, parameter);

                    var whereMethod = typeof(Queryable).GetMethods()
                        .First(m => m.Name == "Where" && m.GetParameters().Length == 2)
                        .MakeGenericMethod(detailType);

                    var detailQuery = whereMethod.Invoke(null, new object[] { dbSet, lambda }) as IQueryable;
                    if (detailQuery != null)
                    {
                        foreach (var d in detailQuery)
                        {
                            listDetails.Add(d);
                        }
                    }
                }
            }

            return Tuple.Create(listDetails, parentMap);
        }

        public async Task<Tuple<List<KeKhaiDangKyGiaCt>, Dictionary<string, DataAccess.Entities.KeKhaiDangKyGia.KeKhaiDangKyGia>>> SearchKeKhaiDangKyGiaCtAsync(string maNghe, DateTime? tuNgay, DateTime? denNgay, string soQd, string moTa, string maHoSo = null)
        {
            var query = _dbContext.KeKhaiDangKyGias.Include(x => x.DoanhNghiepQuanLy).AsQueryable();

            if (!string.IsNullOrEmpty(maNghe))
            {
                query = query.Where(x => x.MaNghe == maNghe);
            }

            if (!string.IsNullOrEmpty(maHoSo))
            {
                query = query.Where(x => x.MaHoSo == maHoSo);
            }

            if (tuNgay.HasValue)
            {
                query = query.Where(x => x.ThoiDiem >= tuNgay.Value);
            }

            if (denNgay.HasValue)
            {
                query = query.Where(x => x.ThoiDiem <= denNgay.Value);
            }

            if (!string.IsNullOrEmpty(soQd))
            {
                query = query.Where(x => x.SoQd != null && x.SoQd.Contains(soQd));
            }

            if (!string.IsNullOrEmpty(moTa))
            {
                query = query.Where(x => x.GhiChu != null && x.GhiChu.Contains(moTa));
            }

            var parents = await query.ToListAsync();
            var parentMap = parents.GroupBy(x => x.MaHoSo).ToDictionary(g => g.Key!, g => g.First());
            var maHoSos = parents.Select(x => x.MaHoSo).ToList();

            var listDetails = new List<KeKhaiDangKyGiaCt>();
            if (maHoSos.Any())
            {
                listDetails = await _dbContext.KeKhaiDangKyGiaCts
                    .Include(x => x.DoanhNghiepQuanLy)
                    .Where(x => maHoSos.Contains(x.MaHoSo))
                    .ToListAsync();
            }

            return Tuple.Create(listDetails, parentMap);
        }

        public async Task<Tuple<List<GiaThiTruongCt>, Dictionary<string, GiaThiTruong>>> SearchGiaThiTruongCtAsync(Guid thongTuId, DateTime? tuNgay, DateTime? denNgay, string soQd, string moTa, string maHoSo = null)
        {
            var query = _dbContext.GiaThiTruongs.AsQueryable();

            if (thongTuId != Guid.Empty)
            {
                query = query.Where(x => x.ThongTuId == thongTuId);
            }

            if (!string.IsNullOrEmpty(maHoSo))
            {
                query = query.Where(x => x.MaHoSo == maHoSo);
            }

            if (tuNgay.HasValue)
            {
                query = query.Where(x => x.Thoidiem >= tuNgay.Value);
            }

            if (denNgay.HasValue)
            {
                query = query.Where(x => x.Thoidiem <= denNgay.Value);
            }

            if (!string.IsNullOrEmpty(soQd))
            {
                query = query.Where(x => x.SoQd != null && x.SoQd.Contains(soQd));
            }

            if (!string.IsNullOrEmpty(moTa))
            {
                query = query.Where(x => x.GhiChu != null && x.GhiChu.Contains(moTa));
            }

            var parents = await query.ToListAsync();
            var parentMap = parents.GroupBy(x => x.MaHoSo).ToDictionary(g => g.Key!, g => g.First());
            var maHoSos = parents.Select(x => x.MaHoSo).ToList();

            var listDetails = new List<GiaThiTruongCt>();
            if (maHoSos.Any())
            {
                listDetails = await _dbContext.GiaThiTruongCts
                    .Where(x => maHoSos.Contains(x.MaHoSo))
                    .ToListAsync();
            }

            return Tuple.Create(listDetails, parentMap);
        }

        public async Task<List<ThamDinhGiaDanhMucHangHoa>> GetThamDinhGiaDanhMucHangHoaAsync()
        {
            return await _dbContext.ThamDinhGiaDanhMucHangHoas
                .Where(x => x.TrangThai == "Kích hoạt")
                .ToListAsync();
        }

        public async Task<Tuple<List<ThamDinhGiaCt>, Dictionary<Guid, DataAccess.Entities.ThamDinhGia.ThamDinhGia>>> SearchThamDinhGiaCtAsync(
            Guid hangHoaId,
            DateTime? tuNgay,
            DateTime? denNgay,
            string soTbKl,
            string dvYeuCau,
            string maHoSo = null)
        {
            var query = _dbContext.ThamDinhGias.AsQueryable();

            if (!string.IsNullOrEmpty(maHoSo) && Guid.TryParse(maHoSo, out Guid hoSoId))
            {
                query = query.Where(x => x.Id == hoSoId);
            }

            if (tuNgay.HasValue)
            {
                query = query.Where(x => x.Thoidiem >= tuNgay.Value);
            }

            if (denNgay.HasValue)
            {
                query = query.Where(x => x.Thoidiem <= denNgay.Value);
            }

            if (!string.IsNullOrEmpty(soTbKl))
            {
                query = query.Where(x => x.SoTbKl != null && x.SoTbKl.Contains(soTbKl));
            }

            if (!string.IsNullOrEmpty(dvYeuCau))
            {
                query = query.Where(x => x.DvYeuCau != null && x.DvYeuCau.Contains(dvYeuCau));
            }

            var parents = await query.ToListAsync();
            var parentMap = parents.GroupBy(x => x.Id).ToDictionary(g => g.Key, g => g.First());
            var ids = parents.Select(x => x.Id).ToList();

            var listDetails = new List<ThamDinhGiaCt>();
            if (ids.Any())
            {
                var detailQuery = _dbContext.ThamDinhGiaCts.Where(x => ids.Contains(x.MaHoSo));
                if (hangHoaId != Guid.Empty)
                {
                    detailQuery = detailQuery.Where(x => x.HangHoaId == hangHoaId);
                }
                listDetails = await detailQuery.ToListAsync();
            }

            return Tuple.Create(listDetails, parentMap);
        }
    }
}
