using System.ComponentModel;

namespace DataAccess.Enums
{
    public enum LoaiBaoCao17
    {
        [Description("17a - Kết quả chứng thực tại UBND cấp xã")]
        BaoCao17a = 1,

        [Description("17b - Kết quả chứng thực của Phòng Tư pháp và UBND cấp xã trên địa bàn huyện")]
        BaoCao17b = 2,

        [Description("17c - Kết quả chứng thực của Phòng Tư pháp và UBND cấp xã trên địa bàn tỉnh")]
        BaoCao17c = 3,

        [Description("17d - Kết quả chứng thực của các cơ quan đại diện Việt Nam ở nước ngoài")]
        BaoCao17d = 4
    }
}
