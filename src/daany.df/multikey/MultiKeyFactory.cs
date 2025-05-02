//////////////////////////////////////////////////////////////////////////////
//   ____    _    _   _   _   __  __                                       //
//  |  _ \  / \  | \ | | | \ | |\ \/ /                                     //
//  | | | |/ _ \ |  \| | |  \| | \  /                                      //
//  | |_| / ___ \| |\  | | |\  | | |                                       //
//  |____/_/   \_\_| \_| |_| \_| |_|                                       //
//                                                                         //
//  DAata ANalYtics Library                                                //
//  Daany.DataFrame:Implementation of DataFrame.                           //
//  https://github.com/bhrnjica/daany                                      //
//                                                                         //
//  Copyright © 20019-2025 Bahrudin Hrnjica                                //
//                                                                         //
//  Free. Open Source. MIT Licensed.                                       //
//  https://github.com/bhrnjica/daany/blob/master/LICENSE                  //
//////////////////////////////////////////////////////////////////////////////
using System;
using Daany.Interfaces;

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
