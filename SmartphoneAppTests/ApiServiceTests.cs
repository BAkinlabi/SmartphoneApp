using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using SmartphoneApp.Services;
using Microsoft.Extensions.Logging;
using Xunit;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Hosting;
using SmartphoneApp.Config;

public class ApiServiceTests
{
    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsToken()
    {
        // Arrange
        var expectedToken = "test-token";
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent($"{{\"accessToken\":\"{expectedToken}\"}}")
        };

        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri!.ToString().Contains("/auth/login")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        var httpClient = new HttpClient(handlerMock.Object);

        var factoryMock = new Mock<IHttpClientFactory>();
        factoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);

        var loggerMock = new Mock<ILogger<ApiService>>();
        var optionsMock = new Mock<IOptions<ApiSettings>>();
        optionsMock.Setup(o => o.Value).Returns(new ApiSettings { BaseUrl = "https://dummyjson.com" });

        var apiService = new ApiService(factoryMock.Object, loggerMock.Object, optionsMock.Object);

        // Act
        var result = await apiService.LoginAsync("user", "pass");

        // Assert
        Assert.True(result.Success);
        Assert.Equal(expectedToken, result.Token);
        Assert.Equal("", result.ErrorMessage);
    }

    [Fact]
    public async Task LoginAsync_InvalidCredentials_ReturnsFalse()
    {
        var factory = new Mock<IHttpClientFactory>();
        var logger = new Mock<ILogger<ApiService>>();
        var options = new Mock<IOptions<ApiSettings>>();
        var api = new ApiService(factory.Object, logger.Object, options.Object);

        var result = await api.LoginAsync("invalid", "invalid");
        Assert.False(result.Success);
    }

     [Fact]
    public async Task GetSmartphonesAsync_ReturnsMockedProducts()
    {
        // Arrange: Mock HTTP response
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{\"products\":[{\"id\":1,\"brand\":\"BrandA\",\"title\":\"PhoneA\",\"price\":999}]}")
        };

        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        var httpClient = new HttpClient(handlerMock.Object);

        var factoryMock = new Mock<IHttpClientFactory>();
        factoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);

        var loggerMock = new Mock<ILogger<ApiService>>();
        var options = new Mock<IOptions<ApiSettings>>();
        var apiService = new ApiService(factoryMock.Object, loggerMock.Object, options.Object);

        // Act
        var products = await apiService.GetSmartphonesAsync("dummy-token");

        // Assert
        Assert.Single(products);
        Assert.Equal("BrandA", products[0].Brand);
    }

    [Fact]
    public async Task GetSmartphonesAsync_ApiReturnsError_ReturnsEmptyList()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent("")
        };

        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri!.ToString().Contains("/auth/products/category/smartphones")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        var httpClient = new HttpClient(handlerMock.Object);

        var factoryMock = new Mock<IHttpClientFactory>();
        factoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);

        var loggerMock = new Mock<ILogger<ApiService>>();
        var optionsMock = new Mock<IOptions<ApiSettings>>();
        optionsMock.Setup(o => o.Value).Returns(new ApiSettings { BaseUrl = "https://dummyjson.com" });

        var apiService = new ApiService(factoryMock.Object, loggerMock.Object, optionsMock.Object);

        // Act
        var products = await apiService.GetSmartphonesAsync("token");

        // Assert
        Assert.Empty(products);
    }

    [Fact]
    public async Task UpdateProductPriceAsync_Success_ReturnsUpdatedProduct()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{\"id\":1,\"title\":\"PhoneA\",\"brand\":\"BrandA\",\"price\":1200,\"description\":\"desc\"}")
        };

        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Put),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        var httpClient = new HttpClient(handlerMock.Object);

        var factoryMock = new Mock<IHttpClientFactory>();
        factoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);

        var loggerMock = new Mock<ILogger<ApiService>>();
        var optionsMock = new Mock<IOptions<ApiSettings>>();
        optionsMock.Setup(o => o.Value).Returns(new ApiSettings { BaseUrl = "https://dummyjson.com" });

        var apiService = new ApiService(factoryMock.Object, loggerMock.Object, optionsMock.Object);

        // Act
        var updated = await apiService.UpdateProductPriceAsync("token", 1, 1200);

        // Assert
        Assert.NotNull(updated);
        Assert.Equal(1, updated.Id);
        Assert.Equal("PhoneA", updated.Title);
        Assert.Equal("BrandA", updated.Brand);
        Assert.Equal(1200, updated.Price);
        Assert.Equal("desc", updated.Description);
    }

    [Fact]
    public async Task UpdateProductPriceAsync_ApiReturnsError_ReturnsNull()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent("")
        };

        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Put),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        var httpClient = new HttpClient(handlerMock.Object);

        var factoryMock = new Mock<IHttpClientFactory>();
        factoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);

        var loggerMock = new Mock<ILogger<ApiService>>();
        var optionsMock = new Mock<IOptions<ApiSettings>>();
        optionsMock.Setup(o => o.Value).Returns(new ApiSettings { BaseUrl = "https://dummyjson.com" });

        var apiService = new ApiService(factoryMock.Object, loggerMock.Object, optionsMock.Object);

        // Act
        var updated = await apiService.UpdateProductPriceAsync("token", 1, 1200);

        // Assert
        Assert.Null(updated);
    }
}
