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
	}

		
}
