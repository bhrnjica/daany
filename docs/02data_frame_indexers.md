# Access ``DataFrame`` elements (row, col or cell)
``DataFrame`` object is consisted of four components: ```row```,```indexer```, ```column``` and ```cell```.

Note: The Indexers are not implemented.

During data manipulation indexers are key components for accessing the data frame components.
Lets create a simple data frame:
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
````
As can be seen the data frame has 10 columns ("col", ..."col10") and 10 rows.

### Accesing ```row```

In case specific row should be accessed the following code is called:

````csharp
//row at 5th position
var row5 = df[4]//due to zero-based index
````
The returned row is Enumerable collection.

### Accesing ```Column```
In case specific column (e.g. `col5`) should be accessed the following code is called:

````csharp
//row at 5th position
var row5 = df["col5"]
````
The column are always accessed by the name of the column.

### Accesing several ```Columns``` at once

In case several columns shoud be accessed at once, the new `dataframe` object is created from specified columns:
````csharp
//access 4 columns, by returning new DataFrame
DatFrame newDf = df["col1", "col3", "col5", "col9"];
````
### Accesing data frame ```cell```
In case the data frame cell value shoudl be accessed, the following code is called:

````csharp
//first column and second row = 11
var cellValue = df["col1", 1];//string indexer (colName, rowIndex)
//second column and fourth row
var cellValue2 = df[3,2];//numeric indexer (rowIndex,colIndex)
````