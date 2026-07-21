using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using Azure;
using DataAccess;
using DataAccess.Entities.Manages.ThongTinHoSo;
using DataAccess.Entities.Settings;
using Microsoft.EntityFrameworkCore;
using Services.DTOs.Manages.ThongTinHoSo;
using Services.DTOs.Manages.ThongTinHoSo.ExportData;
using Services.DTOs.Settings.DanhMucDungChung;
using Services.Helpers;
using Services.Model;
using Services.Settings;
using Services.Settings.DanhMucDungChung;
using Services.Settings.DanhMucDungChung.DmHopDong;
using Services.Systems;

namespace Services.Manages.ThongTinHoSo
{
    public interface IHoSoCCCTService
    {
        Task<CommonResponse> GetListByFilterAsync(HoSoFilter filter);
        Task<CommonResponse> AdvancedSearchHoSoAsync(HoSoFilter filter);
        Task<HoSoCCCT?> GetEntityByMaAsync(string maSoHoSo);
        Task<HoSoCCCT?> GetEntityByIdAsync(Guid hoSoId, bool isAsNoTracking = true, bool isInClude = true);
        Task<CommonResponse> ChuyenAsync(Guid hoSoId);
        Task<List<HoSoCCCTChiPhi>> GetListLePhiByHoSoIdAsync(Guid idHoSo);
        Task<CommonResponse> EditLePhiAsync(Guid idLePhi);
        Task<CommonResponse> UpdateLePhiAsync(HoSoCCCTChiPhi request, double giaTriHopDong);
        Task RemoveHoSoDataRedundantAsync();
    }

