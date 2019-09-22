Daany - Data Analytic C# library with the implementation of Matrix, DataFrame, generator, and decomposition of Time series and various statistics' parameters.

# Data Frame

Daany ``DataFrame`` implementation follows the .NET coding paradigm rather than Panda's look and feel. The ``DataFrame`` implementation try to fill the gap in ML.NET data preparation phase, and it can be easely passed to ML.NET pipeline. The ``DataFrame`` does not require to  any class type implementation prior to data loading and data transformation, which is huge time saving.     

The ``DataFrame`` implementation contains basic capabilities like:

- creation from a list, a dictionary, and csv file,
- persisting ``DataFrame`` into csv file,
- filtering capabilities,
- joining two or more data frame by common column,
- handling rows and columns of the data frame
- handling missing values, by dropping and replacing values on a specific column,
- grouping data in the data frame,
- aggregation on grouped data in the data frame,
- calculated column,
- apply an operation on a specific column/row in the data frame,
- ...

Once the ``DataFrame`` completes the data transformation, the extension method provides the easy way to pass the data into ```MLContex``` of the ML.NET Framework.
