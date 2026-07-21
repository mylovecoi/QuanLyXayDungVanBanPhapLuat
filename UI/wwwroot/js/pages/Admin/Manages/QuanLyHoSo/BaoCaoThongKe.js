$(document).ready(function () {
    $('#DynamicReportForm').on('submit', (e) => {
        //e.preventDefault();

        const form = $('#DynamicReportForm')[0];

        $('#modalDynamicForm').modal('hide');

        //$('#modalDynamicForm').on('hidden.bs.modal', () => {
        //    form.submit();
        //});
    });
});

const HandleShowModelReport = (urlPost, urlGet, modalTitle) => {
    SetupSelect2($('#DynamicReportForm .modal-body select#DonViId'), 'Lựa chọn đơn vị', { enableSearch: true, enableClear: false });
    SetupSelect2($('#DynamicReportForm .modal-body select#DanhMucHopDongIds'), 'Lựa chọn hợp đồng', { enableSearch: true, enableClear: true });
    $('#DynamicReportForm').attr('action', urlPost);
    HandleFetchAjax({
        url: urlGet,
        successCallback: (res) => {
            if (res.isValid) {
                $('#DynamicReportForm .modal-body').empty().html(res.html);
                SetupSelect2($('#DynamicReportForm .modal-body select#DonViId'), 'Lựa chọn đơn vị', { enableSearch: true, enableClear: false });
                SetupSelect2($('#DynamicReportForm .modal-body select#DanhMucHopDongIds'), 'Lựa chọn hợp đồng', { enableSearch: true, enableClear: true });
                $('#modalDynamicForm').modal('show');
                $('#modalDynamicForm_Title').text(modalTitle);
            }
            else {
                toastr.warning(res.message || 'Có lỗi khi khi thao tác với dữ liệu', 'Cảnh Báo!');
            }
        },
        errorCallback: (xhr, status, error) => {
            toastr.error('Không nhận được phản hồi từ máy chủ. Vui lòng thử lại sau.');
        }
    })
}

const HandleShowModeSoBaoCao = (urlPost, urlGet, modalTitle) => {
    $('#DynamicReportForm').attr('action', urlPost);
    HandleFetchAjax({
        url: urlGet,
        successCallback: (res) => {
            if (res.isValid) {
                $('#DynamicReportForm .modal-body').empty().html(res.html);
                $('#modalDynamicForm').modal('show');
                $('#modalDynamicForm_Title').text(modalTitle);
            }
            else {
                toastr.warning(res.message || 'Có lỗi khi khi thao tác với dữ liệu', 'Cảnh Báo!');
            }
        },
        errorCallback: (xhr, status, error) => {
            toastr.error('Không nhận được phản hồi từ máy chủ. Vui lòng thử lại sau.');
        }
    })
}

const HandleFormSubmitInitDataForCreate = (event, form) => {
    event.preventDefault();
    let urlPost = $(form).attr('action');
    HandlePostAjax({
        url: urlPost,
        data: new FormData(form),
        successCallback: (res) => {
            if (res.isValid) {
                console.log(res);
            }
            else {
                toastr.warning(res.message || 'Có lỗi khi khi thao tác với dữ liệu', 'Cảnh Báo!');
                if (res.html) {
                    $('#DynamicReportForm .modal-body').html(res.html);
                    SetupSelect2($('select#DonViId'), 'Lựa chọn đơn vị', { enableSearch: true, enableClear: false });
                    SetupSelect2($('select#DanhMucHopDongIds'), 'Lựa chọn hợp đồng', { enableSearch: true, enableClear: true });
                }
                else {
                    toastr.warning(res.message || 'Có lỗi khi khi thao tác với dữ liệu', 'Cảnh Báo!');
                }
            }
        },
        errorCallback: (xhr, status, error) => {
            toastr.error('Không nhận được phản hồi từ máy chủ. Vui lòng thử lại sau.');
        }
    })
}

