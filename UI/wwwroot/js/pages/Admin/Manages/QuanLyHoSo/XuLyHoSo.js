var signature = false;
var mode = false;

$(document).ready(function () {
    SetupSelect2('#TargetYear_Select', 'Lựa Chọn Nghiệp Vụ', { enableSearch: false, enableClear: false });
    SetupSelect2('#DanhMucDonVi_Select', 'Lựa Chọn Đơn Vị Làm Việc', { enableSearch: true, enableClear: true });
    SetupSelect2('#Status_Select', 'Lựa Chọn Trạng Thái Hồ Sơ', { enableSearch: false, enableClear: false });
    HandleRegisterFormFilter('#FilterForm', '#Pagination', '@Url.Action("Index", "XuLyHoSo")');

    // Xử lý thông báo từ thanh toán MoMo
    const urlParams = new URLSearchParams(window.location.search);
    const resultCode = urlParams.get('resultCode');
    const message = urlParams.get('message');

    // Loại bỏ 2 tham số ResultCode và Message khỏi URL
    if (resultCode && message) {
        const url = new URL(window.location.href);
        url.searchParams.delete('resultCode');
        url.searchParams.delete('message');
        window.history.replaceState({}, document.title, url.toString());

        if (resultCode === '0') {
            toastr.success(decodeURIComponent(message), 'Thanh toán thành công!');
        } else {
            toastr.error(decodeURIComponent(message), 'Thanh toán thất bại!');
        }
    }

    $('#SmartCA_Modal').on('show.bs.modal', () => {
        $('#HoanThanh_Modal').modal('hide');
    });

    $('#SmartCA_Modal').on('hidden.bs.modal', () => {
        $('#HoanThanh_Modal').modal('show');
    });
});

const HandleConfirmTraLai = (hoSoId, soHopDong, tenHopDong) => {
    $('#TraLaiHoSo_Modal #hoSoId').val(hoSoId);
    $('#TraLaiHoSo_Modal #maHopDong').html(soHopDong);
    $('#TraLaiHoSo_Modal #tenHopDong').html(tenHopDong);
    $('#TraLaiHoSo_Modal #lyDoTraLai').val();
    $('#TraLaiHoSo_Modal').modal('show');
}

const HandleConfirmTiepNhan = (hoSoId, soHopDong, tenHopDong) => {
    $('#TiepNhanHoSo_Modal #hoSoId').val(hoSoId);
    $('#TiepNhanHoSo_Modal #maHopDong').html(soHopDong);
    $('#TiepNhanHoSo_Modal #tenHopDong').html(tenHopDong);
    SetupSelect2('#TiepNhanHoSo_Modal #CongChungVienId', '--Lựa Chọn Cán Bộ Công Chứng--', { enableSearch: true, enableClear: false });
    $('#TiepNhanHoSo_Modal').modal('show');
}

const HandleConfirmHoanThanh = (hoSoId, soHopDong, tenHopDong) => {
    $('#HoanThanh_Modal #hoSoId').val(hoSoId);
    $('#HoanThanh_Modal #maHopDong').html(soHopDong);
    $('#HoanThanh_Modal #tenHopDong').html(tenHopDong);
    $('#HoanThanh_Modal #soQdPheDuyet').val();
    const today = new Date().toISOString().slice(0, 10);
    $('#HoanThanh_Modal #ngayQdPheDuyet').val(today);

    SetupDatepicker('#HoanThanh_Modal #ngayQdPheDuyet', '', {
        format: 'yyyy-mm-dd',
        enableClear: false,
    });
    $('#HoanThanh_Modal').modal('show');
}

const HandleShowModelLyDoTraLai = (soHopDong, tenHopDong, lyDoTraLai) => {
    $('#LyDoTraLai_Modal #maHopDong').html(soHopDong);
    $('#LyDoTraLai_Modal #tenHopDong').html(tenHopDong);
    $('#LyDoTraLai_Modal #lyDoTraLai').html(lyDoTraLai);
    $('#LyDoTraLai_Modal').modal('show');
}

