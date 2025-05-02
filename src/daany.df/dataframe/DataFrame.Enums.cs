//////////////////////////////////////////////////////////////////////////////
//   ____    _    _   _   _   __  __                                       //
//  |  _ \  / \  | \ | | | \ | |\ \/ /                                     //
//  | | | |/ _ \ |  \| | |  \| | \  /                                      //
//  | |_| / ___ \| |\  | | |\  | | |                                       //
//  |____/_/   \_\_| \_| |_| \_| |_|                                       //
//                                                                         //
//  DAata ANalYtics Library                                                //
//  Daany.DataFrame:Implementation of DataFrame.                           //
//  https://github.com/bhrnjica/daany                                      //
//                                                                         //
//  Copyright © 20019-2025 Bahrudin Hrnjica                                //
//                                                                         //
//  Free. Open Source. MIT Licensed.                                       //
//  https://github.com/bhrnjica/daany/blob/master/LICENSE                  //
//////////////////////////////////////////////////////////////////////////////
using System.ComponentModel;

namespace Daany
{
    public enum DiffType
    {
        Seasonal,
        Recurrsive
    }
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
        IN,//categorical type.
        I32,//int
        I64,//long
        F32,//float
        DD,//double
        STR,//string
        DT,//datetime
    }
    public enum Aggregation
    {
        None,
        First,
        Last,
        Count,
        Sum,
        [Description("Mean")]
        Avg,
        Min,
        Max,
        Std,
        Unique,
        Top,
        Random,
        Mode,
        Median,
        [Description("25%")]
        FirstQuartile,
        [Description("75%")]
        ThirdQuartile,
        [Description("Freq")]
        Frequency
    }

    //Types of categorical column  encoding 
    public enum ColumnTransformer
    {
        [Description("None")] //no encoding
        None=1,
        [Description("(0,1)")] //binary encoding with 0 and 1
        Binary1,
        [Description("(-1,1)")] //binary encoding with -1 and 1
        Binary2,
        [Description("N")]//for (4 classes, one column) =1,2,3,4 
        Ordinal,
        [Description("1:N")]//one hot vector (4 categories, 4 columns) = (1,0,0,0);(0,1,0,0);(0,0,1,0);(0,0,0,1);
        OneHot,
        [Description("1:N-1(0)")] //category encoding (4 categories, 3 columns) = (1,0,0);(0,1,0);(0,0,1);(0,0,0)
        Dummy,
        //[Description("1:N-1(-1)")] //category encoding (4 categories) = (1,0,0);(0,1,0);(0,0,1);(-1,-1,-1)
        //Dummy2,
        [Description("Min-Max numeric data normalization.")]//transform data to achieve min=0 and max=1
        MinMax,
        [Description("z- Score numeric data standardization.")]//transform data to achive avg=0, std=1
        Standardizer

    }
    
    public enum ValueType
    {
        None,
        Int,
        Float,
    }
}
