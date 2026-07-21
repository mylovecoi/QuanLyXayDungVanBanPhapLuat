using DataAccess;
using DataAccess.Entities.Settings;
using DataAccess.Entities.Systems;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Services.Helpers;
using Services.Model;
using Services.Settings.DanhMucDungChung.DmHopDong;
using Services.Systems;

namespace Services.Settings.DanhMucDungChung
{
    public interface IDanhMucPhiLePhiService
    {
        Task<CommonResponse> GetListDanhMucPhiLePhiAsync(string Search, int PageSize, int PageCurrent, Guid loaiNghiepVu);

        Task<CommonResponse> StoreAsync(DanhMucPhiLePhi request);

        Task<CommonResponse> EditAsync(Guid id);

        Task<CommonResponse> UpdateAsync(DanhMucPhiLePhi request);

        Task<CommonResponse> DeleteAsync(Guid id);

        Task<List<SelectListItem>> GetListDanhMucHopDong();

        Task<List<SelectListItem>> GetListDanhMucHopDong(Guid idHopDong);

        Task<List<DanhMucPhiLePhi>> GetListDanhMucPhiLePhiByLoaiHopDongId(Guid idHopDong);

        Task<List<SelectListItem>> GetListPhanLoaiPhiLePhi();
    }

    public class DanhMucPhiLePhiService(ApplicationDbContext dbContext, IDmHopDongService dmHopDongService, IOptionDataService optionDataService) : IDanhMucPhiLePhiService
    {
        private readonly ApplicationDbContext _dbContext = dbContext;
        private readonly IDmHopDongService _dmHopDongService = dmHopDongService;
        private readonly IOptionDataService _optionDataService = optionDataService;

        public async Task<CommonResponse> GetListDanhMucPhiLePhiAsync(string Search, int PageSize, int PageCurrent, Guid loaiNghiepVu)
        {
            var query = _dbContext.DanhMucPhiLePhis.Include(x => x.LoaiHopDong).Include(x => x.PhanLoai).AsQueryable();

            if (Search != string.Empty)
            {
                Search = Search.ToLower();
                query = query.Where(x => x.MoTa.ToLower().Contains(Search));
            }

            if (loaiNghiepVu != Guid.Empty)
            {
                query = query.Where(x => x.LoaiHopDongId == loaiNghiepVu);
            }

            var dataView = await query.Skip((PageCurrent - 1) * PageSize).Take(PageSize).ToListAsync();

            return new CommonResponse { Status = "success", Data = dataView, TotalRecord = await query.CountAsync() };
        }

        public async Task<CommonResponse> StoreAsync(DanhMucPhiLePhi request)
        {
            try
            {
                _dbContext.DanhMucPhiLePhis.Add(request);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success", Message = "Thêm danh mục phí lệ phí thành công" };
            }
            catch
            {
                return new CommonResponse { Status = "error", Message = "Không thể thêm danh mục phí lệ phí" };
            }
        }

        public async Task<CommonResponse> EditAsync(Guid id)
        {
            try
            {
                var data = await GetDanhMucPhiLePhiById(id);

                data.strPhiCoDinh = FunctionHelper.ConvertDblToStr((double)data.PhiCoDinh);
                data.strPhiToiDa = FunctionHelper.ConvertDblToStr((double)data.PhiToiDa);
                data.strNguongVuotMuc = FunctionHelper.ConvertDblToStr((double)data.NguongVuotMuc);
                data.strTyLeVuotMuc = FunctionHelper.ConvertDblToStr((double)data.TyLeVuotMuc);

                return new CommonResponse { Status = "success", Data = data };
            }
            catch (Exception ex)
            {
                return new CommonResponse { Status = "error", Message = ex.Message };
            }
        }

