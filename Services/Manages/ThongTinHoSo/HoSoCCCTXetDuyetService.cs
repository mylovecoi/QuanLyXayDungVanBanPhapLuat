using DataAccess;
using DataAccess.Entities.Manages.ThongTinHoSo;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Services.DTOs.Manages.ThongTinHoSo;
using Services.Helpers;
using Services.Model;
using Services.Settings;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Services.Manages.ThongTinHoSo
{
    public interface IHoSoCCCTXetDuyetService
    {
        Task<CommonResponse> GetListByFilterAsync(HoSoFilter filter);
        Task<CommonResponse> TraLaiAsync(Guid hopDongId, string lyDo);
        Task<CommonResponse> ChangeStatusAsync(Guid hopDongId, string status, Guid congChungVienId = default(Guid)); // ChangeCTN => CTT, CTT => XL
        Task<CommonResponse> XacNhanThanhToanAsync(Guid hopDongId, DateTime ngayThanhToan);
        Task<CommonResponse> HoanThanhAsync(Guid hopDongId, string soQD, DateTime ngayQD, IFormFile chuKyDienTy, bool isHoanThanh = true);
        Task<CommonResponse> GetChiPhiHoSoAsync(Guid hopDongId);
    }

    public class HoSoCCCTXetDuyetService(
        ApplicationDbContext dbContext,
        IHoSoCCCTService hoSoCCCTService,
        IDanhMucDonViService danhMucDonViService,
        Settings.IDanhMucCanBoService danhMucCanBoService
        ) : IHoSoCCCTXetDuyetService
    {
        private readonly ApplicationDbContext _dbContext = dbContext;
        private readonly IHoSoCCCTService _hoSoCCCTService = hoSoCCCTService;
        private readonly IDanhMucDonViService _danhMucDonViService = danhMucDonViService;
        private readonly Settings.IDanhMucCanBoService _danhMucCanBoService = danhMucCanBoService;

        public async Task<CommonResponse> GetListByFilterAsync(HoSoFilter filter)
        {
            try
            {
                List<string> listStatus = new List<string> { "BTL", "CTN", "CTT", "XL", "HT" };

                IQueryable<HoSoCCCT> queryable = _dbContext.HoSoCCCTs
                    .Include(x => x.LoaiHopDong).AsNoTracking()
                    .Where(x => x.DonViQuanLyId == filter.DonViId)
                    .Where(x => x.Status != null && listStatus.Contains(x.Status));

                var tinhNangThanhToan = await _danhMucDonViService.GetTinhNangThanhToanStatusAsync() ?? false;

                if (!tinhNangThanhToan) queryable = queryable.Where(x => x.Status != "CTT");

                if (filter.TargetYear > 0) queryable = queryable.Where(x => x.NgayThuLy.Year == filter.TargetYear);

                if (!string.IsNullOrWhiteSpace(filter.Status)) queryable = queryable.Where(x => x.Status == filter.Status);

                if (!string.IsNullOrEmpty(filter.Search)) queryable = queryable.Where(x => (x.MaSoHoSo ?? string.Empty).Contains(filter.Search));


                queryable = queryable.OrderByDescending(x => x.Status == "HT" ? x.NgayDuyet : x.NgayChuyen).ThenBy(x => x.MaSoHoSo);

                int totalRecord = await queryable.CountAsync();
                filter.AdjustPageIfInvalid(totalRecord);
                var dataView = queryable.Skip((filter.PageCurrent - 1) * filter.PageSize).Take(filter.PageSize).ToList();

                return new("success", "Lấy thông tin hồ sơ thành công", dataView, totalRecord);
            }
            catch (Exception)
            {
                return new("error", "Có lỗi trong quá trình cập nhật dữ liệu. Hãy kiểm tra lại!");
            }
        }

        public async Task<CommonResponse> TraLaiAsync(Guid hopDongId, string lyDo)
        {
            try
            {
                if (string.IsNullOrEmpty(lyDo)) return new("error", "Lý do trả lại hồ sơ không được để trống.");

                var existingHoSo = await _hoSoCCCTService.GetEntityByIdAsync(hopDongId, isAsNoTracking: false);
                if (existingHoSo == null) return new("error", "Không tìm thấy thông tin hồ sơ. Hãy kiểm tra lại!");

                if (existingHoSo.Status != "CTN") return new("error", "Hồ sơ đã đươc xử lý. Không thể trả lại.");

                existingHoSo.Status = "BTL";
                existingHoSo.LyDoTraLai = lyDo.Trim();
                existingHoSo.NgayChuyen = DateTime.Now;

                _dbContext.HoSoCCCTs.Update(existingHoSo);
                await _dbContext.SaveChangesAsync();

                return new("success", $"Trả lại hồ sơ: {existingHoSo.MaSoHoSo} thành công", existingHoSo.Status);
            }
            catch (Exception)
            {
                return new("error", "Có lỗi trong quá trình cập nhật dữ liệu. Hãy kiểm tra lại!");
            }
        }

        public async Task<CommonResponse> ChangeStatusAsync(Guid hopDongId, string status, Guid congChungVienId = default(Guid)) // ChangeCTN => CTT, CTT => XL
        {
            try
            {
                var existingHoSo = await _hoSoCCCTService.GetEntityByIdAsync(hopDongId, isAsNoTracking: false);
                if (existingHoSo == null) return new("error", "Không tìm thấy thông tin hồ sơ. Hãy kiểm tra lại!");

                if (existingHoSo.Status == "HT") return new("error", "Hồ sơ đã hoàn thành. Không thể chuyển trạng thái.");

                //if (existingHoSo.DonViQuanLy == null) return new("error", "Hồ sơ không hợp lệ. Không thể chuyển trạng thái.");

                // Cập nhât thông tin nếu status existingHoSo.Status == "CTN" thêm thông tin cán bộ công chứng
                if (existingHoSo.Status == "CTN")
                {
                    var canBoTiepNhan = await _danhMucCanBoService.EditAsync(congChungVienId);

                    if (canBoTiepNhan.Status == "error") return new("error", "Không tìm thấy thông tin công chứng viên. Hãy kiểm tra lại.");
                    if (canBoTiepNhan.Data == null) return new("error", "Không tìm thấy thông tin công chứng viên. Hãy kiểm tra lại.");

                    existingHoSo.CongChungVienId = canBoTiepNhan.Data.Id;
                }

                // cập nhật thông tin là nếu user/ đơn vị ko tính phí thì CTN => Xl bỏ qua CTT
                if (ShouldSkipCTT(existingHoSo)) status = "XL";

                existingHoSo.Status = status;
                existingHoSo.NgayChuyen = DateTime.Now;
                existingHoSo.LyDoTraLai = string.Empty;

                _dbContext.HoSoCCCTs.Update(existingHoSo);
                await _dbContext.SaveChangesAsync();

                if (status == "CTT")
                    return new("success", $"Tiếp nhận hồ sơ: {existingHoSo.MaSoHoSo} thành công", existingHoSo.Status);
                else
                    return new("success", $"Xác nhận thông tin hồ sơ: {existingHoSo.MaSoHoSo} thành công", existingHoSo.Status);
            }
            catch (Exception)
            {
                return new("error", "Có lỗi trong quá trình cập nhật dữ liệu. Hãy kiểm tra lại!");
            }
        }

        private bool ShouldSkipCTT(HoSoCCCT hoSo)
        {
            return hoSo.Status == "CTN" && hoSo.DonViQuanLy?.TinhNangThanhToan == false;
        }

        public async Task<CommonResponse> XacNhanThanhToanAsync(Guid hopDongId, DateTime ngayThanhToan)
        {
            try
            {
                if (ngayThanhToan > DateTime.Now) return new("error", "Ngày quyết định phải nhỏ hơn thời điểm hiện tại.");

                var existingHoSo = await _hoSoCCCTService.GetEntityByIdAsync(hopDongId, isAsNoTracking: false);
                if (existingHoSo == null) return new("error", "Không tìm thấy thông tin hồ sơ. Hãy kiểm tra lại!");

                if (existingHoSo.Status != "CTT") return new("error", "Hồ sơ không ở trạng thái chờ thanh toán. Không thể xử lý.");

                existingHoSo.DaThanhToan = true;
                existingHoSo.NgayThanhToan = ngayThanhToan;
                existingHoSo.Status = "XL";

                _dbContext.HoSoCCCTs.Update(existingHoSo);
                await _dbContext.SaveChangesAsync();

                return new("success", $"Xác nhận hồ sơ: {existingHoSo.MaSoHoSo} thành công", existingHoSo.Status);
            }
            catch (Exception)
            {
                return new("error", "Có lỗi trong quá trình cập nhật dữ liệu. Hãy kiểm tra lại!");
            }
        }

        public async Task<CommonResponse> HoanThanhAsync(Guid hopDongId, string soQD, DateTime ngayQD, IFormFile fileChuKyDienTu, bool isHoanThanh = true)
        {
            try
            {
                if (isHoanThanh)
                {
                    if (string.IsNullOrEmpty(soQD)) return new("error", "Số quyết định phê duyệt hồ sơ không được để trống.");
                    if (ngayQD > DateTime.Now) return new("error", "Ngày quyết định không được lớn hơn thời điểm hiện tại.");
                    if (fileChuKyDienTu == null) return new("error", "Chữ ký điện tử không được để trống.");

                }
                var existingHoSo = await _hoSoCCCTService.GetEntityByIdAsync(hopDongId, isAsNoTracking: false);
                if (existingHoSo == null) return new("error", "Không tìm thấy thông tin hồ sơ. Hãy kiểm tra lại!");

                if (isHoanThanh && existingHoSo.Status != "XL")
                    return new("error", "Hồ sơ không ở trạng thái xử lý. Không thể hoàn thành.");

                if (!isHoanThanh && existingHoSo.Status != "HT")
                    return new("error", "Hồ sơ không ở trạng thái hoàn thành. Không thể hủy hoàn thành.");

                await using var transaction = await _dbContext.Database.BeginTransactionAsync();

                if (isHoanThanh)
                {
                    existingHoSo.Status = "HT";
                    existingHoSo.SoQDDuyet = soQD;
                    existingHoSo.NgayDuyet = ngayQD;

                    _dbContext.HoSoCCCTs.Update(existingHoSo);

                    var fileCKDTExisting = await _dbContext.AttachedFiles.FirstOrDefaultAsync(x => x.TableName == "ChuKyDienTu" && x.GroupId == existingHoSo.Id);
                    if (fileCKDTExisting == null) // thêm mới file
                    {
                        fileCKDTExisting = new()
                        {
                            GroupId = existingHoSo.Id,
                            TableName = "ChuKyDienTu",
                            Status = "XD", // hoặc gì bạn quy định
                            MoTa = "Chữ Ký Điện Tử"
                        };

                        await _dbContext.AttachedFiles.AddAsync(fileCKDTExisting);
                    }
                    else // cập nhật file
                    {
                        _dbContext.AttachedFiles.Update(fileCKDTExisting);
                    }

                    using (var ms = new MemoryStream())
                    {
                        await fileChuKyDienTu.CopyToAsync(ms);
                        fileCKDTExisting.FileName = fileChuKyDienTu.FileName;
                        fileCKDTExisting.ContentType = fileChuKyDienTu.ContentType;
                        fileCKDTExisting.FileContent = ms.ToArray();
                    }
                }
                else
                {
                    existingHoSo.Status = "XL";
                    existingHoSo.SoQDDuyet = null;
                    existingHoSo.NgayDuyet = DateTime.MinValue;
                    existingHoSo.NgayChuyen = DateTime.Now;

                    _dbContext.HoSoCCCTs.Update(existingHoSo);
                }

                await _dbContext.SaveChangesAsync();

                await transaction.CommitAsync();

                return new("success", isHoanThanh
                    ? $"Hoàn thành hồ sơ: {existingHoSo.MaSoHoSo} thành công"
                    : $"Đã hoàn t hành huỷ hồ sơ: {existingHoSo.MaSoHoSo}",
                    existingHoSo.Status);
            }
            catch (DbUpdateException)
            {
                return new("error", "Có lỗi trong quá trình lưu dữ liệu. Hãy kiểm tra lại!");
            }
            catch (Exception)
            {
                return new("error", "Có lỗi không xác định xảy ra. Hãy kiểm tra lại!");
            }
        }

        public async Task<CommonResponse> GetChiPhiHoSoAsync(Guid hopDongId)
        {
            try
            {
                var sumChiPhi = await _dbContext.HoSoCCCTChiPhis
                    .Where(x => x.HoSoId == hopDongId)
                    .SumAsync(x => (double?)x.ThanhTien) ?? 0;

                return new("success", "Lấy dữ liệu thành công.", FunctionHelper.ConvertDblToStr(sumChiPhi));
            }
            catch (Exception)
            {
                return new("error", "Có lỗi trong quá trình lấy dữ liệu. Hãy kiểm tra lại!");
            }
        }
    }
}
