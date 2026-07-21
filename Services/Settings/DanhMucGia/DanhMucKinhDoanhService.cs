using DataAccess;
using DataAccess.Entities.Settings;
using DataAccess.Entities.Settings.DanhMucGia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Services.Model;
using Services.DTOs.Settings.DanhMucDungChung;

namespace Services.Settings.DanhMucGia
{
    public class DanhMucKinhDoanhService : IDmKinhDoanhService
    {
        private readonly ApplicationDbContext _dbContext;

        public DanhMucKinhDoanhService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Lấy danh sách DanhMucKinhDoanh có Level = 0 (cấp gốc) và đệ quy các con
        public List<DanhMucKinhDoanh> GetListDmKinhDoanh()
        {
            List<DanhMucKinhDoanh> list = new List<DanhMucKinhDoanh>();
            var model = _dbContext.DanhMucKinhDoanhs
                            .Where(t => t.Level == 0)
                            .OrderBy(t => t.STTSapXep);

            if (model.Any())
            {
                foreach (var item in model)
                {
                    list.Add(new DanhMucKinhDoanh
                    {
                        Id = item.Id,
                        MaNganh = item.MaNganh,
                        MaNghe = item.MaNghe,
                        TenNghe = item.TenNghe,
                        DonViQuanLyId = item.DonViQuanLyId,
                        DonViDongChuyenId = item.DonViDongChuyenId,
                        TheoDoi = item.TheoDoi,
                        PhanLoai = item.PhanLoai,
                        LoaiGia = item.LoaiGia,
                        Report = item.Report,
                        MaHH_BTC = item.MaHH_BTC,
                        Level = item.Level,
                        STTSapXep = item.STTSapXep,
                        STTHienThi = item.STTHienThi,
                        Role = item.Role,
                        RoleGoc = item.RoleGoc,
                    });

                    // Đệ quy lấy danh sách con nếu có theo dõi
                    if (item.TheoDoi == "TD" && !string.IsNullOrEmpty(item.Role))
                    {
                        Recursive(list, item.Role);
                    }
                }
            }
            return list;
        }

        // Đệ quy lấy danh sách con của DanhMucKinhDoanh
        private void Recursive(List<DanhMucKinhDoanh> list, string RoleGoc)
        {
            var childList = _dbContext.DanhMucKinhDoanhs
                                .Where(t => t.RoleGoc == RoleGoc)
                                .OrderBy(t => t.STTSapXep);

            if (childList.Any())
            {
                foreach (var child in childList)
                {
                    list.Add(new DanhMucKinhDoanh
                    {
                        Id = child.Id,
                        MaNganh = child.MaNganh,
                        MaNghe = child.MaNghe,
                        TenNghe = child.TenNghe,
                        DonViQuanLyId = child.DonViQuanLyId,
                        DonViDongChuyenId = child.DonViDongChuyenId,
                        TheoDoi = child.TheoDoi,
                        PhanLoai = child.PhanLoai,
                        LoaiGia = child.LoaiGia,
                        Report = child.Report,
                        MaHH_BTC = child.MaHH_BTC,
                        Level = child.Level,
                        STTSapXep = child.STTSapXep,
                        STTHienThi = child.STTHienThi,
                        Role = child.Role,
                        RoleGoc = child.RoleGoc,
                    });

                    if (child.TheoDoi == "TD" && !string.IsNullOrEmpty(child.Role))
                    {
                        Recursive(list, child.Role);
                    }
                }
            }
        }

        // Lấy danh sách Role của một vai trò và các Role con của nó
        public List<string> GetRolesByRole(string Role)
        {
            List<string> list = new List<string>();
            var parent = _dbContext.DanhMucKinhDoanhs.FirstOrDefault(t => t.Role == Role);
            if (parent != null)
            {
                if (!string.IsNullOrEmpty(parent.Role))
                    list.Add(parent.Role);
                if (!string.IsNullOrEmpty(parent.Role))
                {
                    RecursiveRolesByRole(list, parent.Role);
                }
            }
            return list;
        }

