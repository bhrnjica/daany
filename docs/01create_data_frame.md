# Create ``DataFrame`` from the standard .NET collections

``DataFrame`` object can be easely created from the list of numbers, or any .NET objects. For example the following code creates the data frame with two columns and 5 rows:

````csharp
//list of object
var list = new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

//defined columns
var cols = new string[] { "col1", "col2" };

//create data frame with two columns and 5 rows.
var df = new DataFrame(list, cols);

//check the size of the data frame
Assert.Equal(5, df.RowCount());
Assert.Equal(2, df.ColCount());
```` 
The list count must be divisible by column count, otherwise the exception is thrown. 

The following code creates the data frame from a Dictionary collection:
````csharp
var dict = new Dictionary<string, List<object>>
{
    { "col1",new List<object>() { 1,11,21,31,41,51,61,71,81,91} },
    { "col2",new List<object>() { 2,12,22,32,42,52,62,72,82,92 } },
    { "col3",new List<object>() { 3,13,23,33,43,53,63,73,83,93 } },
    { "col4",new List<object>() { 4,14,24,34,44,54,64,74,84,94} },
    { "col5",new List<object>() { 5,15,25,35,45,55,65,75,85,95 } },
    { "col6",new List<object>() { 6,16,26,36,46,56,66,76,86,96} },
    { "col7",new List<object>() { 7,17,27,37,47,57,67,77,87,97 } },
    { "col8",new List<object>() { 8,18,28,38,48,58,68,78,88,98} },
    { "col9",new List<object>() { 9,19,29,39,49,59,69,79,89,99} },
    { "col10",new List<object>() { 10,20,30,40,50,60,70,80,90,100} },
};
//
var df = new DataFrame(dict);

//check the size of the data frame
Assert.Equal(10, df.RowCount());
Assert.Equal(10, df.ColCount());
````

As third option the ``DataFrame`` can be created from the ```csv``` file by calling static method ```DataFrame.FromCsv```:
 ````csharp
 var filePath = $"..\\..\\..\\testdata\\group_sample_testdata.txt";
            var df = DataFrame.FromCsv(filePath: filePath, 
                                                sep: '\t', 
                                                names: null, dformat: null);
//check the size of the data frame
Assert.Equal(27, df.RowCount());
Assert.Equal(6, df.ColCount());
````