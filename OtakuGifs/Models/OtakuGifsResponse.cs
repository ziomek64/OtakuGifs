namespace OtakuGifs.Models;

/// <summary>
/// Response model containing the URL of a GIF returned by the OtakuGIFs API
/// </summary>
public class OtakuGifsResponse
{
    /// <summary>
    /// URL of the image returned by the API
    /// </summary>
    public string Url { get; set; } = null!;
}