$(document).ready(() => {
    HandleRenderApexcharts().init();
    HandleFetchAllDashboardData();

    // Tự động cập nhật dữ liệu biểu đồ mỗi 30 giây (30000ms)
    setInterval(HandleFetchAllDashboardData, 30000);
});

function HandleFetchAllDashboardData() {
    HandleFetchDataForTongHopApexCharts();
    HandleFetchDataForDinhGiaApexCharts();
    HandleFetchDataForKeKhaiApexCharts();
    HandleFetchDataForGiaThiTruongApexCharts();
    HandleFetchDataForThamDinhGiaApexCharts();
}

function generateSeriesData(count, yrange) {
    const data = [];
    for (let i = 0; i < count; i++) {
        const y = Math.floor(Math.random() * (yrange.max - yrange.min + 1)) + yrange.min;
        data.push(y);
    }
    return data;
}

function HandleGetMonthLabels(prefix = 'Tháng') {
    return Array.from({ length: 12 }, (_, i) => `${prefix} ${i + 1}`);
}

const HandleRenderApexcharts = (() => {
    const primary = '#3699FF';
    const success = '#1BC5BD';
    const info = '#8950FC';
    const warning = '#FFA800';
    const danger = '#F64E60';

    const categories = HandleGetMonthLabels('Tháng');

    const labelCCCT = {
        congchung: "Hồ Sơ Công Chứng",
        chungthuc: "Hồ Sơ Chứng Thực",
    }

    const init = (data) => {
        const dataCongChung = data?.congChung ?? generateSeriesData(12, { min: 0, max: 0 });
        const dataChungThuc = data?.chungThuc ?? generateSeriesData(12, { min: 0, max: 0 });
        const dataChenhLech = dataCongChung.map((val, i) => val - dataChungThuc[i]);

        _renderChart_HoSoCCCT_ByMonth_Column(dataCongChung, dataChungThuc);
        _renderChart_HoSoCCCT_ByMonth_Area(dataCongChung, dataChungThuc);
        _renderChart_HoSoCCCT_Trend_Line(dataChenhLech);
    }

    const _renderChart_HoSoCCCT_ByMonth_Column = (dataCongChung, dataChungThuc) => {
        HandleRenderChart({
            targetId: '#CharColumTotalHoSoCCCTByYear',
            type: 'bar',
            //title: 'Hồ sơ công chứng và chứng thực theo tháng',
            plotOptions: {
                bar: {
                    horizontal: false,
                    columnWidth: '55%',
                    endingShape: 'rounded'
                },
            },
            dataLabels: {
                enabled: false
            },
            stroke: {
                show: true,
                width: 2,
                colors: ['transparent']
            },
            series: [
                { name: labelCCCT.congchung, data: dataCongChung },
                { name: labelCCCT.chungthuc, data: dataChungThuc }
            ],
            categories,
            colors: [primary, warning]
        });
    }

    const _renderChart_HoSoCCCT_ByMonth_Area = (dataCongChung, dataChungThuc) => {
        HandleRenderChart({
            targetId: '#CharAreaTotalHoSoCCCTByYear',
            type: 'area',
            //title: 'Biểu đồ Tổng hồ sơ công chứng và chứng thực theo tháng',
            dataLabels: {
                enabled: false
            },
            stroke: {
                curve: 'smooth'
            },
            series: [
                { name: labelCCCT.congchung, data: dataCongChung },
                { name: labelCCCT.chungthuc, data: dataChungThuc }
            ],
            categories,
            colors: ['#28a745', '#17a2b8']
        });
    }

    const _renderChart_HoSoCCCT_Trend_Line = (dataChenhLech) => {
        HandleRenderChart({
            targetId: '#CharLineInsDesHoSoCCCTByYear',
            type: 'line',
            //title: 'Chênh lệch hồ sơ theo tháng',
            series: [
                { name: 'Chênh lệch (Công chứng - Chứng thực)', data: dataChenhLech }
            ],
            categories,
            colors: ['#dc3545']
        });
    }

    return { init };
});

