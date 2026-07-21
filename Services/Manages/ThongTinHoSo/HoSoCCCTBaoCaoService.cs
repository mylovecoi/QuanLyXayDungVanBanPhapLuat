using DataAccess;
using DataAccess.Entities.Manages.ThongTinHoSo;
using DataAccess.Entities.Settings;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Services.DTOs;
using Services.DTOs.Manages.ThongTinHoSo;
using Services.DTOs.Manages.ThongTinHoSo.BaoCaoThongKe;
using Services.Helpers;
using Services.Model;
using Services.Settings;
using Services.Settings.DanhMucDungChung.DmHopDong;
using Services.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.DTOs.Manages.ThongTinHoSo.ExportData;
using System.IO.Compression;
using DataAccess.Entities.Systems;
using System.Data;
using DocumentFormat.OpenXml.VariantTypes;

namespace Services.Manages.ThongTinHoSo
{
    public interface IHoSoCCCTBaoCaoService
    {
        Task<CommonResponse> ValidateRequestAsync(ReportRequestDto request);
        Task<CommonResponse> ValidateRequestSoCongChungAsync(ReportRequestDto request);
        Task<CommonResponse> ValidateRequestAsync(ExportDataRequestDto request);
        Task<CommonResponse> GetBaoCaoThongKeTongQuatDataAsync(ReportRequestDto request);
        Task<CommonResponse> GetBaoCaoThongKeChiTietDataAsync(ReportRequestDto request);
        Task<CommonResponse> GetSoLuongHoSoTheoThangAsync(bool isTiepNhan);
        Task<CommonResponse> GetBaoCaoThongKeChiPhiTongQuatDataAsync(ReportRequestDto request);
        Task<CommonResponse> GetBaoCaoThongKeChiPhiChiTietDataAsync(ReportRequestDto request);
        Task<CommonResponse> ExportZip(ExportDataRequestDto request);
        Task<CommonResponse> GetYeuCauSoCongChungAsync(ReportRequestDto request);

        Task<CommonResponse> GetSoCongChungGiaoDichAsync(ReportRequestDto request);
        Task<CommonResponse> GetSoCongChungGiaoDichDienTuAsync(ReportRequestDto request);
    }

