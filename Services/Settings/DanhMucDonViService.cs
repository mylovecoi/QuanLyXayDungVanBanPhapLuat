using DataAccess;
using DataAccess.Entities.Settings;
using Microsoft.EntityFrameworkCore;
using Services.Model;
using Services.Systems;
using System.Security.Cryptography;

namespace Services.Settings
{
    public interface IDanhMucDonViService
    {
        Task<CommonResponse> GetDanhMucDonViAsync(string Search, int PageSize = 5, int PageCurrent = 1);
        Task<List<DanhMucDonVi>> GetDanhMucDonViByIdAsync(Guid guid);
        Task<List<DanhMucDonVi>> GetDanhMucDonViChuQuanByIdAsync(Guid guid);
        Task<List<DanhMucDonVi>> GetDanhMucDonViChuQuanBySession();
        Task<CommonResponse> StoreAsync(DanhMucDonVi request);
        Task<CommonResponse> EditAsync(Guid guid);
        Task<CommonResponse> UpdateAsync(DanhMucDonVi request);
        Task<CommonResponse> DeleteAsync(Guid guid);
        Task<CommonResponse> GetDonViInfoByIdAsync(Guid guid);
        Task<int> GetSTTSapXep(Guid guid);
        Task<CommonResponse> GetDonViInfoAsync(Guid guid);
        Task<Boolean?> GetTinhNangThanhToanStatusAsync();
    }
    public class DanhMucDonViService : IDanhMucDonViService
    {
        private readonly ApplicationDbContext _dbContext;

        private readonly IAuthService _authService;
        public DanhMucDonViService(ApplicationDbContext dbContext, IAuthService authService)
        {
            _dbContext = dbContext;

            _authService = authService;
        }

        public async Task<CommonResponse> GetDanhMucDonViAsync(string Search, int PageSize, int PageCurrent)
        {
            List<DanhMucDonVi> data = new List<DanhMucDonVi>();
            HashSet<Guid> addedIds = new HashSet<Guid>(); // Tránh trùng lặp
            var query = _dbContext.DanhMucDonVis.AsQueryable();

            if (!string.IsNullOrEmpty(Search))
            {
                query = query.Where(t => t.TenDonVi.Contains(Search));
            }

            var danhMucs = await query.OrderBy(t => t.Level).ThenBy(t => t.STTSapXep).ToListAsync();

            // Nếu không tìm thấy kết quả, return luôn
            if (!danhMucs.Any()) return new CommonResponse
            {
                Status = "success",
                Data = data,
                TotalRecord = data.Count()
            };

            // Lấy toàn bộ danh sách đơn vị để xử lý quan hệ cha-con
            var allDanhMucs = await _dbContext.DanhMucDonVis.ToListAsync();

            // Lấy danh sách đơn vị cha
            var danhMucIds = danhMucs.Select(t => t.Id).ToList();

            // Lấy danh sách đơn vị con của các đơn vị tìm thấy
            foreach (var item in danhMucs)
            {
                if (addedIds.Add(item.Id)) // Kiểm tra trùng lặp trước khi thêm
                {
                    data.Add(MapDanhMucDonVi(item, allDanhMucs));
                    GetAllChildren(item.Id, allDanhMucs, data, addedIds);
                }
            }
            var dataView = data.Skip((PageCurrent - 1) * PageSize).Take(PageSize).ToList();

            return new CommonResponse { Status = "success", Data = dataView, TotalRecord = data.Count() };
        }

        public async Task<List<DanhMucDonVi>> GetDanhMucDonViChuQuanByIdAsync(Guid guid)
        {
            List<DanhMucDonVi> data = new List<DanhMucDonVi>();
            HashSet<Guid> addedIds = new HashSet<Guid>(); // Tránh trùng lặp

            // Lấy đơn vị gốc theo ID
            var model = await _dbContext.DanhMucDonVis.FindAsync(guid);
            if (model == null) return data; // Nếu không có đơn vị, trả về danh sách rỗng

            // Lấy toàn bộ danh mục để xử lý quan hệ cha-con
            var allDanhMucs = await _dbContext.DanhMucDonVis.ToListAsync();

            // Thêm đơn vị gốc vào danh sách
            // Bỏ chính nó, chỉ lấy cấp trên
            GetAllParents(model.DonViChuQuanId, allDanhMucs, data, addedIds);

            return data;
        }

