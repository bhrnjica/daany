using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Daany.Util
{
    public class DataFrameExt
    {
        public static IEnumerable<string> GetLabels(DataViewSchema schema)
        {
            VBuffer<ReadOnlyMemory<char>> labels = new VBuffer<ReadOnlyMemory<char>>();

            foreach (var d in schema)
            {
                if (d.HasKeyValues())
                {
                    d.GetKeyValues(ref labels);
                    var predLables = labels.DenseValues().Select(x => x.ToString());
                    return predLables;
                }
                else if (d.HasSlotNames())
                {
                    d.GetSlotNames(ref labels);
                    var predLables = labels.DenseValues().Select(x => x.ToString());
                    return predLables;
                }
            }

            //this should not heppend
            throw new Exception("No category labels are defined.");
        }
    }
}
