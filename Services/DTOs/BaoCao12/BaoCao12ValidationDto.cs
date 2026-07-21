namespace Services.DTOs.BaoCao12
{
    /// <summary>
    /// DTO validation cho báo cáo 12
    /// </summary>
    public class BaoCao12ValidationResult
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
    /// Constants cho báo cáo 12
    /// </summary>
    public static class BaoCao12Constants
    {
        // Thời hạn nộp báo cáo theo quy định
        public static class ThoiHanNopBaoCao
        {
            // 12a - Tình hình tổ chức và hoạt động công chứng
            public const int BaoCao12a_6Thang_Ngay = 6;
            public const int BaoCao12a_6Thang_Thang = 6;
            public const int BaoCao12a_Nam_Ngay = 7;
            public const int BaoCao12a_Nam_Thang = 11;
            public const int BaoCao12a_NamChinhThuc_Ngay = 20;
            public const int BaoCao12a_NamChinhThuc_Thang = 1;

            // 12b - Tình hình tổ chức và hoạt động công chứng tại địa bàn tỉnh
            public const int BaoCao12b_6Thang_Ngay = 6;
            public const int BaoCao12b_6Thang_Thang = 6;
            public const int BaoCao12b_Nam_Ngay = 7;
            public const int BaoCao12b_Nam_Thang = 11;
            public const int BaoCao12b_NamChinhThuc_Ngay = 20;
            public const int BaoCao12b_NamChinhThuc_Thang = 1;
        }

        public static class BieuMau
        {
            public const string BieuSo12a = "12a/BTP/BTTP/CC";
            public const string BieuSo12b = "12b/BTP/BTTP/CC";
            public const string ThongTuBanHanh = "Thông tư số 03/2019/TT-BTP ngày 20/3/2019";
        }

        public static class TieuDe
        {
            public const string BaoCao12a = "TÌNH HÌNH TỔ CHỨC VÀ HOẠT ĐỘNG CÔNG CHỨNG";
            public const string BaoCao12b = "TÌNH HÌNH TỔ CHỨC VÀ HOẠT ĐỘNG CÔNG CHỨNG TẠI ĐỊA BÀN TỈNH";
        }
    }
}


