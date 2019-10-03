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
namespace Daany
{
    public enum FilterOperator
    {
        Equal,
        Notequal,
        Greather,
        Less,
        GreatherOrEqual,
        LessOrEqual,
        IsNUll,
        NonNull
    }
    public enum SortOrder
    {
        Asc,
        Desc,
    }
    public enum JoinType
    {
        Inner,
        Left,
    }
    public enum ColType
    {
        I2,//bool
        I32,//int
        I64,//long
        F32,//float
        DD,//double
        STR,//string
        DT,//datetime
    }
    public enum Aggregation
    {
        First,
        Last,
        Count,
        Sum,
        Avg,
        Min,
        Max,
        Std,
    }
}