using DataAccess.Entities.Manages.ThongTinHoSo;
using Services.DTOs.Manages.ThongTinHoSo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Manages.ThongTinHoSo
{
    internal static class HoSoCCCTMapper
    {
        public static void MapTo(HoSoCCCT source, HoSoCCCT? existing = null)
        {
            var entity = existing ?? new HoSoCCCT();

            entity.MaSoHoSo = source.MaSoHoSo.Trim();

            entity.HoTenNguoiNop = source.HoTenNguoiNop?.Trim();
            entity.SoCCCDNguoiNop = source.SoCCCDNguoiNop?.Trim();
            entity.SDTNguoiNop = source.SDTNguoiNop?.Trim();
            entity.ThongTinDonVi = source.ThongTinDonVi?.Trim();

            entity.NgayThuLy = source.NgayThuLy;
            entity.GiaTriHopDong = source.GiaTriHopDong;

            entity.ThongTinBenA = source.ThongTinBenA?.Trim();
            entity.ThongTinBenB = source.ThongTinBenB?.Trim();
            entity.NoiDungHoSo = source.NoiDungHoSo?.Trim();

            entity.LoaiTaiSanId = source.LoaiTaiSanId;
            entity.ThongTinChiTietTaiSan = source.ThongTinChiTietTaiSan?.Trim();
            entity.DiaBanId = source.DiaBanId;

            entity.PhuongThucCongChung = source.PhuongThucCongChung;

            entity.TenNganHang = source.TenNganHang?.Trim();
            entity.CanBoTinDung = source.CanBoTinDung?.Trim();
            entity.ChietKhau = source.ChietKhau;

            entity.SoTrang = source.SoTrang;
            entity.SoVanBan = source.SoVanBan;
            entity.NoiLuuTru = source.NoiLuuTru?.Trim();

            entity.MoTa = source.MoTa?.Trim();

        }

        public static void MapTo(HoSoCCCTDto dto, HoSoCCCT entity)
        {
            entity.Id = dto.Id;
            entity.DonViQuanLyId = dto.DonViQuanLyId;
            entity.LoaiHopDongId = dto.LoaiHopDongId;
            entity.Status = dto.Status;
            entity.MaSoHoSo = dto.MaSoHoSo?.Trim() ?? string.Empty;
            entity.NgayThuLy = dto.NgayThuLy;
            entity.GiaTriHopDong = dto.GiaTriHopDong;
            entity.PhuongThucCongChung = dto.PhuongThucCongChung;
            entity.HoTenNguoiNop = dto.HoTenNguoiNop?.Trim();
            entity.SoCCCDNguoiNop = dto.SoCCCDNguoiNop?.Trim();
            entity.ThongTinDonVi = dto.ThongTinDonVi?.Trim();
        }

        public static HoSoCCCT MapFrom(HoSoCCCTDto dto)
        {
            return new HoSoCCCT
            {
                Id = dto.Id,

                MaSoHoSo = dto.MaSoHoSo?.Trim() ?? string.Empty,
                HoTenNguoiNop = dto.HoTenNguoiNop?.Trim(),
                SoCCCDNguoiNop = dto.SoCCCDNguoiNop?.Trim(),
                NgayThuLy = dto.NgayThuLy,
                PhuongThucCongChung = dto.PhuongThucCongChung
            };
        }

        public static HoSoCCCTDto MapFrom(HoSoCCCT entity)
        {
            return new()
            {
                Id = entity.Id,
                Status = entity.Status ?? "CXD",
                MaSoHoSo = entity.MaSoHoSo,
                LoaiHopDongId = entity.LoaiHopDongId,
                LoaiHopDong = entity.LoaiHopDong,
                DonViQuanLyId = entity.DonViQuanLyId,
                NgayThuLy = entity.NgayThuLy,
                GiaTriHopDong = entity.GiaTriHopDong,
                HoTenNguoiNop = entity.HoTenNguoiNop,
                SoCCCDNguoiNop = entity.SoCCCDNguoiNop,
                ThongTinDonVi = entity.ThongTinDonVi,
                PhuongThucCongChung = entity.PhuongThucCongChung,
                NgayDuyet = entity.NgayDuyet,
                HoSoCCCTChiPhis = entity.HoSoCCCTChiPhis,
                AttachedFiles = entity.AttachedFiles,
            };
        }
    }
}