        public async Task<List<DanhMucDonVi>> GetDanhMucDonViByIdAsync(Guid guid)
        {
            List<DanhMucDonVi> data = new List<DanhMucDonVi>();
            HashSet<Guid> addedIds = new HashSet<Guid>(); // Tránh trùng lặp

            // Lấy đơn vị gốc theo ID
            var model = await _dbContext.DanhMucDonVis.FindAsync(guid);
            if (model == null) return data; // Nếu không có đơn vị, trả về danh sách rỗng

            // Lấy toàn bộ danh mục để xử lý quan hệ cha-con
            var allDanhMucs = await _dbContext.DanhMucDonVis.ToListAsync();

            // Thêm đơn vị gốc vào danh sách
            if (addedIds.Add(model.Id))
            {
                data.Add(MapDanhMucDonVi(model, allDanhMucs));
                GetAllChildren(model.Id, allDanhMucs, data, addedIds);
            }

            return data;
        }

        public async Task<List<DanhMucDonVi>> GetDanhMucDonViChuQuanBySession()
        {
            List<DanhMucDonVi> data = new List<DanhMucDonVi>();
            HashSet<Guid> addedIds = new HashSet<Guid>(); // Tránh trùng lặp
            var sessionInfo = _authService.GetUserInfo();
            Guid guid = sessionInfo?.DanhMucDonViId ?? Guid.Empty;

            // Lấy đơn vị gốc theo ID
            var model = await _dbContext.DanhMucDonVis.FindAsync(guid);
            if (model == null) return data; // Nếu không có đơn vị, trả về danh sách rỗng

            // Lấy toàn bộ danh mục để xử lý quan hệ cha-con
            var allDanhMucs = await _dbContext.DanhMucDonVis.ToListAsync();

            // Thêm đơn vị gốc vào danh sách
            if (addedIds.Add(model.Id))
            {
                data.Add(MapDanhMucDonVi(model, allDanhMucs));
                GetAllChildren(model.Id, allDanhMucs, data, addedIds);
            }

            return data;
        }

        // Hàm lấy tất cả đơn vị con của một đơn vị (đệ quy)
        private void GetAllChildren(Guid parentId, List<DanhMucDonVi> danhMucs, List<DanhMucDonVi> data, HashSet<Guid> addedIds)
        {
            var children = danhMucs.Where(t => t.DonViChuQuanId == parentId).OrderBy(t => t.STTSapXep).ToList();

            foreach (var item in children)
            {
                if (addedIds.Add(item.Id)) // Kiểm tra trùng lặp trước khi thêm
                {
                    data.Add(MapDanhMucDonVi(item, danhMucs));
                    GetAllChildren(item.Id, danhMucs, data, addedIds);
                }
            }
        }

        private void GetAllParents(Guid? parentId, List<DanhMucDonVi> danhMucs, List<DanhMucDonVi> data, HashSet<Guid> addedIds)
        {
            while (parentId.HasValue)
            {
                var parent = danhMucs.FirstOrDefault(t => t.Id == parentId.Value);
                if (parent == null || !addedIds.Add(parent.Id)) break; // Nếu không tìm thấy hoặc bị trùng, dừng lại

                data.Add(MapDanhMucDonVi(parent, danhMucs));
                parentId = parent.DonViChuQuanId; // Tiếp tục lấy cấp trên
            }
        }

        // Hàm chuyển đổi dữ liệu DanhMucDonVi
        private DanhMucDonVi MapDanhMucDonVi(DanhMucDonVi item, List<DanhMucDonVi> danhMucs)
        {
            var danhMucChuQuanMap = danhMucs.ToDictionary(d => d.Id, d => d.TenDonVi);

            return new DanhMucDonVi
            {
                Id = item.Id,
                TenDonVi = item.TenDonVi,
                Level = item.Level,
                STTSapXep = item.STTSapXep,
                DonViChuQuanId = item.DonViChuQuanId,
                DiaChi = item.DiaChi,
                MaQHNS = item.MaQHNS,
                SoDienThoai = item.SoDienThoai,
                ChucDanhQuanLy = item.ChucDanhQuanLy,
                HoVaTenNguoiQuanLy = item.HoVaTenNguoiQuanLy,
                TenDonViChuQuan = danhMucChuQuanMap.GetValueOrDefault(item.DonViChuQuanId, ""),
                PhanLoaiDonVi = item.PhanLoaiDonVi,
            };
        }