const HandleRenderChart = (options = {}) => {
    const {
        targetId,
        type = 'bar',
        title = '',
        series = [],
        colors = [],
        height = 250,
        curve = 'smooth',
        showToolbar = false,
        tooltipSuffix = ' hồ sơ',
        plotOptions = {},
        dataLabels = {
            enabled: false
        },
        stroke = {}
    } = options;

    const categories = Array.isArray(options.categories) ? options.categories : [];

    const el = document.querySelector(targetId);
    if (!el) {
        console.warn(`Không tìm thấy phần tử với targetId: ${targetId}`);
        return;
    }

    const chartOptions = {
        series,
        chart: {
            type,
            height,
            toolbar: { show: showToolbar }
        },
        plotOptions,
        dataLabels,
        stroke: {
            ...stroke,
            curve
        },
        xaxis: {
            categories,
            ...(options.xaxis ?? {})
        },
        colors,
        title: {
            text: title,
            align: 'center',         // 'left', 'center', 'right'
            margin: 0,              // khoảng cách giữa title và chart
            offsetX: 0,              // dịch theo trục X
            offsetY: 0,              // dịch theo trục Y
            floating: false,         // true = không chiếm không gian biểu đồ
            style: {
                fontSize: '16px',
                fontWeight: 'bold',
                fontFamily: undefined,
                color: '#263238'
            }
        },
        tooltip: {
            y: {
                formatter: function (val) {
                    return val + tooltipSuffix;
                }
            }
        }
    };

    //const chart = new ApexCharts(document.querySelector(targetId), chartOptions);
    //chart.render();

    // Nếu đã có chart instance => update
    if (el._chart) {
        el._chart.updateSeries(series, true);
    } else {
        const chart = new ApexCharts(el, chartOptions);
        chart.render();
        el._chart = chart; // gán chart vào DOM để sử dụng lại sau
    }
};

const HandleFetchDataForApexCharts = () => {
    HandleFetchAjax({
        url: '/Reports/BaoCaoKhac/GetSoLuongHoSoTiepNhanTheoThang',
        beforeSendCallback: () => {
        },
        successCallback: (res) => {
            HandleRenderApexcharts().init(res.data);
        },
        errorCallback: (xhr, status, error) => {
            toastr.error('Không nhận được phản hồi từ máy chủ. Vui lòng thử lại sau.');
        }
    });
};

const HandleRenderDinhGiaApexcharts = (() => {
    const primary = '#3699FF';
    const success = '#1BC5BD';

    const init = (data) => {
        if (data && data.year) {
            $('.dinhgia-chart-year').text(data.year);
        }

        const dgByNghe = (data?.dinhGiaByMaNghe ?? data?.DinhGiaByMaNghe ?? []);
        const dgCategories = dgByNghe.map(x => {
            const name = x.tenNghe ?? x.TenNghe ?? 'Khác';
            return name.length > 22 ? name.substring(0, 22) + '...' : name;
        });
        const dgTotalCounts = dgByNghe.map(x => x.totalCount ?? x.TotalCount ?? 0);
        const dgApprovedCounts = dgByNghe.map(x => x.approvedCount ?? x.ApprovedCount ?? 0);

        // Column Chart for DinhGia
        HandleRenderChart({
            targetId: '#CharColumTotalDinhGiaByYear',
            type: 'bar',
            plotOptions: {
                bar: {
                    horizontal: false,
                    columnWidth: '55%',
                    endingShape: 'rounded'
                },
            },
            dataLabels: { enabled: false },
            stroke: {
                show: true,
                width: 2,
                colors: ['transparent']
            },
            series: [
                { name: "Tổng số hồ sơ", data: dgTotalCounts },
                { name: "Hồ sơ đã duyệt/công bố", data: dgApprovedCounts }
            ],
            categories: dgCategories,
            colors: [primary, success],
            xaxis: {
                labels: {
                    rotate: -45,
                    rotateAlways: false,
                    hideOverlappingLabels: true,
                    trim: true,
                    maxHeight: 120,
                    style: {
                        fontSize: '10px'
                    }
                }
            }
        });

        // Donut Chart for DinhGia (Percentage of different MaNghe)
        const elDgDonut = document.querySelector('#CharDonutDinhGiaStatus');
        if (elDgDonut) {
            const donutSeries = dgTotalCounts;
            const donutLabels = dgCategories;
            const donutOptions = {
                series: donutSeries,
                chart: {
                    type: 'donut',
                    height: 280
                },
                labels: donutLabels,
                responsive: [{
                    breakpoint: 480,
                    options: {
                        chart: { width: 200 },
                        legend: { position: 'bottom' }
                    }
                }]
            };

            if (elDgDonut._chart) {
                elDgDonut._chart.updateSeries(donutSeries, true);
                elDgDonut._chart.updateOptions({ labels: donutLabels });
            } else {
                const chart = new ApexCharts(elDgDonut, donutOptions);
                chart.render();
                elDgDonut._chart = chart;
            }
        }
    }

    return { init };
})();

