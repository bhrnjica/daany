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
using System;
using System.Collections.Generic;
using System.Linq;

namespace Daany.Ext;

public static class DataFrameColumnTransformer
{
	/// <summary>
	/// Transforms a column in the DataFrame using the specified transformer.
	/// </summary>
	/// <param name="df">The DataFrame to transform</param>
	/// <param name="colName">Name of the column to transform</param>
	/// <param name="transformer">Type of transformation to apply</param>
	/// <param name="transformedColumnsOnly">If true, returns only transformed columns</param>
	/// <returns>
	/// Tuple containing:
	/// - Transformed DataFrame
	/// - Array of scaling parameters (for scaling transformations)
	/// - Array of label values (for encoding transformations)
	/// </returns>
	public static (DataFrame df, float[] scaledValues, string[] labels) TransformColumn(this DataFrame df, string colName,
   ColumnTransformer transformer, bool transformedColumnsOnly = false)
	{
		return transformer switch
		{
			ColumnTransformer.None => (df, null, null)!,
			ColumnTransformer.Binary1 => BinaryEncoding(df, colName, transformedColumnsOnly) switch
			{
				var (edf, cValues) => (edf, null, cValues)!
			},
			ColumnTransformer.Binary2 => BinaryEncoding2(df, colName, transformedColumnsOnly) switch
			{
				var (edf, cValues) => (edf, null, cValues)!
			},
			ColumnTransformer.Ordinal => OrdinalEncoding(df, colName, transformedColumnsOnly) switch
			{
				var (edf, cValues) => (edf, null, cValues)!
			},
			ColumnTransformer.OneHot => OneHotEncodeColumn(df, colName, transformedColumnsOnly) switch
			{
				var (edf, cValues) => (edf, null, cValues)!
			},
			ColumnTransformer.Dummy => DummyEncodeColumn(df, colName, transformedColumnsOnly) switch
			{
				var (edf, cValues) => (edf, null, cValues)!
			},
			ColumnTransformer.MinMax or ColumnTransformer.Standardizer => ScaleColumn(df, colName, transformer, transformedColumnsOnly) switch
			{
				var (tdf, fValues) => (tdf, fValues, null)!
			},
			_ => throw new NotSupportedException("Data normalization is not supported.")
		};
	}

	/// <summary>
	/// Scales a column using either MinMax or Standard (Z-score) scaling
	/// </summary>
	/// <param name="dff">Input DataFrame</param>
	/// <param name="colName">Column name to scale</param>
	/// <param name="transformer">Scaling type (MinMax or Standardizer)</param>
	/// <param name="transformedColumnsOnly">If true, returns only the scaled column</param>
	/// <returns>Tuple of transformed DataFrame and scaling parameters</returns>
	private static (DataFrame edf, float[] fValues) ScaleColumn(DataFrame dff, string colName, ColumnTransformer transformer, bool transformedColumnsOnly)
    {

		var newColName = $"{colName}_scaled";
		var df = dff[dff.Columns.ToArray()];
		var s = Series.FromDataFrame(df, colName);

		float param1, param2;

		if (transformer == ColumnTransformer.MinMax)
		{
			var minObj = s.Aggregate(Aggregation.Min);
			param1 = Convert.ToSingle(minObj);
			var maxObj = s.Aggregate(Aggregation.Max);
			param2 = Convert.ToSingle(maxObj);

			df.AddCalculatedColumn(newColName, (IDictionary<string, object?> row, int i) =>
			{
				var val = Convert.ToDouble(row[colName]);
				return (val - param1) / (param2 - param1);
			});
		}
		else if (transformer == ColumnTransformer.Standardizer)
		{
			var avgObj = s.Aggregate(Aggregation.Avg);
			param1 = Convert.ToSingle(avgObj);
			var stdObj = s.Aggregate(Aggregation.Std);
			param2 = Convert.ToSingle(stdObj);

			df.AddCalculatedColumn(newColName, (IDictionary<string, object?> row, int i) =>
			{
				var val = Convert.ToDouble(row[colName]);
				return (val - param1) / param2;
			});
		}
		else
		{
			throw new NotSupportedException("Column transformation is not supported.");
		}

		if (transformedColumnsOnly)
		{
			var ddf = df.Create((newColName, null)!);
			return (ddf, new[] { param1, param2 });
		}

		return (df, new[] { param1, param2 });
	}

