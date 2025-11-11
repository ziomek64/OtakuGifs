using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using OtakuGifs.Enums;
using OtakuGifs.Models;
using OtakuGifs.Exceptions;

namespace OtakuGifs.Client;

/// <summary>
/// Client for interacting with the OtakuGIFs API
/// </summary>
public class OtakuGifsClient : IDisposable
{
    private const string BaseUrl = "https://api.otakugifs.xyz";
    private readonly HttpClient _httpClient;
    private readonly bool _disposeHttpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// Creates a new instance of OtakuGifsClient with a default HttpClient
    /// </summary>
    public OtakuGifsClient() : this(new HttpClient(), true)
    {
    }

    /// <summary>
    /// Creates a new instance of OtakuGifsClient with a custom HttpClient
    /// </summary>
    /// <param name="httpClient">The HttpClient to use for API requests</param>
    /// <param name="disposeHttpClient">Whether to dispose the HttpClient when this client is disposed</param>
    public OtakuGifsClient(HttpClient httpClient, bool disposeHttpClient = false)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _disposeHttpClient = disposeHttpClient;

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        if (_httpClient.BaseAddress == null)
        {
            _httpClient.BaseAddress = new Uri(BaseUrl);
        }
    }

    /// <summary>
    /// Fetches a random GIF based on the specified reaction
    /// </summary>
    /// <param name="reaction">The reaction type</param>
    /// <param name="format">The image format (defaults to GIF)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The response containing the GIF URL</returns>
    public async Task<OtakugifsResponse> GetGifAsync(
        OtakuGifReaction reaction,
        OtakuGifFormat format = OtakuGifFormat.GIF,
        CancellationToken cancellationToken = default)
    {
        var reactionParam = reaction.ToString().ToLowerInvariant();
        var formatParam = format.ToString().ToLowerInvariant();
        var url = $"/gif?reaction={reactionParam}&format={formatParam}";

        try
        {
            var response = await _httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                await HandleErrorResponseAsync(response, cancellationToken).ConfigureAwait(false);
            }

            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var result = JsonSerializer.Deserialize<OtakugifsResponse>(content, _jsonOptions);

            if (result == null)
            {
                throw new OtakuGifsException("Failed to deserialize response");
            }

            return result;
        }
        catch (HttpRequestException ex)
        {
            throw new OtakuGifsException("Network error occurred while fetching GIF", ex);
        }
        catch (TaskCanceledException ex)
        {
            throw new OtakuGifsException("Request timed out", ex);
        }
    }

    /// <summary>
    /// Fetches all available reactions from the API
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Array of available reaction names</returns>
    public async Task<string[]> GetAllReactionsAsync(CancellationToken cancellationToken = default)
    {
        const string url = "/gif/allreactions";

        try
        {
            var response = await _httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                await HandleErrorResponseAsync(response, cancellationToken).ConfigureAwait(false);
            }

            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var result = JsonSerializer.Deserialize<AllReactionsResponse>(content, _jsonOptions);

            if (result?.Reactions == null)
            {
                throw new OtakuGifsException("Failed to deserialize reactions response");
            }

            return result.Reactions;
        }
        catch (HttpRequestException ex)
        {
            throw new OtakuGifsException("Network error occurred while fetching reactions", ex);
        }
        catch (TaskCanceledException ex)
        {
            throw new OtakuGifsException("Request timed out", ex);
        }
    }

    private class AllReactionsResponse
    {
        public string[] Reactions { get; set; } = null!;
    }

    private async Task HandleErrorResponseAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var statusCode = (int)response.StatusCode;
        var errorMessage = $"API request failed with status {statusCode} ({response.ReasonPhrase})";

        if (statusCode >= 500)
        {
            throw new OtakuGifsServerException(errorMessage, statusCode);
        }
        else if (statusCode >= 400)
        {
            throw new OtakuGifsBadRequestException(errorMessage, statusCode);
        }
        else
        {
            throw new OtakuGifsException(errorMessage);
        }
    }

    /// <summary>
    /// Disposes the client and optionally the underlying HttpClient
    /// </summary>
    public void Dispose()
    {
        if (_disposeHttpClient)
        {
            _httpClient?.Dispose();
        }
    }
}