const HandleConfirmXacNhanThanhToan = (hoSoId, soHopDong, tenHopDong) => {
    $('#XacNhanThanhToan_Modal #hoSoId').val(hoSoId);
    $('#XacNhanThanhToan_Modal #maHopDong').html(soHopDong);
    $('#XacNhanThanhToan_Modal #tenHopDong').html(tenHopDong);
    const today = new Date().toISOString().slice(0, 10);
    $('#XacNhanThanhToan_Modal #ngayQdPheDuyet').val(today);

    let controllerPath = document.body.dataset.controllerPath;

    let formData = new FormData();
    formData.append('hoSoId', hoSoId);

    HandlePostAjax({
        url: `${controllerPath}/GetChiPhiHoSo`,
        data: formData,
        successCallback: (res) => {
            if (res.isValid) {
                $('#XacNhanThanhToan_Modal #soTienPhaiThanhToan').html(res.status);
                $('#XacNhanThanhToan_Modal').modal('show');
            }
            else {
                toastr.warning(res.message || 'Có lỗi khi khi thao tác với dữ liệu', 'Cảnh Báo!');
            }
        },
        errorCallback: (xhr, status, error) => {
            toastr.error('Không nhận được phản hồi từ máy chủ. Vui lòng thử lại sau.');
        },
    });
}

const HandleShowModelThongTinThanhToan = (hoSoId, soHopDong, tenHopDong) => {
    // call ajax => server get qr code
    $('#ThongTinThanhToan_Modal #hoSoId').val(hoSoId);
    $('#ThongTinThanhToan_Modal #maHopDong').html(soHopDong);
    $('#ThongTinThanhToan_Modal #tenHopDong').html(tenHopDong);

    let controllerPath = document.body.dataset.controllerPath;

    let formData = new FormData();
    formData.append('hoSoId', hoSoId);

    HandlePostAjax({
        url: `${controllerPath}/GetChiPhiHoSo`,
        data: formData,
        successCallback: (res) => {
            if (res.isValid) {
                $('#ThongTinThanhToan_Modal #soTienPhaiThanhToan').html(res.status);
            }
            else {
                toastr.warning(res.message || 'Có lỗi khi khi thao tác với dữ liệu', 'Cảnh Báo!');
                console.log(res);
            }
        },
        errorCallback: (xhr, status, error) => {
            toastr.error('Không nhận được phản hồi từ máy chủ. Vui lòng thử lại sau.');
        },
    })

    $('#ThongTinThanhToan_Modal').modal('show');
}

const HandleConfirmChangeStatus = (hoSoId, formId, title) => {
    Swal.fire({
        icon: "warning",
        // title: "<span class='font-weight-boldest font-size-h1 text-capitalize text-blue mb-n5'>Bạn có chắc chắn muốn " + title + " hồ sơ?</span>",
        title: `<span class='font-weight-boldest font-size-h1 text-capitalize text-blue mb-n5'>${title}?</span>`,
        text: "Hành động này không thể hoàn tác!",
        showCancelButton: true,
        confirmButtonText: "Xác Nhận",
        confirmButtonClass: "btn btn-primary font-weight-bold",
        cancelButtonText: "Hủy",
        cancelButtonClass: "btn btn-light-secondary font-weight-bold text-dark-75",
        reverseButtons: true
    }).then((result) => {
        if (result.isConfirmed) {
            const urlPost = $(`#${formId}`).attr('action');
            $(`form#${formId} #hoSoId`).val(hoSoId)
            HandlePostAjax({
                url: urlPost,
                data: new FormData($(`#${formId}`)[0]),
                successCallback: (res) => {
                    if (res.isValid) {
                        Swal.fire({
                            icon: "success",
                            title: "<span class='font-weight-boldest font-size-h1 text-capitalize text-blue mb-n5'>Thành công!</span>",
                            text: res.message,
                            confirmButtonText: "Tiếp tục",
                            confirmButtonClass: 'btn btn-primary font-weight-bold',
                        }).then(() => {
                            if (res.status) {
                                $('#Status_Select').val(res.status).trigger('change');
                            }
                            else {
                                window.location.hash = HandleGetValueFormMutilSelect($('#FilterForm'));
                                window.location.reload();
                            }
                        });
                    }
                    else {
                        toastr.warning(res.message || 'Có lỗi khi khi thao tác với dữ liệu', 'Cảnh Báo!');
                        console.log(res);
                    }
                },
                errorCallback: (xhr, status, error) => {
                    toastr.error('Không nhận được phản hồi từ máy chủ. Vui lòng thử lại sau.');
                },
            })
        }
    });
}

