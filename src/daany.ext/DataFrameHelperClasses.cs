using System;
using System.IO;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Daany;

namespace Daany.Ext
{
    public class CategoryColumn
    {
        public string Classes { get; set; }
    }

    public class EncodedColumn
    {
        public float[] Classes { get; set; }
    }

    public class CategoryValues
    {
        public uint Classes { get; set; }
    }

    public class LookupMap
    {
        public string Key { get; set; }
    }
}