	/// <summary>
	/// Performs one-hot encoding on a categorical column
	/// </summary>
	/// <param name="df">Input DataFrame</param>
	/// <param name="colName">Column name to encode</param>
	/// <param name="encodedOnly">If true, returns only encoded columns</param>
	/// <returns>Tuple of transformed DataFrame and unique class values</returns>
	private static (DataFrame, string[]) OneHotEncodeColumn(this DataFrame df, string colName, bool encodedOnly = false)
    {
		var colVector = df[colName] ?? throw new ArgumentException($"Column '{colName}' not found");

		var classValues = colVector
			.Where(x => x is not null)
			.Select(x => x!.ToString() ?? string.Empty) 
			.Distinct()
			.ToArray();

		var dict = new Dictionary<string, List<object?>>(classValues.Length);

		// Initialize lists with capacity to avoid resizing
		foreach (var c in classValues)
		{
			dict[c!] = new List<object?>(colVector.Count());
		}

		// Encode values
		foreach (var cValue in colVector)
		{
			var strValue = cValue!.ToString();

			foreach (var cls in classValues)
			{

				dict[cls].Add(strValue == cls ? 1 : 0);
			}
		}

		var newDf = encodedOnly ? new DataFrame(dict) : df.AddColumns(dict);

		return (newDf, classValues);

	}

	/// <summary>
	/// Performs dummy encoding on a categorical column (one less column than one-hot)
	/// </summary>
	/// <param name="df">Input DataFrame</param>
	/// <param name="colName">Column name to encode</param>
	/// <param name="encodedOnly">If true, returns only encoded columns</param>
	/// <returns>Tuple of transformed DataFrame and unique class values</returns>
	private static (DataFrame, string?[]) DummyEncodeColumn(this DataFrame df, string colName, bool encodedOnly = false)
    {
		var colVector = df[colName] ?? throw new ArgumentException($"Column '{colName}' not found");
		var classValues = colVector
			.Where(x => DataFrame.NAN != x)
			.Select(x => x as string)
			.Distinct()
			.ToArray();

		var dummyClasses = classValues.Take(classValues.Length - 1).ToArray();

		var dict = new Dictionary<string, List<object?>>(dummyClasses.Length);

		// Initialize lists with capacity
		foreach (var c in dummyClasses)
		{
			dict[c!] = new List<object?>(colVector.Count());
		}

		// Encode values
		foreach (var cValue in colVector)
		{
			var strValue = cValue as string;
			for (int i = 0; i < dummyClasses.Length; i++)
			{
				if(dummyClasses[i] is not null)
					dict[dummyClasses[i]!].Add(strValue == classValues[i] ? 1 : 0);
			}
		}

		var newDf = encodedOnly ? new DataFrame(dict) : df.AddColumns(dict);
		return (newDf, classValues);

	}

