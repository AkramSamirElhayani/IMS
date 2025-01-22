using Xunit;


namespace IMS.UITests;

public class MainWindowTests
{
    [Fact]
    public void MainWindow_WhenInitialized_ShouldLoadSuccessfully()
    {
        // This is a sample test - implement actual UI testing logic here
        var app = new App();
        Assert.NotNull(app);
    }
}
