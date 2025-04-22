using System.Collections.Generic;

namespace Daany.Interfaces
{
	public interface IMultiKeyDictionary
	{
		int KeyCount { get; }
		IEnumerable<object[]> GetAllKeys();
		bool ContainsKeys(params object[] keys);
	}
}
