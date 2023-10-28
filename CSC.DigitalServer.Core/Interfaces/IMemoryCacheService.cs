using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSC.DigitalServer.Core.Interfaces
{
	public interface IMemoryCacheService
	{
		T CacheGetValue<T>(object key);
		void CacheRemove(object key);
		T CacheTryGetValue<T>(object key, T value);
		T CacheSetValue<T>(object key, T value);
	}
}
