using DataAccess;
using DataAccess.Entities.Manages.ThongTinHoSo;
using DataAccess.Entities.Settings;
using DataAccess.Enums;
using Microsoft.EntityFrameworkCore;
using Services.DTOs.BaoCao17;
using Services.Model;
using static Services.DTOs.BaoCao17.BaoCao17Constants;

namespace Services.BaoCao17
{
    /// <summary>
    /// Service xử lý báo cáo 17 theo Thông tư 03/2019/TT-BTP
    /// </summary>
    public class BaoCao17Service(ApplicationDbContext context) : IBaoCao17Service
    {

        /// <summary>
        /// Validate request báo cáo 17
        /// </summary>
        public async Task<BaoCao17ValidationResult> ValidateRequestAsync(BaoCao17RequestDto request)
        {
            var result = new BaoCao17ValidationResult { IsValid = true };

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
            if (request.KyBaoCao == KyBaoCao17.SauThang)
            {
                var thangBatDau = request.NgayBaoCaoTu.Month;
                var thangKetThuc = request.NgayBaoCaoDen.Month;
                var soThang = thangKetThuc - thangBatDau + 1;

                if (soThang != 6)
                {
                    result.AddWarning("Báo cáo 6 tháng thường tính từ tháng 1-6 hoặc 7-12");
                }
            }
            else if (request.KyBaoCao == KyBaoCao17.Nam || request.KyBaoCao == KyBaoCao17.NamChinhThuc)
            {
                if (request.NgayBaoCaoTu.Month != 1 || request.NgayBaoCaoDen.Month != 12)
                {
                    result.AddWarning("Báo cáo năm thường tính từ 01/01 đến 31/12");
                }
            }

            // Kiểm tra thời hạn nộp báo cáo
            var thoiHanNop = GetThoiHanNopBaoCao(request.LoaiBaoCao, request.KyBaoCao, request.NgayBaoCaoDen.Year);
            if (DateTime.Now > thoiHanNop)
            {
                result.AddWarning($"Đã quá thời hạn nộp báo cáo (thời hạn: {thoiHanNop:dd/MM/yyyy})");
            }

            return result;
        }

        /// <summary>
        /// Lấy dữ liệu báo cáo 17a - UBND cấp xã
        /// </summary>
        public async Task<CommonResponse> GetBaoCao17aAsync(BaoCao17RequestDto request)
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

                // Lấy dữ liệu hồ sơ chứng thực của UBND cấp xã trong kỳ báo cáo
                var hoSoChungThuc = await GetHoSoChungThucByDonViAsync(
                    request.DonViId,
                    request.NgayBaoCaoTu,
                    request.NgayBaoCaoDen);

                var baoCao17a = new BaoCao17aDto
                {
                    TenUBNDCapXa = request.TenDonViBaoCao ?? "",
                    ChungThucBanSao = hoSoChungThuc.Count(x => IsChungThucBanSao(x)),
                    ChungThucChuKy = hoSoChungThuc.Count(x => IsChungThucChuKy(x)),
                    ChungThucHopDong = hoSoChungThuc.Count(x => IsChungThucHopDong(x))
                };

                var response = new BaoCao17ResponseDto
                {
                    Request = request,
                    BaoCao17a = baoCao17a
                };

                return new CommonResponse
                {
                    Status = "success",
                    Message = "Lấy dữ liệu báo cáo 17a thành công",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new CommonResponse
                {
                    Status = "error",
                    Message = $"Lỗi khi tạo báo cáo 17a: {ex.Message}",
                    Data = null
                };
            }
        }

        /// <summary>
        /// Lấy dữ liệu báo cáo 17b - Cấp huyện
        /// </summary>
        public async Task<CommonResponse> GetBaoCao17bAsync(BaoCao17RequestDto request)
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

                // Lấy danh sách UBND cấp xã trên địa bàn huyện
                var danhSachUBNDCapXa = await GetDanhSachUBNDCapXaThuocHuyenAsync(request.DonViId);