const HandleRenderKeKhaiApexcharts = (() => {
    const primary = '#3699FF';
    const success = '#1BC5BD';
    const info = '#8950FC';
    const warning = '#FFA800';
    const danger = '#F64E60';

    const monthCategories = HandleGetMonthLabels('Tháng');

    const init = (data) => {
        if (data && data.year) {
            $('.kekhai-chart-year').text(data.year);
        }

        const kkMonthlyCounts = data?.monthlyCounts ?? data?.MonthlyCounts ?? generateSeriesData(12, { min: 0, max: 0 });
        const kkMonthlyApprovedCounts = data?.monthlyApprovedCounts ?? data?.MonthlyApprovedCounts ?? generateSeriesData(12, { min: 0, max: 0 });

        HandleRenderChart({
            targetId: '#CharColumTotalKeKhaiByYear',
            type: 'bar',
            plotOptions: {
                bar: {
                    horizontal: false,
                    columnWidth: '55%',
                    endingShape: 'rounded'
                },
            },
            dataLabels: { enabled: false },
            stroke: {
                show: true,
                width: 2,
                colors: ['transparent']
            },
            series: [
                { name: "Tổng số hồ sơ", data: kkMonthlyCounts },
                { name: "Hồ sơ đã duyệt/công bố", data: kkMonthlyApprovedCounts }
            ],
            categories: monthCategories,
            colors: [primary, success]
        });

        const kkStatusCounts = data?.statusCounts ?? data?.StatusCounts ?? { CC: 0, CD: 0, DD: 0, CB: 0, BTL: 0 };
        const cc = kkStatusCounts.CC ?? kkStatusCounts.cc ?? kkStatusCounts.cC ?? 0;
        const cd = kkStatusCounts.CD ?? kkStatusCounts.cd ?? kkStatusCounts.cD ?? 0;
        const dd = kkStatusCounts.DD ?? kkStatusCounts.dd ?? kkStatusCounts.dD ?? 0;
        const cb = kkStatusCounts.CB ?? kkStatusCounts.cb ?? kkStatusCounts.cB ?? 0;
        const btl = kkStatusCounts.BTL ?? kkStatusCounts.btl ?? kkStatusCounts.btL ?? 0;
        const kkDonutSeries = [cc, cd, dd, cb, btl];

        const elKkDonut = document.querySelector('#CharDonutKeKhaiStatus');
        if (elKkDonut) {
            const donutOptions = {
                series: kkDonutSeries,
                chart: {
                    type: 'donut',
                    height: 250
                },
                labels: ["Chờ chuyển", "Chờ duyệt", "Đã duyệt", "Công bố", "Bị trả lại"],
                colors: [info, warning, success, primary, danger],
                responsive: [{
                    breakpoint: 480,
                    options: {
                        chart: { width: 200 },
                        legend: { position: 'bottom' }
                    }
                }]
            };

            if (elKkDonut._chart) {
                elKkDonut._chart.updateSeries(kkDonutSeries, true);
            } else {
                const chart = new ApexCharts(elKkDonut, donutOptions);
                chart.render();
                elKkDonut._chart = chart;
            }
        }
    }

    return { init };
})();

