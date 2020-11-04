using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
namespace Daany
{
    public class Series : IEnumerable<object>
    {
        public Series(List<object> data, List<object> ind = null, string name = "series", ColType type= ColType.STR)
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
            else if (ind==null)
               _index = new Index(Enumerable.Range(0, _data.Count).Select(x=>(object)x).ToList());
            this._type = type;
        }

        internal Series(List<object> data, Index index, string name = "series", ColType type = ColType.STR)
        {
            this._data = data.Select(x => x).ToList();
            this.Name = name;
            this._index = new Index(index.Select(x => x).ToList());
            this._type = type;
        }


        public Series (Series ser)
        {
            this._data = ser._data.Select(x=>x).ToList();
            this.Name = ser.Name;
            this._index = new Index(ser._index.Select(x=>x).ToList());
            this._type = ser._type;
        }

       
        public string Name { get; set; }
        private List<object> _data;
        private ColType _type= ColType.STR;
        private Index _index;

        public int Count => _data.Count;
        public Index Index => _index;
        public ColType ColType => _type;
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
            set { _data[i] =  value;}
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


        public DataFrame AppendHorizontaly(Series ser)
        {
            var df = new DataFrame(this._data, this._index.ToList(), new List<string>{this.Name }, new ColType[] { ser.ColType});
            df = df.AddColumn(ser);
           
            return df;
        }

        public Series Rolling(int window, Aggregation agg)
        {
            DataFrame df = this.ToDataFrame();
            var dff = df.Rolling(window, agg);
            Series ser = dff.ToSeries(); 
            return ser;
        }

