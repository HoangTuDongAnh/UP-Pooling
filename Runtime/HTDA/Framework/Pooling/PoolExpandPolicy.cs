namespace HTDA.Framework.Pooling
{
    public enum PoolExpandPolicy
    {
        /// <summary>Hết item thì tạo thêm (Instantiate) cho tới khi chạm MaxSize (nếu có).</summary>
        Expand,

        /// <summary>Hết item thì trả null / fail.</summary>
        Fail,

        /// <summary>Hết item thì lấy lại item cũ nhất đang dùng (thường không khuyến nghị cho gameplay).</summary>
        ReuseOldest
    }
}