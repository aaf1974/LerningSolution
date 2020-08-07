using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreWebAppSample2.MemoryCacheSample
{
    public class CachedDataProvider
    {
        public static long ProviderRequestCount { get; set; } = 0;

        public List<AnyCachedItem> GetData()
        {
            ProviderRequestCount++;

            return new List<AnyCachedItem>()
            {
                new AnyCachedItem(){Id = 1}
            };
        }
    }


    public class AnyCachedItem
    {
        public int Id { get; internal set; }
    }
}