        // Đệ quy lấy danh sách Role con
        private void RecursiveRolesByRole(List<string> list, string RoleGoc)
        {
            var children = _dbContext.DanhMucKinhDoanhs.Where(t => t.RoleGoc == RoleGoc);
            if (children.Any())
            {
                foreach (var child in children)
                {
                    if (!string.IsNullOrEmpty(child.Role))
                        list.Add(child.Role);
                    if (!string.IsNullOrEmpty(child.Role))
                    {
                        RecursiveRolesByRole(list, child.Role);
                    }
                }
            }
        }

        // Lấy danh sách Role từ danh sách mã nghề (dùng để lọc quyền theo ngành nghề)
        public List<string> GetListRolesByMaNghe(List<string> listMaNghe)
        {
            List<string> listRole = new List<string>();
            var data = _dbContext.DanhMucKinhDoanhs.Where(t => listMaNghe.Contains(t.MaNghe ?? ""));
            if (data.Any())
            {
                foreach (var item in data)
                {
                    if (!string.IsNullOrEmpty(item.Role))
                        listRole.Add(item.Role);

                    // Đệ quy lấy RoleGoc nếu có
                    if (!string.IsNullOrEmpty(item.RoleGoc))
                    {
                        RecursiveAddRoleGoc(listRole, item.RoleGoc);
                    }
                }
            }
            return listRole;
        }

        // Đệ quy lấy RoleGoc nếu có
        private void RecursiveAddRoleGoc(List<string> listRole, string RoleGoc)
        {
            var parent = _dbContext.DanhMucKinhDoanhs.FirstOrDefault(t => t.Role == RoleGoc);
            if (parent != null)
            {
                if (!string.IsNullOrEmpty(parent.Role))
                    listRole.Add(parent.Role);
                if (!string.IsNullOrEmpty(parent.RoleGoc))
                {
                    RecursiveAddRoleGoc(listRole, parent.RoleGoc);
                }
            }
        }

        public async Task<CommonResponse> GetListByFilterAsync(DanhMucKinhDoanhFilter filter)
        {
            try
            {
                var data = GetListDmKinhDoanh();
                if (!string.IsNullOrEmpty(filter.LoaiGia))
                {
                    data = data.Where(t => t.LoaiGia == filter.LoaiGia).ToList();
                }

                if (!string.IsNullOrEmpty(filter.Search))
                {
                    var searchLower = filter.Search.ToLower().Trim();
                    data = data.Where(t =>
                        (t.TenNghe ?? "").ToLower().Contains(searchLower) ||
                        (t.MaNghe ?? "").ToLower().Contains(searchLower) ||
                        (t.MaNganh ?? "").ToLower().Contains(searchLower) ||
                        (t.MaHH_BTC ?? "").ToLower().Contains(searchLower)
                    ).ToList();
                }

                int totalRecord = data.Count;
                filter.AdjustPageIfInvalid(totalRecord);

                var paginatedData = data.Skip((filter.PageCurrent - 1) * filter.PageSize).Take(filter.PageSize).ToList();

                return new CommonResponse { Status = "success", Data = paginatedData, TotalRecord = totalRecord };
            }
            catch (Exception ex)
            {
                return new CommonResponse { Status = "error", Message = "Có lỗi xảy ra: " + ex.Message };
            }
        }

        public async Task<CommonResponse> CreateAsync(Guid Id, string LoaiGia)
        {
            try
            {
                var data = new DanhMucKinhDoanh
                {
                    Id = Guid.Empty,
                    TenNghe = "",
                    Level = 0,
                    TheoDoi = "TD",
                    LoaiGia = !string.IsNullOrEmpty(LoaiGia) ? LoaiGia : "KKG"
                };

                if (Id != Guid.Empty)
                {
                    var parent = await _dbContext.DanhMucKinhDoanhs.FindAsync(Id);
                    if (parent != null)
                    {
                        data.RoleGoc = parent.Role;
                        data.Level = parent.Level + 1;
                        data.MaNganh = parent.MaNghe;
                        data.Role = !string.IsNullOrEmpty(parent.MaNghe) ? (parent.MaNghe + ".") : "";
                        data.LoaiGia = !string.IsNullOrEmpty(parent.LoaiGia) ? parent.LoaiGia : LoaiGia;

                        var maxChildSort = await _dbContext.DanhMucKinhDoanhs
                            .Where(t => t.RoleGoc == parent.Role)
                            .Select(t => (int?)t.STTSapXep)
                            .MaxAsync();

                        data.STTSapXep = maxChildSort.HasValue ? (maxChildSort.Value + 1) : (parent.STTSapXep + 1);
                    }
                }
                else
                {
                    int count = await _dbContext.DanhMucKinhDoanhs.CountAsync(t => t.RoleGoc == null || t.RoleGoc == "");
                    data.STTSapXep = count + 1;
                }

                return new CommonResponse { Status = "success", Data = data };
            }
            catch (Exception ex)
            {
                return new CommonResponse { Status = "error", Message = "Có lỗi xảy ra: " + ex.Message };
            }
        }

