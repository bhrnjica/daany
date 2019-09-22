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