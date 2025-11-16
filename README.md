# OtakuGifs

[![NuGet Version](https://img.shields.io/nuget/v/OtakuGifs.svg)](https://www.nuget.org/packages/OtakuGifs)
[![NuGet Downloads](https://img.shields.io/nuget/dt/OtakuGifs.svg)](https://www.nuget.org/packages/OtakuGifs)
[![CI](https://github.com/ziomek64/OtakuGifs/actions/workflows/ci.yml/badge.svg)](https://github.com/ziomek64/OtakuGifs/actions/workflows/ci.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

A .NET client library for the [OtakuGIFs API](https://otakugifs.xyz) - easily fetch anime reaction GIFs in your .NET applications.

## Installation

```bash
dotnet add package OtakuGifs
```

## Features

- Fully async/await API
- Type-safe reaction and format enums
- Fluent API for building requests
- Built-in dependency injection support
- Comprehensive error handling
- Supports .NET Standard 2.0+ (.NET Framework 4.6.1+, .NET Core 2.0+, .NET 5+)
- Cancellation token support

## Quick Start

```csharp
using OtakuGifs.Client;
using OtakuGifs.Enums;

// Create a client
using var client = new OtakuGifsClient();

// Get a random kiss GIF
var result = await client.GetGifAsync(OtakuGifReaction.Kiss);
Console.WriteLine(result.Url);
// Output: https://cdn.otakugifs.xyz/gifs/kiss/d7e51440.gif

// Get a hug GIF in WebP format
var webpResult = await client.GetGifAsync(
    OtakuGifReaction.Hug,
    OtakuGifFormat.WebP
);
Console.WriteLine(webpResult.Url);

// Get all available reactions
var reactions = await client.GetAllReactionsAsync();
Console.WriteLine($"Available reactions: {string.Join(", ", reactions)}");
```

## Usage

### Basic Usage

```csharp
using var client = new OtakuGifsClient();

// Fetch a GIF
var gif = await client.GetGifAsync(OtakuGifReaction.Wave);
Console.WriteLine($"GIF URL: {gif.Url}");
```

### Fluent API

```csharp
using var client = new OtakuGifsClient();

// Build requests using fluent syntax
var gif = await client.Request()
    .WithReaction(OtakuGifReaction.Kiss)
    .WithFormat(OtakuGifFormat.WebP)
    .ExecuteAsync();

Console.WriteLine($"GIF URL: {gif.Url}");
```

### Dependency Injection (ASP.NET Core, etc.)

```csharp
// In Program.cs or Startup.cs
services.AddOtakuGifsClient();

// In your service/controller
public class MyService
{
    private readonly IOtakuGifsClient _client;

    public MyService(IOtakuGifsClient client)
    {
        _client = client;
    }

    public async Task<string> GetRandomKissGifAsync()
    {
        // Use direct method
        var result = await _client.GetGifAsync(OtakuGifReaction.Kiss);
        return result.Url;

        // Or use fluent API
        var result2 = await _client.Request()
            .WithReaction(OtakuGifReaction.Hug)
            .WithFormat(OtakuGifFormat.GIF)
            .ExecuteAsync();
        return result2.Url;
    }
}
```

### Custom HttpClient Configuration (Advanced DI)

```csharp
// Configure the HttpClient used by OtakuGifs
services.AddOtakuGifsClient()
    .ConfigureHttpClient(client =>
    {
        client.Timeout = TimeSpan.FromSeconds(10);
    })
    .AddPolicyHandler(GetRetryPolicy()); // Add Polly policies, etc.
```

### With Cancellation Token

```csharp
var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

try
{
    var gif = await client.GetGifAsync(
        OtakuGifReaction.Happy,
        cancellationToken: cts.Token
    );
}
catch (OtakuGifsException ex)
{
    Console.WriteLine($"Request failed: {ex.Message}");
}
```

### Error Handling

```csharp
try
{
    var gif = await client.GetGifAsync(OtakuGifReaction.Kiss);
}
catch (OtakuGifsBadRequestException ex)
{
    // 400-level errors (invalid parameters)
    Console.WriteLine($"Bad request: {ex.Message} (Status: {ex.StatusCode})");
}
catch (OtakuGifsServerException ex)
{
    // 500-level errors (server issues)
    Console.WriteLine($"Server error: {ex.Message} (Status: {ex.StatusCode})");
}
catch (OtakuGifsException ex)
{
    // Network errors, timeouts, etc.
    Console.WriteLine($"Error: {ex.Message}");
}
```

## Available Reactions

The library includes 76+ reaction types:

- Airkiss, AngryStare, Bite, Bleh, Blush, Brofist
- Celebrate, Cheers, Clap, Confused, Cool, Cry, Cuddle
- Dance, Drool, EvilLaugh, Facepalm
- Handhold, Happy, Headbang, Hug, Huh
- Kiss, Laugh, Lick, Love
- Mad, Nervous, No, Nom, Nosebleed, Nuzzle, Nyah
- Pat, Peek, Pinch, Poke, Pout, Punch
- Roll, Run, Sad, Scared, Shout, Shrug, Shy, Sigh, Sip, Slap
- Sleep, SlowClap, Smack, Smile, Smug, Sneeze, Sorry
- Stare, Stop, Surprised, Sweat
- Thumbsup, Tickle, Tired
- Wave, Wink, Woah
- Yawn, Yay, Yes

## Supported Formats

- `OtakuGifFormat.GIF` (default)
- `OtakuGifFormat.WebP`
- `OtakuGifFormat.AVIF`

## API Reference

### Client Methods

#### `GetGifAsync`

Fetches a random GIF based on the specified reaction.

```csharp
Task<OtakuGifsResponse> GetGifAsync(
    OtakuGifReaction reaction,
    OtakuGifFormat format = OtakuGifFormat.GIF,
    CancellationToken cancellationToken = default
)
```

#### `GetAllReactionsAsync`

Fetches all available reactions from the API.

> **Note:** This method is provided for completeness. Since `GetGifAsync` uses the type-safe `OtakuGifReaction` enum which already includes all possible reactions, you typically won't need to call this method. If you discover new reactions available in the API, feel free to open a PR to add them to the enum!

```csharp
Task<string[]> GetAllReactionsAsync(
    CancellationToken cancellationToken = default
)
```

#### `Request`

Creates a fluent request builder for constructing GIF requests.

```csharp
GifRequestBuilder Request()
```

### Fluent API Methods

#### `WithReaction`

Specifies the reaction type for the request.

```csharp
GifRequestBuilder WithReaction(OtakuGifReaction reaction)
```

#### `WithFormat`

Specifies the format for the request.

```csharp
GifRequestBuilder WithFormat(OtakuGifFormat format)
```

#### `ExecuteAsync`

Executes the request with the configured parameters.

```csharp
Task<OtakuGifsResponse> ExecuteAsync(CancellationToken cancellationToken = default)
```

## License

MIT

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## Credits

This library is a wrapper for the [OtakuGIFs API](https://otakugifs.xyz).
