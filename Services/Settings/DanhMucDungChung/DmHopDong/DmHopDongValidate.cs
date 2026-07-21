using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Entities.Settings;
using FluentValidation;
using Services.Systems;

namespace Services.Settings.DanhMucDungChung.DmHopDong
{
    internal class DmHopDongValidate : AbstractValidator<DanhMucHopDong>
    {
        private readonly IDmHopDongService _dmHopDongService;
        private readonly IOptionDataService _optionDataService;
        private int _minLength = 0, _maxLength = 0;
        private const string strMesNotEmpty = "không được để trống";
        private const string strMesMinLength = "không được ít hơn {0} ký tự";
        private const string strMesMaxLength = "không được vượt quá {0} ký tự";

        public DmHopDongValidate(IDmHopDongService dmHopDongService, IOptionDataService optionDataService)
        {
            _dmHopDongService = dmHopDongService;
            _optionDataService = optionDataService;
            ValidateMaHopDong();
            ValidateTenHopDong();
            ValidateParent();
            ValidateLevel();
            ValidateMoTa();
            ValidateLoaiGiayTo();
        }

        private string GetDisplayName(DanhMucHopDong x)
        {
            return x.ParentId == Guid.Empty ? "nhóm nghiệp vụ" : "hợp đồng";
        }

        private void ValidateMaHopDong()
        {
            _minLength = 0; _maxLength = 100;
            RuleFor(x => x.MaHopDong).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage(x => $"Mã {GetDisplayName(x)} {strMesNotEmpty}")
                .Must(x => string.IsNullOrWhiteSpace(x) || x == x.Trim()).WithMessage(x => $"Mã {GetDisplayName(x)} không được chứa khoảng trắng đầu/cuối.")
                .MinimumLength(_minLength).WithMessage(x => $"Mã {GetDisplayName(x)} {string.Format(strMesMinLength, _minLength)}")
                .MaximumLength(_maxLength).WithMessage(x => $"Mã {GetDisplayName(x)} {string.Format(strMesMaxLength, _maxLength)}")
                .MustAsync(async (hodong, maHopDong, _) =>
                {
                    var danhMucExiting = await _dmHopDongService.GetEntityByMaAsync(maHopDong);
                    if (danhMucExiting != null)
                        if (hodong.Id == danhMucExiting.Id) return true;
                    return danhMucExiting == null;
                }).WithMessage(x => $"Mã {GetDisplayName(x)} đã tồn tại trong hệ thống.");
        }

        private void ValidateTenHopDong()
        {
            _minLength = 0; _maxLength = 100;
            RuleFor(x => x.TenHopDong).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage(x => $"Tên {GetDisplayName(x)} {strMesNotEmpty}")
                .MinimumLength(_minLength).WithMessage(x => $"Tên {GetDisplayName(x)} {string.Format(strMesMinLength, _minLength)}")
                .MaximumLength(_maxLength).WithMessage(x => $"Tên {GetDisplayName(x)} {string.Format(strMesMaxLength, _maxLength)}");
        }

        private void ValidateParent()
        {
            RuleFor(x => x.ParentId).Cascade(CascadeMode.Stop)
                .MustAsync(async (id, _) =>
                {
                    if (!id.HasValue || id == Guid.Empty) return true;
                    var existing = await _dmHopDongService.GetEntityByIdAsync(id.Value);
                    return existing != null;
                })
                .WithMessage("Danh mục nhóm nghiệp vụ không còn khả dụng.");
        }

        private void ValidateMoTa()
        {
            _minLength = 0; _maxLength = 200;
            RuleFor(x => x.MoTa).Cascade(CascadeMode.Stop)
                .MinimumLength(_minLength).WithMessage($"{Helper.CapitalizeFirstLetter(Helper.GetDisplayName<DanhMucHopDong, string>(x => x.MoTa!))} {string.Format(strMesMinLength, _minLength)}")
                .MaximumLength(_maxLength).WithMessage($"{Helper.CapitalizeFirstLetter(Helper.GetDisplayName<DanhMucHopDong, string>(x => x.MoTa!))}  {string.Format(strMesMaxLength, _maxLength)}");
        }

        private void ValidateLevel()
        {
            RuleFor(x => x.Level).Cascade(CascadeMode.Stop)
                .LessThan(2).WithMessage("Không được tạo mới danh mục cấp 3");
        }

        private void ValidateLoaiGiayTo()
        {
            RuleFor(x => x.DanhSachOption).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage($"{Helper.CapitalizeFirstLetter(Helper.GetDisplayName<DanhMucHopDong, List<string>>(x => x.DanhSachOption))} {strMesNotEmpty}")
                .When(x => x.ParentId.HasValue && x.ParentId != Guid.Empty)
                .MustAsync(async (model, lstStr, _) =>
                {
                    if (!model.ParentId.HasValue) return true;
                    var options = await _optionDataService.GetDataOptionsByCodeAsync("LoaiGiayTo");
                    var validValues = options.Select(o => o.Value).ToList();

                    var invalids = lstStr.Where(code => !validValues.Contains(code)).ToList();
                    return invalids.Count == 0;
                    //return lstStr.All(code => validValues.Contains(code));
                }).WithMessage($"Có một hoặc nhiều {Helper.GetDisplayName<DanhMucHopDong, List<string>>(x => x.DanhSachOption).ToLower()} không còn khả dụng.");
        }
    }
}