                var baoCao17b = new BaoCao17bDto();

                // Tính kết quả chứng thực tại Phòng Tư pháp
                var hoSoPhongTuPhap = await GetHoSoChungThucByDonViAsync(
                    request.DonViId,
                    request.NgayBaoCaoTu,
                    request.NgayBaoCaoDen);

                baoCao17b.KetQuaPhongTuPhap = new BaoCao17PhongTuPhapDto
                {
                    ChungThucBanSao = hoSoPhongTuPhap.Count(x => IsChungThucBanSao(x)),
                    ChungThucChuKy = hoSoPhongTuPhap.Count(x => IsChungThucChuKy(x)),
                    ChungThucChuKyNguoiDich = hoSoPhongTuPhap.Count(x => IsChungThucChuKyNguoiDich(x)),
                    ChungThucHopDong = hoSoPhongTuPhap.Count(x => IsChungThucHopDong(x))
                };

                // Tính kết quả chứng thực của từng UBND cấp xã
                foreach (var ubndCapXa in danhSachUBNDCapXa)
                {
                    var hoSoUBND = await GetHoSoChungThucByDonViAsync(
                        ubndCapXa.Id,
                        request.NgayBaoCaoTu,
                        request.NgayBaoCaoDen);

                    var ketQuaUBND = new BaoCao17aDto
                    {
                        TenUBNDCapXa = ubndCapXa.TenDonVi,
                        ChungThucBanSao = hoSoUBND.Count(x => IsChungThucBanSao(x)),
                        ChungThucChuKy = hoSoUBND.Count(x => IsChungThucChuKy(x)),
                        ChungThucHopDong = hoSoUBND.Count(x => IsChungThucHopDong(x))
                    };

                    baoCao17b.DanhSachUBNDCapXa.Add(ketQuaUBND);
                }

                // Tính tổng số
                baoCao17b.TongSoUBNDCapXa = new BaoCao17aDto
                {
                    TenUBNDCapXa = "Tổng số",
                    ChungThucBanSao = baoCao17b.DanhSachUBNDCapXa.Sum(x => x.ChungThucBanSao),
                    ChungThucChuKy = baoCao17b.DanhSachUBNDCapXa.Sum(x => x.ChungThucChuKy),
                    ChungThucHopDong = baoCao17b.DanhSachUBNDCapXa.Sum(x => x.ChungThucHopDong)
                };

                var response = new BaoCao17ResponseDto
                {
                    Request = request,
                    BaoCao17b = baoCao17b
                };