    public class HoSoCCCTBaoCaoService(
        ApplicationDbContext dbContext,
        IAuthService authService,
        IDanhMucDonViService danhMucDonViService,
        IDmHopDongService dmHopDongService) : IHoSoCCCTBaoCaoService
    {
        private readonly ApplicationDbContext _dbContext = dbContext;
        private IAuthService _authService = authService;
        private IDmHopDongService _dmHopDongService = dmHopDongService;
        private IDanhMucDonViService _danhMucDonViService = danhMucDonViService;

        public async Task<CommonResponse> ValidateRequestAsync(ReportRequestDto request)
        {
            var resultValidate = await new HoSoCCCTBaoCaoValidate(_danhMucDonViService, _dmHopDongService).ValidateAsync(request);
            if (!resultValidate.IsValid)
                return new("error", Helper.GetValidationErrorsDictionary(resultValidate), request, "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại!");
            return new("success");
        }

        public async Task<CommonResponse> ValidateRequestSoCongChungAsync(ReportRequestDto request)
        {
            var resultValidate = await new HoSoCCCTBaoCaoValidate(_danhMucDonViService, _dmHopDongService, true).ValidateAsync(request);
            if (!resultValidate.IsValid)
                return new("error", Helper.GetValidationErrorsDictionary(resultValidate), request, "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại!");
            return new("success");
        }

        public async Task<CommonResponse> ValidateRequestAsync(ExportDataRequestDto request)
        {
            var resultValidate = await new ExportDataRequestDtoValidate(_danhMucDonViService, _dmHopDongService).ValidateAsync(request);
            if (!resultValidate.IsValid)
                return new("error", Helper.GetValidationErrorsDictionary(resultValidate), request, "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại!");
            return new("success");
        }

        public async Task<CommonResponse> GetBaoCaoThongKeTongQuatDataAsync(ReportRequestDto request)
        {
            try
            {
                var dataView = new List<BaoCaoTongQuatResponseDto>();
                var dmHopDong = await _dmHopDongService.GetEntityByIdsAsync(request.DanhMucHopDongIds, null);

                foreach (var item in dmHopDong)
                {
                    int countHoSoHT = 0;
                    if (item.ParentId == null)
                    {
                        var children = dmHopDong.Where(x => x.ParentId == item.Id).ToList();
                        foreach (var child in children)
                        {
                            countHoSoHT += child.HoSoCCCTs.Count(x => x.Status == "HT" && x.DonViQuanLyId == request.DonViId);
                        }
                    }
                    else
                    {
                        countHoSoHT = item.HoSoCCCTs.Count(x => x.Status == "HT" && x.DonViQuanLyId == request.DonViId);
                    }
                    dataView.Add(new BaoCaoTongQuatResponseDto()
                    {
                        TenHopDong = item.TenHopDong,
                        SoLuong = countHoSoHT,
                        Level = item.Level,
                    });
                }
                var user = _authService.GetUserInfo();

                // var strCCCT = request.LoaiNghiepVu ? "công chứng" : "chứng thực";
                var strCCCT = "công chứng - chứng thực";
                var result = new ReportResponseDto<BaoCaoTongQuatResponseDto>()
                {
                    TenBaoCao = $"Báo cáo tổng quát hồ sơ {strCCCT}",
                    NgayBaoCao = request.NgayBaoCao,
                    NgayBaoCaoTu = request.NgayBaoCaoTu,
                    NgayBaoCaoDen = request.NgayBaoCaoDen,
                    TenDiaDanh = user?.DiaDanh ?? string.Empty,
                    TenDonVi = user?.TenDonViBaoCao ?? string.Empty,
                    TenDonViChuQuan = user?.TenDonViChuQuanBaoCao ?? string.Empty,
                    NguoiKy = user?.ChucDanhKy ?? string.Empty,
                    ChucDanhNguoiKy = user?.ChucDanhKy ?? string.Empty,
                    KyHieuDonVi = user?.KyHieuDonVi,
                    Data = dataView,
                };


                return new("success", "Tạo báo cáo thành công", result);
            }
            catch (Exception)
            {
                return new("error", "Có lỗi trong quá trình lấy dữ liệu. Hãy kiểm tra lại!");
            }
        }

        public async Task<CommonResponse> GetBaoCaoThongKeChiTietDataAsync(ReportRequestDto request)
        {
            try
            {
                var dataView = new List<BaoCaoTongQuatResponseDto>();
                var dmHopDong = await _dmHopDongService.GetEntityByIdsAsync(request.DanhMucHopDongIds, null);

                foreach (var item in dmHopDong)
                {
                    int countHoSoHT = 0;
                    List<BaoCaoChiTietDto> chiTiets = new();
                    if (item.ParentId == null)
                    {
                        var children = dmHopDong.Where(x => x.ParentId == item.Id).ToList();
                        foreach (var child in children)
                        {
                            countHoSoHT += child.HoSoCCCTs.Count(x => x.Status == "HT" && x.DonViQuanLyId == request.DonViId);
                        }
                    }
                    else
                    {
                        var lsHoSoHT = item.HoSoCCCTs.Where(x => x.Status == "HT" && x.DonViQuanLyId == request.DonViId).ToList();
                        foreach (var hoso in lsHoSoHT)
                        {
                            var childItem = new BaoCaoChiTietDto()
                            {
                                MaSoHoSo = hoso.MaSoHoSo,
                                NgayXuLy = hoso.NgayThuLy,
                                StrGiaTriHopDong = string.IsNullOrEmpty(hoso.StrSoTienMienThue) ? "0" : hoso.StrSoTienMienThue,
                                LoaiTaiSan = hoso.LoaiTaiSan?.DisplayName,
                                ThongTinTaiSan = hoso.ThongTinChiTietTaiSan,
                                ThongTinGiayToChungThuc = hoso.ThongTinGiayToChungThuc,
                                DiaBan = hoso.DiaBan?.TenDiaDanh
                            };

                            if (item.HopDongChiTiet.Count() > 0)
                            {
                                
                            }
                            chiTiets.Add(childItem);
                        }
                        countHoSoHT = lsHoSoHT.Count();

                    }
                    dataView.Add(new BaoCaoTongQuatResponseDto()
                    {
                        TenHopDong = item.TenHopDong,
                        SoLuong = countHoSoHT,
                        Level = item.Level,
                        ChiTiets = item != null ? chiTiets : new()
                    });
                }
                var user = _authService.GetUserInfo();

                // var strCCCT = request.LoaiNghiepVu ? "công chứng" : "chứng thực";
                var strCCCT = "công chứng - chứng thực";
                var result = new ReportResponseDto<BaoCaoTongQuatResponseDto>()
                {
                    TenBaoCao = $"Báo cáo chi tiết hồ sơ {strCCCT}",
                    NgayBaoCao = request.NgayBaoCao,
                    NgayBaoCaoTu = request.NgayBaoCaoTu,
                    NgayBaoCaoDen = request.NgayBaoCaoDen,
                    TenDiaDanh = user?.DiaDanh ?? string.Empty,
                    TenDonVi = user?.TenDonViBaoCao ?? string.Empty,
                    TenDonViChuQuan = user?.TenDonViChuQuanBaoCao ?? string.Empty,
                    NguoiKy = user?.ChucDanhKy ?? string.Empty,
                    ChucDanhNguoiKy = user?.ChucDanhKy ?? string.Empty,
                    KyHieuDonVi = user?.KyHieuDonVi,
                    Data = dataView,
                };


                return new("success", "Tạo báo cáo thành công", result);
            }
            catch (Exception)
            {
                return new("error", "Có lỗi trong quá trình lấy dữ liệu. Hãy kiểm tra lại!");
            }
        }

        public async Task<CommonResponse> GetBaoCaoThongKeChiPhiTongQuatDataAsync(ReportRequestDto request)
        {
            try
            {
                var dataView = new List<BaoCaoTongQuatResponseDto>();
                var dmHopDong = await _dmHopDongService.GetEntityByIdsAsync(request.DanhMucHopDongIds, null);

                foreach (var item in dmHopDong)
                {
                    int countHoSoHT = 0;
                    double sumChiPhi = 0;
                    if (item.ParentId == null)
                    {
                        var children = dmHopDong.Where(x => x.ParentId == item.Id).ToList();
                        foreach (var child in children)
                        {
                            countHoSoHT += child.HoSoCCCTs.Count(x => x.Status == "HT" && x.DonViQuanLyId == request.DonViId);
                            sumChiPhi += child.HoSoCCCTs.Where(x => x.Status == "HT" && x.DonViQuanLyId == request.DonViId).Sum(x => x.HoSoCCCTChiPhis?.Sum(x => x.ThanhTien) ?? 0);
                        }
                    }
                    else
                    {
                        countHoSoHT = item.HoSoCCCTs.Count(x => x.Status == "HT" && x.DonViQuanLyId == request.DonViId);
                        sumChiPhi += item.HoSoCCCTs.Where(x => x.Status == "HT" && x.DonViQuanLyId == request.DonViId).Sum(x => x.HoSoCCCTChiPhis?.Sum(x => x.ThanhTien) ?? 0);
                    }
                    dataView.Add(new BaoCaoTongQuatResponseDto()
                    {
                        TenHopDong = item.TenHopDong,
                        SoLuong = countHoSoHT,
                        Level = item.Level,
                        ChiPhi = sumChiPhi,
                    });
                }
                var user = _authService.GetUserInfo();

                // var strCCCT = request.LoaiNghiepVu ? "công chứng" : "chứng thực";
                var strCCCT = "công chứng - chứng thực";
                var result = new ReportResponseDto<BaoCaoTongQuatResponseDto>()
                {
                    TenBaoCao = $"Báo cáo chi phí tổng quát hồ sơ {strCCCT}",
                    NgayBaoCao = request.NgayBaoCao,
                    NgayBaoCaoTu = request.NgayBaoCaoTu,
                    NgayBaoCaoDen = request.NgayBaoCaoDen,
                    TenDiaDanh = user?.DiaDanh ?? string.Empty,
                    TenDonVi = user?.TenDonViBaoCao ?? string.Empty,
                    TenDonViChuQuan = user?.TenDonViChuQuanBaoCao ?? string.Empty,
                    NguoiKy = user?.ChucDanhKy ?? string.Empty,
                    ChucDanhNguoiKy = user?.ChucDanhKy ?? string.Empty,
                    KyHieuDonVi = user?.KyHieuDonVi,
                    Data = dataView,
                };


                return new("success", "Tạo báo cáo thành công", result);
            }
            catch (Exception)
            {
                return new("error", "Có lỗi trong quá trình lấy dữ liệu. Hãy kiểm tra lại!");
            }
        }

        public async Task<CommonResponse> GetBaoCaoThongKeChiPhiChiTietDataAsync(ReportRequestDto request)
        {
            try
            {
                var dataView = new List<BaoCaoTongQuatResponseDto>();
                var dmHopDong = await _dmHopDongService.GetEntityByIdsAsync(request.DanhMucHopDongIds, null);

                foreach (var item in dmHopDong)
                {
                    int countHoSoHT = 0;
                    List<BaoCaoChiTietDto> chiTiets = new();
                    if (item.ParentId == null)
                    {
                        var children = dmHopDong.Where(x => x.ParentId == item.Id).ToList();
                        foreach (var child in children)
                        {
                            countHoSoHT += child.HoSoCCCTs.Count(x => x.Status == "HT" && x.DonViQuanLyId == request.DonViId);
                        }
                    }
                    else
                    {
                        var lsHoSoHT = item.HoSoCCCTs.Where(x => x.Status == "HT" && x.DonViQuanLyId == request.DonViId).ToList();
                        foreach (var hoso in lsHoSoHT)
                        {
                            chiTiets.Add(new()
                            {
                                MaSoHoSo = hoso.MaSoHoSo,
                                NgayXuLy = hoso.NgayThuLy,
                                StrGiaTriHopDong = string.IsNullOrEmpty(hoso.StrSoTienMienThue) ? "0" : hoso.StrSoTienMienThue,
                                LoaiTaiSan = hoso.LoaiTaiSan?.DisplayName,
                                ThongTinTaiSan = hoso.ThongTinChiTietTaiSan,
                                ThongTinGiayToChungThuc = hoso.ThongTinGiayToChungThuc,
                                DiaBan = hoso.DiaBan?.TenDiaDanh,
                                ChiPhi = hoso.HoSoCCCTChiPhis.Sum(x => x.ThanhTien)
                            });
                        }
                        countHoSoHT = lsHoSoHT.Count();

                    }
                    dataView.Add(new BaoCaoTongQuatResponseDto()
                    {
                        TenHopDong = item.TenHopDong,
                        SoLuong = countHoSoHT,
                        Level = item.Level,
                        ChiTiets = item != null ? chiTiets : new()
                    });
                }
                var user = _authService.GetUserInfo();

                // var strCCCT = request.LoaiNghiepVu ? "công chứng" : "chứng thực";
                var strCCCT = "công chứng - chứng thực";
                var result = new ReportResponseDto<BaoCaoTongQuatResponseDto>()
                {
                    TenBaoCao = $"Báo cáo chi phí chi tiết hồ sơ {strCCCT}",
                    NgayBaoCao = request.NgayBaoCao,
                    NgayBaoCaoTu = request.NgayBaoCaoTu,
                    NgayBaoCaoDen = request.NgayBaoCaoDen,
                    TenDiaDanh = user?.DiaDanh ?? string.Empty,
                    TenDonVi = user?.TenDonViBaoCao ?? string.Empty,
                    TenDonViChuQuan = user?.TenDonViChuQuanBaoCao ?? string.Empty,
                    NguoiKy = user?.ChucDanhKy ?? string.Empty,
                    ChucDanhNguoiKy = user?.ChucDanhKy ?? string.Empty,
                    KyHieuDonVi = user?.KyHieuDonVi,
                    Data = dataView,
                };


                return new("success", "Tạo báo cáo thành công", result);
            }
            catch (Exception)
            {
                return new("error", "Có lỗi trong quá trình lấy dữ liệu. Hãy kiểm tra lại!");
            }
        }

        public async Task<CommonResponse> GetSoLuongHoSoTheoThangAsync(bool isTiepNhan)
        {
            try
            {
                var userInfo = _authService.GetUserInfo();
                var result = new HoSoTheoThangResponseDto();

                var hoso = await _dbContext.HoSoCCCTs
                    .AsNoTracking()
                    .Include(x => x.LoaiHopDong)
                    .Where(x => userInfo != null ? x.DonViQuanLyId == userInfo.DanhMucDonViId : true)
                    .Where(x => x.NgayThuLy.Year == DateTime.Now.Year)
                    .ToListAsync();

                var congChungs = hoso.Where(x => x.LoaiHopDong!.IsCC == true).ToList();
                var chungThucs = hoso.Where(x => x.LoaiHopDong!.IsCC == false).ToList();

                for (int i = 1; i <= 12; i++)
                {
                    result.CongChung.Add(congChungs.Count(x => x.NgayDuyet.Month == i));
                    result.ChungThuc.Add(chungThucs.Count(x => x.NgayDuyet.Month == i));
                }
                return new("success", "Lấy dữ liêu thành công", result);
            }
            catch (Exception)
            {
                return new("error", "Có lỗi trong quá trình lấy dữ liệu. Hãy kiểm tra lại!");
            }
        }

        public async Task<CommonResponse> ExportZip(ExportDataRequestDto request)
        {
            try
            {
                var dmHopDong = await _dmHopDongService.GetEntityByIdsAsync(request.HopDongIds, null);
                if (!dmHopDong.Any()) return new("error", "Không tìm thấy nghiệp vụ tương ứng. Vui lòng kiểm tra lại danh sách đã chọn.");

                var dmChild = dmHopDong.Where(x => x.ParentId != Guid.Empty).ToList();

                var dmChildId = dmChild.Select(x => x.Id).ToList();

                var hoSoList = await _dbContext.HoSoCCCTs
                    .Include(x => x.LoaiHopDong).ThenInclude(x => x!.Parent)
                    .AsNoTracking()
                    .Where(x => dmChildId.Contains(x.LoaiHopDongId) && x.NgayDuyet.Year == request.NamKetXuat && x.DonViQuanLyId == request.DonViId && x.Status == "HT")
                    .ToListAsync();

                var files = await _dbContext.AttachedFiles
                    .AsNoTracking()
                    .Where(x => (x.TableName == nameof(HoSoCCCT) || x.TableName == "ChuKyDienTu") && x.Status == "XD" && hoSoList.Select(hs => hs.Id).ToList().Contains(x.GroupId) && x.FileContent != null).ToListAsync();

                foreach (var hoSo in hoSoList)
                {
                    hoSo.AttachedFiles = files.Where(f => f.GroupId == hoSo.Id).ToList();
                }

                if (!hoSoList.Any()) return new("error", $"Không tìm thấy hồ sơ được xử lý trong năm {request.NamKetXuat}. Vui lòng kiểm tra lại năm kết xuất.");


                var memoryStream = new MemoryStream();
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var hoSo in hoSoList)
                    {
                        foreach (var file in hoSo.AttachedFiles)
                        {
                            if (file.FileContent != null)
                            {
                                var contentType = Path.GetExtension(file.FileName)?.TrimStart('.').ToLower() ?? throw new InvalidOperationException("Tệp không có phần mở rộng hợp lệ.");
                                var fileName = Helper.ConvertStrToSlug(file.MoTa!);

                                string strCCCT = hoSo.LoaiHopDong!.Parent!.IsCC ? "CongChung" : "ChungThuc";

                                var entryPath = $"{request.NamKetXuat}/{strCCCT}/{hoSo.LoaiHopDong!.Parent!.NameAscii}/{hoSo.LoaiHopDong.NameAscii}/{Helper.ConvertStrToSlug(hoSo.MaSoHoSo)}/{fileName}.{contentType}";

                                var entry = archive.CreateEntry(entryPath);
                                using var entryStream = entry.Open();
                                await entryStream.WriteAsync(file.FileContent, 0, file.FileContent.Length);
                            }
                        }

                    }
                }
                memoryStream.Position = 0;

                return new("success", $"Kết xuất dữ liệu thành công", memoryStream);
            }
            catch (Exception)
            {
                return new("error", "Có lỗi trong quá trình kết xuất dữ liệu. Hãy kiểm tra lại!");
            }
        }

        public async Task<CommonResponse> GetYeuCauSoCongChungAsync(ReportRequestDto request)
        {
            try
            {
                var userInfo = _authService.GetUserInfo();

                var hoso = await _dbContext.HoSoCCCTs
                    .AsNoTracking()
                    .Include(x => x.LoaiHopDong)
                    .Where(x => userInfo != null ? x.DonViQuanLyId == userInfo.DanhMucDonViId : true)
                    .Where(x => x.LoaiHopDong != null && x.LoaiHopDong.IsCC == request.LoaiNghiepVu)
                    .Where(date => request.NgayBaoCaoTu.Date <= date.NgayThuLy.Date && date.NgayThuLy.Date <= request.NgayBaoCaoDen.Date)
                    .OrderBy(x => x.NgayThuLy)
                    .ToListAsync();

                var result = new ReportResponseDto<HoSoCCCT>()
                {
                    TenBaoCao = "Sổ yêu cầu công chứng",
                    NgayBaoCao = request.NgayBaoCao,
                    NgayBaoCaoTu = request.NgayBaoCaoTu,
                    NgayBaoCaoDen = request.NgayBaoCaoDen,
                    TenDiaDanh = userInfo?.DiaDanh ?? string.Empty,
                    TenDonVi = userInfo?.TenDonViBaoCao ?? string.Empty,
                    TenDonViChuQuan = userInfo?.TenDonViChuQuanBaoCao ?? string.Empty,
                    Data = hoso,
                };

                return new("success", "Lấy dữ liêu thành công", result);
            }
            catch (Exception)
            {
                return new("error", "Có lỗi trong quá trình lấy dữ liệu. Hãy kiểm tra lại!");
            }
        }

        public async Task<CommonResponse> GetSoCongChungGiaoDichAsync(ReportRequestDto request)
        {
            try
            {
                var result = await GetDataForSoCongChungGiaoDich(request);

                return new("success", "Lấy dữ liêu thành công", result);
            }
            catch (Exception)
            {
                return new("error", "Có lỗi trong quá trình lấy dữ liệu. Hãy kiểm tra lại!");
            }
        }

        public async Task<CommonResponse> GetSoCongChungGiaoDichDienTuAsync(ReportRequestDto request)
        {
            try
            {
                var result = await GetDataForSoCongChungGiaoDich(request);

                return new("success", "Lấy dữ liêu thành công", result);
            }
            catch (Exception)
            {
                return new("error", "Có lỗi trong quá trình lấy dữ liệu. Hãy kiểm tra lại!");
            }
        }

        private async Task<ReportResponseDto<HoSoCCCT>> GetDataForSoCongChungGiaoDich(ReportRequestDto request)
        {
            var userInfo = _authService.GetUserInfo();

            var hoSos = await _dbContext.HoSoCCCTs
                .AsNoTracking()
                .Include(x => x.LoaiHopDong)
                .Include(x => x.CongChungVien)
                .Include(x => x.HoSoCCCTChiPhis)
                .Include(x => x.LoaiTaiSan)
                .Where(x => userInfo != null ? x.DonViQuanLyId == userInfo.DanhMucDonViId : true)
                .Where(x => x.LoaiHopDong != null && x.LoaiHopDong.IsCC == request.LoaiNghiepVu)
                .Where(x => x.Status == "HT" && x.PhuongThucCongChung == request.IsHoSoDienTu)
                .Where(date => request.NgayBaoCaoTu.Date <= date.NgayDuyet.Date && date.NgayDuyet.Date <= request.NgayBaoCaoDen.Date)
                .OrderBy(x => x.NgayDuyet)
                .ToListAsync();

            return new ReportResponseDto<HoSoCCCT>()
            {
                TenBaoCao = "Sổ công chứng giao dịch",
                NgayBaoCao = request.NgayBaoCao,
                NgayBaoCaoTu = request.NgayBaoCaoTu,
                NgayBaoCaoDen = request.NgayBaoCaoDen,
                TenDiaDanh = userInfo?.DiaDanh ?? string.Empty,
                TenDonVi = userInfo?.TenDonViBaoCao ?? string.Empty,
                TenDonViChuQuan = userInfo?.TenDonViChuQuanBaoCao ?? string.Empty,
                Data = hoSos,
            };
        }


    }
}
