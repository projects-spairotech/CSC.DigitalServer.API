using System;
using CSC.DigitalServer.Core.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace CSC.DigitalServer.Services
{
	public class MemoryCacheService : IMemoryCacheService
	{
		private readonly IMemoryCache _memoryCache;
		public MemoryCacheService(IMemoryCache memoryCache)
		{
			_memoryCache = memoryCache;
		}
		public T CacheGetValue<T>(object key)
		{
			return _memoryCache.Get<T>(key);
		}
		public void CacheRemove(object key)
		{
			_memoryCache.Remove(key);
		}
		public T CacheTryGetValue<T>(object key, T value)
		{
			if (!_memoryCache.TryGetValue(key, out value))
			{
				return CacheSetValue<T>(key, value);
			}
			else
				return CacheGetValue<T>(key);

		}
		public T CacheSetValue<T>(object key, T value)
		{
			var cacheEntryOptions = new MemoryCacheEntryOptions()
							   .SetSlidingExpiration(TimeSpan.FromHours(3));
			return _memoryCache.Set<T>(key, value, cacheEntryOptions);
		}
	}
}
