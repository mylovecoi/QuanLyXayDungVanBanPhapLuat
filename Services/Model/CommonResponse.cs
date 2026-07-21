using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Model
{
    public class CommonResponse
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public dynamic? Data { get; set; }
        public int TotalRecord { get; set; }
        public Dictionary<string, string> ErrorMessages { get; set; } = new();

        // Constructor mặc định (nếu không truyền tham số, dùng giá trị mặc định)
        public CommonResponse()
        {
            Status = "error";
            Message = "Có lỗi xảy ra! Vui lòng thử lại sau!";
        }

        public CommonResponse(string status) : this() => Status = status;
        public CommonResponse(string status, string message) : this(status) => Message = message;
        public CommonResponse(string status, string message, dynamic? data) : this(status, message) => Data = data;
        public CommonResponse(string status, Dictionary<string, string> errorMessages) : this(status) => ErrorMessages = errorMessages;
        public CommonResponse(string status, Dictionary<string, string> errorMessages, dynamic? data) : this(status, errorMessages) => Data = data;
        public CommonResponse(string status, Dictionary<string, string> errorMessages, dynamic data, string message) : this(status, errorMessages, (object)data) => Message = message;
        public CommonResponse(string status, string message, dynamic data, int totalRecord = 0) : this(status, message, (object)data) => TotalRecord = totalRecord;
    }
}
