using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Daany;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Threading.Tasks;

namespace Unit.Test.DF
{
    public class LoadSaveTests
    {

        public LoadSaveTests()
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, errors) =>
            {
                // local dev, just approve all certs
                return true;
            };
        }
      
        [Fact]
        public void LoadromCSV_Test()
        {
            string path = "testdata/titanic_full_1310.csv";
            var df = DataFrame.FromCsv(path, '\t', names: null); //
            //row test
            var r1 = df[393].ToList();

            //2	0	Denbury Mr. Herbert	male	25	0	0	C.A. 31029	31.5000		S		
            var e1 = new object[] { 2, 0, "Denbury Mr. Herbert", "male", 25, 0, 0, "C.A. 31029", "31.5",DataFrame.NAN, "S", DataFrame.NAN, DataFrame.NAN, "Guernsey / Elizabeth NJ"};


            for (int i = 0; i < e1.Length; i++)
            {
                if (r1[i] == null)
                {
                    Assert.Null(r1[i]);
                    Assert.Null(e1[i]);
                }

                else
                {
                    object v1 = r1[i].ToString();
                    object v2 = e1[i].ToString();
                    Assert.True(v1.Equals(v2));
                }

            }
        }

        [Fact]
        public void Loadrom10RowsFromCSV_Test()
        {
            string path = "testdata/titanic_full_1310.csv";
            var df = DataFrame.FromCsv(path, '\t', names: null, nRows:400); //
            //row test
            var r1 = df[393].ToList();

            //2	0	Denbury Mr. Herbert	male	25	0	0	C.A. 31029	31.5000		S		
            var e1 = new object[] { 2, 0, "Denbury Mr. Herbert", "male", 25, 0, 0, "C.A. 31029", "31.5", DataFrame.NAN, "S", DataFrame.NAN, DataFrame.NAN, "Guernsey / Elizabeth NJ" };


            for (int i = 0; i < e1.Length; i++)
            {
                if (r1[i] == null)
                {
                    Assert.Null(r1[i]);
                    Assert.Null(e1[i]);
                }

                else
                {
                    object v1 = r1[i].ToString();
                    object v2 = e1[i].ToString();
                    Assert.True(v1.Equals(v2));
                }

            }
        }


        [Fact]
        public async Task LoadFromWeb_Test()
        {

            string url = "https://archive.ics.uci.edu/ml/machine-learning-databases/iris/iris.data";
            var df = await DataFrame.FromWebAsync(url, sep: ',', names: new string[] { "sepal_length", "sepal_width", "petal_length", "petal_width", "flower_type" }); //
            //row test
            var r100 = df[100].ToList();
            //
            Assert.Equal(new List<object> { 6.3, 3.3, 6.0, 2.5, "Iris-virginica" }, r100);

        }

        [Fact]
        public void LoadromCSV_Test2()
        {
            string path = "testdata/titanic_train.csv";
            var df = DataFrame.FromCsv(path, ',', names: null, parseDate: true); //

            //
            //row test
            var r1 = df[0].ToList();
            //1,0,3,Braund Mr. Owen Harris,male,22,1,0,A/5 21171,7.25,,S,youth		
            var e1 = new object[] { 1, 0, 3, "Braund Mr. Owen Harris", "male", 22, 1, 0, "A/5 21171", 7.25, DataFrame.NAN ,"S", "youth" };


            for (int i = 0; i < e1.Length; i++)
            {
                if(r1[i]==null)
                {
                    Assert.Null(r1[i]);
                    Assert.Null(e1[i]);
                }

                else
                {
                    object v1 = r1[i].ToString();
                    object v2 = e1[i].ToString();
                    Assert.True(v1.Equals(v2));
                }
                
            }
            
        }


        [Fact]
        public void SaveToCSV_Test()
        {
            string saveDfPath = $"testdata/savedcsv_{DateTime.Now.Ticks}.csv";

            string url = "https://archive.ics.uci.edu/ml/machine-learning-databases/iris/iris.data";
            var nms = new string[] { "sepal_length", "sepal_width", "petal_length", "petal_width", "flower_type" };
            var colTy = new ColType[] { ColType.F32, ColType.F32, ColType.F32, ColType.F32, ColType.STR};
            var df = DataFrame.FromWeb(url, sep: ',', names:nms, colTypes:colTy); //
            //row test
            var retVal = DataFrame.ToCsv(saveDfPath, df);
            var dfsaved = DataFrame.FromCsv(saveDfPath, colTypes:colTy);
            File.Delete(saveDfPath);
            for (int i = 0; i < df.Values.Count; i++)
                Assert.Equal(dfsaved.Values[i],df.Values[i]);
            Assert.True(retVal);
        }


		[Fact]
		public void Read_from_CSV_ColumnTypes_resolution()
		{
			string saveDfPath = $"testdata/savedcsv_{DateTime.Now.Ticks}.csv";

			string url = "https://archive.ics.uci.edu/ml/machine-learning-databases/iris/iris.data";
			var nms = new string[] { "sepal_length", "sepal_width", "petal_length", "petal_width", "flower_type" };
			var colTy = new ColType[] { ColType.F32, ColType.F32, ColType.F32, ColType.F32, ColType.STR };
			var df = DataFrame.FromWeb(url, sep: ',', names: nms, colTypes: colTy); //
																					//row test
			var retVal = DataFrame.ToCsv(saveDfPath, df);
			var dfsaved = DataFrame.FromCsv(saveDfPath, colTypes: colTy);
			File.Delete(saveDfPath);
			for (int i = 0; i < df.Values.Count; i++)
				Assert.Equal(dfsaved.Values[i], df.Values[i]);
			Assert.True(retVal);
		}


		[Fact]
        public void SaveToCSV_TestWithMissingValues()
        {

			string filePath = $"testdata/sample_data_with_missing_values.txt";
			char separator = ',';
			string dateFormat = "yyyy-MM-dd HH:mm:ss.fff"; // Ensure milliseconds are included
			var missingValue = new char[1] { '*' };
			bool hasHeader = true;


			var df = DataFrame.FromCsv(filePath, separator, null, dateFormat, hasHeader, null, missingValue, -1, 0);

            //check column types

		}
    }
}
