using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure;
using DataAccess;
using DataAccess.Entities.Settings;
using Microsoft.EntityFrameworkCore;
using Services.DTOs.Settings.DanhMucDungChung;
using Services.Helpers;
using Services.Model;
using Services.Systems;

namespace Services.Settings.DanhMucDungChung.DmHopDong
{
    public interface IDmHopDongService
    {
        Task<CommonResponse> GetListByFilterAsync(DmHopDongFilter filter);
        Task<CommonResponse> GetSingleByIdAsync(Guid hopDongId);
        Task<CommonResponse> GetSingleByIdWithParentAsync(Guid hopDongId, Guid parentId = default(Guid));
        Task<DanhMucHopDong?> GetEntityByMaAsync(string maDanhMuc);
        Task<DanhMucHopDong?> GetEntityByIdAsync(Guid hopDongId, bool isAsNoTracking = true);
        Task<List<DanhMucHopDong>> GetEntityByIdsAsync(List<Guid> hopDongIds, bool? loaiNghiepVu);
        Task<CommonResponse> ValidateRequestAsync(DanhMucHopDong request);
        Task<CommonResponse> StoreAsync(DanhMucHopDong request);
        Task<CommonResponse> UpdateAsync(DanhMucHopDong request);
        Task<CommonResponse> DeleteAsync(Guid hopDongId);
    }

    public class DmHopDongService(ApplicationDbContext dbContext, IOptionDataService optionDataService) : IDmHopDongService
    {
        private readonly ApplicationDbContext _dbContext = dbContext;
        private readonly IOptionDataService _optionDataService = optionDataService;

        public async Task<CommonResponse> GetListByFilterAsync(DmHopDongFilter filter)
        {
            try
            {
                IQueryable<DanhMucHopDong> queryable = _dbContext.DanhMucHopDongs.AsNoTracking();

                if (!string.IsNullOrEmpty(filter.Search)) queryable = queryable.Where(x =>
                    (x.TenHopDong ?? string.Empty).Contains(filter.Search) ||
                    (x.MaHopDong ?? string.Empty).Contains(filter.Search));

                if (filter.LoaiNghiepVu.HasValue)
                    queryable = queryable.Where(x => x.IsCC == filter.LoaiNghiepVu.Value);

                var allData = await queryable.ToListAsync();

                var allIds = allData.Select(x => x.Id).ToHashSet();
                var rootNodes = allData.Where(x => x.ParentId == null || !allIds.Contains(x.ParentId.Value)).ToList();

                var sortedList = new List<DanhMucHopDong>();
                var visited = new HashSet<Guid>();

                foreach (var root in rootNodes)
                    TreeHelper.AddWithChildren(root, allData, sortedList, visited, x => x.Id, x => x.ParentId, x => x.STTSapXep);

                int totalRecord = sortedList.Count();
                filter.AdjustPageIfInvalid(totalRecord);
                var dataView = sortedList.Skip((filter.PageCurrent - 1) * filter.PageSize).Take(filter.PageSize).ToList();

                var optionLoaiGiayTos = await _dbContext.OptionDatas.AsNoTracking().Where(x => x.Code == "LoaiGiayTo").ToListAsync();
                foreach (var item in dataView)
                {
                    item.DanhSachOption = optionLoaiGiayTos.Where(opt => item.DanhSachOption.Contains(opt.Value!)).Select(x => x.DisplayName!).ToList();
                }

                return new("success", "Lấy thông tin danh mục thành công", dataView, totalRecord);
            }
            catch (Exception)
            {
                return new("error", "Có lỗi trong quá trình lấy dữ liệu. Hãy kiểm tra lại!");
            }
        }

        public async Task<CommonResponse> GetSingleByIdAsync(Guid hopDongId)
        {
            try
            {
                var result = await GetEntityByIdAsync(hopDongId, isAsNoTracking: true);
                if (result == null) return new("error", "Không tìm thấy thông tin danh mục. Hãy kiểm tra lại!");

                return new("success", "Lấy thông tin thành công", result);
            }
            catch (Exception)
            {
                return new("error", "Có lỗi trong quá trình lấy dữ liệu. Hãy kiểm tra lại!");
            }
        }

