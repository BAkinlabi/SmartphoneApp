
namespace ProductServiceTests
{
    public class ProductServiceTests
    {
        private ProductService CreateService(HttpResponseMessage response)
        {
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

            var loggerMock = new Mock<ILogger<ProductService>>();

            var configurationMock = new Mock<IConfiguration>();
            configurationMock.SetupGet(m => m["ApiSettings:BaseUrl"]).Returns("https://dummyjson.com");

            var configMock = new Mock<IConfiguration>();
            configMock.Setup(c => c["ApiSettings:BaseUrl"]).Returns("https://dummyjson.com");

            return new ProductService(factoryMock.Object, loggerMock.Object, configurationMock.Object);
        }

        [Fact]
        public async Task GetTopThreeExpensiveSmartphonesAsync_ReturnsProducts_OnSuccess()
        {
            // Arrange
            var products = new[]
            {
                new { id = 1, title = "A", brand = "B", price = 1000, description = "desc1" },
                new { id = 2, title = "C", brand = "D", price = 900, description = "desc2" },
                new { id = 3, title = "D", brand = "F", price = 800, description = "desc3" }
            };
            var responseObj = new { products };
            var json = JsonSerializer.Serialize(responseObj);

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json)
            };

            var service = CreateService(response);

            // Act
            var result = await service.GetTopThreeExpensiveSmartphonesAsync("token");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Equal("A", result[0].Title);
            Assert.Equal(1000, result[0].Price);
        }

        [Fact]
        public async Task GetTopThreeExpensiveSmartphonesAsync_ReturnsEmpty_OnFailure()
        {
            // Arrange
            var response = new HttpResponseMessage(HttpStatusCode.BadRequest);
            var service = CreateService(response);

            // Act
            var result = await service.GetTopThreeExpensiveSmartphonesAsync("token");

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task UpdateProductPriceAsync_ReturnsProductDTO_OnSuccess()
        {
            // Arrange
            var product = new { id = 1, title = "A", brand = "B", price = 1200, description = "desc" };
            var json = JsonSerializer.Serialize(product);

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json)
            };

            var service = CreateService(response);

            // Act
            var result = await service.UpdateProductPriceAsync("token", 1, 1200);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("A", result.Title);
            Assert.Equal(1200, result.Price);
        }

        [Fact]
        public async Task UpdateProductPriceAsync_ReturnsNull_OnFailure()
        {
            // Arrange
            var response = new HttpResponseMessage(HttpStatusCode.BadRequest);
            var service = CreateService(response);

            // Act
            var result = await service.UpdateProductPriceAsync("token", 1, 1200);

            // Assert
            Assert.Null(result);
        }
    }
}