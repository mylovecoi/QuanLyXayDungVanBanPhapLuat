using DataAccess;
using DataAccess.Entities.Manages;
using Services.Model;
using Microsoft.EntityFrameworkCore;

namespace Services.Manages
{
    public interface IVanBanPhapLuatService
    {
        Task<CommonResponse> GetVanBanPhapLuatsAsync(string Search, int PageSize, int PageCurrent, bool isPublic);
        Task<CommonResponse> StoreAsync(AttachedFile request);
        Task<CommonResponse> EditAsync(Guid guid);
        Task<CommonResponse> UpdateAsync(AttachedFile request);
        Task<CommonResponse> DeleteAsync(Guid guid);
    }
    public class VanBanPhapLuatService : IVanBanPhapLuatService
    {
        private readonly ApplicationDbContext _dbContext;
        public VanBanPhapLuatService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<CommonResponse> GetVanBanPhapLuatsAsync(string Search, int PageSize, int PageCurrent, bool onlyPublic)
        {
            try
            {
                var data = _dbContext.AttachedFiles.Where(t => t.TableName == "VanBanPhapLuat");
                if (onlyPublic == true)
                {
                    data = data.Where(t => t.Public);
                }

                if (!string.IsNullOrWhiteSpace(Search))
                {
                    string keyword = Search.Trim().ToLower();

                    data = data.Where(t =>
                        (!string.IsNullOrEmpty(t.SoVanBan) && t.SoVanBan.ToLower().Contains(keyword)) ||
                        (!string.IsNullOrEmpty(t.MoTa) && t.MoTa.ToLower().Contains(keyword)) ||
                        (!string.IsNullOrEmpty(t.FileName) && t.FileName.ToLower().Contains(keyword)) ||
                        (!string.IsNullOrEmpty(t.ContentType) && t.ContentType.ToLower().Contains(keyword)) ||
                        (!string.IsNullOrEmpty(t.Url) && t.Url.ToLower().Contains(keyword))
                    );
                }
                int totalRecords = await data.CountAsync();
                var dataView = await data.Skip((PageCurrent - 1) * PageSize).Take(PageSize).ToListAsync();
                return new CommonResponse { Status = "success", Data = dataView, TotalRecord = totalRecords };
            }
            catch
            {
                return new CommonResponse { Status = "error", Message = "Đã xảy ra lỗi khi lấy dữ liệu. Vui lòng thử lại sau!" };
            }
        }

        public async Task<CommonResponse> StoreAsync(AttachedFile request)
        {
            try
            {
                var attachedFile = new AttachedFile
                {
                    SoVanBan = request.SoVanBan,
                    NgayApDung = request.NgayApDung,
                    NgayBanHanh = request.NgayBanHanh,
                    TableName = "VanBanPhapLuat",
                    MoTa = request.MoTa,
                    Url = request.Url,
                    Public = request.Public,
                    Status = "XD"
                };

                if (request.FileUpLoad != null && request.FileUpLoad.Length != 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        await request.FileUpLoad.CopyToAsync(ms);
                        attachedFile.FileName = request.FileUpLoad.FileName;
                        attachedFile.ContentType = request.FileUpLoad.ContentType;
                        attachedFile.FileContent = ms.ToArray();
                    }
                }
                _dbContext.AttachedFiles.Add(attachedFile);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success" };
            }
            catch
            {
                return new CommonResponse { Status = "error", Message = "Đã xảy ra lỗi khi cập nhật dữ liệu. Vui lòng thử lại sau!" };
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
                return new CommonResponse { Status = "success", Data = data ?? new AttachedFile() };
            }
            catch
            {
                return new CommonResponse { Status = "error", Message = "Đã xảy ra lỗi khi lấy dữ liệu. Vui lòng thử lại sau!" };
            }
        }

        public async Task<CommonResponse> UpdateAsync(AttachedFile request)
        {
            try
            {
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
                if (request.FileUpLoad != null && request.FileUpLoad.Length != 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        await request.FileUpLoad.CopyToAsync(ms);
                        data.FileName = request.FileUpLoad.FileName;
                        data.ContentType = request.FileUpLoad.ContentType;
                        data.FileContent = ms.ToArray();
                    }
                }
                _dbContext.AttachedFiles.Update(data);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success" };
            }
            catch
            {
                return new CommonResponse { Status = "error", Message = "Đã xảy ra lỗi khi cập nhật dữ liệu. Vui lòng thử lại sau!" };
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
                return new CommonResponse { Status = "error", Message = "Đã xảy ra lỗi khi xóa dữ liệu. Vui lòng thử lại sau!" };
            }
        }

        
    }
}
