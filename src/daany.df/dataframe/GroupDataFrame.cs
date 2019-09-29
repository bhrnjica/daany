//////////////////////////////////////////////////////////////////////////////////////////
// Daany - DAta ANalYtic Library                                                        //
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
using System.Linq;
using System.Collections.Generic;
using System;

namespace Daany
{

    public class TwoKeysDictionary<K1, K2, T> : Dictionary<K1, Dictionary<K2, T>>
    {
        public T this[K1 key1, K2 key2]
        {
            get => base.ContainsKey(key1) && base[key1].ContainsKey(key2) ? base[key1][key2] : default;
            set
            {
                if (ContainsKey(key1) && base[key1].ContainsKey(key2))
                    base[key1][key2] = value;
                else
                    Add(key1, key2, value);
            }
        }

        public void Add(K1 key1, K2 key2, T value)
        {
            if (ContainsKey(key1))
            {
                if (base[key1].ContainsKey(key2))
                    throw new Exception("Couple " + key1 + "/" + key2 + " already exists!");
                base[key1].Add(key2, value);
            }
            else
                Add(key1, new Dictionary<K2, T>() { { key2, value } });
        }

        public bool ContainsKey(K1 key1, K2 key2) => ContainsKey(key1) && base[key1].ContainsKey(key2);
    }

    public class GroupDataFrame
    {
        public GroupDataFrame(string colName, Dictionary<object, DataFrame> grp)
        {
            GroupedColumn = colName;
            Group = grp;
        }

        public GroupDataFrame(string colName1, string colName2, TwoKeysDictionary<object, object, DataFrame> grp)
        {
            GroupedColumn = colName1;
            GroupedColumn2 = colName2;
            Groups = grp;
        }
       

        public string GroupedColumn { get; set; }
        public string GroupedColumn2 { get; set; }

        public Dictionary<object, DataFrame> Group { get; set; }
        public TwoKeysDictionary<object, object, DataFrame> Groups { get; set; }

        public DataFrame this[object Key]
        {
            get
            {
                return Group[Key];
            }
        }

        public DataFrame this[object Key1, object Key2]
        {
            get
            {
                return Groups[Key1,Key2];
            }
        }

        public List<object> Keys
        {
            get
            {
                return Group.Keys.ToList();
            }
        }

        public List<(object key1,object key2)> Keys2
        {
            get
            {
                var lst = new List<(object key1, object key2)>();
                foreach(var kk in Groups)
                {
                    var k1 = kk.Key;
                    foreach(var kkk in kk.Value)
                    {
                        var k2 = kkk.Key;
                        lst.Add((k1, k2));
                    }
                }

                return lst;
            }
        }

        public DataFrame Rolling(int rollingWindow, int window, Dictionary<string, Aggregation> agg)
        {
            DataFrame df = null;
            foreach (var gr in Group)
            {
                var df1 = gr.Value.Rolling(GroupedColumn, rollingWindow, agg).TakeEvery(window);
                if (df == null)
                    df = new DataFrame(df1);
                else
                    df.AddRows(df1);
            }

            return df;
        }

        public DataFrame Aggregation(Aggregation agg)
        {
            DataFrame df = null;
            if (Group == null && Groups == null)
                throw new Exception("Group is  empty.");

            //grouping with one column
            if(Group!=null && Group.Count>0)
            {
                var df1 = DataFrame.CreateEmpty(Group.ElementAt(0).Value.Columns);
                foreach (var gr in Group)
                {
                    var lst = new List<object>();
                    lst.Add(GroupedColumn);
                    var row = gr.Value.Aggregations(lst,agg);
                    df1.AddRow(row);
                }

                return df1;
            }
            //grouping with two columns
            else if(Groups!=null && Groups.Count>0)
            {
                var df1 = DataFrame.CreateEmpty(Groups.ElementAt(0).Value.ElementAt(0).Value.Columns);
                foreach (var gr in Groups)
                {
                    var lst = new List<object>();
                    lst.Add(GroupedColumn);
                    lst.Add(GroupedColumn2);
                    foreach(var g2 in gr.Value)
                    {
                        var row = g2.Value.Aggregations(lst, agg);
                        df1.AddRow(row);
                    }
                   
                }
                return df1;
            }
            return df;
        }
    }
}
