
```powershell
# Restore dependencies (needed once, or after dependency changes).
dotnet restore src\AudioFormatLib.Tests\AudioFormatLib.Tests.csproj

# Build the test project and its library dependency.
dotnet build src\AudioFormatLib.Tests\AudioFormatLib.Tests.csproj

# Run the complete test suite.
dotnet test src\AudioFormatLib.Tests\AudioFormatLib.Tests.csproj

# Run without restoring packages again.
dotnet test src\AudioFormatLib.Tests\AudioFormatLib.Tests.csproj --no-restore

# Run one test class.
dotnet test src\AudioFormatLib.Tests\AudioFormatLib.Tests.csproj --filter "FullyQualifiedName~AudioResamplerTests"

# Run one test method.
dotnet test src\AudioFormatLib.Tests\AudioFormatLib.Tests.csproj --filter "FullyQualifiedName~Pcm16LittleEndianTests.EncodeDecode_RoundTripsSignedBoundaryValues"
```
