namespace UserInputServiceTests
{
    public class UserInputServiceTests
    {
        [Fact]
        public void GetCredentials_ReturnsCorrectTuple()
        {
            // Arrange
            var input = "user\nuserpass\n";
            Console.SetIn(new StringReader(input));


            // Arrange
            var logger = new LoggerFactory().CreateLogger<UserInputService>();
            var service = new UserInputService(logger);

            // Act
            var (username, password) = service.GetCredentials();

            // Assert
            Assert.Equal("user", username);
            Assert.Equal("userpass", password);
        }

        [Fact]
        public void GetPercentage_ValidInput_ReturnsParsedInt()
        {
            // Arrange
            var input = "15\n";
            Console.SetIn(new StringReader(input));
            var logger = new LoggerFactory().CreateLogger<UserInputService>();
            var service = new UserInputService(logger);

            // Act
            var percent = service.GetPercentage();

            // Assert
            Assert.Equal(15, percent);
        }

        [Fact]
        public void GetPercentage_InvalidThenValidInput_ReturnsValidInt()
        {
            // Arrange: first input is invalid, second is valid
            var input = "abc\n0\n25\n";
            Console.SetIn(new StringReader(input));
            var logger = new LoggerFactory().CreateLogger<UserInputService>();
            var service = new UserInputService(logger);

            // Act
            var percent = service.GetPercentage();

            // Assert
            Assert.Equal(25, percent);
        }
    }
}