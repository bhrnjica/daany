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
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Daany.Binding
{
	[StructLayout(LayoutKind.Explicit)]
	internal struct CellValue
	{
		[FieldOffset(0)] public int intValue;
		[FieldOffset(0)] public long longValue;
		[FieldOffset(0)] public float floatValue;
		[FieldOffset(0)] public double doubleValue;
		[FieldOffset(0)] public IntPtr stringValue;
		[FieldOffset(0)] public long datetimeValue;
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct CellObject
	{
		public CellValue value;
		public int typeId;
	}

	internal static class DaanyRust
	{
		[DllImport("daany_rust_lib", CallingConvention = CallingConvention.Cdecl)]
		internal static extern void free_columns(IntPtr columnsPtr, ulong colCount);

		[DllImport("daany_rust_lib", CallingConvention = CallingConvention.Cdecl)]
		internal static extern void free_data(IntPtr dataPtr, ulong rowCount, ulong colCount);

		[DllImport("daany_rust_lib", CallingConvention = CallingConvention.Cdecl)]
		internal static extern void to_csv(
				IntPtr filePath,
				IntPtr data, int dataLength,
				IntPtr columns, int colLength,
				char separator, bool hasHeader,
				IntPtr dateFormat);

		[DllImport("daany_rust_lib", CallingConvention = CallingConvention.Cdecl)]
		internal static extern void from_csv(
				string filePath,
				char separator,
				string dateFormat,
				string missingValue,
				bool hasHeader,
				out IntPtr columnsOut,
				out ulong colCountOut,
				out IntPtr dataOut,
				out ulong rowCountOut);

		[DllImport("daany_rust_lib", CallingConvention = CallingConvention.Cdecl)]
		internal static extern double series_sum(IntPtr data, nuint length);

		[DllImport("daany_rust_lib", CallingConvention = CallingConvention.Cdecl)]
		internal static extern double series_mean(IntPtr data, nuint length);

		[DllImport("daany_rust_lib", CallingConvention = CallingConvention.Cdecl)]
		internal static extern double series_min(IntPtr data, nuint length);

		[DllImport("daany_rust_lib", CallingConvention = CallingConvention.Cdecl)]
		internal static extern double series_max(IntPtr data, nuint length);

		[DllImport("daany_rust_lib", CallingConvention = CallingConvention.Cdecl)]
		internal static extern double series_std(IntPtr data, nuint length);

		[DllImport("daany_rust_lib", CallingConvention = CallingConvention.Cdecl)]
		internal static extern double series_median(IntPtr data, nuint length);


		//Helpers
		internal static object[] exctractData(nint dataPtr, ulong rowCount, ulong colCount)
		{
			// Allocate an array to hold the CellObjects
			var data = new object[rowCount * colCount];
			// Correctly copy the raw memory into the managed struct array
			IntPtr currentPtr = dataPtr;
			for (int i = 0; i < data.Length; i++)
			{
				var cell = Marshal.PtrToStructure<CellObject>(currentPtr);
				currentPtr = IntPtr.Add(currentPtr, Marshal.SizeOf<CellObject>());

				if (cell.typeId == 0) data[i] = cell.value.intValue;
				else if (cell.typeId == 1) data[i] = cell.value.longValue;
				else if (cell.typeId == 2) data[i] = cell.value.floatValue;
				else if (cell.typeId == 3) data[i] = cell.value.doubleValue;
				else if (cell.typeId == 4)
				{
					data[i] = cell.value.stringValue != IntPtr.Zero
									? Marshal.PtrToStringAnsi(cell.value.stringValue)!
									: null!;
				}
				else if (cell.typeId == 5) data[i] = DateTimeOffset.FromUnixTimeMilliseconds(cell.value.datetimeValue).DateTime;
				else throw new NotSupportedException();

			}

			DaanyRust.free_data(dataPtr, rowCount, colCount);
			return data;
		}

		internal static string[] exctractColumns(nint columnsPtr, ulong colCount)
		{
			// Convert column headers
			string[] columns = new string[colCount];
			columns = new string[colCount];

			for (int i = 0; i < (long)colCount; i++)
			{
				IntPtr columnPtr = Marshal.ReadIntPtr(columnsPtr, i * IntPtr.Size);

				if (columnPtr == IntPtr.Zero)
				{
					columns[i] = "[Invalid Column]";
				}
				else
				{
					columns[i] = Marshal.PtrToStringAnsi(columnPtr)!;
				}
			}

			DaanyRust.free_columns(columnsPtr, colCount);
			return columns;
		}
		internal static IntPtr AllocateString(string value)
		{
			byte[] utf8Bytes = Encoding.UTF8.GetBytes(value + "\0");
			IntPtr ptr = Marshal.AllocHGlobal(utf8Bytes.Length);
			Marshal.Copy(utf8Bytes, 0, ptr, utf8Bytes.Length);
			return ptr; // Simply return the allocated memory pointer
		}

		internal static void EnsureFreshCsv(string filePath)
		{
			if (File.Exists(filePath))
			{
				File.Delete(filePath);
				Console.WriteLine($"Deleted existing file: {filePath}");
			}
		}

		/// <summary>
		/// Converts Series data to CellObject array for FFI calls
		/// </summary>
		/// <param name="data">Series data list</param>
		/// <param name="colType">Column type</param>
		/// <returns>Pointer to allocated CellObject array and its length</returns>
		internal static (IntPtr dataPtr, int length) SeriesDataToCellObjects(IList<object?> data, ColType colType)
		{
			if (data == null || data.Count == 0)
				return (IntPtr.Zero, 0);

			int length = data.Count;
			int cellObjectSize = Marshal.SizeOf<CellObject>();
			IntPtr dataPtr = Marshal.AllocHGlobal(cellObjectSize * length);

			IntPtr currentPtr = dataPtr;
			for (int i = 0; i < length; i++)
			{
				var cellObject = CreateCellObject(data[i], colType);
				Marshal.StructureToPtr(cellObject, currentPtr, false);
				currentPtr = IntPtr.Add(currentPtr, cellObjectSize);
			}

			return (dataPtr, length);
		}

		/// <summary>
		/// Creates a CellObject from a value and column type
		/// </summary>
		private static CellObject CreateCellObject(object? value, ColType colType)
		{
			var cellObject = new CellObject();
			
			if (value == null)
			{
				cellObject.typeId = -1; // Null/missing value
				return cellObject;
			}

			switch (colType)
			{
				case ColType.I32:
					cellObject.value.intValue = Convert.ToInt32(value);
					cellObject.typeId = 1;
					break;
				case ColType.I64:
					cellObject.value.longValue = Convert.ToInt64(value);
					cellObject.typeId = 2;
					break;
				case ColType.F32:
					cellObject.value.floatValue = Convert.ToSingle(value);
					cellObject.typeId = 3;
					break;
				case ColType.DD:
					cellObject.value.doubleValue = Convert.ToDouble(value);
					cellObject.typeId = 5;
					break;
				default:
					// Try to convert to double as fallback for numeric operations
					if (double.TryParse(value?.ToString(), out double doubleVal))
					{
						cellObject.value.doubleValue = doubleVal;
						cellObject.typeId = 5;
					}
					else
					{
						cellObject.typeId = -1; // Non-numeric
					}
					break;
			}

			return cellObject;
		}

		/// <summary>
		/// Frees memory allocated for CellObject array
		/// </summary>
		/// <param name="dataPtr">Pointer to allocated memory</param>
		internal static void FreeCellObjects(IntPtr dataPtr)
		{
			if (dataPtr != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(dataPtr);
			}
		}
	}

		
}
