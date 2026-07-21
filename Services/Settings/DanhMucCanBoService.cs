using DataAccess;
using DataAccess.Entities.Settings;
using DataAccess.Entities.Systems;
using DataAccess.Enums;
using Microsoft.EntityFrameworkCore;
using Services.DTOs.BaoCaoKhac;
using Services.Model;
using Services.ReportGenerators;
using Services.Systems;

namespace Services.Settings
{
    public interface IDanhMucCanBoService
    {
        Task<CommonResponse> GetDanhMucCanBoAsync(string search, int pageSize, int currentPage, Guid? donViId = null, Guid? phongBanId = null, string status = "");
        Task<CommonResponse> StoreAsync(DanhMucCanBo request);
        Task<CommonResponse> EditAsync(Guid id);
        Task<CommonResponse> UpdateAsync(DanhMucCanBo request);
        Task<CommonResponse> DeleteAsync(Guid id);
        Task<CommonResponse> GetBaoCaoSuDungLaoDongAsync(BaoCaoSuDungLaoDongRequest request, Guid currentUserId);
        Task<CommonResponse> ExportBaoCaoSuDungLaoDongToWordAsync(BaoCaoSuDungLaoDongRequest request, Guid currentUserId, string templatePath);
    }

    public class DanhMucCanBoService(ApplicationDbContext context, IUserService userService) : IDanhMucCanBoService
    {
        public async Task<CommonResponse> DeleteAsync(Guid id)
        {
            try
            {
                var entity = await context.DanhMucCanBos.FindAsync(id);
                if (entity is null)
                    return new CommonResponse
                    {
                        Status = "error",
                        Message = "Không tìm thấy thông tin cán bộ cần xóa"
                    };

                await userService.DeleteAsync(entity.UserId);
                context.DanhMucCanBos.Remove(entity);
                await context.SaveChangesAsync();

                return new CommonResponse { Status = "success" };
            }
            catch (Exception ex)
            {
                return new CommonResponse
                {
                    Status = "error",
                    Message = $"Đã xảy ra lỗi khi xóa dữ liệu: {ex.Message}"
                };
            }
        }

        public async Task<CommonResponse> EditAsync(Guid id)
        {
            try
            {
                var entity = await context.DanhMucCanBos
                    .Include(d => d.DonViQuanLy)
                    .Include(d => d.PhongBan)
                    .FirstOrDefaultAsync(d => d.Id == id);

                if (entity is null)
                    return new CommonResponse
                    {
                        Status = "error",
                        Message = "Không tìm thấy thông tin cán bộ cần cập nhật"
                    };

                var userResponse = await userService.EditAsync(entity.UserId);
                if (userResponse.Status == "success" && userResponse.Data != null)
                {
                    if (userResponse.Data is User user)
                    {
                        entity.Status = user.Status;
                        entity.Username = user.Username;
                        entity.Email = user.Email;
                    }
                }

                return new CommonResponse
                {
                    Status = "success",
                    Data = entity
                };
            }
            catch (Exception ex)
            {
                return new CommonResponse
                {
                    Status = "error",
                    Message = $"Đã xảy ra lỗi khi lấy dữ liệu: {ex.Message}"
                };
            }
        }

