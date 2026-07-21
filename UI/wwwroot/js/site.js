const SetUpToast = ({
    position = "toast-top-right",
    timeOut = 5000,
    extendedTimeOut = 1000,
    showMethod = "fadeIn",
    hideMethod = "fadeOut",
    progressBar = false,
    closeButton = false
} = {}) => {
    toastr.options = {
        closeButton: closeButton,
        debug: false,
        progressBar: progressBar,
        preventDuplicates: false,
        positionClass: position,
        showDuration: 300,
        hideDuration: 1000,
        timeOut: timeOut,
        extendedTimeOut: extendedTimeOut,
        showEasing: "swing",
        hideEasing: "linear",
        showMethod: showMethod,
        hideMethod: hideMethod
    };
};

const SetupSelect2 = (element, placeholderText, options = { enableClear: false}) => {
    if ($(element).hasClass('select2-hidden-accessible')) {
        UpdateSelect2Value(element, placeholderText, '__reset__', options);
        return;
    }

    $(element).select2({
        placeholder: placeholderText,
        allowClear: options.enableClear,
        width: '100%',
        minimumResultsForSearch: (options.enableSearch !== false) ? 0 : Infinity, // ← bật/tắt search box
        templateResult: function (data) {
            if (!data.id) return data.text;

            const $el = $(data.element);
            const level = parseInt($el.data('level')) || 0;
            const indentPx = level * 20;
            const isGroup = $el.prop('disabled');

            const $span = $('<span>')
                .text(data.text)
                .css({
                    'padding-left': indentPx + 'px',
                    'font-weight': isGroup ? 'bold' : 'normal',
                    'opacity': 1
                });

            return $span;
        },
        templateSelection: function (data) {
            return data.text;
        }
    });

    $(element).on('select2:open', () => {
        setTimeout(() => {
            $('.select2-results__option[aria-disabled="true"]').css({
                'cursor': 'default',
                'pointer-events': 'none',
                'color': '#333',
                'font-weight': 'bold'
            });
        }, 10);
    });
}

const UpdateSelect2Value = (element, placeholderText, value, options = {}) => {
    $(element).select2('destroy'); // Phá hủy select2 trước
    if (value === '__reset__') {
        if ($(element).prop('multiple')) {
            $(element).val([]);
        } else {
            $(element).val(null);
        }
    } else {
        $(element).val(value); // Cập nhật giá trị mới nếu cần
    }
    SetupSelect2(element, placeholderText, options) // Khởi tạo lại select2
};

const SetupDatepicker = (element, placeholderText = 'dd/mm/yyyy', options = {}) => {
    const $el = $(element);

    if ($el.attr('type') !== 'date') $el.attr('type', 'date');

    // Inject CSS để ẩn calendar icon nếu chưa có
    if ($('#datepicker-no-calendar-style').length === 0) {
        $('<style>', {
            id: 'datepicker-no-calendar-style',
            html: `
            input[type="date"].no-calendar-popup::-webkit-calendar-picker-indicator {
                display: none;
                -webkit-appearance: none;
                pointer-events: none;
                opacity: 0;
            }
        `
        }).appendTo('head');
    }


    $el.addClass('no-calendar-popup');

    // Nếu đã khởi tạo rồi thì reset lại
    if ($el.data('datepicker')) {
        $el.datepicker('destroy');
    }

    // Thiết lập mặc định
    const defaultOptions = {
        format: options.format || 'yyyy-mm-dd',
        autoclose: true,
        todayHighlight: true,
        clearBtn: options.enableClear !== false,
        orientation: 'bottom',
        language: options.language || 'vi',
        templates: {
            leftArrow: '&laquo;',
            rightArrow: '&raquo;'
        }
    };

    $el
        .attr('placeholder', placeholderText)
        .datepicker($.extend({}, defaultOptions, {
            //language: 'vi'
        }, options));
};


