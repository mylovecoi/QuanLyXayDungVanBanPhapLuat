using System.ComponentModel;

namespace Services.DTOs.BaoCao17
{
    /// <summary>
    /// DTO cơ sở cho dữ liệu báo cáo 17
    /// </summary>
    public abstract class BaoCao17BaseDto
    {
        [DisplayName("Chứng thực bản sao (Bản)")]
        public int ChungThucBanSao { get; set; }

        [DisplayName("Chứng thực chữ ký trong giấy tờ, văn bản (Việc)")]
        public int ChungThucChuKy { get; set; }

        [DisplayName("Chứng thực hợp đồng, giao dịch (Việc)")]
        public int ChungThucHopDong { get; set; }
    }

    /// <summary>
    /// DTO cho dữ liệu báo cáo 17a - UBND cấp xã
    /// </summary>
    public class BaoCao17aDto : BaoCao17BaseDto
    {
        [DisplayName("Tên UBND Cấp Xã")]
        public string TenUBNDCapXa { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO cho dữ liệu báo cáo 17b - Cấp huyện
    /// </summary>
    public class BaoCao17bDto
    {
        [DisplayName("Kết quả chứng thực tại Phòng Tư pháp")]
        public BaoCao17PhongTuPhapDto KetQuaPhongTuPhap { get; set; } = new();

        [DisplayName("Tổng hợp kết quả chứng thực của UBND cấp xã")]
        public List<BaoCao17aDto> DanhSachUBNDCapXa { get; set; } = new();

        [DisplayName("Tổng số UBND cấp xã")]
        public BaoCao17aDto TongSoUBNDCapXa { get; set; } = new();
    }

    /// <summary>
    /// DTO cho kết quả chứng thực tại Phòng Tư pháp (có thêm chứng thực chữ ký người dịch)
    /// </summary>
    public class BaoCao17PhongTuPhapDto : BaoCao17BaseDto
    {
        [DisplayName("Chứng thực chữ ký người dịch (Việc)")]
        public int ChungThucChuKyNguoiDich { get; set; }
    }

    /// <summary>
    /// DTO cho dữ liệu báo cáo 17c - Cấp tỉnh
    /// </summary>
    public class BaoCao17cDto
    {
        [DisplayName("Kết quả chứng thực tại các Phòng Tư pháp trên địa bàn tỉnh")]
        public List<BaoCao17PhongTuPhapItemDto> DanhSachPhongTuPhap { get; set; } = new();

        [DisplayName("Tổng số Phòng Tư pháp")]
        public BaoCao17PhongTuPhapDto TongSoPhongTuPhap { get; set; } = new();

        [DisplayName("Kết quả chứng thực tại các UBND cấp xã trên địa bàn tỉnh")]
        public List<BaoCao17HuyenItemDto> DanhSachHuyen { get; set; } = new();

        [DisplayName("Tổng số UBND cấp xã")]
        public BaoCao17aDto TongSoUBNDCapXa { get; set; } = new();
    }

    /// <summary>
    /// DTO cho từng Phòng Tư pháp trong báo cáo 17c
    /// </summary>
    public class BaoCao17PhongTuPhapItemDto : BaoCao17PhongTuPhapDto
    {
        [DisplayName("Tên Phòng Tư pháp")]
        public string TenPhongTuPhap { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO cho từng huyện trong báo cáo 17c (tổng hợp UBND cấp xã theo huyện)
    /// </summary>
    public class BaoCao17HuyenItemDto : BaoCao17BaseDto
    {
        [DisplayName("Tên Huyện")]
        public string TenHuyen { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO cho dữ liệu báo cáo 17d - Cơ quan đại diện nước ngoài
    /// </summary>
    public class BaoCao17dDto
    {
        [DisplayName("Danh sách cơ quan đại diện")]
        public List<BaoCao17CoQuanDaiDienDto> DanhSachCoQuanDaiDien { get; set; } = new();

        [DisplayName("Tổng số")]
        public BaoCao17CoQuanDaiDienBaseDto TongSo { get; set; } = new();
    }

    /// <summary>
    /// DTO cơ sở cho cơ quan đại diện nước ngoài
    /// </summary>
    public class BaoCao17CoQuanDaiDienBaseDto
    {
        [DisplayName("Chứng thực bản sao (Bản)")]
        public int ChungThucBanSao { get; set; }

        [DisplayName("Chứng thực chữ ký trong giấy tờ, văn bản (Việc)")]
        public int ChungThucChuKy { get; set; }

        [DisplayName("Chứng thực chữ ký người dịch trong các giấy tờ, văn bản (Việc)")]
        public int ChungThucChuKyNguoiDich { get; set; }
    }

    /// <summary>
    /// DTO cho từng cơ quan đại diện nước ngoài
    /// </summary>
    public class BaoCao17CoQuanDaiDienDto : BaoCao17CoQuanDaiDienBaseDto
    {
        [DisplayName("Tên Cơ quan đại diện")]
        public string TenCoQuanDaiDien { get; set; } = string.Empty;

        [DisplayName("Quốc gia")]
        public string QuocGia { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO response chung cho tất cả loại báo cáo 17
    /// </summary>
    public class BaoCao17ResponseDto
    {
        public BaoCao17RequestDto Request { get; set; } = new();
        public BaoCao17aDto? BaoCao17a { get; set; }
        public BaoCao17bDto? BaoCao17b { get; set; }
        public BaoCao17cDto? BaoCao17c { get; set; }
        public BaoCao17dDto? BaoCao17d { get; set; }
        public DateTime NgayTaoBaoCao { get; set; } = DateTime.Now;
    }
}
