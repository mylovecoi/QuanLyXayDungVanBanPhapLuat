$(document).ready(function () {
    SetupSelect2('#TargetYear_Select', 'Lựa Chọn Năm Làm Việc', { enableSearch: false, enableClear: false });
    SetupSelect2('#DanhMucDonVi_Select', 'Lựa Chọn Đơn Vị Làm Việc', { enableSearch: true, enableClear: true });
    SetupSelect2('#LoaiHopDong_Select', 'Lựa Chọn Nghiệp Vụ', { enableSearch: true, enableClear: true });
    SetupSelect2('#DanhMucHopDongId_Select', 'Lựa Chọn Nghiệp Vụ', { enableSearch: true, enableClear: false });
    HandleRegisterFormFilter('#FilterForm', '#Pagination', '');

    $('#NamKetXuat, #DonViId, #HopDongIds').on('change', (event) => {
        const ele = event.currentTarget;
        DisplayValidationErrors.des('frmExportZip', ele.id);
    })
});

const HandleBtnShowModalNghiepVu = () => {
    const donViId = $('#DanhMucDonVi_Select').val();
    if (!donViId) {
        toastr.warning('Hiện tại bạn chưa thuộc quản lý của đơn vị/ phòng ban.');
        return;
    }

    $('#DanhMucId_Input').val(donViId);
    SetupSelect2('#modalSelectDanhMucHopDong_Body #DanhMucHopDongId_Select', 'Lựa chọn danh mục hợp đồng khởi tạo dữ liệu', { enableSearch: true, enableClear: true });
    $('#modalSelectDanhMucHopDong').modal('show');
}

const HandleFormSubmitInitDataForCreate = (e, form) => {
    e.preventDefault();
    const hopDongId = $('#DanhMucHopDongId_Select').val()
    if (!hopDongId) {
        toastr.warning('Hãy chọn một nghiệp vụ để khởi tạo dữ liệu.');
        return;
    }
    e.currentTarget.submit();
}

const HandleConfirmChuyen = (hoSoId) => {
    Swal.fire({
        icon: "question",
        title: "<span class='font-weight-boldest font-size-h1 text-capitalize text-blue mb-n5'>Bạn có chắc chắn muốn chuyển hồ sơ?</span>",
        text: "Hành động này không thể hoàn tác!",
        showCancelButton: true,
        confirmButtonText: "Xác Nhận",
        confirmButtonClass: "btn btn-primary font-weight-bold",
        cancelButtonText: "Hủy",
        cancelButtonClass: "btn btn-light-secondary font-weight-bold text-dark-75",
        reverseButtons: true
    }).then((result) => {
        if (result.isConfirmed) {
            const urlPost = $('#ChuyenForm').attr('action');
            $('form#ChuyenForm #chuyenId').val(hoSoId)
            HandlePostAjax({
                url: urlPost,
                data: new FormData($('#ChuyenForm')[0]),
                successCallback: (res) => {
                    if (res.isValid) {
                        Swal.fire({
                            icon: "success",
                            title: "<span class='font-weight-boldest font-size-h1 text-capitalize text-blue mb-n5'>Thành công!</span>",
                            text: res.message,
                            confirmButtonText: "Tiếp tục",
                            confirmButtonClass: 'btn btn-primary font-weight-bold',
                        }).then(() => {
                            window.location.reload();
                        });
                    }
                    else {
                        toastr.warning(res.message || 'Có lỗi khi khi thao tác với dữ liệu', 'Cảnh Báo!');
                    }
                },
                errorCallback: (xhr, status, error) => {
                    toastr.error('Không nhận được phản hồi từ máy chủ. Vui lòng thử lại sau.');
                },
            })
        }
    });
}

const HandleShowModelLyDoTraLai = (soHopDong, tenHopDong, lyDoTraLai) => {
    $('#LyDoTraLai_Modal #maHopDong').html(soHopDong);
    $('#LyDoTraLai_Modal #tenHopDong').html(tenHopDong);
    $('#LyDoTraLai_Modal #lyDoTraLai').html(lyDoTraLai);
    $('#LyDoTraLai_Modal').modal('show');
}