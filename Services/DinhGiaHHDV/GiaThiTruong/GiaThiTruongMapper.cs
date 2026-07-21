using DataAccess.Entities.DinhGiaHHDV;
using System;

namespace Services.DinhGiaHHDV.GiaThiTruong
{
    internal static class GiaThiTruongMapper
    {
        public static void CopyTo(DataAccess.Entities.DinhGiaHHDV.GiaThiTruong source, DataAccess.Entities.DinhGiaHHDV.GiaThiTruong target)
        {
            target.DiaBanId = source.DiaBanId;
            target.SoQd = source.SoQd;
            target.Thoidiem = source.Thoidiem;
            target.SoQdLk = source.SoQdLk;
            target.ThoiDiemLk = source.ThoiDiemLk;
            target.Thang = source.Thang;
            target.Nam = source.Nam;
            target.GhiChu = source.GhiChu;
            target.ChiTietExcel = source.ChiTietExcel;
            target.UpdatedDate = DateTime.Now;
        }
    }
}
