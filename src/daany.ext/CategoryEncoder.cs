//////////////////////////////////////////////////////////////////////////////////////////
// Daany - DAta ANalYtics Library                                                        //
// https://github.com/bhrnjica/daany                                                    //
//                                                                                      //
// Copyright 2006-2018 Bahrudin Hrnjica                                                 //
//                                                                                      //
// This code is free software under the MIT License                                     //
// See license section of  https://github.com/bhrnjica/daany/blob/master/LICENSE        //
//                                                                                      //
// Bahrudin Hrnjica                                                                     //
// bhrnjica at hotmail.com                                                              //
// Bihac, Bosnia and Herzegovina                                                        //
// http://bhrnjica.wordpress.com                                                        //
//////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Daany;
using Daany.Ext;

namespace Daany.Ext
{
    public static class CategoryEncoder
    {
        public static (DataFrame, string[]) EncodeColumn(this DataFrame df, string colName, CategoryEncoding encoder, bool encodedOnly = false)
        {
            switch (encoder)
            {
                case CategoryEncoding.None:
                    return (df, null);
                case CategoryEncoding.Binary1:
                    return BinaryEncoding(df, colName, encodedOnly);
                case CategoryEncoding.Binary2:
                    return BinaryEncoding2(df, colName, encodedOnly);
                case CategoryEncoding.Ordinal:
                    return OrdinalEncoding(df, colName, encodedOnly);
                case CategoryEncoding.OneHot:
                    return OneHotEncodeColumn(df, colName, encodedOnly);
                case CategoryEncoding.Dummy :
                    return DummyEncodeColumn(df, colName, encodedOnly);
            }

            throw new NotImplementedException();
        }

        private static (DataFrame, string[]) OneHotEncodeColumn(this DataFrame df, string colName, bool encodedOnly = false)
        {
            var colVector = df[colName];
            var classValues = colVector.Where(x=> DataFrame.NAN != x).Select(x => x.ToString()).Distinct().ToArray();
            
            //define encoded columns
            var dict = new Dictionary<string, List<object>>();

            //add one-hot encoded columns
            foreach (var c in classValues)
                dict.Add(c, new List<object>());

            //encode values
            foreach (var cValue in colVector)
            {
                for (int i = 0; i < classValues.Length; i++)
                {
                    if (cValue.ToString() == classValues[i])
                        dict[classValues[i]].Add((int)1);
                    else
                        dict[classValues[i]].Add((int)0);
                }

            }
            if (encodedOnly)
            {
                var newDf = new DataFrame(dict);
                return (newDf, classValues);
            }
            else
            {
                var newDf = df.AddColumns(dict);
                return (newDf, classValues);
            }

        }

        private static (DataFrame, string[]) DummyEncodeColumn(this DataFrame df, string colName, bool encodedOnly = false)
        {
            var colVector = df[colName];
            var classValues = colVector.Where(x => DataFrame.NAN != x).Select(x => x.ToString()).Distinct().ToArray();

            //define encoded columns
            var dict = new Dictionary<string, List<object>>();
            var dummyClasses = classValues.SkipLast(1);
            //add dummy encoded columns
            foreach (var c in dummyClasses)
                dict.Add(c, new List<object>());

            //encode values
            foreach (var cValue in colVector)
            {
                for (int i = 0; i < dummyClasses.Count(); i++)
                {
                    if (cValue.ToString() == classValues[i])
                        dict[classValues[i]].Add((int)1);
                    else
                        dict[classValues[i]].Add((int)0);
                }

            }
            if (encodedOnly)
            {
                var newDf = new DataFrame(dict);
                return (newDf, classValues);
            }
            else
            {
                var newDf = df.AddColumns(dict);
                return (newDf, classValues);
            }

        }

        private static (DataFrame, string[]) OrdinalEncoding(this DataFrame df, string colName, bool encodedOnly = false)
        {
            var colVector = df[colName];

            var classValues = colVector.Where(x => DataFrame.NAN != x).Select(x => x.ToString()).Distinct().ToList();

            //define encoded columns
            var dict = new Dictionary<string, List<object>>();
            var encodedValues = new List<object>();
            foreach(var value in colVector)
            {
                int ordinalValue = classValues.IndexOf(value.ToString())+1;
                encodedValues.Add(ordinalValue);
            }
            
            //
            dict.Add(colName + "_cvalues", encodedValues);
            var newDf = df.AddColumns(dict);

            if (encodedOnly)
                return (newDf[new string[] { colName + "_cvalues" }],classValues.ToArray());
            else
                return (newDf, classValues.ToArray());

        }

        private static (DataFrame, string[]) BinaryEncoding(this DataFrame df, string colName, bool encodedOnly = false)
        {
            var colVector = df[colName];

            var classValues = colVector.Where(x => DataFrame.NAN != x).Select(x => x.ToString()).Distinct().ToList();

            //define encoded columns
            var dict = new Dictionary<string, List<object>>();
            var encodedValues = new List<object>();
            foreach (var value in colVector)
            {
                int ordinalValue = classValues.IndexOf(value.ToString());
                encodedValues.Add(ordinalValue);
            }

            //
            dict.Add(colName + "_cvalues", encodedValues);
            var newDf = df.AddColumns(dict);

            if (encodedOnly)
                return (newDf[new string[] { colName + "_cvalues" }], classValues.ToArray());
            else
                return (newDf, classValues.ToArray());

        }

        private static (DataFrame, string[]) BinaryEncoding2(this DataFrame df, string colName, bool encodedOnly = false)
        {
            var colVector = df[colName];

            var classValues = colVector.Where(x => DataFrame.NAN != x).Select(x => x.ToString()).Distinct().ToList();

            //define encoded columns
            var dict = new Dictionary<string, List<object>>();
            var encodedValues = new List<object>();
            foreach (var value in colVector)
            {
                int ordinalValue = classValues.IndexOf(value.ToString());
                if (ordinalValue == 0)
                    ordinalValue = -1;
                encodedValues.Add(ordinalValue);
            }

            //
            dict.Add(colName + "_cvalues", encodedValues);
            var newDf = df.AddColumns(dict);

            if (encodedOnly)
                return (newDf[new string[] { colName + "_cvalues" }], classValues.ToArray());
            else
                return (newDf, classValues.ToArray());

        }

    }
}
