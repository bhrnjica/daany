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
using System.Collections;
using System.Collections.Generic;

using Daany.Interfaces;

namespace Daany.Multikey
{
	public abstract class MultiKeyDictionaryBase<TValue> : IMultiKeyDictionary
	{
		public abstract int KeyCount { get; }
		public abstract IEnumerable<object[]> GetAllKeys();
		public abstract bool ContainsKeys(params object[] keys);
		public abstract bool TryGetValue(object[] keys, out TValue value);
		public abstract void Add(object[] keys, TValue value);
		public abstract TValue? this[object[] keys] { get; set; }
	}

	public class TwoKeysDictionary<K1, K2, T> : MultiKeyDictionaryBase<T>, IEnumerable<KeyValuePair<K1, Dictionary<K2, T>>>
		where K1 : notnull
		where K2 : notnull

	{
		private readonly Dictionary<K1, Dictionary<K2, T>> _dictionary = new();
		public override int KeyCount => 2;

		public T? this[K1 key1, K2 key2]
		{
			get => ContainsKey(key1, key2) ? _dictionary[key1][key2] : default;
			set
			{
				if (value == null)
					throw new ArgumentNullException(nameof(value), "Value cannot be null");	

				if (!_dictionary.ContainsKey(key1))
					_dictionary[key1] = new Dictionary<K2, T>();
				_dictionary[key1][key2] = value;
			}
		}

		public override T? this[object[] keys]
		{
			get
			{
				if (keys == null || keys.Length != 2)
					throw new ArgumentException("Exactly 2 keys required");

				return this[(K1)keys[0], (K2)keys[1]];
			}
			set
			{
				if (keys == null || keys.Length != 2)
					throw new ArgumentException("Exactly 2 keys required");
				this[(K1)keys[0], (K2)keys[1]] = value;
			}
		}

		public bool TryGetValue(K1 key1, K2 key2, out T value)
		{
			value = default!;
			return _dictionary.TryGetValue(key1, out var innerDict) &&
				   innerDict.TryGetValue(key2, out value!);
		}

		public override bool TryGetValue(object[] keys, out T value)
		{
			value = default!;
			if (keys == null || keys.Length != 2)
				return false;
			return TryGetValue((K1)keys[0], (K2)keys[1], out value);
		}

		public void Add(K1 key1, K2 key2, T value)
		{
			if (!_dictionary.ContainsKey(key1))
				_dictionary[key1] = new Dictionary<K2, T>();

			if (_dictionary[key1].ContainsKey(key2))
				throw new ArgumentException($"Key pair ({key1}, {key2}) already exists");

			_dictionary[key1].Add(key2, value);
		}

		public override void Add(object[] keys, T value)
		{
			if (keys == null || keys.Length != 2)
				throw new ArgumentException("Exactly 2 keys required");
			Add((K1)keys[0], (K2)keys[1], value);
		}

		public bool ContainsKey(K1 key1, K2 key2)
		{
			return _dictionary.TryGetValue(key1, out var innerDict) &&
				   innerDict.ContainsKey(key2);
		}


		public override bool ContainsKeys(params object[] keys)
		{
			if (keys == null || keys.Length != 2)
				return false;
			return ContainsKey((K1)keys[0], (K2)keys[1]);
		}


		public bool Remove(K1 key1, K2 key2)
		{
			if (!_dictionary.TryGetValue(key1, out var innerDict))
				return false;

			bool removed = innerDict.Remove(key2);
			if (removed && innerDict.Count == 0)
				_dictionary.Remove(key1);

			return removed;
		}

		public override IEnumerable<object[]> GetAllKeys()
		{
			foreach (var outerPair in _dictionary)
			{
				foreach (var innerKey in outerPair.Value.Keys)
				{
					yield return new object[] { outerPair.Key, innerKey };
				}
			}
		}


		public IEnumerable<T> GetAllValues()
		{
			foreach (var innerDict in _dictionary.Values)
			{
				foreach (var value in innerDict.Values)
				{
					yield return value;
				}
			}
		}

		public int Count => _dictionary.Count;

		public void Clear()
		{
			_dictionary.Clear();
		}

		public IEnumerator<KeyValuePair<K1, Dictionary<K2, T>>> GetEnumerator()
			=> _dictionary.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator()
			=> GetEnumerator();

		public Dictionary<K2, T> this[K1 key1] => _dictionary[key1];

		public Dictionary<K2, T> GetValues(K1 key1)
			=> _dictionary.TryGetValue(key1, out var innerDict) ? innerDict : new Dictionary<K2, T>();

		public List<(K1 key1, K2 key2)> Keys
		{
			get
			{
				var keys = new List<(K1, K2)>();
				foreach (var outerPair in _dictionary)
				{
					foreach (var innerKey in outerPair.Value.Keys)
					{
						keys.Add((outerPair.Key, innerKey));
					}
				}
				return keys;
			}
		}


	}

