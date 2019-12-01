using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daany
{
    public class Keys
    {
        object[] keys;
    }
    public class MyLookup<Keys, TOut>
    {
        private ILookup<Keys, TOut> lookup;
        public MyLookup(IEnumerable<TOut> source, Func<TOut, Keys> keySelector)
        {
            lookup = source.ToLookup(keySelector);
        }

        public IEnumerable<TOut> this[Keys elems]
        {
            get
            {
                return lookup[elems];
            }
        }

        //feel free to either expose the lookup directly, or add other methods to access the lookup
        //public TOut keySelector(Keys keys)
        //{
        //    return null;
        //}
    }

    
}
