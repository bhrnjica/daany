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