const Preloader = {
    enable: (element) => $(element).addClass('d-flex'),
    disable: (element) => $(element).removeClass('d-flex')
};

const perfectScrollbar = {
    instances: new Map(),

    active: (element) => {
        if (!perfectScrollbar.instances.has(element)) {
            const ps = new PerfectScrollbar($(element)[0], {
                wheelPropagation: false,
                suppressScrollX: true
            });
            perfectScrollbar.instances.set(element, ps);
        }
    },

    unactive: (element) => {
        if (perfectScrollbar.instances.has(element)) {
            $(element).scrollTop(0);
            perfectScrollbar.instances.get(element).destroy();
            perfectScrollbar.instances.delete(element);
        }
    }
};

const DisplayValidationErrors = {
    des: (frmId, fieldName = null) => {
        if (fieldName) {
            $(`#${frmId} #${fieldName}Error`).empty();
        } else {
            $(`#${frmId} [name=Error]`).empty();
        }
    },
    show: (frmId, errors) => {
        $.each(errors, (key, value) => {
            $(`#${frmId} #${key}Error`).append($('<i>').addClass('la la-info-circle text-danger ml-1')).append(value);
        });
    }
};

const HandleRegisterFormFilter = (formFilterId, paginationId, urlIndex, defaultQuery = {}) => {
    let form = $(formFilterId);

    const buildQueryString = () => {
        const formQuery = HandleGetValueFormMutilSelect(form); // key1=value1&key2=value2
        const formParams = new URLSearchParams(formQuery);

        // Tạo URLSearchParams mới từ defaultQuery trước
        const mergedParams = new URLSearchParams();

        for (const key in defaultQuery) {
            mergedParams.set(key, defaultQuery[key]);
        }

        // Ghi đè các giá trị từ form (nếu trùng key)
        for (const [key, value] of formParams.entries()) {
            mergedParams.set(key, value);
        }

        return mergedParams.toString();
    };

    form.on('submit', (event) => {
        event.preventDefault();
        window.location.href = '?' + buildQueryString();
    });

    form.on('reset', (event) => {
        //window.location.href = urlIndex;
        const hasDefaultQuery = Object.keys(defaultQuery).length > 0;

        if (hasDefaultQuery) {
            const searchParams = new URLSearchParams();

            for (const key in defaultQuery) {
                searchParams.set(key, defaultQuery[key]);
            }

            const baseUrl = window.location.origin + window.location.pathname;
            window.location.href = `${baseUrl}?${searchParams.toString()}`;
        } else {
            // Nếu không có query mặc định thì quay về urlIndex gốc
            if (typeof urlIndex === 'string' && urlIndex.trim() !== '') {
                // Nếu urlIndex được truyền hợp lệ, dùng luôn
                window.location.href = urlIndex;
            } else {
                // Nếu không có urlIndex, tự động remove toàn bộ query string hiện tại
                const cleanUrl = window.location.origin + window.location.pathname;
                window.location.href = cleanUrl;
            }
        }
    });

    form.find('select').not('#PageSize_Select').on('change', () => {
        window.location.href = '?' + buildQueryString();
    });

    SetupSelect2($(formFilterId + ' #PageSize_Select'), 'Lựa chọn số bản ghi hiển thị', { enableSearch: false, enableClear: false });

    $(formFilterId + ' #PageSize_Select').on('change', (event) => {
        window.location.href = '?' + buildQueryString();
    });

    //Pagination
    $(`${paginationId} button`).on('click', (event) => {
        const onclickText = $(event.currentTarget).attr('onclick');
        if (onclickText) {
            const match = onclickText.match(/ClickPage\((.+)\)/);
            if (match && match[1]) {
                const expression = match[1];
                const pageValue = eval(expression); // sẽ ra: 2
                $(`${formFilterId} #PageCurrent`).val(pageValue);
            }
            window.location.href = '?' + buildQueryString();
        }
    });
}