        private DataFrame ToDataFrame()
        {
            var cols = new List<string>() { this.Name };
            var colType = new ColType[] { this.ColType};
            var df = new DataFrame(this._data, this._index, cols, colType);
            return df;
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


        /// <summary>
        /// Return number of missing values in the Series.
        /// </summary>
        /// <returns></returns>
        public int MissingValues()
        {
            return this.Where(x => x == DataFrame.NAN).Count();
        }
        public Series DropNA()
        {
            var dat = new List<object>();
            var indLst = new List<object>();

            for(int i=0; i< this._data.Count; i++)
            {
                if(this[i] != DataFrame.NAN)
                {
                    dat.Add(this[i]);
                    indLst.Add(this._index[i]);
                }
            }

            //
            return new Series(dat, indLst, this.Name);
        }

        public Series FillNA(object replacedValue)
        {
            var dat = new List<object>();
            var indLst = new List<object>();

            for (int i = 0; i < this._data.Count; i++)
            {
                if (this[i] != DataFrame.NAN)
                {
                    dat.Add(this[i]);
                    indLst.Add(this._index[i]);
                }
                else
                {
                    dat.Add(replacedValue);
                    indLst.Add(this._index[i]);
                }
            }

            //
            return new Series(dat, indLst, this.Name);
        }

        public Series FillNA(Aggregation aggValue)
        {
            var dat = new List<object>();
            var indLst = new List<object>();
            var replacedValue = DataFrame._calculateAggregation(this._data.Where(x=>x != DataFrame.NAN), aggValue, this._type);
            for (int i = 0; i < this._data.Count; i++)
            {
                if (this[i] != DataFrame.NAN)
                {
                    dat.Add(this[i]);
                    indLst.Add(this._index[i]);
                }
                else
                {
                    dat.Add(replacedValue);
                    indLst.Add(this._index[i]);
                }
            }
            //
            return new Series(dat, indLst, this.Name);
        }

        public Series FillNA(object replacedValue, Func<int, object> replDelg)
        {
            var dat = new List<object>();
            var indLst = new List<object>();

            for (int i = 0; i < this._data.Count; i++)
            {
                if (this[i] != DataFrame.NAN)
                {
                    dat.Add(this[i]);
                    indLst.Add(this._index[i]);
                }
                else
                {
                    var rplVal = replDelg(i);
                    dat.Add(rplVal);
                    indLst.Add(this._index[i]);
                }
            }

            //
            return new Series(dat, indLst, this.Name);
        }
        #endregion

        #region Operators
        /// <summary>
        /// Element-wise subtraction of two series
        /// </summary>
        /// <param name="ser1"></param>
        /// <param name="ser2"></param>
        /// <returns></returns>
        public static Series operator -(Series ser1, Series ser2)
        {
            var t = ser1.detectType();
            var ser3 = new Series(ser1);
            switch (t)
            {
                case ColType.I2:
                    throw new Exception("Series is of boolean type. Substraction cannot be applied.");
                case ColType.IN:
                    throw new Exception("Series is of categorical type. Substraction cannot be applied.");
                case ColType.STR:
                    throw new Exception("Series is of string type. Substraction cannot be applied.");

                case ColType.I32:
                    {
                        for (int i = 0; i < ser1._data.Count; i++)
                        {
                            ser3[i] = Convert.ToInt32(ser1[i]) - Convert.ToInt32(ser2[i]);
                        }
                    }
                    break;
                case ColType.I64:
                    {
                        for (int i = 0; i < ser1._data.Count; i++)
                        {
                            ser3[i] = Convert.ToInt64(ser1[i]) - Convert.ToInt64(ser2[i]);
                        }
                    }
                    break;
                case ColType.F32:
                    {
                        for (int i = 0; i < ser1._data.Count; i++)
                        {
                            ser3[i] = Convert.ToSingle(ser1[i]) - Convert.ToSingle(ser2[i]);
                        }
                    }
                    break;
                case ColType.DD:
                    {
                        for (int i = 0; i < ser1._data.Count; i++)
                        {
                            ser3[i] = Convert.ToDouble(ser1[i]) - Convert.ToDouble(ser2[i]);
                        }
                    }
                    break;
                case ColType.DT:
                    throw new Exception("Series is of datetime type. Substraction cannot be applied.");
                default:
                    throw new Exception("Series is of unknown type. Substraction cannot be applied.");
            }
            return ser3;
        }

        /// <summary>
        /// Element-wise addition of two series
        /// </summary>
        /// <param name="ser1"></param>
        /// <param name="ser2"></param>
        /// <returns></returns>
        public static Series operator +(Series ser1, Series ser2)
        {
            var t = ser1.detectType();
            var ser3 = new Series(ser1);
            switch (t)
            {
                case ColType.I2:
                    throw new Exception("Series is of boolean type. Addition cannot be applied.");
                case ColType.IN:
                    throw new Exception("Series is of categorical type. Addition cannot be applied.");
                case ColType.STR:
                    throw new Exception("Series is of string type. Addition cannot be applied.");

                case ColType.I32:
                    {
                        for (int i = 0; i < ser1._data.Count; i++)
                        {
                            ser3[i] = Convert.ToInt32(ser1[i]) + Convert.ToInt32(ser2[i]);
                        }
                    }    
                    break;
                case ColType.I64:
                    {
                        for (int i = 0; i < ser1._data.Count; i++)
                        {
                            ser3[i] = Convert.ToInt64(ser1[i]) + Convert.ToInt64(ser2[i]);
                        }
                    }
                    break;
                case ColType.F32:
                    {
                        for (int i = 0; i < ser1._data.Count; i++)
                        {
                            ser3[i] = Convert.ToSingle(ser1[i]) + Convert.ToSingle(ser2[i]);
                        }
                    }
                    break;
                case ColType.DD:
                    {
                        for (int i = 0; i < ser1._data.Count; i++)
                        {
                            ser3[i] = Convert.ToDouble(ser1[i]) + Convert.ToDouble(ser2[i]);
                        }
                    }
                    break;
                case ColType.DT:
                    throw new Exception("Series is of datetime type. Addition cannot be applied.");
                default:
                    throw new Exception("Series is of unknown type. Addition cannot be applied.");
            }
            return ser3;
        }

        /// <summary>
        /// Addition series element with floating scalar value
        /// </summary>
        /// <param name="ser1"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static Series operator +(Series ser1, float scalar)
        {
            var t = ser1.detectType();
            var ser3 = new Series(ser1);
            switch (t)
            {
                case ColType.I2:
                    throw new Exception("Series is of boolean type. Addition cannot be applied.");
                case ColType.IN:
                    throw new Exception("Series is of categorical type. Addition cannot be applied.");
                case ColType.STR:
                    throw new Exception("Series is of string type. Addition cannot be applied.");

                case ColType.I32:
                    {
                        for (int i = 0; i < ser1._data.Count; i++)
                        {
                            ser3[i] = Convert.ToInt32(ser1[i]) + scalar;
                        }
                    }
                    break;
                case ColType.I64:
                    {
                        for (int i = 0; i < ser1._data.Count; i++)
                        {
                            ser3[i] = Convert.ToInt64(ser1[i]) + scalar;
                        }
                    }
                    break;
                case ColType.F32:
                    {
                        for (int i = 0; i < ser1._data.Count; i++)
                        {
                            ser3[i] = Convert.ToSingle(ser1[i]) + scalar;
                        }
                    }
                    break;
                case ColType.DD:
                    {
                        for (int i = 0; i < ser1._data.Count; i++)
                        {
                            ser3[i] = Convert.ToDouble(ser1[i]) + scalar;
                        }
                    }
                    break;
                case ColType.DT:
                    throw new Exception("Series is of datetime type. Addition cannot be applied.");
                default:
                    throw new Exception("Series is of unknown type. Addition cannot be applied.");
            }
            return ser3;
        }

        /// <summary>
        /// Element-wise multiplication of two series
        /// </summary>
        /// <param name="ser1"></param>
        /// <param name="ser2"></param>
        /// <returns></returns>
        public static Series operator *(Series ser1, Series ser2)
        {
            var t = ser1.detectType();
            var ser3 = new Series(ser1);
            switch (t)
            {
                case ColType.I2:
                    throw new Exception("Series is of boolean type. Multiplication cannot be applied.");
                case ColType.IN:
                    throw new Exception("Series is of categorical type. Multiplication cannot be applied.");
                case ColType.STR:
                    throw new Exception("Series is of string type. Multiplication cannot be applied.");

                case ColType.I32:
                    {
                        for (int i = 0; i < ser1._data.Count; i++)
                        {
                            ser3[i] = Convert.ToInt32(ser1[i]) * Convert.ToInt32(ser2[i]);
                        }
                    }
                    break;
                case ColType.I64:
                    {
                        for (int i = 0; i < ser1._data.Count; i++)
                        {
                            ser3[i] = Convert.ToInt64(ser1[i]) * Convert.ToInt64(ser2[i]);
                        }
                    }
                    break;
                case ColType.F32:
                    {
                        for (int i = 0; i < ser1._data.Count; i++)
                        {
                            ser3[i] = Convert.ToSingle(ser1[i]) * Convert.ToSingle(ser2[i]);
                        }
                    }
                    break;
                case ColType.DD:
                    {
                        for (int i = 0; i < ser1._data.Count; i++)
                        {
                            ser3[i] = Convert.ToDouble(ser1[i]) * Convert.ToDouble(ser2[i]);
                        }
                    }
                    break;
                case ColType.DT:
                    throw new Exception("Series is of datetime type. Multiplication cannot be applied.");
                default:
                    throw new Exception("Series is of unknown type. Multiplication cannot be applied.");
            }
            return ser3;
        }

        /// <summary>
        /// Multiplication series element with floating scalar value
        /// </summary>
        /// <param name="ser1"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static Series operator *(Series ser1, float scalar)
        {
            var t = ser1.detectType();
            var ser3 = new Series(ser1);
            switch (t)
            {
                case ColType.I2:
                    throw new Exception("Series is of boolean type. Multiplication cannot be applied.");
                case ColType.IN:
                    throw new Exception("Series is of categorical type. Multiplication cannot be applied.");
                case ColType.STR:
                    throw new Exception("Series is of string type. Multiplication cannot be applied.");

                case ColType.I32:
                    {
                        for (int i = 0; i < ser1._data.Count; i++)
                        {
                            ser3[i] = Convert.ToInt32(ser1[i]) * scalar;
                        }
                    }
                    break;
                case ColType.I64:
                    {
                        for (int i = 0; i < ser1._data.Count; i++)
                        {
                            ser3[i] = Convert.ToInt64(ser1[i]) * scalar;
                        }
                    }
                    break;
                case ColType.F32:
                    {
                        for (int i = 0; i < ser1._data.Count; i++)
                        {
                            ser3[i] = Convert.ToSingle(ser1[i]) + scalar;
                        }
                    }
                    break;
                case ColType.DD:
                    {
                        for (int i = 0; i < ser1._data.Count; i++)
                        {
                            ser3[i] = Convert.ToDouble(ser1[i]) * scalar;
                        }
                    }
                    break;
                case ColType.DT:
                    throw new Exception("Series is of datetime type. Multiplication cannot be applied.");
                default:
                    throw new Exception("Series is of unknown type. Multiplication cannot be applied.");
            }
            return ser3;
        }
        #endregion


        #region Internal and Private Methods
        internal void Reset()
        {
            _data = nc.GenerateIntSeries(0, _data.Count, 1);
        }

        private ColType detectType()
        {
            int cc = SCount();
            int k = 0;
            while (_data[k] == DataFrame.NAN)
            {
                k++;
                //if the type is not found put default type to the column
                if (_data.Count < k)
                {
                    _data[k] = ColType.STR;
                    k = 0;
                    break;
                }
                continue;
            }
            //
            if (_data[k].GetType() == typeof(bool))
                _type = ColType.I2;
            else if (_data[k].GetType() == typeof(int))
                _type = ColType.I32;
            else if (_data[k].GetType() == typeof(long))
                _type = ColType.I64;
            else if (_data[k].GetType() == typeof(float))
                _type = ColType.F32;
            else if (_data[k].GetType() == typeof(double))
                _type = ColType.DD;
            else if (_data[k].GetType() == typeof(string))
                _type = ColType.STR;
            else if (_data[k].GetType() == typeof(DateTime))
                _type = ColType.DT;
            else
                throw new Exception("Unknown column type");
            return _type;
        }

        private int SCount()
        {
            if (_data == null)
                return 0;
            else
                return _data.Count;
        }
        #endregion

    }
}
