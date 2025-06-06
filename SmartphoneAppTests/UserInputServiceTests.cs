using Xunit;
using SmartphoneApp.Services;

public class UserInputServiceTests
{
    [Fact]
    public void GetCredentials_ReturnsCorrectTuple()
    {
        // Arrange
        var input = "testuser\nsecretpass\n";
        Console.SetIn(new StringReader(input));
        var service = new UserInputService();

        // Act
        var (username, password) = service.GetCredentials();

        // Assert
        Assert.Equal("testuser", username);
        Assert.Equal("secretpass", password);
    }

    [Fact]
    public void GetPercentage_ValidInput_ReturnsParsedInt()
    {
        // Arrange
        var input = "15\n";
        Console.SetIn(new StringReader(input));
        var service = new UserInputService();

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
        var service = new UserInputService();

        // Act
        var percent = service.GetPercentage();

        // Assert
        Assert.Equal(25, percent);
    }
}
