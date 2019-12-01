﻿//////////////////////////////////////////////////////////////////////////////////////////
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

namespace Daany
{
    public class SortDataFrame
    {
        ColType[] m_dfTypes;
        int m_ColCount;
        public SortDataFrame(int[] indCols, ColType[] dfTypes)
        {
            m_dfTypes = dfTypes;
            m_ColCount = dfTypes.Length;
        }

        internal (List<object> lst, List<object> ind) QuickSort(IList<object> array, List<object> dfIndex, int[] indCols)
        {
            if (array == null)
                throw new Exception("data array cannot be null.");

            int end = array.Count / m_ColCount - 1;
            var sortedList = array.ToList();
            var sortedIndex = dfIndex.ToList();
            quickSort(sortedList, sortedIndex, 0, end, indCols);
            return (sortedList, sortedIndex);
        }

        internal (List<object> val, List<object> ind) MergeSort(object[] array, object[] index, int[] indCols)
        {
            (object[] val, object[] ind)= mergeSort(array, index, indCols);

            return (val.ToList(), ind.ToList());
        }
        
        #region QuickSort
        /// <summary>
        /// Classic QuickSort algorithm for sorting 
        /// </summary>
        /// <param name="init"></param>
        /// <param name="end"></param>
        /// <param name="cols"></param>
        private void quickSort(List<object> sortedList, List<object> sortedIndex, int init, int end, int[] indCols)
        {
            if (init < end)
            {
                int pivot = partition(sortedList, sortedIndex, init, end, indCols);
                quickSort(sortedList, sortedIndex, init, pivot - 1, indCols);
                quickSort(sortedList, sortedIndex, pivot + 1, end, indCols);
            }
        }

        //O(n)
        private int partition(List<object> sortedList, List<object> sortedIndex, int init, int end, int[] indCols)
        {
            var last = getRowFromList(sortedList, end);
            int i = init - 1;
            for (int j = init; j < end; j++)
            {
                var row = getRowFromList(sortedList, j);
                if (lessThanOrEqual(row, last, indCols))
                {
                    i++;
                    swap(sortedList, sortedIndex, i, j);
                }
            }
            swap(sortedList,sortedIndex, i + 1, end);
            return i + 1;
        }

        private IEnumerable<object> getRowFromList(IList<object> list, int jthRow)
        {
            var start = jthRow * m_ColCount;
            for (int i = start; i < start + m_ColCount; i++)
                yield return list[i];
        }

        private void swap(List<object> sortedList, List<object> sortedIndex, int i1, int i2)
        {
            if (i1 == i2)
                return;
            //
            for (int i = 0; i < m_ColCount; i++)
            {
                int lstIndex1 = i1 * m_ColCount + i;
                int lstIndex2 = i2 * m_ColCount + i;
                var temVal = sortedList[lstIndex1];
                sortedList[lstIndex1] = sortedList[lstIndex2];
                sortedList[lstIndex2] = temVal;
            }

            var temp = sortedIndex[i1];
            sortedIndex[i1] = sortedIndex[i2];
            sortedIndex[i2] = temp;
        }

        private bool lessThanOrEqual(IEnumerable<object> left, IEnumerable<object> right, int[] indCols, int leftIndex = 0, int rightIndex = 0)
        {
            bool prevEqual = true;
            //
            for (int i = 0; i < indCols.Length; i++)
            {
                if (!prevEqual)
                    return true;
                int colInd = indCols[i];
                var l = left.ElementAt(leftIndex + colInd);
                var r = right.ElementAt(rightIndex + colInd);

                //
                if (m_dfTypes[colInd] == ColType.STR)
                {
                   
                    var retVal = string.Compare(l.ToString(), r.ToString());

                    if (retVal < 0)
                        return false;
                    else if (retVal > 0)
                        return true;
                    else 
                        prevEqual = true;
                }
                //
                else if (m_dfTypes[colInd] == ColType.I32 || m_dfTypes[colInd] == ColType.IN)
                {
                    var ll = Convert.ToInt32(l);
                    var rr = Convert.ToInt32(r);

                    if (ll > rr)
                        return false;
                    else if (ll < rr)
                        return true;
                    else 
                        prevEqual = true;

                }
                else if (m_dfTypes[colInd] == ColType.I64)
                {
                    var ll = Convert.ToInt64(l);
                    var rr = Convert.ToInt64(r);

                    if (ll > rr)
                        return false;
                    else if (ll < rr)
                        return true;
                    else
                        prevEqual = true;
                }
                else if (m_dfTypes[colInd] == ColType.DD)
                {
                    var ll = Convert.ToDouble(l);
                    var rr = Convert.ToDouble(r);

                    if (ll > rr)
                        return false;
                    else if (ll < rr)
                        return true;
                    else
                        prevEqual = true;
                }
                else if (m_dfTypes[colInd] == ColType.F32)
                {
                    var ll = Convert.ToSingle(l);
                    var rr = Convert.ToSingle(r);

                    if (ll > rr)
                        return false;
                    else if (ll < rr)
                        return true;
                    else
                        prevEqual = true;
                }
                else if (m_dfTypes[colInd] == ColType.DT)
                {
                    var ll = Convert.ToDateTime(l);
                    var rr = Convert.ToDateTime(r);

                    if (ll > rr)
                        return false;
                    else if (ll < rr)
                        return true;
                    else
                        prevEqual = true;
                }
                else
                    throw new Exception("Sorting is not supported");
            }

            return true;
        }

