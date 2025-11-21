using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OtakuGifs.Client;
using OtakuGifs.Enums;
using OtakuGifs.Exceptions;
using OtakuGifs.Extensions;

Console.WriteLine("OtakuGifs Dependency Injection Example");
Console.WriteLine("=======================================\n");

// Create and configure the host with dependency injection
var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // Register OtakuGifs client using the extension method
        services.AddOtakuGifsClient()
            .ConfigureHttpClient(client =>
            {
                client.Timeout = TimeSpan.FromSeconds(30);
            });

        // Register our application service
        services.AddTransient<GifService>();
    })
    .Build();

// Get the service from the container and run it
var gifService = host.Services.GetRequiredService<GifService>();
await gifService.RunAsync();

Console.WriteLine("\nPress any key to exit...");
Console.ReadKey();

/// <summary>
/// Example service that uses IOtakuGifsClient via dependency injection
/// </summary>
public class GifService
{
    private readonly IOtakuGifsClient _client;

    public GifService(IOtakuGifsClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        Console.WriteLine("✓ GifService created with injected IOtakuGifsClient\n");
    }

    public async Task RunAsync()
    {
        try
        {
            // Example 1: Using direct method call
            Console.WriteLine("Example 1: Using direct method call");
            var kissGif = await _client.GetGifAsync(OtakuGifReaction.Kiss);
            Console.WriteLine($"Kiss GIF: {kissGif.Url}\n");

            // Example 2: Using fluent API
            Console.WriteLine("Example 2: Using fluent API");
            var hugGif = await _client.Request()
                .WithReaction(OtakuGifReaction.Hug)
                .WithFormat(OtakuGifFormat.WebP)
                .ExecuteAsync();
            Console.WriteLine($"Hug GIF (WebP): {hugGif.Url}\n");

            // Example 3: Using fluent API with different reactions
            Console.WriteLine("Example 3: Multiple reactions using fluent API");
            var reactions = new[]
            {
                OtakuGifReaction.Happy,
                OtakuGifReaction.Wave,
                OtakuGifReaction.Dance
            };

            foreach (var reaction in reactions)
            {
                var gif = await _client.Request()
                    .WithReaction(reaction)
                    .ExecuteAsync();
                Console.WriteLine($"{reaction}: {gif.Url}");
            }

            Console.WriteLine("\n✓ All examples completed successfully!");
        }
        catch (OtakuGifsBadRequestException ex)
        {
            Console.WriteLine($"Bad Request Error: {ex.Message} (Status: {ex.StatusCode})");
        }
        catch (OtakuGifsServerException ex)
        {
            Console.WriteLine($"Server Error: {ex.Message} (Status: {ex.StatusCode})");
        }
        catch (OtakuGifsException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}