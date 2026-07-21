namespace UI.ViewModels
{
    public class VMPageInfo
    {
        public string? Search { get; set; }
        public int TotalRecord { get; set; }
        public int PageSize { get; set; }
        public int PageCurrent { get; set; }
        public int PageTotal { get; set; }
        public string? Sort { get; set; }
        public List<int>? PageRange { get; set; }
        public int RecordStart { get; set; }

    }

    public class VMPageInfoWithData<T> : VMPageInfo
    {
        public List<T> Data { get; set; }

        public VMPageInfoWithData() => Data = new List<T>();

        public VMPageInfoWithData(VMPageInfo basePageInfo, List<T> data) : this()
        {
            Search = basePageInfo.Search;
            TotalRecord = basePageInfo.TotalRecord;
            PageSize = basePageInfo.PageSize;
            PageCurrent = basePageInfo.PageCurrent;
            PageTotal = basePageInfo.PageTotal;
            PageRange = basePageInfo.PageRange;
            RecordStart = basePageInfo.RecordStart;
            Sort = basePageInfo.Sort; // Nếu cần
            Data = data;
        }
    }
}
