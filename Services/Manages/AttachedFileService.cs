using DataAccess;
using DataAccess.Entities.Manages;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Services.Model;
using Services.Systems;
using System.Data;

namespace Services.Manages
{
    public interface IAttachedFileService
    {
        Task<CommonResponse> GetAttachedFilesAsync(Guid GroupId, string TableName, string Search, int PageSize, int PageCurrent);
        Task<List<AttachedFile>> GetAllAttachedFilesAsync(Guid GroupId, string TableName);
        Task<CommonResponse> StoreAsync(AttachedFile request);
        Task RemoveDatarRedundantAsync();
        Task RemoveDataRedundantAsync(string tableName);
        Task<AttachedFile> EditAsync(Guid guid);
        Task<CommonResponse> UpdateAsync(AttachedFile request);
        Task<CommonResponse> DeleteAsync(Guid guid);
        Task RemoveRangeByGroupId(Guid guid);
        Task UpdateRangeStatus(Guid guid, string groupName);

    }
    public class AttachedFileService : IAttachedFileService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IAuthService _authService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AttachedFileService(ApplicationDbContext dbContext, IAuthService authService, IWebHostEnvironment webHostEnvironment)
        {
            _dbContext = dbContext;
            _authService = authService;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<CommonResponse> GetAttachedFilesAsync(Guid GroupId, string TableName, string Search, int PageSize, int PageCurrent)
        {
            try
            {
                var data = _dbContext.AttachedFiles.Where(t => t.GroupId == GroupId && t.TableName == TableName);
                if (!string.IsNullOrEmpty(Search))
                {
                    data = data.Where(t => t.MoTa != null && t.MoTa.ToLower().Contains(Search.ToLower()));
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

        public async Task<List<AttachedFile>> GetAllAttachedFilesAsync(Guid GroupId, string TableName)
        {
            var data = _dbContext.AttachedFiles.Where(t => t.GroupId == GroupId && t.TableName == TableName);
            return await data.ToListAsync();
        }

        public async Task<CommonResponse> StoreAsync(AttachedFile request)
        {
            try
            {
                var attachedFile = new AttachedFile
                {
                    GroupId = request.GroupId,
                    TableName = request.TableName,
                    MoTa = request.MoTa,
                    Url = request.Url,
                    Public = request.Public,
                    Status = "CXD",
                };

                if (request.FileUpLoad != null && request.FileUpLoad.Length != 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        await request.FileUpLoad.CopyToAsync(ms);
                        attachedFile.FileName = request.FileUpLoad.FileName;
                        var contentType = request.FileUpLoad.ContentType ?? "";
                        attachedFile.ContentType = contentType.Length > 50 ? contentType.Substring(0, 50) : contentType;
                        attachedFile.FileContent = ms.ToArray();
                    }
                }
                else if (!string.IsNullOrEmpty(request.ScannedFilePath) && !string.IsNullOrEmpty(request.ScannedFileName))
                {
                    var absolutePath = Path.Combine(_webHostEnvironment.WebRootPath, request.ScannedFilePath.TrimStart('/'));

                    if (System.IO.File.Exists(absolutePath))
                    {
                        byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(absolutePath);
                        attachedFile.FileName = request.ScannedFileName;
                        attachedFile.ContentType = "application/pdf";
                        attachedFile.FileContent = fileBytes;
                        // 👇 Xóa file sau khi lưu vào DB
                        try
                        {
                            System.IO.File.Delete(absolutePath);
                        }
                        catch (Exception ex)
                        {
                            // Ghi log nếu cần
                            Console.WriteLine($"Không thể xoá file tạm: {ex.Message}");
                        }
                    }
                    else
                    {
                        return new CommonResponse { Status = "error", Message = "Không tìm thấy file scan trên hệ thống." };
                    }
                }
                //else
                //{
                //    return new CommonResponse { Status = "error", Message = "Không có file đính kèm hoặc file scan để lưu." };
                //}
                _dbContext.AttachedFiles.Add(attachedFile);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success" };
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                {
                    msg += " -> " + ex.InnerException.Message;
                }
                return new CommonResponse { Status = "error", Message = "Đã xảy ra lỗi khi cập nhật dữ liệu: " + msg };
            }
        }

        public async Task RemoveDatarRedundantAsync()
        {
            var user = _authService.GetUserInfo();
            if (user == null || user.Id == Guid.Empty)
            {
                return; // Không làm gì nếu user không hợp lệ
            }
            await _dbContext.AttachedFiles
                .Where(t => t.Status != "XD" && t.CreatedBy == user.Id)
                .ExecuteDeleteAsync();
        }

        public async Task RemoveDataRedundantAsync(string tableName)
        {
            var user = _authService.GetUserInfo();
            if (user == null || user.Id == Guid.Empty)
            {
                return; // Không làm gì nếu user không hợp lệ
            }
            await _dbContext.AttachedFiles
                .Where(t => t.TableName == tableName && t.Status != "XD" && t.CreatedBy == user.Id)
                .ExecuteDeleteAsync();
        }

        public async Task<AttachedFile> EditAsync(Guid guid)
        {
            var data = await _dbContext.AttachedFiles.FindAsync(guid);
            return data ?? new AttachedFile();
        }

        public async Task<CommonResponse> UpdateAsync(AttachedFile request)
        {
            try
            {
                var data = await _dbContext.AttachedFiles.FindAsync(request.Id);
                if (data == null)
                {
                    return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin dữ liệu cần cập nhật!" };
                }
                if (request.FileUpLoad != null && request.FileUpLoad.Length != 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        await request.FileUpLoad.CopyToAsync(ms);
                        data.FileName = request.FileUpLoad.FileName;
                        var contentType = request.FileUpLoad.ContentType ?? "";
                        data.ContentType = contentType.Length > 50 ? contentType.Substring(0, 50) : contentType;
                        data.FileContent = ms.ToArray();
                    }
                }
                data.MoTa = request.MoTa;
                data.Url = request.Url;
                data.Public = request.Public;
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

        public async Task RemoveRangeByGroupId(Guid guid)
        {
            await _dbContext.AttachedFiles
                    .Where(t => t.GroupId == guid)
                    .ExecuteDeleteAsync();
        }

        public async Task UpdateRangeStatus(Guid guid, string tableName)
        {
            await _dbContext.AttachedFiles
                    .Where(t => t.GroupId == guid && t.TableName == tableName)
                    .ExecuteUpdateAsync(setters => setters.SetProperty(t => t.Status, "XD"));
        }
    }
}