        public async Task<CommonResponse> GetDanhMucCanBoAsync(string search, int pageSize, int pageCurrent, Guid? donViId = null, Guid? phongBanId = null, string status = "")
        {
            try
            {
                var query = context.DanhMucCanBos.AsQueryable();

                // Lọc theo từ khóa tìm kiếm
                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(t =>
                        EF.Functions.Like(t.TenCanBo, $"%{search}%"));
                }

                // Lọc theo đơn vị nếu có
                if (donViId.HasValue && donViId != Guid.Empty)
                {
                    query = query.Where(t => t.DonViQuanLyId == donViId);
                }

                // Lọc theo phòng ban nếu có
                if (phongBanId.HasValue && phongBanId != Guid.Empty)
                {
                    query = query.Where(t => t.PhongBanId == phongBanId);
                }

                var total = await query.CountAsync();
                query = query.OrderByDescending(t => t.UpdatedDate)
                    .Skip((pageCurrent - 1) * pageSize).Take(pageSize);

                var data = await query.Include(d => d.PhongBan).ToListAsync();

                // Lấy thông tin user cho mỗi cán bộ dựa trên UserId
                var filteredData = new List<DanhMucCanBo>();
                foreach (var item in data)
                {
                    var userResponse = await userService.EditAsync(item.UserId);
                    if (userResponse.Status == "success" && userResponse.Data != null)
                    {
                        if (userResponse.Data is User user)
                        {
                            item.Username = user.Username;
                            item.Email = user.Email;
                            item.Status = user.Status;

                            // Lọc theo trạng thái nếu có
                            if (string.IsNullOrEmpty(status) || item.Status == status)
                            {
                                filteredData.Add(item);
                            }
                        }
                    }
                }

                return new CommonResponse
                {
                    Status = "success",
                    Data = filteredData,
                    TotalRecord = string.IsNullOrEmpty(status) ? total : filteredData.Count
                };
            }
            catch (Exception ex)
            {
                return new CommonResponse
                {
                    Status = "error",
                    Message = $"Đã xảy ra lỗi khi lấy dữ liệu: {ex.Message}"
                };
            }
        }

        public async Task<CommonResponse> StoreAsync(DanhMucCanBo request)
        {
            try
            {
                // Kiểm tra username đã tồn tại chưa
                var isExist = await userService.IsUserlExitAsync(request.Username, request.Email);
                if (isExist)
                {
                    return new CommonResponse
                    {
                        Status = "error",
                        Message = "Tên đăng nhập hoặc email đã tồn tại trong hệ thống!"
                    };
                }

                // Kiểm tra phòng ban có thuộc đơn vị không
                var phongBan = await context.DanhMucPhongBans.FindAsync(request.PhongBanId);
                if (phongBan == null || phongBan.DanhMucDonViId != request.DonViQuanLyId)
                {
                    return new CommonResponse
                    {
                        Status = "error",
                        Message = "Phòng ban không thuộc đơn vị này!"
                    };
                }

                // Tạo user mới
                var newUser = new User
                {
                    Id = Guid.NewGuid(),
                    Username = request.Username,
                    Password = request.Password,
                    Email = request.Email,
                    Name = request.TenCanBo,
                    SSA = false,
                    DanhMucDonViId = request.DonViQuanLyId,
                    Status = request.Status,
                    FirstLogin = true,
                    LoginCount = 0,
                    OTPSecretKey = Guid.NewGuid().ToString(),
                    GroupPermissionId = Guid.Empty // Cần thiết lập một GroupPermissionId mặc định
                };

                var userResult = await userService.StoreAsync(newUser);
                if (userResult.Status != "success")
                {
                    return userResult;
                }

                request.UserId = newUser.Id;

                // Tạo cán bộ
                context.DanhMucCanBos.Add(request);
                await context.SaveChangesAsync();

                return new CommonResponse { Status = "success" };
            }
            catch (Exception ex)
            {
                return new CommonResponse
                {
                    Status = "error",
                    Message = $"Đã xảy ra lỗi khi lưu dữ liệu: {ex.Message}"
                };
            }
        }