const HandleRenderGiaThiTruongApexcharts = (() => {
    const primary = '#3699FF';
    const success = '#1BC5BD';
    const info = '#8950FC';
    const warning = '#FFA800';
    const danger = '#F64E60';

    const monthCategories = HandleGetMonthLabels('Tháng');

    const init = (data) => {
        if (data && data.year) {
            $('.giathitruong-chart-year').text(data.year);
        }

        const gttMonthlyCounts = data?.monthlyCounts ?? data?.MonthlyCounts ?? generateSeriesData(12, { min: 0, max: 0 });
        const gttMonthlyApprovedCounts = data?.monthlyApprovedCounts ?? data?.MonthlyApprovedCounts ?? generateSeriesData(12, { min: 0, max: 0 });

        HandleRenderChart({
            targetId: '#CharColumTotalGiaThiTruongByYear',
            type: 'bar',
            plotOptions: {
                bar: {
                    horizontal: false,
                    columnWidth: '55%',
                    endingShape: 'rounded'
                },
            },
            dataLabels: { enabled: false },
            stroke: {
                show: true,
                width: 2,
                colors: ['transparent']
            },
            series: [
                { name: "Tổng số hồ sơ", data: gttMonthlyCounts },
                { name: "Hồ sơ đã duyệt/công bố", data: gttMonthlyApprovedCounts }
            ],
            categories: monthCategories,
            colors: [primary, success]
        });

        const gttStatusCounts = data?.statusCounts ?? data?.StatusCounts ?? { CC: 0, CD: 0, DD: 0, CB: 0, BTL: 0 };
        const cc = gttStatusCounts.CC ?? gttStatusCounts.cc ?? gttStatusCounts.cC ?? 0;
        const cd = gttStatusCounts.CD ?? gttStatusCounts.cd ?? gttStatusCounts.cD ?? 0;
        const dd = gttStatusCounts.DD ?? gttStatusCounts.dd ?? gttStatusCounts.dD ?? 0;
        const cb = gttStatusCounts.CB ?? gttStatusCounts.cb ?? gttStatusCounts.cB ?? 0;
        const btl = gttStatusCounts.BTL ?? gttStatusCounts.btl ?? gttStatusCounts.btL ?? 0;
        const gttDonutSeries = [cc, cd, dd, cb, btl];

        const elGttDonut = document.querySelector('#CharDonutGiaThiTruongStatus');
        if (elGttDonut) {
            const donutOptions = {
                series: gttDonutSeries,
                chart: {
                    type: 'donut',
                    height: 250
                },
                labels: ["Chờ chuyển", "Chờ duyệt", "Đã duyệt", "Công bố", "Bị trả lại"],
                colors: [info, warning, success, primary, danger],
                responsive: [{
                    breakpoint: 480,
                    options: {
                        chart: { width: 200 },
                        legend: { position: 'bottom' }
                    }
                }]
            };

            if (elGttDonut._chart) {
                elGttDonut._chart.updateSeries(gttDonutSeries, true);
            } else {
                const chart = new ApexCharts(elGttDonut, donutOptions);
                chart.render();
                elGttDonut._chart = chart;
            }
        }
    }

    return { init };
})();

const HandleFetchDataForDinhGiaApexCharts = () => {
    HandleFetchAjax({
        url: '/DinhGiaBaoCao/GetSoLuongDinhGiaTheoThang',
        successCallback: (res) => {
            HandleRenderDinhGiaApexcharts.init(res.data);
        },
        errorCallback: (xhr, status, error) => {
            toastr.error('Không nhận được phản hồi từ máy chủ. Vui lòng thử lại sau.');
        }
    });
};

const HandleFetchDataForKeKhaiApexCharts = () => {
    HandleFetchAjax({
        url: '/KeKhaiDangKyGia/GetSoLuongKeKhaiTheoThang',
        successCallback: (res) => {
            HandleRenderKeKhaiApexcharts.init(res.data);
        },
        errorCallback: (xhr, status, error) => {
            toastr.error('Không nhận được phản hồi từ máy chủ. Vui lòng thử lại sau.');
        }
    });
};

const HandleFetchDataForGiaThiTruongApexCharts = () => {
    HandleFetchAjax({
        url: '/GiaThiTruong/GetSoLuongGiaThiTruongTheoThang',
        successCallback: (res) => {
            HandleRenderGiaThiTruongApexcharts.init(res.data);
        },
        errorCallback: (xhr, status, error) => {
            toastr.error('Không nhận được phản hồi từ máy chủ. Vui lòng thử lại sau.');
        }
    });
};