        public async Task<CommonResponse> StoreAsync(DanhMucKinhDoanh request, string[] DonViQuanLyList, string[] DonViDongChuyenList)
        {
            try
            {
                request.Id = Guid.NewGuid();
                request.DonViQuanLyId = DonViQuanLyList != null ? string.Join(",", DonViQuanLyList) : null;
                request.DonViDongChuyenId = DonViDongChuyenList != null ? string.Join(",", DonViDongChuyenList) : null;
                _dbContext.DanhMucKinhDoanhs.Add(request);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success", Message = "Thêm mới thành công!" };
            }
            catch (Exception ex)
            {
                return new CommonResponse { Status = "error", Message = "Có lỗi xảy ra: " + ex.Message };
            }
        }

        public async Task<CommonResponse> EditAsync(Guid id)
        {
            try
            {
                var data = await _dbContext.DanhMucKinhDoanhs.FindAsync(id);
                if (data == null)
                {
                    return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin!" };
                }
                if (string.IsNullOrEmpty(data.LoaiGia))
                {
                    data.LoaiGia = "KKG";
                }
                return new CommonResponse { Status = "success", Data = data };
            }
            catch (Exception ex)
            {
                return new CommonResponse { Status = "error", Message = "Có lỗi xảy ra: " + ex.Message };
            }
        }

        public async Task<CommonResponse> UpdateAsync(DanhMucKinhDoanh request, string[] DonViQuanLyList, string[] DonViDongChuyenList)
        {
            try
            {
                var data = await _dbContext.DanhMucKinhDoanhs.FindAsync(request.Id);
                if (data == null)
                {
                    return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin cần cập nhật!" };
                }

                data.MaNganh = request.MaNganh;
                data.MaNghe = request.MaNghe;
                data.TenNghe = request.TenNghe;
                data.DonViQuanLyId = DonViQuanLyList != null ? string.Join(",", DonViQuanLyList) : null;
                data.DonViDongChuyenId = DonViDongChuyenList != null ? string.Join(",", DonViDongChuyenList) : null;
                data.TheoDoi = request.TheoDoi;
                data.PhanLoai = request.PhanLoai;
                data.LoaiGia = request.LoaiGia;
                data.Report = request.Report;
                data.MaHH_BTC = request.MaHH_BTC;
                data.STTSapXep = request.STTSapXep;
                data.STTHienThi = request.STTHienThi;
                data.Role = request.Role;
                data.RoleGoc = request.RoleGoc;
                data.Level = request.Level;

                _dbContext.DanhMucKinhDoanhs.Update(data);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success", Message = "Cập nhật thành công!" };
            }
            catch (Exception ex)
            {
                return new CommonResponse { Status = "error", Message = "Có lỗi xảy ra: " + ex.Message };
            }
        }

        public async Task<CommonResponse> DeleteAsync(Guid id_delete)
        {
            try
            {
                var data = await _dbContext.DanhMucKinhDoanhs.FindAsync(id_delete);
                if (data == null)
                {
                    return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin!" };
                }

                _dbContext.DanhMucKinhDoanhs.Remove(data);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success", Message = "Xóa thành công!" };
            }
            catch (Exception ex)
            {
                return new CommonResponse { Status = "error", Message = "Có lỗi xảy ra: " + ex.Message };
            }
        }
    }
}
