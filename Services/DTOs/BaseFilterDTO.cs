using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace Services.DTOs
{
    public class BaseFilterDTO
    {
        public int TargetYear { get; private set; }
        public string? Search { get; private set; }
        public int PageSize { get; private set; } = 5;
        public int PageCurrent { get; private set; } = 1;

        public BaseFilterDTO() { }

        public BaseFilterDTO(int pageSize)
        {
            PageSize = pageSize;
        }

        public BaseFilterDTO(HttpRequest request)
        {
            TargetYear = int.TryParse(request.Query["Year"], out var targetYear) ? targetYear : DateTime.Now.Year;
            Search = request.Query.TryGetValue("Search", out var keyword) && !string.IsNullOrEmpty(keyword) ? keyword.ToString().ToLower().Trim() : string.Empty;

            PageCurrent = int.TryParse(request.Query["PageCurrent"], out var pageCurrent) ? pageCurrent : 1;
            PageCurrent = PageCurrent < 1 ? 1 : PageCurrent;

            PageSize = int.TryParse(request.Query["PageSize"], out var pageSize) ? pageSize : 5;
            PageSize = PageSize < 5 ? 5 : (PageSize > 100 ? 100 : PageSize);
        }

        /// <summary>
        /// Trường hợp request gửi lên là trang quá số totalpage thì trả về trang = totalpage
        /// </summary>
        /// <param name="totalCount"></param>
        public void AdjustPageIfInvalid(int totalCount)
        {
            var maxPage = (int)Math.Ceiling((double)Math.Max(1, totalCount) / PageSize);
            PageCurrent = Math.Min(PageCurrent, maxPage);
        }

        public void SetDefaultPaging(int page, int size)
        {
            PageCurrent = page;
            PageSize = size;
        }
    }
}