                return new CommonResponse
                {
                    Status = "success",
                    Message = "Lấy dữ liệu báo cáo 17b thành công",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new CommonResponse
                {
                    Status = "error",
                    Message = $"Lỗi khi tạo báo cáo 17b: {ex.Message}",
                    Data = null
                };
            }
        }

        /// <summary>
        /// Lấy dữ liệu báo cáo 17c - Cấp tỉnh
        /// </summary>
        public async Task<CommonResponse> GetBaoCao17cAsync(BaoCao17RequestDto request)
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

                // Lấy danh sách Phòng Tư pháp trên địa bàn tỉnh
                var danhSachPhongTuPhap = await GetDanhSachPhongTuPhapThuocTinhAsync(request.DonViId);

                // Lấy danh sách huyện trên địa bàn tỉnh
                var danhSachHuyen = await GetDanhSachHuyenThuocTinhAsync(request.DonViId);

                var baoCao17c = new BaoCao17cDto();

                // Tính kết quả chứng thực của từng Phòng Tư pháp
                foreach (var phongTuPhap in danhSachPhongTuPhap)
                {
                    var hoSoPhongTuPhap = await GetHoSoChungThucByDonViAsync(
                        phongTuPhap.Id,
                        request.NgayBaoCaoTu,
                        request.NgayBaoCaoDen);

                    var ketQuaPhongTuPhap = new BaoCao17PhongTuPhapItemDto
                    {
                        TenPhongTuPhap = phongTuPhap.TenDonVi,
                        ChungThucBanSao = hoSoPhongTuPhap.Count(x => IsChungThucBanSao(x)),
                        ChungThucChuKy = hoSoPhongTuPhap.Count(x => IsChungThucChuKy(x)),
                        ChungThucChuKyNguoiDich = hoSoPhongTuPhap.Count(x => IsChungThucChuKyNguoiDich(x)),
                        ChungThucHopDong = hoSoPhongTuPhap.Count(x => IsChungThucHopDong(x))
                    };

                    baoCao17c.DanhSachPhongTuPhap.Add(ketQuaPhongTuPhap);
                }

                // Tính tổng số Phòng Tư pháp
                baoCao17c.TongSoPhongTuPhap = new BaoCao17PhongTuPhapDto
                {
                    ChungThucBanSao = baoCao17c.DanhSachPhongTuPhap.Sum(x => x.ChungThucBanSao),
                    ChungThucChuKy = baoCao17c.DanhSachPhongTuPhap.Sum(x => x.ChungThucChuKy),
                    ChungThucChuKyNguoiDich = baoCao17c.DanhSachPhongTuPhap.Sum(x => x.ChungThucChuKyNguoiDich),
                    ChungThucHopDong = baoCao17c.DanhSachPhongTuPhap.Sum(x => x.ChungThucHopDong)
                };

                // Tính kết quả chứng thực của UBND cấp xã theo từng huyện
                foreach (var huyen in danhSachHuyen)
                {
                    var danhSachUBNDCapXa = await GetDanhSachUBNDCapXaThuocHuyenAsync(huyen.Id);

                    int tongChungThucBanSao = 0;
                    int tongChungThucChuKy = 0;
                    int tongChungThucHopDong = 0;

                    foreach (var ubndCapXa in danhSachUBNDCapXa)
                    {
                        var hoSoUBND = await GetHoSoChungThucByDonViAsync(
                            ubndCapXa.Id,
                            request.NgayBaoCaoTu,
                            request.NgayBaoCaoDen);

                        tongChungThucBanSao += hoSoUBND.Count(x => IsChungThucBanSao(x));
                        tongChungThucChuKy += hoSoUBND.Count(x => IsChungThucChuKy(x));
                        tongChungThucHopDong += hoSoUBND.Count(x => IsChungThucHopDong(x));
                    }

                    var ketQuaHuyen = new BaoCao17HuyenItemDto
                    {
                        TenHuyen = huyen.TenDonVi,
                        ChungThucBanSao = tongChungThucBanSao,
                        ChungThucChuKy = tongChungThucChuKy,
                        ChungThucHopDong = tongChungThucHopDong
                    };

                    baoCao17c.DanhSachHuyen.Add(ketQuaHuyen);
                }

                // Tính tổng số UBND cấp xã
                baoCao17c.TongSoUBNDCapXa = new BaoCao17aDto
                {
                    TenUBNDCapXa = "Tổng số",
                    ChungThucBanSao = baoCao17c.DanhSachHuyen.Sum(x => x.ChungThucBanSao),
                    ChungThucChuKy = baoCao17c.DanhSachHuyen.Sum(x => x.ChungThucChuKy),
                    ChungThucHopDong = baoCao17c.DanhSachHuyen.Sum(x => x.ChungThucHopDong)
                };

                var response = new BaoCao17ResponseDto
                {
                    Request = request,
                    BaoCao17c = baoCao17c
                };

                return new CommonResponse
                {
                    Status = "success",
                    Message = "Lấy dữ liệu báo cáo 17c thành công",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new CommonResponse
                {
                    Status = "error",
                    Message = $"Lỗi khi tạo báo cáo 17c: {ex.Message}",
                    Data = null
                };
            }
        }

        /// <summary>
        /// Lấy dữ liệu báo cáo 17d - Cơ quan đại diện nước ngoài
        /// </summary>
        public Task<CommonResponse> GetBaoCao17dAsync(BaoCao17RequestDto request)
        {
            try
            {
                // Báo cáo 17d chỉ dành cho Bộ Ngoại giao - tạm thời trả về dữ liệu mẫu
                var baoCao17d = new BaoCao17dDto
                {
                    TongSo = new BaoCao17CoQuanDaiDienBaseDto
                    {
                        ChungThucBanSao = 0,
                        ChungThucChuKy = 0,
                        ChungThucChuKyNguoiDich = 0
                    }
                };

                var response = new BaoCao17ResponseDto
                {
                    Request = request,
                    BaoCao17d = baoCao17d
                };

                return Task.FromResult(new CommonResponse
                {
                    Status = "success",
                    Message = "Báo cáo 17d - Cơ quan đại diện nước ngoài (chưa có dữ liệu)",
                    Data = response
                });
            }
            catch (Exception ex)
            {
                return Task.FromResult(new CommonResponse
                {
                    Status = "error",
                    Message = $"Lỗi khi tạo báo cáo 17d: {ex.Message}",
                    Data = null
                });
            }
        }

        /// <summary>
        /// Export báo cáo ra Word - tạm thời chưa implement
        /// </summary>
        public async Task<CommonResponse> ExportBaoCao17ToWordAsync(BaoCao17RequestDto request)
        {
            await Task.CompletedTask;
            return new CommonResponse
            {
                Status = "error",
                Message = "Chức năng export Word đang phát triển",
                Data = null
            };
        }

        /// <summary>
        /// Export báo cáo ra Excel - tạm thời chưa implement
        /// </summary>
        public async Task<CommonResponse> ExportBaoCao17ToExcelAsync(BaoCao17RequestDto request)
        {
            await Task.CompletedTask;
            return new CommonResponse
            {
                Status = "error",
                Message = "Chức năng export Excel đang phát triển",
                Data = null
            };
        }

        /// <summary>
        /// Lấy thông tin đơn vị cho báo cáo
        /// </summary>
        public async Task<CommonResponse> GetDonViInfoForBaoCaoAsync(Guid donViId, LoaiBaoCao17 loaiBaoCao)
        {
            var donVi = await context.DanhMucDonVis.FindAsync(donViId);
            if (donVi == null)
            {
                return new CommonResponse
                {
                    Status = "error",
                    Message = "Đơn vị không tồn tại",
                    Data = null
                };
            }

            return new CommonResponse
            {
                Status = "success",
                Message = "Lấy thông tin đơn vị thành công",
                Data = donVi
            };
        }

        /// <summary>
        /// Tính thời hạn nộp báo cáo theo quy định
        /// </summary>
        public DateTime GetThoiHanNopBaoCao(LoaiBaoCao17 loaiBaoCao, KyBaoCao17 kyBaoCao, int nam)
        {
            return loaiBaoCao switch
            {
                LoaiBaoCao17.BaoCao17a => kyBaoCao switch
                {
                    KyBaoCao17.SauThang => new DateTime(nam, ThoiHanNopBaoCao.BaoCao17a_6Thang_Thang, ThoiHanNopBaoCao.BaoCao17a_6Thang_Ngay),
                    KyBaoCao17.Nam => new DateTime(nam, ThoiHanNopBaoCao.BaoCao17a_Nam_Thang, ThoiHanNopBaoCao.BaoCao17a_Nam_Ngay),
                    KyBaoCao17.NamChinhThuc => new DateTime(nam + 1, ThoiHanNopBaoCao.BaoCao17a_NamChinhThuc_Thang, ThoiHanNopBaoCao.BaoCao17a_NamChinhThuc_Ngay),
                    _ => DateTime.Now
                },
                LoaiBaoCao17.BaoCao17b => kyBaoCao switch
                {
                    KyBaoCao17.SauThang => new DateTime(nam, ThoiHanNopBaoCao.BaoCao17b_6Thang_Thang, ThoiHanNopBaoCao.BaoCao17b_6Thang_Ngay),
                    KyBaoCao17.Nam => new DateTime(nam, ThoiHanNopBaoCao.BaoCao17b_Nam_Thang, ThoiHanNopBaoCao.BaoCao17b_Nam_Ngay),
                    KyBaoCao17.NamChinhThuc => new DateTime(nam + 1, ThoiHanNopBaoCao.BaoCao17b_NamChinhThuc_Thang, ThoiHanNopBaoCao.BaoCao17b_NamChinhThuc_Ngay),
                    _ => DateTime.Now
                },
                LoaiBaoCao17.BaoCao17c => kyBaoCao switch
                {
                    KyBaoCao17.SauThang => new DateTime(nam, ThoiHanNopBaoCao.BaoCao17c_6Thang_Thang, ThoiHanNopBaoCao.BaoCao17c_6Thang_Ngay),
                    KyBaoCao17.Nam => new DateTime(nam, ThoiHanNopBaoCao.BaoCao17c_Nam_Thang, ThoiHanNopBaoCao.BaoCao17c_Nam_Ngay),
                    KyBaoCao17.NamChinhThuc => new DateTime(nam + 1, ThoiHanNopBaoCao.BaoCao17c_NamChinhThuc_Thang, ThoiHanNopBaoCao.BaoCao17c_NamChinhThuc_Ngay),
                    _ => DateTime.Now
                },
                LoaiBaoCao17.BaoCao17d => new DateTime(nam + 1, ThoiHanNopBaoCao.BaoCao17d_NamChinhThuc_Thang, ThoiHanNopBaoCao.BaoCao17d_NamChinhThuc_Ngay),
                _ => DateTime.Now,
            };
        }

        /// <summary>
        /// Kiểm tra quyền tạo báo cáo
        /// </summary>
        public Task<bool> CheckPermissionAsync(Guid userId, LoaiBaoCao17 loaiBaoCao)
        {
            // Tạm thời cho phép tất cả user - cần implement logic phân quyền theo cấp đơn vị
            return Task.FromResult(true);
        }

        #region Private Methods

        /// <summary>
        /// Lấy hồ sơ chứng thực theo đơn vị và khoảng thời gian
        /// </summary>
        private async Task<List<HoSoCCCT>> GetHoSoChungThucByDonViAsync(Guid donViId, DateTime tuNgay, DateTime denNgay)
        {
            return await context.HoSoCCCTs
                .Where(x => x.DonViQuanLyId == donViId
                        && x.NgayThuLy >= tuNgay
                        && x.NgayThuLy <= denNgay
                        && x.LoaiHopDong != null
                        && x.LoaiHopDong.IsCC == false) // false = chứng thực, true = công chứng
                .Include(x => x.LoaiHopDong)
                .ToListAsync();
        }

        /// <summary>
        /// Lấy danh sách UBND cấp xã thuộc huyện
        /// </summary>
        private async Task<List<DanhMucDonVi>> GetDanhSachUBNDCapXaThuocHuyenAsync(Guid huyenId)
        {
            // Tạm thời lấy tất cả đơn vị con theo cấu trúc phân cấp - cần cải tiến theo logic nghiệp vụ
            return await context.DanhMucDonVis
                .Where(x => x.DonViChuQuanId == huyenId)
                .ToListAsync();
        }

        /// <summary>
        /// Lấy danh sách Phòng Tư pháp thuộc tỉnh
        /// </summary>
        private async Task<List<DanhMucDonVi>> GetDanhSachPhongTuPhapThuocTinhAsync(Guid tinhId)
        {
            // Lấy các Phòng Tư pháp thuộc tỉnh (Level = 1, có chứa "Phòng Tư pháp" trong tên)
            return await context.DanhMucDonVis
                .Where(x => x.DonViChuQuanId == tinhId
                         && x.Level == 1
                         && x.TenDonVi.Contains("Phòng Tư pháp"))
                .ToListAsync();
        }

        /// <summary>
        /// Lấy danh sách huyện thuộc tỉnh
        /// </summary>
        private async Task<List<DanhMucDonVi>> GetDanhSachHuyenThuocTinhAsync(Guid tinhId)
        {
            // Lấy các huyện thuộc tỉnh (Level = 1, không phải Phòng Tư pháp)
            return await context.DanhMucDonVis
                .Where(x => x.DonViChuQuanId == tinhId
                         && x.Level == 1
                         && !x.TenDonVi.Contains("Phòng Tư pháp"))
                .ToListAsync();
        }

        /// <summary>
        /// Phân loại hồ sơ chứng thực theo thứ tự ưu tiên
        /// Mỗi hồ sơ chỉ được tính vào 1 loại duy nhất theo thứ tự:
        /// 1. Chữ ký người dịch (ưu tiên cao nhất)
        /// 2. Hợp đồng, giao dịch
        /// 3. Chữ ký trong giấy tờ, văn bản
        /// 4. Bản sao (ưu tiên thấp nhất)
        /// </summary>
        private static LoaiChungThuc GetLoaiChungThuc(HoSoCCCT hoSo)
        {
            var tenHopDong = hoSo.LoaiHopDong?.TenHopDong?.ToLower() ?? "";
            var noiDungHoSo = hoSo.NoiDungHoSo?.ToLower() ?? "";

            // 1. Ưu tiên cao nhất: Chứng thực chữ ký người dịch
            if (tenHopDong.Contains("người dịch") || tenHopDong.Contains("dịch thuật") ||
                noiDungHoSo.Contains("người dịch") || noiDungHoSo.Contains("dịch thuật"))
            {
                return LoaiChungThuc.ChuKyNguoiDich;
            }

            // 2. Chứng thực hợp đồng, giao dịch
            if (tenHopDong.Contains("hợp đồng") || tenHopDong.Contains("giao dịch") ||
                noiDungHoSo.Contains("hợp đồng") || noiDungHoSo.Contains("giao dịch"))
            {
                return LoaiChungThuc.HopDongGiaoDich;
            }

            // 3. Chứng thực chữ ký trong giấy tờ, văn bản
            if (tenHopDong.Contains("chữ ký") || noiDungHoSo.Contains("chữ ký"))
            {
                return LoaiChungThuc.ChuKy;
            }

            // 4. Ưu tiên thấp nhất: Chứng thực bản sao
            if (tenHopDong.Contains("bản sao") || noiDungHoSo.Contains("bản sao"))
            {
                return LoaiChungThuc.BanSao;
            }

            // Mặc định là bản sao nếu không xác định được
            return LoaiChungThuc.BanSao;
        }

        /// <summary>
        /// Kiểm tra hồ sơ có phải chứng thực bản sao không
        /// </summary>
        private static bool IsChungThucBanSao(HoSoCCCT hoSo)
        {
            return GetLoaiChungThuc(hoSo) == LoaiChungThuc.BanSao;
        }

        /// <summary>
        /// Kiểm tra hồ sơ có phải chứng thực chữ ký không
        /// </summary>
        private static bool IsChungThucChuKy(HoSoCCCT hoSo)
        {
            return GetLoaiChungThuc(hoSo) == LoaiChungThuc.ChuKy;
        }

        /// <summary>
        /// Kiểm tra hồ sơ có phải chứng thực chữ ký người dịch không
        /// </summary>
        private static bool IsChungThucChuKyNguoiDich(HoSoCCCT hoSo)
        {
            return GetLoaiChungThuc(hoSo) == LoaiChungThuc.ChuKyNguoiDich;
        }

        /// <summary>
        /// Kiểm tra hồ sơ có phải chứng thực hợp đồng, giao dịch không
        /// </summary>
        private static bool IsChungThucHopDong(HoSoCCCT hoSo)
        {
            return GetLoaiChungThuc(hoSo) == LoaiChungThuc.HopDongGiaoDich;
        }

        /// <summary>
        /// Enum loại chứng thực
        /// </summary>
        private enum LoaiChungThuc
        {
            BanSao,
            ChuKy,
            ChuKyNguoiDich,
            HopDongGiaoDich
        }

        #endregion
    }
}