	public class ThreeKeysDictionary<K1, K2, K3, T> : MultiKeyDictionaryBase<T>,
						IEnumerable<KeyValuePair<K1, Dictionary<K2, Dictionary<K3, T>>>>
		where K1 : notnull
		where K2 : notnull
		where K3 : notnull
	{
		private readonly Dictionary<K1, Dictionary<K2, Dictionary<K3, T>>> _dictionary = new();
		public override int KeyCount => 3;

		public T? this[K1 key1, K2 key2, K3 key3]
		{
			get => ContainsKey(key1, key2, key3) ? _dictionary[key1][key2][key3] : default;
			set
			{
				if (value == null)
					throw new ArgumentNullException(nameof(value), "Value cannot be null");

				if (!_dictionary.ContainsKey(key1))
					_dictionary[key1] = new Dictionary<K2, Dictionary<K3, T>>();

				if (!_dictionary[key1].ContainsKey(key2))
					_dictionary[key1][key2] = new Dictionary<K3, T>();

				_dictionary[key1][key2][key3] = value;
			}
		}

		public override T? this[object[] keys]
		{
			get
			{
				if (keys.Length != 3) throw new ArgumentException("Exactly 3 keys required");
				return this[(K1)keys[0], (K2)keys[1], (K3)keys[2]];
			}
			set
			{
				if (keys.Length != 3) throw new ArgumentException("Exactly 3 keys required");
				this[(K1)keys[0], (K2)keys[1], (K3)keys[2]] = value;
			}
		}

		public bool TryGetValue(K1 key1, K2 key2, K3 key3, out T value)
		{
			value = default!;
			return _dictionary.TryGetValue(key1, out var midDict) &&
				   midDict.TryGetValue(key2, out var innerDict) &&
				   innerDict.TryGetValue(key3, out value!);
		}

		public override bool TryGetValue(object[] keys, out T value)
		{
			if (keys.Length != 3)
			{
				value = default!;
				return false;
			}
			return TryGetValue((K1)keys[0], (K2)keys[1], (K3)keys[2], out value);
		}

		public void Add(K1 key1, K2 key2, K3 key3, T value)
		{
			if (!_dictionary.ContainsKey(key1))
				_dictionary[key1] = new Dictionary<K2, Dictionary<K3, T>>();

			if (!_dictionary[key1].ContainsKey(key2))
				_dictionary[key1][key2] = new Dictionary<K3, T>();

			if (_dictionary[key1][key2].ContainsKey(key3))
				throw new ArgumentException($"Key triple ({key1}, {key2}, {key3}) already exists");

			_dictionary[key1][key2].Add(key3, value);
		}

		public override void Add(object[] keys, T value)
		{
			if (keys.Length != 3) throw new ArgumentException("Exactly 3 keys required");
			Add((K1)keys[0], (K2)keys[1], (K3)keys[2], value);
		}

		public bool ContainsKey(K1 key1, K2 key2, K3 key3)
		{
			return _dictionary.TryGetValue(key1, out var midDict) &&
				   midDict.TryGetValue(key2, out var innerDict) &&
				   innerDict.ContainsKey(key3);
		}

		public override bool ContainsKeys(params object[] keys)
		{
			return keys.Length == 3 &&
				   ContainsKey((K1)keys[0], (K2)keys[1], (K3)keys[2]);
		}

		public bool Remove(K1 key1, K2 key2, K3 key3)
		{
			if (!_dictionary.TryGetValue(key1, out var midDict))
				return false;

			if (!midDict.TryGetValue(key2, out var innerDict))
				return false;

			bool removed = innerDict.Remove(key3);

			if (removed)
			{
				if (innerDict.Count == 0)
				{
					midDict.Remove(key2);
					if (midDict.Count == 0)
					{
						_dictionary.Remove(key1);
					}
				}
			}

			return removed;
		}

		public override IEnumerable<object[]> GetAllKeys()
		{
			foreach (var outerPair in _dictionary)
			{
				foreach (var midPair in outerPair.Value)
				{
					foreach (var innerKey in midPair.Value.Keys)
					{
						yield return new object[] { outerPair.Key, midPair.Key, innerKey };
					}
				}
			}
		}

		public IEnumerable<T> GetAllValues()
		{
			foreach (var midDict in _dictionary.Values)
			{
				foreach (var innerDict in midDict.Values)
				{
					foreach (var value in innerDict.Values)
					{
						yield return value;
					}
				}
			}
		}

		public int Count => _dictionary.Count;

		public void Clear()
		{
			_dictionary.Clear();
		}

		public IEnumerator<KeyValuePair<K1, Dictionary<K2, Dictionary<K3, T>>>> GetEnumerator()
			=> _dictionary.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator()
			=> GetEnumerator();

		public Dictionary<K2, Dictionary<K3, T>> this[K1 key1] => _dictionary[key1];

		public Dictionary<K3, T> this[K1 key1, K2 key2] => _dictionary[key1][key2];


		public Dictionary<K3, T> GetValues(K1 key1, K2 key2)
			=> _dictionary.TryGetValue(key1, out var midDict) &&
			   midDict.TryGetValue(key2, out var innerDict)
				? innerDict
				: new Dictionary<K3, T>();

		public List<(K1 key1, K2 key2, K3 key3)> Keys
		{
			get
			{
				var keys = new List<(K1, K2, K3)>();
				foreach (var outerPair in _dictionary)
				{
					foreach (var midPair in outerPair.Value)
					{
						foreach (var innerKey in midPair.Value.Keys)
						{
							keys.Add((outerPair.Key, midPair.Key, innerKey));
						}
					}
				}
				return keys;
			}
		}
	}
}
