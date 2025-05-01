# MathStuff project

`MathStuff` project contains implementation about basic math e.g Scalar, Vector and Matrix operations, Norms, Description Statistics, Metrics, Interpolations and Confusion Matrix.

Since .NET 7 whole MathStuff project is going to be rewtitten in generic manner. For this reason now there are duplicated implementation for all classes in the project.
The `Readme` is going to describe only generic implementation and how to use it.

## Basic Math operations

The objective of this section is to provide guidance on performing operations involving `scalars`, `vectors`, and `matrices`. 
The `MathStuff` project facilitates these operations and eliminates the need for users to differentiate between those types. 
Furthermore, when working with matrices, `MathStuff` accommodates all forms of 2D arrays and lists, as well as all numeric types, such as int, float, and double.
Assume there is a need to perfome matrix operations. 
The `MathStuff` projects is consisted of the following assets:

- `Interpolation` - `linar`, `spline` and `polynomial` interpolation between points. See `Interpolation_Test` for how to use those classes.

- `Matrix` - implementation of the `matrix` and `vector` operations, decompositions and special matrix. See `Matrix_test` for usage of the implementation.

- `Norms` - implementation of different norms e.g. Euclidian, Mahhattan etc norms.

- `Random` - implementation of the Random number generator.

- `Stat-Metrics` - the implementation of various statistical metrics including confusion matrix and related implementation for time series.