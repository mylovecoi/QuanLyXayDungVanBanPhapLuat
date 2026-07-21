using System;
using DataAccess.Entities.KeKhaiDangKyGia;

namespace Services.KeKhaiDangKyGia
{
    internal static class DoanhNghiepMapper
    {
        public static void CopyTo(DoanhNghiep source, DoanhNghiep target)
        {
            target.TenDoanhNghiep = source.TenDoanhNghiep;
            target.DiaChi = source.DiaChi;
            target.SoDienThoai = source.SoDienThoai;
            target.Email = source.Email;
            target.TrangThai = "Chờ kích hoạt";
            target.UpdatedDate = DateTime.Now;
        }
    }

    internal static class KeKhaiDangKyGiaMapper
    {
        public static void CopyTo(DataAccess.Entities.KeKhaiDangKyGia.KeKhaiDangKyGia source, DataAccess.Entities.KeKhaiDangKyGia.KeKhaiDangKyGia target)
        {
            target.SoQd = source.SoQd;
            target.NgayQd = source.NgayQd;
            target.NgayThucHien = source.NgayThucHien;
            target.DonViTinh = source.DonViTinh;
            target.GhiChu = source.GhiChu;
            target.ThoiDiem = source.ThoiDiem == DateTime.MinValue ? source.NgayQd : source.ThoiDiem;
            target.ChiTietExcel = source.ChiTietExcel;
            target.ThoiGianThucHien = source.ThoiGianThucHien;
            target.SoQdLk = source.SoQdLk;
            target.NgayQdLk = source.NgayQdLk;
            target.YtCauThanhGia = source.YtCauThanhGia;
            target.ThyDgGadGia = source.ThyDgGadGia;
            target.DonViDongChuyenId = source.DonViDongChuyenId;
        }
    }
}
