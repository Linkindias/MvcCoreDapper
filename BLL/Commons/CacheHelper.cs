using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using System.Text;
using static Base.Enums;

namespace BLL.Commons
{
    public static class CacheHelper
    {
        static MemoryCache cache;
        static CacheItemPolicy cacheItemPolicy;

        const string LockKey = "core"; //識別唯一的 lock

        /// <summary>
        /// 取得快取物件
        /// </summary>
        /// <param name="key">caceh key</param>
        /// <returns>是否有值</returns>
        /// <returns>值</returns>
        public static (bool Iskey, object value) GetCacheObject(string key)
        {
            string lockKey = $"{LockKey}{key}"; // 取得每個 Key 的鎖定 object

            if (cache[lockKey] != null) return (true, cache[lockKey]);

            return (false, new object());
        }

        public static Object AddCacheCollection<T>(string key, List<T> value, CacheStatus status, int minutes = 0, int days = 0)
        {
            return CacheHelper.AddCacheObject(key, value, status, minutes, days);
        }

        public static Object AddCacheString(string key, string value, CacheStatus status, int minutes = 0, int days = 0)
        {
            return CacheHelper.AddCacheObject(key, value, status, minutes, days);
        }

        /// <summary>
        /// 加入快取物件
        /// </summary>
        /// <param name="key">caceh key</param>
        /// <param name="value">值</param >
        /// <returns>值</returns>
        private static Object AddCacheObject(string key, object value, CacheStatus status, int minutes = 0, int days = 0)
        {
            string lockKey = $"{LockKey}{key}"; // 取得每個 Key 的鎖定 object

            //仍然會 lock 整個 memorycahce object 但少了取資料過程  lock 時間會縮短
            lock (cache)
            {
                //當cache不存在時，則建立該cache且絕對時間為隔天整點
                if (cache[lockKey] == null)
                {
                    cacheItemPolicy = status == (int)CacheStatus.Sliding
                        ? new CacheItemPolicy()
                        {
                            SlidingExpiration = TimeSpan.FromMinutes(minutes),
                            UpdateCallback = new CacheEntryUpdateCallback(OnCacheUpdated),
                        }
                        : new CacheItemPolicy()
                        {
                            AbsoluteExpiration = DateTime.Today.AddDays(days),
                            UpdateCallback = new CacheEntryUpdateCallback(OnCacheUpdated),
                        };
                    cache.Set(lockKey, value, cacheItemPolicy);
                }
            }
            return cache[lockKey];
        }

        /// <summary>
        /// 建立快取物件
        /// </summary>
        public static void CreateCache()
        {
            cache = MemoryCache.Default;
        }

        //移除前通知
        private static void OnCacheUpdated(CacheEntryUpdateArguments arguments)
        {
            if (1 == 1) { }
        }

        /// <summary>
        /// 移除該項快取物件
        /// </summary>
        public static void RemoveItem(string key)
        {
            cache.Remove($"{LockKey}{key}");
        }

        /// <summary>
        /// 移除快取物件
        /// </summary>
        public static void RemoveCache()
        {
            foreach (var item in cache)
                cache.Remove($"{LockKey}{item.Key}");

            cache = null;
            cacheItemPolicy = null;
        }
    }
}
