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
using System.Collections.Generic;
using System.Linq;

namespace Daany.Multikey
{
	public class TwoKeyLookup<T1, T2, TOut>
		   where T1 : notnull
		   where T2 : notnull
	{
		private readonly ILookup<(T1, T2), TOut> _lookup;

		public TwoKeyLookup(IEnumerable<TOut> source, Func<TOut, (T1, T2)> keySelector)
		{
			_lookup = source.ToLookup(keySelector);
		}

		public IEnumerable<TOut> this[T1 first, T2 second] => _lookup[(first, second)];

		public bool Contains(T1 first, T2 second)
		{
			return _lookup.Contains((first, second));
		}
	}

	public class ThreeKeyLookup<T1, T2, T3, TOut>
		   where T1 : notnull
		   where T2 : notnull
		   where T3 : notnull
	{
		private readonly ILookup<(T1, T2, T3), TOut> _lookup;

		public ThreeKeyLookup(IEnumerable<TOut> source, Func<TOut, (T1, T2, T3)> keySelector)
		{
			_lookup = source.ToLookup(keySelector);
		}

		public IEnumerable<TOut> this[T1 first, T2 second, T3 third] => _lookup[(first, second, third)];

		public bool Contains(T1 first, T2 second, T3 third)
		{
			return _lookup.Contains((first, second, third));
		}
	}
}