        public async Task<CommonResponse> UpdateAsync(DanhMucCanBo request)
        {
            try
            {
                var entity = await context.DanhMucCanBos.FindAsync(request.Id);
                if (entity is null)
                    return new CommonResponse
                    {
                        Status = "error",
                        Message = "Không tìm thấy thông tin cán bộ cần cập nhật"
                    };

                // Kiểm tra phòng ban có thuộc đơn vị không
                var phongBan = await context.DanhMucPhongBans.FindAsync(request.PhongBanId);
                if (phongBan == null || phongBan.DanhMucDonViId != request.DonViQuanLyId)
                {
                    return new CommonResponse
                    {
                        Status = "error",
                        Message = "Phòng ban không thuộc đơn vị này!"
                    };
                }

                // Cập nhật thông tin cán bộ
                entity.TenCanBo = request.TenCanBo;
                entity.NgaySinh = request.NgaySinh;
                entity.DonViQuanLyId = request.DonViQuanLyId;
                entity.PhongBanId = request.PhongBanId;
                entity.GioiTinh = request.GioiTinh;
                entity.TrinhDoChuyenMon = request.TrinhDoChuyenMon;
                entity.LoaiLaoDong = request.LoaiLaoDong;
                entity.SoTienBHXH = request.SoTienBHXH;
                entity.SoTienBHYT = request.SoTienBHYT;
                entity.GhiChu = request.GhiChu;

                // Cập nhật thông tin quyết định dừng nếu trạng thái là Khóa
                if (request.Status == "Khóa")
                {
                    entity.SoQuyetDinhDung = request.SoQuyetDinhDung;
                    entity.NgayQuyetDinhDung = request.NgayQuyetDinhDung;
                }

                // Cập nhật thông tin theo loại lao động
                if (request.LoaiLaoDong == LoaiLaoDong.CongChungVien)
                {
                    entity.SoQuyetDinhBoNhiem = request.SoQuyetDinhBoNhiem;
                    entity.NgayQuyetDinhBoNhiem = request.NgayQuyetDinhBoNhiem;
                    entity.SoQuyetDinhCapThe = request.SoQuyetDinhCapThe;
                    entity.NgayQuyetDinhCapThe = request.NgayQuyetDinhCapThe;
                    entity.SoTheCongChungVien = request.SoTheCongChungVien;
                    entity.ChucVu = request.ChucVu;
                    entity.MucPhiBaoHiemTrachNhiem = request.MucPhiBaoHiemTrachNhiem;
                }

                if (request.LoaiLaoDong == LoaiLaoDong.NhanVienNghiepVu || request.LoaiLaoDong == LoaiLaoDong.NhanVienKhac)
                {
                    entity.ViTriViecLam = request.ViTriViecLam;
                    entity.NgayTuyenDung = request.NgayTuyenDung;
                    entity.SoHopDongLaoDong = request.SoHopDongLaoDong;
                    entity.NgayKyHopDongLaoDong = request.NgayKyHopDongLaoDong;
                }

                context.DanhMucCanBos.Update(entity);
                await context.SaveChangesAsync();

                // Cập nhật thông tin user tương ứng dựa trên UserId
                if (entity.UserId != Guid.Empty)
                {
                    var userResponse = await userService.EditAsync(entity.UserId);
                    if (userResponse.Status == "success" && userResponse.Data != null)
                    {
                        if (userResponse.Data is User user)
                        {
                            user.Name = request.TenCanBo;
                            user.DanhMucDonViId = request.DonViQuanLyId;
                            user.Status = request.Status;

                            await userService.UpdateAsync(user);
                        }
                    }
                }

                return new CommonResponse { Status = "success" };
            }
            catch (Exception ex)
            {
                return new CommonResponse
                {
                    Status = "error",
                    Message = $"Đã xảy ra lỗi khi cập nhật dữ liệu: {ex.Message}"
                };
            }
        }