const HandleSubmitChangeStatus = (event, form) => {
    event.preventDefault();
    let urlPost = $(form).attr('action');
    HandlePostAjax({
        url: urlPost,
        data: new FormData(form),
        beforeSendCallback: (res) => {

        },
        successCallback: (res) => {
            if (res.isValid) {
                $('#TraLaiHoSo_Modal').modal('hide');
                $('#TiepNhanHoSo_Modal').modal('hide');
                $('#HoanThanh_Modal').modal('hide');
                $('#ThongTinThanhToan_Modal').modal('hide');
                $('#XacNhanThanhToan_Modal').modal('hide');
                Swal.fire({
                    icon: "success",
                    title: "<span class='font-weight-boldest font-size-h1 text-capitalize text-blue mb-n5'>Thành công!</span>",
                    text: res.message,
                    confirmButtonText: "OK"
                }).then(() => {
                    if (res.status) {
                        $('#Status_Select').val(res.status).trigger('change');
                    }
                    else {
                        window.location.hash = HandleGetValueFormMutilSelect($('#FilterForm'));
                        window.location.reload();
                    }
                });
            }
            else {
                toastr.warning(res.message || 'Có lỗi khi khi thao tác với dữ liệu', 'Cảnh Báo!');
            }
        },
        errorCallback: (xhr, status, error) => {
            toastr.error('Không nhận được phản hồi từ máy chủ. Vui lòng thử lại sau.');
        },
    });
}

const HandleConfirmHuyDuyetHoSo = (hoSoId) => {
    Swal.fire({
        icon: "warning",
        title: "<span class='font-weight-boldest font-size-h1 text-capitalize text-danger mb-n5'>Bạn có chắc chắn muốn hủy hồ sơ?</span>",
        text: "Hành động này không thể hoàn tác!",
        showCancelButton: true,
        confirmButtonText: "Xác Nhận",
        confirmButtonClass: "btn btn-danger font-weight-bold",
        cancelButtonText: "Hủy",
        cancelButtonClass: "btn btn-light-secondary font-weight-bold text-dark-75",
        reverseButtons: true
    }).then((result) => {
        if (result.isConfirmed) {
            let urlPost = $('#HuyDuyetHoSoForm').attr('action');
            $('form#HuyDuyetHoSoForm #hoSoId').val(hoSoId)
            HandlePostAjax({
                url: urlPost,
                data: new FormData($('#HuyDuyetHoSoForm')[0]),
                successCallback: (res) => {
                    if (res.isValid) {
                        Swal.fire({
                            icon: "success",
                            title: "<span class='font-weight-boldest font-size-h1 text-capitalize text-blue mb-n5'>Thành Công!</span>",
                            text: res.message,
                            confirmButtonText: "Tiếp tục",
                            confirmButtonClass: 'btn btn-primary font-weight-bold',
                        }).then(() => {
                            if (res.status) {
                                $('#Status_Select').val(res.status).trigger('change');
                            }
                            else {
                                window.location.hash = HandleGetValueFormMutilSelect($('#FilterForm'));
                                window.location.reload();
                            }
                        });
                    }
                    else {
                        $('form#HuyDuyetHoSoForm #hoSoId').val('')
                        toastr.warning(res.message || 'Có lỗi khi khi thao tác với dữ liệu', 'Cảnh Báo!');
                    }
                },
                errorCallback: (xhr, status, error) => {
                    $('form#HuyDuyetHoSoForm #hoSoId').val('')
                    toastr.error('Không nhận được phản hồi từ máy chủ. Vui lòng thử lại sau.');
                }

            })
        }
    });
}


