using DataAccess.Entities.DinhGiaHHDV;
using System;

namespace Services.DinhGiaHHDV.DinhGiaKhac
{
    internal static class DinhGiaMapper
    {
        public static void CopyTo(DinhGia source, DinhGia target, bool isNew = false)
        {
            if (isNew)
            {
                target.MaNghe = source.MaNghe;
            }
            target.SoQd = source.SoQd;
            target.ThoiDiem = source.ThoiDiem;
            target.MoTa = source.MoTa;
            target.GhiChu = source.GhiChu;
            target.ChiTietExcel = source.ChiTietExcel;
            target.UpdatedDate = DateTime.Now;
        }
    }
}
