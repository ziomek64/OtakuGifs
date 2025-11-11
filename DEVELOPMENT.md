# OtakuGifs Development Guide

## Project Structure

```
OtakuGifs/
├── OtakuGifs/                    # Main library project
│   ├── Client/
│   │   └── OtakuGifsClient.cs   # Main API client
│   ├── Enums/
│   │   ├── OtakuGifFormat.cs    # Format enum (GIF, WebP, AVIF)
│   │   └── OtakuGifReaction.cs  # Reaction enum (Kiss, Hug, etc.)
│   ├── Exceptions/
│   │   ├── OtakuGifsException.cs
│   │   ├── OtakuGifsBadRequestException.cs
│   │   └── OtakuGifsServerException.cs
│   └── Models/
│       └── OtakuGifsResponse.cs # API response model
├── OtakuGifs.Tests/             # Unit test project
│   └── UnitTest1.cs             # Client tests with Moq
├── OtakuGifs.Example/           # Example console app
│   └── Program.cs               # Demonstrates library usage
└── README.md                    # User documentation
```

## Building the Project

```bash
# Restore dependencies
dotnet restore

# Build all projects
dotnet build

# Run tests
dotnet test

# Run the example
dotnet run --project OtakuGifs.Example
```

## Running Tests

The test project includes 7 unit tests covering:
- Basic GIF fetching
- Format parameter handling
- Error handling (400, 500 errors)
- Fetching all reactions
- Cancellation token support
- HttpClient disposal

```bash
dotnet test OtakuGifs.Tests
```

## Creating a NuGet Package

```bash
# Build in Release mode
dotnet build OtakuGifs/OtakuGifs.csproj -c Release

# Create the package
dotnet pack OtakuGifs/OtakuGifs.csproj -c Release -o ./packages

# Publish to NuGet (update API key)
dotnet nuget push ./packages/OtakuGifs.1.0.0.nupkg --api-key YOUR_API_KEY --source https://api.nuget.org/v3/index.json
```

## Version History

### 1.0.0
- Initial release
- Support for GetGifAsync() and GetAllReactionsAsync()
- 76+ reaction types
- 3 image formats (GIF, WebP, AVIF)
- Comprehensive error handling
- Full async/await support

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new functionality
5. Ensure all tests pass
6. Submit a pull request

## Testing Against Live API

The example project can be used to test against the live OtakuGifs API:

```bash
cd OtakuGifs.Example
dotnet run
```

## Design Decisions

### Why HttpClient Injection?
The client supports both creating its own HttpClient and accepting one via dependency injection. This allows for:
- Better testability (can mock HttpClient)
- Integration with IHttpClientFactory in ASP.NET Core
- Proper HttpClient lifecycle management

### Why Separate Exception Types?
Having `OtakuGifsBadRequestException` and `OtakuGifsServerException` allows consumers to:
- Handle client errors (400) differently from server errors (500)
- Implement different retry strategies
- Provide better error messages to end users

### Why .NET Standard 2.0?
Targets .NET Standard 2.0 for maximum compatibility:
- .NET Framework 4.6.1+
- .NET Core 2.0+
- .NET 5+
- Xamarin
- Unity

## API Reference

See README.md for full API documentation and usage examples.