    public class HoSoCCCTService(
        ApplicationDbContext dbContext,
        IAuthService authService,
        IDmHopDongService danhMucHopDongService,
        IDanhMucDonViService danhMucDonViService
        ) : IHoSoCCCTService
    {
        private readonly ApplicationDbContext _dbContext = dbContext;
        private readonly IAuthService _authService = authService;
        private readonly IDmHopDongService _danhMucHopDongService = danhMucHopDongService;
        private readonly IDanhMucDonViService _danhMucDonViService = danhMucDonViService;

        public async Task<CommonResponse> GetListByFilterAsync(HoSoFilter filter)
        {
            try
            {
                IQueryable<HoSoCCCT> queryable = _dbContext.HoSoCCCTs
                    .Include(x => x.LoaiHopDong)
                    .Include(x => x.CongChungVien)
                    .AsNoTracking()
                    .Where(x => x.DonViQuanLyId == filter.DonViId && x.Status != "CXD");

                var tinhNangThanhToan = await _danhMucDonViService.GetTinhNangThanhToanStatusAsync() ?? false;

                if (!tinhNangThanhToan) queryable = queryable.Where(x => x.Status != "CTT");

                if (filter.Status == "HT") queryable = queryable.Where(x => x.Status == "HT");

                //if (filter.LoaiNghiepVu.HasValue)
                //queryable = queryable.Where(x => x.LoaiHopDong != null && x.LoaiHopDong.IsCC == filter.LoaiNghiepVu);

                if (filter.TargetYear > 0) queryable = queryable.Where(x => x.NgayThuLy.Year == filter.TargetYear);

                if (filter.LoaiHopDong != Guid.Empty)
                {
                    var loaiexistingHoSo = await _danhMucHopDongService.GetEntityByIdAsync(filter.LoaiHopDong, isAsNoTracking: true);
                    if (loaiexistingHoSo != null && loaiexistingHoSo.Children.Count() > 0)
                    {
                        var listIdChil = loaiexistingHoSo.Children.Select(x => x.Id).ToList();
                        queryable = queryable.Where(x => listIdChil.Contains(x.LoaiHopDongId));
                    }
                    else
                    {
                        queryable = queryable.Where(x => x.LoaiHopDongId == filter.LoaiHopDong);
                    }
                }

                if (!string.IsNullOrEmpty(filter.Search))
                    queryable = queryable.Where(x =>
                        (x.LoaiHopDong != null && EF.Functions.Like(x.LoaiHopDong.TenHopDong, $"%{filter.Search}%")) ||
                        EF.Functions.Like(x.MaSoHoSo, $"%{filter.Search}%") ||
                        (x.GiaTriHopDong.HasValue && EF.Functions.Like(x.GiaTriHopDong.Value.ToString(), $"%{filter.Search}%")) ||
                        EF.Functions.Like(x.NgayThuLy.Year.ToString(), $"%{filter.Search}%") ||
                        EF.Functions.Like(x.HoTenNguoiNop, $"%{filter.Search}%") ||
                        EF.Functions.Like(x.SoCCCDNguoiNop, $"%{filter.Search}%") ||
                        EF.Functions.Like(x.ThongTinDonVi, $"%{filter.Search}%")
                    );

                int totalRecord = await queryable.CountAsync();
                filter.AdjustPageIfInvalid(totalRecord);

                queryable = queryable.OrderByDescending(x => x.NgayThuLy).ThenBy(x => x.MaSoHoSo).ThenBy(x => x.GiaTriHopDong);

                var dataView = queryable.Skip((filter.PageCurrent - 1) * filter.PageSize).Take(filter.PageSize).ToList();

                return new("success", "Lấy thông tin danh mục thành công", dataView, totalRecord);
            }
            catch (Exception)
            {
                return new("error", "Có lỗi trong quá trình lấy dữ liệu. Hãy kiểm tra lại!");
            }
        }

        public async Task<CommonResponse> AdvancedSearchHoSoAsync(HoSoFilter filter)
        {
            try
            {
                IQueryable<HoSoCCCT> queryable = _dbContext.HoSoCCCTs
                    .Include(x => x.LoaiHopDong)
                    .Include(x => x.CongChungVien)
                    .AsNoTracking()
                    .Where(x => x.DonViQuanLyId == filter.DonViId && x.Status != "CXD");

                var tinhNangThanhToan = await _danhMucDonViService.GetTinhNangThanhToanStatusAsync() ?? false;

                if (!tinhNangThanhToan) queryable = queryable.Where(x => x.Status != "CTT");

                if (filter.Status == "HT") queryable = queryable.Where(x => x.Status == "HT");

                if (filter.TargetYear > 0) queryable = queryable.Where(x => x.NgayThuLy.Year == filter.TargetYear);

                if (filter.LoaiHopDong != Guid.Empty)
                {
                    var loaiexistingHoSo = await _danhMucHopDongService.GetEntityByIdAsync(filter.LoaiHopDong, isAsNoTracking: true);
                    if (loaiexistingHoSo != null && loaiexistingHoSo.Children.Count() > 0)
                    {
                        var listIdChil = loaiexistingHoSo.Children.Select(x => x.Id).ToList();
                        queryable = queryable.Where(x => listIdChil.Contains(x.LoaiHopDongId));
                    }
                    else
                    {
                        queryable = queryable.Where(x => x.LoaiHopDongId == filter.LoaiHopDong);
                    }
                }

                if (filter.LoaiCongChung != null)
                    queryable = queryable.Where(x => x.PhuongThucCongChung == filter.LoaiCongChung);

                if (filter.NgayYeuCauTu != null)
                    queryable = queryable.Where(x => filter.NgayYeuCauTu.Value.Date <= x.NgayThuLy.Date);

                if (filter.NgayYeuCauDen != null)
                    queryable = queryable.Where(x => x.NgayThuLy.Date <= filter.NgayYeuCauDen.Value.Date);

                if (filter.NgayCongChungTu != null)
                    queryable = queryable.Where(x => filter.NgayCongChungTu.Value.Date <= x.NgayDuyet.Date);

                if (filter.NgayCongChungDen != null)
                    queryable = queryable.Where(x => x.NgayDuyet.Date <= filter.NgayCongChungDen.Value.Date);

                if (!string.IsNullOrEmpty(filter.Search))
                    queryable = queryable.Where(x =>
                        (x.LoaiHopDong != null && EF.Functions.Like(x.LoaiHopDong.TenHopDong, $"%{filter.Search}%")) ||
                        EF.Functions.Like(x.MaSoHoSo, $"%{filter.Search}%") ||
                        (x.GiaTriHopDong.HasValue && EF.Functions.Like(x.GiaTriHopDong.Value.ToString(), $"%{filter.Search}%")) ||
                        EF.Functions.Like(x.NgayThuLy.Year.ToString(), $"%{filter.Search}%") ||
                        EF.Functions.Like(x.HoTenNguoiNop, $"%{filter.Search}%") ||
                        EF.Functions.Like(x.SoCCCDNguoiNop, $"%{filter.Search}%") ||
                        EF.Functions.Like(x.ThongTinDonVi, $"%{filter.Search}%")
                    );

                int totalRecord = await queryable.CountAsync();
                filter.AdjustPageIfInvalid(totalRecord);

                queryable = queryable.OrderByDescending(x => x.NgayThuLy).ThenBy(x => x.MaSoHoSo).ThenBy(x => x.GiaTriHopDong);

                var dataView = queryable.Skip((filter.PageCurrent - 1) * filter.PageSize).Take(filter.PageSize).ToList();

                return new("success", "Lấy thông tin danh mục thành công", dataView, totalRecord);
            }
            catch (Exception)
            {
                return new("error", "Có lỗi trong quá trình lấy dữ liệu. Hãy kiểm tra lại!");
            }
        }

        public async Task<HoSoCCCT?> GetEntityByMaAsync(string maSoHoSo)
        {
            var user = _authService.GetUserInfo();
            var result = await _dbContext.HoSoCCCTs.AsNoTracking().FirstOrDefaultAsync(x => x.MaSoHoSo == maSoHoSo && (user == null || x.DonViQuanLyId == user.DanhMucDonViId));
            return result;
        }

        public async Task<HoSoCCCT?> GetEntityByIdAsync(Guid hoSoId, bool isAsNoTracking = true, bool isInClude = true)
        {
            IQueryable<HoSoCCCT> queryable = _dbContext.HoSoCCCTs.AsQueryable();

            if (isInClude)
            {
                queryable = queryable
                   .Include(x => x.DonViQuanLy)
                   .Include(x => x.LoaiHopDong).ThenInclude(x => x!.Parent)
                   .Include(x => x.LoaiTaiSan)
                   .Include(x => x.DiaBan)
                   .Include(x => x.HoSoCCCTChiPhis)
                   .Include(x => x.HoSoCCCTChiTiets);
            }

            if (isAsNoTracking)
            {
                queryable = queryable
                    .AsNoTracking();
            }

            return await queryable.FirstOrDefaultAsync(x => x.Id == hoSoId);
        }

        public async Task<CommonResponse> ChuyenAsync(Guid hoSoId)
        {
            try
            {
                var existingHoSo = await GetEntityByIdAsync(hoSoId, isAsNoTracking: false);
                if (existingHoSo == null) return new("error", "Không tìm thấy thông tin danh mục. Hãy kiểm tra lại!");

                existingHoSo.Status = "CTN";
                existingHoSo.NgayChuyen = DateTime.Now;
                existingHoSo.LyDoTraLai = null;

                _dbContext.HoSoCCCTs.Update(existingHoSo);
                await _dbContext.SaveChangesAsync();

                return new("success", $"Chuyển hồ sơ: {existingHoSo.MaSoHoSo} thành công");
            }
            catch (Exception)
            {
                return new("error", "Có lỗi trong quá trình cập nhật dữ liệu. Hãy kiểm tra lại!");
            }
        }

        #region Phí Lệ Phí
        public async Task<List<HoSoCCCTChiPhi>> GetListLePhiByHoSoIdAsync(Guid idHoSo)
        {
            return await _dbContext.HoSoCCCTChiPhis.Where(x => x.HoSoId == idHoSo).ToListAsync();
        }

        private async Task<HoSoCCCTChiPhi> GetLePhiHoSoByIdAsync(Guid id)
        {
            return await _dbContext.HoSoCCCTChiPhis.FindAsync(id) ?? throw new Exception("Không tìm thấy thông tin lệ phí hồ sơ!");
        }

        public async Task<CommonResponse> EditLePhiAsync(Guid idLePhi)
        {
            try
            {
                var data = await GetLePhiHoSoByIdAsync(idLePhi);

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

        public async Task<CommonResponse> UpdateLePhiAsync(HoSoCCCTChiPhi request, double giaTriHopDong)
        {
            try
            {
                var model = await GetLePhiHoSoByIdAsync(request.Id);

                if (model.SoLuongToiDa > 0 && request.SoLuong > model.SoLuongToiDa)
                {
                    throw new Exception("Không được vượt quá số lượng tối đa! Số lượng tối đa là " + model.SoLuongToiDa + ".");
                }

                model.SoLuong = request.SoLuong;

                model.ThanhTien = model.SoLuong * (model.PhiCoDinh + model.TyLeVuotMuc * (giaTriHopDong - model.NguongVuotMuc));

                if (model.ThanhTien < 0)
                {
                    throw new Exception("Loại lệ phí này không phù hợp với giá trị hợp đồng!");
                }

                if (model.PhiToiDa > 0 && model.ThanhTien > model.PhiToiDa)
                {
                    model.ThanhTien = model.PhiToiDa;
                }

                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success", Message = "Cập nhật thông tin lệ phí thành công!" };
            }
            catch (Exception ex)
            {
                return new CommonResponse { Status = "error", Message = ex.Message };
            }
        }

        public async Task RemoveHoSoDataRedundantAsync()
        {
            var user = _authService.GetUserInfo();
            if (user == null || user.Id == Guid.Empty)
            {
                return; // Không làm gì nếu user không hợp lệ
            }
            await _dbContext.HoSoCCCTs
                .Where(t => t.Status == "CXD" && t.CreatedBy == user.Id)
                .ExecuteDeleteAsync();
        }
        #endregion
    }
}
