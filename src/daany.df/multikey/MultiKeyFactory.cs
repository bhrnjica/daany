using Daany.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daany.Multikey
{

	public static class MultiKeyDictionaryFactory
	{
		public static IMultiKeyDictionary Create(int keyCount, Type[] keyTypes, Type valueType)
		{
			if (keyCount == 2)
			{
				Type dictType = typeof(TwoKeysDictionary<,,>).MakeGenericType(
					keyTypes[0], keyTypes[1], valueType);
				if (Activator.CreateInstance(dictType) is IMultiKeyDictionary dic)
					return dic;
				else
					throw new InvalidOperationException("Failed to create TwoKeysDictionary instance.");
			}
			else if (keyCount == 3)
			{
				Type dictType = typeof(ThreeKeysDictionary<,,,>).MakeGenericType(
					keyTypes[0], keyTypes[1], keyTypes[2], valueType);
				if (Activator.CreateInstance(dictType) is IMultiKeyDictionary dic)
					return dic;
				else
					throw new InvalidOperationException("Failed to create ThreeKeysDictionary instance.");
			}

			throw new NotSupportedException("Only 2 or 3 key dictionaries are supported");
		}
	}
}