const HandleRenderThamDinhGiaApexcharts = (() => {
    const primary = '#3699FF';
    const success = '#1BC5BD';
    const info = '#8950FC';
    const warning = '#FFA800';
    const danger = '#F64E60';

    const monthCategories = HandleGetMonthLabels('Tháng');

    const init = (data) => {
        if (data && data.year) {
            $('.thamdinhgia-chart-year').text(data.year);
        }

        const tdMonthlyCounts = data?.monthlyCounts ?? data?.MonthlyCounts ?? generateSeriesData(12, { min: 0, max: 0 });
        const tdMonthlyApprovedCounts = data?.monthlyApprovedCounts ?? data?.MonthlyApprovedCounts ?? generateSeriesData(12, { min: 0, max: 0 });

        HandleRenderChart({
            targetId: '#CharColumTotalThamDinhGiaByYear',
            type: 'bar',
            plotOptions: {
                bar: {
                    horizontal: false,
                    columnWidth: '55%',
                    endingShape: 'rounded'
                },
            },
            dataLabels: { enabled: false },
            stroke: {
                show: true,
                width: 2,
                colors: ['transparent']
            },
            series: [
                { name: "Tổng số hồ sơ", data: tdMonthlyCounts },
                { name: "Hồ sơ đã duyệt/công bố", data: tdMonthlyApprovedCounts }
            ],
            categories: monthCategories,
            colors: [primary, success]
        });

        const tdStatusCounts = data?.statusCounts ?? data?.StatusCounts ?? { CC: 0, CD: 0, DD: 0, CB: 0, BTL: 0 };
        const cc = tdStatusCounts.CC ?? tdStatusCounts.cc ?? tdStatusCounts.cC ?? 0;
        const cd = tdStatusCounts.CD ?? tdStatusCounts.cd ?? tdStatusCounts.cD ?? 0;
        const dd = tdStatusCounts.DD ?? tdStatusCounts.dd ?? tdStatusCounts.dD ?? 0;
        const cb = tdStatusCounts.CB ?? tdStatusCounts.cb ?? tdStatusCounts.cB ?? 0;
        const btl = tdStatusCounts.BTL ?? tdStatusCounts.btl ?? tdStatusCounts.btL ?? 0;
        const tdDonutSeries = [cc, cd, dd, cb, btl];

        const elTdDonut = document.querySelector('#CharDonutThamDinhGiaStatus');
        if (elTdDonut) {
            const donutOptions = {
                series: tdDonutSeries,
                chart: {
                    type: 'donut',
                    height: 250
                },
                labels: ["Chờ chuyển", "Chờ duyệt", "Đã duyệt", "Công bố", "Bị trả lại"],
                colors: [info, warning, success, primary, danger],
                responsive: [{
                    breakpoint: 480,
                    options: {
                        chart: { width: 200 },
                        legend: { position: 'bottom' }
                    }
                }]
            };

            if (elTdDonut._chart) {
                elTdDonut._chart.updateSeries(tdDonutSeries, true);
            } else {
                const chart = new ApexCharts(elTdDonut, donutOptions);
                chart.render();
                elTdDonut._chart = chart;
            }
        }
    }

    return { init };
})();

const HandleFetchDataForThamDinhGiaApexCharts = () => {
    HandleFetchAjax({
        url: '/ThamDinhGia/GetSoLuongThamDinhGiaTheoThang',
        successCallback: (res) => {
            HandleRenderThamDinhGiaApexcharts.init(res.data);
        },
        errorCallback: (xhr, status, error) => {
            toastr.error('Không nhận được phản hồi từ máy chủ. Vui lòng thử lại sau.');
        }
    });
};


const HandleRenderTongHopApexcharts = (() => {
    const primary = '#3699FF';
    const success = '#1BC5BD';
    const warning = '#FFA800';
    const danger = '#F64E60';

    const monthCategories = HandleGetMonthLabels('Tháng');

    const init = (data) => {
        if (data && data.year) {
            $('.tonghop-chart-year').text(data.year);
        }

        const monthlyDG = data?.monthlyDG ?? data?.MonthlyDG ?? generateSeriesData(12, { min: 0, max: 0 });
        const monthlyKK = data?.monthlyKK ?? data?.MonthlyKK ?? generateSeriesData(12, { min: 0, max: 0 });
        const monthlyGTT = data?.monthlyGTT ?? data?.MonthlyGTT ?? generateSeriesData(12, { min: 0, max: 0 });
        const monthlyTotal = data?.monthlyTotal ?? data?.MonthlyTotal ?? generateSeriesData(12, { min: 0, max: 0 });

        HandleRenderChart({
            targetId: '#CharCombinedSinWaveByYear',
            type: 'area',
            height: 350,
            curve: 'smooth',
            series: [
                { name: "Định giá HHDV", data: monthlyDG },
                { name: "Kê khai đăng ký giá", data: monthlyKK },
                { name: "Giá thị trường", data: monthlyGTT },
                { name: "Tổng cộng hồ sơ", data: monthlyTotal }
            ],
            categories: monthCategories,
            colors: [success, warning, primary, danger]
        });
    }

    return { init };
})();

const HandleFetchDataForTongHopApexCharts = () => {
    HandleFetchAjax({
        url: '/Home/GetTongHopHoSoStats',
        successCallback: (res) => {
            HandleRenderTongHopApexcharts.init(res.data);
        },
        errorCallback: (xhr, status, error) => {
            toastr.error('Không nhận được phản hồi từ máy chủ. Vui lòng thử lại sau.');
        }
    });
};