        #endregion

        #region MergeSort
        private (object[] val, object[] ind) mergeSort(object[] array, object [] index,  int[] indCols)
        {
            object[] left, leftInd;
            object[] right, rightInd;

            object[] result = new object[array.Length];
            object[] resultInd = new object[index.Length];

            //As this is a recursive algorithm, we need to have a base case to 
            //avoid an infinite recursion and therefore a stack overflow
            if (array.Length <= m_ColCount)
                return (array, index);

            // The exact midpoint of our array 
            int rowCount = array.Length / m_ColCount;
            int midPoint = rowCount/ 2;

            //Will represent our 'left' array
            left = new object[midPoint * m_ColCount];
            leftInd = index.Take(midPoint).ToArray();

            //if array has an even number of elements, the left and right array will have the same number of 
            //elements
            if (rowCount % 2 == 0)
                right = new object[midPoint * m_ColCount];
            //if array has an odd number of elements, the right array will have one more element than left
            else
                right = new object[(midPoint +1) * m_ColCount];
            

            //populate left array
            for (int i = 0; i < midPoint * m_ColCount; i++)
                left[i] = array[i];

            //populate right index
            rightInd = index.Skip(midPoint).ToArray();

            //We start our index from the midpoint, as we have already populated the left array from 0 to midpont
            int k = 0;
            for (int i = midPoint * m_ColCount; i < array.Length; i++)
            {
                right[k]= array[i];
                k++;
            }

            //Recursively sort the left array
            (left, leftInd) = mergeSort(left, leftInd, indCols);

            //Recursively sort the right array
            (right, rightInd) = mergeSort(right,rightInd, indCols);

            //Merge our two sorted arrays
            (result, resultInd) = merge(left, right, leftInd, rightInd, indCols);

            return (result, resultInd);
        }

        //This method will be responsible for combining our two sorted arrays into one giant array
        private (object[] val, object[] ind) merge(object[] left, object[] right, object[] leftInd, object[] rightInd, int[] indCols)
        {
            int resultLength = right.Length + left.Length;
            object[] result = new object[resultLength];
            object[] resultInd = new object[leftInd.Length + rightInd.Length];
            //
            int indexLeft = 0, indexRight = 0, indexResult = 0;
            int leftCount = left.Length; int rightCount = right.Length;
            
            //while either array still has an element
            while (indexLeft < leftCount  || indexRight < rightCount)
            {
                //if both arrays have elements  
                if (indexLeft < leftCount && indexRight < rightCount)
                {
                    //If item on left array is less than item on right array, add that item to the result array 
                    if(lessThanOrEqual(left, right, indCols, indexLeft, indexRight))
                    {
                        //populate values
                        for (int i = 0; i < m_ColCount; i++)
                            result[indexResult + i] = left[indexLeft + i];

                        //populate index
                        resultInd[indexResult / m_ColCount] = leftInd[indexLeft / m_ColCount];

                        indexLeft += m_ColCount;
                        indexResult += m_ColCount;
                    }
                    // else the item in the right array will be added to the results array
                    else
                    {
                        //populate values
                        for (int i = 0; i < m_ColCount; i++)
                            result[indexResult + i] = right[indexRight + i];

                        //populate index
                        resultInd[indexResult / m_ColCount] = rightInd[indexRight / m_ColCount];

                        indexRight += m_ColCount;
                        indexResult += m_ColCount;
                    }
                }
                //if only the left array still has elements, add all its items to the results array
                else if (indexLeft < left.Length)
                {
                    for (int i = 0; i < m_ColCount; i++)
                        result[indexResult + i] = left[indexLeft + i];

                    //populate index
                    resultInd[indexResult / m_ColCount] = leftInd[indexLeft / m_ColCount];

                    indexLeft += m_ColCount;
                    indexResult += m_ColCount;
                }
                //if only the right array still has elements, add all its items to the results array
                else if (indexRight < right.Length)
                {
                    for(int i=0; i< m_ColCount; i++)
                        result[indexResult + i] = right[indexRight + i];

                    //populate index
                    resultInd[indexResult/m_ColCount] = rightInd[indexRight/m_ColCount];

                    indexRight += m_ColCount;
                    indexResult += m_ColCount;
                }
                //increase 

            }
            return (result, resultInd);
        }

        #endregion
    }
     
}
