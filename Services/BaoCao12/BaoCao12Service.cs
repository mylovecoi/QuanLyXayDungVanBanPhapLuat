using DataAccess;
using DataAccess.Entities.Manages.ThongTinHoSo;
using DataAccess.Entities.Settings;
using DataAccess.Enums;
using Microsoft.EntityFrameworkCore;
using Services.DTOs.BaoCao12;
using Services.Model;
using Services.Settings;
using Services.Systems;
using static Services.DTOs.BaoCao12.BaoCao12Constants;

namespace Services.BaoCao12
{
    /// <summary>
    /// Interface cho service báo cáo 12 - Tình hình tổ chức và hoạt động công chứng
    /// </summary>
    public interface IBaoCao12Service
    {
        Task<BaoCao12ValidationResult> ValidateRequestAsync(BaoCao12RequestDto request);
        Task<CommonResponse> GetBaoCao12aAsync(BaoCao12RequestDto request);
        Task<CommonResponse> GetBaoCao12bAsync(BaoCao12RequestDto request);
        Task<CommonResponse> ExportBaoCao12ToWordAsync(BaoCao12RequestDto request);
        Task<CommonResponse> ExportBaoCao12ToExcelAsync(BaoCao12RequestDto request);
    }

    public class BaoCao12Service(
        ApplicationDbContext context,
        IAuthService authService,
        IDanhMucDonViService danhMucDonViService) : IBaoCao12Service
    {
        public async Task<BaoCao12ValidationResult> ValidateRequestAsync(BaoCao12RequestDto request)
        {
            var result = new BaoCao12ValidationResult { IsValid = true };

            // Kiểm tra đơn vị tồn tại
            if (request.DonViId == Guid.Empty)
            {
                result.AddError("Vui lòng chọn đơn vị báo cáo");
            }
            else
            {
                var donVi = await context.DanhMucDonVis.FindAsync(request.DonViId);
                if (donVi == null)
                {
                    result.AddError("Đơn vị không tồn tại");
                }
            }

            // Kiểm tra thời gian báo cáo
            if (request.NgayBaoCaoTu > request.NgayBaoCaoDen)
            {
                result.AddError("Ngày bắt đầu không được lớn hơn ngày kết thúc");
            }

            // Kiểm tra kỳ báo cáo hợp lệ
            if (request.KyBaoCao == KyBaoCao12.SauThang)
            {
                var thangBatDau = request.NgayBaoCaoTu.Month;
                var thangKetThuc = request.NgayBaoCaoDen.Month;
                var soThang = thangKetThuc - thangBatDau + 1;

                if (soThang != 6)
                {
                    result.AddWarning("Báo cáo 6 tháng thường tính từ tháng 1-6 hoặc 7-12");
                }
            }
            else if (request.KyBaoCao == KyBaoCao12.Nam || request.KyBaoCao == KyBaoCao12.NamChinhThuc)
            {
                var namBatDau = request.NgayBaoCaoTu.Year;
                var namKetThuc = request.NgayBaoCaoDen.Year;

                if (namBatDau != namKetThuc)
                {
                    result.AddWarning("Báo cáo năm thường tính từ 01/01 đến 31/12");
                }
            }

            // Kiểm tra các trường bắt buộc
            if (string.IsNullOrWhiteSpace(request.NguoiLapBieu))
            {
                result.AddError("Vui lòng nhập tên người lập biểu");
            }

            // Validate theo loại báo cáo
            switch (request.LoaiBaoCao)
            {
                case LoaiBaoCao12.BaoCao12a:
                    ValidateBaoCao12a(request, result);
                    break;
                case LoaiBaoCao12.BaoCao12b:
                    ValidateBaoCao12b(request, result);
                    break;
            }

            return result;
        }

        private void ValidateBaoCao12a(BaoCao12RequestDto request, BaoCao12ValidationResult result)
        {
            // Validate cho báo cáo 12a - chỉ cần người lập biểu
            // Không cần validate các chức danh ký vì sẽ để trống cho thực tế ký ngoài
        }

        private void ValidateBaoCao12b(BaoCao12RequestDto request, BaoCao12ValidationResult result)
        {
            // Validate cho báo cáo 12b - chỉ cần người lập biểu
            // Không cần validate các chức danh ký vì sẽ để trống cho thực tế ký ngoài
        }

        /// <summary>
        /// Lấy dữ liệu báo cáo 12a
        /// </summary>
        public async Task<CommonResponse> GetBaoCao12aAsync(BaoCao12RequestDto request)
        {
            try
            {
                var validation = await ValidateRequestAsync(request);
                if (!validation.IsValid)
                {
                    return new CommonResponse
                    {
                        Status = "error",
                        Message = "Dữ liệu không hợp lệ",
                        Data = validation.ErrorMessages
                    };
                }

                var userInfo = authService.GetUserInfo();
                var donViList = await danhMucDonViService.GetDanhMucDonViByIdAsync(request.DonViId);
                var donVi = donViList?.FirstOrDefault();

                // Tính toán dữ liệu cho Mẫu 12a từ database
                var hoSos = await context.HoSoCCCTs 
                    .AsNoTracking()
                    .Include(x => x.DonViQuanLy)
                    .Include(x => x.LoaiHopDong)
                    .Include(x => x.CongChungVien)
                    .Include(x => x.HoSoCCCTChiPhis)
                    .Where(x => x.DonViQuanLyId == request.DonViId)
                    .Where(x => x.Status == "HT")
                    .Where(date => request.NgayBaoCaoTu.Date <= date.NgayDuyet.Date && date.NgayDuyet.Date <= request.NgayBaoCaoDen.Date)
                    .ToListAsync();

                // Lọc theo danh mục hợp đồng nếu có
                if (request.DanhMucHopDongIds.Any())
                {
                    hoSos = hoSos.Where(x => request.DanhMucHopDongIds.Contains(x.LoaiHopDongId)).ToList();
                }

                // Tính số công chứng viên
                var soCongChungVien = await context.DanhMucCanBos
                    .Where(x => x.DonViQuanLyId == request.DonViId)
                    .CountAsync();

                // Tính số việc công chứng
                var congChungHoSos = hoSos.Where(x => x.LoaiHopDong?.IsCC == true).ToList();
                var chungThucHoSos = hoSos.Where(x => x.LoaiHopDong?.IsCC == false).ToList();

                // Tính các chỉ số cho bảng
                var soViecCongChung = congChungHoSos.Count;
                var congChungHopDong = congChungHoSos.Count(x => x.LoaiHopDong?.TenHopDong?.Contains("hợp đồng") == true);
                var congChungBanDich = congChungHoSos.Count - congChungHopDong;
                
                var chungThucBanSao = chungThucHoSos.Count(x => x.LoaiHopDong?.TenHopDong?.Contains("bản sao") == true);
                var chungThucChuKy = chungThucHoSos.Count(x => x.LoaiHopDong?.TenHopDong?.Contains("chữ ký") == true);
                 
                // Tính tổng chi phí - sửa lỗi conversion
                var tongThuLaoCong = (decimal)hoSos.SelectMany(x => x.HoSoCCCTChiPhis).Sum(x => x.ThanhTien);
                var tongPhiCongChung = (decimal)congChungHoSos.SelectMany(x => x.HoSoCCCTChiPhis).Sum(x => x.ThanhTien);
                var phiChungThucBanSao = (decimal)chungThucHoSos.Where(x => x.LoaiHopDong?.TenHopDong?.Contains("bản sao") == true)
                    .SelectMany(x => x.HoSoCCCTChiPhis).Sum(x => x.ThanhTien);
                var phiChungThucChuKy = (decimal)chungThucHoSos.Where(x => x.LoaiHopDong?.TenHopDong?.Contains("chữ ký") == true)
                    .SelectMany(x => x.HoSoCCCTChiPhis).Sum(x => x.ThanhTien);
                
                // Giả sử thuế = 10% tổng doanh thu
                var tongTienNopNganSach = (double)(tongThuLaoCong * 0.1m);

                var baoCao12a = new BaoCao12aDto
                {
                    TenDonVi = donVi?.TenDonVi ?? request.TenDonViBaoCao ?? "",
                    SoCongChungVien = soCongChungVien,
                    SoViecCongChung = soViecCongChung,
                    CongChungHopDong = congChungHopDong,
                    CongChungBanDich = congChungBanDich,
                    TongThuLaoCong = tongThuLaoCong,
                    TongPhiCongChung = tongPhiCongChung,
                    ChungThucBanSao = chungThucBanSao,
                    PhiChungThucBanSao = phiChungThucBanSao,
                    SoViecChungThucChuKy = chungThucChuKy,
                    PhiChungThucChuKy = phiChungThucChuKy,
                    TongTienNopNganSach = tongTienNopNganSach
                };

                var response = new BaoCao12ResponseDto
                {
                    Request = request,
                    BaoCao12a = baoCao12a
                };

                return new CommonResponse
                {
                    Status = "success",
                    Message = "Lấy dữ liệu báo cáo 12a thành công",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new CommonResponse
                {
                    Status = "error",
                    Message = $"Lỗi khi tạo báo cáo 12a: {ex.Message}",
                    Data = null
                };
            }
        }

        /// <summary>
        /// Lấy dữ liệu báo cáo 12b - Tình hình tổ chức và hoạt động công chứng tại địa bàn tỉnh
        /// </summary>
        public async Task<CommonResponse> GetBaoCao12bAsync(BaoCao12RequestDto request)
        {
            try
            {
                var validation = await ValidateRequestAsync(request);
                if (!validation.IsValid)
                {
                    return new CommonResponse
                    {
                        Status = "error",
                        Message = "Dữ liệu không hợp lệ",
                        Data = validation.ErrorMessages
                    };
                }

                var userInfo = authService.GetUserInfo();
                var donViList = await danhMucDonViService.GetDanhMucDonViByIdAsync(request.DonViId);
                var donVi = donViList?.FirstOrDefault();

                // Lấy tất cả hồ sơ trong khoảng thời gian
                var hoSos = await context.HoSoCCCTs 
                    .AsNoTracking()
                    .Include(x => x.DonViQuanLy)
                    .Include(x => x.LoaiHopDong)
                    .Include(x => x.CongChungVien)
                    .Include(x => x.HoSoCCCTChiPhis)
                    .Where(x => x.DonViQuanLyId == request.DonViId)
                    .Where(x => x.Status == "HT")
                    .Where(date => request.NgayBaoCaoTu.Date <= date.NgayDuyet.Date && date.NgayDuyet.Date <= request.NgayBaoCaoDen.Date)
                    .ToListAsync();

                // Lọc theo danh mục hợp đồng nếu có
                if (request.DanhMucHopDongIds.Any())
                {
                    hoSos = hoSos.Where(x => request.DanhMucHopDongIds.Contains(x.LoaiHopDongId)).ToList();
                }

                // Phân loại hồ sơ theo loại đơn vị (giả sử có trường phân biệt Phòng CC và Văn phòng CC)
                // Tạm thời chia đều để demo, trong thực tế cần có logic phân biệt rõ ràng
                var phongCCHoSos = hoSos.Take(hoSos.Count / 2).ToList();
                var vanPhongCCHoSos = hoSos.Skip(hoSos.Count / 2).ToList();

                // Tính số công chứng viên
                var tongSoCongChungVien = await context.DanhMucCanBos
                    .Where(x => x.DonViQuanLyId == request.DonViId)
                    .CountAsync();
                
                // Chia đều số công chứng viên (trong thực tế cần logic phân biệt)
                var phongCC_SoCongChungVien = tongSoCongChungVien / 2;
                var vanPhongCC_SoCongChungVien = tongSoCongChungVien - phongCC_SoCongChungVien;

                // === TÍNH TOÁN CHO PHÒNG CÔNG CHỨNG ===
                var phongCC_CongChungHoSos = phongCCHoSos.Where(x => x.LoaiHopDong?.IsCC == true).ToList();
                var phongCC_ChungThucHoSos = phongCCHoSos.Where(x => x.LoaiHopDong?.IsCC == false).ToList();

                var phongCC_SoViecCongChung = phongCC_CongChungHoSos.Count;
                var phongCC_CongChungHopDong = phongCC_CongChungHoSos.Count(x => x.LoaiHopDong?.TenHopDong?.Contains("hợp đồng") == true);
                var phongCC_CongChungBanDich = phongCC_CongChungHoSos.Count - phongCC_CongChungHopDong;
                var phongCC_ChungThucBanSao = phongCC_ChungThucHoSos.Count(x => x.LoaiHopDong?.TenHopDong?.Contains("bản sao") == true);
                var phongCC_ChungThucChuKy = phongCC_ChungThucHoSos.Count(x => x.LoaiHopDong?.TenHopDong?.Contains("chữ ký") == true);

                var phongCC_TongThuLaoCong = (decimal)phongCCHoSos.SelectMany(x => x.HoSoCCCTChiPhis).Sum(x => x.ThanhTien);
                var phongCC_TongPhiCongChung = (decimal)phongCC_CongChungHoSos.SelectMany(x => x.HoSoCCCTChiPhis).Sum(x => x.ThanhTien);
                var phongCC_PhiChungThucBanSao = (decimal)phongCC_ChungThucHoSos.Where(x => x.LoaiHopDong?.TenHopDong?.Contains("bản sao") == true)
                    .SelectMany(x => x.HoSoCCCTChiPhis).Sum(x => x.ThanhTien);
                var phongCC_PhiChungThucChuKy = (decimal)phongCC_ChungThucHoSos.Where(x => x.LoaiHopDong?.TenHopDong?.Contains("chữ ký") == true)
                    .SelectMany(x => x.HoSoCCCTChiPhis).Sum(x => x.ThanhTien);
                var phongCC_TongTienNopNganSach = (double)(phongCC_TongThuLaoCong * 0.1m);

                // === TÍNH TOÁN CHO VĂN PHÒNG CÔNG CHỨNG ===
                var vanPhongCC_CongChungHoSos = vanPhongCCHoSos.Where(x => x.LoaiHopDong?.IsCC == true).ToList();
                var vanPhongCC_ChungThucHoSos = vanPhongCCHoSos.Where(x => x.LoaiHopDong?.IsCC == false).ToList();

                var vanPhongCC_SoViecCongChung = vanPhongCC_CongChungHoSos.Count;
                var vanPhongCC_CongChungHopDong = vanPhongCC_CongChungHoSos.Count(x => x.LoaiHopDong?.TenHopDong?.Contains("hợp đồng") == true);
                var vanPhongCC_CongChungBanDich = vanPhongCC_CongChungHoSos.Count - vanPhongCC_CongChungHopDong;
                var vanPhongCC_ChungThucBanSao = vanPhongCC_ChungThucHoSos.Count(x => x.LoaiHopDong?.TenHopDong?.Contains("bản sao") == true);
                var vanPhongCC_ChungThucChuKy = vanPhongCC_ChungThucHoSos.Count(x => x.LoaiHopDong?.TenHopDong?.Contains("chữ ký") == true);

                var vanPhongCC_TongThuLaoCong = (decimal)vanPhongCCHoSos.SelectMany(x => x.HoSoCCCTChiPhis).Sum(x => x.ThanhTien);
                var vanPhongCC_TongPhiCongChung = (decimal)vanPhongCC_CongChungHoSos.SelectMany(x => x.HoSoCCCTChiPhis).Sum(x => x.ThanhTien);
                var vanPhongCC_PhiChungThucBanSao = (decimal)vanPhongCC_ChungThucHoSos.Where(x => x.LoaiHopDong?.TenHopDong?.Contains("bản sao") == true)
                    .SelectMany(x => x.HoSoCCCTChiPhis).Sum(x => x.ThanhTien);
                var vanPhongCC_PhiChungThucChuKy = (decimal)vanPhongCC_ChungThucHoSos.Where(x => x.LoaiHopDong?.TenHopDong?.Contains("chữ ký") == true)
                    .SelectMany(x => x.HoSoCCCTChiPhis).Sum(x => x.ThanhTien);
                var vanPhongCC_TongTienNopNganSach = (double)(vanPhongCC_TongThuLaoCong * 0.1m);

                var baoCao12b = new BaoCao12bDto
                {
                    TenDonVi = donVi?.TenDonVi ?? request.TenDonViBaoCao ?? "",
                    
                    // I. Phòng Công chứng
                    PhongCongChung_SoCongChungVien = phongCC_SoCongChungVien,
                    PhongCongChung_SoViecCongChung = phongCC_SoViecCongChung,
                    PhongCongChung_CongChungHopDong = phongCC_CongChungHopDong,
                    PhongCongChung_CongChungBanDich = phongCC_CongChungBanDich,
                    PhongCongChung_TongThuLaoCong = phongCC_TongThuLaoCong,
                    PhongCongChung_TongPhiCongChung = phongCC_TongPhiCongChung,
                    PhongCongChung_ChungThucBanSao = phongCC_ChungThucBanSao,
                    PhongCongChung_PhiChungThucBanSao = phongCC_PhiChungThucBanSao,
                    PhongCongChung_SoViecChungThucChuKy = phongCC_ChungThucChuKy,
                    PhongCongChung_PhiChungThucChuKy = phongCC_PhiChungThucChuKy,
                    PhongCongChung_TongTienNopNganSach = phongCC_TongTienNopNganSach,
                    
                    // II. Văn phòng Công chứng
                    VanPhongCongChung_SoCongChungVien = vanPhongCC_SoCongChungVien,
                    VanPhongCongChung_SoViecCongChung = vanPhongCC_SoViecCongChung,
                    VanPhongCongChung_CongChungHopDong = vanPhongCC_CongChungHopDong,
                    VanPhongCongChung_CongChungBanDich = vanPhongCC_CongChungBanDich,
                    VanPhongCongChung_TongThuLaoCong = vanPhongCC_TongThuLaoCong,
                    VanPhongCongChung_TongPhiCongChung = vanPhongCC_TongPhiCongChung,
                    VanPhongCongChung_ChungThucBanSao = vanPhongCC_ChungThucBanSao,
                    VanPhongCongChung_PhiChungThucBanSao = vanPhongCC_PhiChungThucBanSao,
                    VanPhongCongChung_SoViecChungThucChuKy = vanPhongCC_ChungThucChuKy,
                    VanPhongCongChung_PhiChungThucChuKy = vanPhongCC_PhiChungThucChuKy,
                    VanPhongCongChung_TongTienNopNganSach = vanPhongCC_TongTienNopNganSach
                };

                var response = new BaoCao12ResponseDto
                {
                    Request = request,
                    BaoCao12b = baoCao12b
                };

                return new CommonResponse
                {
                    Status = "success",
                    Message = "Lấy dữ liệu báo cáo 12b thành công",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new CommonResponse
                {
                    Status = "error",
                    Message = $"Lỗi khi tạo báo cáo 12b: {ex.Message}",
                    Data = null
                };
            }
        }

        /// <summary>
        /// Export báo cáo 12 ra file Word
        /// </summary>
        public async Task<CommonResponse> ExportBaoCao12ToWordAsync(BaoCao12RequestDto request)
        {
            // TODO: Implement export to Word
            await Task.CompletedTask;
            return new CommonResponse
            {
                Status = "error",
                Message = "Chức năng export Word chưa được triển khai",
                Data = null
            };
        }

        /// <summary>
        /// Export báo cáo 12 ra file Excel
        /// </summary>
        public async Task<CommonResponse> ExportBaoCao12ToExcelAsync(BaoCao12RequestDto request)
        {
            // TODO: Implement export to Excel
            await Task.CompletedTask;
            return new CommonResponse
            {
                Status = "error",
                Message = "Chức năng export Excel chưa được triển khai",
                Data = null
            };
        }
    }
}