	/// <summary>
	/// Ordinal encoding of classification data. First value start from 0. So in case of example:
	/// var colors = new string[] { "red", "green", "blue", "green", "red" }; endoded values are:
	/// var colors = new int { 0, 1, 2, 1, 0 };
	/// </summary>
	/// <param name="df">Input DataFrame</param>
	/// <param name="colName">Column name to encode</param>
	/// <param name="encodedOnly">If true, returns only encoded column</param>
	/// <returns>Tuple of transformed DataFrame and unique class values</returns>
	private static (DataFrame, string[]) OrdinalEncoding(this DataFrame df, string colName, bool encodedOnly = false)
    {
        var colVector = df[colName];

        var classValues = colVector.Where(x => x != null).Select(x => x!.ToString()).Distinct().ToList();

        //define encoded columns
        var dict = new Dictionary<string, List<object?>>();
        var encodedValues = new List<object?>();
        foreach(var value in colVector)
        {
			if (value is not string)
				continue;

            int ordinalValue = classValues.IndexOf(value.ToString());
            encodedValues.Add(ordinalValue);
        }

		//
		var newClumn = colName + "_cvalues";

		dict.Add(newClumn, encodedValues);
        var newDf = df.AddColumns(dict);

		var classCols = classValues.Select(x => x!.ToString()).ToArray();

		var cols = new string[] { newClumn };

		if (encodedOnly)
            return (newDf[cols], classCols);
        else
            return (newDf, classCols);

    }

	/// <summary>
	/// Performs binary encoding (0/1) on a column
	/// </summary>
	/// <param name="df">Input DataFrame</param>
	/// <param name="colName">Column name to encode</param>
	/// <param name="encodedOnly">If true, returns only encoded column</param>
	/// <returns>Tuple of transformed DataFrame and unique class values</returns>
	private static (DataFrame, string[]) BinaryEncoding(this DataFrame df, string colName, bool encodedOnly = false)
    {
        var colVector = df[colName];

        var classValues = colVector.Where(x => DataFrame.NAN != x).Select(x => x!.ToString()).Distinct().ToList();

		if (classValues.Count != 2)
			throw new FormatException("The DataFrame column contains more than two categories");

		//define encoded columns
		var dict = new Dictionary<string, List<object?>>();
        var encodedValues = new List<object?>();
        foreach (var value in colVector)
        {
            if (value is bool)
            {
                int ordinalValue = Convert.ToInt16(value);
                encodedValues.Add(ordinalValue);
            }
            else
            {
                int ordinalValue = classValues.IndexOf(value!.ToString());
                encodedValues.Add(ordinalValue);
            }
        }

		var newClumn = colName + "_cvalues";

		dict.Add(newClumn, encodedValues);
        var newDf = df.AddColumns(dict);

        if (encodedOnly)
            return (newDf[new string[] { newClumn }], classValues.Select(x=>x!.ToString()).ToArray());
        else
            return (newDf, classValues.Select(x => x!.ToString()).ToArray());

    }

	/// <summary>
	/// Performs binary encoding (-1/1) on a column
	/// </summary>
	/// <param name="df">Input DataFrame</param>
	/// <param name="colName">Column name to encode</param>
	/// <param name="encodedOnly">If true, returns only encoded column</param>
	/// <returns>Tuple of transformed DataFrame and unique class values</returns>
	private static (DataFrame, string[]) BinaryEncoding2(this DataFrame df, string colName, bool encodedOnly = false)
    {
        var colVector = df[colName];

        var classValues = colVector.Where(x => DataFrame.NAN != x).Select(x => x!.ToString()).Distinct().ToList();
		if (classValues.Count != 2)
			throw new FormatException("The DataFrame column contains more than two categories");
        //define encoded columns
        var dict = new Dictionary<string, List<object?>>();
        var encodedValues = new List<object?>();
        foreach (var value in colVector)
        {
            if (value is bool)
            {
                int ordinalValue = Convert.ToInt16(value);
                if (ordinalValue == 0)
                    ordinalValue = -1;
                encodedValues.Add(ordinalValue);
            }
            else
            {
                int ordinalValue = classValues.IndexOf(value!.ToString());
                if (ordinalValue == 0)
                    ordinalValue = -1;
                encodedValues.Add(ordinalValue);
            }
        }

		var newClumn = colName + "_cvalues";
		dict.Add(newClumn, encodedValues);
        var newDf = df.AddColumns(dict);

        if (encodedOnly)
            return (newDf[new string[] { newClumn }], classValues.Select(x => x!.ToString()).ToArray());
        else
            return (newDf, classValues.Select(x => x!.ToString()).ToArray());

    }

}