        public async Task<CommonResponse> GetBaoCaoSuDungLaoDongAsync(BaoCaoSuDungLaoDongRequest request, Guid currentUserId)
        {
            try
            {
                // Lấy thông tin user hiện tại
                var currentUser = await context.Users.FindAsync(currentUserId);
                if (currentUser == null)
                {
                    return new CommonResponse
                    {
                        Status = "error",
                        Message = "Không tìm thấy thông tin người dùng"
                    };
                }

                // Lấy thông tin đơn vị
                var donVi = await context.DanhMucDonVis.FindAsync(request.DonViId);
                if (donVi == null)
                {
                    return new CommonResponse
                    {
                        Status = "error",
                        Message = "Không tìm thấy thông tin đơn vị"
                    };
                }

                // Query cán bộ theo điều kiện
                var query = context.DanhMucCanBos
                    .Include(cb => cb.DonViQuanLy)
                    .Include(cb => cb.PhongBan)
                    .Where(cb => cb.DonViQuanLyId == request.DonViId);

                // Lọc theo khoảng thời gian bắt buộc
                query = query.Where(cb =>
                    (cb.NgayTuyenDung >= request.TuNgay && cb.NgayTuyenDung <= request.DenNgay) ||
                    (cb.NgayQuyetDinhBoNhiem >= request.TuNgay && cb.NgayQuyetDinhBoNhiem <= request.DenNgay) ||
                    (cb.CreatedDate >= request.TuNgay && cb.CreatedDate <= request.DenNgay));

                var danhSachCanBo = await query.ToListAsync();

                // Phân loại theo loại lao động
                var congChungVien = danhSachCanBo
                    .Where(cb => cb.LoaiLaoDong == LoaiLaoDong.CongChungVien)
                    .ToList();

                var nhanVienNghiepVu = danhSachCanBo
                    .Where(cb => cb.LoaiLaoDong == LoaiLaoDong.NhanVienNghiepVu)
                    .ToList();

                var nhanVienKhac = danhSachCanBo
                    .Where(cb => cb.LoaiLaoDong == LoaiLaoDong.NhanVienKhac)
                    .ToList();

                // Tạo response sử dụng trực tiếp entities
                var response = new BaoCaoSuDungLaoDongResponse
                {
                    ThongTinToChuc = new ThongTinToChuc
                    {
                        TenToChuc = Helper.CleanHTMLTag(currentUser.TenDonViBaoCao),
                        TinhThanhPho = currentUser.DiaDanh ?? string.Empty,
                        QuyenSo = request.QuyenSo,
                        NgayMoSo = request.TuNgay,
                        NgayKhoaSo = request.DenNgay,
                        Nam = DateTime.Now.Year
                    },

                    DanhSachCongChungVien = congChungVien,
                    DanhSachNhanVien = [.. nhanVienNghiepVu, .. nhanVienKhac],

                    ThongKe = new ThongKeTongHop
                    {
                        TongSoLaoDong = danhSachCanBo.Count,
                        SoCongChungVien = congChungVien.Count,
                        SoNhanVienNghiepVu = nhanVienNghiepVu.Count,
                        SoNhanVienKhac = nhanVienKhac.Count,
                        TongSoHopDongDaKy = nhanVienNghiepVu.Count + nhanVienKhac.Count,
                        SoHopDongDaChamDut = danhSachCanBo.Count(cb => cb.NgayQuyetDinhDung.HasValue),
                        SoHopDongDangThucHien = danhSachCanBo.Count(cb => !cb.NgayQuyetDinhDung.HasValue),
                        TongTienBaoHiemTrachNhiem = congChungVien.Sum(cb => cb.MucPhiBaoHiemTrachNhiem ?? 0),
                        TongTienBHXH = danhSachCanBo.Sum(cb => cb.SoTienBHXH),
                        TongTienBHYT = danhSachCanBo.Sum(cb => cb.SoTienBHYT),
                        NgayBaoCao = DateTime.Now,
                        DiaDanh = currentUser.DiaDanh ?? string.Empty
                    }
                };

                return new CommonResponse
                {
                    Status = "success",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new CommonResponse
                {
                    Status = "error",
                    Message = $"Đã xảy ra lỗi khi tạo báo cáo: {ex.Message}"
                };
            }
        }

        public async Task<CommonResponse> ExportBaoCaoSuDungLaoDongToWordAsync(BaoCaoSuDungLaoDongRequest request, Guid currentUserId, string templatePath)
        {
            try
            {
                // Lấy dữ liệu báo cáo
                var reportDataResponse = await GetBaoCaoSuDungLaoDongAsync(request, currentUserId);
                if (reportDataResponse.Status != "success" || reportDataResponse.Data == null)
                {
                    return reportDataResponse;
                }

                var baoCaoResponse = (BaoCaoSuDungLaoDongResponse)reportDataResponse.Data!;

                // Tạo file Word trực tiếp từ response model
                var fileBytes = WordReportGenerator.GenerateReport(templatePath, baoCaoResponse);

                return new CommonResponse
                {
                    Status = "success",
                    Data = fileBytes,
                    Message = "Xuất báo cáo thành công"
                };
            }
            catch (Exception ex)
            {
                return new CommonResponse
                {
                    Status = "error",
                    Message = $"Đã xảy ra lỗi khi xuất báo cáo: {ex.Message}"
                };
            }
        }
    }
}
