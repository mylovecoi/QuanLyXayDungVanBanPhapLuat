using System;

namespace Services.ThamDinhGia
{
    public static class ThamDinhGiaMapper
    {
        public static void CopyTo(DataAccess.Entities.ThamDinhGia.ThamDinhGia source, DataAccess.Entities.ThamDinhGia.ThamDinhGia target)
        {
            target.DiaBanId = source.DiaBanId;
            target.DiaDiem = source.DiaDiem;
            target.DvYeuCau = source.DvYeuCau;
            target.DonViThamDinhId = source.DonViThamDinhId;
            target.DonViChuQuanId = source.DonViChuQuanId;
            target.HoiDongId = source.HoiDongId;
            target.ThoiHan = source.ThoiHan;
            target.SoTbKl = source.SoTbKl;
            target.HangHoaId = source.HangHoaId;
            target.PhanLoai = source.PhanLoai;
            target.SoQdPheDuyet = source.SoQdPheDuyet;
            target.NgayQdPheDuyet = source.NgayQdPheDuyet;
            target.SoNgayKq = source.SoNgayKq;
            target.TtTsTd = source.TtTsTd;
            target.GhiChu = source.GhiChu;
            target.Thoidiem = source.Thoidiem;
            target.ThongTin = source.ThongTin;
            target.ChiTietExcel = source.ChiTietExcel;
            target.UpdatedDate = DateTime.Now;
        }
    }
}
