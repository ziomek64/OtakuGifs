using System;
using System.Threading;
using System.Threading.Tasks;
using OtakuGifs.Enums;
using OtakuGifs.Models;

namespace OtakuGifs.Client;

/// <summary>
/// Interface for the OtakuGIFs API client
/// </summary>
public interface IOtakuGifsClient : IDisposable
{
    /// <summary>
    /// Fetches a random GIF based on the specified reaction
    /// </summary>
    /// <param name="reaction">The reaction type</param>
    /// <param name="format">The image format (defaults to GIF)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The response containing the GIF URL</returns>
    Task<OtakuGifsResponse> GetGifAsync(
        OtakuGifReaction reaction,
        OtakuGifFormat format = OtakuGifFormat.GIF,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Fetches all available reactions from the API
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Array of available reaction names</returns>
    Task<string[]> GetAllReactionsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new fluent request builder for constructing GIF requests
    /// </summary>
    /// <returns>A new instance of GifRequestBuilder</returns>
    GifRequestBuilder Request();
}