        public async Task<CommonResponse> UpdateAsync(DanhMucPhiLePhi request)
        {
            try
            {
                _dbContext.DanhMucPhiLePhis.Update(request);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success", Message = "Sửa danh mục phí lệ phí thành công" };
            }
            catch
            {
                return new CommonResponse { Status = "error", Message = "Không thể sửa danh mục phí lệ phí" };
            }
        }

        public async Task<CommonResponse> DeleteAsync(Guid id)
        {
            try
            {
                var model = await GetDanhMucPhiLePhiById(id);

                _dbContext.DanhMucPhiLePhis.Remove(model);
                await _dbContext.SaveChangesAsync();

                return new CommonResponse { Status = "success", Message = "Xoá danh mục phí lệ phí thành công" };
            }
            catch
            {
                return new CommonResponse { Status = "error", Message = "Không thể xoá danh mục phí lệ phí" };
            }
        }

        public async Task<List<SelectListItem>> GetListDanhMucHopDong()
        {
            List<DanhMucHopDong> data = (await _dmHopDongService.GetListByFilterAsync(new(1000, null))).Data ?? new List<DanhMucHopDong>();

            var result = new List<SelectListItem>();

            foreach (var item in data)
            {
                if (item.Level == 0)
                {
                    result.Add(new SelectListItem
                    {
                        Value = item.Id.ToString(),
                        Text = FunctionHelper.GetBackIn(item.Level) + item.TenHopDong,
                        Disabled = true
                    });
                }
                else if (item.Level == 1)
                {
                    result.Add(new SelectListItem
                    {
                        Value = item.Id.ToString(),
                        Text = FunctionHelper.GetBackIn(item.Level) + item.TenHopDong,
                        Disabled = false
                    });
                }
            }

            return result;
        }

        public async Task<List<SelectListItem>> GetListDanhMucHopDong(Guid idHopDong)
        {
            List<DanhMucHopDong> data = (await _dmHopDongService.GetListByFilterAsync(new(1000, null))).Data ?? new List<DanhMucHopDong>();

            var result = new List<SelectListItem>();

            foreach (var item in data)
            {
                if (item.Level == 0)
                {
                    result.Add(new SelectListItem
                    {
                        Value = item.Id.ToString(),
                        Text = FunctionHelper.GetBackIn(item.Level) + item.TenHopDong,
                        Disabled = true
                    });
                }
                else if (item.Level == 1)
                {
                    if (item.Id == idHopDong)
                    {
                        result.Add(new SelectListItem
                        {
                            Value = item.Id.ToString(),
                            Text = FunctionHelper.GetBackIn(item.Level) + item.TenHopDong,
                            Disabled = false,
                            Selected = true
                        });
                    }
                    else
                    {

                        result.Add(new SelectListItem
                        {
                            Value = item.Id.ToString(),
                            Text = FunctionHelper.GetBackIn(item.Level) + item.TenHopDong,
                            Disabled = false,
                            Selected = false
                        });
                    }
                }
            }

            return result;
        }

        public async Task<List<DanhMucPhiLePhi>> GetListDanhMucPhiLePhiByLoaiHopDongId(Guid idHopDong)
        {
            return await _dbContext.DanhMucPhiLePhis.Where(x => x.LoaiHopDongId == idHopDong).ToListAsync();
        }

        public async Task<List<SelectListItem>> GetListPhanLoaiPhiLePhi()
        {
            List<OptionData> data = await _optionDataService.GetDataOptionsByCodeAsync("PhiLePhi");

            var result = new List<SelectListItem>();

            foreach (var item in data)
            {
                result.Add(new SelectListItem
                {
                    Value = item.Id.ToString(),
                    Text = item.DisplayName
                });
            }

            return result;
        }

        private async Task<DanhMucPhiLePhi> GetDanhMucPhiLePhiById(Guid id)
        {
            return await _dbContext.DanhMucPhiLePhis.FindAsync(id) ?? throw new Exception("Không tìm thấy thông tin danh mục phí lệ phí!");
        }
    }
}
