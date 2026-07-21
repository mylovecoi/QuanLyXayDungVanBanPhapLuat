const GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var urlIndex = '';
$(document).ready(function () {
    SetupSelect2($('select#LoaiTaiSanId'), 'Lựa chọn loại tài sản', { enableSearch: true, enableClear: true });
    SetupSelect2($('select#PhuongThucCongChung'), 'Lựa chọn phương thức công chứng', { enableSearch: false, enableClear: false });
    $('#frmThongTinGiayToiKemTheo button[title="Xóa"]').hide(); // hide btn delete attached file
    MoneyDecimalMask();

    $('#modalSelectDmDiaDanh').on('hidden.bs.modal', () => {
        const $row = $('#modalSelectDmDiaDanh_Body .modal-body .row').first();
        HandleClearDiaDanhLevels($row, 1); // giữ lại cap1


    });
});

const HandleSubmitFormThongTin = (event, form) => {
    event.preventDefault();
    let urlPost = $(form).attr('action');
    HandlePostAjax({
        url: urlPost,
        data: new FormData(form),
        successCallback: (res) => {
            if (res.isValid) {
                Swal.fire({
                    icon: "success",
                    title: "<span class='font-weight-boldest font-size-h1 text-capitalize text-blue mb-n5'>Thành Công!</span>",
                    text: res.message,
                    confirmButtonText: "Tiếp tục",
                    confirmButtonClass: 'btn btn-primary font-weight-bold',
                }).then(() => {
                    window.location.assign(urlIndex);
                });
            }
            else {
                toastr.warning(res.message || 'Có lỗi khi khi thao tác với dữ liệu', 'Cảnh Báo!');
                if (res.html) {
                    $('#FormFields .card-body').html(res.html);
                    MoneyDecimalMask();
                }
            }
        },
        errorCallback: (xhr, status, error) => {
            toastr.error('Không nhận được phản hồi từ máy chủ. Vui lòng thử lại sau.');
        }
    });
}

const HandleRegisterSelectDanhMucDiaDanh = (selectRootId) => {
    let selectDiaDanhRoot = $(`#${selectRootId}`);

    SetupSelect2(`#${selectRootId}`, `Lựa Chọn Danh Mục Địa Danh Cấp ${1}`, { enableSearch: true, enableClear: false });

    const hasValidOptions = selectDiaDanhRoot.find('option').filter(function () {
        const val = $(this).val();
        return val && val !== 'null' && val !== '' && val !== GUID_EMPTY;
    }).length > 0;

    if (!hasValidOptions) {
        FetchDanhMucDiaDanh(GUID_EMPTY)
            .then(list => {
                if (!list.length) return;

                HandelFillDanhMucOption(selectRootId, list);
                selectDiaDanhRoot.trigger('change');
            })
            .catch(err => toastr.error(err.message || 'Lỗi tải dữ liệu'));
    }
    else {
        const $row = $('#modalSelectDmDiaDanh_Body .modal-body .row').first();
        HandleClearDiaDanhLevels($row, 1);
        const $options = selectDiaDanhRoot.find('option');
        const firstVal = $options.first().val();
        selectDiaDanhRoot.val(firstVal).trigger('change');
        //selectDiaDanhRoot.trigger('change');
    }

    selectDiaDanhRoot.on('change', HandleDiaDanhSelectChange);
}

