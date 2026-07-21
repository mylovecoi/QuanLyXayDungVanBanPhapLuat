using DataAccess;
using DataAccess.Entities.KeKhaiDangKyGia;
using DataAccess.Entities.Systems;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Services.Model;
using Services.Systems;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Services.KeKhaiDangKyGia
{
    public class DoanhNghiepService(
        ApplicationDbContext dbContext,
        OTPService otpService) : IDoanhNghiepService
    {
        private readonly ApplicationDbContext _dbContext = dbContext;
        private readonly OTPService _otpService = otpService;

        public async Task<DoanhNghiep?> GetDoanhNghiepByMaSoThueAsync(string maSoThue)
        {
            return await _dbContext.DoanhNghieps
                .FirstOrDefaultAsync(t => t.MaSoThue == maSoThue);
        }

        public async Task<DoanhNghiep> GetOrCreateTempDoanhNghiepAsync(string maSoThue, Guid? defaultUnitId = null)
        {
            var doanhNghiep = await GetDoanhNghiepByMaSoThueAsync(maSoThue);
            if (doanhNghiep == null)
            {
                if (defaultUnitId == null || defaultUnitId == Guid.Empty)
                {
                    throw new ArgumentException("Đơn vị tiếp nhận hồ sơ không hợp lệ hoặc chưa được chọn!");
                }
                doanhNghiep = new DoanhNghiep
                {
                    Id = Guid.NewGuid(),
                    MaSoThue = maSoThue,
                    DonViQuanLyId = defaultUnitId.Value,
                    TrangThai = "CXD",
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now
                };
                _dbContext.DoanhNghieps.Add(doanhNghiep);
                await _dbContext.SaveChangesAsync();
            }
            return doanhNghiep;
        }

        public async Task<List<DoanhNghiepLvKd>> GetLvkdByDoanhNghiepIdAsync(Guid doanhNghiepId)
        {
            return await _dbContext.DoanhNghiepLvKds
                .Where(t => t.DoanhNghiepQuanLyId == doanhNghiepId)
                .ToListAsync();
        }

        public async Task<CommonResponse> StoreLvKdAsync(string maSoThue, string maNganh, string maNghe, Guid donViQuanLyId)
        {
            try
            {
                if (donViQuanLyId == Guid.Empty)
                {
                    return new CommonResponse { Status = "error", Message = "Đơn vị tiếp nhận hồ sơ không hợp lệ hoặc chưa được chọn!" };
                }

                var existingDn = await _dbContext.DoanhNghieps.FirstOrDefaultAsync(t => t.MaSoThue == maSoThue);
                if (existingDn != null && existingDn.TrangThai != "CXD")
                {
                    return new CommonResponse { Status = "error", Message = "Mã số thuế này đã được đăng ký tài khoản trên hệ thống!" };
                }

                var doanhNghiep = await GetOrCreateTempDoanhNghiepAsync(maSoThue, donViQuanLyId);

                // Check if already exists
                var exists = await _dbContext.DoanhNghiepLvKds
                    .AnyAsync(t => t.DoanhNghiepQuanLyId == doanhNghiep.Id && t.MaNghe == maNghe);
                if (exists)
                {
                    return new CommonResponse { Status = "error", Message = "Lĩnh vực kinh doanh này đã được thêm!" };
                }

                var lvkd = new DoanhNghiepLvKd
                {
                    Id = Guid.NewGuid(),
                    DoanhNghiepQuanLyId = doanhNghiep.Id,
                    MaNganh = maNganh,
                    MaNghe = maNghe,
                    DonViQuanLyId = donViQuanLyId,
                    TrangThai = "CXD",
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now
                };

                _dbContext.DoanhNghiepLvKds.Add(lvkd);
                await _dbContext.SaveChangesAsync();

                return new CommonResponse { Status = "success" };
            }
            catch (Exception ex)
            {
                return new CommonResponse { Status = "error", Message = "Lỗi khi thêm LVKD: " + ex.Message };
            }
        }

        public async Task<DoanhNghiepLvKd?> GetLvKdByIdAsync(Guid id)
        {
            return await _dbContext.DoanhNghiepLvKds.FindAsync(id);
        }

        public async Task<CommonResponse> UpdateLvKdAsync(Guid id, string maNganh, string maNghe, Guid donViQuanLyId)
        {
            try
            {
                var lvkd = await _dbContext.DoanhNghiepLvKds.FindAsync(id);
                if (lvkd == null)
                {
                    return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin!" };
                }

                lvkd.MaNganh = maNganh;
                lvkd.MaNghe = maNghe;
                lvkd.DonViQuanLyId = donViQuanLyId;
                lvkd.UpdatedDate = DateTime.Now;

                _dbContext.DoanhNghiepLvKds.Update(lvkd);
                await _dbContext.SaveChangesAsync();

                return new CommonResponse { Status = "success" };
            }
            catch (Exception ex)
            {
                return new CommonResponse { Status = "error", Message = "Lỗi khi cập nhật LVKD: " + ex.Message };
            }
        }

        public async Task<CommonResponse> DeleteLvKdAsync(Guid id)
        {
            try
            {
                var lvkd = await _dbContext.DoanhNghiepLvKds.FindAsync(id);
                if (lvkd == null)
                {
                    return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin!" };
                }

                _dbContext.DoanhNghiepLvKds.Remove(lvkd);
                await _dbContext.SaveChangesAsync();

                return new CommonResponse { Status = "success" };
            }
            catch (Exception ex)
            {
                return new CommonResponse { Status = "error", Message = "Lỗi khi xóa LVKD: " + ex.Message };
            }
        }

        public async Task<CommonResponse> CompleteRegistrationAsync(DoanhNghiep request, string username, string password)
        {
            try
            {
                if (string.IsNullOrEmpty(request.MaSoThue))
                {
                    return new CommonResponse { Status = "error", Message = "Mã số thuế không được để trống!" };
                }

                if (string.IsNullOrEmpty(request.Email))
                {
                    return new CommonResponse { Status = "error", Message = "Email không được để trống!" };
                }

                if (!new System.ComponentModel.DataAnnotations.EmailAddressAttribute().IsValid(request.Email))
                {
                    return new CommonResponse { Status = "error", Message = "Định dạng Email không hợp lệ!" };
                }

                if (await _dbContext.Users.AnyAsync(t => t.Username == username))
                {
                    return new CommonResponse { Status = "error", Message = "Tên tài khoản đăng nhập đã tồn tại!" };
                }

                if (await _dbContext.Users.AnyAsync(t => t.Email == request.Email))
                {
                    return new CommonResponse { Status = "error", Message = "Email này đã được đăng ký tài khoản trên hệ thống!" };
                }

                if (string.IsNullOrEmpty(password) || password.Length < 8 ||
                    !password.Any(char.IsUpper) || !password.Any(char.IsLower) ||
                    !password.Any(char.IsDigit) || !password.Any(c => !char.IsLetterOrDigit(c)))
                {
                    return new CommonResponse { Status = "error", Message = "Mật khẩu phải tối thiểu 8 ký tự, bao gồm ít nhất 1 chữ hoa, 1 chữ thường, 1 chữ số và 1 ký tự đặc biệt!" };
                }

                var doanhNghiep = await GetDoanhNghiepByMaSoThueAsync(request.MaSoThue);
                if (doanhNghiep == null)
                {
                    return new CommonResponse { Status = "error", Message = "Vui lòng thêm ít nhất một lĩnh vực kinh doanh!" };
                }

                // Check if Lvkd is empty
                var lvkdList = await GetLvkdByDoanhNghiepIdAsync(doanhNghiep.Id);
                if (lvkdList == null || lvkdList.Count == 0)
                {
                    return new CommonResponse { Status = "error", Message = "Vui lòng thêm ít nhất một lĩnh vực kinh doanh!" };
                }

                if (request.DonViQuanLyId == Guid.Empty)
                {
                    return new CommonResponse { Status = "error", Message = "Vui lòng chọn cơ quan quản lý tài khoản!" };
                }

                if (!lvkdList.Any(x => x.DonViQuanLyId == request.DonViQuanLyId))
                {
                    return new CommonResponse { Status = "error", Message = "Cơ quan quản lý tài khoản không hợp lệ!" };
                }

                // Update business info
                DoanhNghiepMapper.CopyTo(request, doanhNghiep);
                doanhNghiep.DonViQuanLyId = request.DonViQuanLyId;

                // Handle file upload if present
                if (request.GiayPhepKdUpload != null && request.GiayPhepKdUpload.Length > 0)
                {
                    var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "FileUpload", "GiayPhepKd");
                    if (!Directory.Exists(uploadDir))
                    {
                        Directory.CreateDirectory(uploadDir);
                    }
                    var fileName = $"{Guid.NewGuid()}_{request.GiayPhepKdUpload.FileName}";
                    var filePath = Path.Combine(uploadDir, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await request.GiayPhepKdUpload.CopyToAsync(stream);
                    }
                    doanhNghiep.GiayPhepKd = $"/FileUpload/GiayPhepKd/{fileName}";
                }

                // Update DoanhNghiepLvKd records associated with this tax code from TEMP to active or same as DN
                foreach (var item in lvkdList)
                {
                    item.TrangThai = "Chờ kích hoạt";
                    _dbContext.DoanhNghiepLvKds.Update(item);
                }

                // Query for default GroupPermission for business if exists, otherwise Guid.Empty
                var groupPermission = await _dbContext.GroupsPermision
                    .FirstOrDefaultAsync(t => t.Name.Contains("Doanh nghiệp") || t.Name.Contains("DN"));
                var groupPermissionId = groupPermission?.Id ?? Guid.Empty;

                // Create User
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Username = username,
                    Password = Helper.BCryptHash(password),
                    Email = request.Email ?? "",
                    Name = request.TenDoanhNghiep ?? "",
                    Status = "Chờ kích hoạt",
                    Level = "Doanh nghiệp",
                    OTPSecretKey = _otpService.GenerateSecretKey(),
                    DoanhNghiepId = doanhNghiep.Id,
                    DanhMucDonViId = request.DonViQuanLyId,
                    GroupPermissionId = groupPermissionId,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now
                };

                _dbContext.Users.Add(user);
                _dbContext.DoanhNghieps.Update(doanhNghiep);
                await _dbContext.SaveChangesAsync();

                return new CommonResponse { Status = "success" };
            }
            catch (Exception ex)
            {
                return new CommonResponse { Status = "error", Message = "Đăng ký thất bại: " + ex.Message };
            }
        }

        public async Task<List<DoanhNghiep>> GetListDoanhNghiepAsync()
        {
            return await _dbContext.DoanhNghieps
                .Where(t => t.TrangThai != "CXD")
                .OrderBy(t => t.TenDoanhNghiep)
                .ToListAsync();
        }

        public async Task<DoanhNghiep?> GetDoanhNghiepByIdAsync(Guid id)
        {
            return await _dbContext.DoanhNghieps
                .FirstOrDefaultAsync(t => t.Id == id);
        }
    }
}
