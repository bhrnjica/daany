# Daany - .NET Data Analytics Library

Daany is a .NET 7.0+ data analytics library with DataFrame, time series decomposition, and linear algebra capabilities. The project includes C# components with Rust native libraries for performance-critical operations.

Always reference these instructions first and fallback to search or bash commands only when you encounter unexpected information that does not match the info here.

## Working Effectively

### Prerequisites and Setup
Install required SDKs and dependencies:
- **CRITICAL**: Install .NET 9.0, 8.0, and 7.0 SDKs AND runtimes. Project targets all three frameworks.
  ```bash
  # Download and install .NET SDKs
  wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
  chmod +x dotnet-install.sh
  ./dotnet-install.sh --channel 9.0 --install-dir $HOME/.dotnet
  ./dotnet-install.sh --channel 8.0 --install-dir $HOME/.dotnet  
  ./dotnet-install.sh --channel 7.0 --install-dir $HOME/.dotnet
  
  # Install .NET 7.0 runtime (required for tests)
  ./dotnet-install.sh --runtime dotnet --channel 7.0 --install-dir $HOME/.dotnet
  
  export PATH=$HOME/.dotnet:$PATH
  ```
- Rust toolchain (cargo/rustc) - usually pre-installed on development systems
- Verify installations: `dotnet --list-sdks` should show 7.0.x, 8.0.x, and 9.0.x

### Build Process
**ALWAYS build in this exact order**:

1. **Rust Native Library** (24 seconds - NEVER CANCEL):
   ```bash
   cd src/daany_rust
   cargo build --release
   ```
   **Timeout: 60+ minutes** - Set generous timeouts for Rust compilation

2. **Restore Dependencies** (21 seconds - NEVER CANCEL):
   ```bash
   cd [repo-root]
   export PATH=$HOME/.dotnet:$PATH
   dotnet restore
   ```
   **Timeout: 15+ minutes**

3. **Build Solution** (31 seconds - NEVER CANCEL):
   ```bash
   dotnet build --configuration Release --no-restore
   ```
   **Timeout: 60+ minutes** - Builds all target frameworks (net7.0, net8.0, net9.0)

### Testing
Run tests (7 seconds - NEVER CANCEL):
```bash
dotnet test --configuration Release --no-build --logger "console;verbosity=minimal"
```
**Timeout: 30+ minutes**

**Expected Test Results**: 1176 total tests, ~38 failures due to network connectivity issues (external UCI dataset downloads). This is NORMAL in isolated environments.

## Known Issues and Limitations

### Network Dependencies
- Tests attempting to download datasets from `archive.ics.uci.edu` will fail in environments without internet access
- **This is expected behavior** - the core library functionality works correctly
- Failed network tests do NOT indicate build problems

### Runtime Warnings
- Build produces ~265 warnings related to obsolete APIs and nullable annotations
- **This is normal** - warnings do not prevent successful compilation

### Multi-Framework Targeting
- Project targets .NET 7.0, 8.0, and 9.0 simultaneously
- Missing any framework will cause build failures
- Tests may fail for .NET 7.0 if runtime is not available

## Project Structure

### Main Components (NuGet Packages)
- `Daany.DataFrame` - Core data frame implementation
- `Daany.DataFrame.Ext` - Extensions for plotting and data scaling  
- `Daany.Stat` - Time series decomposition (SSA, STL, ARIMA)
- `Daany.LinA` - Linear algebra wrapper around Intel MKL LAPACK/BLAS
- `Daany.MathStuff` - Mathematical operations and statistics
- `daany.util` - Utility functions

### Key Directories
```
src/
├── daany.df/           # Core DataFrame implementation
├── daany.ext/          # DataFrame extensions
├── daany.stat/         # Statistics and time series
├── daany.lina/         # Linear algebra
├── daany.mathstuff/    # Math operations
├── daany.util/         # Utilities
└── daany_rust/         # Rust native performance library

test/
├── XUnit.Test/         # Main test suite with test data
└── Daany.Test.App/     # Test application
```

## Validation

### Manual Testing Scenarios
After making changes, **ALWAYS validate** by:

1. **Build Validation**: Ensure all projects compile successfully
   ```bash
   dotnet build --configuration Release
   ```

2. **Basic DataFrame Operations**: Test core functionality by running a subset of tests
   ```bash
   # Test core DataFrame functionality (excludes network-dependent tests)
   dotnet test --filter "FullyQualifiedName~ConfusionMatrix" --configuration Release --no-build
   
   # Or test specific component areas
   dotnet test --filter "FullyQualifiedName~Daany.MathStuff" --configuration Release --no-build
   ```

3. **Rust Integration**: Verify native library builds and integration
   ```bash
   # Check Rust library compilation
   find src/daany_rust/target/release -name "*.so" -o -name "*.dll" | head -5
   
   # Verify output directories contain test data
   ls test/XUnit.Test/bin/Release/net*/testdata/
   ```

### Continuous Integration
- GitHub Actions workflow: `.github/workflows/CI_dotnet.yml`
- Builds on Ubuntu and Windows
- **Full CI takes 45+ minutes** - includes Rust compilation and extensive testing
- **NEVER CANCEL CI builds** - they require complete execution

## Development Workflow

### Making Changes
1. **ALWAYS** build Rust library first if modifying native code
2. Build incrementally: `dotnet build --no-restore` for faster iteration
3. Run targeted tests: `dotnet test --filter "FullyQualifiedName~YourTestNamespace"`
4. **Always run full test suite** before committing

### Performance Critical Code
- Rust components in `src/daany_rust/` handle performance-critical operations
- C# wrapper code handles high-level API and integration
- Modifications to native code require Rust rebuild

### Documentation
- Main docs in `docs/DevGuide/developer_guide.md`
- API examples in README.MD
- Jupyter notebook formatting support in `script/jupyter_format.txt`

## Common Commands Reference

### Repository Status
```bash
ls [repo-root]
# Expected: .github/ .vscode/ src/ test/ docs/ *.sln *.md LICENSE
```

### Quick Health Check
```bash
# Verify all SDKs available
dotnet --list-sdks

# Check Rust toolchain  
cargo --version

# Quick build test (excludes tests)
dotnet build --configuration Release --verbosity minimal
```

### Troubleshooting Build Issues
- **"NETSDK1045" errors**: Missing .NET SDK version - install required framework
- **Rust build failures**: Check cargo version, run `cargo clean` then rebuild
- **Test failures**: Expected for network-dependent tests in isolated environments
- **Missing native libraries**: Rust build may not have completed - rebuild Rust components

## Time Expectations
- **Rust build**: 24 seconds (normal), up to 10 minutes (cold cache)
- **Dependency restore**: 21 seconds (normal), up to 5 minutes (first time)
- **Full solution build**: 31 seconds (normal), up to 15 minutes (clean build)
- **Test execution**: 7 seconds (normal), up to 10 minutes (full suite)
- **Complete CI pipeline**: 45+ minutes - **NEVER CANCEL**

**CRITICAL**: Always set timeouts of 60+ minutes for build operations and 30+ minutes for test operations to prevent premature cancellation.