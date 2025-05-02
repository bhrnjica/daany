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

namespace Daany
{
    public class Index : IEnumerable<object>
    {
        public Index(List<object> ind, string name="index")
        {
            if (ind == null)
                throw new ArgumentNullException("the list object cannot be null");
            //
            _index = new List<object>(ind);
            Name = name;
        }
        public string Name { get; set; }
        private List<object> _index = new List<object>();

        public int Count => _index.Count;

        public IEnumerator<object> GetEnumerator()
        {
            return _index.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public void Add(object itm)
        {
            _index.Add(itm);
        }

        public List<object> ToList()
        {
            return _index;
        }
        public void Insert(int ind, object itm)
        {
            _index.Insert(ind, itm);
        }

        public object this[int i]
        {
            get { return _index[i]; }
           // set { _index.Insert(i, value); }
        }

        internal void Reset()
        {
            _index = nc.GenerateIntSeries(0, _index.Count, 1);
        }

        internal void AddRange(Index index)
        {
            _index.AddRange(index._index);
        }

		public int IndexOf(object value)
		{
			if (value == null)
				throw new ArgumentNullException(nameof(value), "Value cannot be null.");

			for (int i = 0; i < _index.Count; i++)
			{
				if (_index[i].Equals(value))
					return i;
			}

			return -1; // Return -1 if the value is not found  
		}
	}
}
