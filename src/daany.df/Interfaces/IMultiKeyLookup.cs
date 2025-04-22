using System.Collections.Generic;

namespace Daany
{
	public interface IMultiKeyLookup<out TOut>
	{
		IEnumerable<TOut> GetByKeys(params object[] keys);
		bool ContainsKeys(params object[] keys);
	}
}
