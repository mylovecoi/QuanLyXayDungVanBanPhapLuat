using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Entities.Manages.ThongTinHoSo;
using DataAccess.Entities.Settings;
using FluentValidation;
using Services.DTOs.Manages.ThongTinHoSo;
using Services.DTOs.Manages.ThongTinHoSo.ExportData;
using Services.Settings;
using Services.Settings.DanhMucDungChung.DmHopDong;

namespace Services.Manages.ThongTinHoSo
{
    internal class HoSoCCCTBaoCaoValidate : AbstractValidator<ReportRequestDto>
    {
        private readonly IDanhMucDonViService _danhMucDonViService;
        private readonly IDmHopDongService _dmHopDongService;
        private const string strMesNotEmpty = "không được để trống";
        private bool _IsValidSoCongChung { get; set; }
        public HoSoCCCTBaoCaoValidate(IDanhMucDonViService danhMucDonViService, IDmHopDongService dmHopDongService, bool isValidSoCongChung = true)
        {
            _danhMucDonViService = danhMucDonViService;
            _dmHopDongService = dmHopDongService;
            _IsValidSoCongChung = isValidSoCongChung;
            InitValidationRules();
        }

        private void InitValidationRules()
        {
            if (!_IsValidSoCongChung)
            {
                ValidateDonVi();
                ValidateHopDong();
            }

            ValidateNgayTu();
            ValidateNgayDen();
            ValidateNgayBaoCao();
        }

        private void ValidateDonVi()
        {
            string fieldName = Helper.CapitalizeFirstLetter(Helper.GetDisplayName<ReportRequestDto, Guid>(x => x.DonViId));
            RuleFor(x => x.DonViId).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage($"{fieldName} {strMesNotEmpty}");
        }

        private void ValidateHopDong()
        {
            string fieldName = Helper.CapitalizeFirstLetter(Helper.GetDisplayName<ReportRequestDto, List<Guid>>(x => x.DanhMucHopDongIds));
            RuleFor(x => x.DanhMucHopDongIds).Cascade(CascadeMode.Stop);
        }

        private void ValidateNgayTu()
        {
            string fieldName = Helper.CapitalizeFirstLetter(Helper.GetDisplayName<ReportRequestDto, DateTime>(x => x.NgayBaoCaoTu));
            string fieldName2 = Helper.CapitalizeFirstLetter(Helper.GetDisplayName<ReportRequestDto, DateTime>(x => x.NgayBaoCaoDen));
            RuleFor(x => x.NgayBaoCaoTu).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage($"{fieldName} {strMesNotEmpty}")
                .Must(date => date.Date <= DateTime.Now.Date).WithMessage($"{fieldName} phải nhỏ hơn hoặc bằng hiện tại")
                .LessThan(x => x.NgayBaoCaoDen).WithMessage($"{fieldName} phải nhỏ hơn {fieldName2}");
        }

        private void ValidateNgayDen()
        {
            string fieldName = Helper.CapitalizeFirstLetter(Helper.GetDisplayName<ReportRequestDto, DateTime>(x => x.NgayBaoCaoDen));
            string fieldName2 = Helper.CapitalizeFirstLetter(Helper.GetDisplayName<ReportRequestDto, DateTime>(x => x.NgayBaoCaoTu));
            RuleFor(x => x.NgayBaoCaoDen).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage($"{fieldName} {strMesNotEmpty}")
                .Must(date =>
                {
                    var endOfYear = new DateTime(DateTime.Now.Year, 12, 31);
                    return date.Date <= endOfYear;
                }).WithMessage($"{fieldName} phải nhỏ hơn hoặc bằng ngày 31/12/{DateTime.Now.Year}")
                .GreaterThan(x => x.NgayBaoCaoTu).WithMessage($"{fieldName} phải lớn hơn {fieldName2}");
        }

        private void ValidateNgayBaoCao()
        {
            string fieldName = Helper.CapitalizeFirstLetter(Helper.GetDisplayName<ReportRequestDto, DateTime>(x => x.NgayBaoCao));
            RuleFor(x => x.NgayBaoCao).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage($"{fieldName} {strMesNotEmpty}")
                .Must(date => date.Date <= DateTime.Now.Date).WithMessage($"{fieldName} phải nhỏ hơn hoặc bằng hiện tại");
        }
    }

    internal class ExportDataRequestDtoValidate : AbstractValidator<ExportDataRequestDto>
    {
        private readonly IDanhMucDonViService _danhMucDonViService;
        private readonly IDmHopDongService _dmHopDongService;
        private const string strMesNotEmpty = "không được để trống";

        public ExportDataRequestDtoValidate(IDanhMucDonViService danhMucDonViService, IDmHopDongService dmHopDongService)
        {
            _danhMucDonViService = danhMucDonViService;
            _dmHopDongService = dmHopDongService;
            ValidateNamKetXuat();
            ValidateDmDonVi();
            ValidateDanhMucHoSos();
        }

        private void ValidateNamKetXuat()
        {
            string fieldName = "Năm kết xuất";
            RuleFor(x => x.NamKetXuat).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage($"{fieldName} {strMesNotEmpty}")
                .GreaterThan(0).WithMessage($"{fieldName} không được nhỏ hơn 0")
                .InclusiveBetween(1900, DateTime.Now.Year).WithMessage($"{fieldName} phải nằm trong khoảng từ 1900 đến {DateTime.Now.Year}"); ;
        }

        private void ValidateDmDonVi()
        {
            string fieldName = "Đơn vị kết xuât";
            RuleFor(x => x.DonViId).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage($"{fieldName} {strMesNotEmpty}")
                .MustAsync(async (dmId, _) =>
                {
                    var dmExisting = await _danhMucDonViService.EditAsync(dmId);

                    return dmExisting.Status == "success" && dmExisting.Data is DanhMucDonVi;
                }).WithMessage($"{fieldName} vừa chọn không còn khả dụng.");
        }

        private void ValidateDanhMucHoSos()
        {
            string fieldName = "Nghiệp vụ";
            RuleFor(x => x.HopDongIds).Cascade(CascadeMode.Stop)
                .CustomAsync(async (ids, context, _) =>
                {
                    int indexer = 1;
                    foreach (var i in ids)
                    {
                        var hopDongExisting = await _dmHopDongService.GetEntityByIdAsync(i, true);
                        if (hopDongExisting == null)
                        {
                            context.AddFailure($"{fieldName} thứ {indexer} không hợp lệ hoặc đã bị xoá.");
                            return;
                        }
                        indexer++;
                    }
                }).When(x => x.HopDongIds != null && x.HopDongIds.Any());
        }
    }
}
