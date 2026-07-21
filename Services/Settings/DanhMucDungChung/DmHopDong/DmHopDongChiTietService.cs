using DataAccess;
using DataAccess.Entities.Settings;
using Microsoft.EntityFrameworkCore;
using Services.Model;

namespace Services.Settings.DanhMucDungChung.DmHopDong
{
    public class DmHopDongChiTietService : IDmHopDongChiTietService
    {
        private readonly ApplicationDbContext _dbContext;
        public DmHopDongChiTietService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<CommonResponse> GetListByDanhMucIdAsync(Guid danhMucId)
        {
            var data = await _dbContext.DanhMucHopDongChiTiets.AsNoTracking()
                .Where(x => x.DanhMucHopDongId == danhMucId)
                .OrderBy(x => x.Order)
                .ThenBy(x => x.Title)
                .ToListAsync();
            return new("success", "Lấy dữ liệu thành công", data, data.Count);
        }

        public async Task<ICollection<DanhMucHopDongChiTiet>> GetRawListByDanhMucIdAsync(Guid danhMucId) =>
            await _dbContext.DanhMucHopDongChiTiets.AsNoTracking()
                .Where(x => x.DanhMucHopDongId == danhMucId)
                .OrderBy(x => x.Order).ThenBy(x => x.Title)
                .ToListAsync();

        public async Task<DanhMucHopDongChiTiet?> GetByIdAsync(Guid id)
        {
            return await _dbContext.DanhMucHopDongChiTiets.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<CommonResponse> StoreAsync(DanhMucHopDongChiTiet request)
        {
            try
            {
                // Validate request
                var validationResult = await ValidateFieldAsync(request, isUpdate: false);
                if (validationResult.Status == "error")
                    return validationResult;

                // Handle order swapping if needed
                await HandleOrderSwappingAsync(request.DanhMucHopDongId, request.Order, Guid.Empty);

                await _dbContext.DanhMucHopDongChiTiets.AddAsync(request);
                await _dbContext.SaveChangesAsync();
                return new("success", "Thêm mới trường thành công");
            }
            catch (Exception ex)
            {
                return new("error", ex.Message);
            }
        }

        public async Task<CommonResponse> UpdateAsync(DanhMucHopDongChiTiet request)
        {
            try
            {
                // Validate request
                var validationResult = await ValidateFieldAsync(request, isUpdate: true);
                if (validationResult.Status == "error")
                    return validationResult;

                // Handle order swapping if needed
                await HandleOrderSwappingAsync(request.DanhMucHopDongId, request.Order, request.Id);

                _dbContext.DanhMucHopDongChiTiets.Update(request);
                await _dbContext.SaveChangesAsync();
                return new("success", "Cập nhật trường thành công");
            }
            catch (Exception ex)
            {
                return new("error", ex.Message);
            }
        }

        private async Task<CommonResponse> ValidateFieldAsync(DanhMucHopDongChiTiet request, bool isUpdate)
        {
            // Validate Title (required and unique within contract)
            if (string.IsNullOrWhiteSpace(request.Title))
                return new("error", "Tiêu đề trường là bắt buộc");

            var existingTitle = await _dbContext.DanhMucHopDongChiTiets.AsNoTracking()
                .FirstOrDefaultAsync(x => x.DanhMucHopDongId == request.DanhMucHopDongId
                    && EF.Functions.Like(x.Title, request.Title)
                    && (!isUpdate || x.Id != request.Id));

            if (existingTitle != null)
                return new("error", $"Tiêu đề '{request.Title}' đã được sử dụng cho trường khác trong hợp đồng này");

            // Validate Order (must be >= 1)
            if (request.Order < 1)
                return new("error", "Thứ tự phải từ 1 trở lên");

            // Validate ColSize (1-12)
            if (request.ColSize < 1 || request.ColSize > 12)
                return new("error", "Độ rộng cột phải từ 1 đến 12");

            // Validate OptionData Code for select/radio/checkbox types
            var needsOptionData = new[] { FieldType.Select, FieldType.Radio, FieldType.Checkbox };
            if (needsOptionData.Contains(request.Type))
            {
                if (string.IsNullOrWhiteSpace(request.Code))
                    return new("error", "Mã OptionData là bắt buộc");
            }
            else
            {
                request.Code = null;
            }

            return new("success");
        }

        private async Task HandleOrderSwappingAsync(Guid danhMucHopDongId, int newOrder, Guid excludeId)
        {
            // Find existing field with the same order
            var existingField = await _dbContext.DanhMucHopDongChiTiets
                .FirstOrDefaultAsync(x => x.DanhMucHopDongId == danhMucHopDongId
                    && x.Order == newOrder
                    && x.Id != excludeId);

            if (existingField != null)
            {
                // Find next available order number
                var maxOrder = await _dbContext.DanhMucHopDongChiTiets
                    .Where(x => x.DanhMucHopDongId == danhMucHopDongId && x.Id != excludeId)
                    .MaxAsync(x => (int?)x.Order) ?? 0;

                existingField.Order = maxOrder + 1;
                _dbContext.DanhMucHopDongChiTiets.Update(existingField);
            }
        }

        public async Task<CommonResponse> DeleteAsync(Guid id)
        {
            try
            {
                var entity = await _dbContext.DanhMucHopDongChiTiets.FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null) return new("error", "Không tìm thấy trường. Hãy kiểm tra lại!");
                _dbContext.DanhMucHopDongChiTiets.Remove(entity);
                await _dbContext.SaveChangesAsync();
                return new("success", "Xóa trường thành công");
            }
            catch (Exception ex)
            {
                return new("error", ex.Message);
            }
        }
    }
}



