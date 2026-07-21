using DataAccess;
using DataAccess.Entities.Settings;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Services.Hubs;
using Services.Model;
using System;

namespace Services.Settings
{
    public interface IDanhMucDiaDanhService
    {
        Task<CommonResponse> GetDanhMucDiaDanhsAsync(string Search, int PageSize = 5, int PageCurrent = 1);

        Task<List<DanhMucDiaDanh>> GetDanhMucDiaDanhsByIdAsync(Guid id);

        Task<List<DanhMucDiaDanh>> GetListByParentAsync(Guid danhMucId);

        Task<CommonResponse> StoreAsync(DanhMucDiaDanh request);

        Task<CommonResponse> EditAsync(Guid id);

        Task<CommonResponse> UpdateAsync(DanhMucDiaDanh request);

        Task<CommonResponse> DeleteAsync(Guid id);

        Task<int> GetSTTSapXep(Guid guid);
    }

    public class DanhMucDiaDanhService(ApplicationDbContext dbContext) : IDanhMucDiaDanhService
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        public async Task<CommonResponse> GetDanhMucDiaDanhsAsync(string search, int pageSize = 5, int pageCurrent = 1)
        {
            try
            {
                List<DanhMucDiaDanh> data = [];

                var query = _dbContext.DanhMucDiaDanhs.AsQueryable();

                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(dm => dm.TenDiaDanh.Contains(search));
                }

                var listDanhMuc = await query.OrderBy(dm => dm.Level).ThenBy(dm => dm.STTSapXep).ToListAsync();

                if (listDanhMuc.Any())
                {
                    HashSet<Guid> addedIds = [];
                    var allDanhMuc = await _dbContext.DanhMucDiaDanhs.ToListAsync();
                    var listDanhMucId = listDanhMuc.Select(dm => dm.Id).ToList();

                    foreach (var item in listDanhMuc)
                    {
                        if (addedIds.Add(item.Id))
                        {
                            data.Add(MapDanhMucDiaDanh(item, allDanhMuc));
                            GetAllChildren(item.Id, allDanhMuc, data, addedIds);
                        }
                    }
                }
                var dataView = data.Skip((pageCurrent - 1) * pageSize).Take(pageSize).ToList();
                return new CommonResponse { Status = "success", Data = dataView, TotalRecord = data.Count };
            }
            catch
            {
                return new CommonResponse();
            }
        }

        public async Task<List<DanhMucDiaDanh>> GetDanhMucDiaDanhsByIdAsync(Guid id)
        {
            List<DanhMucDiaDanh> data = [];
            HashSet<Guid> addedIds = [];

            var model = await _dbContext.DanhMucDiaDanhs.FindAsync(id);

            if (model == null) return data;

            var allDanhMuc = await _dbContext.DanhMucDiaDanhs.ToListAsync();

            if (addedIds.Add(model.Id))
            {
                data.Add(MapDanhMucDiaDanh(model, allDanhMuc));
                GetAllChildren(model.Id, allDanhMuc, data, addedIds);
            }

            return data;
        }

        public async Task<List<DanhMucDiaDanh>> GetListByParentAsync(Guid danhMucId) =>
            await _dbContext.DanhMucDiaDanhs.AsNoTracking().Where(x => x.DiaDanhCapTrenId == danhMucId).ToListAsync();

        public async Task<CommonResponse> StoreAsync(DanhMucDiaDanh request)
        {
            try
            {
                _dbContext.DanhMucDiaDanhs.Add(request);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success", Message = "Thành công" };
            }
            catch
            {
                return new CommonResponse { Status = "error", Message = "Có lỗi xảy ra! Vui lòng thử lại sau!" };
            }
        }

        public async Task<CommonResponse> EditAsync(Guid id)
        {
            try
            {
                var data = await _dbContext.DanhMucDiaDanhs.FindAsync(id);
                if (data == null)
                {
                    return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin!" };
                }
                if (data.DiaDanhCapTrenId != Guid.Empty)
                    data.TenDiaDanhChuQuan = await _dbContext.DanhMucDiaDanhs
                                                                .Where(dm => dm.Id == data.DiaDanhCapTrenId)
                                                                .Select(dm => dm.TenDiaDanh)
                                                                .FirstOrDefaultAsync() ?? "";

                return new CommonResponse { Status = "success", Data = data };
            }
            catch
            {
                return new CommonResponse();
            }

        }

        public async Task<CommonResponse> UpdateAsync(DanhMucDiaDanh request)
        {
            try
            {
                _dbContext.DanhMucDiaDanhs.Update(request);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success", Message = "Thành công" };
            }
            catch
            {
                return new CommonResponse();
            }
        }

        public async Task<CommonResponse> DeleteAsync(Guid id)
        {
            try
            {
                var listDanhMuc = await _dbContext.DanhMucDiaDanhs
                                                    .AsNoTracking()
                                                    .ToListAsync();

                List<DanhMucDiaDanh> dataToDelete = [];
                HashSet<Guid> addedIds = [];

                GetAllChildren(id, listDanhMuc, dataToDelete, addedIds);

                var parent = await _dbContext.DanhMucDiaDanhs
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (parent != null && addedIds.Add(parent.Id))
                {
                    dataToDelete.Add(parent);
                }

                var trackedEntities = await _dbContext.DanhMucDiaDanhs
                    .Where(x => addedIds.Contains(x.Id))
                    .ToListAsync();

                _dbContext.DanhMucDiaDanhs.RemoveRange(trackedEntities);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success", Message = "Thành công" };
            }
            catch
            {
                return new CommonResponse();
            }
        }

        private async Task<DanhMucDiaDanh> GetDiaDanhById(Guid id)
        {
            return await _dbContext.DanhMucDiaDanhs.FindAsync(id) ?? throw new Exception("Không tìm thấy thông tin địa danh!");
        }

        private static DanhMucDiaDanh MapDanhMucDiaDanh(DanhMucDiaDanh item, List<DanhMucDiaDanh> listDanhMuc)
        {
            var tenDiaDanhDict = listDanhMuc.ToDictionary(d => d.Id, d => d.TenDiaDanh);

            return new DanhMucDiaDanh
            {
                Id = item.Id,
                TenDiaDanh = item.TenDiaDanh,
                Level = item.Level,
                STTSapXep = item.STTSapXep,
                DiaDanhCapTrenId = item.DiaDanhCapTrenId,
                TenDiaDanhChuQuan = tenDiaDanhDict.GetValueOrDefault(item.DiaDanhCapTrenId, ""),
            };
        }

        private static void GetAllChildren(Guid parentId, List<DanhMucDiaDanh> listDanhMuc, List<DanhMucDiaDanh> data, HashSet<Guid> addedIds)
        {
            var children = listDanhMuc.Where(t => t.DiaDanhCapTrenId == parentId).OrderBy(t => t.STTSapXep).ToList();

            foreach (var item in children)
            {
                if (addedIds.Add(item.Id))
                {
                    data.Add(MapDanhMucDiaDanh(item, listDanhMuc));
                    GetAllChildren(item.Id, listDanhMuc, data, addedIds);
                }
            }
        }

        private static void GetAllParents(Guid? parentId, List<DanhMucDiaDanh> listDanhMuc, List<DanhMucDiaDanh> data, HashSet<Guid> addedIds)
        {
            while (parentId.HasValue)
            {
                var parent = listDanhMuc.FirstOrDefault(t => t.Id == parentId.Value);
                if (parent == null || !addedIds.Add(parent.Id)) break;

                data.Add(MapDanhMucDiaDanh(parent, listDanhMuc));
                parentId = parent.DiaDanhCapTrenId;
            }
        }

        public async Task<int> GetSTTSapXep(Guid guid)
        {
            bool exits = _dbContext.DanhMucDiaDanhs.Any(x => x.DiaDanhCapTrenId == guid);
            if (!exits)
            {
                return 1;
            }
            int count = await _dbContext.DanhMucDiaDanhs.CountAsync(t => t.DiaDanhCapTrenId == guid);
            return count + 1;
        }
    }
}
