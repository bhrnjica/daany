using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
namespace Daany
{
    public class Series : IEnumerable<object>
    {
        public Series(List<object> data, List<object> ind = null, string name = "series")
        {
            if (data == null)
                throw new Exception("the list object cannot be null");
            //
            _data = new List<object>(data);
            Name = name;
            if (ind != null && ind.Count == data.Count)
                this._index = new Index(ind);
            else if (ind != null && ind.Count != data.Count)
                throw new Exception("Series index is not consistent with the series");
        }

        public Series (Series ser)
        {
            this._data = ser._data.Select(x=>x).ToList();
            this._index = new Index(ser._index.Select(x=>x).ToList());
        }
        public string Name { get; set; }
        private List<object> _data;
        private Index _index;

        public int Count => _data.Count;

        public IEnumerator<object> GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }


        #region Indexers
        public Series this[params int[] indexes]
        {
            get
            {
                var lst = new List<object>();
                var ind = new List<object>();
                for (int i = 0; i < indexes.Length; i++)
                {
                    lst.Add(this._data[indexes[i]]);
                    ind.Add(this._index[indexes[i]]);
                }

                return new Series(lst, ind);
            }
        }

        public object this[int i]
        {
            get { return _data[i]; }
            // set { _index.Insert(i, value); }
        }
        
        #endregion

        #region Operations


        public Series AppendVerticaly(Series ser)
        {
            var s = new Series(this);
            s._data.AddRange(ser._data);
            s._index.AddRange(ser._index);

            return s;
        }


        public Series AppendHorizontaly(Series ser)
        {
            var df = new DataFrame(this._data, this._index.ToList(), new List<string>{this.Name });
            df.AddColumn(ser);
            var s = new Series(this);
            s._data.AddRange(ser._data);
            s._index.AddRange(ser._index);

            return s;
        }

        public void Add(object itm)
        {
            _data.Add(itm);
        }

        public List<object> ToList()
        {
            return _data;
        }
        public void Insert(int ind, object itm)
        {
            _data.Insert(ind, itm);
        }
        #endregion
        #region Internal and Private Methods
        internal void Reset()
        {
            _data = nc.GenerateIntNSeries(0, 1, _data.Count);
        }
        #endregion

    }
}
