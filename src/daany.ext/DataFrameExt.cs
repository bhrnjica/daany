using System;
using System.IO;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Daany;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Tools;
using Microsoft.ML.Transforms;

namespace Daany.Ext
{
    public static class DataFrameExt
    {
        public static void EncodeColumn(this DataFrame df, MLContext mlContext, string colName)
        {
            var colVector = df[colName];
            IDataView data = mlContext.Data.LoadFromEnumerable<CategoryColumn>(colVector.Select(x => new CategoryColumn() { Classes = x.ToString() }));
            var fitData = mlContext.Transforms.Categorical.OneHotEncoding(nameof(CategoryColumn.Classes)).Fit(data);
            var transData = fitData.Transform(data);

            //retrieve annotation from the column about slotnames
            VBuffer<ReadOnlyMemory<char>> labels = default;
            transData.Schema[nameof(CategoryColumn.Classes)].GetSlotNames(ref labels);

            var convertedData = mlContext.Data.CreateEnumerable<EncodedColumn>(transData, true);
            var originalLabels = labels.DenseValues().Select(x=>x.ToString()).ToList();
            var dict = new Dictionary<string, List<object>>();
            foreach(var r in convertedData)
            {
                for(int i=0; i< originalLabels.Count; i++)
                {
                    if (!dict.ContainsKey(originalLabels[i]))
                    {
                        var lst = new List<object>();
                        lst.Add((object)r.Classes[i]);
                        dict.Add(originalLabels[i], lst);
                    }
                    else
                        dict[originalLabels[i]].Add((object)r.Classes[i]);
                }
                
            }
            df.AddColumns(dict);
            return;
        }

        public static void CategoryToKey(this DataFrame df, MLContext mlContext, string colName)
        {
            var colVector = df[colName];
            IDataView data = mlContext.Data.LoadFromEnumerable<CategoryColumn>(colVector.Select(x => new CategoryColumn() { Classes = x.ToString() }));
            var fitData = mlContext.Transforms.Categorical.OneHotEncoding(nameof(CategoryColumn.Classes), outputKind:OneHotEncodingEstimator.OutputKind.Key).Fit(data);
            var transData = fitData.Transform(data);
            var convertedData = mlContext.Data.CreateEnumerable<CategoryValues>(transData, true);

            var dict = new Dictionary<string, List<object>>();
            var colValues = new List<object>();
            foreach (var r in convertedData)
            {
                colValues.Add(r.Classes);
            }
            dict.Add(colName+"_cvalues",colValues);
            df.AddColumns(dict);
            return;
        }

    }

   
}
