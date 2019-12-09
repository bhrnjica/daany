using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Daany
{
    public class Index : IEnumerable<object>
    {
        public Index(List<object> ind, string name="index")
        {
            if (ind == null)
                throw new Exception("the list object cannot be null");
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
            _index = nc.GenerateIntNSeries(0, 1, _index.Count);
        }

        internal void AddRange(Index index)
        {
            _index.AddRange(index._index);
        }
    }
}
