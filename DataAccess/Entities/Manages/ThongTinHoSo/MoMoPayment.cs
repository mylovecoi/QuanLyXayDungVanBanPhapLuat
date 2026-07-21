using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Entities.Manages.ThongTinHoSo
{
    public class MoMoPayment : BaseEntity
    {
        #region Result Code

        /// <summary>
        /// Mã kết quả giao dịch thành công
        /// </summary>
        /// <value>0</value>
        /// <remarks>
        /// Khi giao dịch trả về mã này, giao dịch đã được xử lý thành công và không cần kiểm tra lại
        /// </remarks>
        public const int RESULT_SUCCESS = 0;

        /// <summary>
        /// Mã kết quả cho giao dịch vượt quá số lần kiểm tra cho phép
        /// </summary>
        /// <value>9999</value>
        /// <remarks>
        /// Mã này được hệ thống tự động gán khi một giao dịch đã được kiểm tra quá 5 lần 
        /// mà vẫn không chuyển sang trạng thái final. Đây là mã do hệ thống nội bộ định nghĩa,
        /// không phải từ MoMo API.
        /// </remarks>
        public const int RESULT_RETRY_EXCEEDED = 9999;

        /// <summary>
        /// Danh sách các mã kết quả có trạng thái final (không cần kiểm tra lại)
        /// </summary>
        /// <remarks>
        /// <para>Các mã kết quả này đại diện cho trạng thái cuối cùng của giao dịch:</para>
        /// <list type="bullet">
        /// <item><description><c>0</c>: Thành công</description></item>
        /// <item><description><c>98</c>: QR Code tạo không thành công. Vui lòng thử lại sau</description></item>
        /// <item><description><c>99</c>: Lỗi không xác định</description></item>
        /// <item><description><c>1001</c>: Giao dịch thanh toán thất bại do tài khoản người dùng không đủ tiền</description></item>
        /// <item><description><c>1002</c>: Giao dịch bị từ chối do nhà phát hành tài khoản thanh toán</description></item>
        /// <item><description><c>1003</c>: Giao dịch bị đã bị hủy</description></item>
        /// <item><description><c>1004</c>: Giao dịch thất bại do số tiền thanh toán vượt quá hạn mức thanh toán của người dùng</description></item>
        /// <item><description><c>1005</c>: Giao dịch thất bại do url hoặc QR code đã hết hạn</description></item>
        /// <item><description><c>1006</c>: Giao dịch thất bại do người dùng đã từ chối xác nhận thanh toán</description></item>
        /// <item><description><c>1007</c>: Giao dịch bị từ chối vì tài khoản không tồn tại hoặc đang ở trạng thái ngưng hoạt động</description></item>
        /// <item><description><c>1017</c>: Giao dịch bị hủy bởi đối tác</description></item>
        /// <item><description><c>1026</c>: Giao dịch bị hạn chế theo thể lệ chương trình khuyến mãi</description></item>
        /// <item><description><c>1080</c>: Giao dịch hoàn tiền thất bại trong quá trình xử lý. Vui lòng thử lại trong khoảng thời gian ngắn, tốt hơn là sau một giờ</description></item>
        /// <item><description><c>1081</c>: Giao dịch hoàn tiền bị từ chối. Giao dịch thanh toán ban đầu có thể đã được hoàn</description></item>
        /// <item><description><c>1088</c>: Giao dịch hoàn tiền bị từ chối. Giao dịch thanh toán ban đầu không được hỗ trợ hoàn tiền</description></item>
        /// <item><description><c>2019</c>: Yêu cầu bị từ chối vì orderGroupId không hợp lệ</description></item>
        /// <item><description><c>4001</c>: Giao dịch bị từ chối do tài khoản người dùng đang bị hạn chế</description></item>
        /// <item><description><c>4002</c>: Giao dịch bị từ chối do tài khoản người dùng chưa được xác thực với C06</description></item>
        /// <item><description><c>4100</c>: Giao dịch thất bại do người dùng không đăng nhập thành công</description></item>
        /// <item><description><c>9999</c>: Giao dịch vượt quá số lần kiểm tra trạng thái thanh toán (do hệ thống nội bộ định nghĩa)</description></item>
        /// </list>
        /// <para>
        /// Khi giao dịch có mã kết quả thuộc danh sách này, hệ thống sẽ không thực hiện kiểm tra lại 
        /// trạng thái giao dịch nữa vì đây là trạng thái cuối cùng.
        /// </para>
        /// </remarks>
        public static readonly int[] FINAL_STATUS_CODES =
        [
            0, 98, 99, 1001, 1002, 1003, 1004, 1005, 1006, 1007,
            1017, 1026, 1080, 1081, 1088, 2019, 4001, 4002, 4100, 9999
        ];

        /// <summary>
        /// Danh sách các mã kết quả không phải trạng thái final (cần kiểm tra lại)
        /// </summary>
        /// <remarks>
        /// <para>Các mã kết quả này đại diện cho trạng thái tạm thời của giao dịch cần được kiểm tra lại:</para>
        /// <list type="bullet">
        /// <item><description><c>10</c>: Hệ thống đang được bảo trì</description></item>
        /// <item><description><c>11</c>: Truy cập bị từ chối</description></item>
        /// <item><description><c>12</c>: Phiên bản API không được hỗ trợ cho yêu cầu này</description></item>
        /// <item><description><c>13</c>: Xác thực doanh nghiệp thất bại</description></item>
        /// <item><description><c>20</c>: Yêu cầu sai định dạng</description></item>
        /// <item><description><c>21</c>: Yêu cầu bị từ chối vì số tiền giao dịch không hợp lệ</description></item>
        /// <item><description><c>22</c>: Số tiền giao dịch không hợp lệ</description></item>
        /// <item><description><c>40</c>: RequestId bị trùng</description></item>
        /// <item><description><c>41</c>: OrderId bị trùng</description></item>
        /// <item><description><c>42</c>: OrderId không hợp lệ hoặc không được tìm thấy</description></item>
        /// <item><description><c>43</c>: Yêu cầu bị từ chối vì xung đột trong quá trình xử lý giao dịch</description></item>
        /// <item><description><c>45</c>: Trùng ItemId</description></item>
        /// <item><description><c>47</c>: Yêu cầu bị từ chối vì thông tin không hợp lệ trong danh sách dữ liệu khả dụng</description></item>
        /// <item><description><c>1000</c>: Giao dịch đã được khởi tạo, chờ người dùng xác nhận thanh toán</description></item>
        /// <item><description><c>7000</c>: Giao dịch đang được xử lý</description></item>
        /// <item><description><c>7002</c>: Giao dịch đang được xử lý bởi nhà cung cấp loại hình thanh toán</description></item>
        /// <item><description><c>9000</c>: Giao dịch đã được xác nhận thành công</description></item>
        /// </list>
        /// <para>
        /// Khi giao dịch có mã kết quả thuộc danh sách này, hệ thống sẽ đưa giao dịch vào hàng đợi 
        /// để kiểm tra lại sau một khoảng thời gian nhất định (thường là 2 phút).
        /// </para>
        /// <para>
        /// Số lần kiểm tra tối đa là 5 lần. Nếu sau 5 lần kiểm tra giao dịch vẫn ở trạng thái 
        /// non-final, hệ thống sẽ tự động gán mã <see cref="RESULT_RETRY_EXCEEDED"/>.
        /// </para>
        /// </remarks>
        public static readonly int[] NON_FINAL_STATUS_CODES =
        [
            10, 11, 12, 13, 20, 21, 22, 40, 41, 42, 43, 45, 47, 1000, 7000, 7002, 9000
        ];

        #endregion

        public Guid HoSoId { get; set; }

        [ForeignKey(nameof(HoSoId))]
        public virtual HoSoCCCT? HoSoCCCT { get; set; }

        public string RequestId { get; set; } = string.Empty;
        public string OrderId { get; set; } = string.Empty;
        [Precision(18, 2)]
        public decimal Amount { get; set; }
        public long? TransId { get; set; }
        public int? ResultCode { get; set; }
        public string? Message { get; set; }
        public string? PayType { get; set; }
        public long? ResponseTime { get; set; }
        public string? ExtraData { get; set; }
        public string? PaymentUrl { get; set; }
        public string? RequestSignature { get; set; }
        public string? ResponseSignature { get; set; }

        public int RetryCount { get; set; } = 0;
        public DateTime? ProcessedDate { get; set; }

        // Thêm ConcurrencyToken để tránh update conflicts
        [Timestamp]
        public byte[]? RowVersion { get; set; }
    }
}