const HandleBtnSmartCAClick = () => {
    $("#SmartCA_Modal").modal("show");
    $('#btnLoginSmartCA').on('click', HandleBtnLoginSmartCAClick);
    $('#btnCancelSmartCA').on('click', HandelBtnCancelSmartCAClick);
}


const HandleBtnLoginSmartCAClick = async () => {
    const username = document.getElementById('userSmartCA').value;
    const password = document.getElementById('passSmartCA').value;

    const loginRes = await fetch('/SmartCA/LoginAjax', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ username, password })
    });

    if (!loginRes.ok) {
        alert("Đăng nhập thất bại!");
        return;
    }

    const data = await loginRes.json();
    if (!data.success) {
        alert("Sai thông tin hoặc không lấy được credential!");
        return;
    }

    // Lưu accessToken và credentialId tạm (có thể dùng sessionStorage nếu cần)
    window.smartcaAccessToken = data.accessToken;
    window.smartcaCredentialId = data.credentialId;

    // Thay nội dung modal sau khi đăng nhập
    document.getElementById('smartca-modal-body').innerHTML = `
            <div class="form-group">
                <label>Chọn file cần ký (PDF)</label>
                <input type="file" class="form-control" id="fileSmartCA" accept=".pdf" />
            </div>
            <div class="text-center">
                 <button id="btnSignSmartCA" class="btn btn-success font-weight-bold">Xác nhận ký</button>
            </div>
        `;

    // Gắn lại sự kiện click sau khi thay DOM
    setTimeout(() => {
        $('#btnSignSmartCA').on('click', HandleBtnSignSmartCAClick);
    }, 100);
}

const HandelBtnCancelSmartCAClick = () => {
    $("#SmartCA_Modal").modal("hide");
}

const HandleBtnSignSmartCAClick = async () => {
    signature = true;
    const fileInput = document.getElementById('fileSmartCA');
    const file = fileInput.files[0];
    if (!file) {
        alert("Vui lòng chọn file cần ký.");
        return;
    }

    const formData = new FormData();
    formData.append('file', file);
    formData.append('accessToken', window.smartcaAccessToken);
    formData.append('credentialId', window.smartcaCredentialId);

    const res = await fetch('/SmartCA/SignFileAjax', {
        method: 'POST',
        body: formData
    });

    const result = await res.json();
    if (result.success) {
        signature = false;
        mode = true;
        alert("🎉 Ký thành công!");
        $("#pdfPreview").attr("src", res.fileUrl).show();
        $("#signedFileBase64").val(res.base64);
        $("#SmartCA_Modal").modal("hide")
        // Gợi ý thêm:
        // window.location.href = result.downloadUrl;
    } else if (result.requireOtp) {
        // Nếu server yêu cầu OTP
        signature = false;

        document.getElementById('smartca-modal-body').innerHTML = `
                <div class="form-group">
                    <label>Nhập mã OTP</label>
                    <input type="text" class="form-control" id="otpSmartCA_Input" placeholder="Nhập mã OTP từ app hoặc SMS">
                </div>
                <div class="text-center">
                    <button class="btn btn-primary" onclick="HandleSubmitOtpBtnClick('${result.tranId}')">Xác nhận OTP</button>
                </div>
            `;
    }
    else {
        alert("❌ Lỗi ký: " + result.message);
        signature = false;
    }
}

const HandleSubmitOtpBtnClick = async (tranId) => {
    const otp = document.getElementById("otpSmartCA_Input").value.trim();
    if (!otp) {
        alert("Vui lòng nhập mã OTP.");
        return;
    }

    const res = await fetch('/SmartCA/PostOtpAndConfirm', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            tranId: tranId,
            otp: otp,
            accessToken: window.smartcaAccessToken
        })
    });

    const result = await res.json();
    if (result.success) {
        alert("✅ OTP hợp lệ, đang xác nhận ký...");
        // Gọi lại check ký
        await HandleBtnSignSmartCAClick(); // Gửi lại file để kiểm tra ký đã hoàn tất chưa
    } else {
        alert("❌ OTP sai hoặc quá hạn: " + result.message);
    }
}