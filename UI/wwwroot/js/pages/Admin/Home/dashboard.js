$(document).ready(function () {
    const data = window.homeDashboardData || {};
    renderMonthlyHoSoChart(data.hoSoChart || data.HoSoChart || {});
    renderXepLoaiChart(data.hoSoChart || data.HoSoChart || {});
    renderByStepChart(data.hoSoByStep || data.HoSoByStep || []);
    renderByDonViChart(data.hoSoByDonVi || data.HoSoByDonVi || []);
    renderThiHanhStatusChart(data.thiHanhChart || data.ThiHanhChart || {});
    renderThiHanhWarningChart(data.thiHanhChart || data.ThiHanhChart || {});
});

function createOrUpdateChart(targetId, options) {
    const el = document.querySelector(targetId);
    if (!el || typeof ApexCharts === 'undefined') {
        return;
    }

    if (el._chart) {
        el._chart.updateOptions(options, true, true);
        return;
    }

    const chart = new ApexCharts(el, options);
    chart.render();
    el._chart = chart;
}

function navigateTo(url) {
    if (url) {
        window.location.href = url;
    }
}

function buildMonthRange(category) {
    if (!category) {
        return null;
    }

    const match = category.match(/(\d{1,2})\/(\d{4})/);
    if (!match) {
        return null;
    }

    const month = parseInt(match[1], 10);
    const year = parseInt(match[2], 10);
    if (!month || !year) {
        return null;
    }

    const from = new Date(year, month - 1, 1);
    const to = new Date(year, month, 0);
    return {
        from: formatDate(from),
        to: formatDate(to)
    };
}

function formatDate(date) {
    const dd = `${date.getDate()}`.padStart(2, '0');
    const mm = `${date.getMonth() + 1}`.padStart(2, '0');
    const yyyy = date.getFullYear();
    return `${yyyy}-${mm}-${dd}`;
}

function renderMonthlyHoSoChart(data) {
    const categories = data.categories || data.Categories || [];
    createOrUpdateChart('#dashboard_ho_so_monthly', {
        chart: {
            type: 'bar',
            height: 340,
            toolbar: { show: false },
            events: {
                dataPointSelection: function (event, chartContext, config) {
                    const category = categories[config.dataPointIndex];
                    const range = buildMonthRange(category);
                    if (!range) {
                        return;
                    }

                    navigateTo(`/Manages/TraCuuDangKyVanBan?TuNgayTao=${range.from}&DenNgayTao=${range.to}`);
                }
            }
        },
        series: [
            { name: 'Tạo mới', data: data.hoSoTaoMoiTheoThang || data.HoSoTaoMoiTheoThang || [] },
            { name: 'Hoàn thành', data: data.hoSoHoanThanhTheoThang || data.HoSoHoanThanhTheoThang || [] },
            { name: 'Ban hành', data: data.hoSoBanHanhTheoThang || data.HoSoBanHanhTheoThang || [] }
        ],
        xaxis: { categories: categories },
        colors: ['#3699FF', '#1BC5BD', '#8950FC'],
        dataLabels: { enabled: false },
        plotOptions: { bar: { columnWidth: '45%', endingShape: 'rounded' } },
        legend: { position: 'top' }
    });
}

function renderXepLoaiChart(data) {
    const labels = data.xepLoaiLabels || data.XepLoaiLabels || [];
    const values = data.xepLoaiValues || data.XepLoaiValues || [];

    createOrUpdateChart('#dashboard_xep_loai', {
        chart: { type: 'donut', height: 340 },
        series: values,
        labels: labels.length ? labels : ['Chưa có dữ liệu'],
        colors: ['#1BC5BD', '#3699FF', '#FFA800', '#F64E60', '#8950FC', '#6c757d'],
        legend: { position: 'bottom' },
        noData: { text: 'Chưa có dữ liệu' }
    });
}

function renderByStepChart(items) {
    createOrUpdateChart('#dashboard_by_step', {
        chart: {
            type: 'bar',
            height: 320,
            toolbar: { show: false },
            events: {
                dataPointSelection: function (event, chartContext, config) {
                    const item = items[config.dataPointIndex];
                    if (!item) {
                        return;
                    }

                    const maBuoc = item.maBuoc || item.MaBuoc;
                    navigateTo(maBuoc
                        ? `/Manages/TraCuuDangKyVanBan?MaBuoc=${encodeURIComponent(maBuoc)}`
                        : '/Manages/TraCuuDangKyVanBan');
                }
            }
        },
        series: [{ name: 'Số lượng', data: items.map(x => x.soLuong ?? x.SoLuong ?? 0) }],
        xaxis: { categories: items.map(x => x.tenBuoc ?? x.TenBuoc ?? '') },
        colors: ['#3699FF'],
        plotOptions: { bar: { horizontal: true, borderRadius: 4 } },
        dataLabels: { enabled: false }
    });
}

function renderByDonViChart(items) {
    createOrUpdateChart('#dashboard_by_donvi', {
        chart: {
            type: 'bar',
            height: 320,
            toolbar: { show: false },
            events: {
                dataPointSelection: function (event, chartContext, config) {
                    const item = items[config.dataPointIndex];
                    if (!item) {
                        return;
                    }

                    const donViId = item.donViId || item.DonViId;
                    navigateTo(donViId
                        ? `/Manages/TraCuuDangKyVanBan?DonViId=${encodeURIComponent(donViId)}`
                        : '/Manages/TraCuuDangKyVanBan');
                }
            }
        },
        series: [{ name: 'Số hồ sơ', data: items.map(x => x.soLuongHoSo ?? x.SoLuongHoSo ?? 0) }],
        xaxis: { categories: items.map(x => x.tenDonVi ?? x.TenDonVi ?? '') },
        colors: ['#8950FC'],
        plotOptions: { bar: { horizontal: true, borderRadius: 4 } },
        dataLabels: { enabled: false }
    });
}

function renderThiHanhStatusChart(data) {
    createOrUpdateChart('#dashboard_thihanh_status', {
        chart: { type: 'pie', height: 320 },
        series: data.trangThaiValues || data.TrangThaiValues || [],
        labels: data.trangThaiLabels || data.TrangThaiLabels || [],
        colors: ['#3699FF', '#1BC5BD', '#FFA800', '#F64E60', '#8950FC'],
        legend: { position: 'bottom' },
        noData: { text: 'Chưa có dữ liệu' }
    });
}

function renderThiHanhWarningChart(data) {
    const labels = data.canhBaoLabels || data.CanhBaoLabels || [];
    const items = (window.homeDashboardData && (window.homeDashboardData.thiHanhCanhBao || window.homeDashboardData.ThiHanhCanhBao)) || [];

    createOrUpdateChart('#dashboard_thihanh_warning', {
        chart: {
            type: 'bar',
            height: 320,
            toolbar: { show: false },
            events: {
                dataPointSelection: function (event, chartContext, config) {
                    const item = items[config.dataPointIndex];
                    const maCanhBao = item ? (item.maCanhBao || item.MaCanhBao) : null;
                    navigateTo(maCanhBao
                        ? `/Manages/DanhGiaKetQuaThiHanhPhapLuat?CanhBao=${encodeURIComponent(maCanhBao)}`
                        : '/Manages/DanhGiaKetQuaThiHanhPhapLuat');
                }
            }
        },
        series: [{ name: 'Số lượng', data: data.canhBaoValues || data.CanhBaoValues || [] }],
        xaxis: { categories: labels },
        colors: ['#F64E60'],
        plotOptions: { bar: { borderRadius: 4, columnWidth: '50%' } },
        dataLabels: { enabled: true }
    });
}