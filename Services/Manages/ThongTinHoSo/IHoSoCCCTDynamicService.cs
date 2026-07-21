using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess;
using DataAccess.Entities.Manages.ThongTinHoSo;
using Microsoft.EntityFrameworkCore;
using Services.DTOs.Manages.ThongTinHoSo;
using Services.Model;
using Services.Settings.DanhMucDungChung.DmHopDong;
using Services.Settings.DanhMucDungChung;
using Services.Settings;
using Services.Systems;
using Services.Helpers;
using Microsoft.Extensions.Hosting;
using Azure;
using System.Net.WebSockets;

namespace Services.Manages.ThongTinHoSo
{
    public interface IHoSoCCCTDynamicService
    {
        Task<CommonResponse> GetSingleByIdAsync(Guid hoSoId);
        Task<CommonResponse> InitDataForCreate(Guid dmHopDongId, Guid donViId);
        Task<CommonResponse> ValidateRequestAsync(HoSoCCCTDto request);
        Task<CommonResponse> StoreAsync(HoSoCCCTDto request);
        Task<CommonResponse> UpdateAsync(HoSoCCCTDto request);
        Task<CommonResponse> DeleteAsync(Guid hoSoId);
    }

    public class HoSoCCCTDynamicService(
        ApplicationDbContext dbContext,
        IHoSoCCCTService hoSoCCCTService,
        IDmHopDongService dmHopDongService,
        IOptionDataService optionDataService,
        IDanhMucDonViService danhMucDonViService,
        IAttachedFileService attachedFileService,
        IDanhMucPhiLePhiService danhMucPhiLePhiService,
        IDmHopDongChiTietService dmHopDongChiTietService
        ) : IHoSoCCCTDynamicService
    {
        private readonly ApplicationDbContext _dbContext = dbContext;
        private readonly IHoSoCCCTService _hoSoCCCTService = hoSoCCCTService;
        private readonly IDmHopDongService _dmHopDongService = dmHopDongService;
        private readonly IOptionDataService _optionDataService = optionDataService;
        private readonly IDanhMucDonViService _danhMucDonViService = danhMucDonViService;
        private readonly IAttachedFileService _attachedFileService = attachedFileService;
        private readonly IDanhMucPhiLePhiService _danhMucPhiLePhiService = danhMucPhiLePhiService;
        private readonly IDmHopDongChiTietService _dmHopDongChiTietService = dmHopDongChiTietService;

        public async Task<CommonResponse> GetSingleByIdAsync(Guid hoSoId)
        {
            try
            {
                var existingHoSo = await _hoSoCCCTService.GetEntityByIdAsync(hoSoId, true);

                if (existingHoSo == null) return new("error", "Hồ sơ vừa chọn không còn tồn tại. Hãy kiểm tra lại.");

                var metas = await _dmHopDongChiTietService.GetRawListByDanhMucIdAsync(existingHoSo.LoaiHopDongId);

                //if (!metas.Any()) return new("error", "Hồ sơ vừa chọn không còn tồn tại V2. Hãy kiểm tra lại.");

                var newDto = HoSoCCCTMapper.MapFrom(existingHoSo);

                var result = metas.Select(meta =>
                {
                    var value = existingHoSo.HoSoCCCTChiTiets.FirstOrDefault(x => x.DanhMucHopDongChiTietId == meta.Id);

                    var newChiTiet = new HoSoCCCTChiTietDto()
                    {
                        Id = value?.Id ?? Guid.Empty,
                        HoSoId = existingHoSo.Id,
                        HopDongChiTietId = meta.Id,
                        Title = meta.Title,
                        Type = meta.Type,
                        ColSize = meta.ColSize,
                        Code = meta.Code,
                        Order = meta.Order,
                        IsRequired = meta.IsRequired,
                        Value = value?.Value ?? string.Empty
                    };
                    return newChiTiet;
                }).ToList();

                newDto.HoSoCCCTChiTietDtos = result;

                foreach (var chiTiet in newDto.HoSoCCCTChiTietDtos)
                {
                    var code = chiTiet.Code;
                    if (!string.IsNullOrEmpty(code))
                    {
                        switch (code)
                        {
                            case "LoaiTaiSan":
                                var loaiTaiSans = await _dbContext.OptionDatas.AsNoTracking().Where(x => x.Code == code).ToListAsync();
                                if (loaiTaiSans.Any())
                                    chiTiet.Options = loaiTaiSans;
                                break;
                        }
                    }
                }

                var files = await _attachedFileService.GetAllAttachedFilesAsync(existingHoSo.Id, nameof(HoSoCCCT));
                if (files.Count() > 0)
                    newDto.AttachedFiles = files;

                var chuKyDienTu = await _attachedFileService.GetAllAttachedFilesAsync(existingHoSo.Id, "ChuKyDienTu");
                newDto.AttachedFiles.AddRange(chuKyDienTu);

                return new("success", "Thành Công", newDto);
            }
            catch (Exception)
            {
                return new("error", "Có lỗi trong quá trình truy vấn dữ liệu. Hãy kiểm tra lại!");
            }
        }

        public async Task<CommonResponse> InitDataForCreate(Guid dmHopDongId, Guid donViId)
        {
            try
            {
                var donViExisting = await _danhMucDonViService.EditAsync(donViId);
                if (donViExisting == null) return new("error", "Đơn vị đang làm việc không còn khả dụng. Hãy kiểm tra lại");

                var loaiHopDongExisting = await _dmHopDongService.GetEntityByIdAsync(dmHopDongId, isAsNoTracking: true);
                if (loaiHopDongExisting == null) return new("error", "Nghiệp vụ vừa chọn không còn khả dụng. Hãy kiểm tra lại");

                if (!loaiHopDongExisting.ParentId.HasValue) return new("error", "Nghiệp vụ vừa chọn không phù hợp. Hãy kiểm tra lại");

                await _hoSoCCCTService.RemoveHoSoDataRedundantAsync();

                var model = new HoSoCCCT()
                {
                    Id = Guid.NewGuid(),
                    LoaiHopDongId = dmHopDongId,
                    DonViQuanLyId = donViId,
                    NgayThuLy = DateTime.Now,
                    PhuongThucCongChung = true,
                    Status = "CXD"
                };

                var loaiGiayTo = await _dbContext.OptionDatas.AsNoTracking().Where(x => x.Code == "LoaiGiayTo" && loaiHopDongExisting.LoaiGiayTo.Contains(x.Value ?? string.Empty)).ToListAsync();

                foreach (var giayTo in loaiGiayTo)
                {
                    if (model.AttachedFiles == null) model.AttachedFiles = new();
                    model.AttachedFiles.Add(new()
                    {
                        GroupId = model.Id,
                        TableName = nameof(HoSoCCCT),
                        MoTa = giayTo.DisplayName,
                        Status = "CXD",
                    });
                }

                await _attachedFileService.RemoveDataRedundantAsync(nameof(HoSoCCCT));
                await _dbContext.AttachedFiles.AddRangeAsync(model.AttachedFiles);
                await _dbContext.HoSoCCCTs.AddAsync(model);

                if (await _danhMucDonViService.GetTinhNangThanhToanStatusAsync() ?? false)
                {
                    var listDMLePhi = await _danhMucPhiLePhiService.GetListDanhMucPhiLePhiByLoaiHopDongId(model.LoaiHopDongId);

                    foreach (var item in listDMLePhi)
                    {
                        model.HoSoCCCTChiPhis.Add(new()
                        {
                            HoSoId = model.Id,
                            SoLuong = 0,
                            SoLuongToiDa = item.SoLuongToiDa,
                            PhiCoDinh = item.PhiCoDinh,
                            TyLeVuotMuc = item.TyLeVuotMuc,
                            MoTa = item.MoTa,
                            PhiToiDa = item.PhiToiDa,
                            NguongVuotMuc = item.NguongVuotMuc,
                            DonViTinh = item.DonViTinh,
                            ThanhTien = 0
                        });
                    }

                    await _dbContext.HoSoCCCTChiPhis.AddRangeAsync(model.HoSoCCCTChiPhis);
                }

                await _dbContext.SaveChangesAsync();

                model.LoaiHopDong = loaiHopDongExisting;


                var response = HoSoCCCTMapper.MapFrom(model);

                response.HoSoCCCTChiTietDtos = loaiHopDongExisting.HopDongChiTiet.Select(chiTietHopDong => new HoSoCCCTChiTietDto()
                {
                    HoSoId = model.Id,
                    HopDongChiTietId = chiTietHopDong.Id,
                    Title = chiTietHopDong.Title,
                    Type = chiTietHopDong.Type,
                    ColSize = chiTietHopDong.ColSize,
                    Code = chiTietHopDong.Code,
                    Order = chiTietHopDong.Order,
                    IsRequired = chiTietHopDong.IsRequired,
                }).OrderBy(x => x.Order).ToList();

                foreach (var chiTiet in response.HoSoCCCTChiTietDtos)
                {
                    var code = chiTiet.Code;
                    if (!string.IsNullOrEmpty(code))
                    {
                        switch (code)
                        {
                            case "LoaiTaiSan":
                                var loaiTaiSans = await _dbContext.OptionDatas.AsNoTracking().Where(x => x.Code == code).ToListAsync();
                                if (loaiTaiSans.Any())
                                    chiTiet.Options = loaiTaiSans;
                                break;
                        }
                    }
                }

                return new("success", "Khởi tạo dữ liệu thành công", response);
            }
            catch (Exception)
            {
                return new("error", "Có lỗi trong quá trình truy vấn dữ liệu. Hãy kiểm tra lại!");
            }
        }

        public async Task<CommonResponse> ValidateRequestAsync(HoSoCCCTDto request)
        {
            var resultValidate = await new HoSoCCCTDtoValidate(_hoSoCCCTService, _optionDataService, _dmHopDongService, _dmHopDongChiTietService).ValidateAsync(request);
            if (!resultValidate.IsValid)
            {
                var attachedFileTemp = await _attachedFileService.GetAllAttachedFilesAsync(request.Id, nameof(HoSoCCCT));
                if (attachedFileTemp != null)
                    request.AttachedFiles = attachedFileTemp;
                var listLePhi = await _hoSoCCCTService.GetListLePhiByHoSoIdAsync(request.Id);
                if (listLePhi.Any())
                    request.HoSoCCCTChiPhis = listLePhi;

                foreach (var chiTiet in request.HoSoCCCTChiTietDtos)
                {
                    var code = chiTiet.Code;
                    if (!string.IsNullOrEmpty(code))
                    {
                        switch (code)
                        {
                            case "LoaiTaiSan":
                                var loaiTaiSans = await _dbContext.OptionDatas.AsNoTracking().Where(x => x.Code == code).ToListAsync();
                                if (loaiTaiSans.Any())
                                    chiTiet.Options = loaiTaiSans;
                                break;
                        }
                    }
                }

                return new("error", Helper.GetValidationErrorsDictionary(resultValidate), request, "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại!");
            }
            return new("success");
        }

        public async Task<CommonResponse> StoreAsync(HoSoCCCTDto request)
        {
            try
            {
                request.Status = "CC";
                HoSoCCCT newHoSo = new();
                HoSoCCCTMapper.MapTo(request, newHoSo);

                var listLePhi = await _hoSoCCCTService.GetListLePhiByHoSoIdAsync(request.Id);

                foreach (var item in listLePhi)
                {
                    item.Status = true;
                }

                var listChiTiet = request.HoSoCCCTChiTietDtos.Select(x => new HoSoCCCTChiTiet()
                {
                    HoSoId = x.HoSoId,
                    DanhMucHopDongChiTietId = x.HopDongChiTietId,
                    Value = x.Value?.Trim() ?? string.Empty
                });

                newHoSo.DiaBan = null;
                newHoSo.LoaiHopDong = null;

                _dbContext.HoSoCCCTs.Update(newHoSo);
                _dbContext.HoSoCCCTChiPhis.UpdateRange(listLePhi);
                await _dbContext.HoSoCCCTChiTiets.AddRangeAsync(listChiTiet);
                await _dbContext.SaveChangesAsync();

                await _attachedFileService.UpdateRangeStatus(request.Id, nameof(HoSoCCCT));
                return new("success", "Thêm mới dữ liệu thành công");
            }
            catch (Exception)
            {
                return new("error", "Có lỗi trong quá trình cập nhật dữ liệu. Hãy kiểm tra lại!", request);
            }
        }

        public async Task<CommonResponse> UpdateAsync(HoSoCCCTDto request)
        {
            try
            {
                var existingHoSo = await _hoSoCCCTService.GetEntityByIdAsync(request.Id, isAsNoTracking: false);
                if (existingHoSo == null) return new("error", "Không tìm thấy thông tin danh mục. Hãy kiểm tra lại!");

                HoSoCCCTMapper.MapTo(request, existingHoSo);

                existingHoSo.DiaBan = null;
                existingHoSo.LoaiHopDong = null;

                var existingChiTiets = await _dbContext.HoSoCCCTChiTiets.AsNoTracking().Where(x => x.HoSoId == existingHoSo.Id).ToListAsync();
                var incomingChiTietDict = request.HoSoCCCTChiTietDtos.ToDictionary(x => x.HopDongChiTietId);

                foreach (var chiTiet in existingChiTiets)
                {
                    if (incomingChiTietDict.TryGetValue(chiTiet.DanhMucHopDongChiTietId, out var updated))
                    {
                        chiTiet.Value = updated.Value?.Trim();
                    }
                }

                _dbContext.HoSoCCCTs.Update(existingHoSo);
                _dbContext.HoSoCCCTChiTiets.UpdateRange(existingChiTiets);
                await _dbContext.SaveChangesAsync();
                return new("success", "Cập nhật dữ liệu thành công");
            }
            catch (Exception)
            {
                return new("error", "Có lỗi trong quá trình cập nhật dữ liệu. Hãy kiểm tra lại!");
            }
        }

        public async Task<CommonResponse> DeleteAsync(Guid hoSoId)
        {
            try
            {
                var existingHoSo = await _hoSoCCCTService.GetEntityByIdAsync(hoSoId, isAsNoTracking: false);
                if (existingHoSo == null) return new("error", "Không tìm thấy thông tin danh mục. Hãy kiểm tra lại!");

                var existingChiTiets = await _dbContext.HoSoCCCTChiTiets.AsNoTracking().Where(x => x.HoSoId == existingHoSo.Id).ToListAsync();

                _dbContext.HoSoCCCTChiTiets.RemoveRange(existingChiTiets);
                _dbContext.HoSoCCCTs.Remove(existingHoSo);
                await _dbContext.SaveChangesAsync();

                return new("success", $"Xóa hồ sơ: {existingHoSo.MaSoHoSo} thành công");
            }
            catch (Exception)
            {
                return new("error", "Có lỗi trong quá trình cập nhật dữ liệu. Hãy kiểm tra lại!");
            }
        }
    }
}