        public async Task<CommonResponse> StoreAsync(DanhMucDonVi request)
        {
            try
            {
                _dbContext.DanhMucDonVis.Add(request);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success", Message = "Thêm đơn vị thành công" };
            }
            catch
            {
                return new CommonResponse { Status = "error", Message = "Không thể thêm đơn vị" };
            }
        }
        public async Task<CommonResponse> EditAsync(Guid guid)
        {
            try
            {
                var data = await _dbContext.DanhMucDonVis.FindAsync(guid);
                if (data == null)
                {
                    return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin đơn vị" };
                }

                if (data.DonViChuQuanId != Guid.Empty)
                {
                    var donViChuQuan = await _dbContext.DanhMucDonVis
                                                      .Where(t => t.Id == data.DonViChuQuanId)
                                                      .Select(t => t.TenDonVi)
                                                      .FirstOrDefaultAsync();

                    data.TenDonViChuQuan = donViChuQuan ?? "";
                }

                return new CommonResponse { Status = "success", Data = data };
            }
            catch
            {
                return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin đơn vị" };
            }
        }

        public async Task<CommonResponse> UpdateAsync(DanhMucDonVi request)
        {
            try
            {                
                _dbContext.DanhMucDonVis.Update(request);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success" };
            }
            catch
            {
                return new CommonResponse { Status = "error", Message = "Có lỗi xảy ra! Vui lòng thử lại sau!" };
            }
        }

        public async Task<CommonResponse> DeleteAsync(Guid guid)
        {
            try
            {
                var model = await _dbContext.DanhMucDonVis.FindAsync(guid);
                if (model == null)
                {
                    return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin!" };
                }
                List<Guid> listIds = (await this.GetDanhMucDonViByIdAsync(guid))
                                    .Select(t => t.Id).ToList();
                var dataRemove = _dbContext.DanhMucDonVis.Where(t => listIds.Contains(t.Id));
                _dbContext.DanhMucDonVis.RemoveRange(dataRemove);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success" };
            }
            catch
            {
                return new CommonResponse { Status = "error", Message = "Có lỗi xảy ra! Vui lòng thử lại sau!" };
            }
        }

        public async Task<CommonResponse> GetDonViInfoByIdAsync(Guid guid)
        {
            try
            {
                var data = await _dbContext.DanhMucDonVis.FindAsync(guid);
                if (data == null)
                {
                    return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin đơn vị" };
                }
                return new CommonResponse { Status = "success", Data = data };
            }
            catch
            {
                return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin đơn vị" };
            }
        }

        public async Task<int> GetSTTSapXep(Guid guid)
        {
            bool exists = await _dbContext.DanhMucDonVis.AnyAsync(t => t.DonViChuQuanId == guid);

            if (!exists)
            {
                return 1; // No existing RoleActions, return 1
            }

            // Count the number of RoleActions asynchronously
            int count = await _dbContext.DanhMucDonVis.CountAsync(t => t.DonViChuQuanId == guid);
            return count + 1;
        }

        public async Task<CommonResponse> GetDonViInfoAsync(Guid guid)
        {
            var data = await _dbContext.DanhMucDonVis.FindAsync(guid);
            if (data == null)
            {
                return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin đơn vị" };
            }
            return new CommonResponse { Status = "success", Data = data };
        }

        public async Task<Boolean?> GetTinhNangThanhToanStatusAsync()
        {
            var sessionInfo = _authService.GetUserInfo();
            Guid guid = sessionInfo?.DanhMucDonViId ?? Guid.Empty;

            var donVi = await _dbContext.DanhMucDonVis.FindAsync(guid);
            return donVi?.TinhNangThanhToan;
        }
    }
}