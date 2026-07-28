using DataAccess;
using DataAccess.Entities.Manages;
using Microsoft.EntityFrameworkCore;
using Services.Model;
using Services.Systems;

namespace Services.Manages
{
    public interface IVanBanPhapLuatService
    {
        Task<CommonResponse> GetVanBanPhapLuatsAsync(string search, Guid? donViId, int pageSize, int pageCurrent, bool onlyPublic);
        Task<CommonResponse> StoreAsync(AttachedFile request);
        Task<CommonResponse> EditAsync(Guid guid);
        Task<CommonResponse> UpdateAsync(AttachedFile request);
        Task<CommonResponse> DeleteAsync(Guid guid);
    }

    public class VanBanPhapLuatService(
        ApplicationDbContext dbContext,
        IAuthService authService) : IVanBanPhapLuatService
    {
        private readonly ApplicationDbContext _dbContext = dbContext;
        private readonly IAuthService _authService = authService;

        public async Task<CommonResponse> GetVanBanPhapLuatsAsync(string search, Guid? donViId, int pageSize, int pageCurrent, bool onlyPublic)
        {
            try
            {
                var data = _dbContext.AttachedFiles
                    .AsNoTracking()
                    .Where(t => t.TableName == "VanBanPhapLuat");

                if (onlyPublic)
                {
                    data = data.Where(t => t.Public);
                }

                if (donViId.HasValue && donViId.Value != Guid.Empty)
                {
                    data = data.Where(t => t.DonViId == donViId.Value);
                }

                if (!string.IsNullOrWhiteSpace(search))
                {
                    var keyword = search.Trim().ToLower();
                    data = data.Where(t =>
                        (!string.IsNullOrEmpty(t.SoVanBan) && t.SoVanBan.ToLower().Contains(keyword)) ||
                        (!string.IsNullOrEmpty(t.MoTa) && t.MoTa.ToLower().Contains(keyword)) ||
                        (!string.IsNullOrEmpty(t.FileName) && t.FileName.ToLower().Contains(keyword)) ||
                        (!string.IsNullOrEmpty(t.ContentType) && t.ContentType.ToLower().Contains(keyword)) ||
                        (!string.IsNullOrEmpty(t.Url) && t.Url.ToLower().Contains(keyword)));
                }

                var totalRecords = await data.CountAsync();
                var dataView = await data
                    .OrderByDescending(t => t.NgayBanHanh)
                    .ThenByDescending(t => t.CreatedDate)
                    .Skip((pageCurrent - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return new CommonResponse
                {
                    Status = "success",
                    Data = dataView,
                    TotalRecord = totalRecords
                };
            }
            catch
            {
                return new CommonResponse
                {
                    Status = "error",
                    Message = "Đã xảy ra lỗi khi lấy dữ liệu. Vui lòng thử lại sau!"
                };
            }
        }

        public async Task<CommonResponse> StoreAsync(AttachedFile request)
        {
            try
            {
                var currentUser = _authService.GetUserInfo();
                var attachedFile = new AttachedFile
                {
                    SoVanBan = request.SoVanBan,
                    NgayApDung = request.NgayApDung,
                    NgayBanHanh = request.NgayBanHanh,
                    TableName = "VanBanPhapLuat",
                    MoTa = request.MoTa,
                    Url = request.Url,
                    Public = request.Public,
                    Status = "XD",
                    DonViId = request.DonViId ?? (currentUser?.DanhMucDonViId != Guid.Empty ? currentUser?.DanhMucDonViId : null)
                };

                if (request.FileUpLoad != null && request.FileUpLoad.Length != 0)
                {
                    using var ms = new MemoryStream();
                    await request.FileUpLoad.CopyToAsync(ms);
                    attachedFile.FileName = request.FileUpLoad.FileName;
                    attachedFile.ContentType = request.FileUpLoad.ContentType;
                    attachedFile.FileContent = ms.ToArray();
                }

                _dbContext.AttachedFiles.Add(attachedFile);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success" };
            }
            catch
            {
                return new CommonResponse
                {
                    Status = "error",
                    Message = "Đã xảy ra lỗi khi cập nhật dữ liệu. Vui lòng thử lại sau!"
                };
            }
        }

        public async Task<CommonResponse> EditAsync(Guid guid)
        {
            try
            {
                var data = await _dbContext.AttachedFiles.FindAsync(guid);
                if (data == null)
                {
                    return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin dữ liệu!" };
                }

                return new CommonResponse { Status = "success", Data = data };
            }
            catch
            {
                return new CommonResponse
                {
                    Status = "error",
                    Message = "Đã xảy ra lỗi khi lấy dữ liệu. Vui lòng thử lại sau!"
                };
            }
        }

        public async Task<CommonResponse> UpdateAsync(AttachedFile request)
        {
            try
            {
                var currentUser = _authService.GetUserInfo();
                var data = await _dbContext.AttachedFiles.FindAsync(request.Id);
                if (data == null)
                {
                    return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin dữ liệu cần cập nhật" };
                }

                data.SoVanBan = request.SoVanBan;
                data.NgayApDung = request.NgayApDung;
                data.NgayBanHanh = request.NgayBanHanh;
                data.MoTa = request.MoTa;
                data.Public = request.Public;
                data.Url = request.Url;
                data.DonViId = request.DonViId ?? data.DonViId ?? (currentUser?.DanhMucDonViId != Guid.Empty ? currentUser?.DanhMucDonViId : null);

                if (request.FileUpLoad != null && request.FileUpLoad.Length != 0)
                {
                    using var ms = new MemoryStream();
                    await request.FileUpLoad.CopyToAsync(ms);
                    data.FileName = request.FileUpLoad.FileName;
                    data.ContentType = request.FileUpLoad.ContentType;
                    data.FileContent = ms.ToArray();
                }

                _dbContext.AttachedFiles.Update(data);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success" };
            }
            catch
            {
                return new CommonResponse
                {
                    Status = "error",
                    Message = "Đã xảy ra lỗi khi cập nhật dữ liệu. Vui lòng thử lại sau!"
                };
            }
        }

        public async Task<CommonResponse> DeleteAsync(Guid guid)
        {
            try
            {
                var data = await _dbContext.AttachedFiles.FindAsync(guid);
                if (data == null)
                {
                    return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin dữ liệu cần cập nhật" };
                }

                _dbContext.AttachedFiles.Remove(data);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success" };
            }
            catch
            {
                return new CommonResponse
                {
                    Status = "error",
                    Message = "Đã xảy ra lỗi khi xóa dữ liệu. Vui lòng thử lại sau!"
                };
            }
        }
    }
}
