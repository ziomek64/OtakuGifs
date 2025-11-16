using OtakuGifs.Client;
using OtakuGifs.Enums;
using OtakuGifs.Exceptions;

Console.WriteLine("OtakuGifs Example App");
Console.WriteLine("=====================\n");

// Create a client instance
using var client = new OtakuGifsClient();

try
{
    // Example 1: Get a random kiss GIF
    Console.WriteLine("Example 1: Fetching a random kiss GIF...");
    var kissGif = await client.GetGifAsync(OtakuGifReaction.Kiss);
    Console.WriteLine($"Kiss GIF URL: {kissGif.Url}\n");

    // Example 2: Get a hug GIF in WebP format (using fluent API)
    Console.WriteLine("Example 2: Fetching a hug GIF in WebP format using fluent API...");
    var hugGif = await client.Request()
        .WithReaction(OtakuGifReaction.Hug)
        .WithFormat(OtakuGifFormat.WebP)
        .ExecuteAsync();
    Console.WriteLine($"Hug GIF URL: {hugGif.Url}\n");

    // Example 3: Get a pat GIF in AVIF format
    Console.WriteLine("Example 3: Fetching a pat GIF in AVIF format...");
    var patGif = await client.GetGifAsync(OtakuGifReaction.Pat, OtakuGifFormat.AVIF);
    Console.WriteLine($"Pat GIF URL: {patGif.Url}\n");

    // Example 4: Get all available reactions
    Console.WriteLine("Example 4: Fetching all available reactions...");
    var reactions = await client.GetAllReactionsAsync();
    Console.WriteLine($"Total reactions available: {reactions.Length}");
    Console.WriteLine($"Sample reactions: {string.Join(", ", reactions.Take(10))}...\n");

    // Example 5: Try multiple reactions using fluent API
    Console.WriteLine("Example 5: Fetching multiple random reactions using fluent API...");
    var randomReactions = new[]
    {
        OtakuGifReaction.Happy,
        OtakuGifReaction.Laugh,
        OtakuGifReaction.Wave,
        OtakuGifReaction.Thumbsup,
        OtakuGifReaction.Dance
    };

    foreach (var reaction in randomReactions)
    {
        var gif = await client.Request()
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
catch (Exception ex)
{
    Console.WriteLine($"Unexpected error: {ex.Message}");
}

Console.WriteLine("\nPress any key to exit...");
Console.ReadKey();
