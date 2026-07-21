using DataAccess.Enums;
using Services.DTOs.BaoCao17;
using Services.Model;

namespace Services.BaoCao17
{
    /// <summary>
    /// Interface cho service báo cáo 17 theo Thông tư 03/2019/TT-BTP
    /// </summary>
    public interface IBaoCao17Service
    {
        /// <summary>
        /// Validate request báo cáo 17
        /// </summary>
        Task<BaoCao17ValidationResult> ValidateRequestAsync(BaoCao17RequestDto request);

        /// <summary>
        /// Lấy dữ liệu báo cáo 17a - Kết quả chứng thực tại UBND cấp xã
        /// </summary>
        Task<CommonResponse> GetBaoCao17aAsync(BaoCao17RequestDto request);

        /// <summary>
        /// Lấy dữ liệu báo cáo 17b - Kết quả chứng thực của Phòng Tư pháp và UBND cấp xã trên địa bàn huyện
        /// </summary>
        Task<CommonResponse> GetBaoCao17bAsync(BaoCao17RequestDto request);

        /// <summary>
        /// Lấy dữ liệu báo cáo 17c - Kết quả chứng thực của Phòng Tư pháp và UBND cấp xã trên địa bàn tỉnh
        /// </summary>
        Task<CommonResponse> GetBaoCao17cAsync(BaoCao17RequestDto request);

        /// <summary>
        /// Lấy dữ liệu báo cáo 17d - Kết quả chứng thực của các cơ quan đại diện Việt Nam ở nước ngoài
        /// </summary>
        Task<CommonResponse> GetBaoCao17dAsync(BaoCao17RequestDto request);

        /// <summary>
        /// Export báo cáo 17 ra file Word
        /// </summary>
        Task<CommonResponse> ExportBaoCao17ToWordAsync(BaoCao17RequestDto request);

        /// <summary>
        /// Export báo cáo 17 ra file Excel
        /// </summary>
        Task<CommonResponse> ExportBaoCao17ToExcelAsync(BaoCao17RequestDto request);

        /// <summary>
        /// Lấy thông tin đơn vị cho báo cáo theo cấp
        /// </summary>
        Task<CommonResponse> GetDonViInfoForBaoCaoAsync(Guid donViId, LoaiBaoCao17 loaiBaoCao);

        /// <summary>
        /// Tính toán thời hạn nộp báo cáo theo quy định
        /// </summary>
        DateTime GetThoiHanNopBaoCao(LoaiBaoCao17 loaiBaoCao, KyBaoCao17 kyBaoCao, int nam);

        /// <summary>
        /// Kiểm tra quyền tạo báo cáo theo cấp đơn vị
        /// </summary>
        Task<bool> CheckPermissionAsync(Guid userId, LoaiBaoCao17 loaiBaoCao);
    }
}
