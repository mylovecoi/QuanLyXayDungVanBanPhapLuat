namespace Services.DTOs.BaoCao12
{
    /// <summary>
    /// DTO cho response báo cáo 12a - Tình hình tổ chức và hoạt động công chứng
    /// </summary>
    public class BaoCao12aDto
    {
        /// <summary>
        /// Tên đơn vị báo cáo
        /// </summary>
        public string TenDonVi { get; set; } = string.Empty;

        /// <summary>
        /// Số công chứng viên
        /// </summary>
        public int SoCongChungVien { get; set; }

        /// <summary>
        /// Tổng số việc công chứng
        /// </summary>
        public int SoViecCongChung { get; set; }

        /// <summary>
        /// Số việc công chứng hợp đồng
        /// </summary>
        public int CongChungHopDong { get; set; }

        /// <summary>
        /// Số việc công chứng bản dịch
        /// </summary>
        public int CongChungBanDich { get; set; }

        /// <summary>
        /// Tổng thu lao công
        /// </summary>
        public decimal TongThuLaoCong { get; set; }

        /// <summary>
        /// Tổng phí công chứng
        /// </summary>
        public decimal TongPhiCongChung { get; set; }

        /// <summary>
        /// Số việc chứng thực bản sao
        /// </summary>
        public int ChungThucBanSao { get; set; }

        /// <summary>
        /// Phí chứng thực bản sao
        /// </summary>
        public decimal PhiChungThucBanSao { get; set; }

        /// <summary>
        /// Số việc chứng thực chữ ký
        /// </summary>
        public int SoViecChungThucChuKy { get; set; }

        /// <summary>
        /// Phí chứng thực chữ ký
        /// </summary>
        public decimal PhiChungThucChuKy { get; set; }

        /// <summary>
        /// Tổng tiền nộp ngân sách
        /// </summary>
        public double TongTienNopNganSach { get; set; }
    }

    /// <summary>
    /// DTO cho response báo cáo 12b - Tình hình tổ chức và hoạt động công chứng tại địa bàn tỉnh
    /// </summary>
    public class BaoCao12bDto
    {
        /// <summary>
        /// Tên đơn vị báo cáo
        /// </summary>
        public string TenDonVi { get; set; } = string.Empty;

        /// <summary>
        /// I. Phòng Công chứng - Số công chứng viên
        /// </summary>
        public int PhongCongChung_SoCongChungVien { get; set; }

        /// <summary>
        /// I. Phòng Công chứng - Tổng số việc công chứng
        /// </summary>
        public int PhongCongChung_SoViecCongChung { get; set; }

        /// <summary>
        /// I. Phòng Công chứng - Công chứng hợp đồng, giao dịch
        /// </summary>
        public int PhongCongChung_CongChungHopDong { get; set; }

        /// <summary>
        /// I. Phòng Công chứng - Công chứng bản dịch và các loại việc khác
        /// </summary>
        public int PhongCongChung_CongChungBanDich { get; set; }

        /// <summary>
        /// I. Phòng Công chứng - Tổng thu lao công
        /// </summary>
        public decimal PhongCongChung_TongThuLaoCong { get; set; }

        /// <summary>
        /// I. Phòng Công chứng - Tổng phí công chứng
        /// </summary>
        public decimal PhongCongChung_TongPhiCongChung { get; set; }

        /// <summary>
        /// I. Phòng Công chứng - Chứng thực bản sao - Số bản sao
        /// </summary>
        public int PhongCongChung_ChungThucBanSao { get; set; }

        /// <summary>
        /// I. Phòng Công chứng - Chứng thực bản sao - Phí chứng thực bản sao
        /// </summary>
        public decimal PhongCongChung_PhiChungThucBanSao { get; set; }

        /// <summary>
        /// I. Phòng Công chứng - Chứng thực chữ ký - Số việc
        /// </summary>
        public int PhongCongChung_SoViecChungThucChuKy { get; set; }

        /// <summary>
        /// I. Phòng Công chứng - Chứng thực chữ ký - Phí chứng thực chữ ký
        /// </summary>
        public decimal PhongCongChung_PhiChungThucChuKy { get; set; }

        /// <summary>
        /// I. Phòng Công chứng - Tổng số tiền nộp vào ngân sách/thuế
        /// </summary>
        public double PhongCongChung_TongTienNopNganSach { get; set; }

        /// <summary>
        /// II. Văn phòng Công chứng - Số công chứng viên
        /// </summary>
        public int VanPhongCongChung_SoCongChungVien { get; set; }

        /// <summary>
        /// II. Văn phòng Công chứng - Tổng số việc công chứng
        /// </summary>
        public int VanPhongCongChung_SoViecCongChung { get; set; }

        /// <summary>
        /// II. Văn phòng Công chứng - Công chứng hợp đồng, giao dịch
        /// </summary>
        public int VanPhongCongChung_CongChungHopDong { get; set; }

        /// <summary>
        /// II. Văn phòng Công chứng - Công chứng bản dịch và các loại việc khác
        /// </summary>
        public int VanPhongCongChung_CongChungBanDich { get; set; }

        /// <summary>
        /// II. Văn phòng Công chứng - Tổng thu lao công
        /// </summary>
        public decimal VanPhongCongChung_TongThuLaoCong { get; set; }

        /// <summary>
        /// II. Văn phòng Công chứng - Tổng phí công chứng
        /// </summary>
        public decimal VanPhongCongChung_TongPhiCongChung { get; set; }

        /// <summary>
        /// II. Văn phòng Công chứng - Chứng thực bản sao - Số bản sao
        /// </summary>
        public int VanPhongCongChung_ChungThucBanSao { get; set; }

        /// <summary>
        /// II. Văn phòng Công chứng - Chứng thực bản sao - Phí chứng thực bản sao
        /// </summary>
        public decimal VanPhongCongChung_PhiChungThucBanSao { get; set; }

        /// <summary>
        /// II. Văn phòng Công chứng - Chứng thực chữ ký - Số việc
        /// </summary>
        public int VanPhongCongChung_SoViecChungThucChuKy { get; set; }

        /// <summary>
        /// II. Văn phòng Công chứng - Chứng thực chữ ký - Phí chứng thực chữ ký
        /// </summary>
        public decimal VanPhongCongChung_PhiChungThucChuKy { get; set; }

        /// <summary>
        /// II. Văn phòng Công chứng - Tổng số tiền nộp vào ngân sách/thuế
        /// </summary>
        public double VanPhongCongChung_TongTienNopNganSach { get; set; }
    }

    /// <summary>
    /// DTO cho response tổng quát của báo cáo 12
    /// </summary>
    public class BaoCao12ResponseDto
    {
        /// <summary>
        /// Thông tin request gốc
        /// </summary>
        public BaoCao12RequestDto Request { get; set; } = new();

        /// <summary>
        /// Dữ liệu báo cáo 12a (nếu có)
        /// </summary>
        public BaoCao12aDto? BaoCao12a { get; set; }

        /// <summary>
        /// Dữ liệu báo cáo 12b (nếu có)
        /// </summary>
        public BaoCao12bDto? BaoCao12b { get; set; }

        /// <summary>
        /// Thông tin bổ sung
        /// </summary>
        public Dictionary<string, object> MetaData { get; set; } = new();
    }
}
