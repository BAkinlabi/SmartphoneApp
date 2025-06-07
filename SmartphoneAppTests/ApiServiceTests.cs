
namespace ApiServiceTests
{
    public class ApiServiceTests
{
    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsToken()
    {
        // Arrange
        // The test defines an expected token (expectedToken) that the service should return upon successful login.
        var expectedToken = "test-token";
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent($"{{\"accessToken\":\"{expectedToken}\"}}")
        };

        // A mock HttpResponseMessage is created to simulate a successful response from the API.
        // It has a status code of OK (200) and contains a JSON string with the access token.
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri!.ToString().Contains("/auth/login")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        // An HttpClient is instantiated using the mocked handler.
        // This client will be used by the ApiService to make HTTP requests.
        var httpClient = new HttpClient(handlerMock.Object);

        var factoryMock = new Mock<IHttpClientFactory>();
        factoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);

        // Mocks for ILogger<ApiService> and IConfiguration are created.
        // The configuration mock is set up to return a base URL when accessed, simulating the application's configuration settings.
        var loggerMock = new Mock<ILogger<ApiService>>();
        
        var configurationMock = new Mock<IConfiguration>();
        configurationMock.SetupGet(m => m["ApiSettings:BaseUrl"]).Returns("https://dummyjson.com");

        // An instance of ApiService is created using the mocked dependencies (HTTP client factory, logger, and configuration).
        var apiService = new ApiService(factoryMock.Object, loggerMock.Object, configurationMock.Object);

        // Act
        // The LoginAsync method is called with valid credentials ("user", "pass"). This simulates a login attempt.
        var result = await apiService.LoginAsync("user", "pass");

        // Assert
        #region notes
        // The test checks three conditions:
        // result.Success should be true, indicating the login was successful.
        // result.Token should match the expectedToken, confirming the correct token was returned.
        // result.ErrorMessage should be an empty string, indicating no error occurred.
        #endregion
        Assert.True(result.Success);
        Assert.Equal(expectedToken, result.Token);
        Assert.Equal("", result.ErrorMessage);
    }

    [Fact]
    public async Task LoginAsync_InvalidCredentials_ReturnsFalse()
    {
        // Arrange
        var expectedResponse = new HttpResponseMessage(HttpStatusCode.Unauthorized)
        {
            Content = new StringContent("{\"error\":\"Invalid credentials\"}")
        };

        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri!.ToString().Contains("/auth/login")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(expectedResponse);

        var httpClient = new HttpClient(handlerMock.Object);

        var factoryMock = new Mock<IHttpClientFactory>();
        factoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);

        var loggerMock = new Mock<ILogger<ApiService>>();
        
        var configurationMock = new Mock<IConfiguration>();
        configurationMock.SetupGet(m => m["ApiSettings:BaseUrl"]).Returns("https://dummyjson.com");

        var apiService = new ApiService(factoryMock.Object, loggerMock.Object, configurationMock.Object);

        // Act
        var result = await apiService.LoginAsync("invalid", "invalid");

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Invalid credentials", result.ErrorMessage);
    }
}
}