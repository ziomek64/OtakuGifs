using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Moq.Protected;
using OtakuGifs.Client;
using OtakuGifs.Enums;
using OtakuGifs.Exceptions;
using OtakuGifs.Extensions;
using OtakuGifs.Models;
using Xunit;

namespace OtakuGifs.Tests;

public class OtakuGifsClientTests
{
    [Fact]
    public async Task GetGifAsync_ReturnsValidResponse()
    {
        // Arrange
        var expectedUrl = "https://cdn.otakugifs.xyz/gifs/kiss/d7e51440.gif";
        var response = new OtakuGifsResponse { Url = expectedUrl };
        var httpClient = CreateMockHttpClient(HttpStatusCode.OK, JsonSerializer.Serialize(response));
        var client = new OtakuGifsClient(httpClient);

        // Act
        var result = await client.GetGifAsync(OtakuGifReaction.Kiss, OtakuGifFormat.GIF);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedUrl, result.Url);
    }

    [Fact]
    public async Task GetGifAsync_WithDifferentFormat_UsesCorrectParameter()
    {
        // Arrange
        var response = new OtakuGifsResponse { Url = "https://cdn.otakugifs.xyz/gifs/hug/test.webp" };
        var mockHandler = new Mock<HttpMessageHandler>();
        mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.RequestUri!.ToString().Contains("format=webp")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(response))
            });

        var httpClient = new HttpClient(mockHandler.Object)
        {
            BaseAddress = new Uri("https://api.otakugifs.xyz")
        };
        var client = new OtakuGifsClient(httpClient);

        // Act
        var result = await client.GetGifAsync(OtakuGifReaction.Hug, OtakuGifFormat.WebP);

        // Assert
        Assert.NotNull(result);
        mockHandler.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.RequestUri!.ToString().Contains("format=webp")),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task GetGifAsync_WithBadRequest_ThrowsBadRequestException()
    {
        // Arrange
        var httpClient = CreateMockHttpClient(HttpStatusCode.BadRequest, "");
        var client = new OtakuGifsClient(httpClient);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<OtakuGifsBadRequestException>(
            () => client.GetGifAsync(OtakuGifReaction.Kiss));

        Assert.Equal(400, exception.StatusCode);
    }

    [Fact]
    public async Task GetGifAsync_WithServerError_ThrowsServerException()
    {
        // Arrange
        var httpClient = CreateMockHttpClient(HttpStatusCode.InternalServerError, "");
        var client = new OtakuGifsClient(httpClient);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<OtakuGifsServerException>(
            () => client.GetGifAsync(OtakuGifReaction.Kiss));

        Assert.Equal(500, exception.StatusCode);
    }

    [Fact]
    public async Task GetAllReactionsAsync_ReturnsReactions()
    {
        // Arrange
        var expectedReactions = new[] { "kiss", "hug", "pat" };
        var response = new { reactions = expectedReactions };
        var httpClient = CreateMockHttpClient(HttpStatusCode.OK, JsonSerializer.Serialize(response));
        var client = new OtakuGifsClient(httpClient);

        // Act
        var result = await client.GetAllReactionsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedReactions.Length, result.Length);
        Assert.Equal(expectedReactions, result);
    }

    [Fact]
    public async Task GetGifAsync_WithCancellationToken_CanBeCancelled()
    {
        // Arrange
        var mockHandler = new Mock<HttpMessageHandler>();
        mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(() =>
            {
                Thread.Sleep(1000); // Simulate slow response
                return new HttpResponseMessage(HttpStatusCode.OK);
            });

        var httpClient = new HttpClient(mockHandler.Object)
        {
            BaseAddress = new Uri("https://api.otakugifs.xyz")
        };
        var client = new OtakuGifsClient(httpClient);
        var cts = new CancellationTokenSource();
        cts.Cancel(); // Cancel immediately

        // Act & Assert
        await Assert.ThrowsAsync<OtakuGifsException>(
            () => client.GetGifAsync(OtakuGifReaction.Kiss, cancellationToken: cts.Token));
    }

    [Fact]
    public async Task Dispose_DisposesHttpClient_WhenConfigured()
    {
        // Arrange
        var httpClient = new HttpClient();
        var client = new OtakuGifsClient(httpClient, disposeHttpClient: true);

        // Act
        client.Dispose();

        // Assert
        await Assert.ThrowsAsync<ObjectDisposedException>(async () => await httpClient.GetAsync("test"));
    }

    [Fact]
    public async Task FluentAPI_WithReactionAndFormat_ReturnsValidResponse()
    {
        // Arrange
        var expectedUrl = "https://cdn.otakugifs.xyz/gifs/kiss/test.webp";
        var response = new OtakuGifsResponse { Url = expectedUrl };
        var httpClient = CreateMockHttpClient(HttpStatusCode.OK, JsonSerializer.Serialize(response));
        var client = new OtakuGifsClient(httpClient);

        // Act
        var result = await client.Request()
            .WithReaction(OtakuGifReaction.Kiss)
            .WithFormat(OtakuGifFormat.WebP)
            .ExecuteAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedUrl, result.Url);
    }

    [Fact]
    public async Task FluentAPI_WithoutReaction_ThrowsInvalidOperationException()
    {
        // Arrange
        var httpClient = CreateMockHttpClient(HttpStatusCode.OK, "{}");
        var client = new OtakuGifsClient(httpClient);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await client.Request().ExecuteAsync());
    }

    [Fact]
    public async Task FluentAPI_WithOnlyReaction_UsesDefaultFormat()
    {
        // Arrange
        var expectedUrl = "https://cdn.otakugifs.xyz/gifs/hug/test.gif";
        var response = new OtakuGifsResponse { Url = expectedUrl };
        var mockHandler = new Mock<HttpMessageHandler>();
        mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.RequestUri!.ToString().Contains("format=gif")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(response))
            });

        var httpClient = new HttpClient(mockHandler.Object)
        {
            BaseAddress = new Uri("https://api.otakugifs.xyz")
        };
        var client = new OtakuGifsClient(httpClient);

        // Act
        var result = await client.Request()
            .WithReaction(OtakuGifReaction.Hug)
            .ExecuteAsync();

        // Assert
        Assert.NotNull(result);
        mockHandler.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.RequestUri!.ToString().Contains("format=gif")),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public void DependencyInjection_AddOtakuGifsClient_RegistersClient()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddOtakuGifsClient();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var client = serviceProvider.GetService<IOtakuGifsClient>();
        Assert.NotNull(client);
        Assert.IsType<OtakuGifsClient>(client);
    }

    [Fact]
    public void DependencyInjection_AddOtakuGifsClient_ConfiguresHttpClient()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddOtakuGifsClient();
        var serviceProvider = services.BuildServiceProvider();
        var client = serviceProvider.GetService<IOtakuGifsClient>();

        // Assert
        Assert.NotNull(client);
        // Client should be usable
        Assert.IsAssignableFrom<IOtakuGifsClient>(client);
    }

    [Fact]
    public void DependencyInjection_CanResolveMultipleTimes()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddOtakuGifsClient();
        var serviceProvider = services.BuildServiceProvider();

        // Act
        var client1 = serviceProvider.GetService<IOtakuGifsClient>();
        var client2 = serviceProvider.GetService<IOtakuGifsClient>();

        // Assert
        Assert.NotNull(client1);
        Assert.NotNull(client2);
        // Typed clients are transient by default, so they should be different instances
        Assert.NotSame(client1, client2);
    }

    [Fact]
    public void Interface_ImplementsAllRequiredMethods()
    {
        // Arrange
        var httpClient = CreateMockHttpClient(HttpStatusCode.OK, "{}");
        IOtakuGifsClient client = new OtakuGifsClient(httpClient);

        // Assert - verify interface methods are available
        Assert.NotNull(client.GetGifAsync);
        Assert.NotNull(client.GetAllReactionsAsync);
        Assert.NotNull(client.Request);
        Assert.IsAssignableFrom<IDisposable>(client);
    }

    private HttpClient CreateMockHttpClient(HttpStatusCode statusCode, string content)
    {
        var mockHandler = new Mock<HttpMessageHandler>();
        mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent(content)
            });

        return new HttpClient(mockHandler.Object)
        {
            BaseAddress = new Uri("https://api.otakugifs.xyz")
        };
    }
}