const HandleDiaDanhSelectChange = (e) => {
    const $sel = $(e.target);
    const danhMucId = $sel.val();


    // Lấy level hiện tại từ id, ví dụ "DmDiaDanhCap2_Select" -> 2
    const level = parseInt($sel.prop('id').match(/\d+/)?.[0] || '1', 10);
    const nextLevel = level + 1;

    const $row = $('#modalSelectDmDiaDanh_Body .modal-body .row').first();

    const nextDivId = `divLevel${nextLevel}`;
    const nextSelectId = `DmDiaDanhCap${nextLevel}_Select`;

    if (!danhMucId || danhMucId === 'null') {
        if (danhMucId === 'null') toastr.warning('Lựa chọn lại Tỉnh/ Thành phố');
        HandleClearDiaDanhLevels($row, level);
        return;
    }
    // Nạp dữ liệu cho cấp kế
    FetchDanhMucDiaDanh(danhMucId)
        .then(list => {
            if (!list || list.length === 0 || danhMucId == GUID_EMPTY) {
                HandleClearDiaDanhLevels($row, level);
                return;
            }

            // Tạo khung cấp kế nếu chưa có
            let $nextSelect = $row.find(`#${nextSelectId}`);
            if ($nextSelect.length === 0) {
                const $wrap = $('<div>', { id: nextDivId, class: 'col-12 form-group' });
                const $label1 = $('<label>', { class: 'form-label text-blue fs-6 font-weight-boldest', text: `Địa Danh Cấp ${nextLevel}` });
                const $label2 = $('<label>', { class: 'form-label text-blue font-weight-boldest', text: ':' });
                $nextSelect = $('<select>', { id: nextSelectId, class: 'form-control' });
                $nextSelect.on('change.diadanh', HandleDiaDanhSelectChange);
                $wrap.append($label1, $label2, $nextSelect);
                $row.append($wrap);

                SetupSelect2(`#${nextSelectId}`, `Lựa Chọn Danh Mục Địa Danh Cấp ${nextLevel}`, { enableSearch: true, enableClear: true });
            }

            HandelFillDanhMucOption($nextSelect.attr('id'), list);
            $nextSelect.trigger('change');
        })
        .catch(err => {
            toastr.error(err.message || 'Lỗi tải dữ liệu');
        });
}

const FetchDanhMucDiaDanh = (danhMucId) => {
    return new Promise((resolve, reject) => {
        HandleFetchAjax({
            url: `/Global/GetDanhMucDiaDanh?danhMucId=${encodeURIComponent(danhMucId)}`,
            successCallback: (res) => {
                if (!res) return reject(new Error('No response'));
                if (res.isValid) return resolve(res.data || []);
                reject(new Error(res.message || 'Yêu cầu không hợp lệ'));
            },
            errorCallback: () => reject(new Error('Không nhận được phản hồi từ máy chủ. Vui lòng thử lại sau.')),
        });
    });
};

const HandelFillDanhMucOption = (selectId, items, selectedId = null) => {
    const $sel = $(`select#${selectId}`);
    $sel.empty();

    if (!items || items.length === 0) {
        return false;
    }

    items.forEach(it => {
        $sel.append($('<option>', {
            value: it.id,
            text: it.tenDiaDanh,
            selected: it.id === selectedId
        }));
    });
    return true;
};

const HandleClearDiaDanhLevels = ($row, level) => {
    $row.find('[id^="divLevel"]').each((_, el) => {
        const n = parseInt(el.id.match(/\d+/)[0], 10);
        if (n > level) {
            const $sel = $(el).find('select');
            if ($sel.data('select2')) $sel.select2('destroy');
            $(el).remove();
        }
    });
}

const HandleBtnShowModelDiaDanhClick = (groupid) => {
    HandleRegisterSelectDanhMucDiaDanh('DmDiaDanhCap1_Select');
    groupId = groupid;

    const modal = $('#modalSelectDmDiaDanh');
    modal.modal('show');
}

const HandleBtnChooseDiaDanh = () => {

    const $deepest = HandleGetDiaDanh();
    if (!$deepest) {
        toastr.warning('Vui lòng chọn địa danh phù hợp');
        return;
    }

    if ($deepest) {
        const val = $deepest.val();
        const text = $deepest.find('option:selected').text();

        $(`input[data-target-input='${groupId}']`).val(val);
        $(`input[data-target-input='${groupId}_Ten']`).val(text);
        $(`label[data-target-label='${groupId}']`).html(text);

        $('#modalSelectDmDiaDanh').modal('hide');
    } else {
        toastr.warning('Vui lòng chọn địa danh phù hợp');
    }
}

//const HandleGetDiaDanh = () => {
//    if (!groupId) return null;
//    const $row = $('#modalSelectDmDiaDanh_Body .modal-body .row').first();
//    const selects = $row.find('select[id^="DmDiaDanhCap"]').toArray();
//    let $deepest = null;
//    selects.forEach(el => {
//        const $el = $(el);
//        const val = $el.val();
//        if (val && val !== GUID_EMPTY) $deepest = $el;
//    });
//    return $deepest;
//}

const HandleGetDiaDanh = () => {
    if (!groupId) return null;
    return $('#modalSelectDmDiaDanh_Body .modal-body .row')
        .find('select[id^="DmDiaDanhCap"]')
        .toArray()
        .reduce((deepest, el) => {
            const $el = $(el);
            const val = $el.val();
            return (val && val !== GUID_EMPTY) ? $el : deepest;
        }, null);
};
