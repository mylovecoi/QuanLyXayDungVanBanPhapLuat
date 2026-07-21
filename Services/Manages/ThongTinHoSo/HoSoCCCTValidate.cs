using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Entities.Manages.ThongTinHoSo;
using DataAccess.Entities.Settings;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Org.BouncyCastle.Crypto;
using Services.DTOs.Manages.ThongTinHoSo;
using Services.DTOs.Manages.ThongTinHoSo.ExportData;
using Services.Settings;
using Services.Settings.DanhMucDungChung.DmHopDong;
using Services.Systems;

namespace Services.Manages.ThongTinHoSo
{
    internal class HoSoCCCTDtoValidate : AbstractValidator<HoSoCCCTDto>
    {
        private readonly IHoSoCCCTService _hoSoCCCTService;
        private readonly IDmHopDongService _dmHopDongService;
        private readonly IOptionDataService _optionDataService;
        private readonly IDmHopDongChiTietService _dmHopDongChiTietService;
        private int _minLength = 0, _maxLength = 0;
        private const string strMesNotEmpty = "không được để trống";
        private const string strMesMinLength = "không được ít hơn {0} ký tự";
        private const string strMesMaxLength = "không được vượt quá {0} ký tự";

        public HoSoCCCTDtoValidate( IHoSoCCCTService hoSoCCCTService, IOptionDataService optionDataService, IDmHopDongService dmHopDongService, IDmHopDongChiTietService dmHopDongChiTietService)
        {
            _hoSoCCCTService = hoSoCCCTService;
            _dmHopDongService = dmHopDongService;
            _optionDataService = optionDataService;
            _dmHopDongChiTietService = dmHopDongChiTietService;
            ValidateDmHopDong();
            ValidateSoHopDong();
            ValidateNgayThuLy();
            ValidateThongTinNguoiYeuCau();
            ValidateThongTinDynamic();
        }

        private void ValidateDmHopDong()
        {
            string fieldName = Helper.CapitalizeFirstLetter(Helper.GetDisplayName<HoSoCCCT, Guid>(x => x.LoaiHopDongId));
            RuleFor(x => x.LoaiHopDongId).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage($"{fieldName} {strMesNotEmpty}")
                .MustAsync(async (dmId, _) =>
                {
                    var dmExisting = await _dmHopDongService.GetEntityByIdAsync(dmId, true);
                    return dmExisting != null;
                }).WithMessage($"{fieldName} vừa chọn không còn khả dụng.");
        }

        private void ValidateSoHopDong()
        {
            _minLength = 0; _maxLength = 100;
            string fieldName = Helper.CapitalizeFirstLetter(Helper.GetDisplayName<HoSoCCCT, string>(x => x.MaSoHoSo));
            RuleFor(x => x.MaSoHoSo).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage(x => $"{fieldName} {strMesNotEmpty}")
                .Must(x => string.IsNullOrWhiteSpace(x) || x == x.Trim()).WithMessage(x => $"{fieldName} không được chứa khoảng trắng đầu/cuối.")
                .MinimumLength(_minLength).WithMessage(x => $"{fieldName} {string.Format(strMesMinLength, _minLength)}")
                .MaximumLength(_maxLength).WithMessage(x => $"{fieldName} {string.Format(strMesMaxLength, _maxLength)}")
                .MustAsync(async (hodong, maHopDong, _) =>
                {
                    var danhMucExiting = await _hoSoCCCTService.GetEntityByMaAsync(maHopDong);
                    if (danhMucExiting != null)
                        if (hodong.Id == danhMucExiting.Id) return true;
                    return danhMucExiting == null;
                }).WithMessage(x => $"{fieldName} đã tồn tại trong hệ thống.");
        }

        private void ValidateNgayThuLy()
        {
            string fieldName = Helper.CapitalizeFirstLetter(Helper.GetDisplayName<HoSoCCCT, DateTime>(x => x.NgayThuLy));
            RuleFor(x => x.NgayThuLy).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage(x => $"{fieldName} {strMesNotEmpty}")
                .LessThanOrEqualTo(DateTime.Today).WithMessage($"{fieldName} không được vượt quá ngày hiện tại.")
                .Must(date => date != DateTime.MinValue).WithMessage($"{fieldName} không được để mặc định."); ;
        }

        #region Thong tin người yêu cầu
        private void ValidateThongTinNguoiYeuCau()
        {
            ValidateHoTen();
            ValidateTheCCCD();
            ValidateThongTinDonVi();
        }

        private void ValidateHoTen()
        {
            _minLength = 0; _maxLength = 100;
            string fieldName = Helper.CapitalizeFirstLetter(Helper.GetDisplayName<HoSoCCCTDto, string?>(x => x.HoTenNguoiNop));
            RuleFor(x => x.HoTenNguoiNop).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage(x => $"{fieldName} {strMesNotEmpty}")
                .MinimumLength(_minLength).WithMessage($"{fieldName}  {string.Format(strMesMinLength, _minLength)}")
                .MaximumLength(_maxLength).WithMessage($"{fieldName}  {string.Format(strMesMaxLength, _maxLength)}");
        }

