# MagmaSharp
**MagmaSharp** is .NET High Level API for [MAGMA](https://icl.cs.utk.edu/projectsfiles/magma/doxygen/index.html) - Matrix Algebra for GPU and Multicore Architectures.


Only selected MAGMA routines are exposed in the API. Each method can run regardless of the CUDA present. In case the CUDA is not detected, the corresponded Lapack routine would be executed. On this way, the library can be execution engine for other .NET High Level APIs and libraries.

## Implementation of MagmaSharp
Currently the library supports MAGMA driver routines for general rectangular matrix:

1. ```gesv``` - solve linear system, AX = B, A is general non-symetric matrix,
2. ```gels``` - least square solve, AX = B, A is rectangular,
3. ```geev``` - eigen value solver for non-symetric matrix, AX = X lambda
4. ```gesvd```- singular value decomposition (SVD), A) U SIgma V^2H.

The library supports `float` and `double` value types only.

# Software requirements

The project is build on .NET Core 3.1 and .NET Standard 2.1. It is built and tested on Windows 10 1909 only. 

# Software (Native Libraries) requirements
In order to compile, build and use the library the following native libraries are needed to be installed. 

- Intel MKL which can be downloaded at https://software.intel.com/content/www/us/en/develop/tools/math-kernel-library/choose-download.html
- MAGMA runtime library which can be download from the [Official site](https://icl.utk.edu/magma/software/index.html).

However, if you install the MAgmaSharp as Nuget package, both libraries are included, so you don't have to install it. 

# How to use MagmaSharp

 MagmaSharp is packed as Nuget and can be added to your .NET project as ordinary .NET Nuget component. You don't have to worry about native libraries and dependencies. Everything is included in the package.
The package can be installed from this link, or just search MagmaSharp.

# How to Build MagmaSharp from the source

1. Download the MagmaSharp source code from the GitHub page. 
2. Reference Magma static library and put it to folder MagmaLib.

![Magma runtime location](img/magma_lib_location.jpg)
Magma static library can be downloaded and built from the [Official site](https://icl.utk.edu/magma/software/index.html).

3. Open 'MagmaSharp.sln' with Visual Studio 2019.
4. Make Sure the building architecture is x64.
5. Restore Nuget packages.
5. Build and Run the Solution.


# How to Tutorials 

In this section we are going to provide you with the instruction how you can use the Library in your .NET projects.


