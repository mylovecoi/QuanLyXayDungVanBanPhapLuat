using DataAccess.Entities.Settings;
using Services.DTOs.BaoCaoKhac;
using System.Globalization;

namespace Services.ReportGenerators
{
    public static class BaoCaoSuDungLaoDongExtensions
    {
        public static Dictionary<string, string> GetWordExportData(this BaoCaoSuDungLaoDongResponse response)
        {
            ArgumentNullException.ThrowIfNull(response);

            return new Dictionary<string, string>
            {
                // Thông tin tổ chức
                {"{TenToChuc}", response.ThongTinToChuc.TenToChuc ?? ""},
                {"{TinhThanhPho}", response.ThongTinToChuc.TinhThanhPho ?? ""},
                {"{QuyenSo}", response.ThongTinToChuc.QuyenSo ?? ""},
                {"{NgayMoSo}", FormatDateToVietnamese(response.ThongTinToChuc.NgayMoSo)},
                {"{NgayKhoaSo}", FormatDateToVietnamese(response.ThongTinToChuc.NgayKhoaSo)},

                // Thống kê tổng hợp
                {"{TongSoLaoDong}", response.ThongKe.TongSoLaoDong.ToString()},
                {"{SoCongChungVien}", response.ThongKe.SoCongChungVien.ToString()},
                {"{NhanVienNghiepVu}", response.ThongKe.SoNhanVienNghiepVu.ToString()},
                {"{NhanVienKhac}", response.ThongKe.SoNhanVienKhac.ToString()},
                {"{TongSoHopDong}", response.ThongKe.TongSoHopDongDaKy.ToString()},
                {"{HopDongDaChamDut}", response.ThongKe.SoHopDongDaChamDut.ToString()},
                {"{HopDongDangThucHien}", response.ThongKe.SoHopDongDangThucHien.ToString()},
                {"{TongTienBaoHiemTrachNhiem}", FormatCurrency(response.ThongKe.TongTienBaoHiemTrachNhiem)},
                {"{TongTienBHXH}", FormatCurrency(response.ThongKe.TongTienBHXH)},
                {"{TongTienBHYT}", FormatCurrency(response.ThongKe.TongTienBHYT)},

                // Thông tin ngày ký
                {"{NgayKy}", FormatDateToVietnamese(response.ThongKe.NgayBaoCao)},
                {"{TinhThanhPhoKy}", response.ThongKe.DiaDanh ?? ""},
                {"{NgayBaoCao}", response.ThongKe.NgayBaoCao.ToString("dd/MM/yyyy")}
            };
        }

        public static List<string[]> GetCongChungVienForWordExport(this BaoCaoSuDungLaoDongResponse response)
        {
            var result = new List<string[]>();

            if (response.DanhSachCongChungVien != null)
            {
                for (int i = 0; i < response.DanhSachCongChungVien.Count; i++)
                {
                    var canBo = response.DanhSachCongChungVien[i];
                    result.Add(
                    [
                        (i + 1).ToString(),
                        canBo.TenCanBo ?? "",
                        canBo.NgaySinh?.ToString("dd/MM/yyyy") ?? "",
                        canBo.GioiTinh ? "Nữ" : "Nam",
                        canBo.TrinhDoChuyenMon ?? "",
                        FormatQuyetDinh(canBo.SoQuyetDinhBoNhiem, canBo.NgayQuyetDinhBoNhiem),
                        FormatTheCC(canBo.SoQuyetDinhCapThe, canBo.NgayQuyetDinhCapThe, canBo.SoTheCongChungVien),
                        canBo.ChucVu ?? "",
                        FormatCurrency(canBo.MucPhiBaoHiemTrachNhiem ?? 0),
                        FormatCurrency(canBo.SoTienBHXH),
                        FormatCurrency(canBo.SoTienBHYT),
                        canBo.GhiChu ?? ""
                    ]);
                }
            }

            return result;
        }

        public static List<string[]> GetNhanVienForWordExport(this BaoCaoSuDungLaoDongResponse response)
        {
            var result = new List<string[]>();

            if (response.DanhSachNhanVien != null)
            {
                for (int i = 0; i < response.DanhSachNhanVien.Count; i++)
                {
                    var canBo = response.DanhSachNhanVien[i];
                    result.Add(
                    [
                        (i + 1).ToString(),
                        canBo.TenCanBo ?? "",
                        canBo.NgaySinh?.ToString("dd/MM/yyyy") ?? "",
                        canBo.GioiTinh ? "Nữ" : "Nam",
                        canBo.TrinhDoChuyenMon ?? "",
                        canBo.ViTriViecLam ?? "",
                        FormatNgayTuyenDungHopDong(canBo),
                        FormatCurrency(canBo.SoTienBHXH),
                        FormatCurrency(canBo.SoTienBHYT),
                        canBo.GhiChu ?? ""
                    ]);
                }
            }

            return result;
        }

        private static string FormatDateToVietnamese(DateTime date)
        {
            return $"ngày {date.Day} tháng {date.Month} năm {date.Year}";
        }

        private static string FormatCurrency(decimal amount)
        {
            return $"{amount.ToString("N0", new CultureInfo("vi-VN"))} VNĐ";
        }

        private static string FormatQuyetDinh(string? soQuyetDinh, DateTime? ngayQuyetDinh)
        {
            if (string.IsNullOrEmpty(soQuyetDinh)) return "";

            var result = soQuyetDinh;
            if (ngayQuyetDinh.HasValue)
            {
                result += $" ngày {ngayQuyetDinh.Value:dd/MM/yyyy}";
            }
            return result;
        }

        private static string FormatTheCC(string? soQuyetDinhCapThe, DateTime? ngayQuyetDinhCapThe, string? soTheCongChungVien)
        {
            var parts = new List<string>();

            if (!string.IsNullOrEmpty(soQuyetDinhCapThe))
            {
                var part = soQuyetDinhCapThe;
                if (ngayQuyetDinhCapThe.HasValue)
                {
                    part += $" ngày {ngayQuyetDinhCapThe.Value:dd/MM/yyyy}";
                }
                parts.Add(part);
            }

            if (!string.IsNullOrEmpty(soTheCongChungVien))
            {
                parts.Add($"Số thẻ: {soTheCongChungVien}");
            }

            return string.Join("; ", parts);
        }

        private static string FormatNgayTuyenDungHopDong(DanhMucCanBo canBo)
        {
            var parts = new List<string>();

            if (canBo.NgayTuyenDung.HasValue)
            {
                parts.Add(canBo.NgayTuyenDung.Value.ToString("dd/MM/yyyy"));
            }

            if (!string.IsNullOrEmpty(canBo.SoHopDongLaoDong))
            {
                var hopDongPart = canBo.SoHopDongLaoDong;
                if (canBo.NgayKyHopDongLaoDong.HasValue)
                {
                    hopDongPart += $" ngày {canBo.NgayKyHopDongLaoDong.Value:dd/MM/yyyy}";
                }
                parts.Add(hopDongPart);
            }

            return string.Join("/", parts);
        }
    }
}
