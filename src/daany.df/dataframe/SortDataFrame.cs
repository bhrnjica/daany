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
using System.Collections;

namespace Daany
{
	public class SortDataFrame
	{
		private readonly ColType[] _dfTypes;
		private readonly int _colCount;

		public SortDataFrame(ColType[] dfTypes)
		{
			_dfTypes = dfTypes ?? throw new ArgumentNullException(nameof(dfTypes));
			_colCount = dfTypes.Length;
		}

		/// <summary>
		/// Sorts the DataFrame using QuickSort algorithm.
		/// </summary>
		public (List<object> values, List<object> indices) QuickSort(IList<object> array, List<object> dfIndex, int[] sortCols)
		{
			ValidateInput(array, dfIndex);

			int end = array.Count / _colCount - 1;
			var sortedValues = array.ToList();
			var sortedIndices = dfIndex.ToList();

			QuickSortRecursive(sortedValues, sortedIndices, 0, end, sortCols);
			return (sortedValues, sortedIndices);
		}

		/// <summary>
		/// Sorts the DataFrame using MergeSort algorithm.
		/// </summary>
		public (List<object> values, List<object> indices) MergeSort(object[] array, object[] index, int[] sortCols)
		{
			ValidateInput(array, index);

			(object[] sortedValues, object[] sortedIndices) = MergeSortRecursive(array, index, sortCols);
			return (sortedValues.ToList(), sortedIndices.ToList());
		}

		private void ValidateInput(IList<object> array, IList<object> index)
		{
			if (array == null || index == null)
				throw new ArgumentException("Array or index cannot be null.");

			if (array.Count % _colCount != 0)
				throw new ArgumentException("Array length must be divisible by the column count.");

			if (array.Count / _colCount != index.Count)
				throw new ArgumentException("Row count in array and index must match.");
		}

		#region QuickSort Implementation
		private void QuickSortRecursive(List<object> values, List<object> indices, int start, int end, int[] sortCols)
		{
			if (start >= end)
				return;

			int pivot = Partition(values, indices, start, end, sortCols);
			QuickSortRecursive(values, indices, start, pivot - 1, sortCols);
			QuickSortRecursive(values, indices, pivot + 1, end, sortCols);
		}

		private int Partition(List<object> values, List<object> indices, int start, int end, int[] sortCols)
		{
			var pivotRow = GetRow(values, end);
			int i = start - 1;

			for (int j = start; j < end; j++)
			{
				var currentRow = GetRow(values, j);
				if (CompareRows(currentRow, pivotRow, sortCols) <= 0)
				{
					i++;
					SwapRows(values, indices, i, j);
				}
			}

			SwapRows(values, indices, i + 1, end);
			return i + 1;
		}
		#endregion

		#region MergeSort Implementation
		private (object[] values, object[] indices) MergeSortRecursive(object[] values, object[] indices, int[] sortCols)
		{
			int rowCount = values.Length / _colCount;
			if (rowCount <= 1)
				return (values, indices);

			int midPoint = rowCount / 2;

			// Split into left and right arrays
			var leftValues = values.Take(midPoint * _colCount).ToArray();
			var rightValues = values.Skip(midPoint * _colCount).ToArray();
			var leftIndices = indices.Take(midPoint).ToArray();
			var rightIndices = indices.Skip(midPoint).ToArray();

			// Recursively sort
			(leftValues, leftIndices) = MergeSortRecursive(leftValues, leftIndices, sortCols);
			(rightValues, rightIndices) = MergeSortRecursive(rightValues, rightIndices, sortCols);

			return Merge(leftValues, rightValues, leftIndices, rightIndices, sortCols);
		}

		private (object[] values, object[] indices) Merge(object[] leftValues, object[] rightValues, object[] leftIndices, object[] rightIndices, int[] sortCols)
		{
			int leftPointer = 0, rightPointer = 0, resultPointer = 0;
			int leftCount = leftValues.Length, rightCount = rightValues.Length;

			var resultValues = new object[leftCount + rightCount];
			var resultIndices = new object[leftIndices.Length + rightIndices.Length];

			while (leftPointer < leftCount && rightPointer < rightCount)
			{
				if (CompareRows(GetRow(leftValues, leftPointer / _colCount), GetRow(rightValues, rightPointer / _colCount), sortCols) <= 0)
				{
					CopyRow(leftValues, resultValues, leftPointer, resultPointer);
					resultIndices[resultPointer / _colCount] = leftIndices[leftPointer / _colCount];
					leftPointer += _colCount;
				}
				else
				{
					CopyRow(rightValues, resultValues, rightPointer, resultPointer);
					resultIndices[resultPointer / _colCount] = rightIndices[rightPointer / _colCount];
					rightPointer += _colCount;
				}
				resultPointer += _colCount;
			}

			while (leftPointer < leftCount)
			{
				CopyRow(leftValues, resultValues, leftPointer, resultPointer);
				resultIndices[resultPointer / _colCount] = leftIndices[leftPointer / _colCount];
				leftPointer += _colCount;
				resultPointer += _colCount;
			}

			while (rightPointer < rightCount)
			{
				CopyRow(rightValues, resultValues, rightPointer, resultPointer);
				resultIndices[resultPointer / _colCount] = rightIndices[rightPointer / _colCount];
				rightPointer += _colCount;
				resultPointer += _colCount;
			}

			return (resultValues, resultIndices);
		}
		#endregion

		#region Utility Methods
		private IEnumerable<object> GetRow(IList<object> values, int rowIndex)
		{
			int start = rowIndex * _colCount;
			for (int i = start; i < start + _colCount; i++)
				yield return values[i];
		}

		private void SwapRows(List<object> values, List<object> indices, int i1, int i2)
		{
			if (i1 == i2)
				return;

			for (int i = 0; i < _colCount; i++)
			{
				int index1 = i1 * _colCount + i;
				int index2 = i2 * _colCount + i;

				var temp = values[index1];
				values[index1] = values[index2];
				values[index2] = temp;
			}

			var tempIndex = indices[i1];
			indices[i1] = indices[i2];
			indices[i2] = tempIndex;
		}

		private void CopyRow(object[] source, object[] destination, int sourceIndex, int destinationIndex)
		{
			Array.Copy(source, sourceIndex, destination, destinationIndex, _colCount);
		}

		private int CompareRows(IEnumerable<object> leftRow, IEnumerable<object> rightRow, int[] sortCols)
		{
			foreach (var col in sortCols)
			{
				var leftValue = leftRow.ElementAt(col);
				var rightValue = rightRow.ElementAt(col);

				int comparison = Comparer.Default.Compare(leftValue, rightValue);
				if (comparison != 0)
					return comparison;
			}
			return 0;
		}
		#endregion
	}


}
