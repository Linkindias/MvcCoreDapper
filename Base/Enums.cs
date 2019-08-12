using System;

namespace Base
{
    public class Enums
    {
        public enum DataStatus //資料狀態
        {
            Enable = 10, //啟用
            Disable = 20, //停用
            Lock = 30, //鎖定
        }

        public enum CacheStatus //快取狀態
        {
            Sliding, //滑動
            Absolute, //絕對
        }
    }
}
