using System;
using System.Threading;
using System.Threading.Tasks;
using OtakuGifs.Enums;
using OtakuGifs.Models;

namespace OtakuGifs.Client;

/// <summary>
/// Fluent builder for constructing GIF requests
/// </summary>
public class GifRequestBuilder
{
    private readonly IOtakuGifsClient _client;
    private OtakuGifReaction? _reaction;
    private OtakuGifFormat _format = OtakuGifFormat.GIF;

    /// <summary>
    /// Creates a new instance of GifRequestBuilder
    /// </summary>
    /// <param name="client">The client instance to use for executing requests</param>
    internal GifRequestBuilder(IOtakuGifsClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    /// <summary>
    /// Specifies the reaction type for the GIF request
    /// </summary>
    /// <param name="reaction">The reaction type</param>
    /// <returns>The current builder instance for method chaining</returns>
    public GifRequestBuilder WithReaction(OtakuGifReaction reaction)
    {
        _reaction = reaction;
        return this;
    }

    /// <summary>
    /// Specifies the format for the GIF request
    /// </summary>
    /// <param name="format">The image format</param>
    /// <returns>The current builder instance for method chaining</returns>
    public GifRequestBuilder WithFormat(OtakuGifFormat format)
    {
        _format = format;
        return this;
    }

    /// <summary>
    /// Executes the GIF request with the configured parameters
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The response containing the GIF URL</returns>
    /// <exception cref="InvalidOperationException">Thrown when reaction is not specified</exception>
    public Task<OtakuGifsResponse> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        if (!_reaction.HasValue)
        {
            throw new InvalidOperationException("Reaction must be specified using WithReaction() before executing the request");
        }

        return _client.GetGifAsync(_reaction.Value, _format, cancellationToken);
    }
}