const HandleBtnShowModalExportData = () => {
    SetupSelect2('#modalExportData_Body #NamKetXuat', 'Lựa chọn năm kết xuất', { enableSearch: false, enableClear: false });
    SetupSelect2('#modalExportData_Body #DonViId', 'Lựa chọn đơn vị kết xuất', { enableSearch: true, enableClear: false });
    SetupSelect2('#modalExportData_Body #HopDongIds', 'Lựa chọn danh mục hợp đồng kết xuất', { enableSearch: true, enableClear: true });
    DisplayValidationErrors.des('frmExportZip');
    $('#modalExportData').modal('show');
}

const HandleSubmitFormExport = (event, form) => {
    event.preventDefault();
    HandlePostAjax({
        url: $(form).attr('action'),
        data: new FormData(form),
        beforeSendCallback: (res) => {
            $('#modalExportData').modal('hide');
        },
        xhrCallback: () => {
            var xhr = new window.XMLHttpRequest();
            xhr.responseType = 'blob'; // Đảm bảo nhận blob
            return xhr;
        },
        successCallback: (res, status, xhr) => {
            var contentType = xhr.getResponseHeader('Content-Type');
            if (contentType.includes('application/json')) {
                if (res.isValid) {
                    Swal.fire({
                        icon: "success",
                        title: "<span class='font-weight-boldest font-size-h1 text-capitalize text-blue mb-n5'>Thành công!</span>",
                        text: res.message,
                        confirmButtonText: "Tiếp tục",
                        confirmButtonClass: 'btn btn-primary font-weight-bold',
                    }).then(() => {
                        console.log(res);
                    });
                }
                else {
                    toastr.warning(res.message || 'Có lỗi khi khi thao tác với dữ liệu', 'Cảnh Báo!');
                    if (res.errors) {
                        DisplayValidationErrors.des('frmExportZip');
                        DisplayValidationErrors.show('frmExportZip', res.errors);
                    }
                    $('#modalExportData').modal('show');
                }
            }
            else if (contentType.includes('application/zip')) {
                Swal.fire({
                    icon: "success",
                    title: "<span class='font-weight-boldest font-size-h1 text-capitalize text-blue mb-n5'>Kết Xuất Dữ Liệu Thành công!</span>",
                    showCancelButton: true,
                    cancelButtonText: "Hủy",
                    cancelButtonClass: "btn btn-light-dark  font-weight-bold",
                    confirmButtonText: "Tải Xuống",
                    confirmButtonClass: 'btn btn-primary font-weight-bold',
                }).then((result) => {
                    if (result.isConfirmed) {
                        if (!(res instanceof Blob)) {
                            console.error('Dữ liệu phản hồi không phải là Blob:', res);
                            toastr.error("Dữ liệu tệp không hợp lệ.", "Lỗi!");
                            return;
                        }
                        var blob = res;
                        var fileName = xhr.getResponseHeader('Content-Disposition')?.match(/filename="(.+)"/)?.[1] || 'HoSoArchive.zip';
                        if (!fileName) {
                            console.warn('Không tìm thấy tên tệp, sử dụng mặc định:', fileName);
                        }
                        var url = window.URL.createObjectURL(blob);

                        // Tạo phần tử <a> bằng jQuery và thêm vào DOM
                        var $a = $('<a>')
                            .attr({
                                href: url,
                                download: fileName
                            })
                            .appendTo('body');

                        // Kích hoạt sự kiện click
                        $a[0].click();

                        // Loại bỏ phần tử và thu hồi URL sau khi tải
                        $a.remove();
                        window.URL.revokeObjectURL(url);
                    }
                    else {
                        $('#modalExportData').modal('show');
                    }
                });
            }
        },
        errorCallback: (xhr, status, error) => {
            toastr.error('Không nhận được phản hồi từ máy chủ. Vui lòng thử lại sau.');
        }
    })
}