const HandleInitAjaxLoad = (urlListLoad, beforHandle, successHandler, errorHandler, callbackHandle) => {
    $.address.init().change(function (event) {
        var urlTransform = urlListLoad;
        var urlHistory = event.value;
        if (urlHistory.length > 0) {
            urlHistory = urlHistory.substring(1, urlHistory.length);
            if (urlTransform.indexOf("?") > 0)
                urlTransform = urlTransform + "&" + urlHistory;
            else
                urlTransform = urlTransform + "?" + urlHistory;
        }

        $.ajax({
            url: urlTransform,
            type: 'GET',
            beforeSend: (res) => {
                if (typeof beforHandle === "function") {
                    beforHandle();
                }
            },
            success: (res) => {
                if (typeof successHandler === "function") {
                    successHandler(res);
                }
            },
            error: (jqXHR, textStatus, errorThrown) => {
                if (typeof errorHandler === "function") {
                    errorHandler(jqXHR, textStatus, errorThrown);
                } else {
                    console.error("Lỗi Ajax:", textStatus, errorThrown);
                }
            },
            complete: () => {
                if (typeof callbackHandle === "function") {
                    callbackHandle();
                }
            }
        });
    });
};

const HandleGetValueFormMutilSelect = (form) => {
    let arrParam = '';
    const paramMap = {};

    $(form).find("input,textarea,hidden,select").not("input[type='checkbox'], input[type='radio']:checked, input[name='selectItem'], .ms-search input, .mutil,.mutil1").each(function () {
        const inputName = $(this).attr("name");
        const value = $(this).val();

        if (value === null || value === 'null' || value === '' || value === 'GetAll' || value === 'Từ khóa tìm kiếm') return;

        if (inputName === 'Year') {
            const currentYear = new Date().getFullYear().toString();
            if (value === currentYear) return;
        }

        if ((inputName === 'PageSize' && value === '5') || (inputName === 'PageCurrent' && value === '1')) return;

        if ($(this).attr("type") === "date" && $(this).data("ignore-if-default")) {
            const today = new Date();
            const yyyy = today.getFullYear();
            const mm = String(today.getMonth() + 1).padStart(2, '0');

            if (inputName.endsWith('Tu')) {
                const defaultStart = `${yyyy}-${mm}-01`;
                if (value === defaultStart) return;
            }

            if (inputName.endsWith('Den')) {
                const lastDay = new Date(yyyy, today.getMonth() + 1, 0).getDate();
                const defaultEnd = `${yyyy}-${mm}-${String(lastDay).padStart(2, '0')}`;
                if (value === defaultEnd) return;
            }
        }

        paramMap[inputName] = value;
    });

    // Sắp xếp theo thứ tự ưu tiên search → pagesize → page_current → các tham số khác
    const priorityKeys = ['Search', 'PageSize', 'PageCurrent'];
    priorityKeys.forEach(key => {
        if (paramMap[key])
            arrParam += `&${key}=${paramMap[key]}`;
    });

    Object.keys(paramMap).forEach(key => {
        if (!priorityKeys.includes(key)) {
            arrParam += `&${key}=${paramMap[key]}`;
        }
    });

    return arrParam !== '' ? `${arrParam.substring(1)}` : '';
};

const HandleClearInputForm = (form) => {
    $(':input', form)
        .not(':button, :submit, :reset, :hidden')
        .val('')
        .prop('checked', false)
        .prop('selected', false);
}

const HandleSetInputValue = ($input, value) => {
    let type = $input.attr('type'); // Lấy type của input
    let tagName = $input.prop('tagName').toLowerCase(); // Lấy loại thẻ (input, select, textarea)

    switch (tagName) {
        case 'input':
            if (type === 'checkbox' || type === 'radio') {
                $input.prop('checked', value);
            } else {
                $input.val(value);
            }
            break;

        case 'select':
            $input.val(value).prop('selected', true); // Gán giá trị + kích hoạt event change (nếu có)
            break;

        case 'textarea':
            $input.val(value);
            break;

        default:
            console.warn(`Không hỗ trợ tagName: ${tagName}`);
            break;
    }
};