        public async Task<CommonResponse> GetSingleByIdWithParentAsync(Guid hopDongId, Guid parentId = default(Guid))
        {
            try
            {
                var result = await this.GetEntityByIdAsync(hopDongId, true);
                // tạo mới node root
                if (result == null && parentId == Guid.Empty)
                    return new("success", "Khởi tạo dữ liệu thành công", new DanhMucHopDong() { ParentId = Guid.Empty, STTSapXep = await this.GetSortOrderByParentAsync(null), Level = 0 });

                // tạo node con từ node cha trong db
                if (parentId != Guid.Empty)
                {
                    var parentExisting = await this.GetEntityByIdAsync(parentId, true);
                    if (parentExisting == null) return new("error", "Danh mục nhóm nghiệp vụ không còn khả dụng");

                    return new("success", $"Chuẩn bị tạo mới danh mục cấp dưới thuộc: {parentExisting.TenHopDong}", new DanhMucHopDong()
                    {
                        ParentId = parentExisting.Id,
                        Level = parentExisting.Level + 1,
                        STTSapXep = await this.GetSortOrderByParentAsync(parentId),
                        Parent = new() { TenHopDong = parentExisting.TenHopDong ?? string.Empty }
                    });
                }

                return new("success", "Lấy dữ liệu thành công", result);
            }
            catch (Exception)
            {
                return new("error", "Có lỗi trong quá trình lấy dữ liệu. Hãy kiểm tra lại!");
            }
        }

        private async Task<int> GetSortOrderByParentAsync(Guid? parentId) => await _dbContext.DanhMucHopDongs.AsNoTracking().Where(x => x.ParentId == parentId && x.TrangThai != false).CountAsync() + 1;

        public async Task<DanhMucHopDong?> GetEntityByMaAsync(string maDanhMuc) => await _dbContext.DanhMucHopDongs.AsNoTracking().FirstOrDefaultAsync(x => x.MaHopDong == maDanhMuc);

        public async Task<DanhMucHopDong?> GetEntityByIdAsync(Guid hopDongId, bool isAsNoTracking = true)
        {
            var query = _dbContext.DanhMucHopDongs.AsNoTracking()
                .Include(x => x.Parent)
                .Include(x => x.Children)
                .AsQueryable();

            if (isAsNoTracking)
            {
                query = query
                    .Include(x => x.HopDongChiTiet)
                    .AsNoTracking();
            }

            return await query.FirstOrDefaultAsync(x => x.Id == hopDongId);
        }

        public async Task<List<DanhMucHopDong>> GetEntityByIdsAsync(List<Guid> hopDongIds, bool? loaiNghiepVu)
        {
            IQueryable<DanhMucHopDong> queryable = _dbContext.DanhMucHopDongs
                .Include(x => x.HoSoCCCTs).ThenInclude(x => x.LoaiTaiSan)
                .Include(x => x.HoSoCCCTs).ThenInclude(x => x.DiaBan)
                .Include(x => x.HoSoCCCTs).ThenInclude(x => x.HoSoCCCTChiPhis)
                .Include(x => x.HoSoCCCTs).ThenInclude(x => x.HoSoCCCTChiTiets)
                .Include(x => x.HopDongChiTiet)
                .AsNoTracking();

            if (loaiNghiepVu.HasValue)
                queryable = queryable.Where(x => x.IsCC == loaiNghiepVu);

            var allData = await queryable.ToListAsync();

            var selectedData = new List<DanhMucHopDong>();

            if (hopDongIds.Count() > 0)
            {
                var selectedSerchIds = hopDongIds.ToHashSet(); // tối ưu tra cứu
                var tempList = allData.Where(x => selectedSerchIds.Contains(x.Id)).ToList();
                var lookup = allData.ToDictionary(x => x.Id);             // để tra nhanh
                var resultSet = new Dictionary<Guid, DanhMucHopDong>();   // kết quả

                foreach (var item in tempList)
                {
                    // Nếu là cha
                    if (item.ParentId == null)
                    {
                        if (!resultSet.ContainsKey(item.Id))
                            resultSet[item.Id] = item;

                        // thêm tất cả con của cha
                        foreach (var child in allData.Where(x => x.ParentId == item.Id))
                        {
                            if (!resultSet.ContainsKey(child.Id))
                                resultSet[child.Id] = child;
                        }
                    }
                    else // là con
                    {
                        // nếu cha đã được chọn → bỏ qua con
                        if (selectedSerchIds.Contains(item.ParentId.Value))
                            continue;

                        // thêm cha nếu chưa có
                        if (lookup.TryGetValue(item.ParentId.Value, out var parent))
                        {
                            if (!resultSet.ContainsKey(parent.Id))
                                resultSet[parent.Id] = parent;
                        }

                        if (!resultSet.ContainsKey(item.Id))
                            resultSet[item.Id] = item;
                    }
                }

                selectedData = resultSet.Values.ToList(); // kết quả chính thức
            }
            else
            {
                selectedData = allData;
            }

            var allIds = selectedData.Select(x => x.Id).ToHashSet();
            var rootNodes = selectedData.Where(x => x.ParentId == null || !allIds.Contains(x.ParentId.Value)).ToList();

            var sortedList = new List<DanhMucHopDong>();
            var visited = new HashSet<Guid>();

            foreach (var root in rootNodes)
                TreeHelper.AddWithChildren(root, selectedData, sortedList, visited, x => x.Id, x => x.ParentId, x => x.STTSapXep);

            return sortedList;
        }

