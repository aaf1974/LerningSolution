using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreWebAppSample2.MemoryCacheSample
{
    public class MemoryCacheService
    {
        IMemoryCache _memoryCache;
        public MemoryCacheService()
        {
            var option = new MemoryCacheOptions() { SizeLimit = 1024 };
            _memoryCache = new MemoryCache(option);
        }

        public long CacheRequestCount { get; set; } = 0;
        string cacheKey = "sampleKey";

        public ActionResult<GetSampleDataResponce> GetSampleData()
        {
            CacheRequestCount++;

            if(!_memoryCache.TryGetValue(cacheKey, out List<AnyCachedItem> AnyData))
            {
                AnyData = new CachedDataProvider().GetData();

                var option = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(1))
                    .SetSize(1)
                    //.SetPriority(CacheItemPriority.High)
                    //.SetAbsoluteExpiration
                    ;

                _memoryCache.Set(cacheKey, AnyData, option);
            }

            return new GetSampleDataResponce
            {
                CacheRequestCount = CacheRequestCount,
                AnyData = AnyData,
                ProviderRequestCount = CachedDataProvider.ProviderRequestCount
            };
        }

        public class GetSampleDataResponce
        {
            public long CacheRequestCount { get; set; }

            public long ProviderRequestCount { get; set; }

            public List<AnyCachedItem> AnyData { get; set; }
        }
    }
}