const HandlePostAjax = ({ url, data, header, xhrCallback, beforeSendCallback, successCallback, errorCallback, completeCallback }) => {
    $.ajax({
        url: url,
        type: 'POST',
        headers: header,
        contentType: false, // Để tránh jQuery xử lý dữ liệu FormData
        processData: false,
        data: data,
        xhr: xhrCallback ? () => xhrCallback() : undefined,
        beforeSend: (res) => {
            if (beforeSendCallback) beforeSendCallback(res);
        },
        success: (res, status, xhr) => {
            if (successCallback) successCallback(res, status, xhr);
        },
        error: (xhr, status, error) => {
            console.log(error);
            if (errorCallback) errorCallback(xhr, status, error);
        },
        complete: (res) => {
            if (completeCallback) completeCallback(res);
        }
    });
}

const HandleFetchAjax = ({ url, beforeSendCallback, successCallback, errorCallback, completeCallback }) => {
    $.ajax({
        url: url,
        type: 'GET',
        beforeSend: (res) => {
            if (beforeSendCallback) beforeSendCallback(res);
        },
        success: (res) => {
            if (successCallback) successCallback(res);
        },
        error: (xhr, status, error) => {
            console.log(error);
            if (errorCallback) errorCallback(xhr, status, error);
        },
        complete: (res) => {
            if (completeCallback) completeCallback(res);
        }
    });
};

const HandleFetchDonViTiepNhan = (donViChuQuanId, csrfToken, callbacks = {}) => {
    const formData = new FormData();
    formData.append('Id', donViChuQuanId);
    HandlePostAjax({
        url: '/Global/GetDonViTiepNhan',
        header: {
            'RequestVerificationToken': csrfToken
        },
        data: formData,
        ...callbacks
    })
}

const HandleShowModel = (modelName, url, title) => {
    $.ajax({
        url: url,
        type: 'GET',
        beforeSend: () => {
        },
        success: (response) => {
            $('#' + modelName + ' #' + modelName + '_Title').html(title);
            $('#' + modelName + ' #' + modelName + '_Body').html(response);
            $('#' + modelName).modal('show');
        },
        error: (response) => {
            toastr["warning"]('Không thể kết nối tới máy chủ.');
        }
    });
}

const HandleSubmitFormCreateOrEdit = (event, form, modelName) => {
    event.preventDefault();
    let urlPost = $(form).attr('action');
    HandlePostAjax({
        url: urlPost,
        data: new FormData(form),
        beforeSendCallback: (res) => {
            $('#' + modelName).modal('hide');
        },
        successCallback: (res) => {
            if (res.isValid) {
                Swal.fire({
                    icon: "success",
                    title: "<span class='font-weight-boldest font-size-h1 text-capitalize text-blue mb-n5'>Thành Công!</span>",
                    text: res.message,
                    confirmButtonText: "Tiếp tục",
                    confirmButtonClass: 'btn btn-primary font-weight-bold',
                }).then(() => {
                    //window.location.hash = HandleGetValueFormMutilSelect($('#FilterForm'));
                    window.location.reload();
                });
            }
            else {
                toastr.warning(res.message || 'Có lỗi khi khi thao tác với dữ liệu', 'Cảnh Báo!');
                if (res.html) {
                    $('#' + modelName + ' #' + modelName + '_Body').html(res.html);
                    $('#' + modelName).modal('show');
                }
            }
        },
        errorCallback: (xhr, status, error) => {
            toastr.error('Không nhận được phản hồi từ máy chủ. Vui lòng thử lại sau.');
        },
    })
}