        public async Task<CommonResponse> ValidateRequestAsync(DanhMucHopDong request)
        {
            var resultValidate = await new DmHopDongValidate(this, _optionDataService).ValidateAsync(request);
            if (!resultValidate.IsValid)
                return new("error", Helper.GetValidationErrorsDictionary(resultValidate), request, "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại!");
            return new("success");
        }

        public async Task<CommonResponse> StoreAsync(DanhMucHopDong request)
        {
            try
            {
                if (request.ParentId.HasValue && request.ParentId != Guid.Empty)
                {
                    var parentExisting = await this.GetEntityByIdAsync(request.ParentId.Value, isAsNoTracking: true);
                    if (parentExisting == null) return new("error", "Danh mục nhóm nghiệp vụ không còn khả dụng");

                    if (parentExisting.Level >= 2) return new("error", "Không được phép tạo danh mục ở cấp độ thứ 3");

                    request.IsCC = parentExisting.IsCC;
                }
                else
                {
                    request.ParentId = null;
                }

                await _dbContext.DanhMucHopDongs.AddAsync(request);

                await _dbContext.SaveChangesAsync();
                return new("success", "Thêm mới dữ liệu thành công");
            }
            catch (Exception)
            {
                return new("error", "Có lỗi trong quá trình cập nhật dữ liệu. Hãy kiểm tra lại!");
            }
        }

        public async Task<CommonResponse> UpdateAsync(DanhMucHopDong request)
        {
            try
            {
                var existingDanhMuc = await GetEntityByIdAsync(request.Id, isAsNoTracking: false);
                if (existingDanhMuc == null) return new("error", "Không tìm thấy thông tin danh mục. Hãy kiểm tra lại!");

                existingDanhMuc.TenHopDong = request.TenHopDong;
                existingDanhMuc.MaHopDong = request.MaHopDong;
                existingDanhMuc.TrangThai = request.TrangThai;
                existingDanhMuc.MoTa = request.MoTa;
                if (existingDanhMuc.ParentId.HasValue) // lớp con thì có 
                {
                    existingDanhMuc.LoaiGiayTo = request.LoaiGiayTo;
                }
                else
                {
                    existingDanhMuc.IsCC = request.IsCC;
                }

                _dbContext.DanhMucHopDongs.Update(existingDanhMuc);
                await _dbContext.SaveChangesAsync();
                return new("success", "Cập nhật dữ liệu thành công");
            }
            catch (Exception)
            {
                return new("error", "Có lỗi trong quá trình cập nhật dữ liệu. Hãy kiểm tra lại!");
            }
        }

        public async Task<CommonResponse> DeleteAsync(Guid hopDongId)
        {
            try
            {
                var existingDanhMuc = await GetEntityByIdAsync(hopDongId, isAsNoTracking: false);
                if (existingDanhMuc == null) return new("error", "Không tìm thấy thông tin danh mục. Hãy kiểm tra lại!");

                _dbContext.DanhMucHopDongs.Remove(existingDanhMuc);
                await _dbContext.SaveChangesAsync();

                return new("success", $"Xóa danh mục: {existingDanhMuc.TenHopDong} thành công");
            }
            catch (Exception)
            {
                return new("error", "Có lỗi trong quá trình lấy dữ liệu. Hãy kiểm tra lại!");
            }
        }
    }
}
