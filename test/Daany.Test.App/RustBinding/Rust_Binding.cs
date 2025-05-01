using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
namespace Daany.Test.App.Binding
{
	public class CsvBenchmark
	{
		private const string filePath = @"alarms.csv";
		private const char separator = ',';
		private const string dateFormat = "%Y-%m-%d %H:%M:%S%.3f";
		private const bool hasHeader = true;

		[Benchmark]
		public DataFrame LoadCsvEx()
		{
			return DataFrame.FromCsvEx(filePath, separator, null, dateFormat, true, null, new char[1] { '*' }, -1, 0);
		}

		[Benchmark]
		public DataFrame LoadCsv()
		{
			return DataFrame.FromCsv(filePath, separator, null, "yyyy-MM-dd HH:mm:ss.fff", true, null, new char[1] { '*' }, -1, 0);
		}

	}

	internal static class Rust_Binding_Tests
	{
		private static void runBenchmark()
		{
			var summary = BenchmarkRunner.Run<CsvBenchmark>();
			Console.WriteLine(summary);
		}
		public static void Run()
		{
			//runBenchmark();
			//write_to_csv();

			read_from_csv();



		}


		private static void read_from_csv()
		{
			////string filePath = "AirPassengers.csv";
			string filePath = @"alarms.csv";
			char separator = ',';
			string dateFormat = "yyyy-MM-dd HH:mm:ss.fff"; // Ensure milliseconds are included
			string missingValue = "*";
			bool hasHeader = true;


			var df = DataFrame.FromCsv(filePath, separator, null, dateFormat, true, null, new char[1] { '*' }, -1, 0);

			//var tsk = DataFrame.FromCsvAsync(filePath, separator, hasHeader, dateFormat, null, new char[1] { '*' }, 0, -1);
			//tsk.Wait();
			//var df = tsk.Result;

			/*
			// Measure time for FromCsvEx
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			DataFrame df1 = DataFrame.FromCsvEx(filePath, separator, null, dateFormat, true, null, new char[1] { '*' }, -1, 0);
			stopwatch.Stop();
			Console.WriteLine($"FromCsvEx execution time: {stopwatch.ElapsedMilliseconds} ms");

			// Measure time for FromCsv
			stopwatch.Restart();
			DataFrame df2 = DataFrame.FromCsv(filePath, separator, null, dateFormat, true, null, new char[1] { '*' }, -1, 0);
			stopwatch.Stop();
			Console.WriteLine($"FromCsv execution time: {stopwatch.ElapsedMilliseconds} ms");

			*/
		}

		

		private static void write_to_csv()
		{
			var dict = new Dictionary<string, List<object>>
			{
				{"product_id",new List<object>()    {1,1,2,2,2,2,2 } },
				{ "retail_price",new List<object>() { 2,2,5,5,5,5,5 } },
				{ "quantity",new List<object>()     { 1,2,4,8,16,32,64 } },
				{ "city",new List<object>() { "SF","SJ","SF","SJ","Miami", "Orlando","SJ"} },
				{ "state" ,new List<object>() { "CA","CA","CA","CA","FL","FL","PR" } },
			};
			var dataFrame = new DataFrame(dict);
			string filePath = "complex_output.csv";

			////string filePath = "AirPassengers.csv";
			//string filePath = @"C:\Users\bhrnjica\Downloads\alarms.csv";
			char separator = ',';
			string dateFormat = "%Y-%m-%d %H:%M:%S%.3f"; // Ensure milliseconds are included
			bool hasHeader = true;

			DataFrame.ToCsvEx(filePath, dataFrame, separator, dateFormat, hasHeader);

			Process.Start(new ProcessStartInfo
			{
				FileName = "notepad.exe",
				Arguments = filePath,
				UseShellExecute = true
			});
		}

		

		static void EnsureFreshCsv(string filePath)
		{
			if (File.Exists(filePath))
			{
				File.Delete(filePath);
				Console.WriteLine($"Deleted existing file: {filePath}");
			}
		}

	}

}