const HandleConfirmDelteRow = (rowId) => {
    Swal.fire({
        icon: "warning",
        title: "<span class='font-weight-boldest font-size-h1 text-capitalize text-warning mb-n5'>Bạn có chắc chắn muốn xóa?</span>",
        text: "Hành động này không thể hoàn tác!",
        //imageUrl: "/assets/media/loading/trash.gif",
        //imageWidth: 120,
        //imageHeight: 120,
        showCancelButton: true,
        confirmButtonText: "Xác Nhận",
        confirmButtonClass: 'btn btn-danger font-weight-bold',
        cancelButtonText: "Hủy",
        cancelButtonClass: "btn btn-light-secondary font-weight-bold text-dark-75",
        reverseButtons: true
    }).then((result) => {
        if (result.isConfirmed) {
            let urlPost = $('#DeleteForm').attr('action');
            $('#recordId').val(rowId)
            HandlePostAjax({
                url: urlPost,
                data: new FormData($('#DeleteForm')[0]),
                beforeSendCallback: (res) => {
                },
                successCallback: (res) => {
                    if (res.isValid) {
                        Swal.fire({
                            icon: "success",
                            title: "<span class='font-weight-boldest font-size-h1 text-capitalize text-blue mb-n5'>Thành Công!</span>",
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

// Chuyển đổi số vnd => số đúng theo chuẩn
const HandleGetAssetFormData = (ids) => {
    const data = {};
    ids.forEach(id => {
        const $el = $('#' + id);
        if ($el.length) {
            let val = $el.val();

            // Nếu có class money-decimal-mask thì cần chuyển về số thật
            if ($el.hasClass('money-decimal-mask')) {
                // Xoá dấu phân cách nhóm, thay dấu , thành .
                val = val.replace(/\./g, '').replace(',', '.');
                val = parseFloat(val);
                // Nếu parse lỗi thì gán lại 0
                if (isNaN(val)) val = 0;
            }

            data[id] = val;
        }
    });
    return data;
}

const HandleSetFormattedValue = (idInput, val) => {
    const $el = $('#' + idInput);
    if (!$el.length) return;

    if ($el.hasClass('money-decimal-mask')) {
        val = parseFloat(val || 0);

        // Nếu là số tròn không có phần lẻ, thì chỉ hiển thị phần nguyên
        const isInteger = val === Math.floor(val);

        if (isInteger) {
            val = val.toLocaleString('vi-VN', {
                minimumFractionDigits: 0,
                maximumFractionDigits: 0
            });
        } else {
            val = val.toLocaleString('vi-VN', {
                minimumFractionDigits: 3,
                maximumFractionDigits: 3
            });
        }
    }

    $el.val(val);
};

const HandleRegisterInputDateMask = () => {
    const $input = $('.custom-date-mask');

    $input.inputmask({
        mask: "99/99/9999",
        placeholder: "__/__/____",
        showMaskOnHover: false,
        showMaskOnFocus: true,
        positionCaretOnClick: "lvp",

        onBeforeMask: function (value, opts) {
            const clean = value.replace(/\D/g, '');
            let result = '';
            if (clean.length === 8) {
                // ddmmyyyy
                result = clean.replace(/^(\d{2})(\d{2})(\d{4})$/, "$1/$2/$3");
                return result;
            } else if (clean.length === 6) {
                // mmyyyy
                const mm = clean.substring(0, 2);
                const yyyy = clean.substring(2);
                result = `00/` + mm + `/` + yyyy;
                return result;
            } else if (clean.length === 4) {
                // yyyy
                result = `00/00/` + clean;
                return result;
            }

            return value;
        }
    });

    //$input.off('paste').on('paste', (e) => {
    //    e.preventDefault();

    //    const clipboardData = (e.originalEvent || e).clipboardData.getData('text');

    //    var cleanValue = clipboardData.replace(/\D/g, '');
    //    let result = '';

    //    if (cleanValue.length === 8) {
    //        result = cleanValue.replace(/^(\d{2})(\d{2})(\d{4})$/, "$1/$2/$3");
    //    }
    //    if (cleanValue.length === 6) {
    //        let m = cleanValue.substring(0, 2);
    //        let y = cleanValue.substring(2, 6);
    //        result = "00/" + m + "/" + y;
    //    }
    //    if (cleanValue.length === 4) {
    //        result = "00/00/" + cleanValue;
    //    }

    //    $input.val(result).trigger('input');
    //});

    // Validate sau khi nhập xong hoặc dán
    $input.on('blur', function () {
        let value = $(this).val();
        if (value === "") return;

        let parts = value.split('/');
        let isValid = true;
        let mes = '{0} vừa nhập không hợp lệ. Hãy kiểm tra lại!';
        let d = parseInt(parts[0], 10);
        let m = parseInt(parts[1], 10);
        let y = parseInt(parts[2], 10);

        if (isNaN(y) || y < 1900 || y > 2100) {
            isValid = false;
            mes = 'Năm vừa nhập không hợp lệ. Hãy kiểm tra lại.';
        } else {
            // Nếu tháng = 0 → chấp nhận, nhưng cần năm hợp lệ (đã kiểm tra ở trên)
            if (m === 0) {
                // cho phép, không kiểm tra d
            } else if (m > 0 && m <= 12) {
                // Nếu ngày = 0 thì vẫn hợp lệ, miễn là tháng và năm hợp lệ
                if (d > 0 && (d < 1 || d > 31)) {
                    isValid = false;
                    mes = 'Ngày vừa nhập không hợp lệ. Hãy kiểm tra lại.';
                }
            } else {
                // Tháng không hợp lệ
                isValid = false;
                mes = 'Tháng vừa nhập không hợp lệ. Hãy kiểm tra lại.';
            }
        }

        if (!isValid) {
            toastr.warning(mes)
            //$(this).val('').focus();
        }
    });
}

const HandleRegisterDmDiaDanhSelect = ({
    tinhThanhPhoId,
    xaPhuongId,
    selectTinhId,
    selectXaId,
    autoSubmitSelector = null,
}) => {
    let $selectThanhPho = $(`#${selectTinhId}`);
    let $selectXaPhuong = $(`#${selectXaId}`);

    SetupSelect2(`#${selectTinhId}`, 'Lựa Chọn Tỉnh/ Thành Phố');
    SetupSelect2(`#${selectXaId}`, 'Lựa Chọn Xã/ Phường/ Thị Trấn');

    const hasValidOptions = $selectThanhPho.find('option').filter(function () {
        const val = $(this).val();
        return val && val !== 'null' && val !== '' && val !== '00000000-0000-0000-0000-000000000000';
    }).length > 0;

    if (!hasValidOptions) {
        HandleLoadPhuongXaOptions('00000000-0000-0000-0000-000000000000', '', selectTinhId, () => {
            let $optionFirst = $selectThanhPho.find('option').first();
            HandleLoadPhuongXaOptions($optionFirst.val(), '', selectXaId);
        });
    }

    $selectThanhPho.on('change', (event) => {
        const danhMucId = $selectThanhPho.val();
        if (!danhMucId || danhMucId === 'null') {
            if (danhMucId === 'null')
                toastr.warning('Lựa chọn lại Tỉnh/ Thành phố');
            //$selectXaPhuong.empty().append($('<option>', { value: 'null', text: '--Lựa Chọn Tỉnh/ Thành Phố--' }));
            $selectXaPhuong.empty();
            return;
        }
        $selectXaPhuong.empty();
        if (autoSubmitSelector)
            window.location.href = '?' + HandleGetValueFormMutilSelect($(autoSubmitSelector));
        else {
            HandleLoadPhuongXaOptions(danhMucId, '', selectXaId);
        }
    });

    $selectXaPhuong.on('change', (event) => {
        if (autoSubmitSelector)
            window.location.href = '?' + HandleGetValueFormMutilSelect($(autoSubmitSelector));
    });

    if (tinhThanhPhoId && tinhThanhPhoId !== '00000000-0000-0000-0000-000000000000') {
        HandleLoadPhuongXaOptions(tinhThanhPhoId, xaPhuongId, selectXaId);
    }
}

const HandleLoadPhuongXaOptions = (danhMucId, selectedPhuongXaId, selectId, onComplete = null) => {
    HandleFetchAjax({
        url: `/Global/GetDanhMucDiaDanh?danhMucId=${danhMucId}`,
        successCallback: (res) => {
            if (res) {
                if (res.isValid) {
                    if (res.data && res.data.length > 0) {
                        $(`select#${selectId}`).empty()
                        res.data.forEach((item, index) => {
                            $(`select#${selectId}`).append($('<option>', {
                                value: item.id,
                                text: item.tenDiaDanh,
                                selected: item.id == selectedPhuongXaId
                            }));
                        });
                    }
                    else {
                        $(`select#${selectId}`).empty().append($('<option>', {
                            value: 'null',
                            text: '-- Không có dữ liệu --',
                            disabled: true,
                            selected: true
                        }));
                    }

                    if (typeof onComplete === 'function') onComplete();
                }
                else {
                    toastr.warning(res.message);
                }
            }
        },
        errorCallback: (xhr, status, error) => {
            toastr.error('Không nhận được phản hồi từ máy chủ. Vui lòng thử lại sau.');
        },
    });
}

const HandleShowModelDMDiaDanh = (groupid) => {
    HandleRegisterDmDiaDanhSelect({
        selectTinhId: 'DmTinhThanh_Select',
        selectXaId: 'DmXaPhuong_Select',
    });
    groupId = groupid;

    const modal = $('#modalSelectDmDiaDanh');
    //modal.modal('show');
}

const HandleSubmitDmDiaDanh = () => {
    let $dmTinhThanh_Select = $('#DmTinhThanh_Select');
    let valDmTinhThanh = $dmTinhThanh_Select.val();
    let $dmXaPhuong_Select = $('#DmXaPhuong_Select');
    let valDmXaPhuong = $dmXaPhuong_Select.val();

    if (groupId === null) {
        toastr.warning('Lỗi hệ thống. Hãy thử lại hoặc liên hệ nhà phát hành.', 'Cảnh Báo !!!');
        return;
    }

    $inputFill = $(`input[data-target-input='${groupId}']`);
    $inputFillTen = $(`input[data-target-input='${groupId}_Ten']`);
    $labelFill = $(`label[data-target-label='${groupId}']`);

    if (valDmXaPhuong) { // lưu lại mã xã phường
        $inputFill.val(valDmXaPhuong);
        let dmDiaDanh = $dmXaPhuong_Select.find('option:selected').text();
        $inputFillTen.val(dmDiaDanh);
        $labelFill.html(dmDiaDanh);
    }
    else if (valDmTinhThanh) { // lưu lại mã tỉnh/ thành phố
        $inputFill.val(valDmTinhThanh);
        let dmDiaDanh = $dmTinhThanh_Select.find('option:selected').text();
        $inputFillTen.val(dmDiaDanh);
        $labelFill.html(dmDiaDanh);
    }
    else {
        toastr.warning("Vui lòng chọn địa danh phù hợp");
        $inputFill.val('');
        $inputFillTen.val('');
        $labelFill.html('--Chọn địa bàn--');
    }
    const modal = $('#modalSelectDmDiaDanh');
    modal.modal('hide');
}

const HandleShowHideFrom = (btn, form) => {
    const frm = $(form);
    const cssStyle = frm.css('display');

    if (cssStyle === 'none') {
        frm.css('display', 'block');
        $(btn).html('Ẩn');
    } else {
        frm.css('display', 'none');
        $(btn).html('Hiển thị');
    }
}