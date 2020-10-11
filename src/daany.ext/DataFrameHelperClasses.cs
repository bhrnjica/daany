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
using System.ComponentModel;

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

    //Types of categorical column  encoding 
    public enum CategoryEncoding
    {
        [Description("None")] //no encoding
        None,
        [Description("(0,1)")] //binary encoding with 0 and 1
        Binary1,
        [Description("(-1,1)")] //binary encoding with 0 and 1
        Binary2,
        [Description("N")]//for (4 classes, one column) =1,2,3,4 
        Ordinal,
        [Description("1:N")]//one hot vector (4 categories, 4 columns) = (1,0,0);(0,1,0);(0,0,1);
        OneHot,
        [Description("1:N-1(0)")] //category encoding (4 categories, 3 columns) = (1,0,0);(0,1,0);(0,0,1);(0,0,0)
        Dummy,
        //[Description("1:N-1(-1)")] //category encoding (4 categories) = (1,0,0);(0,1,0);(0,0,1);(-1,-1,-1)
        //Dummy2,

    }
}
