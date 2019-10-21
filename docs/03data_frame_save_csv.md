# Persist ``DataFrame`` as csv file

Once the ```DataFrame``` object should be saved for later use, it can be saved into csv file. The following code saves the ```DataFrame``` to csv file:

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

//save data frame into csv file
var retValue = DataFrame.SaveToCsv("dataframe_file.csv", df);
if(retValue)
    Console.WriteLines("DataFrame has been saved successfully.");
````
