using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Daany.Test.App
{
    public class DataFrameTest
    {

        public static void RunMergeTest()
        {
            //datetime,machineID,volt,rotate,pressure,vibration
            var telemetryPath = @"C:\sc\vs\Academic\PrM\Data\telemetry.csv";
            //machineID,model,age
            var machinesPath = @"C:\sc\vs\Academic\PrM\Data\machines.csv";

            //datetime,machineID,comp
            var maintPath = @"C:\sc\vs\Academic\PrM\Data\maint.csv";

            var sw = Stopwatch.StartNew();
            var telemetry = DataFrame.FromCsv(telemetryPath, sep: ',', parseDate: true);
            Console.WriteLine($"Open data frame with {telemetry.RowCount()} rows and {telemetry.ColCount()} cols, for {sw.ElapsedMilliseconds} milisec.");
            sw.Restart();
            var machines = DataFrame.FromCsv(machinesPath, sep: ',', parseDate: true);
            Console.WriteLine($"Open data frame with {machines.RowCount()} rows and {machines.ColCount()} cols, for {sw.ElapsedMilliseconds} milisec.");
            sw.Restart();
            var maint = DataFrame.FromCsv(maintPath, sep: ',', parseDate: true);
            Console.WriteLine($"Open data frame with {maint.RowCount()} rows and {maint.ColCount()} cols, for {sw.ElapsedMilliseconds} milisec.");
            var mCols = new string[] { "datetime", "machineID" };
            sw.Restart();
            var newDf = telemetry.Merge_old(maint, mCols, mCols, JoinType.Left);
            Console.WriteLine($"Merge took {sw.ElapsedMilliseconds} milisec.");

            sw.Restart();
            var newDf1 = telemetry.Merge(maint, mCols, mCols, JoinType.Left);
            Console.WriteLine($"Merge New Way took {sw.ElapsedMilliseconds} milisec.");

           
        }
    }
}
