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

        public List<object> QuickSort(List<object> array, int[] indCols)
        {
            int end = array.Count / m_ColCount - 1;
            var sortedList = array.ToList();
            quickSort(sortedList, 0, end, indCols);
            return sortedList;
        }

        public List<object> MergeSort(List<object> array, int[] indCols)
        {
            var sortedList = mergeSort(array.ToArray(), indCols);
            return sortedList.ToList();
        }


        #region QuickSort
        /// <summary>
        /// Classic QuickSort algorithm for sorting 
        /// </summary>
        /// <param name="init"></param>
        /// <param name="end"></param>
        /// <param name="cols"></param>
        private void quickSort(List<object> sortedList, int init, int end, int[] indCols)
        {
            if (init < end)
            {
                int pivot = partition(sortedList, init, end, indCols);
                quickSort(sortedList, init, pivot - 1, indCols);
                quickSort(sortedList, pivot + 1, end, indCols);
            }
        }

        //O(n)
        private int partition(List<object> sortedList, int init, int end, int[] indCols)
        {
            var last = getRowFromList(sortedList, end);
            int i = init - 1;
            for (int j = init; j < end; j++)
            {
                var row = getRowFromList(sortedList, j);
                if (lessThanOrEqual(row, last, indCols))
                {
                    i++;
                    swap(sortedList, i, j);
                }
            }
            swap(sortedList, i + 1, end);
            return i + 1;
        }

        private IEnumerable<object> getRowFromList(IList<object> list, int jthRow)
        {
            var start = jthRow * m_ColCount;
            for (int i = start; i < start + m_ColCount; i++)
                yield return list[i];
        }

        private void swap(List<object> sortedList, int i1, int i2)
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
                    if (retVal != -1)
                        return false;

                    if (retVal == 0)
                        prevEqual = true;
                    else
                        prevEqual = false;
                }
                //
                else if (m_dfTypes[colInd] == ColType.I32)
                {
                    var ll = (int)l;
                    var rr = (int)r;

                    if (ll > rr)
                        return false;

                    if (ll == rr)
                        prevEqual = true;
                    else
                        prevEqual = false;
                }
                else if (m_dfTypes[colInd] == ColType.I64)
                {
                    var ll = (long)l;
                    var rr = (long)r;

                    if (ll > rr)
                        return false;

                    if (ll == rr)
                        prevEqual = true;
                    else
                        prevEqual = false;
                }
                else if (m_dfTypes[colInd] == ColType.DT)
                {
                    var ll = (DateTime)l;
                    var rr = (DateTime)r;

                    if (ll > rr)
                        return false;

                    if (ll == rr)
                        prevEqual = true;
                    else
                        prevEqual = false;
                }
                else
                    throw new Exception("Sorting is not supported");
            }

            return true;
        }

        #endregion

        #region MergeSort
        private object[] mergeSort(object[] array, int[] indCols)
        {
            object[] left;
            object[] right;
            object[] result = new object[array.Length];

            //As this is a recursive algorithm, we need to have a base case to 
            //avoid an infinite recursion and therefore a stack overflow
            if (array.Length <= m_ColCount)
                return array;

            // The exact midpoint of our array 
            int rowCount = array.Length / m_ColCount;
            int midPoint = rowCount/ 2;

            //Will represent our 'left' array
            left = new object[midPoint * m_ColCount];

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

            //We start our index from the midpoint, as we have already populated the left array from 0 to midpont
            int k = 0;
            for (int i = midPoint * m_ColCount; i < array.Length; i++)
            {
                right[k]= array[i];
                k++;
            }

            //Recursively sort the left array
            left = mergeSort(left, indCols);

            //Recursively sort the right array
            right = mergeSort(right, indCols);

            //Merge our two sorted arrays
            result = merge(left, right, indCols);

            return result;
        }

        //This method will be responsible for combining our two sorted arrays into one giant array
        private object[] merge(object[] left, object[] right, int[] indCols)
        {
            int resultLength = right.Length + left.Length;
            object[] result = new object[resultLength];

            //
            int indexLeft = 0, indexRight = 0, indexResult = 0;
            int leftCount = left.Length ; int rightCount = right.Length;
            //while either array still has an element
            while (indexLeft < leftCount  || indexRight < rightCount)
            {
                //if both arrays have elements  
                if (indexLeft < leftCount && indexRight < rightCount)
                {
                    //If item on left array is less than item on right array, add that item to the result array 
                    //if (left[indexLeft] <= right[indexRight])
                    if(lessThanOrEqual(left, right, indCols, indexLeft, indexRight))
                    {
                        for (int i = 0; i < m_ColCount; i++)
                            result[indexResult + i] = left[indexLeft + i];

                        indexLeft += m_ColCount;
                        indexResult += m_ColCount;
                    }
                    // else the item in the right array will be added to the results array
                    else
                    {
                        for (int i = 0; i < m_ColCount; i++)
                            result[indexResult + i] = right[indexRight + i];

                        indexRight += m_ColCount;
                        indexResult += m_ColCount;
                    }
                }
                //if only the left array still has elements, add all its items to the results array
                else if (indexLeft < left.Length)
                {
                    for (int i = 0; i < m_ColCount; i++)
                        result[indexResult + i] = left[indexLeft + i];

                    indexLeft += m_ColCount;
                    indexResult += m_ColCount;
                }
                //if only the right array still has elements, add all its items to the results array
                else if (indexRight < right.Length)
                {
                    for(int i=0; i< m_ColCount; i++)
                        result[indexResult + i] = right[indexRight + i];

                    indexRight += m_ColCount;
                    indexResult += m_ColCount;
                }
                //increase 
            }
            return result;
        }

        #endregion
    }
     
}
