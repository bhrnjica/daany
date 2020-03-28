using System;
using System.Collections.Generic;
using System.Text;

namespace Daany.MathStuff
{
    public static class Constant
    {
        public static  ThreadSafeRandom rand = new ThreadSafeRandom();
        static bool _fixedRandom = false;
        public static bool FixedRandomSeed
        {
            get
            {
                return _fixedRandom;
            }
            set
            {
                _fixedRandom = value;
                ThreadSafeRandom.FixedRandomSeed = value;
                rand = new ThreadSafeRandom();
            }
        } 
    }
}
