using DataAccess;
using DataAccess.Entities.Systems;
using Microsoft.EntityFrameworkCore;
using Services.Model;

namespace Services.Systems
{
    public interface IOptionDataService
    {
        Task<CommonResponse> GetDataOptionAsync(string Search, int PageSize, int PageCurrent);
        Task<CommonResponse> StoreAsync(OptionData request);
        Task<CommonResponse> EditAsync(Guid guid);
        Task<CommonResponse> UpdateAsync(OptionData request);
        Task<CommonResponse> DeleteAsync(Guid guid);
        Task<List<OptionData>> GetDataOptionsByCodeAsync(string Code);
        Task<OptionData?> GetOptionDataByCodeAndIdAsync(string Code, Guid id);
    }
    public class OptionDataService : IOptionDataService
    {
        private readonly ApplicationDbContext _dbContext;
        public OptionDataService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }       

        public async Task<CommonResponse> GetDataOptionAsync(string Search, int PageSize, int PageCurrent)
        {
            var data = _dbContext.OptionDatas.AsQueryable();
            if (!string.IsNullOrEmpty(Search))
            {
                data = data.Where(x => (x.Code != null && x.Code.ToLower().Contains(Search.ToLower())) ||
                                    (x.Value != null && x.Value.ToLower().Contains(Search.ToLower())) ||
                                    (x.DisplayName != null && x.DisplayName.ToLower().Contains(Search.ToLower())));
            }
            var dataView = await data.OrderBy(t=>t.Code).Skip((PageCurrent - 1) * PageSize).Take(PageSize).ToListAsync();
            return new CommonResponse { Status = "success", Data = dataView, TotalRecord = data.Count() };
        }

        public async Task<CommonResponse> StoreAsync(OptionData request)
        {
            try
            {
                _dbContext.OptionDatas.Add(request);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success" };
            }
            catch
            {
                return new CommonResponse { Status = "error", Message = "Lỗi không xác định" };
            }
        }

        public async Task<CommonResponse> EditAsync(Guid guid)
        {
            try
            {
                var data = await _dbContext.OptionDatas.FindAsync(guid);
                if (data == null)
                {
                    return new CommonResponse { Status = "error", Message = "Không tìm thấy dữ liệu" };
                }                
                return new CommonResponse { Status = "success", Data = data };
            }
            catch
            {
                return new CommonResponse { Status = "error", Message = "Lỗi không xác định" };
            }
        }

        public async Task<CommonResponse> UpdateAsync(OptionData request)
        {
            try
            {
                _dbContext.OptionDatas.Update(request);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success" };
            }
            catch
            {
                return new CommonResponse { Status = "error", Message = "Lỗi không xác định" };
            }
        }

        public async Task<CommonResponse> DeleteAsync(Guid guid)
        {
            try
            {   var data = await _dbContext.OptionDatas.FindAsync(guid);
                if(data == null)
                {
                    return new CommonResponse { Status = "error", Message = "Không tìm thấy dữ liệu" };
                }
                _dbContext.OptionDatas.Remove(data);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success" };
            }
            catch
            {
                return new CommonResponse { Status = "error", Message = "Lỗi không xác định" };
            }
        }

        public async Task<List<OptionData>> GetDataOptionsByCodeAsync(string Code)
        {
            var data = await _dbContext.OptionDatas.Where(x => x.Code == Code).ToListAsync();
            if (data != null)
            {
                return data;
            }
            else
            {
                return new List<OptionData>();
            }
        }

        public async Task<OptionData?> GetOptionDataByCodeAndIdAsync(string Code, Guid id) => await _dbContext.OptionDatas.FirstOrDefaultAsync(x => x.Code == Code && x.Id == id);
    }
}