        private void ValidateTheCCCD()
        {
            _minLength = 0; _maxLength = 12;
            string fieldName = Helper.CapitalizeFirstLetter(Helper.GetDisplayName<HoSoCCCTDto, string?>(x => x.SoCCCDNguoiNop));
            RuleFor(x => x.SoCCCDNguoiNop).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage(x => $"{fieldName} {strMesNotEmpty}")
                .Matches("^[0-9]*$").WithMessage($"{fieldName} phải là chữ số")
                .MinimumLength(_minLength).WithMessage($"{fieldName}  {string.Format(strMesMinLength, _minLength)}")
                .MaximumLength(_maxLength).WithMessage($"{fieldName}  {string.Format(strMesMaxLength, _maxLength)}");
        }

        private void ValidateThongTinDonVi()
        {
            _minLength = 0; _maxLength = 1000;
            string fieldName = Helper.CapitalizeFirstLetter(Helper.GetDisplayName<HoSoCCCTDto, string?>(x => x.ThongTinDonVi));
            RuleFor(x => x.ThongTinDonVi).Cascade(CascadeMode.Stop)
                .MinimumLength(_minLength).WithMessage($"{fieldName}  {string.Format(strMesMinLength, _minLength)}")
                .MaximumLength(_maxLength).WithMessage($"{fieldName}  {string.Format(strMesMaxLength, _maxLength)}");
        }
        #endregion

        #region Thông tin khác
        private void ValidateThongTinDynamic()
        {
            RuleFor(x => x.HoSoCCCTChiTietDtos).CustomAsync(async (list, context, _) =>
            {
                var dto = context.InstanceToValidate as HoSoCCCTDto;

                var loaiHopDongs = await _dmHopDongChiTietService.GetRawListByDanhMucIdAsync(dto.LoaiHopDongId);
                var loaiHopDongIds = loaiHopDongs.ToDictionary(m => m.Id);

                var codes = list.Where(i => !string.IsNullOrEmpty(i.Code)).Select(i => i.Code!).Distinct().ToList();

                foreach (var item in list)
                {

                    var fieldName = $"{nameof(HoSoCCCTDto.HoSoCCCTChiTietDtos)}[{item.HopDongChiTietId}].Error";

                    if (!loaiHopDongIds.TryGetValue(item.HopDongChiTietId, out var chiTiet)) continue;

                    var empty = string.IsNullOrWhiteSpace(item.Value);

                    if (chiTiet.IsRequired && empty)
                    {
                        context.AddFailure(fieldName, $"{chiTiet.Title} là bắt buộc.");
                        continue;
                    }

                    if (!chiTiet.IsRequired && empty) continue;

                    if (!string.IsNullOrEmpty(item.Code))
                    {
                        switch (item.Code)
                        {
                            case "LoaiTaiSan":
                                if (!Guid.TryParse(item.Value, out var taiSanId))
                                {
                                    context.AddFailure(fieldName, $"{chiTiet.Title} không đúng định dạng.");
                                    continue;
                                }

                                if (taiSanId == Guid.Empty && !item.IsRequired) continue;

                                var checkLoaiTaiSan = await _optionDataService.GetOptionDataByCodeAndIdAsync(item.Code, taiSanId);
                                if (checkLoaiTaiSan == null)
                                {
                                    context.AddFailure(fieldName, $"{chiTiet.Title} giá trị chọn không hợp lệ.");
                                    continue;
                                }
                                break;
                            case "DiaDanh":
                                if (!Guid.TryParse(item.Value, out var diaDanhId))
                                {
                                    context.AddFailure(fieldName, $"{chiTiet.Title} không đúng định dạng.");
                                    continue;
                                }

                                if (diaDanhId == Guid.Empty && !item.IsRequired) continue;

                                //var checkDiaDanh = await _dm.GetOptionDataByCodeAndIdAsync(item.Code, taiSanId);
                                //if (checkLoaiTaiSan == null)
                                //{
                                //    context.AddFailure(fieldName, $"[{chiTiet.Title}] giá trị chọn không hợp lệ.");
                                //}
                                break;
                        }
                    }
                    else
                    {
                        switch (item.Type)
                        {
                            case FieldType.Text:
                            case FieldType.TextArea:
                                _maxLength = item.Type == FieldType.Text ? 100 : 3000;

                                if (!string.IsNullOrEmpty(item.Value) && item.Value.Length > _maxLength)
                                {
                                    context.AddFailure(fieldName, $"{chiTiet.Title} {string.Format(strMesMaxLength, _maxLength)}.");
                                    continue;
                                }
                                break;
                            case FieldType.Number:
                                if (!double.TryParse(item.Value, out var num))
                                {
                                    context.AddFailure(fieldName, $"{chiTiet.Title} không phải là số thực.");
                                    continue;
                                }

                                if (num < 0)
                                {
                                    context.AddFailure(fieldName, $"{chiTiet.Title} không được nhỏ hơn 0.");
                                    continue;
                                }

                                break;
                            case FieldType.Date:
                                if (!DateTime.TryParse(item.Value, out var dateTime))
                                {
                                    context.AddFailure(fieldName, $"{chiTiet.Title} không đúng định dạng ngày tháng năm.");
                                    continue;
                                }

                                if (dateTime.Date > DateTime.Now.Date)
                                {
                                    context.AddFailure(fieldName, $"{chiTiet.Title} không được lớn hơn thời điểm hiện tại.");
                                    continue;
                                }
                                break;
                            case FieldType.Money:

                                break;
                        }
                    }
                }
            });
        }
        #endregion
    }
}
