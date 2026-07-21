namespace Services.DTOs.BaoCao17
{
    /// <summary>
    /// DTO validation cho báo cáo 17
    /// </summary>
    public class BaoCao17ValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> ErrorMessages { get; set; } = new();
        public List<string> WarningMessages { get; set; } = new();

        public void AddError(string message)
        {
            IsValid = false;
            ErrorMessages.Add(message);
        }

        public void AddWarning(string message)
        {
            WarningMessages.Add(message);
        }
    }

    /// <summary>
    /// Constants cho báo cáo 17
    /// </summary>
    public static class BaoCao17Constants
    {
        // Thời hạn nộp báo cáo theo Thông tư 03/2019
        public static class ThoiHanNopBaoCao
        {
            // 17a - UBND cấp xã
            public const int BaoCao17a_6Thang_Ngay = 6;
            public const int BaoCao17a_6Thang_Thang = 6;
            public const int BaoCao17a_Nam_Ngay = 7;
            public const int BaoCao17a_Nam_Thang = 11;
            public const int BaoCao17a_NamChinhThuc_Ngay = 20;
            public const int BaoCao17a_NamChinhThuc_Thang = 1;

            // 17b - Phòng Tư pháp
            public const int BaoCao17b_6Thang_Ngay = 16;
            public const int BaoCao17b_6Thang_Thang = 6;
            public const int BaoCao17b_Nam_Ngay = 18;
            public const int BaoCao17b_Nam_Thang = 11;
            public const int BaoCao17b_NamChinhThuc_Ngay = 31;
            public const int BaoCao17b_NamChinhThuc_Thang = 1;

            // 17c - Sở Tư pháp
            public const int BaoCao17c_6Thang_Ngay = 25;
            public const int BaoCao17c_6Thang_Thang = 6;
            public const int BaoCao17c_Nam_Ngay = 28;
            public const int BaoCao17c_Nam_Thang = 11;
            public const int BaoCao17c_NamChinhThuc_Ngay = 20;
            public const int BaoCao17c_NamChinhThuc_Thang = 2;

            // 17d - Bộ Ngoại giao
            public const int BaoCao17d_NamChinhThuc_Ngay = 31;
            public const int BaoCao17d_NamChinhThuc_Thang = 1;
        }

        public static class BieuMau
        {
            public const string BieuSo17a = "17a/BTP/HTQTCT/CT";
            public const string BieuSo17b = "17b/BTP/HTQTCT/CT";
            public const string BieuSo17c = "17c/BTP/HTQTCT/CT";
            public const string BieuSo17d = "17d/BTP/HTQTCT/CT";

            public const string ThongTuBanHanh = "Thông tư số 03/2019/TT-BTP ngày 20/3/2019";
        }

        public static class TieuDe
        {
            public const string BaoCao17a = "KẾT QUẢ CHỨNG THỰC TẠI ỦY BAN NHÂN DÂN (UBND) CẤP XÃ";
            public const string BaoCao17b = "KẾT QUẢ CHỨNG THỰC CỦA PHÒNG TƯ PHÁP VÀ ỦY BAN NHÂN DÂN (UBND) CẤP XÃ TRÊN ĐỊA BÀN HUYỆN";
            public const string BaoCao17c = "KẾT QUẢ CHỨNG THỰC CỦA PHÒNG TƯ PHÁP VÀ ỦY BAN NHÂN DÂN (UBND) CẤP XÃ TRÊN ĐỊA BÀN TỈNH";
            public const string BaoCao17d = "KẾT QUẢ CHỨNG THỰC CỦA CÁC CƠ QUAN ĐẠI DIỆN VIỆT NAM Ở NƯỚC NGOÀI";
        }
    }
}
