using System;

namespace Services.DTOs.Manages.ThongTinHoSo.BaoCaoThongKe
{
    // DTO riêng cho Mẫu báo cáo 12a - Biểu số: 12a/BTP/BTTP/CC
    public class MauSo12ResponseDto
    {
        // Cột (1) - Số công chứng viên
        public int SoCongChungVien { get; set; }
        
        // Cột (2) - Số việc công chứng
        public int SoViecCongChung { get; set; }
        
        // Cột (3) - Công chứng hợp đồng, giao dịch
        public int CongChungHopDong { get; set; }
        
        // Cột (4) - Công chứng bản dịch và các loại việc khác
        public int CongChungBanDich { get; set; }
        
        // Cột (5) - Tổng số thu lao công (đồng)
        public double TongThuLaoCong { get; set; }
        
        // Cột (6) - Tổng số phí công chứng (đồng)
        public double TongPhiCongChung { get; set; }
        
        // Cột (7) - Chứng thực bản sao (việc)
        public int ChungThucBanSao { get; set; }
        
        // Cột (8) - Phí chứng thực bản sao (đồng)
        public double PhiChungThucBanSao { get; set; }
        
        // Cột (9) - Số việc chứng thực chữ ký trong giấy tờ, văn bản (việc)  
        public int SoViecChungThucChuKy { get; set; }
        
        // Cột (10) - Phí chứng thực chữ ký (đồng)
        public double PhiChungThucChuKy { get; set; }
        
        // Cột (11) - Tổng số tiền nộp vào ngân sách/thuế (đồng)
        public double TongTienNopNganSach { get; set; }
    }
}
